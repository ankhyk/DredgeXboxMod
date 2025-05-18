using System;
using UnityEngine;

public class TutorialStepUseAbilityCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilitiesChanged;
	}

	public override void Unsubscribe()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilitiesChanged;
		base.Unsubscribe();
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData, bool enabled)
	{
		base.SetConditionMet(this.abilityData.name == abilityData.name);
	}

	public override void Evaluate()
	{
	}

	[SerializeField]
	public AbilityData abilityData;
}
