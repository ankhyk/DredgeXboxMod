using System;
using UnityEngine;

public class ReflectionSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.REFLECTIONS)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null && GameManager.Instance.SettingsSaveData.reflections == 1)
		{
			this.reflectionsObject.enabled = true;
			this.waterMat.EnableKeyword("_REFLECTIONS");
			return;
		}
		this.reflectionsObject.enabled = false;
		this.waterMat.DisableKeyword("_REFLECTIONS");
	}

	[SerializeField]
	private PlanarReflections reflectionsObject;

	[SerializeField]
	private Material waterMat;
}
