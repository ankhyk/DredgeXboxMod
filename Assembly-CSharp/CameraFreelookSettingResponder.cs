using System;
using UnityEngine;

public class CameraFreelookSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_FREELOOK)
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
		if (this.inputProvider != null)
		{
			this.inputProvider.Freelook = GameManager.Instance.SettingsSaveData.cameraFreelook == 1;
		}
	}

	[SerializeField]
	private CinemachineFreeLookInputProvider inputProvider;
}
