using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UpgradeManager : MonoBehaviour
{
	public bool HasLoadedUpgradeData
	{
		get
		{
			return this._hasLoadedUpgradeData;
		}
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnGameLoaded += this.OnGameLoaded;
		ApplicationEvents.Instance.OnGameUnloaded += this.OnGameUnloaded;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnGameLoaded -= this.OnGameLoaded;
		ApplicationEvents.Instance.OnGameUnloaded -= this.OnGameUnloaded;
	}

	private void OnGameLoaded()
	{
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		this.AddTerminalCommands();
	}

	private void OnGameUnloaded()
	{
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
		this.RemoveTerminalCommands();
	}

	public void LoadData()
	{
		Addressables.LoadAssetsAsync<UpgradeData>(this.upgradeDataAssetLabelReference, null).Completed += this.OnUpgradeDataAddressablesLoaded;
	}

	private void OnUpgradeDataAddressablesLoaded(AsyncOperationHandle<IList<UpgradeData>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (UpgradeData upgradeData in handle.Result)
			{
				this.allUpgradeData.Add(upgradeData);
			}
			this._hasLoadedUpgradeData = true;
		}
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		if (!(upgradeData is HullUpgradeData))
		{
			if (upgradeData is SlotUpgradeData)
			{
				(upgradeData as SlotUpgradeData).CellGroupConfigs.ForEach(delegate(CellGroupConfiguration cgc)
				{
					GameManager.Instance.SaveData.Inventory.ApplyCellGroupConfig(cgc);
				});
				GameManager.Instance.SaveData.Inventory.TriggerRefreshEvent();
			}
			return;
		}
		GridConfiguration gridConfigForHullTier = GameManager.Instance.GameConfigData.GetGridConfigForHullTier(upgradeData.tier);
		if (!gridConfigForHullTier)
		{
			CustomDebug.EditorLogError(string.Format("[SaveData] SetInventoryTier({0}) could not find matching grid config for key.", upgradeData.tier));
			return;
		}
		List<SpatialItemInstance> list = GameManager.Instance.SaveData.Inventory.spatialItems.ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list2 = (from x in list
			where x.GetItemData<SpatialItemData>().itemType == ItemType.EQUIPMENT && x.GetItemData<SpatialItemData>().itemSubtype != ItemSubtype.DREDGE && x.GetItemData<SpatialItemData>().itemSubtype != ItemSubtype.POT
			orderby x.GetItemData<SpatialItemData>().dimensions.Count descending
			select x).ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list3 = (from x in list
			where x.GetItemData<SpatialItemData>().itemType == ItemType.EQUIPMENT && x.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.DREDGE
			orderby x.GetItemData<SpatialItemData>().dimensions.Count descending
			select x).ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list4 = (from x in list
			where x.GetItemData<SpatialItemData>().itemType != ItemType.EQUIPMENT && x.GetItemData<SpatialItemData>().itemSubtype != ItemSubtype.FISH
			orderby x.GetItemData<SpatialItemData>().dimensions.Count descending
			select x).ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list5 = (from x in list
			where x.GetItemData<SpatialItemData>().itemType == ItemType.EQUIPMENT && x.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.POT
			orderby x.GetItemData<SpatialItemData>().dimensions.Count descending
			select x).ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list6 = (from x in list
			where x.GetItemData<SpatialItemData>().itemType != ItemType.EQUIPMENT && x.GetItemData<SpatialItemData>().itemSubtype == ItemSubtype.FISH
			orderby x.GetItemData<SpatialItemData>().dimensions.Count descending
			select x).ToList<SpatialItemInstance>();
		GameManager.Instance.SaveData.Inventory.Init(gridConfigForHullTier, true);
		GameManager.Instance.GridManager.AddBulkItemInstancesToGrid(list3, true, GameManager.Instance.SaveData.Inventory, GameManager.Instance.SaveData.Storage);
		GameManager.Instance.GridManager.AddBulkItemInstancesToGrid(list2, true, GameManager.Instance.SaveData.Inventory, GameManager.Instance.SaveData.Storage);
		GameManager.Instance.GridManager.AddBulkItemInstancesToGrid(list4, true, GameManager.Instance.SaveData.Inventory, GameManager.Instance.SaveData.Storage);
		GameManager.Instance.GridManager.AddBulkItemInstancesToGrid(list5, true, GameManager.Instance.SaveData.Inventory, GameManager.Instance.SaveData.Storage);
		GameManager.Instance.GridManager.AddBulkItemInstancesToGrid(list6, true, GameManager.Instance.SaveData.Inventory, GameManager.Instance.SaveData.Storage);
		GameManager.Instance.SaveData.HullTier = upgradeData.tier;
		GameManager.Instance.PlayerStats.CalculateDamageThreshold();
	}

	public UpgradeData GetHullUpgradeDataByTier(int tier)
	{
		return this.allUpgradeData.Find((UpgradeData u) => u.tier == tier && u is HullUpgradeData);
	}

	public void ApplyOwnedSlotUpgrades()
	{
		List<SlotUpgradeData> slotUpgradesForCurrentTier = this.GetSlotUpgradesForCurrentTier();
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		Action<CellGroupConfiguration> <>9__1;
		slotUpgradesForCurrentTier.ForEach(delegate(SlotUpgradeData s)
		{
			List<CellGroupConfiguration> cellGroupConfigs = s.CellGroupConfigs;
			Action<CellGroupConfiguration> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(CellGroupConfiguration cgc)
				{
					inventory.ApplyCellGroupConfig(cgc);
				});
			}
			cellGroupConfigs.ForEach(action);
		});
	}

	public List<SlotUpgradeData> GetSlotUpgradesForCurrentTier()
	{
		return this.GetSlotUpgradesForTier(GameManager.Instance.SaveData.HullTier);
	}

	public List<SlotUpgradeData> GetSlotUpgradesForTier(int tier)
	{
		return (from u in GameManager.Instance.SaveData.GetOwnedUpgrades().OfType<SlotUpgradeData>()
			where u.tier == tier
			select u).ToList<SlotUpgradeData>();
	}

	public UpgradeData GetUpgradeDataById(string id)
	{
		return this.allUpgradeData.Find((UpgradeData u) => u.id == id);
	}

	public void AddUpgrade(UpgradeData upgradeData, bool free)
	{
		if (upgradeData)
		{
			if (!free)
			{
				GameManager.Instance.AddFunds(-upgradeData.MonetaryCost);
			}
			GameManager.Instance.SaveData.SetUpgradeOwned(upgradeData, true);
			GameManager.Instance.SaveData.UpgradeBought = true;
			return;
		}
		CustomDebug.EditorLogError("[UpgradeManager] AddUpgrade() null upgradeData");
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("upgrade.list", new Action<CommandArg[]>(this.ListUpgrades), 0, 0, "Lists all upgrade ids.");
		Terminal.Shell.AddCommand("upgrade.add", new Action<CommandArg[]>(this.AddUpgradeDebug), 1, 1, "Adds an upgrade by id e.g. upgrade.add tier-1-fishing-1");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("upgrade.list");
		Terminal.Shell.RemoveCommand("upgrade.add");
	}

	private void ListUpgrades(CommandArg[] args)
	{
		string upgrades = "";
		this.allUpgradeData.ForEach(delegate(UpgradeData i)
		{
			upgrades = upgrades + i.id + ", ";
		});
	}

	private void AddUpgradeDebug(CommandArg[] args)
	{
		string @string = args[0].String;
		UpgradeData upgradeDataById = this.GetUpgradeDataById(@string);
		this.AddUpgrade(upgradeDataById, true);
	}

	[SerializeField]
	private AssetLabelReference upgradeDataAssetLabelReference;

	public List<UpgradeData> allUpgradeData = new List<UpgradeData>();

	private bool _hasLoadedUpgradeData;
}
