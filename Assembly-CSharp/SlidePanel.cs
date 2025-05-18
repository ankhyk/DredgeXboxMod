using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class SlidePanel : MonoBehaviour, IScreenSideSwitchResponder
{
	public bool IsShowing
	{
		get
		{
			return this.isShowing;
		}
	}

	public bool WillShow
	{
		get
		{
			return this.willShow;
		}
	}

	private void OnEnable()
	{
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, this.screenSide);
		}
	}

	private void OnDisable()
	{
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.UnregisterSwitchResponder(this, this.screenSide);
		}
	}

	public void Toggle()
	{
		this.Toggle(!this.isShowing, false);
	}

	public void Toggle(bool show, bool canInterrupt = false)
	{
		if (!canInterrupt && (show == this.isShowing || this.isAnimating || this == null))
		{
			return;
		}
		if (this.tween != null)
		{
			this.tween.Kill(false);
			this.tween = null;
		}
		this.willShow = show;
		if (show)
		{
			this.isAnimating = true;
			UnityEvent onShowStart = this.OnShowStart;
			if (onShowStart != null)
			{
				onShowStart.Invoke();
			}
			if (this.dispatchWindowEvents)
			{
				ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
			}
			float width = (base.transform as RectTransform).rect.width;
			float num = ((this.showDirection == ShowDirection.EXTEND_RIGHT) ? width : 0f);
			this.tween = (base.gameObject.transform as RectTransform).DOAnchorPosX(num, 0.5f, false);
			this.tween.SetEase(Ease.OutExpo);
			Tweener tweener = this.tween;
			tweener.onComplete = (TweenCallback)Delegate.Combine(tweener.onComplete, new TweenCallback(delegate
			{
				this.isShowing = true;
				this.isAnimating = false;
				this.RefreshShowSwitchIcon();
				UnityEvent onShowFinish = this.OnShowFinish;
				if (onShowFinish != null)
				{
					onShowFinish.Invoke();
				}
				this.tween = null;
			}));
			GameManager.Instance.AudioPlayer.PlaySFX(this.openSFX, AudioLayer.SFX_UI, 1f, 1f);
			return;
		}
		this.isAnimating = true;
		this.RefreshShowSwitchIcon();
		UnityEvent onHideStart = this.OnHideStart;
		if (onHideStart != null)
		{
			onHideStart.Invoke();
		}
		if (this.dispatchWindowEvents)
		{
			ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
		}
		float width2 = (base.transform as RectTransform).rect.width;
		float num2 = ((this.showDirection == ShowDirection.EXTEND_RIGHT) ? 0f : width2);
		this.tween = (base.gameObject.transform as RectTransform).DOAnchorPosX(num2, 0.5f, false);
		this.tween.SetEase(Ease.OutExpo);
		Tweener tweener2 = this.tween;
		tweener2.onComplete = (TweenCallback)Delegate.Combine(tweener2.onComplete, new TweenCallback(delegate
		{
			this.isShowing = false;
			this.isAnimating = false;
			UnityEvent onHideFinish = this.OnHideFinish;
			if (onHideFinish != null)
			{
				onHideFinish.Invoke();
			}
			this.tween = null;
		}));
	}

	public void SwitchToSide()
	{
		if (this.linkedGridUI)
		{
			this.linkedGridUI.SelectFirstPlaceableCell();
		}
	}

	public void ToggleSwitchIcon(bool show)
	{
		this.shouldBeShowingSwitchIcon = show;
		this.RefreshShowSwitchIcon();
	}

	private void RefreshShowSwitchIcon()
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(this.isShowing && !this.isAnimating && this.shouldBeShowingSwitchIcon);
		}
	}

	public bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	private UIWindowType windowType;

	[Tooltip("Set this to false if this SlidePanel shouldn't dispatch window events. You would want this set to false if another component such as a TabbedPanel was dispatching the events")]
	[SerializeField]
	private bool dispatchWindowEvents;

	[SerializeField]
	private ScreenSide screenSide;

	[SerializeField]
	private GridUI linkedGridUI;

	[SerializeField]
	private AssetReference openSFX;

	[SerializeField]
	private GameObject sideSwitchIcon;

	private bool isShowing;

	private bool willShow;

	private bool isAnimating;

	public ShowDirection showDirection;

	public UnityEvent OnShowStart;

	public UnityEvent OnShowFinish;

	public UnityEvent OnHideStart;

	public UnityEvent OnHideFinish;

	private Tweener tween;

	private bool shouldBeShowingSwitchIcon;
}
