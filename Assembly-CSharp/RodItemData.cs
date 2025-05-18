using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RodItemData", menuName = "Dredge/RodItemData", order = 0)]
public class RodItemData : HarvesterItemData
{
	public RodItemData()
	{
		this.itemSubtype = ItemSubtype.ROD;
		this.canBeDiscardedByPlayer = true;
	}

	public float fishingSpeedModifier;
}
