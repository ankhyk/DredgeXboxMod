using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShortcutResponder : MonoBehaviour
{
	private void Awake()
	{
		this.competingWindows.Add(GameManager.Instance.UI.MapWindow);
		this.competingWindows.Add(GameManager.Instance.UI.JournalWindow);
		this.competingWindows.Add(GameManager.Instance.UI.EncyclopediaWindow);
		this.competingWindows.Add(GameManager.Instance.UI.MessagesWindow);
		this.openMapShortcutAction = new DredgePlayerActionPress("prompt.open-map", GameManager.Instance.Input.Controls.OpenMap);
		this.openMapShortcutAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.openMapShortcutAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnMapShortcutPressed));
		this.openMapShortcutAction.showInControlArea = false;
		this.openJournalShortcutAction = new DredgePlayerActionPress("prompt.open-journal", GameManager.Instance.Input.Controls.OpenJournal);
		this.openJournalShortcutAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.openJournalShortcutAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnJournalShortcutPressed));
		this.openJournalShortcutAction.showInControlArea = false;
		this.openEncyclopediaShortcutAction = new DredgePlayerActionPress("prompt.open-encyclopedia", GameManager.Instance.Input.Controls.OpenEncyclopedia);
		this.openEncyclopediaShortcutAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress3 = this.openEncyclopediaShortcutAction;
		dredgePlayerActionPress3.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressComplete, new Action(this.OnEncyclopediaShortcutPressed));
		this.openEncyclopediaShortcutAction.showInControlArea = false;
		this.openMessagesShortcutAction = new DredgePlayerActionPress("prompt.open-messages", GameManager.Instance.Input.Controls.OpenMessages);
		this.openMessagesShortcutAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress4 = this.openMessagesShortcutAction;
		dredgePlayerActionPress4.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress4.OnPressComplete, new Action(this.OnMessagesShortcutPressed));
		this.openMessagesShortcutAction.showInControlArea = false;
	}

	private void OnEnable()
	{
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.openMapShortcutAction, this.openJournalShortcutAction, this.openEncyclopediaShortcutAction, this.openMessagesShortcutAction }, ActionLayer.PERSISTENT);
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.openMapShortcutAction, this.openJournalShortcutAction, this.openEncyclopediaShortcutAction, this.openMessagesShortcutAction }, ActionLayer.PERSISTENT);
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.cameraAbilityData.name)
		{
			this.isInPhotoMode = enabled;
		}
	}

	private void OnMapShortcutPressed()
	{
		this.TryToggleWindow(GameManager.Instance.UI.MapWindow);
	}

	private void OnJournalShortcutPressed()
	{
		this.TryToggleWindow(GameManager.Instance.UI.JournalWindow);
	}

	private void OnEncyclopediaShortcutPressed()
	{
		this.TryToggleWindow(GameManager.Instance.UI.EncyclopediaWindow);
	}

	private void OnMessagesShortcutPressed()
	{
		this.TryToggleWindow(GameManager.Instance.UI.MessagesWindow);
	}

	private void TryToggleWindow(PopupWindow window)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			if (GameManager.Instance.GridManager.IsShowingGrid)
			{
				return;
			}
			if (GameManager.Instance.Player.IsDocked)
			{
				return;
			}
			if (GameManager.Instance.UI.InventorySlidePanel.IsShowing)
			{
				return;
			}
			if (this.competingWindows.Any((PopupWindow w) => w.IsShowing))
			{
				return;
			}
		}
		if (window.IsShowing)
		{
			window.Hide(PopupWindow.WindowHideMode.CLOSE);
			return;
		}
		if (this.isInPhotoMode)
		{
			return;
		}
		if (GameManager.Instance.UI.IsInCutscene)
		{
			return;
		}
		if ((GameManager.Instance.UI.CurrentDestination != null && !(GameManager.Instance.UI.CurrentDestination is StorageDestination)) || (GameManager.Instance.UI.CurrentDestination is StorageDestination && !GameManager.Instance.UI.CurrentDestinationUI.HasFinishedLoading))
		{
			return;
		}
		if (GameManager.Instance.DialogueRunner.IsDialogueRunning)
		{
			return;
		}
		if (GameManager.Instance.GridManager.CurrentlyHeldObject != null)
		{
			return;
		}
		if (GameManager.Instance.Player.IsFishing)
		{
			return;
		}
		for (int i = 0; i < this.competingWindows.Count; i++)
		{
			PopupWindow popupWindow = this.competingWindows[i];
			if (!(popupWindow == window) && popupWindow.IsShowing)
			{
				popupWindow.Hide(PopupWindow.WindowHideMode.SWITCH);
			}
		}
		window.Show();
	}

	[SerializeField]
	private AbilityData cameraAbilityData;

	private DredgePlayerActionPress openMapShortcutAction;

	private DredgePlayerActionPress openJournalShortcutAction;

	private DredgePlayerActionPress openEncyclopediaShortcutAction;

	private DredgePlayerActionPress openMessagesShortcutAction;

	private List<PopupWindow> competingWindows = new List<PopupWindow>();

	private bool isInPhotoMode;
}
