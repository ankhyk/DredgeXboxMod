using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipSectionRodDetails : TooltipSection<RodItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.Init<T>(itemData, null, tooltipMode);
	}

	public override void Init<T>(T itemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.tags.ForEach(delegate(HarvestableTypeTagUI t)
		{
			t.gameObject.SetActive(false);
		});
		HarvestableType[] harvestableTypes = itemData.harvestableTypes;
		int num = 0;
		while (num < harvestableTypes.Length && num < this.tags.Count)
		{
			HarvestableType harvestableType = harvestableTypes[num];
			this.tags[num].Init(harvestableType, itemData.isAdvancedEquipment);
			this.tags[num].gameObject.SetActive(true);
			num++;
		}
		this.tagContainer.SetActive(harvestableTypes.Length != 0);
		float height = this.heightPerTagRow;
		if (harvestableTypes.Length > 2)
		{
			height = this.heightPerTagRow * 2f;
		}
		this.containersToAdjustHeightByRow.ForEach(delegate(RectTransform rt)
		{
			Vector2 sizeDelta = rt.sizeDelta;
			sizeDelta.y = height;
			rt.sizeDelta = sizeDelta;
		});
		if (tooltipMode == TooltipUI.TooltipMode.MYSTERY)
		{
			this.fishingSpeedContainer.SetActive(false);
			this.aberrationBonusContainer.SetActive(false);
		}
		else
		{
			if (spatialItemInstance != null && spatialItemInstance.GetIsOnDamagedCell())
			{
				this.fishingSpeedValueTextField.text = "0%";
				this.fishingSpeedValueTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			}
			else
			{
				string text = ((float)Mathf.RoundToInt(itemData.fishingSpeedModifier * (1f + GameManager.Instance.PlayerStats.ResearchedFishingSpeedModifier) * 100f)).ToString(".#");
				this.fishingSpeedValueTextField.text = "+" + text + "%";
				this.fishingSpeedValueTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			}
			if (itemData.aberrationBonus > 0f)
			{
				this.aberrationBonusTextField.text = "+" + (itemData.aberrationBonus * 100f).ToString(".#") + "%";
				this.aberrationBonusContainer.SetActive(true);
			}
			else
			{
				this.aberrationBonusContainer.SetActive(false);
			}
			this.fishingSpeedContainer.SetActive(itemData.fishingSpeedModifier != 0f);
		}
		this.isLayedOut = true;
	}

	[SerializeField]
	private List<RectTransform> containersToAdjustHeightByRow;

	[SerializeField]
	private float heightPerTagRow;

	[SerializeField]
	private TextMeshProUGUI fishingSpeedValueTextField;

	[SerializeField]
	private TextMeshProUGUI aberrationBonusTextField;

	[SerializeField]
	private GameObject fishingSpeedContainer;

	[SerializeField]
	private GameObject aberrationBonusContainer;

	[SerializeField]
	private GameObject tagContainer;

	[SerializeField]
	private List<HarvestableTypeTagUI> tags;
}
