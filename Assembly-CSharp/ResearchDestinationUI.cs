using System;

public class ResearchDestinationUI : BaseDestinationUI
{
	protected override void ShowMainUI()
	{
		base.ShowMainUI();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.leaveAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		GameManager.Instance.UI.ResearchWindow.Show();
		ResearchWindow researchWindow = GameManager.Instance.UI.ResearchWindow;
		researchWindow.OnHideComplete = (Action)Delegate.Combine(researchWindow.OnHideComplete, new Action(this.OnResearchWindowHideComplete));
	}

	private void OnResearchWindowHideComplete()
	{
		ResearchWindow researchWindow = GameManager.Instance.UI.ResearchWindow;
		researchWindow.OnHideComplete = (Action)Delegate.Remove(researchWindow.OnHideComplete, new Action(this.OnResearchWindowHideComplete));
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		this.OnLeavePressComplete();
	}

	protected override void OnLeavePressComplete()
	{
		base.OnLeavePressComplete();
	}
}
