using System;
using Cinemachine;
using UnityEngine;

public class CameraShakeSettingResponder : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.UpdateCameraShake();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_SHAKE_AMOUNT)
		{
			this.UpdateCameraShake();
		}
	}

	private void UpdateCameraShake()
	{
		this.impulseListener.m_Gain = GameManager.Instance.GameConfigData.CameraShakeScaleFactor * GameManager.Instance.SettingsSaveData.cameraShakeAmount;
	}

	[SerializeField]
	private CinemachineImpulseListener impulseListener;
}
