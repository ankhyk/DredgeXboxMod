using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TooltipSectionGadgetDetails : TooltipSection<GadgetItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.Init<T>(itemData, null, tooltipMode);
	}

	public override void Init<T>(T itemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		bool flag = false;
		if (spatialItemInstance != null)
		{
			flag = spatialItemInstance.GetIsOnDamagedCell();
		}
		this.localizedEffectNameField.StringReference = this.gadgetEffectNames[itemData.EffectType];
		this.localizedEffectNameField.RefreshString();
		decimal num = (flag ? 0m : (itemData.EffectMagnitude * 100m));
		this.thisMagnitudeTextField.text = string.Format("+{0}%", decimal.Truncate(num));
		this.thisMagnitudeTextField.color = GameManager.Instance.LanguageManager.GetColor(flag ? DredgeColorTypeEnum.NEGATIVE : DredgeColorTypeEnum.NEUTRAL);
		decimal num2 = (GameManager.Instance.PlayerStats.GetGadgetModifierForType(itemData.EffectType) - 1m) * 100m;
		this.totalMagnitudeTextField.text = string.Format("+{0}%", decimal.Truncate(num2));
		this.totalMagnitudeSection.gameObject.SetActive(num2 > num);
		this.isLayedOut = true;
	}

	[SerializeField]
	private LocalizeStringEvent localizedEffectNameField;

	[SerializeField]
	private TextMeshProUGUI thisMagnitudeTextField;

	[SerializeField]
	private TextMeshProUGUI totalMagnitudeTextField;

	[SerializeField]
	private GameObject totalMagnitudeSection;

	[SerializeField]
	private Dictionary<GadgetEffect, LocalizedString> gadgetEffectNames = new Dictionary<GadgetEffect, LocalizedString>();
}
