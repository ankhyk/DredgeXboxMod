using System;

public class TypeSellModeActionHandler : SellModeActionHandler
{
	public TypeSellModeActionHandler()
	{
		this.mode = GridMode.SELL_TYPE;
	}

	public override void SetDestination(BaseDestination destination)
	{
		this.itemTypesBought = (destination as MarketDestination).ItemTypesBought;
		this.itemSubtypesBought = (destination as MarketDestination).ItemSubtypesBought;
		this.bulkItemTypesBought = (destination as MarketDestination).BulkItemTypesBought;
		this.bulkItemSubtypesBought = (destination as MarketDestination).BulkItemSubtypesBought;
		base.SetDestination(destination);
	}

	protected override bool DoesStoreAcceptThisItem(SpatialItemData itemData, bool bulkMode)
	{
		if (!itemData.canBeSoldByPlayer)
		{
			return false;
		}
		if (this.itemSubtypesBought == ItemSubtype.ALL)
		{
			return this.itemTypesBought.HasFlag(itemData.itemType);
		}
		if (bulkMode)
		{
			return this.bulkItemTypesBought.HasFlag(itemData.itemType) && this.bulkItemSubtypesBought.HasFlag(itemData.itemSubtype);
		}
		return this.itemTypesBought.HasFlag(itemData.itemType) && this.itemSubtypesBought.HasFlag(itemData.itemSubtype);
	}
}
