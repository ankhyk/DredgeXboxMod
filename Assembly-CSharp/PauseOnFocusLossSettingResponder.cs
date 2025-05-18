using System;
using UnityEngine;

public class PauseOnFocusLossSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.PAUSE_ON_FOCUS_LOSS)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null && GameManager.Instance.SettingsSaveData.pauseOnFocusLoss == 1)
		{
			Application.runInBackground = false;
			return;
		}
		Application.runInBackground = true;
	}
}
