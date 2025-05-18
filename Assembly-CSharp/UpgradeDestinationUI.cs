using System;

public class UpgradeDestinationUI : BaseDestinationUI
{
	protected override void ShowMainUI()
	{
		base.ShowMainUI();
		GameManager.Instance.UI.HideDialogueView();
		(GameManager.Instance.UI.UpgradeWindow as UpgradeWindow).SetAllowHullTier5Content((this.destination as UpgradeDestination).allowHullTier5Content);
		GameManager.Instance.UI.UpgradeWindow.Show();
		PopupWindow upgradeWindow = GameManager.Instance.UI.UpgradeWindow;
		upgradeWindow.OnHideComplete = (Action)Delegate.Combine(upgradeWindow.OnHideComplete, new Action(this.OnUpgradeWindowHideComplete));
	}

	protected override void ConfigureActionHandlers()
	{
		base.ConfigureActionHandlers();
	}

	private void OnUpgradeWindowHideComplete()
	{
		PopupWindow upgradeWindow = GameManager.Instance.UI.UpgradeWindow;
		upgradeWindow.OnHideComplete = (Action)Delegate.Remove(upgradeWindow.OnHideComplete, new Action(this.OnUpgradeWindowHideComplete));
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		this.OnLeavePressComplete();
	}

	protected override void OnLeavePressComplete()
	{
		base.OnLeavePressComplete();
	}
}
