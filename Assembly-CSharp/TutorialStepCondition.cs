using System;
using UnityEngine;

public abstract class TutorialStepCondition
{
	public bool IsConditionMet
	{
		get
		{
			return this.isConditionMet;
		}
	}

	public virtual void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		this.callback = callback;
		this.linkedData = linkedData;
		if (this.evaluateOnSubscribe)
		{
			this.isConditionMet = false;
			this.Evaluate();
		}
	}

	public virtual void Unsubscribe()
	{
		this.isConditionMet = false;
		this.callback = null;
	}

	public virtual float GetProgress()
	{
		return 0f;
	}

	public virtual void Update(float deltaTime)
	{
	}

	public abstract void Evaluate();

	protected void SetConditionMet(bool met)
	{
		if (this.isConditionMet == met)
		{
			return;
		}
		this.isConditionMet = met;
		if (this.isConditionMet)
		{
			Action<TutorialStepData> action = this.callback;
			if (action == null)
			{
				return;
			}
			action(this.linkedData);
		}
	}

	[SerializeField]
	public bool evaluateOnSubscribe;

	private bool isConditionMet;

	[SerializeField]
	public bool reportsProgress;

	private Action<TutorialStepData> callback;

	private TutorialStepData linkedData;
}
