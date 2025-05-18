using System;

public class CellCountOfItemTypeAndSubtypeCondition : CompletedGridCondition
{
	public override bool Evaluate(SerializableGrid grid)
	{
		int matchingCellCount = 0;
		bool allMatch = true;
		SpatialItemData itemData;
		grid.spatialItems.ForEach(delegate(SpatialItemInstance i)
		{
			itemData = i.GetItemData<SpatialItemData>();
			matchingCellCount += itemData.GetSize();
			allMatch = allMatch && this.itemType.HasFlag(itemData.itemType) && this.itemSubtype.HasFlag(itemData.itemSubtype);
		});
		return allMatch && matchingCellCount >= this.targetCellCount;
	}

	public ItemType itemType;

	public ItemSubtype itemSubtype;

	public int targetCellCount;
}
