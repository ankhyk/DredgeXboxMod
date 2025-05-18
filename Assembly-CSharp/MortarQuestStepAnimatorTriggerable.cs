using System;

public class MortarQuestStepAnimatorTriggerable : QuestStepAnimatorTriggerable
{
	private void Start()
	{
		int activeTrapState = GameManager.Instance.DialogueRunner.GetActiveTrapState(this.triggerThisTrapOnAnimationComplete);
		if (activeTrapState == 1)
		{
			this.OnAnimationComplete();
		}
		if (activeTrapState == 1 || activeTrapState == 2 || activeTrapState == 3)
		{
			this.animator.SetBool("IsDestroyed", true);
		}
	}

	public override void OnAnimationComplete()
	{
		base.OnAnimationComplete();
		GameManager.Instance.QuestManager.CompleteQuestStep(this.completeThisStepOnAnimationComplete.name, false, false);
		GameManager.Instance.DialogueRunner.SetActiveTrapState(this.triggerThisTrapOnAnimationComplete, 2);
	}

	public QuestStepData completeThisStepOnAnimationComplete;

	public int triggerThisTrapOnAnimationComplete;
}
