using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class RecipeGridPanel : SerializedMonoBehaviour, IScreenSideSwitchResponder
{
	private void Start()
	{
	}

	private void OnEnable()
	{
		this.AddListeners();
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, this.screenSide);
		}
	}

	private void OnDisable()
	{
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.UnregisterSwitchResponder(this, this.screenSide);
		}
		this.itemUI.Close();
		this.buildingUI.Close();
		this.gridUI.ClearGridHints();
		this.result = QuestGridResult.INCOMPLETE;
		this.RemoveListeners();
		this.bottomButton.Interactable = false;
	}

	public void SwitchToSide()
	{
		this.gridUI.SelectFirstPlaceableCell();
	}

	public void ToggleSwitchIcon(bool show)
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(show);
		}
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (gridObject.ItemData.moveMode != MoveMode.INSTALL || GameManager.Instance.GridManager.LastSelectedCell.gridCellState != GridObjectState.IN_INVENTORY)
		{
			this.OnStateChanged();
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.OnStateChanged();
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.OnStateChanged();
	}

	private void OnTimeForcefullyPassingChanged(bool isPassing, string reason, TimePassageMode mode)
	{
		if (!isPassing)
		{
			this.OnStateChanged();
		}
	}

	protected virtual void OnStateChanged()
	{
		if (this.currentQuestGridConfig.completeConditions.Count > 0)
		{
			this.result = (this.currentQuestGridConfig.completeConditions.TrueForAll((CompletedGridCondition c) => c.Evaluate(this.currentGrid)) ? QuestGridResult.COMPLETE : QuestGridResult.INCOMPLETE);
		}
		if (this.result == QuestGridResult.COMPLETE && !GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully())
		{
			this.bottomButton.Interactable = true;
		}
		else
		{
			this.bottomButton.Interactable = false;
		}
		List<ItemCountCondition> list = this.currentQuestGridConfig.completeConditions.OfType<ItemCountCondition>().ToList<ItemCountCondition>();
		int totalItemCount = 0;
		int matchingItemCount = 0;
		list.ForEach(delegate(ItemCountCondition c)
		{
			totalItemCount += c.count;
		});
		list.ForEach(delegate(ItemCountCondition c)
		{
			matchingItemCount += Mathf.Min(c.CountItems(this.currentGrid), c.count);
		});
		totalItemCount = Math.Max(totalItemCount, 1);
		Action<float> onPercentageRecipeCompleteChanged = this.OnPercentageRecipeCompleteChanged;
		if (onPercentageRecipeCompleteChanged == null)
		{
			return;
		}
		onPercentageRecipeCompleteChanged((float)matchingItemCount / (float)totalItemCount);
	}

	public void Show(RecipeData recipeData, bool exitOnFulfilled = true)
	{
		this.currentRecipe = recipeData;
		this.result = QuestGridResult.INCOMPLETE;
		this.currentQuestGridConfig = this.currentRecipe.questGridConfig;
		if (this.currentRecipe.cost > 0m)
		{
			this.bottomButton.LocalizedString.StringReference = this.bottomButtomStringWithCost;
		}
		else
		{
			this.bottomButton.LocalizedString.StringReference = this.bottomButtomStringFree;
		}
		this.bottomButton.LocalizedString.RefreshString();
		this.OnPlayerFundsChanged(GameManager.Instance.SaveData.Funds, 0m);
		this.currentGrid = null;
		this.currentGrid = GameManager.Instance.SaveData.GetGridByKey(this.currentQuestGridConfig.gridKey);
		if (this.currentGrid == null)
		{
			this.currentGrid = this.CreateGridWithConfig(this.currentQuestGridConfig, true);
		}
		GameManager.Instance.Player.CanMoveInstalledItems = this.currentQuestGridConfig.allowEquipmentInstallation;
		this.gridUI.OverrideGridCellColor = this.currentQuestGridConfig.overrideGridCellColor;
		this.gridUI.GridCellColor = this.currentQuestGridConfig.gridCellColor;
		this.gridUI.SetLinkedGrid(this.currentGrid);
		this.container.SetActive(false);
		this.container.SetActive(true);
		this.gridBackgroundTransform.DOSizeDelta(this.gridSize, this.gridExpandDurationSec, false).SetDelay(this.gridExpandDelaySec).SetEase(this.gridExpandEase)
			.From(new Vector2(this.gridSize.x, 0f), true, false);
		if (this.currentRecipe is BuildingRecipeData)
		{
			this.buildingUI.Init(this.currentRecipe as BuildingRecipeData);
		}
		else if (this.currentRecipe != null)
		{
			this.itemUI.Init(this.currentRecipe);
		}
		if (this.currentQuestGridConfig.presetGridMode == PresetGridMode.SILHOUETTE || this.currentQuestGridConfig.presetGridMode == PresetGridMode.MYSTERY)
		{
			this.gridUI.ShowGridHints(this.currentQuestGridConfig.presetGrid.spatialItems, this.currentQuestGridConfig.presetGridMode);
		}
		this.OnStateChanged();
	}

	private SerializableGrid CreateGridWithConfig(QuestGridConfig questGridConfig, bool save)
	{
		SerializableGrid serializableGrid;
		if (questGridConfig.presetGridMode == PresetGridMode.CREATE)
		{
			serializableGrid = (SerializableGrid)CollectionUtil.DeepClone(questGridConfig.presetGrid);
			if (questGridConfig.createWithDurabilityValue)
			{
				serializableGrid.spatialItems.ForEach(delegate(SpatialItemInstance itemInstance)
				{
					ItemData itemData = itemInstance.GetItemData<ItemData>();
					if (itemData is DeployableItemData)
					{
						itemInstance.durability = (itemData as DeployableItemData).MaxDurabilityDays * questGridConfig.startingDurabilityProportion;
					}
				});
			}
		}
		else
		{
			serializableGrid = new SerializableGrid();
		}
		serializableGrid.Init(questGridConfig.gridConfiguration, false);
		if (save)
		{
			GameManager.Instance.SaveData.grids.Add(questGridConfig.gridKey, serializableGrid);
		}
		return serializableGrid;
	}

	private void OnPlayerFundsChanged(decimal total, decimal change)
	{
		string text = "$" + this.currentRecipe.cost.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		string text2;
		if (total >= this.currentRecipe.cost)
		{
			text2 = text;
		}
		else
		{
			text2 = string.Concat(new string[]
			{
				"<color=#",
				GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
				">",
				text,
				"</color>"
			});
		}
		this.bottomButton.LocalizedString.StringReference.Arguments = new string[] { text2 };
		this.bottomButton.LocalizedString.StringReference.RefreshString();
	}

	private void AddListeners()
	{
		if (!this.hasAddedListeners)
		{
			GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
			GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
			GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
			BasicButtonWrapper basicButtonWrapper = this.backToBlueprintsButton;
			basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnBackButtonClicked));
			BasicButtonWrapper basicButtonWrapper2 = this.bottomButton;
			basicButtonWrapper2.OnClick = (Action)Delegate.Combine(basicButtonWrapper2.OnClick, new Action(this.OnPurchaseButtonClicked));
			this.hasAddedListeners = true;
		}
	}

	private void RemoveListeners()
	{
		if (this.hasAddedListeners)
		{
			GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
			GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
			GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
			BasicButtonWrapper basicButtonWrapper = this.backToBlueprintsButton;
			basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnBackButtonClicked));
			BasicButtonWrapper basicButtonWrapper2 = this.bottomButton;
			basicButtonWrapper2.OnClick = (Action)Delegate.Remove(basicButtonWrapper2.OnClick, new Action(this.OnPurchaseButtonClicked));
			this.hasAddedListeners = false;
		}
	}

	private void OnBackButtonClicked()
	{
		Action<RecipeData> onRecipeComplete = this.OnRecipeComplete;
		if (onRecipeComplete == null)
		{
			return;
		}
		onRecipeComplete(null);
	}

	private void OnPurchaseButtonClicked()
	{
		if (!this.slidePanelParent.IsShowing)
		{
			return;
		}
		if (GameManager.Instance.SaveData.Funds >= this.currentRecipe.cost)
		{
			GameManager.Instance.AddFunds(-this.currentRecipe.cost);
			Action<RecipeData> onRecipeComplete = this.OnRecipeComplete;
			if (onRecipeComplete != null)
			{
				onRecipeComplete(this.currentRecipe);
			}
			this.gridUI.linkedGrid.Clear(true);
		}
	}

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	private ScreenSide screenSide;

	[SerializeField]
	private GameObject sideSwitchIcon;

	[SerializeField]
	private BasicButtonWrapper backToBlueprintsButton;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	protected GridUI gridUI;

	[SerializeField]
	private bool exitOnComplete;

	[SerializeField]
	private BasicButtonWrapper bottomButton;

	[SerializeField]
	private LocalizedString bottomButtomStringWithCost;

	[SerializeField]
	private LocalizedString bottomButtomStringFree;

	[SerializeField]
	private IConstructableObjectUI<IDisplayableRecipe> itemUI;

	[SerializeField]
	private IConstructableObjectUI<BuildingRecipeData> buildingUI;

	[SerializeField]
	private RectTransform gridBackgroundTransform;

	[Header("Animation Config")]
	[SerializeField]
	private Vector2 gridSize;

	[SerializeField]
	private float gridExpandDurationSec;

	[SerializeField]
	private float gridExpandDelaySec;

	[SerializeField]
	private Ease gridExpandEase;

	[SerializeField]
	private SlidePanel slidePanelParent;

	protected QuestGridResult result;

	protected QuestGridConfig currentQuestGridConfig;

	protected SerializableGrid currentGrid;

	[HideInInspector]
	public Action<RecipeData> OnRecipeComplete;

	[HideInInspector]
	public Action<float> OnPercentageRecipeCompleteChanged;

	private RecipeData currentRecipe;

	private bool hasAddedListeners;
}
