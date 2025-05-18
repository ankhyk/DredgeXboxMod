using System;
using UnityEngine;

public class TutorialStepHarvestCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		GameEvents.Instance.OnHarvestModeToggled += this.OnHarvestModeToggled;
	}

	public override void Unsubscribe()
	{
		GameEvents.Instance.OnHarvestModeToggled -= this.OnHarvestModeToggled;
		base.Unsubscribe();
	}

	private void OnHarvestModeToggled(bool isHarvesting)
	{
		this.Evaluate();
	}

	public override void Evaluate()
	{
		bool isHarvesting = GameManager.Instance.UI.IsHarvesting;
		base.SetConditionMet((this.triggerOnHarvestEnter && isHarvesting) || (this.triggerOnHarvestExit && !isHarvesting));
	}

	[SerializeField]
	public bool triggerOnHarvestEnter;

	[SerializeField]
	public bool triggerOnHarvestExit;
}
