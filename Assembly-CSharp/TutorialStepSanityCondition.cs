using System;
using UnityEngine;

public class TutorialStepSanityCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		if (this.evaluateOnSubscribe)
		{
			this.Update(0f);
		}
	}

	public override void Unsubscribe()
	{
		base.Unsubscribe();
	}

	public override void Update(float deltaTime)
	{
		this.Evaluate();
	}

	public override void Evaluate()
	{
		if (GameManager.Instance.Player)
		{
			this.sanityNow = GameManager.Instance.Player.Sanity.CurrentSanity;
			base.SetConditionMet(this.sanityNow <= this.sanityThreshold);
		}
	}

	[SerializeField]
	public float sanityThreshold;

	private float sanityNow;
}
