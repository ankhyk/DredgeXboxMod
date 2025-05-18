using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntiAliasingSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.ANTI_ALIASING)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null && GameManager.Instance.SettingsSaveData.antiAliasing == 1)
		{
			this.cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			this.cameraData.antialiasingQuality = AntialiasingQuality.Low;
			return;
		}
		this.cameraData.antialiasing = AntialiasingMode.None;
	}

	[SerializeField]
	private UniversalAdditionalCameraData cameraData;
}
