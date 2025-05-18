using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TooltipSectionDemoNote : TooltipSection<global::ItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.descriptionTextField.gameObject.SetActive(false);
		this.isLayedOut = true;
	}

	public override void Init(UpgradeData upgradeData)
	{
		this.isLayedOut = false;
		this.descriptionTextField.gameObject.SetActive(false);
		this.isLayedOut = true;
	}

	private void ShowNotAvailableInDemoNote()
	{
		this.localizedDescriptionField.StringReference = this.notAvailableInDemoKey;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.descriptionTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		this.descriptionTextField.gameObject.SetActive(true);
	}

	[SerializeField]
	private TextMeshProUGUI descriptionTextField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	[SerializeField]
	private LocalizedString notAvailableInDemoKey;
}
