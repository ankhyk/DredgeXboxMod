using System;
using System.Collections.Generic;
using System.Linq;

public class NumItemsOfSizeAndTypeCondition : InventoryCondition
{
	public override bool Evaluate()
	{
		int num = 0;
		List<SpatialItemInstance> list = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(this.itemType, this.itemSubtype).ToList<SpatialItemInstance>();
		for (int i = 0; i < list.Count; i++)
		{
			SpatialItemData itemData = list[i].GetItemData<SpatialItemData>();
			if (this.max && itemData.GetSize() <= this.size)
			{
				num++;
			}
			else if (this.min && itemData.GetSize() >= this.size)
			{
				num++;
			}
		}
		return num >= this.minNumber;
	}

	public int minNumber;

	public int size;

	public bool max;

	public bool min;

	public ItemType itemType;

	public ItemSubtype itemSubtype;
}
