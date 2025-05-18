using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DredgeItemData", menuName = "Dredge/DredgeItemData", order = 0)]
public class DredgeItemData : HarvesterItemData
{
	public DredgeItemData()
	{
		this.itemSubtype = ItemSubtype.DREDGE;
		this.harvestableTypes = new HarvestableType[] { HarvestableType.DREDGE };
	}
}
