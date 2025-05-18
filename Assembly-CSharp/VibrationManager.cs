using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InControl;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
	public void Init()
	{
		this.currentOverridingVibration = new VibrationInfo();
		this.currentOverridingVibration.vibrationParamsList = new List<VibrationParams>();
	}

	public async Task Vibrate(VibrationData vibrationData, VibrationRegion vibrationRegion, bool overrideExistingVibrations = false)
	{
		if (vibrationData == null)
		{
			CustomDebug.EditorLogError("[VibrationManager] Vibrate() VibrationData missing!");
		}
		else if (!this.IsPlayingVibration() || overrideExistingVibrations)
		{
			float startTime = Time.time;
			if (overrideExistingVibrations)
			{
				this.currentOverridingVibration.vibrationParamsList = vibrationData.vibrationParamsList;
				this.currentOverridingVibration.time = startTime;
			}
			this.playingVibration = true;
			foreach (VibrationParams vibrationParams in vibrationData.vibrationParamsList)
			{
				TaskAwaiter<bool> taskAwaiter = this.DoVibrate(vibrationData, vibrationParams, startTime).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					return;
				}
			}
			List<VibrationParams>.Enumerator enumerator = default(List<VibrationParams>.Enumerator);
			this.playingVibration = false;
			this.currentOverridingVibration.time = -1f;
		}
	}

	public async Task<bool> DoVibrate(VibrationData vibrationData, VibrationParams vibrationParams, float startTime)
	{
		float largeMotorIntensity = vibrationParams.LargeMotorIntensity;
		float smallMotorIntensity = vibrationParams.SmallMotorIntensity;
		float time = vibrationParams.Time;
		float postVibrateDelayTime = vibrationParams.PostVibrateDelay;
		float timer = 0f;
		InputManager.ActiveDevice.Vibrate(largeMotorIntensity, smallMotorIntensity);
		while (timer < time)
		{
			if ((this.currentOverridingVibration.vibrationParamsList.Count <= 0 || this.currentOverridingVibration.vibrationParamsList != vibrationData.vibrationParamsList) && this.currentOverridingVibration.time > -1f && this.currentOverridingVibration.time != startTime)
			{
				return true;
			}
			timer += Time.unscaledDeltaTime;
			await Awaiters.NextFrame;
		}
		InputManager.ActiveDevice.StopVibration();
		timer = 0f;
		while (timer < postVibrateDelayTime)
		{
			if ((this.currentOverridingVibration.vibrationParamsList.Count <= 0 || this.currentOverridingVibration.vibrationParamsList != vibrationData.vibrationParamsList) && this.currentOverridingVibration.time > -1f && this.currentOverridingVibration.time != startTime)
			{
				return true;
			}
			timer += Time.unscaledDeltaTime;
			await Awaiters.NextFrame;
		}
		return false;
	}

	public bool IsPlayingVibration()
	{
		return this.playingVibration;
	}

	private void OnDestroy()
	{
	}

	public bool playingVibration;

	private VibrationInfo currentOverridingVibration;
}
