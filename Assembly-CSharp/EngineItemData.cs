using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EngineItemData", menuName = "Dredge/EngineItemData", order = 0)]
public class EngineItemData : SpatialItemData
{
	public EngineItemData()
	{
		this.itemType = ItemType.EQUIPMENT;
		this.itemSubtype = ItemSubtype.ENGINE;
		this.canBeDiscardedByPlayer = true;
	}

	public float speedBonus;
}
