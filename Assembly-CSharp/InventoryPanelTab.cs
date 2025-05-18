using System;
using UnityEngine;

public class InventoryPanelTab : MonoBehaviour
{
	private void Awake()
	{
		this.toggleAction = new DredgePlayerActionPress("prompt.cargo", GameManager.Instance.Input.Controls.ToggleCargo);
		this.altCloseAction = new DredgePlayerActionPress("alt-close-cargo-action", GameManager.Instance.Input.Controls.Back);
		this.controlPromptIcon.Init(this.toggleAction, this.toggleAction.GetPrimaryPlayerAction());
	}

	private void OnEnable()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.toggleAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnTogglePressComplete));
		DredgePlayerActionPress dredgePlayerActionPress2 = this.altCloseAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnTogglePressComplete));
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		DredgePlayerActionBase[] array;
		if (this.isUsingAltAction)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.altCloseAction, this.toggleAction };
			input.AddActionListener(array, ActionLayer.PERSISTENT);
			return;
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.toggleAction };
		input2.AddActionListener(array, ActionLayer.PERSISTENT);
	}

	private void OnDisable()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.toggleAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnTogglePressComplete));
		DredgePlayerActionPress dredgePlayerActionPress2 = this.altCloseAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnTogglePressComplete));
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.toggleAction, this.altCloseAction };
		input.RemoveActionListener(array, ActionLayer.PERSISTENT);
	}

	public void Subscribe()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.toggleAction };
		input.AddActionListener(array, ActionLayer.PERSISTENT);
	}

	public void Unsubscribe()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.toggleAction };
		input.RemoveActionListener(array, ActionLayer.PERSISTENT);
	}

	public void OnTogglePressComplete()
	{
		if (GameManager.Instance.UI.IsHarvesting)
		{
			GameManager.Instance.Player.Harvester.RequestExit();
			return;
		}
		if (!GameManager.Instance.UI.IsShowingQuestGrid)
		{
			if (GameManager.Instance.UI.InventorySlidePanel.IsShowing)
			{
				this.Close();
				return;
			}
			this.Open();
		}
	}

	private void Close()
	{
		if (GameManager.Instance.UI.InventorySlidePanel.IsShowing && GameManager.Instance.GridManager.CurrentlyHeldObject == null)
		{
			GameManager.Instance.UI.ToggleInventory(false);
			GameManager.Instance.GridManager.ClearActionHandlers();
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.altCloseAction };
			input.RemoveActionListener(array, ActionLayer.PERSISTENT);
			this.isUsingAltAction = false;
		}
	}

	private void Open()
	{
		if (!GameManager.Instance.UI.InventorySlidePanel.IsShowing && !GameManager.Instance.UI.InventorySlidePanel.WillShow)
		{
			bool flag = false;
			if (GameManager.Instance.Player.IsDocked)
			{
				BaseDestination baseDestination = GameManager.Instance.Player.CurrentDock.GetDestinations().Find((BaseDestination d) => d is StorageDestination && (d.AlwaysShow || GameManager.Instance.SaveData.availableDestinations.Contains(d.Id)));
				if (baseDestination)
				{
					flag = true;
					GameManager.Instance.UI.ShowDestination(baseDestination);
				}
			}
			if (!flag)
			{
				DredgeInputManager input = GameManager.Instance.Input;
				DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.altCloseAction };
				input.AddActionListener(array, ActionLayer.PERSISTENT);
				this.isUsingAltAction = true;
				GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
				GameManager.Instance.UI.ToggleInventory(true);
			}
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.toggleAction.Disable(false);
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			this.toggleAction.Enable();
		}
	}

	protected virtual void OnItemRemovedFromCursor(GridObject gridObject)
	{
		if (GameManager.Instance.GridManager.CurrentlyHeldObject == null)
		{
			this.toggleAction.Enable();
		}
	}

	[SerializeField]
	private ControlPromptIcon controlPromptIcon;

	private DredgePlayerActionPress toggleAction;

	private DredgePlayerActionPress altCloseAction;

	private bool isUsingAltAction;
}
