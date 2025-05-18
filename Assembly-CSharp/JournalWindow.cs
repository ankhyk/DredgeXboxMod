using System;
using System.Collections.Generic;
using UnityEngine;

public class JournalWindow : PopupWindow
{
	protected override void Awake()
	{
		this.questUIEntries.ForEach(delegate(QuestEntryUI q)
		{
			q.Init(this);
		});
		this.controllerFocusGrabber.enabled = false;
		base.Awake();
	}

	public override void Show()
	{
		this.questUIEntries.ForEach(delegate(QuestEntryUI q)
		{
			q.RefreshUI();
		});
		this.controllerFocusGrabber.enabled = true;
		base.Show();
		this.tabbedPanel.ShowStart();
		this.tabbedPanel.ShowFinish();
		this.controllerFocusGrabber.SelectSelectable();
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		if (this.questDetailWindow.IsShowing)
		{
			this.questDetailWindow.Hide(windowHideMode);
		}
		this.controllerFocusGrabber.enabled = false;
		base.Hide(windowHideMode);
		this.tabbedPanel.HideStart();
		this.tabbedPanel.HideFinish();
	}

	public void OnQuestEntryClicked(QuestEntryUI questEntryUI)
	{
		questEntryUI.ButtonWrapper.SetSelectable(this.controllerFocusGrabber);
		this.canvasGroupDisabler.Disable();
		this.controllerFocusGrabber.enabled = false;
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.RemoveActionListener(array, ActionLayer.POPUP_WINDOW);
		this.questDetailWindow.Init(questEntryUI.QuestData);
		this.questDetailWindow.Show();
		QuestDetailWindow questDetailWindow = this.questDetailWindow;
		questDetailWindow.OnHideComplete = (Action)Delegate.Combine(questDetailWindow.OnHideComplete, new Action(this.OnQuestPopupWindowHidden));
	}

	private void OnQuestPopupWindowHidden()
	{
		QuestDetailWindow questDetailWindow = this.questDetailWindow;
		questDetailWindow.OnHideComplete = (Action)Delegate.Remove(questDetailWindow.OnHideComplete, new Action(this.OnQuestPopupWindowHidden));
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.AddActionListener(array, ActionLayer.POPUP_WINDOW);
		this.controllerFocusGrabber.enabled = true;
		this.canvasGroupDisabler.Enable();
	}

	[SerializeField]
	private TabbedPanelContainer tabbedPanel;

	[SerializeField]
	private List<QuestEntryUI> questUIEntries;

	[SerializeField]
	private QuestDetailWindow questDetailWindow;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private CanvasGroupDisabler canvasGroupDisabler;
}
