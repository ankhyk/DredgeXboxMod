using System;
using Cinemachine;
using UnityEngine;

public class CameraInvertSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_INVERT_X || settingType == SettingType.CAMERA_INVERT_Y)
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
		bool flag = GameManager.Instance.SettingsSaveData.cameraInvertX == 1;
		bool flag2 = GameManager.Instance.SettingsSaveData.cameraInvertY == 1;
		if (this.freelookCamera != null)
		{
			this.freelookCamera.m_XAxis.m_InvertInput = flag;
			this.freelookCamera.m_YAxis.m_InvertInput = flag2;
		}
		if (this.virtualCamera != null)
		{
			CinemachinePOV cinemachineComponent = this.virtualCamera.GetCinemachineComponent<CinemachinePOV>();
			cinemachineComponent.m_HorizontalAxis.m_InvertInput = flag;
			cinemachineComponent.m_VerticalAxis.m_InvertInput = flag2;
		}
	}

	[SerializeField]
	private CinemachineFreeLook freelookCamera;

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
}
