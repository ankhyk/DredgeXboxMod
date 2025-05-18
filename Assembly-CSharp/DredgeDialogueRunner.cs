using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using Yarn.Unity;

public class DredgeDialogueRunner : DialogueRunner
{
	public InMemoryVariableStorage InMemoryVariableStorage
	{
		get
		{
			return this.inMemoryVariableStorage;
		}
	}

	public bool ShouldImmediatelyResolveNextLine { get; set; }

	public bool ShouldAutoResolveNextLine { get; set; }

	public bool ShouldShowUnavailableOptions { get; set; }

	public bool DidPlayAnyLines { get; set; }

	private int LastQuestGridResult { get; set; }

	public ExplosivePOI CurrentExplosivePOI { get; set; }

	public string PreviousNodeName { get; set; }

	private void Awake()
	{
		GameManager.Instance.DialogueRunner = this;
		base.AddFunction<string, bool>("GetCanAffordItemById", new Func<string, bool>(this.GetCanAffordItemById));
		base.AddFunction<string, int>("GetNumItemInInventoryById", new Func<string, int>(this.GetNumItemInInventoryById));
		base.AddFunction<string, int>("GetNumItemInInventoryAndStorageById", new Func<string, int>(this.GetNumItemInInventoryAndStorageById));
		base.AddFunction<string, int>("GetNumItemInInventoryAndNetById", new Func<string, int>(this.GetNumItemInInventoryAndNetById));
		base.AddFunction<string, int>("GetNumItemInStorageById", new Func<string, int>(this.GetNumItemInStorageById));
		base.AddFunction<string, int>("GetNumItemAnywhereById", new Func<string, int>(this.GetNumItemAnywhereById));
		base.AddFunction<string, int, int, int>("GetNumFishByIdAndCondition", new Func<string, int, int, int>(this.GetNumFishByIdAndCondition));
		base.AddFunction<string, int>("GetNumItemsByType", new Func<string, int>(this.GetNumItemsByType));
		base.AddFunction<string, bool>("GetHasEquipmentForHarvestType", new Func<string, bool>(this.GetHasEquipmentForHarvestType));
		base.AddFunction<bool>("GetDidAddEnoughFishToMakeBait", new Func<bool>(this.GetDidAddEnoughFishToMakeBait));
		base.AddFunction<bool>("TryConvertDogTags", new Func<bool>(this.TryConvertDogTags));
		base.AddFunction<int>("GetNumTrophyFish", new Func<int>(this.GetNumTrophyFish));
		base.AddFunction<int>("GetNumAberrantFish", new Func<int>(this.GetNumAberrantFish));
		base.AddFunction<int>("GetNumAberrantFishInInventory", new Func<int>(this.GetNumAberrantFishInInventory));
		base.AddFunction<int>("GetNumOwnedItemsThatCanCatchFish", new Func<int>(this.GetNumOwnedItemsThatCanCatchFish));
		base.AddFunction<string, string, int>("GetNumItemsByTypeAndSubtypeIncludingStorage", new Func<string, string, int>(this.GetNumItemsByTypeAndSubtypeIncludingStorage));
		base.AddFunction<string, string, int>("GetNumItemsByTypeAndSubtype", new Func<string, string, int>(this.GetNumItemsByTypeAndSubtype));
		base.AddFunction<string, string, int>("GetNumItemInGridById", new Func<string, string, int>(this.GetNumItemInGridById));
		base.AddFunction<int>("GetNumDamage", new Func<int>(this.GetNumDamage));
		base.AddFunction<int>("GetNumDamagedDeployables", new Func<int>(this.GetNumDamagedDeployables));
		base.AddFunction<string, bool>("GetHasSpaceForItem", new Func<string, bool>(this.GetHasSpaceForItem));
		base.AddCommandHandler<string>("AddItemById", new Action<string>(this.AddItemById));
		base.AddCommandHandler<string, int, float>("SellItemById", new Action<string, int, float>(this.SellItemById));
		base.AddCommandHandler<float>("SellSingleTrophyFish", new Action<float>(this.SellSingleTrophyFish));
		base.AddCommandHandler<float>("SellSingleAberrantFish", new Action<float>(this.SellSingleAberrantFish));
		base.AddCommandHandler<string, int>("RemoveItemById", new Action<string, int>(this.RemoveItemById));
		base.AddCommandHandler<string>("RemoveItemByIdInInventoryAndStorage", new Action<string>(this.RemoveItemByIdInInventoryAndStorage));
		base.AddCommandHandler<string, int, int, int>("RemoveFishByIdAndCondition", new Action<string, int, int, int>(this.RemoveFishByIdAndCondition));
		base.AddCommandHandler<string>("UnlockAbility", new Action<string>(this.UnlockAbility));
		base.AddCommandHandler<string, string>("AddItemToGrid", new Action<string, string>(this.AddItemToGrid));
		base.AddFunction<decimal>("GetFunds", new Func<decimal>(this.GetFunds));
		base.AddCommandHandler<decimal>("ChangeFunds", new Action<decimal>(this.ChangeFunds));
		base.AddFunction<int>("GetNumDaysPassed", new Func<int>(this.GetNumDaysPassed));
		base.AddFunction<int>("GetDayPeriod", new Func<int>(this.GetDayPeriod));
		base.AddFunction<int>("GetHour", new Func<int>(this.GetHour));
		base.AddFunction<string, bool>("GetTemporalMarkerExists", new Func<string, bool>(this.GetTemporalMarkerExists));
		base.AddFunction<string, int>("GetHoursSinceTemporalMarker", new Func<string, int>(this.GetHoursSinceTemporalMarker));
		base.AddFunction<string, int>("GetDaysSinceTemporalMarker", new Func<string, int>(this.GetDaysSinceTemporalMarker));
		base.AddCommandHandler<string, bool>("SetTemporalMarker", new Action<string, bool>(this.SetTemporalMarker));
		base.AddCommandHandler<string>("RemoveTemporalMarker", new Action<string>(this.RemoveTemporalMarker));
		base.AddCommandHandler<int, string>("PassTime", (int hour, string reason) => base.StartCoroutine(this.PassTime(hour, reason)));
		base.AddCommandHandler<int, string>("PassTimeUntil", (int hour, string reason) => base.StartCoroutine(this.PassTimeUntil(hour, reason)));
		base.AddFunction<string, bool>("GetHasVisitedNode", new Func<string, bool>(this.GetHasVisitedNode));
		base.AddCommandHandler("AutoResolveNextLine", new Action(this.AutoResolveNextLine));
		base.AddCommandHandler("ImmediatelyResolveNextLine", new Action(this.ImmediatelyResolveNextLine));
		base.AddCommandHandler("RequestExitDestination", new Action(this.RequestExitDestination));
		base.AddCommandHandler<bool>("ShowUnavailableOptions", new Action<bool>(this.ShowUnavailableOptions));
		base.AddCommandHandler<int>("SetRollDice", new Action<int>(this.SetRollDice));
		base.AddCommandHandler<string>("PlayClip", new Action<string>(this.PlayClip));
		base.AddCommandHandler<string>("PlayLoopingDialogueAudio", new Action<string>(this.PlayLoopingDialogueAudio));
		base.AddCommandHandler("StopLoopingDialogueAudio", new Action(this.StopLoopingDialogueAudio));
		base.AddCommandHandler<string>("ShowQuestGrid", (string param) => base.StartCoroutine(this.ShowQuestGrid(param)));
		base.AddCommandHandler<string>("ShowAndWaitForAnimation", (string param) => base.StartCoroutine(this.ShowAndWaitForAnimation(param)));
		base.AddCommandHandler<string>("ClearQuestGrid", new Action<string>(this.ClearQuestGrid));
		base.AddCommandHandler<string, float>("SellAllItemsInQuestGrid", new Action<string, float>(this.SellAllItemsInQuestGrid));
		base.AddCommandHandler("MakeBait", new Action(this.MakeBait));
		base.AddCommandHandler<string>("ShowPortrait", new Action<string>(this.ShowPortrait));
		base.AddCommandHandler("HidePortrait", new Action(this.HidePortrait));
		base.AddCommandHandler("ClearDialogue", new Action(this.ClearDialogue));
		base.AddCommandHandler<int, int>("ChangeBoatColor", new Action<int, int>(this.ChangeBoatColor));
		base.AddCommandHandler<int>("ChangeBoatFlag", new Action<int>(this.ChangeBoatFlag));
		base.AddCommandHandler<int>("ChangeBoatBunting", new Action<int>(this.ChangeBoatBunting));
		base.AddCommandHandler<int>("ChangeBoatHorn", new Action<int>(this.ChangeBoatHorn));
		base.AddFunction<int>("GetLastQuestGridResult", new Func<int>(this.GetLastQuestGridResult));
		base.AddFunction<bool>("GetHasDeluxeEntitlement", new Func<bool>(this.GetHasDeluxeEntitlement));
		base.AddCommandHandler("RestartWorldMusic", new Action(this.RestartWorldMusic));
		base.AddCommandHandler<string, float>("TransitionToAudioMixerSnapshot", new Action<string, float>(this.TransitionToAudioMixerSnapshot));
		base.AddFunction<string, int>("GetNumShopVisits", new Func<string, int>(this.GetNumShopVisits));
		base.AddFunction<string, decimal>("GetShopTransactionTotalById", new Func<string, decimal>(this.GetShopTransactionTotalById));
		base.AddFunction<string, int>("GetDaysSinceFirstTransaction", new Func<string, int>(this.GetDaysSinceFirstTransaction));
		base.AddFunction<string, int>("GetDaysSinceLastTransaction", new Func<string, int>(this.GetDaysSinceLastTransaction));
		base.AddFunction<string, int>("GetDaysSinceLastVisit", new Func<string, int>(this.GetDaysSinceLastVisit));
		base.AddFunction<string, int>("GetDaysSinceFirstVisit", new Func<string, int>(this.GetDaysSinceFirstVisit));
		base.AddFunction<string, int>("GetNumShopVisitDaysUnique", new Func<string, int>(this.GetNumShopVisitDaysUnique));
		base.AddFunction<string, int>("GetNumShopTransactionDaysUnique", new Func<string, int>(this.GetNumShopTransactionDaysUnique));
		base.AddFunction<string, int>("GetNumItemsSoldById", new Func<string, int>(this.GetNumItemsSoldById));
		base.AddFunction<string, bool>("GetHasEverOwnedItem", new Func<string, bool>(this.GetHasEverOwnedItem));
		base.AddFunction<string, bool>("GetIsQuestInactive", new Func<string, bool>(this.GetIsQuestInactive));
		base.AddFunction<string, bool>("GetIsQuestAvailable", new Func<string, bool>(this.GetIsQuestAvailable));
		base.AddFunction<string, bool>("GetIsQuestCanBeStarted", new Func<string, bool>(this.GetIsQuestCanBeStarted));
		base.AddFunction<string, bool>("GetIsQuestStarted", new Func<string, bool>(this.GetIsQuestStarted));
		base.AddFunction<string, bool>("GetIsQuestInProgress", new Func<string, bool>(this.GetIsQuestInProgress));
		base.AddFunction<string, bool>("GetIsQuestCompleted", new Func<string, bool>(this.GetIsQuestCompleted));
		base.AddFunction<string, bool>("GetIsQuestStepActive", new Func<string, bool>(this.GetIsQuestStepActive));
		base.AddFunction<string, bool>("GetIsQuestStepCompleted", new Func<string, bool>(this.GetIsQuestStepCompleted));
		base.AddCommandHandler<string>("SetQuestAvailable", new Action<string>(this.SetQuestAvailable));
		base.AddCommandHandler<string>("SetQuestStarted", new Action<string>(this.SetQuestStarted));
		base.AddCommandHandler<string>("SetQuestStartedSilent", new Action<string>(this.SetQuestStartedSilent));
		base.AddCommandHandler<string>("SetQuestCompleted", new Action<string>(this.SetQuestCompleted));
		base.AddCommandHandler<string, int>("SetQuestCompletedWithResolution", new Action<string, int>(this.SetQuestCompletedWithResolution));
		base.AddCommandHandler<string>("SetQuestStepCompleted", new Action<string>(this.SetQuestStepCompleted));
		base.AddCommandHandler<string>("SetQuestStepCompletedSilent", new Action<string>(this.SetQuestStepCompletedSilent));
		base.AddCommandHandler<string>("ConstructBuildingTier", new Action<string>(this.ConstructBuildingTier));
		base.AddFunction<string, bool>("GetIsBuildingTierConstructed", new Func<string, bool>(this.GetIsBuildingTierConstructed));
		base.AddFunction<string, bool>("GetCanBuildingTierBeConstructed", new Func<string, bool>(this.GetCanBuildingTierBeConstructed));
		base.AddFunction<string, decimal>("GetDockProportionalProgress", new Func<string, decimal>(this.GetDockProportionalProgress));
		base.AddFunction<string, decimal>("GetDockProgressRemainingRaw", new Func<string, decimal>(this.GetDockProgressRemainingRaw));
		base.AddFunction<int, int>("GetActiveTrapState", new Func<int, int>(this.GetActiveTrapState));
		base.AddFunction<string, bool>("GetSpeakerAvailability", new Func<string, bool>(this.GetSpeakerAvailability));
		base.AddFunction<string>("GetCurrentZone", new Func<string>(this.GetCurrentZone));
		base.AddFunction<int>("GetWorldPhase", new Func<int>(this.GetWorldPhase));
		base.AddCommandHandler<int, int>("SetActiveTrapState", new Action<int, int>(this.SetActiveTrapState));
		base.AddCommandHandler("IncreaseWorldPhase", new Action(this.IncreaseWorldPhase));
		base.AddCommandHandler("IncreaseTPRWorldPhase", new Action(this.IncreaseTPRWorldPhase));
		base.AddFunction<int>("GetTPRWorldPhase", new Func<int>(this.GetTPRWorldPhase));
		base.AddCommandHandler<int>("SetTIRWorldPhase", new Action<int>(this.SetTIRWorldPhase));
		base.AddFunction<int>("GetTIRWorldPhase", new Func<int>(this.GetTIRWorldPhase));
		base.AddCommandHandler("RecordRelicHandedIn", new Action(this.RecordRelicHandedIn));
		base.AddCommandHandler("DetonateExplosives", new Action(this.DetonateExplosives));
		base.AddCommandHandler<string, bool>("SetDestinationAvailable", new Action<string, bool>(this.SetDestinationAvailable));
		base.AddCommandHandler<string, bool>("SetSpeakerAvailability", new Action<string, bool>(this.SetSpeakerAvailability));
		base.AddCommandHandler<bool>("SetIcebreakerEquipped", new Action<bool>(this.SetIcebreakerEquipped));
		base.AddCommandHandler<string, decimal>("RepayDebt", new Action<string, decimal>(this.RepayDebt));
		base.AddCommandHandler<string>("ChangeWeather", new Action<string>(this.ChangeWeather));
		base.AddCommandHandler("EmitLightning", new Action(this.EmitLightning));
		base.AddCommandHandler("DoFinalePreparations", new Action(this.DoFinalePreparations));
		base.AddCommandHandler("DoFinaleCutscenePreparations", new Action(this.DoFinaleCutscenePreparations));
		base.AddCommandHandler("RefreshDockVCams", new Action(this.RefreshDockVCams));
		base.AddFunction<bool>("GetIsDemoMode", new Func<bool>(this.GetIsDemoMode));
		base.AddFunction<float>("GetPlayerSanity", new Func<float>(this.GetPlayerSanity));
		base.AddCommandHandler("RequestImmediateShopRestock", new Action(this.RequestImmediateShopRestock));
		base.AddCommandHandler("EndDemo", new Action(this.EndDemo));
		base.AddFunction<int>("GetOozePatchArea", new Func<int>(this.GetOozePatchArea));
		base.AddFunction<bool>("GetIsAnyOozeInWorld", new Func<bool>(this.GetIsAnyOozeInWorld));
		base.AddCommandHandler("ShowRigTentacles", new Action(this.ShowRigTentacles));
		base.AddCommandHandler<bool>("ToggleFreezeTime", new Action<bool>(this.ToggleFreezeTime));
		base.AddCommandHandler<string, bool>("SetBoolVariable", new Action<string, bool>(this.SetBoolVariable));
		base.AddFunction<string, bool>("GetBoolVariable", new Func<string, bool>(this.GetBoolVariable));
		base.AddCommandHandler<string, int>("SetIntVariable", new Action<string, int>(this.SetIntVariable));
		base.AddCommandHandler<string, int>("AdjustIntVariable", new Action<string, int>(this.AdjustIntVariable));
		base.AddFunction<string, int>("GetIntVariable", new Func<string, int>(this.GetIntVariable));
		base.AddFunction<int, bool>("GetHasPaint", new Func<int, bool>(this.GetHasPaint));
		base.AddFunction<int, bool>("GetHasFlag", new Func<int, bool>(this.GetHasFlag));
		base.AddFunction<int>("GetIdOfFlagInInventoryAndStorage", new Func<int>(this.GetIdOfFlagInInventoryAndStorage));
		base.AddCommandHandler<string, bool>("SetHasFlag", new Action<string, bool>(this.SetHasFlag));
		base.AddCommandHandler<string, bool>("SetHasPaint", new Action<string, bool>(this.SetHasPaint));
		base.AddCommandHandler<string, bool>("AddMapMarker", new Action<string, bool>(this.AddMapMarker));
		base.AddCommandHandler<string>("RemoveMapMarker", new Action<string>(this.RemoveMapMarker));
		base.AddCommandHandler<float>("wait", (float param) => base.StartCoroutine(this.Wait(param)));
		this.onDialogueComplete.AddListener(new UnityAction(this.FireDialogueCompletedEvent));
		this.onNodeStart.AddListener(new UnityAction<string>(this.RecordNodeStart));
	}

	private void RecordNodeStart(string nodeName)
	{
		this.PreviousNodeName = nodeName;
	}

	public IEnumerator Wait(float duration)
	{
		yield return new WaitForSeconds(duration);
		yield break;
	}

	private void OnEnable()
	{
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
	}

	private void AddMapMarker(string name, bool seen)
	{
		GameManager.Instance.SaveData.AddMapMarker(name, seen);
	}

	private void RemoveMapMarker(string name)
	{
		GameManager.Instance.SaveData.RemoveMapMarker(name);
	}

	private void IncreaseWorldPhase()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		int worldPhase = saveData.WorldPhase;
		saveData.WorldPhase = worldPhase + 1;
		GameEvents.Instance.TriggerWorldPhaseChanged(GameManager.Instance.SaveData.WorldPhase);
	}

	private void IncreaseTPRWorldPhase()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		int tprworldPhase = saveData.TPRWorldPhase;
		saveData.TPRWorldPhase = tprworldPhase + 1;
	}

	private void SetTIRWorldPhase(int phase)
	{
		GameManager.Instance.SaveData.TIRWorldPhase = phase;
		GameEvents.Instance.TriggerTIRWorldPhaseChanged(phase);
	}

	private int GetTIRWorldPhase()
	{
		return GameManager.Instance.SaveData.TIRWorldPhase;
	}

	private int GetTPRWorldPhase()
	{
		return GameManager.Instance.SaveData.TPRWorldPhase;
	}

	private void RecordRelicHandedIn()
	{
		AutoSplitterData.relicsRelinquished++;
	}

	private void DetonateExplosives()
	{
		if (this.CurrentExplosivePOI != null)
		{
			this.CurrentExplosivePOI.Detonate();
		}
	}

	private void RefreshDockVCams()
	{
		GameManager.Instance.UI.DockUI.RefreshVCams();
	}

	private void ToggleFreezeTime(bool freeze)
	{
		GameManager.Instance.Time.ToggleFreezeTime(freeze);
	}

	private void SetBoolVariable(string key, bool val)
	{
		GameManager.Instance.SaveData.SetBoolVariable(key, val);
	}

	private bool GetBoolVariable(string key)
	{
		return GameManager.Instance.SaveData.GetBoolVariable(key, false);
	}

	private void SetIntVariable(string key, int val)
	{
		GameManager.Instance.SaveData.SetIntVariable(key, val);
	}

	private int GetIntVariable(string key)
	{
		return GameManager.Instance.SaveData.GetIntVariable(key, 0);
	}

	private void AdjustIntVariable(string key, int change)
	{
		GameManager.Instance.SaveData.AdjustIntVariable(key, change);
	}

	private void SetDestinationAvailable(string destinationId, bool available)
	{
		if (available)
		{
			GameManager.Instance.SaveData.availableDestinations.Add(destinationId);
			return;
		}
		GameManager.Instance.SaveData.availableDestinations.Remove(destinationId);
	}

	public bool GetSpeakerAvailability(string speakerId)
	{
		return GameManager.Instance.SaveData.availableSpeakers.Contains(speakerId);
	}

	public bool GetHasPaint(int paintId)
	{
		return GameManager.Instance.SaveData.GetBoolVariable(string.Format("has-paint-{0}", paintId), false);
	}

	private void SetHasPaint(string paintId, bool hasPaint)
	{
		GameManager.Instance.SaveData.SetBoolVariable("has-paint-" + paintId, hasPaint);
	}

	public bool GetHasFlag(int flagId)
	{
		return GameManager.Instance.SaveData.GetBoolVariable(string.Format("has-flag-{0}", flagId), false);
	}

	public int GetIdOfFlagInInventoryAndStorage()
	{
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.GENERAL, ItemSubtype.GENERAL);
		allItemsOfType.AddRange(GameManager.Instance.SaveData.Storage.GetAllItemsOfType<SpatialItemInstance>(ItemType.GENERAL, ItemSubtype.GENERAL));
		int num = 0;
		int num2 = 7;
		int num3 = 0;
		while (num3 < allItemsOfType.Count && num == 0)
		{
			int num4 = 1;
			while (num4 <= num2 && num == 0)
			{
				if (allItemsOfType[num3].id == string.Format("flag-{0}", num4))
				{
					num = num4;
				}
				num4++;
			}
			num3++;
		}
		return num;
	}

	private void SetHasFlag(string flagId, bool hasFlag)
	{
		GameManager.Instance.SaveData.SetBoolVariable("has-flag-" + flagId, hasFlag);
	}

	public string GetCurrentZone()
	{
		return GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone().ToString();
	}

	public int GetWorldPhase()
	{
		return GameManager.Instance.SaveData.WorldPhase;
	}

	private void SetSpeakerAvailability(string speakerId, bool available)
	{
		if (available)
		{
			GameManager.Instance.SaveData.availableSpeakers.Add(speakerId);
			return;
		}
		GameManager.Instance.SaveData.availableSpeakers.Remove(speakerId);
	}

	private void SetIcebreakerEquipped(bool isEquipped)
	{
		GameManager.Instance.SaveData.SetBoolVariable(BoatSubModelToggler.ICEBREAKER_EQUIP_STRING_KEY, isEquipped);
		GameEvents.Instance.TriggerIcebreakerEquipChanged();
	}

	private void SetQuestStepCompleted(string questStepId)
	{
		GameManager.Instance.QuestManager.CompleteQuestStep(questStepId, false, false);
	}

	private void SetQuestStepCompletedSilent(string questStepId)
	{
		GameManager.Instance.QuestManager.CompleteQuestStep(questStepId, true, false);
	}

	private void SetQuestAvailable(string questId)
	{
		GameManager.Instance.QuestManager.OfferQuest(questId);
	}

	private void SetQuestStarted(string questId)
	{
		GameManager.Instance.QuestManager.StartQuest(questId, false);
	}

	private void SetQuestStartedSilent(string questId)
	{
		GameManager.Instance.QuestManager.StartQuest(questId, true);
	}

	private void SetQuestCompleted(string questId)
	{
		GameManager.Instance.QuestManager.CompleteQuest(questId, 0, false);
	}

	private void SetQuestCompletedWithResolution(string questId, int resolutionIndex)
	{
		GameManager.Instance.QuestManager.CompleteQuest(questId, resolutionIndex, false);
	}

	private bool GetIsQuestCompleted(string questId)
	{
		return GameManager.Instance.QuestManager.IsQuestCompleted(questId);
	}

	private bool GetIsQuestStarted(string questId)
	{
		return GameManager.Instance.QuestManager.IsQuestStarted(questId);
	}

	private bool GetIsQuestCanBeStarted(string questId)
	{
		return !this.GetIsQuestStarted(questId) && !this.GetIsQuestCompleted(questId);
	}

	private bool GetIsQuestInProgress(string questId)
	{
		return this.GetIsQuestStarted(questId) && !this.GetIsQuestCompleted(questId);
	}

	private bool GetIsQuestAvailable(string questId)
	{
		return GameManager.Instance.QuestManager.IsQuestAvailable(questId);
	}

	private bool GetIsQuestInactive(string questId)
	{
		return GameManager.Instance.QuestManager.IsQuestInactive(questId);
	}

	private bool GetIsQuestStepActive(string stepId)
	{
		return GameManager.Instance.QuestManager.GetIsQuestStepActive(stepId);
	}

	private bool GetIsQuestStepCompleted(string stepId)
	{
		return GameManager.Instance.QuestManager.GetIsQuestStepCompleted(stepId);
	}

	public bool GetHasSpaceForItem(string id)
	{
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id);
		if (!itemDataById || itemDataById is NonSpatialItemData)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] GetHasSpaceForItem(" + id + ") could not find SpatialItemData with that id.");
			return false;
		}
		Vector3Int zero = Vector3Int.zero;
		return GameManager.Instance.SaveData.Inventory.FindPositionForObject(itemDataById as SpatialItemData, out zero, 0, false);
	}

	public void SetTemporalMarker(string id, bool replace)
	{
		GameManager.Instance.SaveData.RecordTemporalMarker(id, GameManager.Instance.Time.TimeAndDay, replace);
	}

	public void RemoveTemporalMarker(string id)
	{
		GameManager.Instance.SaveData.RemoveTemporalMarker(id);
	}

	public IEnumerator PassTime(int hours, string reasonKey)
	{
		GameManager.Instance.Time.ForcefullyPassTime((float)hours, reasonKey, TimePassageMode.OTHER);
		yield return new WaitUntil(() => !GameManager.Instance.Time.IsTimePassingForcefully());
		yield break;
	}

	public IEnumerator PassTimeUntil(int hour, string reasonKey)
	{
		float num = (float)hour * 0.041666668f;
		float num2 = 0f;
		float num3 = GameManager.Instance.Time.Time % 1f;
		if (num3 > num)
		{
			num2 = 1f - num3 + num;
		}
		else if (num3 < num)
		{
			num2 = num3 - num;
		}
		else if (num3 == num)
		{
			num2 = 0f;
		}
		num2 /= 0.041666668f;
		GameManager.Instance.Time.ForcefullyPassTime(num2, reasonKey, TimePassageMode.OTHER);
		yield return new WaitUntil(() => !GameManager.Instance.Time.IsTimePassingForcefully());
		yield break;
	}

	public int GetDayPeriod()
	{
		return Mathf.FloorToInt(GameManager.Instance.Time.Time / 0.25f);
	}

	public int GetHour()
	{
		return Mathf.FloorToInt(GameManager.Instance.Time.Time % 1f / 0.041666668f);
	}

	public bool GetTemporalMarkerExists(string id)
	{
		return GameManager.Instance.SaveData.GetTemporalMarkerById(id) != null;
	}

	public int GetHoursSinceTemporalMarker(string id)
	{
		SerializedTemporalMarker temporalMarkerById = GameManager.Instance.SaveData.GetTemporalMarkerById(id);
		if (temporalMarkerById == null)
		{
			return int.MaxValue;
		}
		float timeAndDay = GameManager.Instance.Time.TimeAndDay;
		float timeAndDay2 = temporalMarkerById.timeAndDay;
		return Mathf.RoundToInt((timeAndDay - timeAndDay2) * 24f);
	}

	public int GetDaysSinceTemporalMarker(string id)
	{
		SerializedTemporalMarker temporalMarkerById = GameManager.Instance.SaveData.GetTemporalMarkerById(id);
		if (temporalMarkerById == null)
		{
			return int.MaxValue;
		}
		float timeAndDay = GameManager.Instance.Time.TimeAndDay;
		float timeAndDay2 = temporalMarkerById.timeAndDay;
		return Mathf.FloorToInt(timeAndDay) - Mathf.FloorToInt(timeAndDay2);
	}

	public IEnumerator ShowQuestGrid(string param)
	{
		QuestGridConfig questGridConfig = GameManager.Instance.DataLoader.allQuestGrids.Find((QuestGridConfig c) => c.name == param);
		if (questGridConfig == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] ShowQuestGrid(" + param + ") could not find matching QuestGridConfig");
			yield break;
		}
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		GameManager.Instance.UI.SetIsShowingQuestGrid(true);
		GameManager.Instance.UI.QuestGridPanel.Show(questGridConfig, true);
		List<int> list = new List<int> { 0, 1, 2 };
		if (questGridConfig.allowStorageAccess)
		{
			list.Add(3);
		}
		GameManager.Instance.UI.ToggleInventoryAll(true, true, list);
		bool complete = false;
		Action<int> ExitResponseFunction = delegate(int response)
		{
			this.LastQuestGridResult = response;
			complete = true;
		};
		QuestGridPanel questGridPanel = GameManager.Instance.UI.QuestGridPanel;
		questGridPanel.ExitEvent = (Action<int>)Delegate.Combine(questGridPanel.ExitEvent, ExitResponseFunction);
		yield return new WaitUntil(() => complete);
		QuestGridPanel questGridPanel2 = GameManager.Instance.UI.QuestGridPanel;
		questGridPanel2.ExitEvent = (Action<int>)Delegate.Remove(questGridPanel2.ExitEvent, ExitResponseFunction);
		GameManager.Instance.UI.QuestGridPanel.Hide();
		GameManager.Instance.UI.ToggleInventoryAll(false, false, null);
		GameManager.Instance.UI.SetIsShowingQuestGrid(false);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
		yield break;
	}

	public void ClearQuestGrid(string questGridId)
	{
		QuestGridConfig questGridConfig = GameManager.Instance.DataLoader.allQuestGrids.Find((QuestGridConfig c) => c.name == questGridId);
		if (questGridConfig == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] ClearQuestGrid(" + questGridId + ") could not find matching QuestGridConfig");
			return;
		}
		GridKey gridKey = questGridConfig.gridKey;
		if (gridKey != GridKey.NONE)
		{
			SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(gridKey);
			if (gridByKey != null)
			{
				gridByKey.Clear(true);
			}
		}
	}

	public void MakeBait()
	{
		SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(GridKey.BAIT_INPUT);
		SerializableGrid gridByKey2 = GameManager.Instance.SaveData.GetGridByKey(GridKey.BAIT_OUTPUT);
		if (gridByKey != null)
		{
			int num = gridByKey.spatialItems.Count<SpatialItemInstance>();
			gridByKey.Clear(true);
			SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>("bait");
			for (int i = 0; i < num; i++)
			{
				gridByKey2.FindSpaceAndAddObjectToGridData(itemDataById, false, null);
			}
		}
	}

	public void ShowPortrait(string id)
	{
		GameManager.Instance.UI.ShowDialoguePortrait(id);
	}

	public void HidePortrait()
	{
		GameManager.Instance.UI.HideDialoguePortrait();
	}

	public void ClearDialogue()
	{
		GameManager.Instance.UI.ClearDialogue();
	}

	public void ChangeBoatColor(int area, int index)
	{
		if (area == 0)
		{
			GameManager.Instance.SaveData.RoofColorIndex = index;
		}
		else if (area == 1)
		{
			GameManager.Instance.SaveData.HullColorIndex = index;
		}
		GameManager.Instance.SaveData.HasChangedBoatColors = true;
		GameEvents.Instance.TriggerBoatColorsChanged(area, index);
	}

	public void ChangeBoatFlag(int flagIndex)
	{
		GameManager.Instance.SaveData.BoatFlagStyle = flagIndex;
		GameEvents.Instance.TriggerBoatFlagChanged(flagIndex);
	}

	public void ChangeBoatBunting(int buntingNum)
	{
		GameManager.Instance.SaveData.IsBoatBuntingEnabled = buntingNum != 0;
		GameEvents.Instance.TriggerBoatBuntingChanged(buntingNum != 0);
	}

	public void ChangeBoatHorn(int hornNum)
	{
		GameManager.Instance.SaveData.SetIntVariable("foghorn-style-index", hornNum);
		GameEvents.Instance.TriggerFoghornStyleChanged();
	}

	public void SellAllItemsInQuestGrid(string questGridId, float sellModifier)
	{
		QuestGridConfig questGridConfig = GameManager.Instance.DataLoader.allQuestGrids.Find((QuestGridConfig c) => c.name == questGridId);
		if (questGridConfig == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] SellAllItemsInQuestGrid(" + questGridId + ") could not find matching QuestGridConfig");
			return;
		}
		GridKey gridKey = questGridConfig.gridKey;
		if (gridKey != GridKey.NONE)
		{
			SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(gridKey);
			if (gridByKey != null)
			{
				GameManager.Instance.ItemManager.SellItems(gridByKey.spatialItems, sellModifier, false);
			}
		}
	}

	public int GetLastQuestGridResult()
	{
		return this.LastQuestGridResult;
	}

	public bool GetHasDeluxeEntitlement()
	{
		return GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DELUXE);
	}

	public decimal GetDockProgressRemainingRaw(string dockProgressTypeString)
	{
		int num = (int)((DockProgressType)Enum.Parse(typeof(DockProgressType), dockProgressTypeString));
		decimal num2 = 0m;
		decimal num3 = 0m;
		if (num == 1)
		{
			num2 = GameManager.Instance.SaveData.GreaterMarrowRepayments;
			num3 = GameManager.Instance.GameConfigData.GreaterMarrowDebt;
		}
		return num3 - num2;
	}

	public void ConstructBuildingTier(string buildingTierId)
	{
		GameManager.Instance.ConstructableBuildingManager.SetIsBuildingConstructed(buildingTierId, true, true);
	}

	public bool GetIsBuildingTierConstructed(string buildingTierId)
	{
		BuildingTierId buildingTierId2 = (BuildingTierId)Enum.Parse(typeof(BuildingTierId), buildingTierId);
		return GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(buildingTierId2);
	}

	public bool GetCanBuildingTierBeConstructed(string buildingTierId)
	{
		BuildingTierId buildingTierId2 = (BuildingTierId)Enum.Parse(typeof(BuildingTierId), buildingTierId);
		return GameManager.Instance.ConstructableBuildingManager.GetCanBuildingBeConstructed(buildingTierId2);
	}

	public int GetActiveTrapState(int id)
	{
		return GameManager.Instance.SaveData.GetIntVariable(string.Format("trap-{0}-state", id), 0);
	}

	public void SetActiveTrapState(int id, int state)
	{
		GameManager.Instance.SaveData.SetIntVariable(string.Format("trap-{0}-state", id), state);
	}

	public IEnumerator ShowAndWaitForAnimation(string animationId)
	{
		bool complete = false;
		Action<string> AnimationCompleteFunction = delegate(string animationCompleteId)
		{
			if (animationCompleteId == animationId)
			{
				complete = true;
			}
		};
		GameEvents.Instance.OnAnimationCompleted += AnimationCompleteFunction;
		GameEvents.Instance.TriggerAnimationStartRequested(animationId);
		ActionLayer prevActionLayer = GameManager.Instance.Input.GetActiveActionLayer();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.NONE);
		yield return new WaitUntil(() => complete);
		GameManager.Instance.Input.SetActiveActionLayer(prevActionLayer);
		GameEvents.Instance.OnAnimationCompleted -= AnimationCompleteFunction;
		yield break;
	}

	public decimal GetDockProportionalProgress(string dockProgressTypeString)
	{
		int num = (int)((DockProgressType)Enum.Parse(typeof(DockProgressType), dockProgressTypeString));
		decimal num2 = 0m;
		decimal num3 = 0m;
		if (num == 1)
		{
			num2 = GameManager.Instance.SaveData.GreaterMarrowRepayments;
			num3 = GameManager.Instance.GameConfigData.GreaterMarrowDebt;
		}
		return num2 / num3;
	}

	public void RepayDebt(string dockProgressTypeString, decimal debtRepaymentAmount)
	{
		if ((DockProgressType)Enum.Parse(typeof(DockProgressType), dockProgressTypeString) == DockProgressType.GM_REPAYMENTS)
		{
			if (debtRepaymentAmount == -1m)
			{
				debtRepaymentAmount = GameManager.Instance.GameConfigData.GreaterMarrowDebt - GameManager.Instance.SaveData.GreaterMarrowRepayments;
			}
			if (GameManager.Instance.SaveData.Funds >= debtRepaymentAmount)
			{
				GameManager.Instance.AddFunds(-debtRepaymentAmount);
				GameManager.Instance.SaveData.GreaterMarrowRepayments += debtRepaymentAmount;
				string text = debtRepaymentAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
				string text2 = string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
					">-$",
					text,
					"</color>"
				});
				GameManager.Instance.UI.ShowNotification(NotificationType.DEBT_REPAID, "notification.debt-adjusted", new object[] { text2 });
			}
		}
	}

	private void ChangeWeather(string weatherName)
	{
		GameManager.Instance.WeatherController.ChangeWeather(weatherName);
	}

	private void EmitLightning()
	{
		GameManager.Instance.WeatherController.EmitLightning();
	}

	private void DoFinalePreparations()
	{
		GameManager.Instance.SaveData.ForbidSave = true;
		GameManager.Instance.Time.ToggleFreezeTime(true);
		GameEvents.Instance.TriggerFinaleVoyageStarted();
	}

	private void DoFinaleCutscenePreparations()
	{
		GameEvents.Instance.TriggerFinaleCutsceneStarted();
	}

	private bool GetIsDemoMode()
	{
		return false;
	}

	private float GetPlayerSanity()
	{
		return GameManager.Instance.Player.Sanity.CurrentSanity;
	}

	private void EndDemo()
	{
		ApplicationEvents.Instance.TriggerDemoEndToggled(true);
	}

	private int GetOozePatchArea()
	{
		int num = GameManager.Instance.OozePatchManager.GetAreaIndexWithOozePresent();
		if (num == -1)
		{
			num = GameManager.Instance.OozePatchManager.RepopulateRandomAreaWithOoze();
		}
		return num;
	}

	private bool GetIsAnyOozeInWorld()
	{
		return GameManager.Instance.OozePatchManager.GetAreaIndexWithOozePresent() != -1;
	}

	private void RequestImmediateShopRestock()
	{
		GameEvents.Instance.TriggerShopRestockRequested();
	}

	public int GetNumDamage()
	{
		return GameManager.Instance.SaveData.GetNumberOfDamagedSlots();
	}

	public int GetNumDamagedDeployables()
	{
		return GameManager.Instance.ItemManager.GetDamagedDeployables().Count;
	}

	public int GetNumItemInInventoryById(string id)
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Inventory);
	}

	public bool GetDidAddEnoughFishToMakeBait()
	{
		int cellCount = 0;
		GameManager.Instance.SaveData.GetGridByKey(GridKey.BAIT_INPUT).spatialItems.ForEach(delegate(SpatialItemInstance i)
		{
			cellCount += i.GetItemData<SpatialItemData>().GetSize();
		});
		return cellCount >= 1;
	}

	public bool TryConvertDogTags()
	{
		string id = GameManager.Instance.ResearchHelper.ResearchItemData.id;
		string dogTagId = "quest-dog-tag";
		SerializableGrid inputGrid = GameManager.Instance.SaveData.GetGridByKey(GridKey.SOLDIER_DOG_TAG_INPUT);
		SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(GridKey.SOLDIER_DOG_TAG_OUTPUT);
		int count = inputGrid.spatialItems.Where((SpatialItemInstance i) => i.id == dogTagId).ToList<SpatialItemInstance>().Count;
		if (count <= 0)
		{
			return false;
		}
		List<SpatialItemInstance> itemsToRemove = new List<SpatialItemInstance>();
		inputGrid.spatialItems.ForEach(delegate(SpatialItemInstance itemInstance)
		{
			if (itemInstance.id == dogTagId)
			{
				itemsToRemove.Add(itemInstance);
			}
		});
		itemsToRemove.ForEach(delegate(SpatialItemInstance i)
		{
			inputGrid.RemoveObjectFromGridData(i, false);
		});
		GameManager.Instance.SaveData.AdjustIntVariable("dog-tags-returned", count);
		int num = Mathf.Min(count, gridByKey.GridConfiguration.GetSize());
		SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>(id);
		for (int j = 0; j < num; j++)
		{
			Vector3Int vector3Int;
			if (gridByKey.FindPositionForObject(itemDataById, out vector3Int, 0, false))
			{
				SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemDataById);
				gridByKey.AddObjectToGridData(spatialItemInstance, vector3Int, false, null);
			}
		}
		return true;
	}

	public int GetNumItemInInventoryAndNetById(string id)
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Inventory) + GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.TrawlNet);
	}

	public int GetNumItemInInventoryAndStorageById(string id)
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Inventory) + GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Storage);
	}

	public int GetNumItemInStorageById(string id)
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Storage);
	}

	public int GetNumItemAnywhereById(string id)
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Inventory) + GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.TrawlNet) + GameManager.Instance.SaveData.GetNumItemInGridById(id, GameManager.Instance.SaveData.Storage);
	}

	public int GetNumFishByIdAndCondition(string id, int conditionMin, int conditionMax)
	{
		if (GameManager.Instance.ItemManager.GetItemDataById<FishItemData>(id) == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] GetNumFishByIdAndCondition(" + id + ") could not find ItemData with that id.");
			return 0;
		}
		return (from i in GameManager.Instance.SaveData.Inventory.spatialItems.OfType<FishItemInstance>()
			where i.id == id && i.freshness < (float)conditionMax && i.freshness >= (float)conditionMin
			select i).Count<FishItemInstance>();
	}

	public bool GetCanAffordItemById(string itemId)
	{
		bool flag = true;
		SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>(itemId);
		if (itemDataById)
		{
			flag = GameManager.Instance.SaveData.Funds >= itemDataById.value;
		}
		return flag;
	}

	public decimal GetFunds()
	{
		return GameManager.Instance.SaveData.Funds;
	}

	public void ChangeFunds(decimal changeAmount)
	{
		string text = changeAmount.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		GameManager.Instance.AddFunds(changeAmount);
		if (changeAmount > 0m)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_GAINED, "notification.funds-added", new object[] { string.Concat(new string[]
			{
				"<color=#",
				GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
				">$",
				text,
				"</color>"
			}) });
			return;
		}
		if (changeAmount < 0m)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_LOST, "notification.funds-removed", new object[] { string.Concat(new string[]
			{
				"<color=#",
				GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
				">$",
				text,
				"</color>"
			}) });
		}
	}

	public int GetNumItemsByType(string typeString)
	{
		ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), typeString);
		return GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(itemType, ItemSubtype.NONE).Count;
	}

	public bool GetHasEquipmentForHarvestType(string harvestTypeString)
	{
		HarvestableType harvestableType = (HarvestableType)Enum.Parse(typeof(HarvestableType), harvestTypeString);
		return GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(harvestableType, false);
	}

	public int GetNumTrophyFish()
	{
		return this.FindAllFishWithCondition((FishItemInstance fish) => fish.IsTrophySize()).Count;
	}

	public int GetNumAberrantFish()
	{
		return this.FindAllFishWithCondition((FishItemInstance fish) => fish.IsAberrant()).Count;
	}

	public int GetNumAberrantFishInInventory()
	{
		return (from fish in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH)
			where fish.IsAberrant()
			select fish).ToList<FishItemInstance>().Count;
	}

	public int GetNumItemsByTypeAndSubtype(string typeString, string subtypeString)
	{
		ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), typeString);
		ItemSubtype itemSubtype = (ItemSubtype)Enum.Parse(typeof(ItemSubtype), subtypeString);
		return GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(itemType, itemSubtype).Count;
	}

	public int GetNumItemsByTypeAndSubtypeIncludingStorage(string typeString, string subtypeString)
	{
		ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), typeString);
		ItemSubtype itemSubtype = (ItemSubtype)Enum.Parse(typeof(ItemSubtype), subtypeString);
		return GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(itemType, itemSubtype).Count + GameManager.Instance.SaveData.Storage.GetAllItemsOfType<SpatialItemInstance>(itemType, itemSubtype).Count;
	}

	public int GetNumOwnedItemsThatCanCatchFish()
	{
		int num = 0;
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD);
		List<SpatialItemInstance> allItemsOfType2 = GameManager.Instance.SaveData.Storage.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD);
		return num + allItemsOfType.Count((SpatialItemInstance rodItem) => rodItem.GetItemData<RodItemData>().harvestableTypes.Length != 0) + allItemsOfType2.Count((SpatialItemInstance rodItem) => rodItem.GetItemData<RodItemData>().harvestableTypes.Length != 0) + GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.POT).Count + GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NET).Count;
	}

	public int GetNumItemInGridById(string itemId, string gridKeyString)
	{
		int num = 0;
		GridKey gridKey = (GridKey)Enum.Parse(typeof(GridKey), gridKeyString);
		SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(gridKey);
		if (gridByKey != null)
		{
			num = gridByKey.spatialItems.Count((SpatialItemInstance i) => i.id == itemId);
		}
		return num;
	}

	public bool GetHasVisitedNode(string nodeName)
	{
		return GameManager.Instance.SaveData.visitedNodes.Contains(nodeName);
	}

	public int GetNumDaysPassed()
	{
		return GameManager.Instance.Time.Day;
	}

	public int GetDaysSinceFirstVisit(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		int num = GameManager.Instance.Time.Day;
		for (int i = 0; i < shopHistoryById.visitDays.Count; i++)
		{
			if (shopHistoryById.visitDays[i] < num)
			{
				num = shopHistoryById.visitDays[i];
			}
		}
		return GameManager.Instance.Time.Day - num;
	}

	public int GetDaysSinceLastVisit(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < shopHistoryById.visitDays.Count; i++)
		{
			if (shopHistoryById.visitDays[i] > num)
			{
				num = shopHistoryById.visitDays[i];
			}
		}
		return GameManager.Instance.Time.Day - num;
	}

	public int GetDaysSinceFirstTransaction(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		int num = GameManager.Instance.Time.Day;
		for (int i = 0; i < shopHistoryById.transactionDays.Count; i++)
		{
			if (shopHistoryById.transactionDays[i] < num)
			{
				num = shopHistoryById.transactionDays[i];
			}
		}
		return GameManager.Instance.Time.Day - num;
	}

	public int GetDaysSinceLastTransaction(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < shopHistoryById.transactionDays.Count; i++)
		{
			if (shopHistoryById.transactionDays[i] > num)
			{
				num = shopHistoryById.transactionDays[i];
			}
		}
		return GameManager.Instance.Time.Day - num;
	}

	public int GetNumShopVisits(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		return shopHistoryById.visits;
	}

	public decimal GetShopTransactionTotalById(string shopId)
	{
		return GameManager.Instance.SaveData.GetShopTransactionTotalById(shopId);
	}

	public int GetNumShopVisitDaysUnique(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		return shopHistoryById.visitDays.Count;
	}

	public int GetNumShopTransactionDaysUnique(string shopId)
	{
		SerializedShopHistory shopHistoryById = GameManager.Instance.SaveData.GetShopHistoryById(shopId);
		if (shopHistoryById == null)
		{
			return 0;
		}
		return shopHistoryById.transactionDays.Count;
	}

	public int GetNumItemsSoldById(string itemId)
	{
		SerializedItemTransaction itemTransactionById = GameManager.Instance.SaveData.GetItemTransactionById(itemId);
		if (itemTransactionById != null)
		{
			return itemTransactionById.sold;
		}
		return 0;
	}

	public bool GetHasEverOwnedItem(string itemId)
	{
		return GameManager.Instance.SaveData.historyOfItemsOwned.Contains(itemId);
	}

	public void AddItemById(string id)
	{
		this.AddItemById(id, GameManager.Instance.SaveData.Inventory, 1);
	}

	public void AddItemById(string id, SerializableGrid grid, int quantity = 1)
	{
		for (int i = 0; i < quantity; i++)
		{
			ItemInstance itemInstance = GameManager.Instance.ItemManager.AddItemById(id, grid, true);
			if (itemInstance is SpatialItemInstance)
			{
				GameEvents.Instance.TriggerItemAddedEvent(itemInstance as SpatialItemInstance, true);
			}
			if (itemInstance is FishItemInstance && (itemInstance as FishItemInstance).GetItemData<FishItemData>().IsAberration)
			{
				SaveData saveData = GameManager.Instance.SaveData;
				int numAberrationsCaught = saveData.NumAberrationsCaught;
				saveData.NumAberrationsCaught = numAberrationsCaught + 1;
			}
		}
	}

	public void SellItemById(string id, int count = 1, float valueMultiplier = 1f)
	{
		if (GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id) == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] SellItemById() could not find ItemData with id: " + id + ".");
			return;
		}
		if (count == -1)
		{
			count = this.GetNumItemInInventoryById(id);
		}
		List<SpatialItemInstance> list = GameManager.Instance.SaveData.Inventory.spatialItems.Where((SpatialItemInstance i) => i.id == id).ToList<SpatialItemInstance>();
		list.AddRange(GameManager.Instance.SaveData.TrawlNet.spatialItems.FindAll((SpatialItemInstance s) => s.id == id));
		GameManager.Instance.ItemManager.SellItems(list.Take(count).ToList<SpatialItemInstance>(), valueMultiplier, true);
	}

	public void SellSingleTrophyFish(float valueMultiplier = 1f)
	{
		FishItemInstance fishItemInstance = this.FindFirstFishWithCondition((FishItemInstance fish) => fish.IsTrophySize());
		if (!(fishItemInstance == null))
		{
			GameManager.Instance.ItemManager.SellItems(new List<SpatialItemInstance> { fishItemInstance }, valueMultiplier, true);
		}
	}

	public void SellSingleAberrantFish(float valueMultiplier = 1f)
	{
		FishItemInstance fishItemInstance = this.FindFirstFishWithCondition((FishItemInstance fish) => fish.IsAberrant());
		if (!(fishItemInstance == null))
		{
			GameManager.Instance.ItemManager.SellItems(new List<SpatialItemInstance> { fishItemInstance }, valueMultiplier, true);
		}
	}

	private FishItemInstance FindFirstFishWithCondition(Func<FishItemInstance, bool> condition)
	{
		FishItemInstance fishItemInstance = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).FirstOrDefault(condition);
		if (fishItemInstance == null)
		{
			fishItemInstance = GameManager.Instance.SaveData.TrawlNet.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).FirstOrDefault(condition);
		}
		return fishItemInstance;
	}

	private List<FishItemInstance> FindAllFishWithCondition(Func<FishItemInstance, bool> condition)
	{
		List<FishItemInstance> list = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).Where(condition).ToList<FishItemInstance>();
		list.AddRange(GameManager.Instance.SaveData.TrawlNet.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).Where(condition));
		list.AddRange(GameManager.Instance.SaveData.Storage.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).Where(condition));
		return list;
	}

	public void RemoveItemById(string id, int removeCount = 1)
	{
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id);
		if (itemDataById == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] RemoveItemById(" + id + ") could not find ItemData with that id.");
			return;
		}
		if (removeCount == -1)
		{
			removeCount = this.GetNumItemInInventoryById(id);
		}
		if (itemDataById is SpatialItemData)
		{
			GameManager.Instance.SaveData.Inventory.spatialItems.Where((SpatialItemInstance i) => i.id == id).Take(removeCount).ToList<SpatialItemInstance>()
				.ForEach(delegate(SpatialItemInstance i)
				{
					GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(i, true);
				});
		}
		else if (itemDataById is NonSpatialItemData)
		{
			GameManager.Instance.SaveData.ownedNonSpatialItems.Where((NonSpatialItemInstance i) => i.id == id).Take(removeCount).ToList<NonSpatialItemInstance>()
				.ForEach(delegate(NonSpatialItemInstance i)
				{
					GameManager.Instance.SaveData.ownedNonSpatialItems.Remove(i);
				});
		}
		GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_HANDED_IN, "notification.item-removed", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
	}

	public void RemoveItemByIdInInventoryAndStorage(string id)
	{
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id);
		if (itemDataById == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] RemoveItemById(" + id + ") could not find ItemData with that id.");
			return;
		}
		GameManager.Instance.SaveData.Inventory.spatialItems.Where((SpatialItemInstance i) => i.id == id).ToList<SpatialItemInstance>().ForEach(delegate(SpatialItemInstance i)
		{
			GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(i, true);
		});
		GameManager.Instance.SaveData.Storage.spatialItems.Where((SpatialItemInstance i) => i.id == id).ToList<SpatialItemInstance>().ForEach(delegate(SpatialItemInstance i)
		{
			GameManager.Instance.SaveData.Storage.RemoveObjectFromGridData(i, true);
		});
		GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_HANDED_IN, "notification.item-removed", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
	}

	public void RemoveFishByIdAndCondition(string id, int conditionMin, int conditionMax, int removeCount)
	{
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(id);
		if (itemDataById == null)
		{
			CustomDebug.EditorLogError("[DredgeDialogueRunner] RemoveFishByIdAndCondition(" + id + ") could not find ItemData with that id.");
			return;
		}
		if (removeCount == -1)
		{
			removeCount = this.GetNumItemInInventoryById(id);
		}
		(from i in GameManager.Instance.SaveData.Inventory.spatialItems.OfType<FishItemInstance>()
			where i.id == id && i.freshness < (float)conditionMax && i.freshness >= (float)conditionMin
			select i).Take(removeCount).ToList<FishItemInstance>().ForEach(delegate(FishItemInstance i)
		{
			GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(i, true);
		});
		GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_REMOVED, "notification.item-removed", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
	}

	public void SetRollDice(int maxExclusive)
	{
		int num = global::UnityEngine.Random.Range(0, maxExclusive);
		this.inMemoryVariableStorage.SetValue("$roll", (float)num);
	}

	public void PlayClip(string clipName)
	{
		GameManager.Instance.AudioPlayer.PlaySFX(clipName, AudioLayer.SFX_UI, 1f, 1f);
	}

	public void PlayLoopingDialogueAudio(string clipName)
	{
		GameManager.Instance.AudioPlayer.PlayLoopingDialogueAudio(clipName, AudioLayer.SFX_UI, 1f);
	}

	public void StopLoopingDialogueAudio()
	{
		GameManager.Instance.AudioPlayer.StopLoopingDialogueAudio();
	}

	public void UnlockAbility(string abilityName)
	{
		GameManager.Instance.PlayerAbilities.UnlockAbility(abilityName);
	}

	public void AddItemToGrid(string itemId, string gridKeyString)
	{
		SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>(itemId);
		if (itemDataById)
		{
			GridKey gridKey = (GridKey)Enum.Parse(typeof(GridKey), gridKeyString);
			SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(gridKey);
			if (gridByKey != null)
			{
				gridByKey.FindSpaceAndAddObjectToGridData(itemDataById, false, null);
			}
		}
	}

	public void ImmediatelyResolveNextLine()
	{
		this.ShouldImmediatelyResolveNextLine = true;
		this.ShouldAutoResolveNextLine = true;
	}

	public void AutoResolveNextLine()
	{
		this.ShouldAutoResolveNextLine = true;
	}

	public void RequestExitDestination()
	{
		GameManager.Instance.UI.ExitDestinationRequested = true;
	}

	public void ShowUnavailableOptions(bool show)
	{
		this.ShouldShowUnavailableOptions = show;
	}

	private void FireDialogueCompletedEvent()
	{
		if (this.DidPlayAnyLines)
		{
			this.DidPlayAnyLines = false;
			GameEvents.Instance.TriggerDialogueCompleted();
			GameManager.Instance.AudioPlayer.PlaySFX(this.dialogueCompleteSFX, AudioLayer.SFX_UI, 1f, 1f);
			GameManager.Instance.AudioPlayer.StopLoopingDialogueAudio();
		}
	}

	private void RestartWorldMusic()
	{
		GameManager.Instance.AudioPlayer.RestartWorldMusic();
	}

	private void TransitionToAudioMixerSnapshot(string snapshotName, float durationSec)
	{
		SnapshotType snapshotType;
		if (Enum.TryParse<SnapshotType>(snapshotName, out snapshotType))
		{
			GameManager.Instance.AudioPlayer.TransitionToSnapshot(snapshotType, durationSec);
		}
	}

	private void ShowRigTentacles()
	{
		GameEvents.Instance.TriggerShowRigTentacles();
	}

	public new void StartDialogue(string nodeName)
	{
		base.StartDialogue(nodeName);
		GameEvents.Instance.TriggerDialogueStarted();
	}

	private void OnDestroy()
	{
		this.onDialogueComplete.RemoveListener(new UnityAction(this.FireDialogueCompletedEvent));
		this.onNodeStart.RemoveListener(new UnityAction<string>(this.RecordNodeStart));
		GameManager.Instance.DialogueRunner = null;
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("func.addmarker", new Action<CommandArg[]>(this.DebugAddMarker), 1, 1, "Adds a temporal marker with a string id");
			Terminal.Shell.AddCommand("func.showmarker", new Action<CommandArg[]>(this.DebugShowMarker), 1, 1, "Displays info about a temporal marker with a string id");
			Terminal.Shell.AddCommand("money.add", new Action<CommandArg[]>(this.DebugChangeFunds), 1, 1, "Changes money e.g. 'money.add -10' removes $10");
			Terminal.Shell.AddCommand("item.add", new Action<CommandArg[]>(this.DebugAddItemInventory), 1, 3, "Adds an item with a string id to player's inventory. Optional quantity param. Optional size param. e.g. 'item.add eel 2 0.9'");
			Terminal.Shell.AddCommand("item.addstore", new Action<CommandArg[]>(this.DebugAddItemStorage), 1, 3, "Adds an item with a string id to storage. Optional quantity param. Optional size param. e.g. 'item.addstore eel 2 0.9'");
			Terminal.Shell.AddCommand("item.addover", new Action<CommandArg[]>(this.DebugAddItemOverflowStorage), 1, 3, "Adds an item with a string id to overflow storage. Optional quantity param. Optional size param. e.g. 'item.addstore eel 2 0.9'");
			Terminal.Shell.AddCommand("item.addnet", new Action<CommandArg[]>(this.DebugAddItemNet), 1, 3, "Adds an item with a string id to the player's trawl net. Optional size param. e.g. 'item.addnet eel 0.9'");
			Terminal.Shell.AddCommand("catch", new Action<CommandArg[]>(this.DebugCatch), 1, 2, "Catches an item as though it was caught from a fishing spot. Optional quantity e.g. 'catch eel 1'");
			Terminal.Shell.AddCommand("item.remove", new Action<CommandArg[]>(this.DebugRemoveItem), 1, 2, "Removes a number of items with a string id and quantity (-1 for ALL). e.g. 'item.remove eel 2'");
			Terminal.Shell.AddCommand("func.numitem", new Action<CommandArg[]>(this.DebugNumItems), 1, 1, "Counts owned items with a string id");
			Terminal.Shell.AddCommand("func.canfititem", new Action<CommandArg[]>(this.DebugCanFitItem), 1, 1, "Checks to see if an item with a string id can fit in the player's inventory");
			Terminal.Shell.AddCommand("marker.add", new Action<CommandArg[]>(this.DebugAddMapMarker), 1, 1, "Adds a map marker by id e.g. 'marker.add relic1'");
			Terminal.Shell.AddCommand("marker.remove", new Action<CommandArg[]>(this.DebugRemoveMapMarker), 0, 0, "Removes a map marker by id e.g. 'marker.remove relic1'");
			Terminal.Shell.AddCommand("node.visit", new Action<CommandArg[]>(this.DebugVisitNode), 1, 1, "Adds a node id to the list of visited dialogue nodes.");
			Terminal.Shell.AddCommand("boat.color", new Action<CommandArg[]>(this.DebugChangeBoatColor), 2, 2, "Changes boat color <area: 0 - 1> <color: 0 - 7>.");
			Terminal.Shell.AddCommand("boat.flag", new Action<CommandArg[]>(this.DebugChangeBoatFlag), 1, 1, "Changes boat flag <style: 0 - 7>.");
			Terminal.Shell.AddCommand("boat.bunting", new Action<CommandArg[]>(this.DebugChangeBoatBunting), 1, 1, "Changes boat bunting <enabled: 0 - 1> .");
			Terminal.Shell.AddCommand("boat.horn", new Action<CommandArg[]>(this.DebugChangeBoatHorn), 1, 1, "Changes boat horn <pitch: 0 - 2>.");
			Terminal.Shell.AddCommand("item.add-all-cabin", new Action<CommandArg[]>(this.DebugAddAllCabinItems), 0, 0, "Adds all books and messages to cabin.");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("func.addmarker");
			Terminal.Shell.RemoveCommand("func.getmarker");
			Terminal.Shell.RemoveCommand("money.add");
			Terminal.Shell.RemoveCommand("item.add");
			Terminal.Shell.RemoveCommand("item.addstore");
			Terminal.Shell.RemoveCommand("item.addover");
			Terminal.Shell.RemoveCommand("item.addnet");
			Terminal.Shell.RemoveCommand("item.remove");
			Terminal.Shell.RemoveCommand("catch");
			Terminal.Shell.RemoveCommand("func.numitem");
			Terminal.Shell.RemoveCommand("func.canfititem");
			Terminal.Shell.RemoveCommand("func.phase");
			Terminal.Shell.RemoveCommand("marker.add");
			Terminal.Shell.RemoveCommand("marker.remove");
			Terminal.Shell.RemoveCommand("node.visit");
			Terminal.Shell.RemoveCommand("boat.color");
			Terminal.Shell.RemoveCommand("boat.flag");
			Terminal.Shell.RemoveCommand("boat.bunting");
			Terminal.Shell.RemoveCommand("boat.horn");
			Terminal.Shell.RemoveCommand("item.add-all-cabin");
		}
	}

	private void DebugVisitNode(CommandArg[] args)
	{
		GameManager.Instance.SaveData.visitedNodes.Add(args[0].String);
		GameEvents.Instance.TriggerNodeVisited(args[0].String);
	}

	private void DebugAddMapMarker(CommandArg[] args)
	{
		GameManager.Instance.SaveData.AddMapMarker(args[0].String, false);
	}

	private void DebugRemoveMapMarker(CommandArg[] args)
	{
		GameManager.Instance.SaveData.RemoveMapMarker(args[0].String);
	}

	private void DebugWorldPhase(CommandArg[] args)
	{
	}

	private void DebugAddItemInventory(CommandArg[] args)
	{
		this.DebugAddItem(args, GameManager.Instance.SaveData.Inventory);
	}

	private void DebugAddItemStorage(CommandArg[] args)
	{
		this.DebugAddItem(args, GameManager.Instance.SaveData.Storage);
	}

	private void DebugAddItemOverflowStorage(CommandArg[] args)
	{
		this.DebugAddItem(args, GameManager.Instance.SaveData.OverflowStorage);
	}

	private void DebugAddAllCabinItems(CommandArg[] args)
	{
		new List<string>
		{
			"message-1", "message-2", "message-3", "message-4", "message-5", "message-6", "message-7", "message-8", "message-9", "message-10",
			"message-fisherman-1", "message-tpr-1", "message-tpr-2", "message-tpr-3", "message-tpr-4", "message-tpr-5", "message-tpr-6", "message-tpr-7", "message-tpr-8", "message-tpr-9",
			"book-barter-1", "book-barter-2", "book-ecology-1", "book-ecology-2", "book-equip-1", "book-equip-2", "book-fishing-1", "book-fishing-2", "book-speed-1", "book-speed-2",
			"book-sanity-1", "book-sanity-2", "book-aberration-1"
		}.ForEach(delegate(string s)
		{
			this.AddItemById(s);
		});
	}

	private void DebugAddItemNet(CommandArg[] args)
	{
		if (GameManager.Instance.SaveData.EquippedTrawlNetInstance() == null)
		{
			return;
		}
		this.DebugAddItem(args, GameManager.Instance.SaveData.TrawlNet);
	}

	private void DebugAddItem(CommandArg[] args, SerializableGrid grid)
	{
		string @string = args[0].String;
		int num = 1;
		if (args.Length >= 2)
		{
			num = args[1].Int;
		}
		if (args.Length >= 3)
		{
			GameManager.Instance.ItemManager.DebugNextFishSize = args[2].Float;
		}
		if (@string == "dark-splash")
		{
			GameManager.Instance.GridManager.TryAddDarkSplashToInventory();
		}
		else
		{
			this.AddItemById(@string, grid, num);
		}
		GameManager.Instance.ItemManager.DebugNextFishSize = -1f;
	}

	private void DebugRemoveItem(CommandArg[] args)
	{
		string @string = args[0].String;
		int num = 1;
		if (args.Length != 0)
		{
			num = args[1].Int;
		}
		this.RemoveItemById(@string, num);
	}

	private void DebugCatch(CommandArg[] args)
	{
		string @string = args[0].String;
		ItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(@string);
		if (itemDataById == null || !(itemDataById is FishItemData))
		{
			CustomDebug.EditorLogError("[ItemManager] CreateItem() invalid itemId '" + @string + "'");
			return;
		}
		int num = 1;
		if (args.Length > 1)
		{
			num = args[1].Int;
		}
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		for (int i = 0; i < num; i++)
		{
			if (itemDataById is FishItemData)
			{
				FishItemInstance fishItemInstance = GameManager.Instance.ItemManager.CreateFishItem(@string, FishAberrationGenerationMode.RANDOM_CHANCE, false, FishSizeGenerationMode.ANY, 1f);
				Vector3Int vector3Int;
				if (inventory.FindPositionForObject(itemDataById as SpatialItemData, out vector3Int, 0, false))
				{
					inventory.AddObjectToGridData(fishItemInstance, vector3Int, true, null);
					GameManager.Instance.ItemManager.SetItemSeen(fishItemInstance);
					GameEvents.Instance.TriggerFishCaught();
					GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_ADDED, "notification.item-added", itemDataById.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
				}
			}
		}
	}

	private void DebugNumItems(CommandArg[] args)
	{
		string @string = args[0].String;
		this.GetNumItemInInventoryById(@string);
	}

	private void DebugCanFitItem(CommandArg[] args)
	{
		string @string = args[0].String;
		this.GetHasSpaceForItem(@string);
	}

	private void DebugAddMarker(CommandArg[] args)
	{
		string @string = args[0].String;
		this.SetTemporalMarker(@string, true);
	}

	private void DebugChangeFunds(CommandArg[] args)
	{
		float @float = args[0].Float;
		this.ChangeFunds((decimal)@float);
	}

	private void DebugShowMarker(CommandArg[] args)
	{
		string @string = args[0].String;
		this.GetHoursSinceTemporalMarker(@string);
		this.GetDaysSinceTemporalMarker(@string);
		GameManager.Instance.SaveData.GetTemporalMarkerById(@string);
	}

	private void DebugChangeBoatColor(CommandArg[] args)
	{
		int @int = args[0].Int;
		int int2 = args[1].Int;
		this.ChangeBoatColor(@int, int2);
	}

	private void DebugChangeBoatFlag(CommandArg[] args)
	{
		int @int = args[0].Int;
		this.ChangeBoatFlag(@int);
	}

	private void DebugChangeBoatBunting(CommandArg[] args)
	{
		int @int = args[0].Int;
		this.ChangeBoatBunting(@int);
	}

	private void DebugChangeBoatHorn(CommandArg[] args)
	{
		int @int = args[0].Int;
		this.ChangeBoatHorn(@int);
	}

	[SerializeField]
	private InMemoryVariableStorage inMemoryVariableStorage;

	[SerializeField]
	private AudioClip dialogueCompleteSFX;
}
