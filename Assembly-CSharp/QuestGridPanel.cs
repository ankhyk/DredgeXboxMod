using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestGridPanel : MonoBehaviour
{
	public SlidePanel SlidePanel
	{
		get
		{
			return this.slidePanel;
		}
		set
		{
			this.slidePanel = value;
		}
	}

	private void Start()
	{
		this.manualExitAction = new DredgePlayerActionPress("prompt.exit", GameManager.Instance.Input.Controls.Back);
		this.manualExitAction.priority = 0;
		DredgePlayerActionPress dredgePlayerActionPress = this.manualExitAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnManualExitPressComplete));
	}

	private void OnManualExitPressComplete()
	{
		if (this.currentQuestGridConfig.questGridExitMode == QuestGridExitMode.RISK_ITEM_LOSS)
		{
			if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully())
			{
				this.HandleShowExitConfirmation();
				return;
			}
		}
		else if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully())
		{
			Action<int> exitEvent = this.ExitEvent;
			if (exitEvent != null)
			{
				exitEvent(0);
			}
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.manualExitAction };
			input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		}
	}

	private void HandleShowExitConfirmation()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.manualExitAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "popup.confirm-grid-exit";
		DialogButtonOptions dialogButtonOptions = default(DialogButtonOptions);
		dialogButtonOptions.buttonString = "prompt.cancel";
		dialogButtonOptions.id = 0;
		dialogButtonOptions.hideOnButtonPress = true;
		dialogButtonOptions.isBackOption = true;
		DialogButtonOptions dialogButtonOptions2 = new DialogButtonOptions
		{
			buttonString = "prompt.confirm",
			id = 1,
			hideOnButtonPress = true
		};
		dialogOptions.buttonOptions = new DialogButtonOptions[] { dialogButtonOptions, dialogButtonOptions2 };
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnConfirmationResult));
	}

	private void OnConfirmationResult(DialogButtonOptions options)
	{
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		if (options.id != 1)
		{
			this.gridUI.SelectFirstPlaceableCell();
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.manualExitAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		Action<int> exitEvent = this.ExitEvent;
		if (exitEvent == null)
		{
			return;
		}
		exitEvent(0);
	}

	private void OnEnable()
	{
		this.slidePanel.OnShowFinish.AddListener(new UnityAction(this.OnShowFinish));
		this.slidePanel.OnHideStart.AddListener(new UnityAction(this.OnHideStart));
		this.slidePanel.OnHideFinish.AddListener(new UnityAction(this.OnHideFinish));
	}

	private void OnDisable()
	{
		this.slidePanel.OnShowFinish.RemoveListener(new UnityAction(this.OnShowFinish));
		this.slidePanel.OnHideStart.RemoveListener(new UnityAction(this.OnHideStart));
		this.slidePanel.OnHideFinish.RemoveListener(new UnityAction(this.OnHideFinish));
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
		if (this.result == QuestGridResult.COMPLETE && this.exitOnComplete && !GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully())
		{
			Action<int> exitEvent = this.ExitEvent;
			if (exitEvent != null)
			{
				exitEvent(1);
			}
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.manualExitAction };
			input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		}
	}

	public void Show(QuestGridConfig questGridConfig, bool exitOnFulfilled = true)
	{
		this.result = QuestGridResult.INCOMPLETE;
		this.currentQuestGridConfig = questGridConfig;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
		this.nameTextField.StringReference = this.currentQuestGridConfig.titleString;
		this.nameTextField.enabled = true;
		if (questGridConfig.backgroundImage != null)
		{
			this.backgroundImage.enabled = true;
			this.backgroundImage.sprite = questGridConfig.backgroundImage;
			this.backgroundImage.SetNativeSize();
		}
		else
		{
			this.backgroundImage.enabled = false;
		}
		this.currentGrid = null;
		if (questGridConfig.isSaved)
		{
			this.currentGrid = GameManager.Instance.SaveData.GetGridByKey(questGridConfig.gridKey);
			if (questGridConfig.createItemsIfEmpty && this.currentGrid.spatialItems.Count == 0)
			{
				this.currentGrid = this.CreateGridWithConfig(questGridConfig, false);
				GameManager.Instance.SaveData.grids[questGridConfig.gridKey] = this.currentGrid;
			}
			if (this.currentGrid == null)
			{
				this.currentGrid = this.CreateGridWithConfig(questGridConfig, true);
			}
		}
		else
		{
			this.currentGrid = this.CreateGridWithConfig(questGridConfig, false);
		}
		float gridHeightOverride = this.baseHeight;
		if (questGridConfig.gridHeightOverride != 0f)
		{
			gridHeightOverride = questGridConfig.gridHeightOverride;
		}
		RectTransform rectTransform = this.slidePanel.transform as RectTransform;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, gridHeightOverride + (float)questGridConfig.gridConfiguration.rows * GameManager.Instance.GridManager.cellSize);
		if (this.helpDropdownTween != null)
		{
			DOTween.Kill(this.helpDropdownTween, false);
			this.helpDropdownTween = null;
		}
		Vector2 zero = Vector2.zero;
		Vector2 vector = new Vector2(0f, 60f);
		(this.helpContainer.transform as RectTransform).anchoredPosition = vector;
		this.helpDropdownTween = (this.helpContainer.transform as RectTransform).DOAnchorPos(zero, 0.75f, false);
		this.helpDropdownTween.SetEase(Ease.OutExpo);
		this.helpDropdownTween.SetDelay(1f);
		Tweener tweener = this.helpDropdownTween;
		tweener.onComplete = (TweenCallback)Delegate.Combine(tweener.onComplete, new TweenCallback(delegate
		{
			this.helpDropdownTween = null;
		}));
		switch (questGridConfig.questGridExitMode)
		{
		case QuestGridExitMode.REVISITABLE:
		{
			this.exitPromptLocalizedTextField.StringReference = this.exitPromptString;
			this.helpLocalizedTextField.StringReference = this.revisitableString;
			this.helpTextField.color = Color.white;
			this.exitPromptContainer.SetActive(true);
			RectTransform rectTransform2 = this.helpTextContainer.transform as RectTransform;
			rectTransform2.anchorMin = Vector2.zero;
			rectTransform2.anchorMax = Vector2.zero;
			rectTransform2.pivot = new Vector2(0f, 1f);
			rectTransform2.anchoredPosition = new Vector2(10f, 0f);
			break;
		}
		case QuestGridExitMode.RISK_ITEM_LOSS:
		case QuestGridExitMode.ITEM_DELIVER:
		{
			this.exitPromptLocalizedTextField.StringReference = this.exitPromptString;
			this.helpLocalizedTextField.StringReference = this.riskItemLossString;
			this.helpTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			this.exitPromptContainer.SetActive(true);
			RectTransform rectTransform3 = this.helpTextContainer.transform as RectTransform;
			rectTransform3.anchorMin = Vector2.zero;
			rectTransform3.anchorMax = Vector2.zero;
			rectTransform3.pivot = new Vector2(0f, 1f);
			rectTransform3.anchoredPosition = new Vector2(10f, 0f);
			break;
		}
		case QuestGridExitMode.CANNOT_EXIT_MANUALLY:
		{
			this.exitPromptLocalizedTextField.StringReference = this.exitPromptString;
			this.helpLocalizedTextField.StringReference = this.nonRevisitableString;
			this.helpTextField.color = Color.white;
			this.exitPromptContainer.SetActive(false);
			RectTransform rectTransform4 = this.helpTextContainer.transform as RectTransform;
			rectTransform4.anchorMin = new Vector2(0.5f, 0f);
			rectTransform4.anchorMax = new Vector2(0.5f, 0f);
			rectTransform4.pivot = new Vector2(0.5f, 1f);
			rectTransform4.anchoredPosition = Vector2.zero;
			break;
		}
		}
		if (!questGridConfig.exitPromptOverride.IsEmpty)
		{
			this.exitPromptLocalizedTextField.StringReference = questGridConfig.exitPromptOverride;
		}
		if (!questGridConfig.helpStringOverride.IsEmpty)
		{
			this.helpLocalizedTextField.StringReference = questGridConfig.helpStringOverride;
		}
		this.helpLocalizedTextField.RefreshString();
		this.exitPromptLocalizedTextField.RefreshString();
		GameManager.Instance.Player.CanMoveInstalledItems = this.currentQuestGridConfig.allowEquipmentInstallation;
		this.gridUI.OverrideGridCellColor = this.currentQuestGridConfig.overrideGridCellColor;
		this.gridUI.GridCellColor = this.currentQuestGridConfig.gridCellColor;
		this.gridUI.SetLinkedGrid(this.currentGrid);
		this.container.SetActive(false);
		this.container.SetActive(true);
		this.slidePanel.Toggle(true, true);
		if (questGridConfig.presetGridMode == PresetGridMode.SILHOUETTE || questGridConfig.presetGridMode == PresetGridMode.MYSTERY)
		{
			this.gridUI.ShowGridHints(questGridConfig.presetGrid.spatialItems, questGridConfig.presetGridMode);
		}
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
					if (itemData is DurableItemData)
					{
						itemInstance.durability = (itemData as DurableItemData).MaxDurabilityDays * questGridConfig.startingDurabilityProportion;
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

	public virtual void Hide()
	{
		this.gridUI.ClearGridHints();
		this.slidePanel.Toggle(false, false);
		this.result = QuestGridResult.INCOMPLETE;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
	}

	private void OnShowFinish()
	{
		if (this.currentQuestGridConfig.questGridExitMode != QuestGridExitMode.CANNOT_EXIT_MANUALLY)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.manualExitAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			this.exitControlPromptUI.Init(this.manualExitAction, ControlPromptUI.ControlPromptMode.CUSTOM);
		}
		if (this.currentQuestGridConfig.allowEquipmentInstallation)
		{
			GameManager.Instance.GridManager.AddGridActionHandler(GridMode.EQUIPMENT);
		}
		if (this.currentQuestGridConfig.allowStorageAccess)
		{
			GameManager.Instance.GridManager.AddGridActionHandler(GridMode.STORAGE);
		}
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
	}

	private void OnHideStart()
	{
		GameManager.Instance.Player.CanMoveInstalledItems = false;
		GameManager.Instance.GridManager.ClearActionHandlers();
	}

	private void OnHideFinish()
	{
		this.container.SetActive(false);
	}

	[SerializeField]
	public LocalizedString revisitableString;

	[SerializeField]
	public LocalizedString nonRevisitableString;

	[SerializeField]
	public LocalizedString riskItemLossString;

	[SerializeField]
	public LocalizedString exitPromptString;

	[SerializeField]
	private ControlPromptEntryUI exitControlPromptUI;

	[SerializeField]
	private GameObject helpContainer;

	[SerializeField]
	private GameObject helpTextContainer;

	[SerializeField]
	private GameObject exitPromptContainer;

	[SerializeField]
	private LocalizeStringEvent helpLocalizedTextField;

	[SerializeField]
	private TextMeshProUGUI helpTextField;

	[SerializeField]
	private LocalizeStringEvent exitPromptLocalizedTextField;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private SlidePanel slidePanel;

	[SerializeField]
	protected GridUI gridUI;

	[SerializeField]
	private LocalizeStringEvent nameTextField;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private float baseHeight;

	[SerializeField]
	private bool exitOnComplete;

	protected QuestGridResult result;

	private Tweener helpDropdownTween;

	protected QuestGridConfig currentQuestGridConfig;

	protected SerializableGrid currentGrid;

	private DredgePlayerActionPress manualExitAction;

	public Action<int> ExitEvent;
}
