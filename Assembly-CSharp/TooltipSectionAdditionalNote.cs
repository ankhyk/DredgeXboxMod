using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class TooltipSectionAdditionalNote : TooltipSection<global::ItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.localizedDescriptionField.StringReference = itemData.additionalNoteKey;
		this.localizedDescriptionField.StringReference.RefreshString();
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI descriptionTextField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;
}
