using System;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
	public bool IsShowing { get; set; }

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.RefreshText();
		this.transitionEffect.effectFactor = (this.IsShowing ? 1f : 0f);
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CONTROL_BINDINGS)
		{
			this.RefreshText();
		}
	}

	public void SetData(TutorialStepData stepData)
	{
		this.stepData = stepData;
		this.RefreshText();
	}

	public void RefreshText()
	{
		if (this.stepData)
		{
			string text = DredgeStringLookupHelper.ColorSwappedString(GameManager.Instance.LanguageManager.FormatControlPromptString(this.stepData.localizedString, this.stepData.stringArgumentControls));
			this.textField.text = text;
		}
	}

	public void SetProgress(float progress)
	{
		if (this.progressBar)
		{
			this.progressBar.fillAmount = progress;
		}
	}

	public void Show(TutorialStepData stepData)
	{
		if (stepData)
		{
			this.rectTransform.pivot = stepData.pivot;
			this.rectTransform.anchorMin = stepData.anchorMin;
			this.rectTransform.anchorMax = stepData.anchorMax;
			this.rectTransform.anchoredPosition = stepData.position;
		}
		this.transitionEffect.Show(true);
		this.IsShowing = true;
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			this.transitionEffect.Hide(true);
		}
		else
		{
			this.transitionEffect.effectFactor = 0f;
		}
		this.IsShowing = false;
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private Image progressBar;

	[SerializeField]
	private UITransitionEffect transitionEffect;

	[SerializeField]
	private RectTransform rectTransform;

	private TutorialStepData stepData;
}
