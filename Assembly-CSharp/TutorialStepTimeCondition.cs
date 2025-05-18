using System;
using UnityEngine;

public class TutorialStepTimeCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		this.timer = this.time;
	}

	public override void Unsubscribe()
	{
		base.Unsubscribe();
	}

	public override void Update(float deltaTime)
	{
		this.timer -= deltaTime;
		base.SetConditionMet(this.timer <= 0f);
	}

	public override float GetProgress()
	{
		return 1f - this.timer / this.time;
	}

	public override void Evaluate()
	{
	}

	[SerializeField]
	public float time;

	private float timer;
}
