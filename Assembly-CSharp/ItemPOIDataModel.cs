using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemPOIDataModel : POIDataModel
{
	public override bool GetDoesRestock()
	{
		throw new NotImplementedException();
	}

	public override HarvestableItemData GetFirstHarvestableItem()
	{
		throw new NotImplementedException();
	}

	public override HarvestableItemData GetActiveFirstHarvestableItem()
	{
		throw new NotImplementedException();
	}

	public override ItemData GetFirstItem()
	{
		if (this.items.Count <= 0)
		{
			return null;
		}
		return this.items[0];
	}

	public override bool ContainsItem(ItemData itemData)
	{
		return this.items.Contains(itemData);
	}

	public override ItemSubtype GetHarvestableItemSubType()
	{
		throw new NotImplementedException();
	}

	public override HarvestableType GetHarvestType()
	{
		throw new NotImplementedException();
	}

	public override bool GetIsAdvancedHarvestType()
	{
		throw new NotImplementedException();
	}

	public override float[] GetItemWeights()
	{
		throw new NotImplementedException();
	}

	public override float[] GetNightItemWeights()
	{
		throw new NotImplementedException();
	}

	public override GameObject GetParticlePrefab()
	{
		return this.GetFirstItem().harvestParticlePrefab;
	}

	public override HarvestableItemData GetRandomHarvestableItem()
	{
		throw new NotImplementedException();
	}

	public override HarvestableItemData GetNextHarvestableItem()
	{
		throw new NotImplementedException();
	}

	public override HarvestQueryEnum IsHarvestable()
	{
		if (this.GetStockCount(true) <= 0f)
		{
			return HarvestQueryEnum.INVALID_NO_STOCK;
		}
		return HarvestQueryEnum.VALID;
	}

	public override float GetMaxStock()
	{
		return 1f;
	}

	public override float GetStartStock()
	{
		return 1f;
	}

	public override List<ItemData> GetItems()
	{
		return this.items;
	}

	public override List<ItemData> GetNightItems()
	{
		throw new NotImplementedException();
	}

	[SerializeField]
	public List<ItemData> items = new List<ItemData>();
}
