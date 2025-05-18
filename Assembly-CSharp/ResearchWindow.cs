using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchWindow : PopupWindow
{
	protected override void Awake()
	{
		this.researchAction = new DredgePlayerActionHold("prompt.research", GameManager.Instance.Input.Controls.Confirm, 0.5f);
		this.researchAction.showInTooltip = true;
		this.researchAction.showInControlArea = false;
		this.researchAction.LocalizationArguments = new object[] { "<sprite name=\"cog-icon\"/>" };
		this.researchAction.TriggerPromptArgumentsChanged();
		base.Awake();
		this.tabbedPanel.RequestShowablePanels(new List<int> { 0, 1, 2, 3 });
	}

	public override void Show()
	{
		base.Show();
		this.tabbedPanel.ShowStart();
		this.tabbedPanel.ShowFinish();
		this.animatedResearchItemCountImage.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		GameManager.Instance.ResearchHelper.UpdatePartCount();
		this.UpdateResearchItemCount();
		GameEvents.Instance.OnItemInventoryChanged += this.OnInventoryItemChanged;
		GameEvents.Instance.OnResearchCompleted += this.OnResearchCompleted;
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		base.Hide(windowHideMode);
		GameEvents.Instance.OnItemInventoryChanged -= this.OnInventoryItemChanged;
		GameEvents.Instance.OnResearchCompleted -= this.OnResearchCompleted;
		this.tabbedPanel.HideStart();
		this.tabbedPanel.HideFinish();
	}

	private void OnInventoryItemChanged(SpatialItemData spatialItemData)
	{
		if (spatialItemData.id == this.researchItemData.id)
		{
			this.UpdateResearchItemCount();
		}
	}

	private void OnResearchCompleted(SpatialItemData spatialItemData)
	{
		this.HideResearchPrompt(ActionLayer.POPUP_WINDOW);
	}

	private void UpdateResearchItemCount()
	{
		this.researchItemCount.text = string.Format("x{0}", GameManager.Instance.ResearchHelper.TotalPartCount);
		this.researchItemCountCopy.text = this.researchItemCount.text;
	}

	public void ShowResearchPrompt(Action promptCompleteAction, ActionLayer actionLayer = ActionLayer.POPUP_WINDOW)
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.researchAction };
		input.AddActionListener(array, actionLayer);
		this.researchAction.ClearListeners();
		DredgePlayerActionHold dredgePlayerActionHold = this.researchAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.researchAction.Reset));
		DredgePlayerActionHold dredgePlayerActionHold2 = this.researchAction;
		dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold2.OnPressComplete, promptCompleteAction);
	}

	public void HideResearchPrompt(ActionLayer actionLayer = ActionLayer.POPUP_WINDOW)
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.researchAction };
		input.RemoveActionListener(array, actionLayer);
		this.researchAction.ClearListeners();
	}

	public void FlashResearchItemCount()
	{
		this.researchCountAnimator.SetTrigger("failure");
	}

	[SerializeField]
	private TabbedPanelContainer tabbedPanel;

	[SerializeField]
	private TextMeshProUGUI researchItemCount;

	[SerializeField]
	private TextMeshProUGUI researchItemCountCopy;

	[SerializeField]
	private Image animatedResearchItemCountImage;

	[SerializeField]
	private SpatialItemData researchItemData;

	[SerializeField]
	private Animator researchCountAnimator;

	private DredgePlayerActionHold researchAction;
}
