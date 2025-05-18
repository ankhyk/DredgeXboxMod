using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TooltipSectionHeader : TooltipSection<global::ItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.itemData = itemData;
		this.RefreshString();
		if (this.iconImage)
		{
			if (itemData.itemTypeIcon == null)
			{
				this.iconImage.enabled = false;
			}
			else
			{
				this.iconImage.sprite = itemData.itemTypeIcon;
				this.iconImage.enabled = true;
			}
		}
		this.isLayedOut = true;
	}

	public void SetObscured(bool obscured)
	{
		this.obscured = obscured;
		this.RefreshString();
	}

	private void RefreshString()
	{
		this.headerLocalizedString.enabled = false;
		if (this.obscured)
		{
			this.headerLocalizedString.StringReference = this.obscuredHeaderKey;
		}
		else if (!this.itemData.itemInsaneTitleKey.IsEmpty && GameManager.Instance.Player.IsBelowInsaneTooltipThreshold())
		{
			this.headerLocalizedString.StringReference = this.itemData.itemInsaneTitleKey;
		}
		else
		{
			this.headerLocalizedString.StringReference = this.itemData.itemNameKey;
		}
		this.headerLocalizedString.StringReference.RefreshString();
		this.headerLocalizedString.enabled = true;
	}

	public override void Init(TextTooltipRequester textTooltipRequester)
	{
		this.isLayedOut = false;
		this.headerLocalizedString.StringReference = textTooltipRequester.LocalizedTitleKey;
		this.headerLocalizedString.StringReference.RefreshString();
		this.headerTextField.color = textTooltipRequester.TitleTextColor;
		this.isLayedOut = true;
	}

	public override void Init(AbilityTooltipRequester abilityTooltipRequester)
	{
		this.isLayedOut = false;
		this.headerLocalizedString.StringReference = abilityTooltipRequester.abilityData.nameKey;
		this.headerLocalizedString.StringReference.RefreshString();
		this.headerTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.isLayedOut = true;
	}

	public override void Init(UpgradeData upgradeData)
	{
		this.isLayedOut = false;
		object[] array = null;
		if (upgradeData is SlotUpgradeData)
		{
			array = new object[] { upgradeData.GetNewCellCount() };
		}
		else if (upgradeData is HullUpgradeData)
		{
			array = new object[] { upgradeData.tier };
		}
		this.headerLocalizedString.enabled = false;
		this.headerLocalizedString.StringReference = upgradeData.TitleKey;
		this.headerLocalizedString.StringReference.Arguments = array;
		this.headerLocalizedString.StringReference.RefreshString();
		this.headerLocalizedString.enabled = true;
		this.iconImage.sprite = upgradeData.sprite;
		this.iconImage.enabled = true;
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI headerTextField;

	[SerializeField]
	private LocalizeStringEvent headerLocalizedString;

	[SerializeField]
	private LocalizedString obscuredHeaderKey;

	[SerializeField]
	private Image iconImage;

	private bool obscured;

	private global::ItemData itemData;
}
