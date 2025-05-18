using System;
using Cinemachine;
using UnityEngine;

public class SpyglassCameraSensitivitySettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.SPYGLASS_CAMERA_SENSITIVITY_X || settingType == SettingType.SPYGLASS_CAMERA_SENSITIVITY_Y)
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
		if (this.virtualCamera != null)
		{
			CinemachinePOV cinemachineComponent = this.virtualCamera.GetCinemachineComponent<CinemachinePOV>();
			cinemachineComponent.m_HorizontalAxis.m_MaxSpeed = this.baseSensitivityX * GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityX;
			cinemachineComponent.m_VerticalAxis.m_MaxSpeed = this.baseSensitivityY * GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityY;
		}
	}

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;

	[SerializeField]
	private float baseSensitivityX;

	[SerializeField]
	private float baseSensitivityY;
}
