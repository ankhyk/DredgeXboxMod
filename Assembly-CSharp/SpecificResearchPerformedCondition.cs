using System;

public class SpecificResearchPerformedCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.SaveData.GetIsItemResearched(this.item);
	}

	public SpatialItemData item;
}
