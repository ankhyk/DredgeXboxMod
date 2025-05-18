using System;
using UnityEngine;

public class TriggerableAnimation : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnAnimationStartRequested += this.OnAnimationTriggered;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
		if (this.canJumpToCompletedStateOnStartup && GameManager.Instance.SaveData.GetBoolVariable(this.animationId, false))
		{
			this.SkipToCompletedState();
		}
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnAnimationStartRequested -= this.OnAnimationTriggered;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
	}

	private void OnAnimationTriggered(string animationId)
	{
		if (animationId == this.animationId)
		{
			this.Trigger();
		}
	}

	public void Trigger()
	{
		this.animator.SetTrigger(this.animatorTriggerId);
	}

	public void SkipToCompletedState()
	{
		this.animator.SetTrigger(this.skipAnimationTriggerId);
	}

	public void OnAnimationComplete()
	{
		GameEvents.Instance.TriggerAnimationCompleted(this.animationId);
		if (this.saveStateOnComplete)
		{
			GameManager.Instance.SaveData.SetBoolVariable(this.animationId, true);
		}
	}

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string animatorTriggerId;

	[SerializeField]
	private string animationId;

	[SerializeField]
	private bool saveStateOnComplete;

	[SerializeField]
	private bool canJumpToCompletedStateOnStartup;

	[SerializeField]
	private string skipAnimationTriggerId;
}
