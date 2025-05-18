using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerOozeDetector : MonoBehaviour
{
	private void Update()
	{
		OozePatchManager opm = GameManager.Instance.OozePatchManager;
		if (Time.time > this.timeOfLastDetection + this.detectionRepeatDelaySec)
		{
			Transform transform = this.nearDetectionPositions.Find((Transform p) => opm.IsPositionInOoze(p.position));
			if (transform)
			{
				this.timeOfLastOozeNear = Time.time;
				this.SetIsOozeNear(true, transform);
			}
			else if (this.isOozeNear && !transform && Time.time > this.timeOfLastOozeNear + this.oozeNearLostDelaySec)
			{
				this.SetIsOozeNear(false, null);
			}
			bool flag = this.farDetectionPositions.Any((Transform p) => opm.IsPositionInOoze(p.position));
			if (flag)
			{
				this.timeOfLastOozeFar = Time.time;
				this.SetIsOozeFar(true);
			}
			else if (this.isOozeFar && !flag && Time.time > this.timeOfLastOozeFar + this.oozeFarLostDelaySec)
			{
				this.SetIsOozeFar(false);
			}
		}
		this.oozeOtherworldAmbience.volume = Mathf.Lerp(this.oozeOtherworldAmbience.volume, (float)(this.isOozeFar ? 1 : 0), Time.deltaTime * this.oozeAmbienceFadeSpeedModifier);
	}

	private void SetIsOozeNear(bool value, Transform partOfBoatInOoze)
	{
		if (this.isOozeNear != value)
		{
			this.isOozeNear = value;
			GameManager.Instance.OozePatchManager.isOozeNearToPlayer = this.isOozeNear;
			if (value)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(this.oozeSplashVFX, partOfBoatInOoze.position, Quaternion.identity);
				GameManager.Instance.PlayerStats.OozeMovementSpeedModifier = GameManager.Instance.GameConfigData.MaxMovementPropInOoze;
				GameEvents.Instance.TriggerPlayerEnteredOoze();
				GameManager.Instance.VibrationManager.Vibrate(this.oozeEntryVibrationData, VibrationRegion.WholeBody, true);
				this.oozeEntrySplashAudioSource.clip = this.oozeEntrySplashSFX.PickRandom<AudioClip>();
				this.oozeEntrySplashAudioSource.Play();
				return;
			}
			GameManager.Instance.PlayerStats.OozeMovementSpeedModifier = 1f;
		}
	}

	private void SetIsOozeFar(bool value)
	{
		if (this.isOozeFar != value)
		{
			this.isOozeFar = value;
			GameManager.Instance.OozePatchManager.isOozeFarToPlayer = this.isOozeFar;
		}
	}

	[SerializeField]
	private float detectionRepeatDelaySec;

	[SerializeField]
	private float oozeNearLostDelaySec;

	[SerializeField]
	private float oozeFarLostDelaySec;

	[SerializeField]
	private List<Transform> nearDetectionPositions;

	[SerializeField]
	private List<Transform> farDetectionPositions;

	[SerializeField]
	private GameObject oozeSplashVFX;

	[SerializeField]
	private AudioSource oozeEntrySplashAudioSource;

	[SerializeField]
	private VibrationData oozeEntryVibrationData;

	[SerializeField]
	private List<AudioClip> oozeEntrySplashSFX;

	[SerializeField]
	private AudioSource oozeOtherworldAmbience;

	[SerializeField]
	private float oozeAmbienceFadeSpeedModifier;

	[HideInInspector]
	public bool isOozeNear;

	[HideInInspector]
	public bool isOozeFar;

	private float timeOfLastDetection;

	private float timeOfLastOozeNear;

	private float timeOfLastOozeFar;
}
