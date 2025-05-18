using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public bool IsRecentering
	{
		get
		{
			return this.isRecentering;
		}
		set
		{
			this.isRecentering = value;
		}
	}

	public CinemachineFreeLook CinemachineCamera
	{
		get
		{
			return this.cinemachineCamera;
		}
	}

	private void Start()
	{
		this.recenterCameraAction = new DredgePlayerActionPress("settings.binding.camera-recenter", GameManager.Instance.Input.Controls.CameraRecenter);
		DredgePlayerActionPress dredgePlayerActionPress = this.recenterCameraAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.RecenterCamera));
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.recenterCameraAction };
		input.AddActionListener(array, ActionLayer.BASE);
		DredgePlayerActionPress dredgePlayerActionPress2 = new DredgePlayerActionPress("settings.binding.move-camera", GameManager.Instance.Input.Controls.CameraMoveButton);
		DredgePlayerActionPress dredgePlayerActionPress3 = dredgePlayerActionPress2;
		dredgePlayerActionPress3.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressBegin, new Action(this.OnCameraMovePressBegin));
		DredgePlayerActionPress dredgePlayerActionPress4 = dredgePlayerActionPress2;
		dredgePlayerActionPress4.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionPress4.OnPressEnd, new Action(this.OnCameraMovePressEnd));
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { dredgePlayerActionPress2 };
		input2.AddActionListener(array, ActionLayer.BASE);
		this.cinemachineCamera.m_RecenterToTargetHeading.m_WaitTime = this.passiveRecenteringWaitTime;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_RecenteringTime = this.passiveRecenteringDuration;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		GameManager.Instance.PlayerCamera = this;
		this.UpdateMotionSicknessSmoothing();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
		GameManager.Instance.PlayerCamera = null;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.MOTION_SICKNESS_AMOUNT)
		{
			this.UpdateMotionSicknessSmoothing();
		}
	}

	private void UpdateMotionSicknessSmoothing()
	{
		float num = this.motionSmoothingScaleFactor * Mathf.Clamp01(GameManager.Instance.SettingsSaveData.motionSicknessAmount);
		for (int i = 0; i <= 2; i++)
		{
			this.cinemachineCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = num;
		}
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.hasteAbility.name)
		{
			if (enabled && !this.useHasteVFX)
			{
				return;
			}
			if (this.hasteTween != null)
			{
				this.hasteTween.Kill(false);
				this.hasteTween = null;
			}
			float num;
			if (enabled)
			{
				num = 1f;
			}
			else
			{
				num = 0f;
			}
			this.hasteTween = DOTween.To(() => this.tweenProp, delegate(float x)
			{
				this.tweenProp = x;
			}, num, this.hasteCamLerpDuration);
			this.hasteTween.onUpdate = new TweenCallback(this.OnHasteTweenUpdate);
			this.hasteTween.onComplete = new TweenCallback(this.OnHasteTweenComplete);
		}
	}

	private void OnHasteTweenUpdate()
	{
		this.cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(this.defaultFOV, this.hasteFOV, this.tweenProp);
	}

	private void OnHasteTweenComplete()
	{
		if (this.hasteTween != null)
		{
			this.hasteTween.Kill(false);
			this.hasteTween = null;
		}
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (!(dock == null))
		{
			this.previousDock = dock;
			return;
		}
		this.ToggleDockingCameraMode(false);
		if (this.previousDock)
		{
			if (this.previousDock.Data.HasCameraOverride)
			{
				this.cinemachineCamera.m_XAxis.Value = this.previousDock.Data.CameraOverrideX;
				this.cinemachineCamera.m_YAxis.Value = this.previousDock.Data.CameraOverrideY;
			}
			this.previousDock = null;
			return;
		}
		this.cinemachineCamera.m_XAxis.Value = 0.5f;
	}

	public void ToggleDockingCameraMode(bool isActivelyDocking)
	{
		this.cinemachineCamera.Follow = (isActivelyDocking ? null : GameManager.Instance.Player.transform);
	}

	private void OnCameraMovePressBegin()
	{
		if (!this.isRecentering)
		{
			this.cinemachineCamera.m_RecenterToTargetHeading.m_enabled = false;
		}
	}

	private void OnCameraMovePressEnd()
	{
		if (!this.isRecentering)
		{
			this.cinemachineCamera.m_RecenterToTargetHeading.m_enabled = GameManager.Instance.SettingsSaveData.cameraRecenter == 1;
		}
	}

	private void RecenterCamera()
	{
		this.isRecentering = true;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_RecenteringTime = this.forceRecenteringDuration;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_WaitTime = 0f;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_enabled = true;
		this.timeOfRecenterPress = Time.time;
	}

	private void Update()
	{
		if (this.isRecentering)
		{
			bool flag = false;
			if (this.cinemachineCamera.m_BindingMode == CinemachineTransposer.BindingMode.LockToTargetWithWorldUp)
			{
				if (Mathf.Abs(this.cinemachineCamera.m_XAxis.Value) < this.forceRecenteringCompleteThreshold)
				{
					flag = true;
				}
			}
			else if (this.cinemachineCamera.m_BindingMode == CinemachineTransposer.BindingMode.WorldSpace && Time.time > this.timeOfRecenterPress + this.worldSpaceRecenteringDuration)
			{
				flag = true;
			}
			if (flag)
			{
				this.MarkRecenterAsComplete();
			}
		}
	}

	private void MarkRecenterAsComplete()
	{
		this.isRecentering = false;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_WaitTime = this.passiveRecenteringWaitTime;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_RecenteringTime = this.passiveRecenteringDuration;
		this.cinemachineCamera.m_RecenterToTargetHeading.m_enabled = GameManager.Instance.SettingsSaveData.cameraRecenter == 1;
	}

	[SerializeField]
	private CinemachineFreeLook cinemachineCamera;

	[SerializeField]
	private float passiveRecenteringDuration;

	[SerializeField]
	private float passiveRecenteringWaitTime;

	[SerializeField]
	private float forceRecenteringDuration;

	[SerializeField]
	private float worldSpaceRecenteringDuration;

	[SerializeField]
	private float forceRecenteringCompleteThreshold;

	[SerializeField]
	private float motionSmoothingScaleFactor;

	[SerializeField]
	private float defaultFOV;

	[SerializeField]
	private AbilityData hasteAbility;

	[SerializeField]
	private float hasteCamLerpDuration;

	[SerializeField]
	private float hasteFOV;

	private DredgePlayerActionPress recenterCameraAction;

	private Tweener hasteTween;

	private float tweenProp;

	private bool isRecentering;

	private Dock previousDock;

	public bool useHasteVFX = true;

	private float timeOfRecenterPress;
}
