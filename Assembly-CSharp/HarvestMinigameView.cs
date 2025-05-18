using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class HarvestMinigameView : SerializedMonoBehaviour, IScreenSideSwitchResponder, IScreenSideActivationResponder
{
	protected virtual void Awake()
	{
		this.minigames.ForEach(delegate(HarvestMinigame m)
		{
			m.OnProgressRemoved = (Action)Delegate.Combine(m.OnProgressRemoved, new Action(this.OnProgressRemoved));
		});
		this.startMinigameAction = new DredgePlayerActionPress("prompt.harvest", GameManager.Instance.Input.Controls.Reel);
		DredgePlayerActionPress dredgePlayerActionPress = this.startMinigameAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressStartComplete));
		this.minigameInteractAction = new DredgePlayerActionPress("prompt.minigame-interact", GameManager.Instance.Input.Controls.Reel);
		this.minigameInteractAction.Disable(false);
		DredgePlayerActionPress dredgePlayerActionPress2 = this.minigameInteractAction;
		dredgePlayerActionPress2.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressBegin, new Action(this.OnMinigameInteractPress));
		this.storageTrayGrid = new SerializableGrid();
		this.storageTrayGrid.Init(GameManager.Instance.GameConfigData.GetGridConfigForKey(GridKey.STORAGE_TRAY), false);
		this.storageTrayGridUI.SetLinkedGrid(this.storageTrayGrid);
		this.Hide();
	}

	private void OnDestroy()
	{
		this.minigames.ForEach(delegate(HarvestMinigame m)
		{
			m.OnProgressRemoved = (Action)Delegate.Remove(m.OnProgressRemoved, new Action(this.OnProgressRemoved));
		});
		DredgePlayerActionPress dredgePlayerActionPress = this.startMinigameAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressStartComplete));
		DredgePlayerActionPress dredgePlayerActionPress2 = this.minigameInteractAction;
		dredgePlayerActionPress2.OnPressBegin = (Action)Delegate.Remove(dredgePlayerActionPress2.OnPressBegin, new Action(this.OnMinigameInteractPress));
	}

	private void OnMinigameInteractPress()
	{
		if (this.activeMinigame != null)
		{
			this.activeMinigame.OnMinigameInteractPress();
		}
	}

	private void OnProgressRemoved()
	{
		if (this.isCurrentHarvestSpotInOoze)
		{
			GameManager.Instance.GridManager.TryAddDarkSplashToInventory();
		}
	}

	public void Show(HarvestPOI harvestPOI)
	{
		this.currentPOI = harvestPOI;
		this.RefreshHarvestTarget();
		this.container.SetActive(true);
		this.cannotStartTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		this.activeMinigame.PrepareGame(this.difficultyConfig);
		this.activeMinigame.ResetGame();
		if (this.transitionTween != null)
		{
			DOTween.Kill(this.transitionTween, false);
			this.transitionTween = null;
		}
		this.transitionTween = (this.container.transform as RectTransform).DOAnchorPosX(0f, 0.35f, false).SetEase(Ease.OutExpo);
		this.isCurrentHarvestSpotInOoze = GameManager.Instance.OozePatchManager && GameManager.Instance.OozePatchManager.SampleOozeAtPosition(harvestPOI.transform.position) > 0f;
		this.oozeOverlay.SetActive(this.isCurrentHarvestSpotInOoze);
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnPlayerStatsChanged += this.OnPlayerStatsChanged;
		HarvestPOI harvestPOI2 = this.currentPOI;
		harvestPOI2.OnStockUpdatedAction = (Action)Delegate.Combine(harvestPOI2.OnStockUpdatedAction, new Action(this.RefreshHarvestTarget));
		SerializableGrid serializableGrid = this.storageTrayGrid;
		serializableGrid.OnContentsUpdated = (Action)Delegate.Combine(serializableGrid.OnContentsUpdated, new Action(this.OnStorageTrayGridContentsUpdated));
	}

	public void Hide()
	{
		if (this.activeMinigame)
		{
			this.activeMinigame.ResetGame();
		}
		this.itemDataToHarvest = null;
		if (this.transitionTween != null)
		{
			DOTween.Kill(this.transitionTween, false);
			this.transitionTween = null;
		}
		RectTransform rectTransform = this.container.transform as RectTransform;
		this.transitionTween = rectTransform.DOAnchorPosX(-rectTransform.rect.width, 0.35f, false);
		this.transitionTween.SetEase(Ease.OutExpo);
		this.transitionTween.OnComplete(delegate
		{
			this.controlPromptEntryUIContainer.gameObject.SetActive(true);
			this.progressBar.gameObject.SetActive(false);
			this.progressBarIcon.gameObject.SetActive(false);
			this.container.SetActive(false);
		});
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnPlayerStatsChanged -= this.OnPlayerStatsChanged;
		SerializableGrid serializableGrid = this.storageTrayGrid;
		serializableGrid.OnContentsUpdated = (Action)Delegate.Remove(serializableGrid.OnContentsUpdated, new Action(this.OnStorageTrayGridContentsUpdated));
		if (this.currentPOI)
		{
			HarvestPOI harvestPOI = this.currentPOI;
			harvestPOI.OnStockUpdatedAction = (Action)Delegate.Remove(harvestPOI.OnStockUpdatedAction, new Action(this.RefreshHarvestTarget));
		}
		this.ResetStorageTray();
		this.ResetStorageTrayHelpText();
		if (this.tutorialPopup.IsShowing)
		{
			this.tutorialPopup.Hide();
		}
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.startMinigameAction, this.minigameInteractAction }, ActionLayer.UI_WINDOW);
	}

	private void OnPressStartComplete()
	{
		this.StartGame();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.startMinigameAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
	}

	public void PrepareGame()
	{
		if (this.activeMinigame)
		{
			this.activeMinigame.PrepareGame(this.difficultyConfig);
		}
	}

	private bool HasUndamagedEquipment()
	{
		bool flag = true;
		if (this.itemDataToHarvest && this.itemDataToHarvest.itemSubtype == ItemSubtype.FISH)
		{
			flag = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD).Any((SpatialItemInstance r) => !r.GetIsOnDamagedCell() && r.GetItemData<RodItemData>().harvestableTypes.Contains(this.itemDataToHarvest.harvestableType));
		}
		return flag;
	}

	public void StartGame()
	{
		GameEvents.Instance.ToggleActivelyHarvesting(true);
		if (GameManager.Instance.SettingsSaveData.tutorials == 1 && !GameManager.Instance.SaveData.GetBoolVariable(string.Format("{0}-{1}", this.hasPlayedMinigameTypePrefix, this.activeMinigameType), false))
		{
			this.tutorialPopup.SetData(this.tutorialDictionary[this.activeMinigameType]);
			this.tutorialPopup.Show(null);
		}
		this.progressBar.gameObject.SetActive(true);
		this.progressBarIcon.gameObject.SetActive(true);
		this.cannotStartText.gameObject.SetActive(false);
		this.controlPromptEntryUI.Init(this.minigameInteractAction, ControlPromptUI.ControlPromptMode.CUSTOM);
		this.controlPromptLocalizedTextField.StringReference = this.interactString;
		this.controlPromptShiny.Stop(true);
		bool flag = this.HasUndamagedEquipment();
		bool flag2 = this.currentPOI.Harvestable.GetHarvestType() != HarvestableType.DREDGE && ((flag && this.currentPOI.IsBaitPOI && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.BaitedTrophyNotchSpawnChance) || (GameManager.Instance.SaveData.RodFishCaught > 0 && GameManager.Instance.SaveData.FishUntilNextTrophyNotch <= 0 && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.TrophyNotchSpawnChance));
		if (flag2)
		{
			GameManager.Instance.SaveData.FishUntilNextTrophyNotch = GameManager.Instance.GameConfigData.FishToCatchBetweenTrophyNotches;
		}
		this.activeMinigame.StartGame(this.difficultyConfig, this.currentPOI, flag, flag2);
		if (flag)
		{
			this.minigameInteractAction.Enable();
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.minigameInteractAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		this.controlPromptEntryUIContainer.SetActive(false);
		this.cannotStartText.gameObject.SetActive(true);
	}

	protected virtual void Update()
	{
		if (this.activeMinigame && this.activeMinigame.IsGameRunning)
		{
			this.progressBar.fillAmount = 1f - this.activeMinigame.Progress;
			this.progressBarIcon.rectTransform.anchoredPosition = new Vector2(this.progressBarIcon.rectTransform.anchoredPosition.x, Mathf.Lerp(this.progressBarIconMaxY, this.progressBarIconMinY, this.activeMinigame.Progress));
			if (this.activeMinigame.Progress >= 1f)
			{
				this.OnProgressComplete();
			}
		}
	}

	private void OnProgressComplete()
	{
		this.activeMinigame.StopGame();
		if (this.tutorialPopup.IsShowing)
		{
			GameManager.Instance.SaveData.SetBoolVariable(string.Format("{0}-{1}", this.hasPlayedMinigameTypePrefix, this.activeMinigameType), true);
			this.tutorialPopup.Hide();
		}
		this.RefreshHarvestTarget();
		if (!this.isStorageTrayShowing && GameManager.Instance.QuestManager.IsQuestCompleted(this.storageTrayUnlockQuest.name))
		{
			this.ShowStorageTray();
		}
		this.progressBarIcon.gameObject.SetActive(false);
		this.minigameInteractAction.Disable(true);
		this.controlPromptEntryUIContainer.gameObject.SetActive(false);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.minigameInteractAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		GameEvents.Instance.ToggleActivelyHarvesting(false);
		GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(ScreenSide.NONE);
		this.SpawnItem(this.activeMinigame.DidHitSpecialTarget);
	}

	private void SpawnItem(bool isTrophy)
	{
		bool flag = true;
		SpatialItemInstance spatialItemInstance = null;
		if (this.itemDataToHarvest != null)
		{
			if (this.itemDataToHarvest.itemSubtype == ItemSubtype.FISH)
			{
				FishAberrationGenerationMode fishAberrationGenerationMode = FishAberrationGenerationMode.RANDOM_CHANCE;
				if (this.currentPOI.IsCurrentlySpecial && this.currentPOI.Stock < 2f)
				{
					fishAberrationGenerationMode = FishAberrationGenerationMode.FORCE;
				}
				spatialItemInstance = GameManager.Instance.ItemManager.CreateFishItem(this.itemDataToHarvest.id, fishAberrationGenerationMode, this.currentPOI.IsCurrentlySpecial, isTrophy ? FishSizeGenerationMode.FORCE_BIG_TROPHY : FishSizeGenerationMode.NO_BIG_TROPHY, 1f);
				flag = !this.itemDataToHarvest.affectedByFishingSustain || global::UnityEngine.Random.value > GameManager.Instance.PlayerStats.ResearchedFishingSustainModifier;
				if (spatialItemInstance.GetItemData<FishItemData>().IsAberration)
				{
					this.currentPOI.SetIsCurrentlySpecial(false);
				}
				SaveData saveData = GameManager.Instance.SaveData;
				int num = saveData.FishUntilNextTrophyNotch;
				saveData.FishUntilNextTrophyNotch = num - 1;
				SaveData saveData2 = GameManager.Instance.SaveData;
				num = saveData2.RodFishCaught;
				saveData2.RodFishCaught = num + 1;
			}
			else if (this.itemDataToHarvest.canBeReplacedWithResearchItem && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.ResearchItemDredgeSpotSpawnChance)
			{
				spatialItemInstance = new SpatialItemInstance
				{
					id = GameManager.Instance.ResearchHelper.ResearchItemData.id
				};
				flag = false;
			}
			else
			{
				spatialItemInstance = new SpatialItemInstance
				{
					id = this.itemDataToHarvest.id
				};
			}
		}
		this.currentPOI.OnHarvested(flag);
		GameManager.Instance.GridManager.AddItemOfTypeToCursor(spatialItemInstance, GridObjectState.BEING_HARVESTED);
		GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance);
		GameEvents.Instance.TriggerFishCaught();
		this.itemDataToHarvest = null;
	}

	private void OnItemPickedUp(GridObject o)
	{
		this.startMinigameAction.Disable(true);
		this.controlPromptShiny.Stop(true);
	}

	private void OnItemRemovedFromCursor(GridObject o)
	{
		this.RefreshHarvestTarget();
	}

	private void OnItemPlaceComplete(GridObject o, bool result)
	{
		if (result)
		{
			this.RefreshHarvestTarget();
		}
	}

	private void OnPlayerStatsChanged()
	{
		this.RefreshHarvestTarget();
	}

	private void RefreshHarvestTypeInfo()
	{
		int harvestableItemSubType = (int)this.currentPOI.Harvestable.GetHarvestableItemSubType();
		bool isAdvancedHarvestType = this.currentPOI.Harvestable.GetIsAdvancedHarvestType();
		HarvestableType harvestableType = HarvestableType.NONE;
		if (this.itemDataToHarvest != null)
		{
			harvestableType = this.itemDataToHarvest.harvestableType;
		}
		if (harvestableType != HarvestableType.NONE)
		{
			this.typeTagUI.Init(harvestableType, isAdvancedHarvestType);
		}
		if (harvestableItemSubType == 1)
		{
			this.titleText.StringReference.SetReference(LanguageManager.STRING_TABLE, "spot.fishing");
			this.controlPromptLocalizedTextField.StringReference = this.startFishingString;
			return;
		}
		this.titleText.StringReference.SetReference(LanguageManager.STRING_TABLE, "spot.dredging");
		this.controlPromptLocalizedTextField.StringReference = this.startDredgingString;
	}

	private void RefreshHarvestTarget()
	{
		if (this.activeMinigame && this.activeMinigame.IsGameRunning)
		{
			return;
		}
		int num = Mathf.FloorToInt(this.currentPOI.Stock);
		Color stockColor;
		string text;
		if (num >= 5)
		{
			text = "stock.high";
			stockColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
		}
		else if (num >= 3)
		{
			text = "stock.medium";
			stockColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		}
		else if (num > 0)
		{
			text = "stock.low";
			stockColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.WARNING);
		}
		else
		{
			text = "stock.none";
			stockColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
		AsyncOperationHandle<string> localizedStringAsync = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, text, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		if (localizedStringAsync.IsDone)
		{
			this.FormatStockText(localizedStringAsync.Result, stockColor);
		}
		else
		{
			localizedStringAsync.Completed += delegate(AsyncOperationHandle<string> o)
			{
				this.FormatStockText(o.Result, stockColor);
			};
		}
		bool flag = true;
		bool flag2 = false;
		HarvestableType harvestType = this.currentPOI.Harvestable.GetHarvestType();
		bool isAdvancedHarvestType = this.currentPOI.Harvestable.GetIsAdvancedHarvestType();
		if (GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(harvestType, isAdvancedHarvestType))
		{
			flag2 = true;
		}
		this.invalidEquipmentIndicator.gameObject.SetActive(!flag2);
		if (flag2)
		{
			this.typeTagShiny.Stop(true);
		}
		else
		{
			this.typeTagShiny.Play(true);
		}
		HarvestQueryEnum isHarvestable = this.currentPOI.IsHarvestable;
		if (flag && !flag2)
		{
			flag = false;
		}
		if (flag && isHarvestable != HarvestQueryEnum.VALID)
		{
			flag = false;
		}
		if (flag && GameManager.Instance.GridManager.CurrentlyHeldObject)
		{
			flag = false;
		}
		if (this.itemDataToHarvest == null)
		{
			this.itemDataToHarvest = this.currentPOI.Harvestable.GetNextHarvestableItem();
		}
		bool flag3 = this.HasUndamagedEquipment();
		if (isHarvestable == HarvestQueryEnum.INVALID_NO_STOCK)
		{
			this.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.depleted");
		}
		else if (isHarvestable == HarvestQueryEnum.INVALID_INCORRECT_TIME)
		{
			this.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.no-fish-available");
		}
		else if (!flag2)
		{
			this.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.insufficient-equipment.fish");
			this.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.insufficient-equipment.dredge");
		}
		else if (!flag3)
		{
			this.cannotStartText.StringReference.SetReference(LanguageManager.STRING_TABLE, "poi-label.equipment-damaged");
		}
		if (flag)
		{
			this.startMinigameAction.Enable();
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.startMinigameAction }, ActionLayer.UI_WINDOW);
		}
		else
		{
			this.progressBar.gameObject.SetActive(false);
			this.progressBarIcon.gameObject.SetActive(false);
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.startMinigameAction }, ActionLayer.UI_WINDOW);
		}
		this.brokenEquipmentOverlay.SetActive(flag2 && !flag3);
		this.controlPromptEntryUI.Init(this.startMinigameAction, ControlPromptUI.ControlPromptMode.CUSTOM);
		this.controlPromptEntryUIContainer.gameObject.SetActive(flag);
		this.hintImage.gameObject.SetActive(flag);
		this.cannotStartText.gameObject.SetActive(!flag);
		if (flag && GameManager.Instance.GridManager.CurrentlyHeldObject == null)
		{
			this.controlPromptShiny.Play(true);
		}
		else
		{
			this.controlPromptShiny.Stop(true);
		}
		if (this.itemDataToHarvest)
		{
			if (this.itemDataToHarvest.harvestableType == HarvestableType.DREDGE)
			{
				this.difficultyConfig = GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[this.itemDataToHarvest.harvestDifficulty];
				this.progressBarIcon.sprite = this.dredgeSprite;
			}
			else
			{
				this.difficultyConfig = GameManager.Instance.GameConfigData.FishingDifficultyConfigs[this.itemDataToHarvest.harvestDifficulty];
				this.progressBarIcon.sprite = this.fishSprite;
			}
			if (this.itemDataToHarvest.itemSubtype == ItemSubtype.TRINKET)
			{
				this.hintImage.sprite = this.hiddenSilhouetteSprite;
				this.hintImage.transform.localScale = Vector3.one;
			}
			else
			{
				this.hintImage.sprite = this.itemDataToHarvest.sprite;
				if (this.itemDataToHarvest.GetSize() == 1)
				{
					this.hintImage.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
				}
				else
				{
					this.hintImage.transform.localScale = Vector3.one;
				}
			}
			if (this.itemDataToHarvest.harvestMinigameType == HarvestMinigameType.FISHING_SPIRAL)
			{
				(this.hintImage.transform as RectTransform).offsetMin = new Vector2(30f, 20f);
				(this.hintImage.transform as RectTransform).offsetMax = new Vector2(-20f, 0f);
			}
			else
			{
				(this.hintImage.transform as RectTransform).offsetMin = new Vector2(10f, 10f);
				(this.hintImage.transform as RectTransform).offsetMax = new Vector2(-10f, -10f);
			}
			this.minigames.ForEach(delegate(HarvestMinigame m)
			{
				m.gameObject.SetActive(m.harvestMinigameType == this.itemDataToHarvest.harvestMinigameType);
			});
			this.activeMinigameType = this.itemDataToHarvest.harvestMinigameType;
			this.activeMinigame = this.minigames.First((HarvestMinigame m) => m.harvestMinigameType == this.activeMinigameType);
			this.activeMinigame.PrepareGame(this.difficultyConfig);
		}
		this.RefreshHarvestTypeInfo();
	}

	private void FormatStockText(string text, Color color)
	{
		int num = text.IndexOf(":");
		if (num != -1)
		{
			string text2 = ColorUtility.ToHtmlStringRGB(color);
			text = text.Insert(num + 1, "<color=#" + text2 + ">");
		}
		this.stockText.text = text;
	}

	private void ShowStorageTray()
	{
		this.storageTrayContainer.SetActive(true);
		GameManager.Instance.ScreenSideSwitcher.RegisterActivationResponder(this, ScreenSide.LEFT);
		GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, ScreenSide.LEFT);
		this.isStorageTrayShowing = true;
		Vector2 zero = Vector2.zero;
		Vector2 vector = new Vector2(0f, 245f);
		(this.storageTrayContainer.transform as RectTransform).anchoredPosition = vector;
		this.storageTrayTween = (this.storageTrayContainer.transform as RectTransform).DOAnchorPos(zero, 0.75f, false);
		this.storageTrayTween.SetEase(Ease.OutExpo);
		Tweener tweener = this.storageTrayTween;
		tweener.onComplete = (TweenCallback)Delegate.Combine(tweener.onComplete, new TweenCallback(delegate
		{
			this.storageTrayTween = null;
		}));
	}

	private void OnStorageTrayGridContentsUpdated()
	{
		if (this.isStorageTrayHelpTextShowing && this.storageTrayGrid.spatialItems.Count == 0)
		{
			this.ToggleStorageTrayHelpText(false);
			return;
		}
		if (!this.isStorageTrayHelpTextShowing && this.storageTrayGrid.spatialItems.Count > 0)
		{
			this.ToggleStorageTrayHelpText(true);
		}
	}

	private void ToggleStorageTrayHelpText(bool show)
	{
		this.isStorageTrayHelpTextShowing = show;
		RectTransform rectTransform = this.storageTrayHelpTextContainer.transform as RectTransform;
		if (show)
		{
			Vector2 vector = new Vector2(0f, 3f);
			rectTransform.anchoredPosition = new Vector2(0f, rectTransform.rect.height);
			this.storageTrayHelpTextTween = rectTransform.DOAnchorPos(vector, 0.75f, false);
		}
		else
		{
			Vector2 vector2 = new Vector2(0f, rectTransform.rect.height);
			this.storageTrayHelpTextTween = rectTransform.DOAnchorPos(vector2, 0.75f, false);
		}
		this.storageTrayHelpTextTween.SetEase(Ease.OutExpo);
		Tweener tweener = this.storageTrayHelpTextTween;
		tweener.onComplete = (TweenCallback)Delegate.Combine(tweener.onComplete, new TweenCallback(delegate
		{
			this.storageTrayTween = null;
		}));
	}

	private void ResetStorageTrayHelpText()
	{
		GameManager.Instance.ScreenSideSwitcher.UnregisterActivationResponder(this, ScreenSide.LEFT);
		GameManager.Instance.ScreenSideSwitcher.UnregisterSwitchResponder(this, ScreenSide.LEFT);
		this.storageTrayHelpTextTween.Kill(false);
		(this.storageTrayHelpTextContainer.transform as RectTransform).anchoredPosition = new Vector2(0f, (this.storageTrayHelpTextContainer.transform as RectTransform).rect.height);
		this.isStorageTrayHelpTextShowing = false;
	}

	private void ResetStorageTray()
	{
		this.storageTrayTween.Kill(false);
		(this.storageTrayContainer.transform as RectTransform).anchoredPosition = new Vector2(0f, (this.storageTrayContainer.transform as RectTransform).rect.height);
		this.isStorageTrayShowing = false;
		this.storageTrayGrid.spatialItems.ForEach(delegate(SpatialItemInstance spatialItemInstance)
		{
			SpatialItemData itemData = spatialItemInstance.GetItemData<SpatialItemData>();
			if (itemData.itemSubtype == ItemSubtype.FISH)
			{
				SaveData saveData = GameManager.Instance.SaveData;
				int fishDiscardCount = saveData.FishDiscardCount;
				saveData.FishDiscardCount = fishDiscardCount + 1;
			}
			GameEvents.Instance.TriggerItemDestroyed(itemData, false);
		});
		this.storageTrayGrid.Clear(true);
		this.storageTrayContainer.SetActive(false);
	}

	public void SwitchToSide()
	{
		this.storageTrayGridUI.SelectFirstPlaceableCell();
	}

	public void ToggleSwitchIcon(bool show)
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(show);
		}
	}

	public void OnSideBecomeActive()
	{
	}

	public void OnSideBecomeInactive()
	{
	}

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[Header("References")]
	[SerializeField]
	private QuestData storageTrayUnlockQuest;

	[SerializeField]
	private GameObject sideSwitchIcon;

	[SerializeField]
	private GameObject storageTrayContainer;

	[SerializeField]
	private GameObject storageTrayHelpTextContainer;

	[SerializeField]
	private GridUI storageTrayGridUI;

	[SerializeField]
	private List<HarvestMinigame> minigames;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private LocalizeStringEvent titleText;

	[SerializeField]
	private TextMeshProUGUI stockText;

	[SerializeField]
	private LocalizeStringEvent cannotStartText;

	[SerializeField]
	private TextMeshProUGUI cannotStartTextField;

	[SerializeField]
	private GameObject controlPromptEntryUIContainer;

	[SerializeField]
	private ControlPromptEntryUI controlPromptEntryUI;

	[SerializeField]
	private LocalizeStringEvent controlPromptLocalizedTextField;

	[SerializeField]
	public LocalizedString startFishingString;

	[SerializeField]
	public LocalizedString startDredgingString;

	[SerializeField]
	public LocalizedString interactString;

	[SerializeField]
	private UIShiny controlPromptShiny;

	[SerializeField]
	private UIShiny typeTagShiny;

	[SerializeField]
	private HarvestableTypeTagUI typeTagUI;

	[SerializeField]
	private Image invalidEquipmentIndicator;

	[SerializeField]
	private Image progressBar;

	[SerializeField]
	private Image progressBarIcon;

	[SerializeField]
	private Sprite fishSprite;

	[SerializeField]
	private Sprite dredgeSprite;

	[SerializeField]
	private Sprite hiddenSilhouetteSprite;

	[SerializeField]
	private Image hintImage;

	[SerializeField]
	private GameObject brokenEquipmentOverlay;

	[SerializeField]
	private GameObject oozeOverlay;

	[Header("Configuration")]
	[SerializeField]
	private float progressBarIconMinY;

	[SerializeField]
	private float progressBarIconMaxY;

	[SerializeField]
	private Dictionary<HarvestMinigameType, TutorialStepData> tutorialDictionary;

	[SerializeField]
	private TutorialPopup tutorialPopup;

	private HarvestMinigameType activeMinigameType;

	private HarvestMinigame activeMinigame;

	private DredgePlayerActionPress minigameInteractAction;

	private DredgePlayerActionPress startMinigameAction;

	private HarvestableItemData itemDataToHarvest;

	protected HarvestDifficultyConfigData difficultyConfig;

	private HarvestPOI currentPOI;

	private Tweener transitionTween;

	private string hasPlayedMinigameTypePrefix = "played-minigame-type";

	private bool isCurrentHarvestSpotInOoze;

	private Tweener storageTrayTween;

	private Tweener storageTrayHelpTextTween;

	private SerializableGrid storageTrayGrid;

	private bool isStorageTrayShowing;

	private bool isStorageTrayHelpTextShowing;
}
