using System;

public class DefaultActionHandler : BaseGridModeActionHandler
{
	public DefaultActionHandler()
	{
		this.mode = GridMode.DEFAULT;
		this.pickUpAction = new DredgePlayerActionPress("prompt.pickup", GameManager.Instance.Input.Controls.PickUpPlace);
		this.pickUpAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.pickUpAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPickUpPressed));
		this.pickUpAction.priority = 1;
		this.placeAction = new DredgePlayerActionPress("prompt.place", GameManager.Instance.Input.Controls.PickUpPlace);
		this.placeAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.placeAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnPlacePressed));
		this.placeAction.priority = 1;
		this.rotateAction = new DredgePlayerActionAxis("prompt.rotate", GameManager.Instance.Input.Controls.RotateItem);
		this.rotateAction.showInTooltip = true;
		DredgePlayerActionAxis dredgePlayerActionAxis = this.rotateAction;
		dredgePlayerActionAxis.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionAxis.OnPressComplete, new Action(this.OnRotatePressed));
		this.rotateAction.priority = 2;
		this.discardAction = new DredgePlayerActionHold(this.defaultDiscardPrompt, GameManager.Instance.Input.Controls.DiscardItem, this.defaultDiscardHoldTimeSec);
		this.discardAction.showInTooltip = true;
		this.discardAction.usesHoldSFX = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.discardAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnDiscardPressed));
		this.discardAction.priority = 3;
		GameEvents.Instance.OnActivelyHarvestingChanged += this.OnHarvestMinigameToggled;
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

	protected virtual void OnRotatePressed()
	{
		Action<float> onRotatePressedCallback = this.OnRotatePressedCallback;
		if (onRotatePressedCallback == null)
		{
			return;
		}
		onRotatePressedCallback(this.rotateAction.Value);
	}

	private void OnDiscardPressed()
	{
		this.discardAction.Reset();
		GameManager.Instance.GridManager.DiscardCurrentObject();
		this.TryAddDiscardAction(GameManager.Instance.GridManager.GetCurrentlyFocusedObject());
	}

	protected override void OnItemPickedUp(GridObject gridObject)
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpAction }, ActionLayer.UI_WINDOW);
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.rotateAction }, ActionLayer.UI_WINDOW);
		this.TryAddPlaceAction(gridObject, gridObject.state);
		this.TryAddDiscardAction(gridObject);
	}

	protected override void OnFocusedGridCellChanged(GridCell gridCell)
	{
		GridObjectState gridObjectState = GridObjectState.NONE;
		if (gridCell)
		{
			gridObjectState = gridCell.gridCellState;
		}
		this.TryAddPlaceAction(GameManager.Instance.GridManager.CurrentlyHeldObject, gridObjectState);
		this.TryAddDiscardAction(GameManager.Instance.GridManager.GetCurrentlyFocusedObject());
	}

	private void TryAddPlaceAction(GridObject gridObject, GridObjectState destinationCellState)
	{
		bool flag = false;
		if (gridObject && (destinationCellState == GridObjectState.IN_STORAGE || (destinationCellState == GridObjectState.IN_QUEST_GRID && GameManager.Instance.GridManager.LastActiveGrid.GridConfiguration.CanAddItemsInQuestMode) || (destinationCellState == GridObjectState.IN_QUEST_GRID && gridObject.state == GridObjectState.IN_QUEST_GRID) || (destinationCellState == GridObjectState.BEING_HARVESTED && gridObject.state == GridObjectState.BEING_HARVESTED) || (destinationCellState == GridObjectState.IN_INVENTORY && gridObject.ItemData.moveMode == MoveMode.FREE) || (destinationCellState == GridObjectState.IN_TRAY && !gridObject.ItemData.ForbidStorageTray)))
		{
			flag = true;
		}
		if (flag)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.placeAction }, ActionLayer.UI_WINDOW);
	}

	private void TryAddDiscardAction(GridObject gridObject)
	{
		SpatialItemData spatialItemData = null;
		if (gridObject)
		{
			spatialItemData = gridObject.ItemData;
		}
		DredgePlayerActionBase[] array;
		if (gridObject && !GameManager.Instance.GridManager.IsInRepairMode && !this.isInHarvestMinigame && (!GameManager.Instance.UI.IsShowingQuestGrid || (GameManager.Instance.UI.IsShowingQuestGrid && spatialItemData.canBeDiscardedDuringQuestPickup)) && (gridObject.state == GridObjectState.BEING_HARVESTED || gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.IN_STORAGE || gridObject.state == GridObjectState.IN_QUEST_GRID || gridObject.state == GridObjectState.IN_TRAY) && (spatialItemData.canBeDiscardedByPlayer || gridObject.state == GridObjectState.BEING_HARVESTED) && !GameManager.Instance.GridManager.IsInRepairMode && (spatialItemData.itemType != ItemType.EQUIPMENT || GameManager.Instance.QuestManager.IsIntroQuestCompleted()))
		{
			this.discardAction.showAlertOnHold = spatialItemData.showAlertOnDiscardHold;
			this.discardAction.holdTimeRequiredSec = (spatialItemData.discardHoldTimeOverride ? spatialItemData.discardHoldTimeSec : this.defaultDiscardHoldTimeSec);
			if (spatialItemData.hasSpecialDiscardAction)
			{
				this.discardAction.SetPromptString(spatialItemData.discardPromptOverride);
			}
			else
			{
				this.discardAction.SetPromptString(this.defaultDiscardPrompt);
			}
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionHold[] { this.discardAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionHold[] { this.discardAction };
		input2.RemoveActionListener(array, ActionLayer.UI_WINDOW);
	}

	protected override void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		if (result)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpAction, this.placeAction, this.rotateAction }, ActionLayer.UI_WINDOW);
		}
	}

	protected override void OnItemRemovedFromCursor(GridObject gridObject)
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpAction, this.placeAction, this.rotateAction }, ActionLayer.UI_WINDOW);
		this.TryAddDiscardAction(null);
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			this.TryAddDiscardAction(gridObject);
		}
		bool flag = false;
		if (!GameManager.Instance.GridManager.IsInRepairMode && gridObject && gridObject.ItemData.moveMode == MoveMode.FREE && !this.isInHarvestMinigame && !GameManager.Instance.GridManager.CurrentlyHeldObject && (gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.IN_STORAGE || gridObject.state == GridObjectState.BEING_HARVESTED || gridObject.state == GridObjectState.IN_QUEST_GRID || gridObject.state == GridObjectState.IN_TRAY))
		{
			flag = true;
		}
		if (flag)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.pickUpAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.pickUpAction }, ActionLayer.UI_WINDOW);
	}

	private void OnHarvestMinigameToggled(bool enabled)
	{
		this.isInHarvestMinigame = enabled;
		if (this.isInHarvestMinigame)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.discardAction, this.pickUpAction }, ActionLayer.UI_WINDOW);
			return;
		}
		this.TryAddDiscardAction(GameManager.Instance.GridManager.GetCurrentlyFocusedObject());
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.pickUpAction }, ActionLayer.UI_WINDOW);
	}

	public override void Shutdown()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.discardAction, this.pickUpAction, this.placeAction, this.rotateAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionPress dredgePlayerActionPress = this.pickUpAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPickUpPressed));
		DredgePlayerActionPress dredgePlayerActionPress2 = this.placeAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnPlacePressed));
		DredgePlayerActionAxis dredgePlayerActionAxis = this.rotateAction;
		dredgePlayerActionAxis.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionAxis.OnPressComplete, new Action(this.OnRotatePressed));
		GameEvents.Instance.OnActivelyHarvestingChanged -= this.OnHarvestMinigameToggled;
		base.Shutdown();
	}

	protected DredgePlayerActionAxis rotateAction;

	protected DredgePlayerActionPress pickUpAction;

	protected DredgePlayerActionPress placeAction;

	protected DredgePlayerActionHold discardAction;

	private bool isInHarvestMinigame;

	private float defaultDiscardHoldTimeSec = 0.75f;

	private string defaultDiscardPrompt = "prompt.discard";

	private GridConfiguration focusedGridConfiguration;
}
