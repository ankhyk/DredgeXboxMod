using System;
using DG.Tweening;
using UnityEngine;

public class TrawlActiveTab : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		this.trawlAbility = GameManager.Instance.PlayerAbilities.GetAbilityForData(this.abilityData);
		if (this.trawlAbility)
		{
			Ability ability = this.trawlAbility;
			ability.ItemCountChanged = (Action<int>)Delegate.Combine(ability.ItemCountChanged, new Action<int>(this.OnItemCountChanged));
			this.RefreshItemCount(false);
		}
		if (this.trawlAbility && this.trawlAbility.IsActive)
		{
			this.container.anchoredPosition = new Vector2(0f, this.container.anchoredPosition.y);
			return;
		}
		this.container.anchoredPosition = new Vector2(-this.container.rect.width, this.container.anchoredPosition.y);
	}

	private void OnItemCountChanged(int count)
	{
		this.RefreshItemCount(true);
	}

	private void RefreshItemCount(bool pulse)
	{
		this.itemCounterUI.SetCount(this.trawlAbility.GetItemCount(), pulse);
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (ability.name == this.abilityData.name)
		{
			this.Toggle(enabled);
		}
	}

	private void Toggle(bool show)
	{
		if (this.tween != null)
		{
			this.tween.Kill(false);
			this.tween = null;
		}
		if (show)
		{
			this.container.gameObject.SetActive(true);
		}
		float num = (show ? 0f : (-this.container.rect.width));
		this.tween = this.container.DOAnchorPosX(num, 0.5f, false);
		this.tween.SetEase(Ease.OutExpo);
		Tweener tweener = this.tween;
		tweener.onComplete = (TweenCallback)Delegate.Combine(tweener.onComplete, new TweenCallback(delegate
		{
			this.tween = null;
			if (!show)
			{
				this.container.gameObject.SetActive(false);
			}
		}));
	}

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private AbilityData abilityData;

	[SerializeField]
	private ItemCounterUI itemCounterUI;

	private Ability trawlAbility;

	private Tweener tween;
}
