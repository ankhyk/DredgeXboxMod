using System;

public class ConstructionTierCalloutCondition : AttentionCalloutCondition
{
	public override bool Evaluate(AttentionCallout callout)
	{
		return !(this.matchingDestinationData == null) && callout.constructableDestinationUI.ConstructableDestinationData.name == this.matchingDestinationData.name && GameManager.Instance.QuestManager.GetIsQuestStepActive(this.activeQuestStepData.name);
	}

	public ConstructableDestinationData matchingDestinationData;

	public QuestStepData activeQuestStepData;
}
