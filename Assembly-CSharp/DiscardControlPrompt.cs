using System;
using InControl;
using UnityEngine;
using UnityEngine.UI;

public class DiscardControlPrompt : MonoBehaviour
{
	private void Awake()
	{
		PlayerAction discardItem = GameManager.Instance.Input.Controls.DiscardItem;
		this.discardDredgeAction = new DredgePlayerActionHold("prompt.discard", discardItem, 0.75f);
		this.discardDredgeAction.Disable(false);
		this.controlPromptIcon.Init(this.discardDredgeAction, this.discardDredgeAction.GetPrimaryPlayerAction());
	}

	private void OnEnable()
	{
		DredgePlayerActionHold dredgePlayerActionHold = this.discardDredgeAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnDiscardPressComplete));
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemHoveredChanged += this.OnItemHoveredChanged;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		this.Refresh();
	}

	private void OnDisable()
	{
		DredgePlayerActionHold dredgePlayerActionHold = this.discardDredgeAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold.OnPressComplete, new Action(this.OnDiscardPressComplete));
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemHoveredChanged -= this.OnItemHoveredChanged;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
	}

	private void OnDiscardPressComplete()
	{
		this.discardDredgeAction.Reset();
		GameManager.Instance.GridManager.DiscardCurrentObject();
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.Refresh();
	}

	private void OnItemHoveredChanged(GridObject gridObject)
	{
		this.Refresh();
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.Refresh();
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		this.Refresh();
	}

	private void Refresh()
	{
		GridObject currentlyFocusedObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		DredgePlayerActionBase[] array;
		if (currentlyFocusedObject && (currentlyFocusedObject.state == GridObjectState.BEING_HARVESTED || currentlyFocusedObject.state == GridObjectState.IN_INVENTORY || currentlyFocusedObject.state == GridObjectState.IN_STORAGE) && (currentlyFocusedObject.ItemData.canBeDiscardedByPlayer || currentlyFocusedObject.state == GridObjectState.BEING_HARVESTED) && !GameManager.Instance.GridManager.IsInRepairMode)
		{
			this.image.color = Color.white;
			this.discardDredgeAction.Enable();
			if (currentlyFocusedObject.state == GridObjectState.BEING_HARVESTED)
			{
				this.image.sprite = this.releaseIcon;
			}
			else
			{
				this.image.sprite = this.discardIcon;
			}
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionHold[] { this.discardDredgeAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		this.image.color = Color.grey;
		this.image.sprite = this.discardIcon;
		this.discardDredgeAction.Disable(true);
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionHold[] { this.discardDredgeAction };
		input2.RemoveActionListener(array, ActionLayer.UI_WINDOW);
	}

	[SerializeField]
	private ControlPromptIcon controlPromptIcon;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Sprite discardIcon;

	[SerializeField]
	private Sprite releaseIcon;

	private DredgePlayerActionHold discardDredgeAction;
}
