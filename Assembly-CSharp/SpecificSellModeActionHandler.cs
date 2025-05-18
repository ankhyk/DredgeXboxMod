using System;
using System.Linq;

public class SpecificSellModeActionHandler : SellModeActionHandler
{
	public SpecificSellModeActionHandler()
	{
		this.mode = GridMode.SELL_SPECIFIC;
	}

	public override void SetDestination(BaseDestination destination)
	{
		this.specificItemsBought = (destination as MarketDestination).SpecificItemsBought;
		base.SetDestination(destination);
	}

	protected override bool DoesStoreAcceptThisItem(SpatialItemData itemData, bool bulkMode)
	{
		return this.specificItemsBought.Contains(itemData);
	}

	private SpatialItemData[] specificItemsBought;
}
