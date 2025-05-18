using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandTerminal;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AchievementManager : MonoBehaviour
{
	public void Awake()
	{
		GameManager.Instance.AchievementManager = this;
	}

	public async Task Init()
	{
		bool flag = true;
		this.statsStrategy = new DefaultStatStrategy();
		this.achievementStrategy = new GDKPCAchievementStrategy();
		if (flag)
		{
			Addressables.LoadAssetsAsync<AchievementData>(this.achievementDataAssetLabelReference, null).Completed += this.OnAchievementDataAddressablesLoaded;
		}
		else
		{
			this.waitingForAddressableLoad = false;
		}
		while (this.waitingForAddressableLoad)
		{
			await Awaiters.NextFrame;
		}
		await this.achievementStrategy.Init(this.allAchievements);
	}

	public void Reinit()
	{
	}

	private void OnEnable()
	{
		this.AddTerminalCommands();
		ApplicationEvents.Instance.OnGameLoaded += this.Subscribe;
		ApplicationEvents.Instance.OnGameUnloaded += this.Unsubscribe;
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
		ApplicationEvents.Instance.OnGameLoaded -= this.Subscribe;
		ApplicationEvents.Instance.OnGameUnloaded -= this.Unsubscribe;
		this.Unsubscribe();
	}

	private void Subscribe()
	{
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		GameEvents.Instance.OnResearchCompleted += this.OnResearchCompleted;
		GameEvents.Instance.OnPlayerStatsChanged += this.OnPlayerStatsChanged;
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
		GameEvents.Instance.OnQuestCompleted += this.OnQuestCompleted;
		GameEvents.Instance.OnQuestStepCompleted += this.OnQuestStepCompleted;
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnItemSold += this.OnItemSold;
		GameEvents.Instance.OnItemDestroyed += this.OnItemDestroyed;
		GameEvents.Instance.OnFishingSpotDepleted += this.OnFishingSpotDepleted;
		GameEvents.Instance.OnFishCaught += this.OnFishCaught;
		GameEvents.Instance.OnNodeVisited += this.OnNodeVisited;
		GameEvents.Instance.OnThreatBanished += this.OnThreatBanished;
		GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		GameManager.Instance.DialogueRunner.AddCommandHandler<string, bool>("SetAchievementState", new Action<string, bool>(this.SetAchievementState));
		List<FishItemData> list = GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH).Cast<FishItemData>().ToList<FishItemData>();
		this.allRegularFishBaseGame = list.Where((FishItemData f) => !f.IsAberration && f.entitlementsRequired.Contains(Entitlement.NONE)).ToList<FishItemData>();
		this.allAberratedFishBaseGame = list.Where((FishItemData f) => f.IsAberration && f.entitlementsRequired.Contains(Entitlement.NONE)).ToList<FishItemData>();
		this.allRegularFishDLC1 = list.Where((FishItemData f) => !f.IsAberration && f.entitlementsRequired.Contains(Entitlement.DLC_1)).ToList<FishItemData>();
		this.allAberratedFishDLC1 = list.Where((FishItemData f) => f.IsAberration && f.entitlementsRequired.Contains(Entitlement.DLC_1) && !f.entitlementsRequired.Contains(Entitlement.DLC_2)).ToList<FishItemData>();
		this.allRegularFishDLC2 = list.Where((FishItemData f) => !f.IsAberration && f.entitlementsRequired.Contains(Entitlement.DLC_2)).ToList<FishItemData>();
		this.allAberratedFishDLC2 = list.Where((FishItemData f) => f.IsAberration && f.entitlementsRequired.Contains(Entitlement.DLC_2) && !f.LocationHiddenUntilCaught && f.NonAberrationParent != null).ToList<FishItemData>();
		this.allExoticAberrations = list.Where((FishItemData f) => f.IsAberration && f.LocationHiddenUntilCaught).ToList<FishItemData>();
		this.allAchievements.ForEach(delegate(AchievementData a)
		{
			this.EvaluateAchievement(a);
		});
	}

	private void Unsubscribe()
	{
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
			GameEvents.Instance.OnResearchCompleted -= this.OnResearchCompleted;
			GameEvents.Instance.OnPlayerStatsChanged -= this.OnPlayerStatsChanged;
			GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
			GameEvents.Instance.OnQuestCompleted -= this.OnQuestCompleted;
			GameEvents.Instance.OnQuestStepCompleted -= this.OnQuestStepCompleted;
			GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
			GameEvents.Instance.OnItemSold -= this.OnItemSold;
			GameEvents.Instance.OnItemDestroyed -= this.OnItemDestroyed;
			GameEvents.Instance.OnFishingSpotDepleted -= this.OnFishingSpotDepleted;
			GameEvents.Instance.OnFishCaught -= this.OnFishCaught;
			GameEvents.Instance.OnNodeVisited -= this.OnNodeVisited;
			GameEvents.Instance.OnThreatBanished -= this.OnThreatBanished;
			GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
		}
	}

	private void OnAchievementDataAddressablesLoaded(AsyncOperationHandle<IList<AchievementData>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (AchievementData achievementData in handle.Result)
			{
				this.allAchievements.Add(achievementData);
			}
		}
		this.allAchievements.Sort((AchievementData x, AchievementData y) => string.Compare(x.steamId, y.steamId));
		this.waitingForAddressableLoad = false;
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		this.EvaluateAchievement(DredgeAchievementId.HULL_2);
		this.EvaluateAchievement(DredgeAchievementId.HULL_3);
		this.EvaluateAchievement(DredgeAchievementId.HULL_4);
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_8);
	}

	private void OnResearchCompleted(SpatialItemData spatialItemData)
	{
		this.EvaluateAchievement(DredgeAchievementId.RESEARCH_ENGINES);
		this.EvaluateAchievement(DredgeAchievementId.RESEARCH_RODS);
		this.EvaluateAchievement(DredgeAchievementId.RESEARCH_NETS);
		this.EvaluateAchievement(DredgeAchievementId.RESEARCH_POTS);
	}

	private void OnPlayerStatsChanged()
	{
		this.EvaluateAchievement(DredgeAchievementId.STAT_ENGINE_SPEED);
		this.EvaluateAchievement(DredgeAchievementId.STAT_FISHING_SPEED);
		this.EvaluateAchievement(DredgeAchievementId.STAT_LIGHT_STRENGTH);
		if (!this.GetAchievementState(DredgeAchievementId.FULL_EQUIPMENT))
		{
			int filledCells = GameManager.Instance.SaveData.Inventory.GetFilledCells(ItemSubtype.ENGINE);
			int filledCells2 = GameManager.Instance.SaveData.Inventory.GetFilledCells(ItemSubtype.ROD);
			int filledCells3 = GameManager.Instance.SaveData.Inventory.GetFilledCells(ItemSubtype.NET);
			int filledCells4 = GameManager.Instance.SaveData.Inventory.GetFilledCells(ItemSubtype.LIGHT);
			int num = filledCells + filledCells2 + filledCells3 + filledCells4;
			int countCellsAcceptingType = GameManager.Instance.SaveData.Inventory.GetCountCellsAcceptingType(ItemType.EQUIPMENT);
			if (num >= countCellsAcceptingType)
			{
				this.SetAchievementState(DredgeAchievementId.FULL_EQUIPMENT, true);
			}
		}
	}

	private void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		if (!this.GetAchievementState(DredgeAchievementId.FULL_CARGO) && GameManager.Instance.SaveData.Inventory.GetFillProportional(ItemSubtype.ALL, ItemSubtype.DREDGE) >= 1f)
		{
			this.SetAchievementState(DredgeAchievementId.FULL_CARGO, true);
		}
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1) && !this.GetAchievementState(DredgeAchievementId.DLC_3_7))
		{
			if (GameManager.Instance.SaveData.Inventory.spatialItems.Count((SpatialItemInstance i) => i.GetItemData<SpatialItemData>().id.Contains("ice-block")) >= 5)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_3_7, true);
			}
		}
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2) && !this.GetAchievementState(DredgeAchievementId.DLC_4_11))
		{
			if (GameManager.Instance.SaveData.Inventory.spatialItems.Count((SpatialItemInstance i) => i.GetItemData<SpatialItemData>().id == "dark-splash") >= 5)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_4_11, true);
			}
		}
	}

	private void OnItemDestroyed(SpatialItemData spatialItemData, bool playerDestroyed)
	{
		this.EvaluateAchievement(DredgeAchievementId.DISCARD_FISH);
	}

	private void OnFishingSpotDepleted()
	{
		this.EvaluateAchievement(DredgeAchievementId.DEPLETE_FISH_SPOTS);
	}

	private void OnQuestCompleted(QuestData questData)
	{
		this.EvaluateAchievement(DredgeAchievementId.INTRODUCTIONS);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_ALL_SIDE_QUESTS);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_CHAPTER_1);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_CHAPTER_2);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_CHAPTER_3);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_CHAPTER_4);
		this.EvaluateAchievement(DredgeAchievementId.COMPLETE_CHAPTER_5);
		this.EvaluateAchievement(DredgeAchievementId.DLC_3_3);
		this.EvaluateAchievement(DredgeAchievementId.DLC_3_4);
		this.EvaluateAchievement(DredgeAchievementId.DLC_3_5);
		this.EvaluateAchievement(DredgeAchievementId.DLC_3_6);
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_4);
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_5);
	}

	private void OnQuestStepCompleted(QuestStepData questStepData)
	{
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_3);
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_6);
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		this.EvaluateAchievement(DredgeAchievementId.DISCOVER_ALL_DOCKS);
	}

	public void OnItemSold(SpatialItemData spatialItemData)
	{
		this.EvaluateAchievement(DredgeAchievementId.SELL_FISH_VALUE_1);
		this.EvaluateAchievement(DredgeAchievementId.SELL_TRINKETS_VALUE_1);
	}

	public void OnFishCaught()
	{
		this.EvaluateAchievement(DredgeAchievementId.CATCH_CRABS_POT_1);
		this.EvaluateAchievement(DredgeAchievementId.CATCH_FISH_NET_1);
		this.EvaluateAchievement(DredgeAchievementId.CATCH_FISH_ROD_1);
		bool flag = this.allRegularFishBaseGame.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
		bool flag2 = this.allAberratedFishBaseGame.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
		if (flag)
		{
			this.SetAchievementState(DredgeAchievementId.CATCH_ALL_REGULAR_FISH, true);
		}
		if (flag2)
		{
			this.SetAchievementState(DredgeAchievementId.CATCH_ALL_ABERRATIONS, true);
		}
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1))
		{
			bool flag3 = this.allRegularFishDLC1.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
			flag2 = this.allAberratedFishDLC1.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
			if (flag3)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_3_1, true);
			}
			if (flag2)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_3_2, true);
			}
			if (GameManager.Instance.SaveData.GetCaughtCountById("colossal-squid") > 0)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_3_8, true);
			}
		}
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2))
		{
			bool flag4 = this.allRegularFishDLC2.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
			flag2 = this.allAberratedFishDLC2.TrueForAll((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0);
			if (flag4)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_4_1, true);
			}
			if (flag2)
			{
				this.SetAchievementState(DredgeAchievementId.DLC_4_2, true);
			}
			if (this.allExoticAberrations.Any((FishItemData fish) => GameManager.Instance.SaveData.GetCaughtCountById(fish.id) > 0))
			{
				this.SetAchievementState(DredgeAchievementId.DLC_4_9, true);
			}
		}
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		this.EvaluateAchievement(DredgeAchievementId.DLC_4_10);
	}

	private void OnNodeVisited(string nodeName)
	{
		this.EvaluateAchievement(DredgeAchievementId.SOLVE_ALL_SHRINES);
	}

	private void OnThreatBanished(bool countForAchievement)
	{
		if (countForAchievement)
		{
			SaveData saveData = GameManager.Instance.SaveData;
			int threatsBanishedCount = saveData.ThreatsBanishedCount;
			saveData.ThreatsBanishedCount = threatsBanishedCount + 1;
			this.EvaluateAchievement(DredgeAchievementId.ABILITY_BANISH);
		}
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("ach.list", new Action<CommandArg[]>(this.ListAchievements), 0, 0, "Lists the ids of all achievements.");
		Terminal.Shell.AddCommand("ach.test", new Action<CommandArg[]>(this.TestAchievements), 0, 0, "Prints the current state of all achievements.");
		Terminal.Shell.AddCommand("ach.get", new Action<CommandArg[]>(this.GetAchievementStateDebug), 1, 1, "Gets the earn state of an achievement by id.");
		Terminal.Shell.AddCommand("ach.set", new Action<CommandArg[]>(this.SetAchievementStateDebug), 2, 2, "Sets the earn state of an achievement by id.");
		Terminal.Shell.AddCommand("ach.clear", new Action<CommandArg[]>(this.ClearAllAchievementsDebug), 0, 0, "Sets all achievements to unearned.");
		Terminal.Shell.AddCommand("ach.final", new Action<CommandArg[]>(this.TestFinalAchievement), 0, 0, "Unlock all achievement excuding final unlock 'From the Depths'");
		Terminal.Shell.AddCommand("ach.ps4Test", new Action<CommandArg[]>(this.PS4FinalTest), 0, 0, "Unlocked all base game achievements excuding DISCOVER_ALL_DOCKS. Used to test PS4 softlock");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("ach.list");
		Terminal.Shell.RemoveCommand("ach.test");
		Terminal.Shell.RemoveCommand("ach.get");
		Terminal.Shell.RemoveCommand("ach.set");
		Terminal.Shell.RemoveCommand("ach.clear");
		Terminal.Shell.RemoveCommand("ach.final");
		Terminal.Shell.RemoveCommand("ach.ps4Test");
	}

	private void ListAchievements(CommandArg[] args)
	{
		string ids = "";
		this.allAchievements.ForEach(delegate(AchievementData a)
		{
			ids = ids + a.steamId + ", ";
		});
	}

	private void TestAchievements(CommandArg[] args)
	{
		if (GameManager.Instance.SaveData == null)
		{
			return;
		}
		this.allAchievements.ForEach(delegate(AchievementData a)
		{
			a.Print();
		});
	}

	public void EvaluateAchievement(DredgeAchievementId id)
	{
		AchievementData achievementData = this.GetAchievementData(id);
		if (achievementData)
		{
			this.EvaluateAchievement(achievementData);
		}
	}

	public void EvaluateAchievement(AchievementData achievementData)
	{
		if (this.GetAchievementState(achievementData))
		{
			return;
		}
		if (achievementData.Evaluate())
		{
			this.SetAchievementState(achievementData, true);
		}
	}

	public bool GetAchievementState(DredgeAchievementId achievementId)
	{
		AchievementData achievementData = this.GetAchievementData(achievementId);
		return this.GetAchievementState(achievementData);
	}

	public bool GetAchievementState(AchievementData achievementData)
	{
		return achievementData && this.achievementStrategy.GetAchievement(achievementData);
	}

	public void SetAchievementState(DredgeAchievementId achievementId, bool newState)
	{
		AchievementData achievementData = this.GetAchievementData(achievementId);
		if (achievementData)
		{
			this.SetAchievementState(achievementData, newState);
		}
	}

	private void SetAchievementState(AchievementData achievementData, bool newState)
	{
		if (this.GetAchievementState(achievementData) != newState)
		{
			this.achievementStrategy.SetAchievement(achievementData, newState);
			if (newState)
			{
				this.EvaluateAchievement(DredgeAchievementId.ALL_ACHIEVEMENTS);
			}
		}
	}

	private AchievementData GetAchievementData(DredgeAchievementId dredgeAchievementId)
	{
		return this.allAchievements.Find((AchievementData a) => a.id == dredgeAchievementId);
	}

	private void GetAchievementStateDebug(CommandArg[] args)
	{
		DredgeAchievementId dredgeAchievementId;
		if (Enum.TryParse<DredgeAchievementId>(args[0].String, out dredgeAchievementId))
		{
			this.GetAchievementState(dredgeAchievementId);
		}
	}

	private void SetAchievementStateDebug(CommandArg[] args)
	{
		this.SetAchievementState(args[0].String, args[1].Int > 0);
	}

	private void SetAchievementState(string id, bool state)
	{
		DredgeAchievementId dredgeAchievementId;
		Enum.TryParse<DredgeAchievementId>(id, out dredgeAchievementId);
		this.SetAchievementState(dredgeAchievementId, state);
	}

	private void ClearAllAchievementsDebug(CommandArg[] args)
	{
		this.allAchievements.ForEach(delegate(AchievementData a)
		{
			this.SetAchievementState(a, false);
		});
	}

	private void TestFinalAchievement(CommandArg[] args)
	{
		TaskUtil.Run(new Func<Task>(this.TestFinalAchievementAsync));
	}

	private void PS4FinalTest(CommandArg[] args)
	{
		TaskUtil.Run(new Func<Task>(this.TestFinalPS4AchievementAsync));
	}

	private async Task TestFinalPS4AchievementAsync()
	{
		foreach (AchievementData achievementData in this.allAchievements)
		{
			if (achievementData.id != DredgeAchievementId.ALL_ACHIEVEMENTS && achievementData.id != DredgeAchievementId.DISCOVER_ALL_DOCKS && achievementData.id < DredgeAchievementId.DLC_3_1)
			{
				this.SetAchievementState(achievementData.id, true);
				await Awaiters.Seconds(1f);
			}
		}
		List<AchievementData>.Enumerator enumerator = default(List<AchievementData>.Enumerator);
	}

	private async Task TestFinalAchievementAsync()
	{
		foreach (AchievementData achievementData in this.allAchievements)
		{
			if (achievementData.id != DredgeAchievementId.ALL_ACHIEVEMENTS)
			{
				this.SetAchievementState(achievementData.id, true);
				await Awaiters.Seconds(1f);
			}
		}
		List<AchievementData>.Enumerator enumerator = default(List<AchievementData>.Enumerator);
	}

	[SerializeField]
	private AssetLabelReference achievementDataAssetLabelReference;

	private IAchievementStrategy achievementStrategy;

	private IStatsStrategy statsStrategy;

	private List<AchievementData> allAchievements = new List<AchievementData>();

	private List<FishItemData> allRegularFishBaseGame;

	private List<FishItemData> allAberratedFishBaseGame;

	private List<FishItemData> allRegularFishDLC1;

	private List<FishItemData> allAberratedFishDLC1;

	private List<FishItemData> allRegularFishDLC2;

	private List<FishItemData> allAberratedFishDLC2;

	private List<FishItemData> allExoticAberrations;

	private bool waitingForAddressableLoad = true;
}
