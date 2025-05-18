using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
	public CreditsMode CreditsMode
	{
		get
		{
			return this.creditsMode;
		}
	}

	private void Awake()
	{
		this.longCreditsCanvasGroup.alpha = 0f;
	}

	public void SetCreditsMode(CreditsMode creditsMode)
	{
		this.creditsMode = creditsMode;
		this.scrimImage.color = new Color(0f, 0f, 0f, (creditsMode == CreditsMode.SHOWING_ON_MENU) ? 0.8f : 0.5f);
	}

	private void Start()
	{
		this.returnAction = new DredgePlayerActionHold("prompt.skip", GameManager.Instance.Input.Controls.Back, 1f);
		this.returnAction.showInControlArea = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.returnAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnReturnPressComplete));
		if (this.creditsMode == CreditsMode.SHOWING_ON_MENU)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.returnAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		}
		ApplicationEvents.Instance.TriggerCreditsToggled(true);
	}

	public void CanShowSkipAction()
	{
		if (this.creditsMode == CreditsMode.SHOWING_IN_GAME)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.returnAction };
			input.AddActionListener(array, ActionLayer.UI_WINDOW);
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		}
	}

	private void OnReturnPressComplete()
	{
		if (this.isEnding)
		{
			return;
		}
		this.isEnding = true;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.SYSTEM);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.returnAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		DredgePlayerActionHold dredgePlayerActionHold = this.returnAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold.OnPressComplete, new Action(this.OnReturnPressComplete));
		ApplicationEvents.Instance.TriggerCreditsToggled(false);
		if (this.creditsMode == CreditsMode.SHOWING_ON_MENU)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (this.creditsMode == CreditsMode.SHOWING_IN_GAME)
		{
			GameManager.Instance.Loader.LoadTitleFromGame();
		}
	}

	private void OnMainCreditsComplete()
	{
		this.longCreditsCanvasGroup.DOFade(1f, 0.35f).OnComplete(delegate
		{
			float num = this.longCreditsPage.rect.height + (float)Screen.height;
			float num2 = num / this.speedPxPerSecond;
			this.longCreditsPage.DOLocalMoveY(num, num2, false).SetEase(Ease.Linear).OnComplete(delegate
			{
				this.OnFullCreditsComplete();
			});
		});
	}

	private void OnFullCreditsComplete()
	{
		this.OnReturnPressComplete();
	}

	[SerializeField]
	private RectTransform longCreditsPage;

	[SerializeField]
	private CanvasGroup longCreditsCanvasGroup;

	[SerializeField]
	private float speedPxPerSecond;

	[SerializeField]
	private Image scrimImage;

	private CreditsMode creditsMode;

	private bool isEnding;

	private DredgePlayerActionHold returnAction;
}
