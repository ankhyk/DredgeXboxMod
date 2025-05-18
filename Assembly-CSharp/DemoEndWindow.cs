using System;
using UnityEngine;

public class DemoEndWindow : PopupWindow
{
	private void OnEnable()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
	}

	private void OnDisable()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
	}

	private void OnAnimationComplete()
	{
		this.controllerFocusGrabber.enabled = true;
		this.controllerFocusGrabber.SelectSelectable();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.POPUP_WINDOW);
	}

	public override void Show()
	{
		base.Show();
		BasicButtonWrapper basicButtonWrapper = this.menuButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnMenuButtonClicked));
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		this.controllerFocusGrabber.enabled = false;
		base.Hide(windowHideMode);
		BasicButtonWrapper basicButtonWrapper = this.menuButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnMenuButtonClicked));
	}

	private void OnMenuButtonClicked()
	{
		if (this.didClickMenu)
		{
			return;
		}
		this.didClickMenu = true;
		GameManager.Instance.Loader.LoadTitleFromGame();
	}

	[SerializeField]
	private BasicButtonWrapper menuButton;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private AnimationEvents animationEvents;

	private bool didClickMenu;
}
