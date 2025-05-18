using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class TooltipSectionDescription : TooltipSection<global::ItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		if (!itemData.itemInsaneDescriptionKey.IsEmpty && GameManager.Instance.Player.IsBelowInsaneTooltipThreshold())
		{
			this.localizedDescriptionField.StringReference = itemData.itemInsaneDescriptionKey;
			this.descriptionTextField.color = this.insaneTextColor;
		}
		else if (itemData.linkedDialogueNode != "" && GameManager.Instance.DialogueRunner.GetHasVisitedNode(itemData.linkedDialogueNode))
		{
			this.localizedDescriptionField.StringReference = itemData.dialogueNodeSpecificDescription;
		}
		else
		{
			this.localizedDescriptionField.StringReference = itemData.itemDescriptionKey;
			this.descriptionTextField.color = itemData.tooltipTextColor;
		}
		this.localizedDescriptionField.StringReference.RefreshString();
		this.isLayedOut = true;
	}

	public override void Init(TextTooltipRequester textTooltipRequester)
	{
		this.isLayedOut = false;
		this.localizedDescriptionField.StringReference = textTooltipRequester.LocalizedDescriptionKey;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.descriptionTextField.color = textTooltipRequester.DescriptionTextColor;
		this.isLayedOut = true;
	}

	public override void Init(AbilityTooltipRequester abilityTooltipRequester)
	{
		this.isLayedOut = false;
		this.localizedDescriptionField.StringReference = abilityTooltipRequester.abilityData.descriptionKey;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.descriptionTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.isLayedOut = true;
	}

	public override void Init(UpgradeData upgradeData)
	{
		this.isLayedOut = false;
		object[] array = null;
		if (upgradeData is HullUpgradeData)
		{
			array = new object[]
			{
				upgradeData.tier,
				upgradeData.GetNewCellCount()
			};
		}
		else if (upgradeData is SlotUpgradeData)
		{
			array = new object[] { upgradeData.GetNewCellCount() };
		}
		this.localizedDescriptionField.enabled = false;
		this.localizedDescriptionField.StringReference = upgradeData.DescriptionKey;
		this.localizedDescriptionField.StringReference.Arguments = array;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.localizedDescriptionField.enabled = true;
		this.descriptionTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI descriptionTextField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	[SerializeField]
	private Color insaneTextColor;
}
