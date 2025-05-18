using System;
using UnityEngine;

public class ItemProductPanel : MonoBehaviour, IScreenSideSwitchResponder
{
	public void SetQuestGridConfig(QuestGridConfig questGridConfig)
	{
		this.questGridConfig = questGridConfig;
		if (questGridConfig != null)
		{
			this.currentGrid = GameManager.Instance.SaveData.GetGridByKey(questGridConfig.gridKey);
			return;
		}
		this.currentGrid = null;
	}

	public bool HasItemsToPickUp()
	{
		return this.currentGrid != null && this.currentGrid.spatialItems.Count > 0;
	}

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
		this.result = QuestGridResult.INCOMPLETE;
		this.RemoveListeners();
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.UnregisterSwitchResponder(this, this.screenSide);
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
		if (this.questGridConfig.completeConditions.Count > 0)
		{
			this.result = (this.questGridConfig.completeConditions.TrueForAll((CompletedGridCondition c) => c.Evaluate(this.currentGrid)) ? QuestGridResult.COMPLETE : QuestGridResult.INCOMPLETE);
		}
		if (this.result == QuestGridResult.COMPLETE && !GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.Time.IsTimePassingForcefully())
		{
			Action onGridComplete = this.OnGridComplete;
			if (onGridComplete == null)
			{
				return;
			}
			onGridComplete();
		}
	}

	public void Show()
	{
		this.result = QuestGridResult.INCOMPLETE;
		GameManager.Instance.Player.CanMoveInstalledItems = this.questGridConfig.allowEquipmentInstallation;
		this.gridUI.OverrideGridCellColor = this.questGridConfig.overrideGridCellColor;
		this.gridUI.GridCellColor = this.questGridConfig.gridCellColor;
		this.gridUI.SetLinkedGrid(this.currentGrid);
		this.container.SetActive(false);
		this.container.SetActive(true);
		if (this.questGridConfig.presetGridMode == PresetGridMode.SILHOUETTE || this.questGridConfig.presetGridMode == PresetGridMode.MYSTERY)
		{
			this.gridUI.ShowGridHints(this.questGridConfig.presetGrid.spatialItems, this.questGridConfig.presetGridMode);
		}
	}

	public void AddItemToGrid(SpatialItemData spatialItemData)
	{
		this.currentGrid.FindSpaceAndAddObjectToGridData(spatialItemData, false, null);
	}

	private void AddListeners()
	{
		if (!this.hasAddedListeners)
		{
			GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
			GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
			GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
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
			this.hasAddedListeners = false;
		}
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

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	private ScreenSide screenSide;

	[SerializeField]
	private GameObject sideSwitchIcon;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	protected GridUI gridUI;

	private QuestGridConfig questGridConfig;

	protected QuestGridResult result;

	protected SerializableGrid currentGrid;

	public Action OnGridComplete;

	private bool hasAddedListeners;
}
