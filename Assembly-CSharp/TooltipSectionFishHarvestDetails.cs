using System;
using UnityEngine;

public class TooltipSectionFishHarvestDetails : TooltipSection<FishItemData>
{
	public override void Init<T>(T fishItemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		if (fishItemData.harvestableType == HarvestableType.NONE)
		{
			this.harvestableTypeContainer.SetActive(false);
		}
		else
		{
			this.harvestableTypeContainer.SetActive(true);
			HarvestableType harvestableType = fishItemData.harvestableType;
			this.harvestableTypeTagUI.Init(harvestableType, fishItemData.requiresAdvancedEquipment);
		}
		this.rodIcon.SetActive(fishItemData.canBeCaughtByRod);
		this.potIcon.SetActive(fishItemData.canBeCaughtByPot);
		this.netIcon.SetActive(fishItemData.canBeCaughtByNet);
		this.isLayedOut = true;
	}

	[SerializeField]
	private GameObject harvestableTypeContainer;

	[SerializeField]
	private HarvestableTypeTagUI harvestableTypeTagUI;

	[SerializeField]
	private GameObject rodIcon;

	[SerializeField]
	private GameObject potIcon;

	[SerializeField]
	private GameObject netIcon;
}
