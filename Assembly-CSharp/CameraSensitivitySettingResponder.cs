using System;
using Cinemachine;
using UnityEngine;

public class CameraSensitivitySettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_SENSITIVITY_X || settingType == SettingType.CAMERA_SENSITIVITY_Y)
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
			this.freelookCamera.m_XAxis.m_MaxSpeed = this.baseSensitivityX * GameManager.Instance.SettingsSaveData.cameraSensitivityX;
			this.freelookCamera.m_YAxis.m_MaxSpeed = this.baseSensitivityY * GameManager.Instance.SettingsSaveData.cameraSensitivityY;
		}
		if (this.virtualCamera != null)
		{
			CinemachinePOV cinemachineComponent = this.virtualCamera.GetCinemachineComponent<CinemachinePOV>();
			cinemachineComponent.m_HorizontalAxis.m_MaxSpeed = this.baseSensitivityX * GameManager.Instance.SettingsSaveData.cameraSensitivityX;
			cinemachineComponent.m_VerticalAxis.m_MaxSpeed = this.baseSensitivityY * GameManager.Instance.SettingsSaveData.cameraSensitivityY;
		}
	}

	[SerializeField]
	private CinemachineFreeLook freelookCamera;

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;

	[SerializeField]
	private float baseSensitivityX;

	[SerializeField]
	private float baseSensitivityY;
}
