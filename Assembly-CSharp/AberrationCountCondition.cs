using System;

public class AberrationCountCondition : CompletedGridCondition
{
	public override bool Evaluate(SerializableGrid grid)
	{
		int matchingItemCount = 0;
		SpatialItemData itemData;
		grid.spatialItems.ForEach(delegate(SpatialItemInstance i)
		{
			itemData = i.GetItemData<SpatialItemData>();
			if (itemData is FishItemData && (itemData as FishItemData).IsAberration)
			{
				int matchingItemCount2 = matchingItemCount;
				matchingItemCount = matchingItemCount2 + 1;
			}
		});
		return matchingItemCount >= this.targetItemCount;
	}

	public int targetItemCount;
}
