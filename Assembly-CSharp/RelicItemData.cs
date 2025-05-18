using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicItemData", menuName = "Dredge/RelicItemData", order = 0)]
public class RelicItemData : HarvestableItemData
{
	public RelicItemData()
	{
		this.itemType = ItemType.GENERAL;
		this.itemSubtype = ItemSubtype.RELIC;
		this.canBeDiscardedByPlayer = false;
		this.harvestableType = HarvestableType.DREDGE;
	}
}
