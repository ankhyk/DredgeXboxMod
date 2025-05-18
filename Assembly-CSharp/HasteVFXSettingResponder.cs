using System;
using UnityEngine;

public class HasteVFXSettingResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.HASTE_VFX)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null && GameManager.Instance.SettingsSaveData.hasteVFX == 1)
		{
			this.playerCamera.useHasteVFX = false;
			this.boostAbility.useHasteVFX = false;
			this.hasteVolume.useHasteVFX = false;
			return;
		}
		this.playerCamera.useHasteVFX = true;
		this.boostAbility.useHasteVFX = true;
		this.hasteVolume.useHasteVFX = true;
	}

	[SerializeField]
	private PlayerCamera playerCamera;

	[SerializeField]
	private BoostAbility boostAbility;

	[SerializeField]
	private HasteVolume hasteVolume;
}
