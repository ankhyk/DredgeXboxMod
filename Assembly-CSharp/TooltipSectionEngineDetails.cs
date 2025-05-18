using System;
using TMPro;
using UnityEngine;

public class TooltipSectionEngineDetails : TooltipSection<EngineItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.Init<T>(itemData, null, tooltipMode);
	}

	public override void Init<T>(T itemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		if (tooltipMode == TooltipUI.TooltipMode.MYSTERY)
		{
			this.speedContainer.SetActive(false);
		}
		else
		{
			if (spatialItemInstance != null && spatialItemInstance.GetIsOnDamagedCell())
			{
				this.speedTextField.text = "0 kn";
				this.speedTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			}
			else
			{
				string text = (itemData.speedBonus * (1f + GameManager.Instance.PlayerStats.ResearchedMovementSpeedModifier)).ToString(".#");
				this.speedTextField.text = "+" + text + " kn";
				this.speedTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			}
			this.speedContainer.SetActive(true);
		}
		this.isLayedOut = true;
	}

	[SerializeField]
	private TextMeshProUGUI speedTextField;

	[SerializeField]
	private GameObject speedContainer;
}
