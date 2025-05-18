using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageItemData", menuName = "Dredge/DamageItemData", order = 0)]
public class DamageItemData : SpatialItemData
{
	public DamageItemData()
	{
		this.id = "dmg";
		this.canBeDiscardedByPlayer = false;
		this.itemType = ItemType.DAMAGE;
	}
}
