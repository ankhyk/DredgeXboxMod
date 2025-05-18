using System;
using Cinemachine;
using UnityEngine;

public class CameraRecenterSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_RECENTER)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance == null || GameManager.Instance.SettingsSaveData == null)
		{
			return;
		}
		if (this.freelookCamera != null)
		{
			this.freelookCamera.m_RecenterToTargetHeading.m_enabled = GameManager.Instance.SettingsSaveData.cameraRecenter == 1;
		}
	}

	[SerializeField]
	private CinemachineFreeLook freelookCamera;
}
