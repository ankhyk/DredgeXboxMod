using System;

public class ItemCountCondition : CompletedGridCondition
{
	public override bool Evaluate(SerializableGrid grid)
	{
		return this.CountItems(grid) >= this.count;
	}

	public int CountItems(SerializableGrid grid)
	{
		int num;
		if (this.item is FishItemData && this.allowLinkedAberrations)
		{
			num = grid.spatialItems.FindAll(delegate(SpatialItemInstance itemInstance)
			{
				FishItemData itemData = itemInstance.GetItemData<FishItemData>();
				return itemInstance.id == this.item.id || (itemData.IsAberration && itemData.NonAberrationParent != null && itemData.NonAberrationParent.id == this.item.id);
			}).Count;
		}
		else
		{
			num = grid.spatialItems.FindAll((SpatialItemInstance i) => i.id == this.item.id).Count;
		}
		return num;
	}

	public SpatialItemData item;

	public int count;

	public bool allowLinkedAberrations;
}
