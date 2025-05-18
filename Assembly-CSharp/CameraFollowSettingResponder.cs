using System;
using Cinemachine;
using UnityEngine;

public class CameraFollowSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_FOLLOW_MODE)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null && GameManager.Instance.SettingsSaveData.cameraFollow == 1)
		{
			this.playerCamera.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
			return;
		}
		this.playerCamera.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
	}

	[SerializeField]
	private CinemachineFreeLook playerCamera;
}
