using System;

public abstract class HarvesterItemData : SpatialItemData
{
	public HarvesterItemData()
	{
		this.itemType = ItemType.EQUIPMENT;
	}

	public HarvestableType[] harvestableTypes;

	public bool isAdvancedEquipment;

	public float aberrationBonus;
}
