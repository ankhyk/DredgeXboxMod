using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GameEvents : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance = this;
	}

	public event Action OnGameWindowToggled;

	public void TriggerGameWindowToggled()
	{
		Action onGameWindowToggled = this.OnGameWindowToggled;
		if (onGameWindowToggled == null)
		{
			return;
		}
		onGameWindowToggled();
	}

	public event Action<bool> OnCutsceneToggled;

	public void TriggerCutsceneToggled(bool showing)
	{
		Action<bool> onCutsceneToggled = this.OnCutsceneToggled;
		if (onCutsceneToggled == null)
		{
			return;
		}
		onCutsceneToggled(showing);
	}

	public event Action OnFinaleVoyageStarted;

	public void TriggerFinaleVoyageStarted()
	{
		Action onFinaleVoyageStarted = this.OnFinaleVoyageStarted;
		if (onFinaleVoyageStarted == null)
		{
			return;
		}
		onFinaleVoyageStarted();
	}

	public event Action OnFinaleCutsceneStarted;

	public void TriggerFinaleCutsceneStarted()
	{
		Action onFinaleCutsceneStarted = this.OnFinaleCutsceneStarted;
		if (onFinaleCutsceneStarted == null)
		{
			return;
		}
		onFinaleCutsceneStarted();
	}

	public event Action<NotificationType, string> OnNotificationTriggered;

	public void TriggerNotification(NotificationType notificationType, string notificationString)
	{
		Action<NotificationType, string> onNotificationTriggered = this.OnNotificationTriggered;
		if (onNotificationTriggered == null)
		{
			return;
		}
		onNotificationTriggered(notificationType, notificationString);
	}

	public event Action<bool> OnHarvestModeToggled;

	public void ToggleHarvestMode(bool showing)
	{
		Action<bool> onHarvestModeToggled = this.OnHarvestModeToggled;
		if (onHarvestModeToggled == null)
		{
			return;
		}
		onHarvestModeToggled(showing);
	}

	public event Action<bool> OnRadialMenuShowingToggled;

	public void ToggleRadialMenuShowing(bool showing)
	{
		Action<bool> onRadialMenuShowingToggled = this.OnRadialMenuShowingToggled;
		if (onRadialMenuShowingToggled == null)
		{
			return;
		}
		onRadialMenuShowingToggled(showing);
	}

	public event Action<bool> OnActivelyHarvestingChanged;

	public void ToggleActivelyHarvesting(bool harvesting)
	{
		Action<bool> onActivelyHarvestingChanged = this.OnActivelyHarvestingChanged;
		if (onActivelyHarvestingChanged == null)
		{
			return;
		}
		onActivelyHarvestingChanged(harvesting);
	}

	public event Action<Dock> OnPlayerDockedToggled;

	public void TogglePlayerDocked(Dock dock)
	{
		Action<Dock> onPlayerDockedToggled = this.OnPlayerDockedToggled;
		if (onPlayerDockedToggled == null)
		{
			return;
		}
		onPlayerDockedToggled(dock);
	}

	public event Action<TutorialStepEnum> OnTutorialStepCompleted;

	public void TriggerTutorialStepCompleted(TutorialStepEnum step)
	{
		Action<TutorialStepEnum> onTutorialStepCompleted = this.OnTutorialStepCompleted;
		if (onTutorialStepCompleted == null)
		{
			return;
		}
		onTutorialStepCompleted(step);
	}

	public event Action<AbilityData> OnPlayerAbilityRegistered;

	public void TriggerPlayerAbilityRegistered(AbilityData abilityData)
	{
		Action<AbilityData> onPlayerAbilityRegistered = this.OnPlayerAbilityRegistered;
		if (onPlayerAbilityRegistered == null)
		{
			return;
		}
		onPlayerAbilityRegistered(abilityData);
	}

	public event Action<AbilityData> OnPlayerAbilitiesChanged;

	public void TogglePlayerAbilitiesChanged(AbilityData abilityData)
	{
		Action<AbilityData> onPlayerAbilitiesChanged = this.OnPlayerAbilitiesChanged;
		if (onPlayerAbilitiesChanged == null)
		{
			return;
		}
		onPlayerAbilitiesChanged(abilityData);
	}

	public event Action<AbilityData> OnPlayerAbilitySelected;

	public void SelectPlayerAbility(AbilityData abilityData)
	{
		Action<AbilityData> onPlayerAbilitySelected = this.OnPlayerAbilitySelected;
		if (onPlayerAbilitySelected == null)
		{
			return;
		}
		onPlayerAbilitySelected(abilityData);
	}

	public event Action<AbilityData, bool> OnPlayerAbilityToggled;

	public void TogglePlayerAbility(AbilityData abilityData, bool active)
	{
		Action<AbilityData, bool> onPlayerAbilityToggled = this.OnPlayerAbilityToggled;
		if (onPlayerAbilityToggled == null)
		{
			return;
		}
		onPlayerAbilityToggled(abilityData, active);
	}

	public event Action<SpatialItemData, int> OnAbilityItemCycled;

	public void TriggerAbilityItemCycled(SpatialItemData spatialItemData, int totalNumItemTypes)
	{
		Action<SpatialItemData, int> onAbilityItemCycled = this.OnAbilityItemCycled;
		if (onAbilityItemCycled == null)
		{
			return;
		}
		onAbilityItemCycled(spatialItemData, totalNumItemTypes);
	}

	public event Action<SpatialItemData> OnItemInventoryChanged;

	public void TriggerItemInventoryChanged(SpatialItemData spatialItemData)
	{
		Action<SpatialItemData> onItemInventoryChanged = this.OnItemInventoryChanged;
		if (onItemInventoryChanged == null)
		{
			return;
		}
		onItemInventoryChanged(spatialItemData);
	}

	public event Action<GridObject> OnItemPickedUp;

	public void TriggerItemPickupEvent(GridObject gridObject)
	{
		Action<GridObject> onItemPickedUp = this.OnItemPickedUp;
		if (onItemPickedUp == null)
		{
			return;
		}
		onItemPickedUp(gridObject);
	}

	public event Action<GridObject, bool> OnItemPlaceComplete;

	public void TriggerItemPlaceCompleteEvent(GridObject gridObject, bool result)
	{
		Action<GridObject, bool> onItemPlaceComplete = this.OnItemPlaceComplete;
		if (onItemPlaceComplete == null)
		{
			return;
		}
		onItemPlaceComplete(gridObject, result);
	}

	public event Action<GridObject> OnItemQuickMoved;

	public void TriggerItemQuickMoved(GridObject gridObject)
	{
		Action<GridObject> onItemQuickMoved = this.OnItemQuickMoved;
		if (onItemQuickMoved == null)
		{
			return;
		}
		onItemQuickMoved(gridObject);
	}

	public event Action<SpatialItemInstance, bool> OnItemAdded;

	public void TriggerItemAddedEvent(SpatialItemInstance spatialItemInstance, bool belongsToPlayer)
	{
		Action<SpatialItemInstance, bool> onItemAdded = this.OnItemAdded;
		if (onItemAdded == null)
		{
			return;
		}
		onItemAdded(spatialItemInstance, belongsToPlayer);
	}

	public event Action<GridObject> OnItemHoveredChanged;

	public void TriggerItemHoveredChanged(GridObject gridObject)
	{
		Action<GridObject> onItemHoveredChanged = this.OnItemHoveredChanged;
		if (onItemHoveredChanged == null)
		{
			return;
		}
		onItemHoveredChanged(gridObject);
	}

	public event Action<GridObject> OnItemRotated;

	public void TriggerItemRotated(GridObject gridObject)
	{
		Action<GridObject> onItemRotated = this.OnItemRotated;
		if (onItemRotated == null)
		{
			return;
		}
		onItemRotated(gridObject);
	}

	public event Action<GridCell> OnFocusedGridCellChanged;

	public void TriggerFocusedGridCellChanged(GridCell gridCell)
	{
		Action<GridCell> onFocusedGridCellChanged = this.OnFocusedGridCellChanged;
		if (onFocusedGridCellChanged == null)
		{
			return;
		}
		onFocusedGridCellChanged(gridCell);
	}

	public event Action OnItemsRepaired;

	public void TriggerItemsRepaired()
	{
		Action onItemsRepaired = this.OnItemsRepaired;
		if (onItemsRepaired == null)
		{
			return;
		}
		onItemsRepaired();
	}

	public event Action<GridObject> OnItemRemovedFromCursor;

	public void TriggerItemRemovedFromCursor(GridObject gridObject)
	{
		Action<GridObject> onItemRemovedFromCursor = this.OnItemRemovedFromCursor;
		if (onItemRemovedFromCursor == null)
		{
			return;
		}
		onItemRemovedFromCursor(gridObject);
	}

	public event Action<SpatialItemData, bool> OnItemDestroyed;

	public void TriggerItemDestroyed(SpatialItemData spatialItemData, bool playerDestroyed)
	{
		Action<SpatialItemData, bool> onItemDestroyed = this.OnItemDestroyed;
		if (onItemDestroyed == null)
		{
			return;
		}
		onItemDestroyed(spatialItemData, playerDestroyed);
	}

	public event Action<SpatialItemData> OnItemPurchased;

	public void TriggerItemPurchased(SpatialItemData spatialItemData)
	{
		Action<SpatialItemData> onItemPurchased = this.OnItemPurchased;
		if (onItemPurchased == null)
		{
			return;
		}
		onItemPurchased(spatialItemData);
	}

	public event Action<SpatialItemData> OnItemSold;

	public void TriggerItemSold(SpatialItemData spatialItemData)
	{
		Action<SpatialItemData> onItemSold = this.OnItemSold;
		if (onItemSold == null)
		{
			return;
		}
		onItemSold(spatialItemData);
	}

	public event Action<int> OnDayChanged;

	public void TriggerDayChanged(int day)
	{
		Action<int> onDayChanged = this.OnDayChanged;
		if (onDayChanged == null)
		{
			return;
		}
		onDayChanged(day);
	}

	public event Action<string, QuestState> OnQuestStateChanged;

	public void TriggerQuestStateChanged(string questId, QuestState newState)
	{
		Action<string, QuestState> onQuestStateChanged = this.OnQuestStateChanged;
		if (onQuestStateChanged == null)
		{
			return;
		}
		onQuestStateChanged(questId, newState);
	}

	public event Action<QuestStepData> OnQuestStepCompleted;

	public void TriggerQuestStepCompleted(QuestStepData questStepCompleted)
	{
		Action<QuestStepData> onQuestStepCompleted = this.OnQuestStepCompleted;
		if (onQuestStepCompleted == null)
		{
			return;
		}
		onQuestStepCompleted(questStepCompleted);
	}

	public event Action<QuestData> OnQuestCompleted;

	public void TriggerQuestCompleted(QuestData questCompleted)
	{
		Action<QuestData> onQuestCompleted = this.OnQuestCompleted;
		if (onQuestCompleted == null)
		{
			return;
		}
		onQuestCompleted(questCompleted);
	}

	public event Action OnPlayerStatsChanged;

	public void TriggerPlayerStatsChanged()
	{
		Action onPlayerStatsChanged = this.OnPlayerStatsChanged;
		if (onPlayerStatsChanged == null)
		{
			return;
		}
		onPlayerStatsChanged();
	}

	public event Action<SpatialItemData> OnEquipmentDamageChanged;

	public void TriggerEquipmentDamageChanged(SpatialItemData spatialItemData)
	{
		Action<SpatialItemData> onEquipmentDamageChanged = this.OnEquipmentDamageChanged;
		if (onEquipmentDamageChanged == null)
		{
			return;
		}
		onEquipmentDamageChanged(spatialItemData);
	}

	public event Action<bool, bool> OnCanInstallHoveredItemChanged;

	public void TriggerCanInstallHoveredItemChanged(bool valid, bool freeOfDamage)
	{
		Action<bool, bool> onCanInstallHoveredItemChanged = this.OnCanInstallHoveredItemChanged;
		if (onCanInstallHoveredItemChanged == null)
		{
			return;
		}
		onCanInstallHoveredItemChanged(valid, freeOfDamage);
	}

	public event Action OnPlayerDamageChanged;

	public void TriggerOnPlayerDamageChanged()
	{
		Action onPlayerDamageChanged = this.OnPlayerDamageChanged;
		if (onPlayerDamageChanged == null)
		{
			return;
		}
		onPlayerDamageChanged();
	}

	public event Action<bool, string, TimePassageMode> OnTimeForcefullyPassingChanged;

	public void TriggerTimeForcefullyPassingChanged(bool isPassing, string reasonKey, TimePassageMode mode)
	{
		Action<bool, string, TimePassageMode> onTimeForcefullyPassingChanged = this.OnTimeForcefullyPassingChanged;
		if (onTimeForcefullyPassingChanged == null)
		{
			return;
		}
		onTimeForcefullyPassingChanged(isPassing, reasonKey, mode);
	}

	public event Action<bool> OnPauseChange;

	public void TriggerPauseChange(bool isPaused)
	{
		Action<bool> onPauseChange = this.OnPauseChange;
		if (onPauseChange == null)
		{
			return;
		}
		onPauseChange(isPaused);
	}

	public event Action<bool> OnTopUIToggleRequested;

	public void TriggerTopUIToggleRequest(bool show)
	{
		Action<bool> onTopUIToggleRequested = this.OnTopUIToggleRequested;
		if (onTopUIToggleRequested == null)
		{
			return;
		}
		onTopUIToggleRequested(show);
	}

	public event Action<bool> OnMaintenanceModeToggled;

	public void TriggerMaintenanceModeToggled(bool isActive)
	{
		Action<bool> onMaintenanceModeToggled = this.OnMaintenanceModeToggled;
		if (onMaintenanceModeToggled == null)
		{
			return;
		}
		onMaintenanceModeToggled(isActive);
	}

	public event Action OnQuestGridViewChange;

	public void TriggerQuestGridViewChange()
	{
		Action onQuestGridViewChange = this.OnQuestGridViewChange;
		if (onQuestGridViewChange == null)
		{
			return;
		}
		onQuestGridViewChange();
	}

	public event Action<bool> OnPopupWindowToggled;

	public void TriggerPopupWindowToggled(bool showing)
	{
		Action<bool> onPopupWindowToggled = this.OnPopupWindowToggled;
		if (onPopupWindowToggled == null)
		{
			return;
		}
		onPopupWindowToggled(showing);
	}

	public event Action<string> OnNodeVisited;

	public void TriggerNodeVisited(string nodeName)
	{
		Action<string> onNodeVisited = this.OnNodeVisited;
		if (onNodeVisited == null)
		{
			return;
		}
		onNodeVisited(nodeName);
	}

	public event Action<bool> OnThreatBanished;

	public void TriggerThreatBanished(bool countForAchievement)
	{
		Action<bool> onThreatBanished = this.OnThreatBanished;
		if (onThreatBanished == null)
		{
			return;
		}
		onThreatBanished(countForAchievement);
	}

	public event Action OnFishingSpotDepleted;

	public void TriggerFishingSpotDepleted()
	{
		Action onFishingSpotDepleted = this.OnFishingSpotDepleted;
		if (onFishingSpotDepleted == null)
		{
			return;
		}
		onFishingSpotDepleted();
	}

	public event Action OnFishCaught;

	public void TriggerFishCaught()
	{
		Action onFishCaught = this.OnFishCaught;
		if (onFishCaught == null)
		{
			return;
		}
		onFishCaught();
	}

	public event Action<decimal, decimal> OnPlayerFundsChanged;

	public void TriggerPlayerFundsChanged(decimal total, decimal change)
	{
		Action<decimal, decimal> onPlayerFundsChanged = this.OnPlayerFundsChanged;
		if (onPlayerFundsChanged == null)
		{
			return;
		}
		onPlayerFundsChanged(total, change);
	}

	public event Action OnHasUnseenItemsChanged;

	public void TriggerHasUnseenItemsChanged()
	{
		Action onHasUnseenItemsChanged = this.OnHasUnseenItemsChanged;
		if (onHasUnseenItemsChanged == null)
		{
			return;
		}
		onHasUnseenItemsChanged();
	}

	public event Action<ResearchableItemInstance> OnBookReadProgressed;

	public void TriggerBookReadProgressed(ResearchableItemInstance researchableItemInstance)
	{
		Action<ResearchableItemInstance> onBookReadProgressed = this.OnBookReadProgressed;
		if (onBookReadProgressed == null)
		{
			return;
		}
		onBookReadProgressed(researchableItemInstance);
	}

	public event Action<ResearchableItemInstance> OnBookReadCompleted;

	public void TriggerBookReadCompleted(ResearchableItemInstance researchableItemInstance)
	{
		Action<ResearchableItemInstance> onBookReadCompleted = this.OnBookReadCompleted;
		if (onBookReadCompleted == null)
		{
			return;
		}
		onBookReadCompleted(researchableItemInstance);
	}

	public event Action<UpgradeData> OnUpgradesChanged;

	public void TriggerUpgradesChanged(UpgradeData upgradeData)
	{
		Action<UpgradeData> onUpgradesChanged = this.OnUpgradesChanged;
		if (onUpgradesChanged == null)
		{
			return;
		}
		onUpgradesChanged(upgradeData);
	}

	public event Action OnBoatModelChanged;

	public void TriggerBoatModelChangedEvent()
	{
		Action onBoatModelChanged = this.OnBoatModelChanged;
		if (onBoatModelChanged == null)
		{
			return;
		}
		onBoatModelChanged();
	}

	public event Action OnShopRestockRequested;

	public void TriggerShopRestockRequested()
	{
		Action onShopRestockRequested = this.OnShopRestockRequested;
		if (onShopRestockRequested == null)
		{
			return;
		}
		onShopRestockRequested();
	}

	public event Action<SpatialItemData> OnResearchCompleted;

	public void TriggerResearchCompleted(SpatialItemData spatialItemData)
	{
		Action<SpatialItemData> onResearchCompleted = this.OnResearchCompleted;
		if (onResearchCompleted == null)
		{
			return;
		}
		onResearchCompleted(spatialItemData);
	}

	public event Action<SpatialItemInstance> OnItemSeen;

	public void TriggerItemSeen(SpatialItemInstance spatialItemInstance)
	{
		Action<SpatialItemInstance> onItemSeen = this.OnItemSeen;
		if (onItemSeen == null)
		{
			return;
		}
		onItemSeen(spatialItemInstance);
	}

	public event Action<FishItemInstance> OnTrophyFishCaught;

	public void TriggerTrophyFishCaught(FishItemInstance fishItemInstance)
	{
		Action<FishItemInstance> onTrophyFishCaught = this.OnTrophyFishCaught;
		if (onTrophyFishCaught == null)
		{
			return;
		}
		onTrophyFishCaught(fishItemInstance);
	}

	public event Action OnTeleportBegin;

	public void TriggerTeleportBegin()
	{
		Action onTeleportBegin = this.OnTeleportBegin;
		if (onTeleportBegin == null)
		{
			return;
		}
		onTeleportBegin();
	}

	public event Action OnTeleportComplete;

	public void TriggerTeleportComplete()
	{
		Action onTeleportComplete = this.OnTeleportComplete;
		if (onTeleportComplete == null)
		{
			return;
		}
		onTeleportComplete();
	}

	public event Action OnDialogueStarted;

	public void TriggerDialogueStarted()
	{
		Action onDialogueStarted = this.OnDialogueStarted;
		if (onDialogueStarted == null)
		{
			return;
		}
		onDialogueStarted();
	}

	public event Action OnDialogueCompleted;

	public void TriggerDialogueCompleted()
	{
		Action onDialogueCompleted = this.OnDialogueCompleted;
		if (onDialogueCompleted == null)
		{
			return;
		}
		onDialogueCompleted();
	}

	public event Action<BuildingTierId> OnBuildingConstructed;

	public void TriggerBuildingConstructed(BuildingTierId tierId)
	{
		Action<BuildingTierId> onBuildingConstructed = this.OnBuildingConstructed;
		if (onBuildingConstructed == null)
		{
			return;
		}
		onBuildingConstructed(tierId);
	}

	public event Action<string> OnAnimationStartRequested;

	public void TriggerAnimationStartRequested(string animationId)
	{
		Action<string> onAnimationStartRequested = this.OnAnimationStartRequested;
		if (onAnimationStartRequested == null)
		{
			return;
		}
		onAnimationStartRequested(animationId);
	}

	public event Action<string> OnAnimationCompleted;

	public void TriggerAnimationCompleted(string animationId)
	{
		Action<string> onAnimationCompleted = this.OnAnimationCompleted;
		if (onAnimationCompleted == null)
		{
			return;
		}
		onAnimationCompleted(animationId);
	}

	public event Action OnIcebreakerEquipChanged;

	public void TriggerIcebreakerEquipChanged()
	{
		Action onIcebreakerEquipChanged = this.OnIcebreakerEquipChanged;
		if (onIcebreakerEquipChanged == null)
		{
			return;
		}
		onIcebreakerEquipChanged();
	}

	public event Action OnPlayerHitByMonster;

	public void TriggerPlayerHitByMonster()
	{
		Action onPlayerHitByMonster = this.OnPlayerHitByMonster;
		if (onPlayerHitByMonster == null)
		{
			return;
		}
		onPlayerHitByMonster();
	}

	public event Action<UpgradeData> OnUpgradePreviewed;

	public void TriggerUpgradePreviewed(UpgradeData upgradeData)
	{
		Action<UpgradeData> onUpgradePreviewed = this.OnUpgradePreviewed;
		if (onUpgradePreviewed == null)
		{
			return;
		}
		onUpgradePreviewed(upgradeData);
	}

	public event Action OnDayNightChanged;

	public void TriggerDayNightChanged()
	{
		Action onDayNightChanged = this.OnDayNightChanged;
		if (onDayNightChanged == null)
		{
			return;
		}
		onDayNightChanged();
	}

	public event Action<bool> OnBanishMachineToggled;

	public void TriggerBanishMachineToggled(bool isActive)
	{
		Action<bool> onBanishMachineToggled = this.OnBanishMachineToggled;
		if (onBanishMachineToggled == null)
		{
			return;
		}
		onBanishMachineToggled(isActive);
	}

	public event Action OnPlayerInteractedWithPOI;

	public void TriggerPlayerInteractedWithPOI()
	{
		Action onPlayerInteractedWithPOI = this.OnPlayerInteractedWithPOI;
		if (onPlayerInteractedWithPOI == null)
		{
			return;
		}
		onPlayerInteractedWithPOI();
	}

	public event Action<int> OnWorldPhaseChanged;

	public void TriggerWorldPhaseChanged(int worldPhase)
	{
		Action<int> onWorldPhaseChanged = this.OnWorldPhaseChanged;
		if (onWorldPhaseChanged == null)
		{
			return;
		}
		onWorldPhaseChanged(worldPhase);
	}

	public event Action<int> OnTIRWorldPhaseChanged;

	public void TriggerTIRWorldPhaseChanged(int tirWorldPhase)
	{
		Action<int> onTIRWorldPhaseChanged = this.OnTIRWorldPhaseChanged;
		if (onTIRWorldPhaseChanged == null)
		{
			return;
		}
		onTIRWorldPhaseChanged(tirWorldPhase);
	}

	public event Action OnHarvestZoneBecomeActive;

	public void TriggerHarvestZoneBecomeActive()
	{
		Action onHarvestZoneBecomeActive = this.OnHarvestZoneBecomeActive;
		if (onHarvestZoneBecomeActive == null)
		{
			return;
		}
		onHarvestZoneBecomeActive();
	}

	public event Action<BaseDestination, bool> OnDestinationVisited;

	public void TriggerDestinationVisited(BaseDestination destination, bool visited)
	{
		Action<BaseDestination, bool> onDestinationVisited = this.OnDestinationVisited;
		if (onDestinationVisited == null)
		{
			return;
		}
		onDestinationVisited(destination, visited);
	}

	public event Action<SpeakerData, bool> OnSpeakerVisited;

	public void TriggerSpeakerVisited(SpeakerData speakerData, bool visited)
	{
		Action<SpeakerData, bool> onSpeakerVisited = this.OnSpeakerVisited;
		if (onSpeakerVisited == null)
		{
			return;
		}
		onSpeakerVisited(speakerData, visited);
	}

	public event Action OnMonsterAttachedToPlayer;

	public void TriggerMonsterAttachedToPlayer()
	{
		Action onMonsterAttachedToPlayer = this.OnMonsterAttachedToPlayer;
		if (onMonsterAttachedToPlayer == null)
		{
			return;
		}
		onMonsterAttachedToPlayer();
	}

	public event Action<float> OnDismissAndSuppressDSLittleMonsters;

	public void TriggerDismissAndSuppressDSLittleMonsters(float duration)
	{
		Action<float> onDismissAndSuppressDSLittleMonsters = this.OnDismissAndSuppressDSLittleMonsters;
		if (onDismissAndSuppressDSLittleMonsters == null)
		{
			return;
		}
		onDismissAndSuppressDSLittleMonsters(duration);
	}

	public event Action OnFoghornStyleChanged;

	public void TriggerFoghornStyleChanged()
	{
		Action onFoghornStyleChanged = this.OnFoghornStyleChanged;
		if (onFoghornStyleChanged == null)
		{
			return;
		}
		onFoghornStyleChanged();
	}

	public event Action<int, int> OnBoatColorsChanged;

	public void TriggerBoatColorsChanged(int area, int index)
	{
		Action<int, int> onBoatColorsChanged = this.OnBoatColorsChanged;
		if (onBoatColorsChanged == null)
		{
			return;
		}
		onBoatColorsChanged(area, index);
	}

	public event Action<bool> OnBoatBuntingChanged;

	public void TriggerBoatBuntingChanged(bool enabled)
	{
		Action<bool> onBoatBuntingChanged = this.OnBoatBuntingChanged;
		if (onBoatBuntingChanged == null)
		{
			return;
		}
		onBoatBuntingChanged(enabled);
	}

	public event Action<int> OnBoatFlagChanged;

	public void TriggerBoatFlagChanged(int index)
	{
		Action<int> onBoatFlagChanged = this.OnBoatFlagChanged;
		if (onBoatFlagChanged == null)
		{
			return;
		}
		onBoatFlagChanged(index);
	}

	public event Action OnTeleportAnchorRemoved;

	public void TriggerTeleportAnchorRemoved()
	{
		Action onTeleportAnchorRemoved = this.OnTeleportAnchorRemoved;
		if (onTeleportAnchorRemoved == null)
		{
			return;
		}
		onTeleportAnchorRemoved();
	}

	public event Action OnTeleportAnchorAdded;

	public void TriggerTeleportAnchorAdded()
	{
		Action onTeleportAnchorAdded = this.OnTeleportAnchorAdded;
		if (onTeleportAnchorAdded == null)
		{
			return;
		}
		onTeleportAnchorAdded();
	}

	public event Action<SpatialItemData> OnSpecialItemHandlerRequested;

	public void TriggerSpecialItemHandlerRequest(SpatialItemData itemData)
	{
		Action<SpatialItemData> onSpecialItemHandlerRequested = this.OnSpecialItemHandlerRequested;
		if (onSpecialItemHandlerRequested == null)
		{
			return;
		}
		onSpecialItemHandlerRequested(itemData);
	}

	public event Action<bool> OnDetailPopupShowChange;

	public void TriggerDetailPopupShowChange(bool isShowing)
	{
		Action<bool> onDetailPopupShowChange = this.OnDetailPopupShowChange;
		if (onDetailPopupShowChange == null)
		{
			return;
		}
		onDetailPopupShowChange(isShowing);
	}

	public event Action OnShowRigTentacles;

	public void TriggerShowRigTentacles()
	{
		Action onShowRigTentacles = this.OnShowRigTentacles;
		if (onShowRigTentacles == null)
		{
			return;
		}
		onShowRigTentacles();
	}

	public event Action OnPlayerEnteredOoze;

	public void TriggerPlayerEnteredOoze()
	{
		Action onPlayerEnteredOoze = this.OnPlayerEnteredOoze;
		if (onPlayerEnteredOoze == null)
		{
			return;
		}
		onPlayerEnteredOoze();
	}

	public static GameEvents Instance;
}
