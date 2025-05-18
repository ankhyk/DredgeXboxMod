using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHarvestable
{
	float[] GetItemWeights();

	float[] GetNightItemWeights();

	HarvestableItemData GetFirstHarvestableItem();

	HarvestableItemData GetActiveFirstHarvestableItem();

	string GetId();

	bool GetUsesTimeSpecificStock();

	bool CanBeSpecial();

	bool RollForSpecial();

	List<ItemData> GetItems();

	List<ItemData> GetNightItems();

	ItemData GetFirstItem();

	bool ContainsItem(ItemData itemData);

	GameObject GetParticlePrefab();

	HarvestableItemData GetRandomHarvestableItem();

	HarvestableItemData GetNextHarvestableItem();

	ItemSubtype GetHarvestableItemSubType();

	HarvestableType GetHarvestType();

	bool GetIsAdvancedHarvestType();

	bool GetDoesRestock();

	float GetStockCount(bool ignoreTimeOfDay);

	float GetStartStock();

	float GetMaxStock();

	HarvestQueryEnum IsHarvestable();

	void OnHarvested(bool deductFromStock, bool countTowardsDepletionAchievement);

	void AddStock(float count, bool normalDepletion);

	void AtrophyStock();

	bool AdjustStockLevels(float count);

	bool AdjustDurability(float newGameTime);

	float GetDurability();

	void SetDurability(float newDurability);

	void Init(float currentInGameTime);
}
