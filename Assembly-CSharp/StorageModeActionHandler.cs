using System;
using UnityEngine;

public class StorageModeActionHandler : BaseGridModeActionHandler
{
	public StorageModeActionHandler()
	{
		this.mode = GridMode.STORAGE;
		this.quickTransferAction = new DredgePlayerActionPress("prompt.to-storage", GameManager.Instance.Input.Controls.QuickMove);
		this.quickTransferAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.quickTransferAction;
		dredgePlayerActionPress.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressEnd, new Action(this.OnQuickTransferPressed));
		this.quickTransferAction.priority = 3;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
	}

	public override void Shutdown()
	{
		base.Shutdown();
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionPress dredgePlayerActionPress = this.quickTransferAction;
		dredgePlayerActionPress.OnPressEnd = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressEnd, new Action(this.OnQuickTransferPressed));
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
	}

	private void OnItemsRepaired()
	{
		this.quickTransferAction.Reset();
	}

	protected override void OnFocusedGridCellChanged(GridCell gc)
	{
		this.RefreshCanUseQuickMoveAction(GameManager.Instance.GridManager.GetCurrentlyFocusedObject());
	}

	protected override void OnItemPickedUp(GridObject gridObject)
	{
		this.RefreshCanUseQuickMoveAction(gridObject);
	}

	protected void RefreshCanUseQuickMoveAction(GridObject gridObject)
	{
		if (this.CanUseQuickMoveAction(gridObject))
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
	}

	protected override void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
		}
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		base.OnItemHoveredChanged(gridObject);
		GridObject currentlyFocusedObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		if (this.CanUseQuickMoveAction(currentlyFocusedObject))
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
	}

	private bool CanUseQuickMoveAction(GridObject gridObject)
	{
		bool flag = false;
		if (gridObject != null)
		{
			if (gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.JUST_PURCHASED || gridObject.state == GridObjectState.IN_QUEST_GRID)
			{
				if (gridObject.state == GridObjectState.IN_INVENTORY && gridObject.ItemData.moveMode == MoveMode.INSTALL && gridObject.ParentGrid != null)
				{
					flag = false;
				}
				else
				{
					this.quickTransferAction.SetPromptString("prompt.to-storage");
					flag = true;
				}
			}
			else if (gridObject.state == GridObjectState.IN_STORAGE)
			{
				this.quickTransferAction.SetPromptString("prompt.to-cargo");
				flag = true;
			}
			if (gridObject.ItemData.isUnderlayItem)
			{
				flag = false;
			}
			if (GameManager.Instance.GridManager.IsInRepairMode)
			{
				flag = false;
			}
		}
		return flag;
	}

	protected override void OnItemRemovedFromCursor(GridObject gridObject)
	{
		base.OnItemRemovedFromCursor(gridObject);
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.quickTransferAction }, ActionLayer.UI_WINDOW);
	}

	private void OnQuickTransferPressed()
	{
		GridObject currentlyFocusedObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		if (!currentlyFocusedObject)
		{
			Debug.LogWarning("[StorageModeActionHandler] OnQuickTransferPressed() found no focused object");
			return;
		}
		GridKey gridKey = GridKey.NONE;
		if (currentlyFocusedObject.state == GridObjectState.IN_INVENTORY || currentlyFocusedObject.state == GridObjectState.JUST_PURCHASED || currentlyFocusedObject.state == GridObjectState.IN_QUEST_GRID)
		{
			gridKey = GridKey.STORAGE;
		}
		else if (currentlyFocusedObject.state == GridObjectState.IN_STORAGE)
		{
			gridKey = GridKey.INVENTORY;
		}
		bool flag = false;
		if (gridKey == GridKey.INVENTORY)
		{
			Vector3Int vector3Int;
			flag = GameManager.Instance.SaveData.Inventory.FindPositionForObject(currentlyFocusedObject.SpatialItemInstance.GetItemData<SpatialItemData>(), out vector3Int, 0, false);
		}
		else if (gridKey == GridKey.STORAGE)
		{
			Vector3Int vector3Int2;
			flag = GameManager.Instance.SaveData.Storage.FindPositionForObject(currentlyFocusedObject.SpatialItemInstance.GetItemData<SpatialItemData>(), out vector3Int2, 0, false);
		}
		if (!flag || (gridKey == GridKey.INVENTORY && currentlyFocusedObject.ItemData.moveMode == MoveMode.INSTALL))
		{
			if (gridKey == GridKey.STORAGE)
			{
				currentlyFocusedObject.state = GridObjectState.IN_STORAGE;
			}
			else
			{
				currentlyFocusedObject.state = GridObjectState.IN_INVENTORY;
			}
			Action onPickUpPressedCallback = this.OnPickUpPressedCallback;
			if (onPickUpPressedCallback != null)
			{
				onPickUpPressedCallback();
			}
			if (gridKey == GridKey.INVENTORY && !this.IsCurrentlyShowingInventory())
			{
				GameManager.Instance.UI.PlayerTabbedPanel.ShowNewPanel(1, true);
			}
			else if (gridKey == GridKey.STORAGE && !this.IsCurrentlyShowingStorage())
			{
				GameManager.Instance.UI.PlayerTabbedPanel.ShowNewPanel(3, true);
			}
			if (!flag)
			{
				GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.quick-move-failed");
			}
			GameManager.Instance.UI.InventoryGrid.SelectMostSuitableCellForObjectPlacement(currentlyFocusedObject.SpatialItemInstance);
			return;
		}
		currentlyFocusedObject.SpatialItemInstance.SetIsOnDamagedCell(false);
		GameManager.Instance.GridManager.QuickTransferItemToGrid(currentlyFocusedObject, gridKey, false);
		GameEvents.Instance.TriggerItemQuickMoved(currentlyFocusedObject);
	}

	private bool IsCurrentlyShowingInventory()
	{
		return GameManager.Instance.UI.PlayerTabbedPanel.CurrentIndex == 1;
	}

	private bool IsCurrentlyShowingStorage()
	{
		return GameManager.Instance.UI.PlayerTabbedPanel.CurrentIndex == 3 || GameManager.Instance.GridManager.GetIsGridShowing(GameManager.Instance.UI.StorageGrid);
	}

	protected DredgePlayerActionPress quickTransferAction;
}
