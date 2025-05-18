using System;
using InControl;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScreenSideSwitcher : MonoBehaviour
{
	public bool HasUsedSideSwitchers
	{
		get
		{
			return this.hasUsedSideSwitchers;
		}
	}

	private void Awake()
	{
		ApplicationEvents.Instance.OnGameLoaded += this.Subscribe;
		ApplicationEvents.Instance.OnGameUnloaded += this.Unsubscribe;
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnGameLoaded -= this.Subscribe;
		ApplicationEvents.Instance.OnGameUnloaded -= this.Unsubscribe;
		this.Unsubscribe();
	}

	private void Subscribe()
	{
		if (this.selectLeftSide == null)
		{
			this.selectLeftSide = new DredgePlayerActionPress("prompt.select-left", GameManager.Instance.Input.Controls.SelectLeft);
			DredgePlayerActionPress dredgePlayerActionPress = this.selectLeftSide;
			dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnSelectLeft));
		}
		if (this.selectRightSide == null)
		{
			this.selectRightSide = new DredgePlayerActionPress("prompt.select-right", GameManager.Instance.Input.Controls.SelectRight);
			DredgePlayerActionPress dredgePlayerActionPress2 = this.selectRightSide;
			dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnSelectRight));
		}
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.selectLeftSide, this.selectRightSide };
		input.AddActionListener(array, ActionLayer.UI_WINDOW);
		DredgeInputManager input2 = GameManager.Instance.Input;
		input2.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input2.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
			GameEvents.Instance.OnHarvestModeToggled += this.OnHarvestModeToggled;
		}
		if (GameManager.Instance.SaveData != null)
		{
			this.hasUsedSideSwitchers = GameManager.Instance.SaveData.GetBoolVariable("has-used-side-switchers", false);
		}
	}

	private void Unsubscribe()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.selectLeftSide, this.selectRightSide };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		DredgeInputManager input2 = GameManager.Instance.Input;
		input2.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input2.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnHarvestModeToggled -= this.OnHarvestModeToggled;
			GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
		}
	}

	private void OnHarvestModeToggled(bool enabled)
	{
		if (enabled)
		{
			this.currentScreenSide = ScreenSide.RIGHT;
			this.RefreshShowSwitchIcons();
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.RefreshShowSwitchIcons();
	}

	private void OnGameWindowToggled()
	{
		this.RefreshShowSwitchIcons();
	}

	private void OnSelectLeft()
	{
		this.SwitchToSide(ScreenSide.LEFT);
	}

	private void OnSelectRight()
	{
		this.SwitchToSide(ScreenSide.RIGHT);
	}

	public void RegisterSwitchResponder(IScreenSideSwitchResponder responder, ScreenSide side)
	{
		if (side == ScreenSide.LEFT)
		{
			this.leftSideSwitchResponder = responder;
			return;
		}
		if (side == ScreenSide.RIGHT)
		{
			this.rightSideSwitchResponder = responder;
		}
	}

	public void UnregisterSwitchResponder(IScreenSideSwitchResponder responder, ScreenSide side)
	{
		if (side == ScreenSide.LEFT)
		{
			if (this.leftSideSwitchResponder == responder)
			{
				this.leftSideSwitchResponder = null;
				return;
			}
		}
		else if (side == ScreenSide.RIGHT && this.rightSideSwitchResponder == responder)
		{
			this.rightSideSwitchResponder = null;
		}
	}

	public void RegisterActivationResponder(IScreenSideActivationResponder responder, ScreenSide side)
	{
		if (side == ScreenSide.LEFT)
		{
			this.leftSideActivationResponder = responder;
			return;
		}
		if (side == ScreenSide.RIGHT)
		{
			this.rightSideActivationResponder = responder;
		}
	}

	public void UnregisterActivationResponder(IScreenSideActivationResponder responder, ScreenSide side)
	{
		if (side == ScreenSide.LEFT)
		{
			if (this.leftSideActivationResponder == responder)
			{
				this.leftSideActivationResponder = null;
				return;
			}
		}
		else if (side == ScreenSide.RIGHT && this.rightSideActivationResponder == responder)
		{
			this.rightSideActivationResponder = null;
		}
	}

	public void SwitchToSide(ScreenSide screenSide)
	{
		bool flag = false;
		if (this.currentScreenSide != screenSide)
		{
			if (GameManager.Instance.UI.PlayerTabbedPanel.CurrentIndex == 2)
			{
				return;
			}
			bool flag2 = false;
			if (screenSide == ScreenSide.LEFT && this.leftSideSwitchResponder != null)
			{
				if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject() || this.leftSideSwitchResponder.GetCanSwitchToThisIfHoldingItem())
				{
					if (!this.hasUsedSideSwitchers)
					{
						flag = true;
						this.hasUsedSideSwitchers = true;
					}
					this.leftSideSwitchResponder.SwitchToSide();
					flag2 = true;
				}
			}
			else if (screenSide == ScreenSide.RIGHT && this.rightSideSwitchResponder != null && (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject() || this.rightSideSwitchResponder.GetCanSwitchToThisIfHoldingItem()))
			{
				if (!this.hasUsedSideSwitchers)
				{
					flag = true;
					this.hasUsedSideSwitchers = true;
				}
				this.rightSideSwitchResponder.SwitchToSide();
				flag2 = true;
			}
			this.currentScreenSide = screenSide;
			if (flag2)
			{
				GameManager.Instance.AudioPlayer.PlaySFX(this.switchSFX, AudioLayer.SFX_UI, 1f, 1f);
			}
			if (flag)
			{
				GameManager.Instance.SaveData.SetBoolVariable("has-used-side-switchers", true);
			}
		}
	}

	public void OnSideBecomeActive(ScreenSide screenSide)
	{
		if (this.currentScreenSide != screenSide)
		{
			this.currentScreenSide = screenSide;
			if (this.currentScreenSide == ScreenSide.LEFT)
			{
				if (this.leftSideActivationResponder != null)
				{
					this.leftSideActivationResponder.OnSideBecomeActive();
					if (this.rightSideActivationResponder != null)
					{
						this.rightSideActivationResponder.OnSideBecomeInactive();
					}
				}
			}
			else if (this.currentScreenSide == ScreenSide.RIGHT && this.rightSideActivationResponder != null)
			{
				this.rightSideActivationResponder.OnSideBecomeActive();
				if (this.leftSideActivationResponder != null)
				{
					this.leftSideActivationResponder.OnSideBecomeInactive();
				}
			}
			this.RefreshShowSwitchIcons();
		}
	}

	private void HideAllSwitchIcons()
	{
		if (this.rightSideSwitchResponder != null)
		{
			this.rightSideSwitchResponder.ToggleSwitchIcon(false);
		}
		if (this.leftSideSwitchResponder != null)
		{
			this.leftSideSwitchResponder.ToggleSwitchIcon(false);
		}
	}

	private void RefreshShowSwitchIcons()
	{
		if (!GameManager.Instance.Input.IsUsingController || GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.CABIN))
		{
			this.HideAllSwitchIcons();
			return;
		}
		if (this.rightSideSwitchResponder != null)
		{
			this.rightSideSwitchResponder.ToggleSwitchIcon(this.currentScreenSide == ScreenSide.LEFT);
		}
		if (this.leftSideSwitchResponder != null)
		{
			this.leftSideSwitchResponder.ToggleSwitchIcon(this.currentScreenSide == ScreenSide.RIGHT);
		}
	}

	public bool CanSideBeActive(ScreenSide side)
	{
		return side == this.currentScreenSide || (side == ScreenSide.RIGHT && this.leftSideActivationResponder == null) || (side == ScreenSide.LEFT && this.rightSideActivationResponder == null);
	}

	[SerializeField]
	private AssetReference switchSFX;

	private IScreenSideSwitchResponder leftSideSwitchResponder;

	private IScreenSideSwitchResponder rightSideSwitchResponder;

	private IScreenSideActivationResponder leftSideActivationResponder;

	private IScreenSideActivationResponder rightSideActivationResponder;

	private ScreenSide currentScreenSide;

	private DredgePlayerActionPress selectLeftSide;

	private DredgePlayerActionPress selectRightSide;

	private bool hasUsedSideSwitchers;
}
