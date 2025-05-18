using System;
using UnityEngine;

public class QuestStepAnimatorTriggerable : QuestStepTriggerable
{
	public override void Trigger()
	{
		this.animator.SetTrigger(this.triggerName);
	}

	public virtual void OnAnimationComplete()
	{
	}

	public Animator animator;

	public string triggerName;
}
