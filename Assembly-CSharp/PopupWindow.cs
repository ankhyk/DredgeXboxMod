using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PopupWindow : MonoBehaviour
{
	public bool IsShowing
	{
		get
		{
			return this.isShowing;
		}
	}

	protected virtual void Awake()
	{
		this.backAction = new DredgePlayerActionPress("prompt.back", GameManager.Instance.Input.Controls.Back);
		this.backAction.showInControlArea = true;
		this.backAction.priority = 0;
		this.backAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.backAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.HideProxy));
	}

	public virtual void Show()
	{
		if (this.toggleGameUI && GameEvents.Instance)
		{
			GameEvents.Instance.TriggerPopupWindowToggled(true);
		}
		if (this.pauseGame)
		{
			GameManager.Instance.PauseLite();
		}
		this.cachedActionLayer = GameManager.Instance.Input.GetActiveActionLayer();
		if (this.canBeClosedByPlayer)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.backAction }, ActionLayer.POPUP_WINDOW);
		}
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.POPUP_WINDOW);
		this.container.SetActive(true);
		this.isShowing = true;
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		Action onShowComplete = this.OnShowComplete;
		if (onShowComplete != null)
		{
			onShowComplete();
		}
		this.canvasGroup.alpha = 0f;
		this.fadeTween = this.canvasGroup.DOFade(1f, 0.35f).SetUpdate(true);
		GameManager.Instance.AudioPlayer.PlaySFX(this.openSFX, AudioLayer.SFX_UI, 1f, 1f);
		if (this.windowType == UIWindowType.ENCYCLOPEDIA || this.windowType == UIWindowType.PURSUITS || this.windowType == UIWindowType.MESSAGES || this.windowType == UIWindowType.MAP)
		{
			GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.POPUP_WINDOW);
		}
	}

	public void HideProxy()
	{
		this.Hide(PopupWindow.WindowHideMode.CLOSE);
	}

	public virtual void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		this.fadeTween.Kill(false);
		if (this.canBeClosedByPlayer)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.backAction }, ActionLayer.POPUP_WINDOW);
		}
		this.container.SetActive(false);
		this.isShowing = false;
		GameManager.Instance.Input.SetActiveActionLayer(this.cachedActionLayer);
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
		if (this.toggleGameUI)
		{
			GameEvents.Instance.TriggerPopupWindowToggled(false);
		}
		if (this.pauseGame)
		{
			GameManager.Instance.UnpauseLite();
		}
		Action onHideComplete = this.OnHideComplete;
		if (onHideComplete != null)
		{
			onHideComplete();
		}
		GameManager.Instance.AudioPlayer.PlaySFX(this.closeSFX, AudioLayer.SFX_UI, 1f, 1f);
		if (this.cachedActionLayer == ActionLayer.BASE)
		{
			GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
			return;
		}
		GameManager.Instance.ChromaManager.StopAllAnimations();
	}

	[SerializeField]
	private AssetReference openSFX;

	[SerializeField]
	private AssetReference closeSFX;

	[SerializeField]
	protected GameObject container;

	[SerializeField]
	public Action OnShowComplete;

	[SerializeField]
	public Action OnHideComplete;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	public UIWindowType windowType;

	[SerializeField]
	public bool toggleGameUI;

	[SerializeField]
	public bool pauseGame;

	[SerializeField]
	public bool canBeClosedByPlayer = true;

	protected DredgePlayerActionPress backAction;

	private Tweener fadeTween;

	private bool isShowing;

	private ActionLayer cachedActionLayer;

	public enum WindowHideMode
	{
		NONE,
		CLOSE,
		SWITCH
	}
}
