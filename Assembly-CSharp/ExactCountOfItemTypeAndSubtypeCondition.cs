using System;

public class ExactCountOfItemTypeAndSubtypeCondition : CompletedGridCondition
{
	public override bool Evaluate(SerializableGrid grid)
	{
		bool allMatch = true;
		SpatialItemData itemData;
		grid.spatialItems.ForEach(delegate(SpatialItemInstance i)
		{
			itemData = i.GetItemData<SpatialItemData>();
			allMatch = allMatch && this.itemType.HasFlag(itemData.itemType) && this.itemSubtype.HasFlag(itemData.itemSubtype);
		});
		return allMatch && grid.spatialItems.Count == this.targetItemCount;
	}

	public ItemType itemType;

	public ItemSubtype itemSubtype;

	public int targetItemCount;
}
