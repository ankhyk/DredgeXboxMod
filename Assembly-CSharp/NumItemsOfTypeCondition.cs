using System;

public class NumItemsOfTypeCondition : InventoryCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(this.itemType, this.itemSubtype).Count >= this.minNumber;
	}

	public int minNumber;

	public ItemType itemType;

	public ItemSubtype itemSubtype;
}
