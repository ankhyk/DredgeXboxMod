using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TimePassScrimUI : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
		this.container.SetActive(false);
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
	}

	private void OnTimeForcefullyPassingChanged(bool passing, string reasonKey, TimePassageMode mode)
	{
		this.isPassing = passing;
		this.container.SetActive(this.isPassing);
		if (!passing)
		{
			BasicButtonWrapper basicButtonWrapper = this.cancelButtonWrapper;
			basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnCancelButtonPressed));
			return;
		}
		this.text.StringReference.SetReference(LanguageManager.STRING_TABLE, reasonKey);
		if (mode == TimePassageMode.SLEEP)
		{
			this.cancelButtonObject.SetActive(true);
			this.cancelButtonPrompt.Init(null, GameManager.Instance.Input.Controls.GetPlayerAction(DredgeControlEnum.BACK));
			BasicButtonWrapper basicButtonWrapper2 = this.cancelButtonWrapper;
			basicButtonWrapper2.OnClick = (Action)Delegate.Combine(basicButtonWrapper2.OnClick, new Action(this.OnCancelButtonPressed));
			return;
		}
		this.cancelButtonObject.SetActive(false);
	}

	private void OnCancelButtonPressed()
	{
		GameManager.Instance.Time.StopForcefullyPassingTime();
	}

	private void Update()
	{
		if (this.isPassing)
		{
			this.progressImage.fillAmount = 1f - GameManager.Instance.Time.GetProportionForcefullyPassedTimeRemaining();
		}
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject hourglass;

	[SerializeField]
	private Image progressImage;

	[SerializeField]
	private LocalizeStringEvent text;

	[SerializeField]
	private GameObject cancelButtonObject;

	[SerializeField]
	private ControlPromptIcon cancelButtonPrompt;

	[SerializeField]
	private BasicButtonWrapper cancelButtonWrapper;

	private bool isPassing;
}
