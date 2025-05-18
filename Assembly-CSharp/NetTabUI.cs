using System;
using UnityEngine;

public class NetTabUI : MonoBehaviour
{
	private void FetchAbility()
	{
		this.ability = GameManager.Instance.PlayerAbilities.GetAbilityForData(this.abilityData);
	}

	private void OnEnable()
	{
		if (this.ability == null)
		{
			this.FetchAbility();
		}
		if (this.ability)
		{
			Ability ability = this.ability;
			ability.ItemCountChanged = (Action<int>)Delegate.Combine(ability.ItemCountChanged, new Action<int>(this.OnItemCountChanged));
			this.itemCounterUI.SetCount(this.ability.GetItemCount(), false);
		}
	}

	private void OnDisable()
	{
		if (this.ability)
		{
			Ability ability = this.ability;
			ability.ItemCountChanged = (Action<int>)Delegate.Remove(ability.ItemCountChanged, new Action<int>(this.OnItemCountChanged));
		}
	}

	private void OnItemCountChanged(int count)
	{
		this.itemCounterUI.SetCount(count, false);
	}

	[SerializeField]
	private ItemCounterUI itemCounterUI;

	[SerializeField]
	private AbilityData abilityData;

	private Ability ability;
}
