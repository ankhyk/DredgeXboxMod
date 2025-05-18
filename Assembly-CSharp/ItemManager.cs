using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemManager : MonoBehaviour, ILoader
{
	public float DebugNextFishSize { get; set; }

	public bool HasLoaded()
	{
		return this._hasLoadedItems;
	}

	private void Awake()
	{
		this.DebugNextFishSize = -1f;
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Combine(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Combine(instance2.OnGameEnded, new Action(this.OnGameEnded));
	}

	private void OnDisable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Remove(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Remove(instance2.OnGameEnded, new Action(this.OnGameEnded));
	}

	public void Load()
	{
		Addressables.LoadAssetsAsync<ItemData>(this.itemDataAssetLabelReference, null).Completed += this.OnItemDataAddressablesLoaded;
	}

	private void OnItemDataAddressablesLoaded(AsyncOperationHandle<IList<ItemData>> handle)
	{
		foreach (ItemData itemData in handle.Result)
		{
			this.allItems.Add(itemData);
		}
		this._hasLoadedItems = true;
	}

	public T GetItemDataById<T>(string id) where T : ItemData
	{
		ItemData itemData = this.allItems.Find((ItemData od) => od.id == id);
		itemData;
		return itemData as T;
	}

	public List<T> GetAllItemsOfType<T>() where T : ItemData
	{
		return this.allItems.OfType<T>().ToList<T>();
	}

	public List<SpatialItemData> GetSpatialItemDataBySubtype(ItemSubtype itemSubtype)
	{
		return (from od in this.allItems.OfType<SpatialItemData>()
			where od.itemSubtype == itemSubtype
			select od).ToList<SpatialItemData>();
	}

	public void OnGameStarted()
	{
		this.coroutineRunners.ForEach(delegate(GameObject go)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(go);
		});
		GameEvents.Instance.OnItemDestroyed += new Action<SpatialItemData, bool>(this.OnItemDestroyed);
		this.AddTerminalCommands();
	}

	public void OnGameEnded()
	{
		this.RemoveTerminalCommands();
		this.allItems.Clear();
		this._hasLoadedItems = false;
		GameEvents.Instance.OnItemDestroyed -= new Action<SpatialItemData, bool>(this.OnItemDestroyed);
	}

	private void OnItemDestroyed(ItemData itemData, bool playerDestroyed)
	{
		if (itemData is HarvestableItemData && (itemData as HarvestableItemData).regenHarvestSpotOnDestroy)
		{
			base.StartCoroutine(this.FindAndRegenHarvestSpot(itemData));
		}
	}

	public decimal GetItemValue(SpatialItemInstance itemInstance, ItemManager.BuySellMode mode, float sellValueModifier = 1f)
	{
		SpatialItemData itemData = itemInstance.GetItemData<SpatialItemData>();
		if (itemData.itemSubtype == ItemSubtype.FISH)
		{
			FishItemInstance fishItemInstance = itemInstance as FishItemInstance;
			decimal num = itemData.value * (decimal)Mathf.Max(1f, sellValueModifier) * fishItemInstance.GetSaleModifierForSize() * fishItemInstance.GetSaleModifierForFreshness();
			if (mode == ItemManager.BuySellMode.BUY)
			{
				num *= (decimal)(2f - GameManager.Instance.PlayerStats.ResearchedBarteringModifier);
			}
			else
			{
				num *= (decimal)GameManager.Instance.PlayerStats.ResearchedBarteringModifier;
				num *= 2m;
			}
			return decimal.Round(num, 2);
		}
		decimal num2 = 0m;
		if (mode == ItemManager.BuySellMode.BUY)
		{
			num2 = itemData.value;
			num2 *= (decimal)(2f - GameManager.Instance.PlayerStats.ResearchedBarteringModifier);
		}
		else
		{
			num2 = (itemData.hasSellOverride ? itemData.sellOverrideValue : itemData.value);
			num2 *= (decimal)sellValueModifier;
			num2 *= (decimal)GameManager.Instance.PlayerStats.ResearchedBarteringModifier;
		}
		if (itemData.damageMode == DamageMode.DURABILITY)
		{
			float num3 = itemInstance.durability / itemInstance.GetItemData<DeployableItemData>().MaxDurabilityDays;
			num3 = Mathf.Clamp(num3, 0.1f, 1f);
			num2 *= (decimal)num3;
		}
		return decimal.Round((decimal)Mathf.Clamp((float)num2, 0f, (float)num2), 2);
	}

	public void SellItems(List<SpatialItemInstance> items, float valueMultiplier = 1f, bool tryRemoveFromGrids = true)
	{
		decimal totalIncome = 0m;
		items.ForEach(delegate(SpatialItemInstance itemInstance)
		{
			totalIncome += GameManager.Instance.ItemManager.GetItemValue(itemInstance, ItemManager.BuySellMode.SELL, valueMultiplier);
			GameManager.Instance.ItemManager.SetItemSeen(itemInstance);
			if (tryRemoveFromGrids && GameManager.Instance.SaveData.Inventory.spatialItems.Contains(itemInstance))
			{
				GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(itemInstance, true);
			}
			else if (tryRemoveFromGrids && GameManager.Instance.SaveData.TrawlNet.spatialItems.Contains(itemInstance))
			{
				GameManager.Instance.SaveData.TrawlNet.RemoveObjectFromGridData(itemInstance, true);
			}
			GameManager.Instance.SaveData.RecordItemTransaction(itemInstance.id, true);
			GameEvents.Instance.TriggerItemSold(itemInstance.GetItemData<SpatialItemData>());
		});
		if (items.Count > 1)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_GAINED, "notification.sell-fish-bulk", new object[]
			{
				items.Count,
				string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
					">+$",
					totalIncome.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
					"</color>"
				})
			});
		}
		else
		{
			GameManager.Instance.UI.PrepareItemNameForSellNotification(NotificationType.MONEY_GAINED, items[0].GetItemData<SpatialItemData>().itemNameKey, totalIncome, 0m, false);
		}
		GameEvents.Instance.TriggerItemInventoryChanged(null);
		GameManager.Instance.AddFunds(totalIncome);
	}

	private IEnumerator FindAndRegenHarvestSpot(ItemData itemData)
	{
		List<HarvestPOI> list = (from harvestPOI in global::UnityEngine.Object.FindObjectsOfType<HarvestPOI>(true).ToList<HarvestPOI>()
			where harvestPOI.Harvestable.ContainsItem(itemData)
			select harvestPOI).ToList<HarvestPOI>();
		HarvestPOI targetPOI = null;
		list.ForEach(delegate(HarvestPOI p)
		{
			if (targetPOI == null || (targetPOI != null && p.Stock < targetPOI.Stock))
			{
				targetPOI = p;
			}
		});
		if (targetPOI)
		{
			targetPOI.AddStock(1f, true);
		}
		yield return null;
		yield break;
	}

	public void SetActiveResearchableItem(ItemInstance itemInstance)
	{
		GameManager.Instance.SaveData.GetResearchableItemInstances(false).ForEach(delegate(ResearchableItemInstance i)
		{
			i.isActive = i == itemInstance;
		});
	}

	public float GetInstallTimeForItem(SpatialItemData itemData)
	{
		return GameManager.Instance.GameConfigData.EquipmentInstallTimePerSquare * (float)itemData.GetSize();
	}

	public void MarkNonSpatialItemAsSeen(NonSpatialItemInstance itemInstance)
	{
		if (itemInstance != null)
		{
			itemInstance.isNew = false;
			GameManager.Instance.SaveData.HasUnseenNonSpatialCabinItems();
			GameEvents.Instance.TriggerHasUnseenItemsChanged();
		}
	}

	public void AddObjectToInventory(SpatialItemInstance spatialItemInstance, Vector3Int position, bool notify)
	{
		GameManager.Instance.SaveData.Inventory.AddObjectToGridData(spatialItemInstance, position, notify, null);
		this.SetItemSeen(spatialItemInstance);
	}

	private T CreateItem<T>(string itemId) where T : ItemInstance
	{
		ItemData itemDataById = this.GetItemDataById<ItemData>(itemId);
		if (itemDataById == null)
		{
			CustomDebug.EditorLogError("[ItemManager] CreateItem() invalid itemId '" + itemId + "'");
			return default(T);
		}
		if (itemDataById is FishItemData)
		{
			return this.CreateFishItem(itemId, FishAberrationGenerationMode.FORBID, false, FishSizeGenerationMode.ANY, 1f) as T;
		}
		return this.CreateItem<T>(itemDataById);
	}

	public FishItemInstance CreateFishItem(string itemId, FishAberrationGenerationMode aberrationGenerationMode, bool isSpecialSpot, FishSizeGenerationMode sizeGenerationMode, float aberrationBonusMultiplier = 1f)
	{
		if (aberrationGenerationMode == FishAberrationGenerationMode.RANDOM_CHANCE || aberrationGenerationMode == FishAberrationGenerationMode.FORCE)
		{
			bool flag = false;
			FishItemData itemDataById = this.GetItemDataById<FishItemData>(itemId);
			if (itemDataById && GameManager.Instance.SaveData.CanCatchAberrations && itemDataById.Aberrations.Count > 0 && GameManager.Instance.SaveData.GetCaughtCountById(itemDataById.id) > 0)
			{
				float num = 0f;
				if (aberrationGenerationMode == FishAberrationGenerationMode.RANDOM_CHANCE)
				{
					float num2 = (GameManager.Instance.Time.IsDaytime ? GameManager.Instance.GameConfigData.BaseAberrationSpawnChance : GameManager.Instance.GameConfigData.NightAberrationSpawnChance);
					float totalAberrationCatchModifier = GameManager.Instance.PlayerStats.TotalAberrationCatchModifier;
					num = Mathf.Min(GameManager.Instance.GameConfigData.MaxAberrationSpawnChance, num2 + (float)GameManager.Instance.SaveData.AberrationSpawnModifier + totalAberrationCatchModifier);
					float num3 = 0f;
					if (isSpecialSpot)
					{
						if (!GameManager.Instance.SaveData.HasCaughtAberrationAtSpecialSpot)
						{
							num3 = 1f;
						}
						else
						{
							num3 = GameManager.Instance.GameConfigData.SpecialSpotAberrationSpawnBonus;
						}
					}
					num *= aberrationBonusMultiplier;
					num += num3;
				}
				if (aberrationGenerationMode == FishAberrationGenerationMode.FORCE || global::UnityEngine.Random.value < num)
				{
					int worldPhase = GameManager.Instance.SaveData.WorldPhase;
					List<FishItemData> candidates = new List<FishItemData>();
					itemDataById.Aberrations.ForEach(delegate(FishItemData aberrationItemData)
					{
						if (worldPhase >= aberrationItemData.MinWorldPhaseRequired && GameManager.Instance.SaveData.GetCaughtCountById(aberrationItemData.id) == 0)
						{
							candidates.Add(aberrationItemData);
						}
					});
					if (candidates.Count == 0)
					{
						itemDataById.Aberrations.ForEach(delegate(FishItemData aberrationItemData)
						{
							if (worldPhase >= aberrationItemData.MinWorldPhaseRequired)
							{
								candidates.Add(aberrationItemData);
							}
						});
					}
					if (candidates.Count > 0)
					{
						FishItemData fishItemData = candidates[global::UnityEngine.Random.Range(0, candidates.Count)];
						if (fishItemData)
						{
							itemId = fishItemData.id;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				GameManager.Instance.SaveData.AberrationSpawnModifier = 0m;
				if (isSpecialSpot)
				{
					GameManager.Instance.SaveData.HasCaughtAberrationAtSpecialSpot = true;
				}
				SaveData saveData = GameManager.Instance.SaveData;
				int numAberrationsCaught = saveData.NumAberrationsCaught;
				saveData.NumAberrationsCaught = numAberrationsCaught + 1;
			}
			else
			{
				GameManager.Instance.SaveData.AberrationSpawnModifier += GameManager.Instance.GameConfigData.SpawnChanceIncreasePerNonAberrationCaught;
			}
		}
		float num4 = 0.5f;
		if (this.DebugNextFishSize == -1f)
		{
			if (sizeGenerationMode == FishSizeGenerationMode.ANY)
			{
				num4 = MathUtil.GetRandomGaussian(0f, 1f);
			}
			else if (sizeGenerationMode == FishSizeGenerationMode.FORCE_BIG_TROPHY)
			{
				num4 = global::UnityEngine.Random.Range(GameManager.Instance.GameConfigData.TrophyMaxSize, 1f);
			}
			else if (sizeGenerationMode == FishSizeGenerationMode.NO_BIG_TROPHY)
			{
				num4 = MathUtil.GetRandomGaussian(0f, GameManager.Instance.GameConfigData.TrophyMaxSize - 0.01f);
			}
		}
		else
		{
			num4 = this.DebugNextFishSize;
		}
		return new FishItemInstance
		{
			id = itemId,
			size = num4,
			freshness = GameManager.Instance.GameConfigData.MaxFreshness
		};
	}

	public T CreateItem<T>(ItemData itemData) where T : ItemInstance
	{
		ItemInstance itemInstance = null;
		if (itemData is SpatialItemData)
		{
			SpatialItemData spatialItemData = itemData as SpatialItemData;
			if (spatialItemData.itemType == ItemType.GENERAL && spatialItemData.itemSubtype == ItemSubtype.FISH)
			{
				itemInstance = new FishItemInstance
				{
					id = itemData.id,
					size = MathUtil.GetRandomGaussian(0f, 1f),
					freshness = GameManager.Instance.GameConfigData.MaxFreshness
				};
			}
			else
			{
				itemInstance = new SpatialItemInstance
				{
					id = itemData.id
				};
				if (spatialItemData.damageMode == DamageMode.DURABILITY)
				{
					(itemInstance as SpatialItemInstance).durability = (spatialItemData as DeployableItemData).MaxDurabilityDays;
				}
				if (spatialItemData is DurableItemData)
				{
					(itemInstance as SpatialItemInstance).durability = (spatialItemData as DurableItemData).MaxDurabilityDays;
				}
			}
		}
		else if (itemData is NonSpatialItemData)
		{
			if (itemData is ResearchableItemData)
			{
				itemInstance = new ResearchableItemInstance
				{
					id = itemData.id,
					isNew = true
				};
			}
			else
			{
				itemInstance = new NonSpatialItemInstance
				{
					id = itemData.id,
					isNew = true
				};
			}
		}
		return itemInstance as T;
	}

	public ItemInstance AddItemById(string itemId, SerializableGrid grid, bool dispatchEvent = true)
	{
		ItemData itemDataById = this.GetItemDataById<ItemData>(itemId);
		ItemInstance itemInstance = this.CreateItem<ItemInstance>(itemId);
		if (itemInstance == null)
		{
			CustomDebug.EditorLogError("[ItemManager] AddItemById() failed to add item '" + itemId + "'");
			return null;
		}
		if (itemDataById is SpatialItemData && grid != null)
		{
			Vector3Int vector3Int;
			if (grid.FindPositionForObject(itemDataById as SpatialItemData, out vector3Int, 0, false))
			{
				grid.AddObjectToGridData(itemInstance as SpatialItemInstance, vector3Int, dispatchEvent, null);
				this.SetItemSeen(itemInstance as SpatialItemInstance);
				GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_ADDED, "notification.item-added", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			}
		}
		else if (itemDataById is NonSpatialItemData)
		{
			bool flag = GameManager.Instance.SaveData.AddNonSpatialItemInstance(itemInstance as NonSpatialItemInstance);
			NotificationType notificationType = NotificationType.ITEM_ADDED;
			if (itemInstance is ResearchableItemInstance)
			{
				notificationType = NotificationType.BOOK_ADDED;
			}
			if (flag && GameManager.Instance.UI)
			{
				GameManager.Instance.UI.ShowNotificationWithItemName(notificationType, "notification.cabin-item-added", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			}
		}
		return itemInstance;
	}

	public void SetItemSeen(SpatialItemInstance spatialItemInstance)
	{
		if (spatialItemInstance.seen)
		{
			return;
		}
		spatialItemInstance.seen = true;
		SpatialItemData itemData = spatialItemInstance.GetItemData<SpatialItemData>();
		GameManager.Instance.SaveData.historyOfItemsOwned.Add(itemData.id);
		GameEvents.Instance.TriggerItemSeen(spatialItemInstance);
		if (spatialItemInstance != null && spatialItemInstance is FishItemInstance)
		{
			FishItemInstance fishItemInstance = spatialItemInstance as FishItemInstance;
			FishItemData itemData2 = spatialItemInstance.GetItemData<FishItemData>();
			GameManager.Instance.SaveData.IncrementCaughtCounterById(itemData2.id);
			GameManager.Instance.SaveData.RecordFishSize(itemData2, (spatialItemInstance as FishItemInstance).size);
			if (fishItemInstance.size >= GameManager.Instance.GameConfigData.TrophyMaxSize)
			{
				GameEvents.Instance.TriggerTrophyFishCaught(fishItemInstance);
			}
		}
	}

	public string GetFormattedDepthString(float depthRaw)
	{
		return ItemManager.GetFormattedDepthString(depthRaw, GameManager.Instance.GameConfigData);
	}

	public static string GetFormattedDepthString(float depthRaw, GameConfigData gameConfigData)
	{
		float num = depthRaw * gameConfigData.DepthModifier;
		bool flag = true;
		if (GameManager.Instance)
		{
			flag = GameManager.Instance.SettingsSaveData.units == 0;
		}
		if (flag)
		{
			return Math.Round((double)num, 1).ToString("n1", LocalizationSettings.SelectedLocale.Formatter) + "m";
		}
		int num2 = (int)(num * 3.281f);
		return string.Format("{0}ft", num2);
	}

	public string GetFormattedFishSizeString(float size, FishItemData fishItemData)
	{
		float num = Mathf.Lerp(fishItemData.MinSizeCentimeters, fishItemData.MaxSizeCentimeters, size);
		bool flag = true;
		if (GameManager.Instance)
		{
			flag = GameManager.Instance.SettingsSaveData.units == 0;
		}
		if (flag)
		{
			if (num > 100f)
			{
				return Math.Round((double)(num / 100f), 2).ToString("n2", LocalizationSettings.SelectedLocale.Formatter) + " m";
			}
			return Math.Round((double)num, 1).ToString("n2", LocalizationSettings.SelectedLocale.Formatter) + " cm";
		}
		else
		{
			float num2 = num / 2.54f;
			int num3 = (int)(num / 2.54f);
			int num4 = (num3 - num3 % 12) / 12;
			int num5 = num3 % 12;
			if (num4 > 0)
			{
				return string.Format("{0}' {1}\"", num4, num5);
			}
			return string.Format("{0}\"", Math.Round((double)num2, 1));
		}
	}

	public List<SpatialItemInstance> GetDamagedDeployables()
	{
		return (from x in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NONE)
			where x.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY && x.durability < x.GetItemData<DeployableItemData>().MaxDurabilityDays
			select x).ToList<SpatialItemInstance>();
	}

	public decimal GetRepairAllCost()
	{
		int count = GameManager.Instance.SaveData.Inventory.spatialUnderlayItems.FindAll((SpatialItemInstance i) => i.GetItemData<SpatialItemData>().itemType == ItemType.DAMAGE).Count;
		List<SpatialItemInstance> damagedDeployables = this.GetDamagedDeployables();
		decimal totalDurabilityCost = 0m;
		damagedDeployables.ForEach(delegate(SpatialItemInstance si)
		{
			if (si.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.POT)
			{
				totalDurabilityCost += GameManager.Instance.GameConfigData.PotRepairCostPerDay * (decimal)si.GetMissingDurabilityAmount();
				return;
			}
			if (si.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.NET)
			{
				totalDurabilityCost += GameManager.Instance.GameConfigData.NetRepairCostPerDay * (decimal)si.GetMissingDurabilityAmount();
			}
		});
		return GameManager.Instance.GameConfigData.HullRepairCostPerSquare * count + totalDurabilityCost;
	}

	public void UseRepairKit()
	{
		bool flag = false;
		int num = 2;
		for (int i = 0; i < num; i++)
		{
			bool flag2 = false;
			SpatialItemInstance damageToRepairWithEquipmentPriority = GameManager.Instance.GridManager.GetDamageToRepairWithEquipmentPriority(new List<ItemSubtype>
			{
				ItemSubtype.ENGINE,
				ItemSubtype.ROD,
				ItemSubtype.LIGHT
			}, out flag2);
			if (flag2)
			{
				flag = true;
			}
			if (damageToRepairWithEquipmentPriority != null)
			{
				GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(damageToRepairWithEquipmentPriority, true);
				GameEvents.Instance.TriggerFocusedGridCellChanged(GameManager.Instance.GridManager.LastSelectedCell);
				GameEvents.Instance.TriggerOnPlayerDamageChanged();
				GameEvents.Instance.TriggerItemInventoryChanged(null);
			}
		}
		if (!flag)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ANY_REPAIR_KIT_USED, "notification.hull-repaired");
		}
	}

	public void RepairHullDamage(bool free = false)
	{
		decimal repairAllCost = this.GetRepairAllCost();
		if (free || GameManager.Instance.SaveData.Funds >= repairAllCost)
		{
			if (!free)
			{
				GameManager.Instance.AddFunds(-repairAllCost);
			}
			GameManager.Instance.SaveData.Inventory.spatialUnderlayItems.FindAll((SpatialItemInstance i) => i.GetItemData<SpatialItemData>().itemType == ItemType.DAMAGE).ForEach(delegate(SpatialItemInstance spatialItemInstance)
			{
				GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(spatialItemInstance, true);
			});
			GameEvents.Instance.TriggerFocusedGridCellChanged(GameManager.Instance.GridManager.LastSelectedCell);
			GameEvents.Instance.TriggerOnPlayerDamageChanged();
			GameEvents.Instance.TriggerItemInventoryChanged(null);
		}
	}

	public void RepairAllItemDurability()
	{
		(from x in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NONE)
			where x.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY
			select x).ToList<SpatialItemInstance>().ForEach(delegate(SpatialItemInstance si)
		{
			si.RepairToFullDurability();
		});
		GameEvents.Instance.TriggerItemsRepaired();
	}

	public void ReplaceFishWithRot(FishItemInstance fishItemInstance, SerializableGrid grid, bool sendNotification)
	{
		if (fishItemInstance.isInfected)
		{
			return;
		}
		FishItemData itemData = fishItemInstance.GetItemData<FishItemData>();
		List<GridCellData> cellsAffectedByObjectAtPosition = grid.GetCellsAffectedByObjectAtPosition(itemData.dimensions, fishItemInstance.GetPosition());
		grid.RemoveObjectFromGridData(fishItemInstance, true);
		GridCellData gridCellData = cellsAffectedByObjectAtPosition[0];
		SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>("rot");
		grid.AddObjectToGridData(spatialItemInstance, new Vector3Int(gridCellData.x, gridCellData.y, 0), true, null);
		if (sendNotification)
		{
			GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.ROT, "notification.rot", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
		}
	}

	public void ReplaceFishWithAberration(FishItemInstance fishItemInstance, SerializableGrid grid, bool autoInfect, bool allowUndiscovered)
	{
		FishItemData itemData = fishItemInstance.GetItemData<FishItemData>();
		Vector3Int position = fishItemInstance.GetPosition();
		float size = fishItemInstance.size;
		float freshness = fishItemInstance.freshness;
		List<FishItemData> list = itemData.Aberrations.Where((FishItemData aberrationItemData) => allowUndiscovered || GameManager.Instance.SaveData.GetCaughtCountById(aberrationItemData.id) > 0).ToList<FishItemData>();
		if (list.Count == 0)
		{
			return;
		}
		FishItemData fishItemData = list.PickRandom<FishItemData>();
		grid.RemoveObjectFromGridData(fishItemInstance, true);
		FishItemInstance fishItemInstance2 = new FishItemInstance
		{
			id = fishItemData.id,
			size = size,
			freshness = freshness
		};
		if (autoInfect)
		{
			fishItemInstance2.Infect();
		}
		grid.AddObjectToGridData(fishItemInstance2, position, true, null);
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("item.list", new Action<CommandArg[]>(this.ListItems), 0, 0, "Lists all item ids");
		Terminal.Shell.AddCommand("history.items", new Action<CommandArg[]>(this.ViewItemHistory), 0, 0, "Lists the item transaction history");
		Terminal.Shell.AddCommand("history.shops", new Action<CommandArg[]>(this.ViewShopHistory), 0, 0, "Lists the shop visit history");
		Terminal.Shell.AddCommand("debt.pay", new Action<CommandArg[]>(this.RepayDebt), 1, 1, "Repays debt at Greater Marrow. e.g. debt.pay 100");
		Terminal.Shell.AddCommand("checkdecimals", new Action<CommandArg[]>(this.CheckDecimals), 0, 0, "Prints the value of all decimals stored in the save file.");
		Terminal.Shell.AddCommand("ency.all", new Action<CommandArg[]>(this.FillEncyclopedia), 0, 0, "Completes the encyclopedia (increments the catch and sell counters of all species by 1).");
		Terminal.Shell.AddCommand("ency.most", new Action<CommandArg[]>(this.AlmostFillEncyclopedia), 0, 0, "Almost completes the encyclopedia (increments the catch and sell counters of all species by 1, except mackerel).");
		Terminal.Shell.AddCommand("ency.most.dlc1", new Action<CommandArg[]>(this.AlmostFillEncyclopediaDLC1), 0, 0, "Almost completes the encyclopedia for DLC1 (increments the catch and sell counters of all species by 1, except icefish).");
		Terminal.Shell.AddCommand("ency.most.dlc2", new Action<CommandArg[]>(this.AlmostFillEncyclopediaDLC2), 0, 0, "Almost completes the encyclopedia for DLC2 (increments the catch and sell counters of all species by 1, except osteostracan).");
		Terminal.Shell.AddCommand("repair", new Action<CommandArg[]>(this.RepairAllDebug), 0, 0, "Repairs all damage to the boat.");
		Terminal.Shell.AddCommand("aberrate", new Action<CommandArg[]>(this.AberrateDebug), 0, 0, "Aberrates a random fish in your inventory.");
		Terminal.Shell.AddCommand("infect", new Action<CommandArg[]>(this.InfectDebug), 0, 0, "Infects a random fish in your inventory.");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("item.list");
		Terminal.Shell.RemoveCommand("history.items");
		Terminal.Shell.RemoveCommand("history.shops");
		Terminal.Shell.RemoveCommand("debt.pay");
		Terminal.Shell.RemoveCommand("checkdecimals");
		Terminal.Shell.RemoveCommand("ency.all");
		Terminal.Shell.RemoveCommand("ency.most");
		Terminal.Shell.RemoveCommand("ency.most.dlc1");
		Terminal.Shell.RemoveCommand("ency.most.dlc2");
		Terminal.Shell.RemoveCommand("repair");
		Terminal.Shell.RemoveCommand("aberrate");
		Terminal.Shell.RemoveCommand("infect");
	}

	private void AberrateDebug(CommandArg[] args)
	{
		FishItemInstance fishItemInstance = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).First((FishItemInstance f) => !f.IsAberrant());
		if (fishItemInstance != null)
		{
			this.ReplaceFishWithAberration(fishItemInstance, GameManager.Instance.SaveData.Inventory, false, true);
		}
	}

	private void InfectDebug(CommandArg[] args)
	{
		FishItemInstance fishItemInstance = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).First((FishItemInstance f) => !f.isInfected);
		if (fishItemInstance != null)
		{
			fishItemInstance.Infect();
		}
	}

	private void RepairAllDebug(CommandArg[] args)
	{
		this.RepairHullDamage(true);
		this.RepairAllItemDurability();
	}

	private void FillEncyclopedia(CommandArg[] args)
	{
		this.GetAllItemsOfType<FishItemData>().ForEach(delegate(FishItemData f)
		{
			GameManager.Instance.SaveData.IncrementCaughtCounterById(f.id);
			GameManager.Instance.SaveData.RecordItemTransaction(f.id, true);
		});
	}

	private void AlmostFillEncyclopedia(CommandArg[] args)
	{
		this.GetAllItemsOfType<FishItemData>().ForEach(delegate(FishItemData f)
		{
			if (!f.id.StartsWith("mackerel"))
			{
				GameManager.Instance.SaveData.IncrementCaughtCounterById(f.id);
				GameManager.Instance.SaveData.RecordItemTransaction(f.id, true);
			}
		});
	}

	private void AlmostFillEncyclopediaDLC1(CommandArg[] args)
	{
		this.GetAllItemsOfType<FishItemData>().ForEach(delegate(FishItemData f)
		{
			if (!f.id.StartsWith("icefish"))
			{
				GameManager.Instance.SaveData.IncrementCaughtCounterById(f.id);
				GameManager.Instance.SaveData.RecordItemTransaction(f.id, true);
			}
		});
	}

	private void AlmostFillEncyclopediaDLC2(CommandArg[] args)
	{
		this.GetAllItemsOfType<FishItemData>().ForEach(delegate(FishItemData f)
		{
			if (!f.id.StartsWith("osteostracan"))
			{
				GameManager.Instance.SaveData.IncrementCaughtCounterById(f.id);
				GameManager.Instance.SaveData.RecordItemTransaction(f.id, true);
			}
		});
	}

	private void CheckDecimals(CommandArg[] args)
	{
		GameManager.Instance.SaveData.decimalVariables.Keys.ToList<string>().ForEach(delegate(string key)
		{
		});
	}

	private void ListItems(CommandArg[] args)
	{
		string items = "";
		this.allItems.ForEach(delegate(ItemData i)
		{
			items = items + i.id + ", ";
		});
	}

	private void ViewItemHistory(CommandArg[] args)
	{
		GameManager.Instance.SaveData.itemTransactions.ForEach(delegate(SerializedItemTransaction t)
		{
		});
	}

	private void ViewShopHistory(CommandArg[] args)
	{
		GameManager.Instance.SaveData.shopHistories.ForEach(delegate(SerializedShopHistory h)
		{
			string visitDays = "[";
			h.visitDays.ForEach(delegate(int d)
			{
				visitDays += string.Format("{0}, ", d);
			});
			visitDays += "]";
			string transactionDays = "[";
			h.transactionDays.ForEach(delegate(int d)
			{
				transactionDays += string.Format("{0}, ", d);
			});
			transactionDays += "]";
		});
	}

	private void RepayDebt(CommandArg[] args)
	{
		decimal num = (decimal)args[0].Float;
		if (num > 0m)
		{
			GameManager.Instance.AddFunds(num);
			GameManager.Instance.DialogueRunner.RepayDebt(DockProgressType.GM_REPAYMENTS.ToString(), num);
			return;
		}
		CustomDebug.EditorLogError("[ItemManager] RepayDebt() no negative numbers please");
	}

	[SerializeField]
	private AssetLabelReference itemDataAssetLabelReference;

	[SerializeField]
	private List<GameObject> coroutineRunners = new List<GameObject>();

	public List<ItemData> allItems = new List<ItemData>();

	private bool _hasLoadedItems;

	public enum BuySellMode
	{
		BUY,
		SELL
	}
}
