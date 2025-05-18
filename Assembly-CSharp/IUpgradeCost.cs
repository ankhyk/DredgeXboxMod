using System;
using System.Collections.Generic;

public interface IUpgradeCost
{
	decimal GetMonetaryCost();

	List<ItemCountCondition> GetItemCost();

	GridKey GetGridKey();
}
