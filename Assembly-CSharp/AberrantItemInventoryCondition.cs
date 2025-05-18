using System;

public class AberrantItemInventoryCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.DialogueRunner.GetNumAberrantFish() > 0;
	}

	public bool wantAberration = true;
}
