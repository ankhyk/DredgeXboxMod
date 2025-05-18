using System;
using UnityEngine;

public class CabinPanel : TabbedPanel
{
	public override void ShowStart()
	{
		base.ShowStart();
	}

	public override void ShowFinish()
	{
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		base.ShowFinish();
		if (!this.hasAddedListeners)
		{
			this.hasAddedListeners = true;
			BasicButtonWrapper basicButtonWrapper = this.mapButton;
			basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnMapButtonClicked));
			BasicButtonWrapper basicButtonWrapper2 = this.journalButton;
			basicButtonWrapper2.OnClick = (Action)Delegate.Combine(basicButtonWrapper2.OnClick, new Action(this.OnJournalButtonClicked));
			BasicButtonWrapper basicButtonWrapper3 = this.messagesButton;
			basicButtonWrapper3.OnClick = (Action)Delegate.Combine(basicButtonWrapper3.OnClick, new Action(this.OnMessagesButtonClicked));
			BasicButtonWrapper basicButtonWrapper4 = this.encyclopediaButton;
			basicButtonWrapper4.OnClick = (Action)Delegate.Combine(basicButtonWrapper4.OnClick, new Action(this.OnEncyclopediaButtonClicked));
		}
	}

	public override void HideStart()
	{
		BasicButtonWrapper basicButtonWrapper = this.mapButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnMapButtonClicked));
		BasicButtonWrapper basicButtonWrapper2 = this.journalButton;
		basicButtonWrapper2.OnClick = (Action)Delegate.Remove(basicButtonWrapper2.OnClick, new Action(this.OnJournalButtonClicked));
		BasicButtonWrapper basicButtonWrapper3 = this.messagesButton;
		basicButtonWrapper3.OnClick = (Action)Delegate.Remove(basicButtonWrapper3.OnClick, new Action(this.OnMessagesButtonClicked));
		BasicButtonWrapper basicButtonWrapper4 = this.encyclopediaButton;
		basicButtonWrapper4.OnClick = (Action)Delegate.Remove(basicButtonWrapper4.OnClick, new Action(this.OnEncyclopediaButtonClicked));
		this.hasAddedListeners = false;
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
		base.HideStart();
	}

	public override void HideFinish()
	{
		base.HideFinish();
	}

	public void OnMapButtonClicked()
	{
		this.mapButton.SetSelectable(this.controllerFocusGrabber);
		GameManager.Instance.UI.InventoryPanelTab.Unsubscribe();
		PopupWindow mapWindow = GameManager.Instance.UI.MapWindow;
		mapWindow.OnHideComplete = (Action)Delegate.Combine(mapWindow.OnHideComplete, new Action(this.OnMapWindowClosed));
		GameManager.Instance.UI.MapWindow.Show();
	}

	private void OnMapWindowClosed()
	{
		PopupWindow mapWindow = GameManager.Instance.UI.MapWindow;
		mapWindow.OnHideComplete = (Action)Delegate.Remove(mapWindow.OnHideComplete, new Action(this.OnMapWindowClosed));
		GameManager.Instance.UI.InventoryPanelTab.Subscribe();
	}

	public void OnJournalButtonClicked()
	{
		this.journalButton.SetSelectable(this.controllerFocusGrabber);
		GameManager.Instance.UI.InventoryPanelTab.Unsubscribe();
		PopupWindow journalWindow = GameManager.Instance.UI.JournalWindow;
		journalWindow.OnHideComplete = (Action)Delegate.Combine(journalWindow.OnHideComplete, new Action(this.OnJournalWindowClosed));
		GameManager.Instance.UI.JournalWindow.Show();
	}

	private void OnJournalWindowClosed()
	{
		PopupWindow journalWindow = GameManager.Instance.UI.JournalWindow;
		journalWindow.OnHideComplete = (Action)Delegate.Remove(journalWindow.OnHideComplete, new Action(this.OnJournalWindowClosed));
		GameManager.Instance.UI.InventoryPanelTab.Subscribe();
	}

	public void OnMessagesButtonClicked()
	{
		this.messagesButton.SetSelectable(this.controllerFocusGrabber);
		GameManager.Instance.UI.InventoryPanelTab.Unsubscribe();
		PopupWindow messagesWindow = GameManager.Instance.UI.MessagesWindow;
		messagesWindow.OnHideComplete = (Action)Delegate.Combine(messagesWindow.OnHideComplete, new Action(this.OnMessagesWindowClosed));
		GameManager.Instance.UI.MessagesWindow.Show();
	}

	private void OnMessagesWindowClosed()
	{
		PopupWindow messagesWindow = GameManager.Instance.UI.MessagesWindow;
		messagesWindow.OnHideComplete = (Action)Delegate.Remove(messagesWindow.OnHideComplete, new Action(this.OnMessagesWindowClosed));
		GameManager.Instance.UI.InventoryPanelTab.Subscribe();
	}

	public void OnEncyclopediaButtonClicked()
	{
		this.encyclopediaButton.SetSelectable(this.controllerFocusGrabber);
		GameManager.Instance.UI.InventoryPanelTab.Unsubscribe();
		PopupWindow encyclopediaWindow = GameManager.Instance.UI.EncyclopediaWindow;
		encyclopediaWindow.OnHideComplete = (Action)Delegate.Combine(encyclopediaWindow.OnHideComplete, new Action(this.OnEncyclopediaWindowClosed));
		GameManager.Instance.UI.EncyclopediaWindow.Show();
	}

	private void OnEncyclopediaWindowClosed()
	{
		PopupWindow encyclopediaWindow = GameManager.Instance.UI.EncyclopediaWindow;
		encyclopediaWindow.OnHideComplete = (Action)Delegate.Remove(encyclopediaWindow.OnHideComplete, new Action(this.OnEncyclopediaWindowClosed));
		GameManager.Instance.UI.InventoryPanelTab.Subscribe();
	}

	public override void SwitchToSide()
	{
	}

	[SerializeField]
	private BasicButtonWrapper mapButton;

	[SerializeField]
	private BasicButtonWrapper journalButton;

	[SerializeField]
	private BasicButtonWrapper messagesButton;

	[SerializeField]
	private BasicButtonWrapper encyclopediaButton;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private UIWindowType windowType;

	private bool hasAddedListeners;
}
