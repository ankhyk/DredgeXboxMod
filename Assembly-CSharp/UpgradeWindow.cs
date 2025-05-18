using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeWindow : PopupWindow
{
	protected override void Awake()
	{
		base.Awake();
		this.enterAction = new DredgePlayerActionPress("prompt.show-details", GameManager.Instance.Input.Controls.Confirm);
		this.enterAction.showInTooltip = true;
		this.enterAction.showInControlArea = false;
	}

	public void SetAllowHullTier5Content(bool allow)
	{
		this.allowHullTier5Content = allow;
	}

	public override void Show()
	{
		base.Show();
		this.allNodes.ForEach(delegate(UpgradeNodeUI node)
		{
			node.OnEntrySelected = (Action<UpgradeNodeUI>)Delegate.Combine(node.OnEntrySelected, new Action<UpgradeNodeUI>(this.OnEntrySelected));
			node.OnEntryHovered = (Action<UpgradeNodeUI>)Delegate.Combine(node.OnEntryHovered, new Action<UpgradeNodeUI>(this.OnEntryHovered));
			node.OnEntryUnhovered = (Action<UpgradeNodeUI>)Delegate.Combine(node.OnEntryUnhovered, new Action<UpgradeNodeUI>(this.OnEntryUnhovered));
		});
		GameEvents.Instance.TriggerTopUIToggleRequest(false);
		this.hullTier5Content.ForEach(delegate(GameObject o)
		{
			o.SetActive(this.allowHullTier5Content);
		});
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		this.allNodes.ForEach(delegate(UpgradeNodeUI node)
		{
			node.OnEntrySelected = (Action<UpgradeNodeUI>)Delegate.Remove(node.OnEntrySelected, new Action<UpgradeNodeUI>(this.OnEntrySelected));
			node.OnEntryHovered = (Action<UpgradeNodeUI>)Delegate.Remove(node.OnEntryHovered, new Action<UpgradeNodeUI>(this.OnEntryHovered));
			node.OnEntryUnhovered = (Action<UpgradeNodeUI>)Delegate.Remove(node.OnEntryUnhovered, new Action<UpgradeNodeUI>(this.OnEntryUnhovered));
		});
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.enterAction };
		input.RemoveActionListener(array, ActionLayer.POPUP_WINDOW);
		base.Hide(windowHideMode);
		GameEvents.Instance.TriggerTopUIToggleRequest(true);
	}

	private void OnEntrySelected(UpgradeNodeUI selectedNode)
	{
		this.controllerFocusGrabber.selectThis = selectedNode.BasicButtonWrapper.Button;
		this.ShowUpgradeGrid(selectedNode.UpgradeData);
	}

	private void OnEntryHovered(UpgradeNodeUI selectedNode)
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.enterAction };
		input.AddActionListener(array, ActionLayer.POPUP_WINDOW);
	}

	private void OnEntryUnhovered(UpgradeNodeUI selectedNode)
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.enterAction };
		input.RemoveActionListener(array, ActionLayer.POPUP_WINDOW);
	}

	private void ShowUpgradeGrid(UpgradeData upgradeData)
	{
		this.allNodes.ForEach(delegate(UpgradeNodeUI node)
		{
			node.SetInteractable(false);
		});
		this.questGridShowing = true;
		this.inventoryGridShowing = true;
		this.controllerFocusGrabber.enabled = false;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		GameManager.Instance.UI.SetIsShowingQuestGrid(true);
		GameManager.Instance.UI.UpgradeGridPanel.Show(upgradeData);
		GameManager.Instance.UI.ToggleInventoryAdHoc(true, new List<int> { 0, 1, 3 });
		this.ShowCoverScrim();
		UpgradeGridPanel upgradeGridPanel = GameManager.Instance.UI.UpgradeGridPanel;
		upgradeGridPanel.ExitEvent = (Action<int>)Delegate.Combine(upgradeGridPanel.ExitEvent, new Action<int>(this.OnQuestGridExitEvent));
	}

	private void OnQuestGridExitEvent(int response)
	{
		UpgradeGridPanel upgradeGridPanel = GameManager.Instance.UI.UpgradeGridPanel;
		upgradeGridPanel.ExitEvent = (Action<int>)Delegate.Remove(upgradeGridPanel.ExitEvent, new Action<int>(this.OnQuestGridExitEvent));
		GameManager.Instance.UI.UpgradeGridPanel.Hide();
		GameManager.Instance.UI.ToggleInventoryAdHoc(false, null);
		GameManager.Instance.UI.InventorySlidePanel.OnHideFinish.AddListener(new UnityAction(this.OnInventoryGridPanelClosed));
		GameManager.Instance.UI.UpgradeGridPanel.SlidePanel.OnHideFinish.AddListener(new UnityAction(this.OnUpgradeGridPanelClosed));
		this.HideCoverScrim();
	}

	private void OnInventoryGridPanelClosed()
	{
		GameManager.Instance.UI.InventorySlidePanel.OnHideFinish.RemoveListener(new UnityAction(this.OnInventoryGridPanelClosed));
		this.inventoryGridShowing = false;
		this.EvaluateSidePanelsClosed();
	}

	private void OnUpgradeGridPanelClosed()
	{
		GameManager.Instance.UI.UpgradeGridPanel.SlidePanel.OnHideFinish.RemoveListener(new UnityAction(this.OnUpgradeGridPanelClosed));
		this.questGridShowing = false;
		this.EvaluateSidePanelsClosed();
	}

	private void EvaluateSidePanelsClosed()
	{
		if (!this.inventoryGridShowing && !this.questGridShowing)
		{
			this.OnSideGridsClosed();
		}
	}

	private void OnSideGridsClosed()
	{
		GameManager.Instance.UI.SetIsShowingQuestGrid(false);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.POPUP_WINDOW);
		this.allNodes.ForEach(delegate(UpgradeNodeUI node)
		{
			node.SetInteractable(true);
		});
		this.controllerFocusGrabber.enabled = true;
	}

	private void ShowCoverScrim()
	{
		if (this.coverScrimTween != null)
		{
			this.coverScrimTween.Kill(false);
			this.coverScrimTween = null;
		}
		this.coverScrimTween = this.coverScrimCanvasGroup.DOFade(1f, 0.35f);
		this.coverScrimCanvasGroup.interactable = false;
		this.coverScrimCanvasGroup.blocksRaycasts = true;
		this.coverScrimTween.OnComplete(delegate
		{
			this.coverScrimTween = null;
		});
	}

	private void HideCoverScrim()
	{
		if (this.coverScrimTween != null)
		{
			this.coverScrimTween.Kill(false);
			this.coverScrimTween = null;
		}
		this.coverScrimTween = this.coverScrimCanvasGroup.DOFade(0f, 0.35f);
		this.coverScrimTween.OnComplete(delegate
		{
			this.coverScrimTween = null;
			this.coverScrimCanvasGroup.interactable = false;
			this.coverScrimCanvasGroup.blocksRaycasts = false;
		});
	}

	[SerializeField]
	private List<UpgradeNodeUI> allNodes = new List<UpgradeNodeUI>();

	[SerializeField]
	private CanvasGroup coverScrimCanvasGroup;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private List<GameObject> hullTier5Content;

	private bool allowHullTier5Content;

	private DredgePlayerActionPress enterAction;

	private Tweener coverScrimTween;

	private bool questGridShowing;

	private bool inventoryGridShowing;
}
