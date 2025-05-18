using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerializedCrabPotPOIData : IHarvestable
{
	public SerializedCrabPotPOIData(SpatialItemInstance spatialItemInstance, float x, float z, float yRot = 0f)
	{
		this.x = x;
		this.z = z;
		this.deployableItemId = spatialItemInstance.id;
		this.durability = spatialItemInstance.durability;
		this.deployableItemData = spatialItemInstance.GetItemData<DeployableItemData>();
		this.timeUntilNextCatchRoll = this.deployableItemData.TimeBetweenCatchRolls;
		this.yRotation = yRot;
		this.Init();
	}

	public void Init()
	{
		this.deployableItemData = GameManager.Instance.ItemManager.GetItemDataById<DeployableItemData>(this.deployableItemId);
		this.grid.Init(this.deployableItemData.GridConfig, false);
		GameEvents.Instance.OnHarvestZoneBecomeActive += this.RefreshCatchableItems;
		this.RefreshCatchableItems();
	}

	private void RefreshCatchableItems()
	{
		this.depth = GameManager.Instance.WaveController.SampleWaterDepthAtPosition(new Vector3(this.x, 0f, this.z));
		List<HarvestableItemData> list = new List<HarvestableItemData>();
		RaycastHit[] array = Physics.RaycastAll(new Vector3(this.x, 9999f, this.z), Vector3.down, 99999f, 1 << LayerMask.NameToLayer("HarvestZone"));
		for (int i = 0; i < array.Length; i++)
		{
			HarvestZone component = array[i].transform.gameObject.GetComponent<HarvestZone>();
			if (component)
			{
				for (int j = 0; j < component.HarvestableItems.Length; j++)
				{
					HarvestableItemData harvestableItemData = component.HarvestableItems[j];
					if (harvestableItemData.canBeCaughtByPot && this.grid.GridConfiguration.MainItemType.HasFlag(harvestableItemData.itemType) && this.grid.GridConfiguration.MainItemSubtype.HasFlag(harvestableItemData.itemSubtype) && harvestableItemData.CanCatchByDepth(this.depth, GameManager.Instance.GameConfigData) && !list.Contains(harvestableItemData))
					{
						list.Add(harvestableItemData);
					}
				}
			}
		}
		this.items = list;
	}

	public float GetStockCount(bool ignoreTimeOfDay)
	{
		return (float)this.grid.spatialItems.Count;
	}

	public float GetDurability()
	{
		return this.durability;
	}

	public void SetDurability(float val)
	{
		this.durability = val;
	}

	public bool AdjustDurability(float newGameTime)
	{
		this.hadDurabilityRemaining = this.durability > 0f;
		float num = newGameTime - this.lastUpdate;
		this.lastUpdate = newGameTime;
		this.durability -= num * (1f - GameManager.Instance.PlayerStats.ResearchedEquipmentMaintenanceModifier);
		this.durability = Mathf.Clamp(this.durability, 0f, this.deployableItemData.MaxDurabilityDays);
		return this.durability <= 0f && this.hadDurabilityRemaining;
	}

	public bool AdjustStockLevels(float newGameTime)
	{
		float num = newGameTime - this.lastUpdate;
		return this.CalculateCatchRoll(num);
	}

	private bool CalculateCatchRoll(float gameTimeElapsed)
	{
		bool flag = false;
		float num = Mathf.Min(gameTimeElapsed, this.deployableItemData.TimeBetweenCatchRolls);
		gameTimeElapsed -= num;
		this.timeUntilNextCatchRoll -= num;
		if (this.timeUntilNextCatchRoll <= 0f && this.durability > 0f)
		{
			if (global::UnityEngine.Random.value < this.deployableItemData.CatchRate)
			{
				MathUtil.GetRandomWeightedIndex(this.GetItemWeights());
				HarvestableItemData harvestableItemData = this.GetRandomHarvestableItem();
				if (harvestableItemData == null)
				{
					return false;
				}
				if (harvestableItemData.canBeReplacedWithResearchItem && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.ResearchItemCrabPotSpawnChance)
				{
					harvestableItemData = GameManager.Instance.ResearchHelper.ResearchItemData;
				}
				Vector3Int vector3Int;
				if (this.grid.FindPositionForObject(harvestableItemData, out vector3Int, 0, false))
				{
					SpatialItemInstance spatialItemInstance;
					if (harvestableItemData.itemSubtype == ItemSubtype.FISH)
					{
						spatialItemInstance = GameManager.Instance.ItemManager.CreateFishItem(harvestableItemData.id, FishAberrationGenerationMode.RANDOM_CHANCE, false, FishSizeGenerationMode.ANY, 1f + this.deployableItemData.aberrationBonus);
					}
					else
					{
						spatialItemInstance = new SpatialItemInstance
						{
							id = harvestableItemData.id
						};
					}
					this.grid.AddObjectToGridData(spatialItemInstance, vector3Int, false, null);
					flag = true;
				}
			}
			this.timeUntilNextCatchRoll = this.deployableItemData.TimeBetweenCatchRolls;
		}
		if (gameTimeElapsed > 0f)
		{
			flag = this.CalculateCatchRoll(gameTimeElapsed) || flag;
		}
		return flag;
	}

	public HarvestQueryEnum IsHarvestable()
	{
		return HarvestQueryEnum.VALID;
	}

	public float[] GetItemWeights()
	{
		return this.items.Select((HarvestableItemData i) => i.harvestItemWeight).ToArray<float>();
	}

	public float[] GetNightItemWeights()
	{
		throw new NotImplementedException();
	}

	public HarvestableItemData GetFirstHarvestableItem()
	{
		if (this.items.Count > 0)
		{
			return this.items[0];
		}
		return null;
	}

	public HarvestableItemData GetActiveFirstHarvestableItem()
	{
		return null;
	}

	public ItemData GetFirstItem()
	{
		return null;
	}

	public bool ContainsItem(ItemData itemData)
	{
		return false;
	}

	public GameObject GetParticlePrefab()
	{
		return null;
	}

	public HarvestableItemData GetNextHarvestableItem()
	{
		return this.GetRandomHarvestableItem();
	}

	public HarvestableItemData GetRandomHarvestableItem()
	{
		if (this.items.Count > 0)
		{
			return this.items[MathUtil.GetRandomWeightedIndex(this.GetItemWeights())];
		}
		return null;
	}

	public ItemSubtype GetHarvestableItemSubType()
	{
		return ItemSubtype.NONE;
	}

	public HarvestableType GetHarvestType()
	{
		return HarvestableType.NONE;
	}

	public bool GetIsAdvancedHarvestType()
	{
		return false;
	}

	public bool GetDoesRestock()
	{
		return true;
	}

	public void OnHarvested(bool deductFromStock, bool countTowardsDepletionAchievement)
	{
		throw new NotImplementedException();
	}

	public void AddStock(float count, bool normalDepletion)
	{
		throw new NotImplementedException();
	}

	public float GetMaxStock()
	{
		throw new NotImplementedException();
	}

	public float GetStartStock()
	{
		throw new NotImplementedException();
	}

	public string GetId()
	{
		return "";
	}

	public List<ItemData> GetItems()
	{
		throw new NotImplementedException();
	}

	public List<ItemData> GetNightItems()
	{
		throw new NotImplementedException();
	}

	public bool GetUsesTimeSpecificStock()
	{
		return false;
	}

	public Vector2 GetPosition()
	{
		return new Vector2(this.x, this.z);
	}

	public bool CanBeSpecial()
	{
		return false;
	}

	public bool RollForSpecial()
	{
		return false;
	}

	public void AtrophyStock()
	{
		this.grid.Clear(true);
	}

	public void Init(float currentInGameTime)
	{
	}

	public float x;

	public float z;

	public string deployableItemId;

	public float durability;

	public float timeUntilNextCatchRoll;

	public float lastUpdate;

	public SerializableGrid grid = new SerializableGrid();

	[NonSerialized]
	private DeployableItemData deployableItemData;

	[NonSerialized]
	public float yRotation;

	[NonSerialized]
	public float depth;

	[NonSerialized]
	public List<HarvestableItemData> items;

	private bool hadDurabilityRemaining;
}
