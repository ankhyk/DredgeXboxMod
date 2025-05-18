using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class POIDataModel : IHarvestable
{
	public void Init(float currentInGameTime)
	{
		this.lastUpdate = currentInGameTime;
	}

	public virtual void AddStock(float count, bool countTowardsDepletionAchievement = true)
	{
		float num = this.GetStockCount(true);
		float maxStock = this.GetMaxStock();
		if (countTowardsDepletionAchievement && count < 0f && num >= 1f && num + count < 1f)
		{
			SaveData saveData = GameManager.Instance.SaveData;
			int depletedSpotCount = saveData.DepletedSpotCount;
			saveData.DepletedSpotCount = depletedSpotCount + 1;
			GameEvents.Instance.TriggerFishingSpotDepleted();
		}
		num += count;
		num = Mathf.Max(num, 0f);
		num = Mathf.Min(num, maxStock);
		GameManager.Instance.SaveData.harvestSpotStocks[this.id] = num;
	}

	public virtual void AtrophyStock()
	{
		GameManager.Instance.SaveData.harvestSpotStocks[this.id] = GameManager.Instance.GameConfigData.AtrophyStockPenalty;
		SaveData saveData = GameManager.Instance.SaveData;
		int depletedSpotCount = saveData.DepletedSpotCount;
		saveData.DepletedSpotCount = depletedSpotCount + 1;
		GameEvents.Instance.TriggerFishingSpotDepleted();
	}

	public bool AdjustStockLevels(float newGameTime)
	{
		float num = this.GetStockCount(true);
		float maxStock = this.GetMaxStock();
		bool flag = false;
		float num2 = newGameTime - this.lastUpdate;
		this.lastUpdate = newGameTime;
		if (num < maxStock && num2 > 0f)
		{
			float num3 = num2;
			float num4 = num / maxStock;
			num4 = Mathf.Max(num4, GameManager.Instance.GameConfigData.MinStockReplenish);
			int num5 = (int)num;
			float num6 = GameManager.Instance.GameConfigData.StockReplenishCoefficient * num3 * num4 * maxStock;
			num += num6;
			num = Mathf.Min(num, maxStock);
			GameManager.Instance.SaveData.harvestSpotStocks[this.id] = num;
			flag = (int)num != num5;
		}
		return flag;
	}

	public void OnHarvested(bool deductFromStock, bool countTowardsDepletionAchievement)
	{
		if (deductFromStock)
		{
			this.AddStock(-1f, countTowardsDepletionAchievement);
		}
	}

	public virtual float GetStockCount(bool ignoreTimeOfDay)
	{
		if (GameManager.Instance.SaveData.harvestSpotStocks.ContainsKey(this.id))
		{
			return GameManager.Instance.SaveData.harvestSpotStocks[this.id];
		}
		return this.GetStartStock();
	}

	public abstract bool GetDoesRestock();

	public abstract HarvestableItemData GetFirstHarvestableItem();

	public abstract HarvestableItemData GetActiveFirstHarvestableItem();

	public abstract ItemData GetFirstItem();

	public abstract bool ContainsItem(ItemData itemData);

	public abstract ItemSubtype GetHarvestableItemSubType();

	public abstract HarvestableType GetHarvestType();

	public abstract bool GetIsAdvancedHarvestType();

	public abstract float[] GetItemWeights();

	public abstract float[] GetNightItemWeights();

	public abstract GameObject GetParticlePrefab();

	public abstract HarvestableItemData GetRandomHarvestableItem();

	public abstract HarvestableItemData GetNextHarvestableItem();

	public abstract HarvestQueryEnum IsHarvestable();

	public abstract float GetMaxStock();

	public bool AdjustDurability(float gameTimeElapsed)
	{
		throw new NotImplementedException();
	}

	public float GetDurability()
	{
		throw new NotImplementedException();
	}

	public void SetDurability(float newDurability)
	{
		throw new NotImplementedException();
	}

	public abstract float GetStartStock();

	public string GetId()
	{
		return this.id;
	}

	public abstract List<ItemData> GetItems();

	public abstract List<ItemData> GetNightItems();

	public virtual bool GetUsesTimeSpecificStock()
	{
		return false;
	}

	public virtual bool RollForSpecial()
	{
		return false;
	}

	public virtual bool CanBeSpecial()
	{
		return false;
	}

	[SerializeField]
	public string id;

	public float lastUpdate;
}
