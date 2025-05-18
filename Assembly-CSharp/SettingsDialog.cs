using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SettingsDialog : MonoBehaviour
{
	private void Awake()
	{
		BasicButtonWrapper basicButtonWrapper = this.closeButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnCloseButtonPressed));
		this.scrim.alpha = 0f;
		this.scrim.gameObject.SetActive(false);
		this.dialogCanvasGroup.alpha = 0f;
		this.dialog.gameObject.SetActive(false);
		ApplicationEvents.Instance.OnToggleSettings += this.OnToggleSettings;
		this.forceExitSliderFocusAction = new DredgePlayerActionPress("prompt.back", GameManager.Instance.Input.Controls.Back);
		this.forceExitSliderFocusAction.evaluateWhenPaused = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.forceExitSliderFocusAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.ForceSliderFocusExit));
	}

	private void OnCloseButtonPressed()
	{
		GameManager.Instance.PauseListener.OnUnpausePressComplete();
	}

	private void OnEnable()
	{
		this.sliders.ForEach(delegate(SliderSettingInput s)
		{
			s.SliderFocusChanged = (Action<SliderSettingInput, bool>)Delegate.Combine(s.SliderFocusChanged, new Action<SliderSettingInput, bool>(this.OnSliderFocusChanged));
		});
	}

	private void OnDisable()
	{
		this.sliders.ForEach(delegate(SliderSettingInput s)
		{
			s.SliderFocusChanged = (Action<SliderSettingInput, bool>)Delegate.Remove(s.SliderFocusChanged, new Action<SliderSettingInput, bool>(this.OnSliderFocusChanged));
		});
	}

	private void OnSliderFocusChanged(SliderSettingInput sliderSettingInput, bool sliderHasInnerFocus)
	{
		DredgePlayerActionBase[] array;
		if (sliderHasInnerFocus)
		{
			this.activeSlider = sliderSettingInput;
			GameManager.Instance.PauseListener.CanShowUnpauseAction(false);
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.forceExitSliderFocusAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
			return;
		}
		this.activeSlider = null;
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.forceExitSliderFocusAction };
		input2.RemoveActionListener(array, ActionLayer.SYSTEM);
		GameManager.Instance.PauseListener.CanShowUnpauseAction(true);
	}

	private void ForceSliderFocusExit()
	{
		if (this.activeSlider != null)
		{
			this.activeSlider.ForceSliderDeselect();
		}
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.forceExitSliderFocusAction };
		input.RemoveActionListener(array, ActionLayer.SYSTEM);
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnToggleSettings -= this.OnToggleSettings;
	}

	private void OnToggleSettings(bool show)
	{
		if (show)
		{
			this.Show();
			return;
		}
		this.Hide();
	}

	public void Show()
	{
		if (this.scrimTween != null)
		{
			this.scrimTween.Kill(false);
			this.scrimTween = null;
		}
		if (this.dialogTween != null)
		{
			this.dialogTween.Kill(false);
			this.dialogTween = null;
		}
		UnityEvent onSettingsDialogShown = this.OnSettingsDialogShown;
		if (onSettingsDialogShown != null)
		{
			onSettingsDialogShown.Invoke();
		}
		this.dialogCanvasGroup.alpha = 0f;
		this.scrim.gameObject.SetActive(true);
		this.dialog.gameObject.SetActive(true);
		this.dialog.ShowStart();
		this.dialog.ShowFinish();
		this.scrimTween = this.scrim.DOFade(1f, this.transitionDurationSec);
		this.scrimTween.SetUpdate(true);
		this.scrimTween.OnComplete(new TweenCallback(this.OnShowScrimComplete));
		this.dialogTween = this.dialogCanvasGroup.DOFade(1f, this.transitionDurationSec);
		this.dialogTween.SetUpdate(true);
		this.dialogTween.OnComplete(delegate
		{
			this.dialogTween = null;
		});
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
	}

	private void OnShowScrimComplete()
	{
		this.scrimTween = null;
		GameManager.Instance.PauseListener.OnSettingsToggled(true);
	}

	public void Hide()
	{
		if (this.scrimTween != null)
		{
			this.scrimTween.Kill(false);
		}
		this.dialog.HideStart();
		this.dialog.HideFinish();
		this.dialog.gameObject.SetActive(false);
		this.scrimTween = this.scrim.DOFade(0f, this.transitionDurationSec);
		this.scrimTween.SetUpdate(true);
		this.scrimTween.OnComplete(new TweenCallback(this.OnHideScrimComplete));
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
		if (this.hasSettingChangedSinceLastSave)
		{
			GameManager.Instance.SettingsSaveData.controlBindings = GameManager.Instance.Input.Controls.Save();
			GameManager.Instance.SaveSettings();
			this.hasSettingChangedSinceLastSave = false;
		}
		UnityEvent onSettingsDialogHidden = this.OnSettingsDialogHidden;
		if (onSettingsDialogHidden != null)
		{
			onSettingsDialogHidden.Invoke();
		}
		GameManager.Instance.PauseListener.OnSettingsToggled(false);
	}

	private void OnSettingChanged(SettingType settingType)
	{
		this.hasSettingChangedSinceLastSave = true;
	}

	private void OnHideScrimComplete()
	{
		this.scrimTween = null;
		this.scrim.gameObject.SetActive(false);
	}

	[SerializeField]
	private TabbedPanelContainer dialog;

	[SerializeField]
	private CanvasGroup scrim;

	[SerializeField]
	private CanvasGroup dialogCanvasGroup;

	[SerializeField]
	private float transitionDurationSec;

	[SerializeField]
	private Tweener scrimTween;

	[SerializeField]
	private Tweener dialogTween;

	[SerializeField]
	private UnityEvent OnSettingsDialogShown;

	[SerializeField]
	private UnityEvent OnSettingsDialogHidden;

	[SerializeField]
	private BasicButtonWrapper closeButton;

	[SerializeField]
	private List<SliderSettingInput> sliders;

	private bool hasSettingChangedSinceLastSave;

	private DredgePlayerActionPress forceExitSliderFocusAction;

	private SliderSettingInput activeSlider;
}
