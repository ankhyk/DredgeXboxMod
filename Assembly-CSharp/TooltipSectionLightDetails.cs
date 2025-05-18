using System;
using TMPro;
using UnityEngine;

public class TooltipSectionLightDetails : TooltipSection<LightItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.Init<T>(itemData, null, tooltipMode);
	}

	public override void Init<T>(T itemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		if (spatialItemInstance != null && spatialItemInstance.GetIsOnDamagedCell())
		{
			this.lumensTextField.text = "0 lm";
			this.rangeTextField.text = GameManager.Instance.LanguageManager.FormatDistanceString(0f) ?? "";
			this.lumensTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			this.rangeTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
		else
		{
			float lumens = itemData.lumens;
			this.lumensTextField.text = string.Format("+{0} lm", lumens);
			float range = itemData.range;
			this.rangeTextField.text = GameManager.Instance.LanguageManager.FormatDistanceString(range) ?? "";
			this.lumensTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			this.rangeTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		}
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI lumensTextField;

	[SerializeField]
	private TextMeshProUGUI rangeTextField;
}
