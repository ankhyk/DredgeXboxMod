using System;
using System.Collections;
using UnityEngine;

public class EquipmentModeActionHandler : BaseGridModeActionHandler
{
	public string _InstallTime { get; set; }

	public EquipmentModeActionHandler()
	{
		this.mode = GridMode.EQUIPMENT;
		this.pickUpActionHold = new DredgePlayerActionHold("prompt.uninstall", GameManager.Instance.Input.Controls.PickUpPlace, 0.6f);
		this.pickUpActionHold.showInTooltip = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.pickUpActionHold;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnPickUpPressed));
		this.pickUpActionHold.priority = 1;
		this.pickUpActionPress = new DredgePlayerActionPress("prompt.uninstall", GameManager.Instance.Input.Controls.PickUpPlace);
		this.pickUpActionPress.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.pickUpActionPress;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPickUpPressed));
		this.pickUpActionPress.priority = 1;
		this.placeAction = new DredgePlayerActionHold("prompt.install", GameManager.Instance.Input.Controls.PickUpPlace, 0.6f);
		this.placeAction.showInTooltip = true;
		DredgePlayerActionHold dredgePlayerActionHold2 = this.placeAction;
		dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold2.OnPressComplete, new Action(this.OnPlacePressed));
		this.placeAction.priority = 1;
	}

	protected virtual void OnPickUpPressed()
	{
		Action onPickUpPressedCallback = this.OnPickUpPressedCallback;
		if (onPickUpPressedCallback == null)
		{
			return;
		}
		onPickUpPressedCallback();
	}

	protected virtual void OnPlacePressed()
	{
		Action onPlacePressedCallback = this.OnPlacePressedCallback;
		if (onPlacePressedCallback == null)
		{
			return;
		}
		onPlacePressedCallback();
	}

	protected override void OnItemPickedUp(GridObject gridObject)
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpActionHold, this.pickUpActionPress }, ActionLayer.UI_WINDOW);
		if (gridObject.ItemData.itemType == ItemType.EQUIPMENT)
		{
			this.OnFocusedGridCellChanged(GameManager.Instance.GridManager.LastSelectedCell);
			GameEvents.Instance.OnFocusedGridCellChanged += this.OnFocusedGridCellChanged;
			GameEvents.Instance.OnItemRotated += this.OnItemRotated;
		}
	}

	private void FormatPlaceActionString(GridObject gridObject)
	{
		string text = GameManager.Instance.ItemManager.GetInstallTimeForItem(gridObject.ItemData).ToString(".#");
		this._InstallTime = " [" + text + "h]";
		this.placeAction.LocalizationArguments = new object[] { this._InstallTime };
		this.placeAction.TriggerPromptArgumentsChanged();
	}

	protected override void OnCanBePlacedChanged(GridObject gridObject, bool canBePlaced)
	{
		if (this.CanAddPlaceAction(gridObject))
		{
			this.FormatPlaceActionString(gridObject);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
	}

	private void OnItemRotated(GridObject gridObject)
	{
		this.OnCellsUnderItemChanged();
	}

	private bool CanAddPlaceAction(GridObject gridObject)
	{
		return gridObject && gridObject.ItemData.moveMode == MoveMode.INSTALL;
	}

	protected override void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		if (result)
		{
			GameEvents.Instance.OnFocusedGridCellChanged -= this.OnFocusedGridCellChanged;
			GameEvents.Instance.OnItemRotated -= this.OnItemRotated;
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpActionHold, this.pickUpActionPress, this.placeAction }, ActionLayer.UI_WINDOW);
			if (gridObject.ItemData.moveMode == MoveMode.INSTALL && this.currentlySelectedCellState == GridObjectState.IN_INVENTORY)
			{
				float installTimeForItem = GameManager.Instance.ItemManager.GetInstallTimeForItem(gridObject.ItemData);
				GameManager.Instance.Time.ForcefullyPassTime(installTimeForItem, "feedback.pass-time-install-equipment", TimePassageMode.INSTALL);
			}
		}
	}

	protected override void OnItemRemovedFromCursor(GridObject gridObject)
	{
		GameEvents.Instance.OnFocusedGridCellChanged -= this.OnFocusedGridCellChanged;
		GameEvents.Instance.OnItemRotated -= this.OnItemRotated;
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpActionHold, this.pickUpActionPress, this.placeAction }, ActionLayer.UI_WINDOW);
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		bool flag = false;
		if (gridObject && !GameManager.Instance.GridManager.CurrentlyHeldObject && (gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.IN_QUEST_GRID || gridObject.state == GridObjectState.IN_STORAGE) && gridObject.ItemData.moveMode == MoveMode.INSTALL)
		{
			flag = true;
		}
		if (flag)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.pickUpActionPress;
			if (gridObject.state == GridObjectState.IN_INVENTORY)
			{
				dredgePlayerActionBase = this.pickUpActionHold;
			}
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { dredgePlayerActionBase }, ActionLayer.UI_WINDOW);
			if (gridObject.state == GridObjectState.IN_STORAGE || gridObject.state == GridObjectState.IN_QUEST_GRID)
			{
				dredgePlayerActionBase.SetPromptString("prompt.pickup");
				return;
			}
			if (gridObject.state == GridObjectState.IN_INVENTORY)
			{
				dredgePlayerActionBase.SetPromptString("prompt.uninstall");
				return;
			}
		}
		else
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpActionHold, this.pickUpActionPress }, ActionLayer.UI_WINDOW);
		}
	}

	protected override void OnFocusedGridCellChanged(GridCell gridCell)
	{
		if (gridCell)
		{
			this.currentlySelectedCellState = gridCell.gridCellState;
		}
		else
		{
			this.currentlySelectedCellState = GridObjectState.NONE;
		}
		if (this.currentlySelectedCellState != GridObjectState.IN_INVENTORY)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
			return;
		}
		if (this.CanAddPlaceAction(GameManager.Instance.GridManager.CurrentlyHeldObject))
		{
			this.FormatPlaceActionString(GameManager.Instance.GridManager.CurrentlyHeldObject);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
			this.OnCellsUnderItemChanged();
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
	}

	private void OnCellsUnderItemChanged()
	{
		GridObject currentlyHeldObject = GameManager.Instance.GridManager.CurrentlyHeldObject;
		if (currentlyHeldObject.ItemData.itemType != ItemType.EQUIPMENT)
		{
			return;
		}
		if (currentlyHeldObject)
		{
			if (this.HitTestCoroutine != null)
			{
				GameManager.Instance.GridManager.StopCoroutine(this.HitTestCoroutine);
			}
			this.HitTestCoroutine = GameManager.Instance.GridManager.StartCoroutine(this.DoHitTest());
		}
	}

	private IEnumerator DoHitTest()
	{
		bool shouldEnableAction = false;
		bool placementCellsAreUndamaged = true;
		yield return new WaitForSeconds(0.1f);
		GridObject currentlyHeldObject = GameManager.Instance.GridManager.CurrentlyHeldObject;
		if (currentlyHeldObject)
		{
			GridObjectPlacementResult placementResult = currentlyHeldObject.GetPlacementResult();
			placementCellsAreUndamaged = placementResult.placementCellsAreUndamaged;
			if (placementResult.placementCellsAcceptObject && placementResult.placementCellsValid && (placementResult.placementUnobstructed || placementResult.objects.Count <= 1) && (placementResult.placementCellsAreUndamaged || currentlyHeldObject.ItemData.ignoreDamageWhenPlacing))
			{
				shouldEnableAction = true;
			}
		}
		GameEvents.Instance.TriggerCanInstallHoveredItemChanged(shouldEnableAction, placementCellsAreUndamaged);
		if (shouldEnableAction)
		{
			this.placeAction.Enable();
		}
		else
		{
			this.placeAction.Reset();
			this.placeAction.Disable(true);
		}
		this.HitTestCoroutine = null;
		yield break;
	}

	public override void Shutdown()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpActionHold, this.pickUpActionPress, this.placeAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionHold dredgePlayerActionHold = this.pickUpActionHold;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold.OnPressComplete, new Action(this.OnPickUpPressed));
		DredgePlayerActionPress dredgePlayerActionPress = this.pickUpActionPress;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPickUpPressed));
		DredgePlayerActionHold dredgePlayerActionHold2 = this.placeAction;
		dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold2.OnPressComplete, new Action(this.OnPlacePressed));
		GameEvents.Instance.OnFocusedGridCellChanged -= this.OnFocusedGridCellChanged;
		GameEvents.Instance.OnItemRotated -= this.OnItemRotated;
		base.Shutdown();
	}

	protected DredgePlayerActionHold pickUpActionHold;

	protected DredgePlayerActionPress pickUpActionPress;

	protected DredgePlayerActionHold placeAction;

	private Coroutine HitTestCoroutine;

	private GridObjectState currentlySelectedCellState;
}
