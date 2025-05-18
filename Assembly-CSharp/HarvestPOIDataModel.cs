using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestPOIDataModel : POIDataModel
{
	public override float[] GetItemWeights()
	{
		return this.items.Select((HarvestableItemData i) => i.harvestItemWeight).ToArray<float>();
	}

	public override float[] GetNightItemWeights()
	{
		return this.nightItems.Select((HarvestableItemData i) => i.harvestItemWeight).ToArray<float>();
	}

	public override HarvestableItemData GetFirstHarvestableItem()
	{
		if (this.items != null && this.items.Count > 0)
		{
			return this.items[0];
		}
		if (this.nightItems != null && this.nightItems.Count > 0)
		{
			return this.nightItems[0];
		}
		return null;
	}

	public override HarvestableItemData GetActiveFirstHarvestableItem()
	{
		bool flag = true;
		if (GameManager.Instance && GameManager.Instance.Time)
		{
			flag = GameManager.Instance.Time.IsDaytime;
		}
		if (this.items != null && this.items.Count > 0 && (!this.usesTimeSpecificStock || (this.usesTimeSpecificStock && flag)))
		{
			return this.items[0];
		}
		if (this.nightItems != null && this.nightItems.Count > 0 && this.usesTimeSpecificStock && !flag)
		{
			return this.nightItems[0];
		}
		return null;
	}

	public override ItemData GetFirstItem()
	{
		if (this.items != null && this.items.Count > 0)
		{
			return this.items[0];
		}
		if (this.nightItems != null && this.nightItems.Count > 0)
		{
			return this.nightItems[0];
		}
		return null;
	}

	public override bool ContainsItem(ItemData itemData)
	{
		return this.items.Contains(itemData) || this.nightItems.Contains(itemData);
	}

	public override GameObject GetParticlePrefab()
	{
		if (this.items.Count > 0 || this.nightItems.Count > 0)
		{
			return this.GetFirstHarvestableItem().harvestParticlePrefab;
		}
		return null;
	}

	public override HarvestableItemData GetNextHarvestableItem()
	{
		return this.GetRandomHarvestableItem();
	}

	public override HarvestableItemData GetRandomHarvestableItem()
	{
		if (this.usesTimeSpecificStock)
		{
			if (GameManager.Instance.Time.IsDaytime && this.items.Count > 0)
			{
				return this.items[MathUtil.GetRandomWeightedIndex(this.GetItemWeights())];
			}
			if (!GameManager.Instance.Time.IsDaytime && this.nightItems.Count > 0)
			{
				return this.nightItems[MathUtil.GetRandomWeightedIndex(this.GetNightItemWeights())];
			}
			return null;
		}
		else
		{
			if (this.items.Count > 0)
			{
				return this.items[MathUtil.GetRandomWeightedIndex(this.GetItemWeights())];
			}
			return null;
		}
	}

	public override ItemSubtype GetHarvestableItemSubType()
	{
		HarvestableItemData firstHarvestableItem = this.GetFirstHarvestableItem();
		if (!(firstHarvestableItem == null))
		{
			return firstHarvestableItem.itemSubtype;
		}
		return ItemSubtype.NONE;
	}

	public override HarvestableType GetHarvestType()
	{
		HarvestableItemData firstHarvestableItem = this.GetFirstHarvestableItem();
		if (!(firstHarvestableItem == null))
		{
			return firstHarvestableItem.harvestableType;
		}
		return HarvestableType.NONE;
	}

	public override bool GetIsAdvancedHarvestType()
	{
		HarvestableItemData firstHarvestableItem = this.GetFirstHarvestableItem();
		return !(firstHarvestableItem == null) && firstHarvestableItem.requiresAdvancedEquipment;
	}

	public override bool GetDoesRestock()
	{
		return this.doesRestock;
	}

	public override HarvestQueryEnum IsHarvestable()
	{
		if (this.usesTimeSpecificStock)
		{
			if (GameManager.Instance.Time.IsDaytime && this.items.Count == 0)
			{
				return HarvestQueryEnum.INVALID_INCORRECT_TIME;
			}
			if (!GameManager.Instance.Time.IsDaytime && this.nightItems.Count == 0)
			{
				return HarvestQueryEnum.INVALID_INCORRECT_TIME;
			}
		}
		if (this.GetStockCount(false) < 1f)
		{
			return HarvestQueryEnum.INVALID_NO_STOCK;
		}
		return HarvestQueryEnum.VALID;
	}

	public override float GetMaxStock()
	{
		return this.maxStock;
	}

	public override float GetStartStock()
	{
		return this.startStock;
	}

	public override List<ItemData> GetItems()
	{
		return this.items.Cast<ItemData>().ToList<ItemData>();
	}

	public override List<ItemData> GetNightItems()
	{
		return this.nightItems.Cast<ItemData>().ToList<ItemData>();
	}

	public override float GetStockCount(bool ignoreTimeOfDay)
	{
		if (this.usesTimeSpecificStock && !ignoreTimeOfDay)
		{
			if (GameManager.Instance.Time.IsDaytime && this.items.Count == 0)
			{
				return 0f;
			}
			if (!GameManager.Instance.Time.IsDaytime && this.nightItems.Count == 0)
			{
				return 0f;
			}
		}
		return base.GetStockCount(ignoreTimeOfDay);
	}

	public override bool GetUsesTimeSpecificStock()
	{
		return this.usesTimeSpecificStock;
	}

	private float GetDaytimeSpecialChance()
	{
		if (!this.MainItemsHaveAberrations())
		{
			return 0f;
		}
		if (!this.overrideDefaultDaySpecialChance)
		{
			return GameManager.Instance.GameConfigData.SpecialSpotChanceDay;
		}
		return this.overriddenDaytimeSpecialChance;
	}

	private float GetNighttimeSpecialChance()
	{
		if ((!this.usesTimeSpecificStock || !this.NighttimeItemsHaveAberrations()) && (this.usesTimeSpecificStock || !this.MainItemsHaveAberrations()))
		{
			return 0f;
		}
		if (!this.overrideDefaultNightSpecialChance)
		{
			return GameManager.Instance.GameConfigData.SpecialSpotChanceNight;
		}
		return this.overriddenNighttimeSpecialChance;
	}

	private bool MainItemsHaveAberrations()
	{
		return this.items.Count > 0 && this.items[0] != null && this.items[0] is FishItemData && (this.items[0] as FishItemData).Aberrations.Count > 0;
	}

	private bool NighttimeItemsHaveAberrations()
	{
		return this.nightItems.Count > 0 && this.nightItems[0] != null && this.nightItems[0] is FishItemData && (this.nightItems[0] as FishItemData).Aberrations.Count > 0;
	}

	public override bool RollForSpecial()
	{
		if (this.GetHarvestType() == HarvestableType.DREDGE)
		{
			return false;
		}
		if (!GameManager.Instance.SaveData.CanCatchAberrations)
		{
			return false;
		}
		float daytimeSpecialChance = this.GetDaytimeSpecialChance();
		float nighttimeSpecialChance = this.GetNighttimeSpecialChance();
		if (daytimeSpecialChance > 0f && GameManager.Instance.Time.IsDaytime)
		{
			return global::UnityEngine.Random.value < daytimeSpecialChance;
		}
		return nighttimeSpecialChance > 0f && !GameManager.Instance.Time.IsDaytime && global::UnityEngine.Random.value < nighttimeSpecialChance;
	}

	public override bool CanBeSpecial()
	{
		return this.GetDaytimeSpecialChance() > 0f || this.GetNighttimeSpecialChance() > 0f;
	}

	[SerializeField]
	public List<HarvestableItemData> items = new List<HarvestableItemData>();

	[SerializeField]
	public List<HarvestableItemData> nightItems = new List<HarvestableItemData>();

	[SerializeField]
	public float startStock = 3f;

	[SerializeField]
	public float maxStock = 5f;

	[SerializeField]
	public bool doesRestock = true;

	[SerializeField]
	public bool usesTimeSpecificStock;

	[SerializeField]
	public bool overrideDefaultDaySpecialChance;

	[SerializeField]
	[Range(0f, 1f)]
	public float overriddenDaytimeSpecialChance;

	[SerializeField]
	public bool overrideDefaultNightSpecialChance;

	[SerializeField]
	[Range(0f, 1f)]
	public float overriddenNighttimeSpecialChance;
}
