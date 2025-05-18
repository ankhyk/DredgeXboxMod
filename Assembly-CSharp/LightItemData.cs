using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LightItemData", menuName = "Dredge/LightItemData", order = 0)]
public class LightItemData : SpatialItemData
{
	public LightItemData()
	{
		this.itemType = ItemType.EQUIPMENT;
		this.itemSubtype = ItemSubtype.LIGHT;
		this.canBeDiscardedByPlayer = true;
	}

	public float lumens;

	public float range;
}
