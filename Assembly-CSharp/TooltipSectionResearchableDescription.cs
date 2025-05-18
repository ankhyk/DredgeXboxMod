using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TooltipSectionResearchableDescription : TooltipSection<ResearchableItemInstance>
{
	public override void Init<T>(T itemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		ResearchableItemData itemData = itemInstance.GetItemData<ResearchableItemData>();
		LocalizedString localizedString = (itemInstance.IsResearchComplete ? itemData.itemDescriptionKey : this.unreadDescriptionKey);
		this.localizedDescriptionField.StringReference = localizedString;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.descriptionTextField.color = itemData.tooltipTextColor;
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI descriptionTextField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	[SerializeField]
	public LocalizedString unreadDescriptionKey;
}
