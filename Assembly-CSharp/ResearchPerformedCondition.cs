using System;

public class ResearchPerformedCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.SaveData.GetBoolVariable("has-spent-research", false);
	}
}
