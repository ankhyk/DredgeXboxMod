using System;
using System.Collections.Generic;
using UnityEngine;

public class BaitPOIDataModel : HarvestPOIDataModel
{
	public override void AddStock(float count, bool regularDepletion = true)
	{
		if (count == -1f)
		{
			bool flag = this.itemStock.Count > 0;
			this.itemStock.Pop();
			if (regularDepletion && flag && this.itemStock.Count == 0)
			{
				SaveData saveData = GameManager.Instance.SaveData;
				int depletedSpotCount = saveData.DepletedSpotCount;
				saveData.DepletedSpotCount = depletedSpotCount + 1;
				GameEvents.Instance.TriggerFishingSpotDepleted();
			}
		}
	}

	public override void AtrophyStock()
	{
		this.itemStock.Clear();
	}

	public override GameObject GetParticlePrefab()
	{
		return null;
	}

	public override bool GetDoesRestock()
	{
		return false;
	}

	public override HarvestableItemData GetActiveFirstHarvestableItem()
	{
		return this.itemStock.Peek();
	}

	public override HarvestQueryEnum IsHarvestable()
	{
		if (this.GetStockCount(false) < 1f)
		{
			return HarvestQueryEnum.INVALID_NO_STOCK;
		}
		return HarvestQueryEnum.VALID;
	}

	public override float GetStockCount(bool ignoreTimeOfDay)
	{
		if (this.itemStock != null)
		{
			return (float)this.itemStock.Count;
		}
		return 0f;
	}

	public override bool RollForSpecial()
	{
		return false;
	}

	public override bool CanBeSpecial()
	{
		return false;
	}

	public override HarvestableType GetHarvestType()
	{
		if (this.itemStock.Count > 0)
		{
			return this.itemStock.Peek().harvestableType;
		}
		return HarvestableType.NONE;
	}

	public override bool GetIsAdvancedHarvestType()
	{
		return this.itemStock.Count > 0 && this.itemStock.Peek().requiresAdvancedEquipment;
	}

	public override ItemSubtype GetHarvestableItemSubType()
	{
		return ItemSubtype.FISH;
	}

	public override HarvestableItemData GetNextHarvestableItem()
	{
		if (this.itemStock.Count > 0)
		{
			return this.itemStock.Peek();
		}
		return null;
	}

	public Stack<HarvestableItemData> itemStock;
}
