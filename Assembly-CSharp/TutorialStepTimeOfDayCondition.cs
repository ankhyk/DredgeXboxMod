using System;
using UnityEngine;

public class TutorialStepTimeOfDayCondition : TutorialStepCondition
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
		if (GameManager.Instance.Time)
		{
			this.timeNow = GameManager.Instance.Time.Time;
			base.SetConditionMet(this.timeNow >= this.minTimeOfDay && this.timeNow <= this.maxTimeOfDay);
		}
	}

	[SerializeField]
	public float minTimeOfDay;

	[SerializeField]
	public float maxTimeOfDay;

	private float timeNow;
}
