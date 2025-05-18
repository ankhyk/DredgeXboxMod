using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HasteAbilityInfoPanel : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnPlayerAbilitySelected += this.OnPlayerAbilitySelected;
		this.FetchAbility();
		if (!this.hasteAbility)
		{
			GameEvents.Instance.OnPlayerAbilityRegistered += this.OnPlayerAbilityRegistered;
		}
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilitySelected -= this.OnPlayerAbilitySelected;
		if (this.hasteAbility)
		{
			BoostAbility boostAbility = this.hasteAbility;
			boostAbility.OnBurnExceeded = (Action)Delegate.Remove(boostAbility.OnBurnExceeded, new Action(this.OnBurnExceeded));
		}
	}

	private void OnPlayerAbilityRegistered(AbilityData abilityData)
	{
		if (abilityData.name == this.hasteAbilityData.name)
		{
			this.FetchAbility();
			GameEvents.Instance.OnPlayerAbilityRegistered -= this.OnPlayerAbilityRegistered;
		}
	}

	private void FetchAbility()
	{
		this.hasteAbility = GameManager.Instance.PlayerAbilities.GetAbilityForData(this.hasteAbilityData) as BoostAbility;
		if (this.hasteAbility)
		{
			BoostAbility boostAbility = this.hasteAbility;
			boostAbility.OnBurnExceeded = (Action)Delegate.Combine(boostAbility.OnBurnExceeded, new Action(this.OnBurnExceeded));
		}
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.RefreshColors();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.COLOR_NEGATIVE)
		{
			this.RefreshColors();
		}
	}

	private void RefreshColors()
	{
		this.animatedFlameIcon.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
	}

	private void OnPlayerAbilitySelected(AbilityData abilityData)
	{
		if (abilityData.name == this.hasteAbilityData.name)
		{
			if (!this.isShowing)
			{
				this.Show();
				return;
			}
		}
		else if (this.isShowing)
		{
			this.Hide();
		}
	}

	private void Show()
	{
		this.container.SetActive(true);
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = this.canvasGroup.DOFade(1f, this.animateDurationSec);
		this.fadeTween.SetEase(Ease.OutExpo);
		this.fadeTween.OnComplete(new TweenCallback(this.OnShowComplete));
		this.fadeTween.SetUpdate(true);
		this.isShowing = true;
	}

	private void OnShowComplete()
	{
		this.fadeTween = null;
	}

	private void Hide()
	{
		this.isShowing = false;
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = this.canvasGroup.DOFade(0f, this.animateDurationSec);
		this.fadeTween.SetEase(Ease.OutExpo);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(new TweenCallback(this.OnHideComplete));
	}

	private void OnHideComplete()
	{
		this.fadeTween = null;
		this.container.SetActive(false);
	}

	private void Update()
	{
		if (this.isShowing && this.hasteAbility)
		{
			float currentBurnProp = this.hasteAbility.CurrentBurnProp;
			this.fill.fillAmount = currentBurnProp;
			this.fill.color = Color.Lerp(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.WARNING), GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE), currentBurnProp);
		}
	}

	private void OnBurnExceeded()
	{
		this.animator.SetTrigger("explode");
	}

	[SerializeField]
	private AbilityData hasteAbilityData;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private Image fill;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Image animatedFlameIcon;

	[SerializeField]
	private float animateDurationSec;

	private Tweener fadeTween;

	private bool isShowing;

	private bool isAbilityActive;

	private BoostAbility hasteAbility;
}
