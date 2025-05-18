using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
	public bool ForbidSave { get; set; }

	public bool CanUseStorageTray
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_CAN_USE_STORAGE_TRAY, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_CAN_USE_STORAGE_TRAY, value);
		}
	}

	public bool UpgradeBought
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_UPGRADE_BOUGHT, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_UPGRADE_BOUGHT, value);
		}
	}

	public bool CanShowFinaleHijack
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_CAN_SHOW_FINALE_HIJACK, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_CAN_SHOW_FINALE_HIJACK, value);
		}
	}

	public bool HasViewedIntroCutscene
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_VIEWED_INTRO_CUTSCENE, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_VIEWED_INTRO_CUTSCENE, value);
		}
	}

	public int FishUntilNextTrophyNotch
	{
		get
		{
			return this.GetIntVariable(SaveData.VAR_FISH_UNTIL_NEXT_TROPHY_NOTCH, 0);
		}
		set
		{
			this.SetIntVariable(SaveData.VAR_FISH_UNTIL_NEXT_TROPHY_NOTCH, value);
		}
	}

	public int RodFishCaught
	{
		get
		{
			return this.GetIntVariable("rod-fish-caught", 0);
		}
		set
		{
			this.SetIntVariable("rod-fish-caught", value);
		}
	}

	public int NetFishCaught
	{
		get
		{
			return this.GetIntVariable("net-fish-caught", 0);
		}
		set
		{
			this.SetIntVariable("net-fish-caught", value);
		}
	}

	public int PotCrabsCaught
	{
		get
		{
			return this.GetIntVariable("pot-crabs-caught", 0);
		}
		set
		{
			this.SetIntVariable("pot-crabs-caught", value);
		}
	}

	public bool HasCaughtAberrationAtSpecialSpot
	{
		get
		{
			return this.GetBoolVariable("aberration-special-spot", false);
		}
		set
		{
			this.SetBoolVariable("aberration-special-spot", value);
		}
	}

	public bool CanCatchAberrations
	{
		get
		{
			return this.GetBoolVariable("can-catch-aberrations", false);
		}
		set
		{
			this.SetBoolVariable("can-catch-aberrations", value);
		}
	}

	public int NumAberrationsCaught
	{
		get
		{
			return this.GetIntVariable("num-aberrations-caught", 0);
		}
		set
		{
			this.SetIntVariable("num-aberrations-caught", value);
		}
	}

	public int WorldPhase
	{
		get
		{
			return this.GetIntVariable("world-phase", 0);
		}
		set
		{
			this.SetIntVariable("world-phase", value);
		}
	}

	public int TPRWorldPhase
	{
		get
		{
			return this.GetIntVariable("tpr-world-phase", 0);
		}
		set
		{
			this.SetIntVariable("tpr-world-phase", value);
		}
	}

	public int TIRWorldPhase
	{
		get
		{
			return this.GetIntVariable("tir-world-phase", 0);
		}
		set
		{
			this.SetIntVariable("tir-world-phase", value);
		}
	}

	public int FishDiscardCount
	{
		get
		{
			return this.GetIntVariable("fish-discard-count", 0);
		}
		set
		{
			this.SetIntVariable("fish-discard-count", value);
		}
	}

	public int DepletedSpotCount
	{
		get
		{
			return this.GetIntVariable("fish-depleted-count", 0);
		}
		set
		{
			this.SetIntVariable("fish-depleted-count", value);
		}
	}

	public int ThreatsBanishedCount
	{
		get
		{
			return this.GetIntVariable(SaveData.VAR_THREATS_BANISHED, 0);
		}
		set
		{
			this.SetIntVariable(SaveData.VAR_THREATS_BANISHED, value);
		}
	}

	public decimal Funds
	{
		get
		{
			return this.GetDecimalVariable("funds", 0m);
		}
		set
		{
			this.SetDecimalVariable("funds", value);
		}
	}

	public decimal FishSaleTotal
	{
		get
		{
			return this.GetDecimalVariable("fish-sale-total", 0m);
		}
		set
		{
			this.SetDecimalVariable("fish-sale-total", value);
		}
	}

	public decimal TrinketSaleTotal
	{
		get
		{
			return this.GetDecimalVariable("trinket-sale-total", 0m);
		}
		set
		{
			this.SetDecimalVariable("trinket-sale-total", value);
		}
	}

	public decimal Time
	{
		get
		{
			return this.GetDecimalVariable("time", 0m);
		}
		set
		{
			this.SetDecimalVariable("time", value);
		}
	}

	public float Sanity
	{
		get
		{
			return this.GetFloatVariable("sanity", 0f);
		}
		set
		{
			this.SetFloatVariable("sanity", value);
		}
	}

	public string Weather
	{
		get
		{
			return this.GetStringVariable("weather", "");
		}
		set
		{
			this.SetStringVariable("weather", value);
		}
	}

	public float WeatherChangeTime
	{
		get
		{
			return this.GetFloatVariable("weather-time", 0f);
		}
		set
		{
			this.SetFloatVariable("weather-time", value);
		}
	}

	public decimal AberrationSpawnModifier
	{
		get
		{
			return this.GetDecimalVariable("aberration-spawn-modifier", 0m);
		}
		set
		{
			this.SetDecimalVariable("aberration-spawn-modifier", value);
		}
	}

	public int HullTier
	{
		get
		{
			return this.GetIntVariable("hull-tier", 0);
		}
		set
		{
			this.SetIntVariable("hull-tier", value);
		}
	}

	public decimal GreaterMarrowRepayments
	{
		get
		{
			return this.GetDecimalVariable("gm-repayments", 0m);
		}
		set
		{
			this.SetDecimalVariable("gm-repayments", value);
		}
	}

	public float BanishMachineExpiry
	{
		get
		{
			return this.GetFloatVariable(SaveData.VAR_BANISH_MACHINE_EXPIRY, 0f);
		}
		set
		{
			this.SetFloatVariable(SaveData.VAR_BANISH_MACHINE_EXPIRY, value);
		}
	}

	public float LastInfectTime
	{
		get
		{
			return this.GetFloatVariable("last-infect-time", 0f);
		}
		set
		{
			this.SetFloatVariable("last-infect-time", value);
		}
	}

	public string LastUnseenCaughtSpecies
	{
		get
		{
			return this.GetStringVariable("last-unseen-caught-species", "");
		}
		set
		{
			this.SetStringVariable("last-unseen-caught-species", value);
		}
	}

	public string LastSelectedAbility
	{
		get
		{
			return this.GetStringVariable(SaveData.VAR_LAST_SELECTED_ABILITY, "");
		}
		set
		{
			this.SetStringVariable(SaveData.VAR_LAST_SELECTED_ABILITY, value);
		}
	}

	public int RoofColorIndex
	{
		get
		{
			return this.GetIntVariable(SaveData.VAR_ROOF_COLOR_INDEX, 0);
		}
		set
		{
			this.SetIntVariable(SaveData.VAR_ROOF_COLOR_INDEX, value);
		}
	}

	public int HullColorIndex
	{
		get
		{
			return this.GetIntVariable(SaveData.VAR_HULL_COLOR_INDEX, 0);
		}
		set
		{
			this.SetIntVariable(SaveData.VAR_HULL_COLOR_INDEX, value);
		}
	}

	public bool HasChangedBoatColors
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_HAS_CHANGED_BOAT_COLORS, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_HAS_CHANGED_BOAT_COLORS, value);
		}
	}

	public bool IsBoatBuntingEnabled
	{
		get
		{
			return this.GetBoolVariable(SaveData.VAR_IS_BOAT_BUNTING_ENABLED, false);
		}
		set
		{
			this.SetBoolVariable(SaveData.VAR_IS_BOAT_BUNTING_ENABLED, value);
		}
	}

	public int BoatFlagStyle
	{
		get
		{
			return this.GetIntVariable(SaveData.VAR_BOAT_FLAG_STYLE, 0);
		}
		set
		{
			this.SetIntVariable(SaveData.VAR_BOAT_FLAG_STYLE, value);
		}
	}

	public float IceMonsterTimeUntilSpawn
	{
		get
		{
			return this.GetFloatVariable(SaveData.VAR_ICE_MONSTER_TIME_UNTIL_SPAWN, 0f);
		}
		set
		{
			this.SetFloatVariable(SaveData.VAR_ICE_MONSTER_TIME_UNTIL_SPAWN, value);
		}
	}

	public SaveData(SaveDataTemplate template)
	{
		this.version = template.version;
		this.dockId = template.dockId;
		this.dockSlotIndex = template.dockSlotIndex;
		this.decimalVariables = (Dictionary<string, decimal>)CollectionUtil.DeepClone(template.decimalVariables);
		this.intVariables = (Dictionary<string, int>)CollectionUtil.DeepClone(template.intVariables);
		this.floatVariables = (Dictionary<string, float>)CollectionUtil.DeepClone(template.floatVariables);
		this.stringVariables = (Dictionary<string, string>)CollectionUtil.DeepClone(template.stringVariables);
		this.boolVariables = (Dictionary<string, bool>)CollectionUtil.DeepClone(template.boolVariables);
		this.eventHistory = new Dictionary<string, float>();
		this.grids = (Dictionary<GridKey, SerializableGrid>)CollectionUtil.DeepClone(template.grids);
		this.ownedNonSpatialItems = (List<NonSpatialItemInstance>)CollectionUtil.DeepClone(template.ownedNonSpatialItems);
		this.visitedNodes = new HashSet<string>();
		this.historyOfItemsOwned = new HashSet<string>();
		this.upgradeIdsOwned = new List<string>();
		this.itemIdsResearched = new List<string>();
		this.questEntries = (Dictionary<string, SerializedQuestEntry>)CollectionUtil.DeepClone(template.questEntries);
		this.temporalMarkers = new List<SerializedTemporalMarker>();
		this.itemTransactions = new List<SerializedItemTransaction>();
		this.shopHistories = new List<SerializedShopHistory>();
		this.caughtFishCounts = new Dictionary<string, int>();
		this.harvestedItemSizeRecords = new Dictionary<string, float>();
		this.availableDestinations = (HashSet<string>)CollectionUtil.DeepClone(template.availableDestinations);
		this.availableSpeakers = (HashSet<string>)CollectionUtil.DeepClone(template.availableSpeakers);
		this.unlockedAbilities = (List<string>)CollectionUtil.DeepClone(template.unlockedAbilities);
		this.abilityHistory = new Dictionary<string, float>();
		this.abilityToggleStates = new Dictionary<string, bool>();
		this.serializedCrabPotPOIs = (List<SerializedCrabPotPOIData>)CollectionUtil.DeepClone(template.serializedCrabPotPOIs);
		this.harvestSpotStocks = new Dictionary<string, float>();
		this.mapMarkers = new List<SerializedMapMarker>();
		this.harvestPOIMapMarkers = new List<string>();
		this.completedActivityPS5 = false;
	}

	public void Init()
	{
		foreach (object obj in Enum.GetValues(typeof(GridKey)))
		{
			GridKey gridKey = (GridKey)obj;
			if (gridKey != GridKey.NONE)
			{
				GridConfiguration gridConfiguration;
				if (gridKey == GridKey.INVENTORY)
				{
					gridConfiguration = GameManager.Instance.GameConfigData.GetGridConfigForHullTier(this.HullTier);
				}
				else
				{
					gridConfiguration = GameManager.Instance.GameConfigData.GetGridConfigForKey(gridKey);
				}
				if (gridConfiguration == null)
				{
					return;
				}
				SerializableGrid serializableGrid;
				if (this.grids.ContainsKey(gridKey))
				{
					serializableGrid = this.grids[gridKey];
				}
				else
				{
					serializableGrid = new SerializableGrid();
					this.grids.Add(gridKey, serializableGrid);
				}
				serializableGrid.Init(gridConfiguration, false);
				if (gridKey == GridKey.INVENTORY)
				{
					GameManager.Instance.UpgradeManager.ApplyOwnedSlotUpgrades();
				}
			}
		}
		if (this.mapStamps == null)
		{
			this.mapStamps = new List<SerializedMapStamp>();
		}
		if (this.harvestPOIMapMarkers == null)
		{
			this.harvestPOIMapMarkers = new List<string>();
		}
		this.ApplyMissingDredgeFixes();
		this.CheckForResearchProgress();
		this.AddHoodedFigureBooks();
		this.RemoveCameraAbilityIfUnearned();
		this.AddDefaultBoatCustomizationOptions();
		this.UnlockPainterIfRequired();
		this.FixTimeFormat();
		this.PreventIcebreakerSoftLock();
		this.RemoveNullDataFromMapMarkers();
	}

	private void RemoveNullDataFromMapMarkers()
	{
		this.harvestPOIMapMarkers.RemoveAll((string m) => string.IsNullOrEmpty(m));
	}

	private void PreventIcebreakerSoftLock()
	{
		if (this.dockId == "dock.tpr-south" && !this.GetIsIcebreakerEquipped())
		{
			this.SetBoolVariable(BoatSubModelToggler.ICEBREAKER_EQUIP_STRING_KEY, true);
		}
	}

	private void FixTimeFormat()
	{
		if ((float)this.GetDecimalVariable("time", 0m) < this.GetFloatVariable("time", 0f))
		{
			this.SetDecimalVariable("time", (decimal)this.GetFloatVariable("time", 0f));
		}
	}

	private void RemoveCameraAbilityIfUnearned()
	{
		if (!this.visitedNodes.Contains("Photographer_HandInItem"))
		{
			this.unlockedAbilities.Remove("camera");
		}
	}

	private void AddDefaultBoatCustomizationOptions()
	{
		this.SetBoolVariable("has-paint-0", true);
		this.SetBoolVariable("has-paint-1", true);
		this.SetBoolVariable("has-paint-2", true);
		this.SetBoolVariable("has-flag-3", true);
	}

	private void UnlockPainterIfRequired()
	{
		if (this.visitedNodes.Contains("Collector_StrangeFish_Proposal_Accepted") && !this.availableDestinations.Contains("destination.lm-painter"))
		{
			this.availableDestinations.Add("destination.lm-painter");
		}
	}

	private void AddHoodedFigureBooks()
	{
		new List<Tuple<string, string>>
		{
			new Tuple<string, string>("HoodedFigure1_Failed", "book-barter-1"),
			new Tuple<string, string>("HoodedFigure2_Failed", "book-fishing-2"),
			new Tuple<string, string>("HoodedFigure3_Failed", "book-speed-2"),
			new Tuple<string, string>("HoodedFigure4_Failed", "book-equip-2")
		}.ForEach(delegate(Tuple<string, string> k)
		{
			if (this.visitedNodes.Contains(k.Item1) && this.ownedNonSpatialItems.Find((NonSpatialItemInstance i) => i.id == k.Item2) == null)
			{
				GameManager.Instance.ItemManager.AddItemById(k.Item2, null, true);
			}
		});
	}

	private void CheckForResearchProgress()
	{
		if (!this.GetBoolVariable("has-spent-research", false))
		{
			if (this.intVariables.Keys.Any((string k) => k.Contains("research-progress")))
			{
				this.SetBoolVariable("has-spent-research", true);
			}
		}
	}

	private void ApplyMissingDredgeFixes()
	{
		if (this.historyOfItemsOwned.Contains("dredge1") && this.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.DREDGE).Count == 0)
		{
			SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>("dredge1");
			if (itemDataById)
			{
				SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemDataById);
				Vector3Int vector3Int;
				if (spatialItemInstance != null && this.Inventory.FindPositionForObject(itemDataById, out vector3Int, 0, false))
				{
					this.Inventory.AddObjectToGridData(spatialItemInstance, vector3Int, false, null);
				}
			}
		}
		this.Storage.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.DREDGE).ForEach(delegate(SpatialItemInstance d)
		{
			this.Storage.RemoveObjectFromGridData(d, false);
		});
	}

	public SpatialItemInstance EquippedTrawlNetInstance()
	{
		List<SpatialItemInstance> allItemsOfType = this.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NET);
		if (allItemsOfType.Count > 0)
		{
			return allItemsOfType[0];
		}
		return null;
	}

	public List<SpatialItemData> GetResearchedItemData()
	{
		List<SpatialItemData> researchedItemData = new List<SpatialItemData>();
		this.itemIdsResearched.ForEach(delegate(string id)
		{
			SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>(id);
			if (itemDataById != null)
			{
				researchedItemData.Add(itemDataById);
			}
		});
		return researchedItemData;
	}

	public List<UpgradeData> GetOwnedUpgrades()
	{
		return this.upgradeIdsOwned.Select((string u) => GameManager.Instance.UpgradeManager.GetUpgradeDataById(u)).ToList<UpgradeData>();
	}

	public int GetCaughtCountById(string id)
	{
		int num = 0;
		if (this.caughtFishCounts.ContainsKey(id))
		{
			num = this.caughtFishCounts[id];
		}
		return num;
	}

	public void IncrementCaughtCounterById(string id)
	{
		if (this.caughtFishCounts.ContainsKey(id))
		{
			int num = this.caughtFishCounts[id];
			num++;
			this.caughtFishCounts[id] = num;
			return;
		}
		this.caughtFishCounts.Add(id, 1);
	}

	public void RecordFishSize(FishItemData fishItemData, float size)
	{
		float largestFishRecordById = this.GetLargestFishRecordById(fishItemData.id);
		if (size > largestFishRecordById)
		{
			if (this.harvestedItemSizeRecords.ContainsKey(fishItemData.id))
			{
				this.harvestedItemSizeRecords[fishItemData.id] = size;
				return;
			}
			this.harvestedItemSizeRecords.Add(fishItemData.id, size);
		}
	}

	public float GetLargestFishRecordById(string id)
	{
		float num = 0f;
		if (this.harvestedItemSizeRecords.ContainsKey(id))
		{
			num = this.harvestedItemSizeRecords[id];
		}
		return num;
	}

	public int AdjustResearchProgress(SpatialItemData spatialItemData, int change = 1)
	{
		int num = this.AdjustIntVariable("research-progress-" + spatialItemData.id, change);
		this.SetBoolVariable("has-spent-research", true);
		if (num >= spatialItemData.ResearchPointsRequired)
		{
			this.itemIdsResearched.Add(spatialItemData.id);
			GameEvents.Instance.TriggerResearchCompleted(spatialItemData);
		}
		return num;
	}

	public int AdjustResearchProgress(RecipeData recipeData, int change = 1)
	{
		int num = this.AdjustIntVariable("research-progress-" + recipeData.recipeId, change);
		this.SetBoolVariable("has-spent-research", true);
		if (num >= recipeData.researchRequired)
		{
			this.itemIdsResearched.Add(recipeData.recipeId);
			if (recipeData is ItemRecipeData)
			{
				GameEvents.Instance.TriggerResearchCompleted((recipeData as ItemRecipeData).itemProduced);
			}
		}
		return num;
	}

	public int GetResearchProgress(SpatialItemData itemData)
	{
		return this.GetIntVariable("research-progress-" + itemData.id, 0);
	}

	public int GetResearchProgress(RecipeData itemRecipeData)
	{
		return this.GetIntVariable("research-progress-" + itemRecipeData.recipeId, 0);
	}

	public int AdjustIntVariable(string key, int changeVal)
	{
		int intVariable = this.GetIntVariable(key, 0);
		int num = intVariable + changeVal;
		this.SetIntVariable(key, intVariable + changeVal);
		return num;
	}

	public void SetIntVariable(string key, int val)
	{
		this.intVariables[key] = val;
	}

	public int GetIntVariable(string key, int defaultValue = 0)
	{
		int num;
		if (!this.intVariables.TryGetValue(key, out num))
		{
			return defaultValue;
		}
		return num;
	}

	public void AdjustFloatVariable(string key, float changeVal)
	{
		float floatVariable = this.GetFloatVariable(key, 0f);
		this.SetFloatVariable(key, floatVariable + changeVal);
	}

	public void SetFloatVariable(string key, float val)
	{
		this.floatVariables[key] = val;
	}

	public float GetFloatVariable(string key, float defaultValue = 0f)
	{
		float num;
		if (!this.floatVariables.TryGetValue(key, out num))
		{
			return defaultValue;
		}
		return num;
	}

	public void AdjustDecimalVariable(string key, decimal changeVal)
	{
		decimal decimalVariable = this.GetDecimalVariable(key, 0m);
		this.SetDecimalVariable(key, decimalVariable + changeVal);
	}

	public void SetDecimalVariable(string key, decimal val)
	{
		this.decimalVariables[key] = val;
	}

	public decimal GetDecimalVariable(string key, decimal defaultValue = 0m)
	{
		decimal num;
		if (!this.decimalVariables.TryGetValue(key, out num))
		{
			return defaultValue;
		}
		return num;
	}

	public void SetStringVariable(string key, string val)
	{
		this.stringVariables[key] = val;
	}

	public string GetStringVariable(string key, string defaultValue = "")
	{
		string text;
		if (!this.stringVariables.TryGetValue(key, out text))
		{
			return defaultValue;
		}
		return text;
	}

	public void SetBoolVariable(string key, bool val)
	{
		this.boolVariables[key] = val;
	}

	public bool GetBoolVariable(string key, bool defaultValue = false)
	{
		bool flag;
		if (!this.boolVariables.TryGetValue(key, out flag))
		{
			return defaultValue;
		}
		return flag;
	}

	public void StampSaveTime()
	{
		this.lastSavedTime = TimeHelper.GetEpochTimeNow();
	}

	public void StampEntitlements()
	{
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1))
		{
			this.SetSaveUsesEntitlement(Entitlement.DLC_1, true);
		}
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2))
		{
			this.SetSaveUsesEntitlement(Entitlement.DLC_2, true);
		}
	}

	public bool GetIsItemResearched(SpatialItemData itemData)
	{
		return this.itemIdsResearched.Contains(itemData.id);
	}

	public bool GetIsItemResearched(RecipeData itemRecipeData)
	{
		return this.itemIdsResearched.Contains(itemRecipeData.recipeId);
	}

	public bool GetIsUpgradeOwned(UpgradeData upgradeData)
	{
		return this.upgradeIdsOwned.Contains(upgradeData.id);
	}

	public void SetUpgradeOwned(UpgradeData upgradeData, bool owned)
	{
		if (owned && !this.upgradeIdsOwned.Contains(upgradeData.id))
		{
			this.upgradeIdsOwned.Add(upgradeData.id);
			GameEvents.Instance.TriggerUpgradesChanged(upgradeData);
			return;
		}
		if (!owned)
		{
			this.upgradeIdsOwned.Remove(upgradeData.id);
			GameEvents.Instance.TriggerUpgradesChanged(upgradeData);
		}
	}

	public bool GetIsBuildingConstructed(BuildingTierId tierId)
	{
		string text = string.Format("{0}-{1}", tierId, this.buildingTierConstructionSuffix);
		return this.GetBoolVariable(text, false);
	}

	public void SetIsBuildingConstructed(BuildingTierId tierId, bool isConstructed)
	{
		string text = string.Format("{0}-{1}", tierId, this.buildingTierConstructionSuffix);
		this.SetBoolVariable(text, isConstructed);
	}

	public float GetOozePatchFillAmount(string oozePatchId)
	{
		string text = "ooze-amount-" + oozePatchId;
		return this.GetFloatVariable(text, 1f);
	}

	public void SetOozePatchFillAmount(string oozePatchId, float fillAmount)
	{
		string text = "ooze-amount-" + oozePatchId;
		this.SetFloatVariable(text, fillAmount);
	}

	public SerializableGrid Inventory
	{
		get
		{
			return this.grids[GridKey.INVENTORY];
		}
	}

	public SerializableGrid TrawlNet
	{
		get
		{
			return this.grids[this.GetCurrentTrawlNetGridKey()];
		}
	}

	public SerializableGrid Storage
	{
		get
		{
			return this.grids[GridKey.STORAGE];
		}
	}

	public SerializableGrid OverflowStorage
	{
		get
		{
			return this.grids[GridKey.OVERFLOW_STORAGE];
		}
	}

	public GridKey GetCurrentTrawlNetGridKey()
	{
		GridKey gridKey = GridKey.TRAWL_NET;
		SpatialItemInstance spatialItemInstance = this.EquippedTrawlNetInstance();
		if (spatialItemInstance != null)
		{
			gridKey = spatialItemInstance.GetItemData<DeployableItemData>().GridKey;
		}
		return gridKey;
	}

	public SerializableGrid GetGridByKey(GridKey key)
	{
		if (this.grids.ContainsKey(key))
		{
			return this.grids[key];
		}
		return null;
	}

	public int GetNumberOfDamagedSlots()
	{
		return this.Inventory.spatialUnderlayItems.FindAll((SpatialItemInstance spatialItemInstance) => spatialItemInstance.id == "dmg").Count;
	}

	public bool HasUnseenNonSpatialCabinItems()
	{
		return this.ownedNonSpatialItems.Find((NonSpatialItemInstance i) => i.isNew && i.GetItemData<NonSpatialItemData>().showInCabin) != null;
	}

	public bool HasUnseenNonSpatialItems()
	{
		return this.ownedNonSpatialItems.Find((NonSpatialItemInstance i) => i.isNew) != null;
	}

	public void AddMapMarker(string mapMarkerName, bool seen = false)
	{
		MapMarkerData mapMarkerData = GameManager.Instance.DataLoader.allMapMarkers.Find((MapMarkerData m) => m.name == mapMarkerName);
		if (mapMarkerData)
		{
			this.AddMapMarker(mapMarkerData, seen);
		}
	}

	public void AddMapMarker(MapMarkerData mapMarkerData, bool seen = false)
	{
		SerializedMapMarker newMarker = new SerializedMapMarker();
		newMarker.id = mapMarkerData.name;
		newMarker.seen = seen;
		if (this.mapMarkers.Find((SerializedMapMarker m) => m.id == newMarker.id) == null)
		{
			this.mapMarkers.Add(newMarker);
		}
		if (!newMarker.seen)
		{
			GameEvents.Instance.TriggerHasUnseenItemsChanged();
		}
	}

	public void RemoveMapMarker(MapMarkerData mapMarkerData)
	{
		this.RemoveMapMarker(mapMarkerData.name);
	}

	public void RemoveMapMarker(string id)
	{
		SerializedMapMarker serializedMapMarker = this.mapMarkers.Find((SerializedMapMarker m) => m.id == id);
		bool flag = serializedMapMarker != null && !serializedMapMarker.seen;
		this.mapMarkers.RemoveAll((SerializedMapMarker m) => m.id == id);
		if (flag)
		{
			GameEvents.Instance.TriggerHasUnseenItemsChanged();
		}
	}

	public void AddHarvestPOIMarker(string id)
	{
		if (!this.harvestPOIMapMarkers.Contains(id))
		{
			this.harvestPOIMapMarkers.Add(id);
		}
	}

	public void RemoveHarvestPOIMarker(string id)
	{
		this.harvestPOIMapMarkers.Remove(id);
	}

	public void RemoveAllHarvestPOIMarkers()
	{
		this.harvestPOIMapMarkers = new List<string>();
	}

	public List<ResearchableItemInstance> GetResearchableItemInstances(bool completedOnly)
	{
		List<ResearchableItemInstance> results = new List<ResearchableItemInstance>();
		this.ownedNonSpatialItems.ForEach(delegate(NonSpatialItemInstance i)
		{
			if (i is ResearchableItemInstance && (!completedOnly || (completedOnly && (i as ResearchableItemInstance).IsResearchComplete)))
			{
				results.Add(i as ResearchableItemInstance);
			}
		});
		return results;
	}

	public List<NonSpatialItemInstance> GetNonSpatialItemsBySubtype(ItemSubtype itemSubtype)
	{
		return this.ownedNonSpatialItems.Where((NonSpatialItemInstance n) => n.GetItemData<NonSpatialItemData>().itemSubtype == itemSubtype).ToList<NonSpatialItemInstance>();
	}

	public List<NonSpatialItemInstance> GetMessages()
	{
		return this.ownedNonSpatialItems.Where((NonSpatialItemInstance n) => n.GetItemData<NonSpatialItemData>() is MessageItemData).ToList<NonSpatialItemInstance>();
	}

	public ResearchableItemInstance GetActiveResearchItem()
	{
		ResearchableItemInstance itemInstance = null;
		this.ownedNonSpatialItems.ForEach(delegate(NonSpatialItemInstance item)
		{
			if (itemInstance == null && item is ResearchableItemInstance && (item as ResearchableItemInstance).isActive)
			{
				itemInstance = item as ResearchableItemInstance;
			}
		});
		return itemInstance;
	}

	public bool AddNonSpatialItemInstance(NonSpatialItemInstance itemInstance)
	{
		bool flag = false;
		if (this.ownedNonSpatialItems.FindIndex((NonSpatialItemInstance x) => x.id == itemInstance.id) == -1)
		{
			flag = true;
			this.ownedNonSpatialItems.Add(itemInstance);
			if (itemInstance.isNew && GameEvents.Instance)
			{
				GameEvents.Instance.TriggerHasUnseenItemsChanged();
			}
		}
		return flag;
	}

	public SerializedShopHistory GetShopHistoryById(string shopId)
	{
		return this.shopHistories.Find((SerializedShopHistory h) => h.id == shopId);
	}

	public decimal GetShopTransactionTotalById(string shopId)
	{
		decimal num = 0m;
		SerializedShopHistory serializedShopHistory = this.shopHistories.Find((SerializedShopHistory h) => h.id == shopId);
		if (serializedShopHistory != null)
		{
			num = serializedShopHistory.totalTransactionValue;
		}
		return num;
	}

	public void RecordShopVisit(string shopId, int day)
	{
		SerializedShopHistory serializedShopHistory = this.GetShopHistoryById(shopId);
		if (serializedShopHistory == null)
		{
			serializedShopHistory = new SerializedShopHistory
			{
				id = shopId
			};
			this.shopHistories.Add(serializedShopHistory);
		}
		serializedShopHistory.visits++;
		if (!serializedShopHistory.visitDays.Contains(day))
		{
			serializedShopHistory.visitDays.Add(day);
		}
	}

	public void RecordShopTransaction(string shopId, int day, decimal totalTransactionValue)
	{
		SerializedShopHistory serializedShopHistory = this.GetShopHistoryById(shopId);
		if (serializedShopHistory == null)
		{
			serializedShopHistory = new SerializedShopHistory
			{
				id = shopId
			};
			this.shopHistories.Add(serializedShopHistory);
		}
		serializedShopHistory.totalTransactionValue += totalTransactionValue;
		if (!serializedShopHistory.transactionDays.Contains(day))
		{
			serializedShopHistory.transactionDays.Add(day);
		}
	}

	public SerializedItemTransaction GetItemTransactionById(string itemId)
	{
		return this.itemTransactions.Find((SerializedItemTransaction i) => i.itemId == itemId);
	}

	public NonSpatialItemInstance GetNonSpatialItemInstance(string itemId)
	{
		return this.ownedNonSpatialItems.FirstOrDefault((NonSpatialItemInstance i) => i.id == itemId);
	}

	public void RecordItemTransaction(string itemId, bool sold)
	{
		SerializedItemTransaction serializedItemTransaction = this.GetItemTransactionById(itemId);
		if (serializedItemTransaction == null)
		{
			serializedItemTransaction = new SerializedItemTransaction
			{
				itemId = itemId
			};
			this.itemTransactions.Add(serializedItemTransaction);
		}
		if (sold)
		{
			serializedItemTransaction.sold++;
			return;
		}
		serializedItemTransaction.bought++;
	}

	public void RemoveTemporalMarker(string id)
	{
		SerializedTemporalMarker temporalMarkerById = this.GetTemporalMarkerById(id);
		if (temporalMarkerById != null)
		{
			this.temporalMarkers.Remove(temporalMarkerById);
		}
	}

	public SerializedTemporalMarker GetTemporalMarkerById(string id)
	{
		return this.temporalMarkers.Find((SerializedTemporalMarker t) => t.id == id);
	}

	public void RecordTemporalMarker(string id, float timeAndDay, bool replace)
	{
		SerializedTemporalMarker serializedTemporalMarker = this.GetTemporalMarkerById(id);
		if (serializedTemporalMarker == null)
		{
			serializedTemporalMarker = new SerializedTemporalMarker
			{
				id = id,
				timeAndDay = timeAndDay
			};
			this.temporalMarkers.Add(serializedTemporalMarker);
			return;
		}
		if (replace)
		{
			serializedTemporalMarker.timeAndDay = timeAndDay;
		}
	}

	public bool HasItemsOfSubtypeInInventory(ItemSubtype itemSubtype, bool allowDamaged, bool allowExhausted)
	{
		List<SpatialItemInstance> spatialItems = this.Inventory.spatialItems;
		for (int i = 0; i < spatialItems.Count; i++)
		{
			SpatialItemData itemData = spatialItems[i].GetItemData<SpatialItemData>();
			if (itemData.itemSubtype == itemSubtype && (allowDamaged || itemData.damageMode != DamageMode.OPERATION || !spatialItems[i].GetIsOnDamagedCell()) && (allowExhausted || itemData.damageMode != DamageMode.DURABILITY || spatialItems[i].durability > 0f))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyOfTheseItemsInInventory(string[] itemIds, bool allowDamaged)
	{
		return this.GetItemInstanceById<SpatialItemInstance>(itemIds, allowDamaged, this.Inventory) != null;
	}

	public void SetAbilityUnseen(AbilityData abilityData, bool unseen)
	{
		this.SetBoolVariable(abilityData.name.ToLowerInvariant() + "-unseen", unseen);
	}

	public bool GetAbilityUnseen(AbilityData abilityData)
	{
		return this.GetBoolVariable(abilityData.name.ToLowerInvariant() + "-unseen", false);
	}

	public T GetItemInstanceById<T>(string[] itemIds, bool allowDamaged, SerializableGrid grid) where T : SpatialItemInstance
	{
		List<SpatialItemInstance> spatialItems = grid.spatialItems;
		for (int i = 0; i < spatialItems.Count; i++)
		{
			if (itemIds.Contains(spatialItems[i].id) && (allowDamaged || !spatialItems[i].GetIsOnDamagedCell()))
			{
				return spatialItems[i] as T;
			}
		}
		return default(T);
	}

	public int GetNumItemInGridById(string id, SerializableGrid grid)
	{
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id);
		if (grid == null)
		{
			return 0;
		}
		if (itemDataById == null)
		{
			CustomDebug.EditorLogError("[SaveData] GetNumItemInGridById(" + id + ") could not find ItemData with that id.");
			return 0;
		}
		if (itemDataById is SpatialItemData)
		{
			return grid.spatialItems.FindAll((SpatialItemInstance i) => i.id == id).Count;
		}
		return 0;
	}

	public void SetHasSeenItem(string id, bool seen)
	{
		this.SetBoolVariable("seen-" + id, seen);
	}

	public bool GetHasSeenItem(string id)
	{
		return this.GetBoolVariable("seen-" + id, false);
	}

	public void SetHasSpiedHarvestCategory(HarvestableType type, bool seen)
	{
		string text = type.ToString().ToLower(new CultureInfo("en-US", false));
		this.SetBoolVariable("spied-" + text, seen);
	}

	public bool GetSaveUsesEntitlement(Entitlement entitlement)
	{
		string text = string.Format("{0}-{1}", SaveData.VAR_ENTITLEMENT_PREFIX, (int)entitlement);
		return this.GetBoolVariable(text, false);
	}

	public void SetSaveUsesEntitlement(Entitlement entitlement, bool value)
	{
		string text = string.Format("{0}-{1}", SaveData.VAR_ENTITLEMENT_PREFIX, (int)entitlement);
		this.SetBoolVariable(text, value);
	}

	public bool GetHasTeleportAnchor()
	{
		return this.GetBoolVariable(SaveData.VAR_HAS_PLACED_TELEPORT_ANCHOR, false);
	}

	public void SetHasTeleportAnchor(bool value)
	{
		this.SetBoolVariable(SaveData.VAR_HAS_PLACED_TELEPORT_ANCHOR, value);
	}

	public Vector3 GetTeleportAnchorPosition()
	{
		return new Vector3(this.GetFloatVariable(SaveData.VAR_TELEPORT_ANCHOR_X, 0f), 0f, this.GetFloatVariable(SaveData.VAR_TELEPORT_ANCHOR_Z, 0f));
	}

	public void SetTeleportAnchorPosition(float x, float z)
	{
		this.SetFloatVariable(SaveData.VAR_TELEPORT_ANCHOR_X, x);
		this.SetFloatVariable(SaveData.VAR_TELEPORT_ANCHOR_Z, z);
	}

	public bool GetIsIcebreakerEquipped()
	{
		return this.GetBoolVariable(BoatSubModelToggler.ICEBREAKER_EQUIP_STRING_KEY, false);
	}

	public int GetTotalUserPlacedMapMarkers()
	{
		return this.harvestPOIMapMarkers.Count + this.mapStamps.Count;
	}

	[NonSerialized]
	public const string VAR_TIME = "time";

	[NonSerialized]
	private const string VAR_SANITY = "sanity";

	[NonSerialized]
	private const string VAR_FUNDS = "funds";

	[NonSerialized]
	private const string VAR_FISH_SALE_TOTAL = "fish-sale-total";

	[NonSerialized]
	private const string VAR_TRINKET_SALE_TOTAL = "trinket-sale-total";

	[NonSerialized]
	private const string VAR_FISH_DISCARD_COUNT = "fish-discard-count";

	[NonSerialized]
	private const string VAR_DEPLETED_SPOT_COUNT = "fish-depleted-count";

	[NonSerialized]
	private const string VAR_GREATER_MARROW_REPAYMENTS = "gm-repayments";

	[NonSerialized]
	private const string VAR_WEATHER = "weather";

	[NonSerialized]
	private const string VAR_WEATHER_TIME = "weather-time";

	[NonSerialized]
	private const string VAR_HULL_TIER_NAME = "hull-tier";

	[NonSerialized]
	private const string VAR_WORLD_PHASE = "world-phase";

	[NonSerialized]
	private const string VAR_TPR_WORLD_PHASE = "tpr-world-phase";

	[NonSerialized]
	private const string VAR_TIR_WORLD_PHASE = "tir-world-phase";

	[NonSerialized]
	private const string VAR_ABERRATION_SPAWN_MODIFIER = "aberration-spawn-modifier";

	[NonSerialized]
	private const string VAR_CAN_CATCH_ABERRATIONS = "can-catch-aberrations";

	[NonSerialized]
	private const string VAR_NUM_ABERRATIONS_CAUGHT = "num-aberrations-caught";

	[NonSerialized]
	private const string VAR_HAS_CAUGHT_ABERRATION_FROM_SPECIAL_SPOT = "aberration-special-spot";

	[NonSerialized]
	private const string VAR_ROD_FISH_CAUGHT = "rod-fish-caught";

	[NonSerialized]
	private const string VAR_NET_FISH_CAUGHT = "net-fish-caught";

	[NonSerialized]
	private const string VAR_POT_CRABS_CAUGHT = "pot-crabs-caught";

	[NonSerialized]
	private const string VAR_RESEARCH_PREFIX = "research-progress";

	[NonSerialized]
	public const string VAR_HAS_SPENT_RESEARCH = "has-spent-research";

	[NonSerialized]
	public const string VAR_USED_SIDE_SWITCHERS = "has-used-side-switchers";

	[NonSerialized]
	private const string VAR_INFECT_TIME = "last-infect-time";

	[NonSerialized]
	private const string VAR_LAST_UNSEEN_CAUGHT_SPECIES = "last-unseen-caught-species";

	[NonSerialized]
	private static string VAR_FISH_UNTIL_NEXT_TROPHY_NOTCH = "fish-before-next-trophy-notch";

	[NonSerialized]
	private static string VAR_BANISH_MACHINE_EXPIRY = "banish-machine-expiry";

	[NonSerialized]
	private static string VAR_VIEWED_INTRO_CUTSCENE = "viewed-intro-cutscene";

	[NonSerialized]
	private static string VAR_THREATS_BANISHED = "threats-banished";

	[NonSerialized]
	private static string VAR_CAN_SHOW_FINALE_HIJACK = "can-show-finale-hijack";

	[NonSerialized]
	private static string VAR_UPGRADE_BOUGHT = "upgrade-bought";

	[NonSerialized]
	private static string VAR_LAST_SELECTED_ABILITY = "last-selected-ability";

	[NonSerialized]
	private static string VAR_CAN_USE_STORAGE_TRAY = "can-use-storage-tray";

	[NonSerialized]
	private static string VAR_ROOF_COLOR_INDEX = "roof-color-index";

	[NonSerialized]
	private static string VAR_HULL_COLOR_INDEX = "hull-color-index";

	[NonSerialized]
	private static string VAR_HAS_CHANGED_BOAT_COLORS = "has-changed-boat-colors";

	[NonSerialized]
	private static string VAR_IS_BOAT_BUNTING_ENABLED = "is-boat-bunting-enabled";

	[NonSerialized]
	private static string VAR_BOAT_FLAG_STYLE = "boat-flag-style";

	[NonSerialized]
	private static string VAR_ENTITLEMENT_PREFIX = "using-entitlement";

	[NonSerialized]
	private static string VAR_ICE_MONSTER_TIME_UNTIL_SPAWN = "ice-monster-time-until-spawn";

	[NonSerialized]
	private static string VAR_HAS_PLACED_TELEPORT_ANCHOR = "has-placed-teleport-anchor";

	[NonSerialized]
	private static string VAR_TELEPORT_ANCHOR_X = "teleport-anchor-x";

	[NonSerialized]
	private static string VAR_TELEPORT_ANCHOR_Z = "teleport-anchor-z";

	[NonSerialized]
	private const string VAR_OOZE_PATCH_FILL_PREFIX = "ooze-amount";

	[SerializeField]
	public long lastSavedTime;

	[SerializeField]
	public int version;

	[SerializeField]
	public string dockId = "";

	[SerializeField]
	public int dockSlotIndex;

	[SerializeField]
	public Dictionary<GridKey, SerializableGrid> grids;

	[SerializeField]
	public Dictionary<string, decimal> decimalVariables;

	[SerializeField]
	public Dictionary<string, int> intVariables;

	[SerializeField]
	public Dictionary<string, float> floatVariables;

	[SerializeField]
	public Dictionary<string, string> stringVariables;

	[SerializeField]
	public Dictionary<string, bool> boolVariables;

	[SerializeField]
	public Dictionary<string, float> eventHistory;

	[SerializeField]
	public HashSet<string> visitedNodes;

	[SerializeField]
	public List<NonSpatialItemInstance> ownedNonSpatialItems;

	[SerializeField]
	public Dictionary<string, SerializedQuestEntry> questEntries;

	[SerializeField]
	public List<SerializedTemporalMarker> temporalMarkers;

	[SerializeField]
	public List<SerializedItemTransaction> itemTransactions;

	[SerializeField]
	public List<SerializedShopHistory> shopHistories;

	[SerializeField]
	public HashSet<string> availableDestinations;

	[SerializeField]
	public HashSet<string> availableSpeakers;

	[SerializeField]
	public HashSet<string> historyOfItemsOwned;

	[SerializeField]
	public Dictionary<string, float> harvestSpotStocks;

	[SerializeField]
	private List<string> upgradeIdsOwned;

	[SerializeField]
	private List<string> itemIdsResearched;

	[SerializeField]
	public List<string> unlockedAbilities;

	[SerializeField]
	public Dictionary<string, float> abilityHistory;

	[SerializeField]
	public Dictionary<string, bool> abilityToggleStates;

	[SerializeField]
	public Dictionary<string, int> caughtFishCounts;

	[SerializeField]
	public Dictionary<string, float> harvestedItemSizeRecords;

	[SerializeField]
	public List<SerializedMapMarker> mapMarkers;

	[SerializeField]
	public List<SerializedCrabPotPOIData> serializedCrabPotPOIs;

	[SerializeField]
	public List<SerializedMapStamp> mapStamps;

	[SerializeField]
	public List<string> harvestPOIMapMarkers;

	[SerializeField]
	public bool completedActivityPS5;

	private string buildingTierConstructionSuffix = "is-constructed";
}
