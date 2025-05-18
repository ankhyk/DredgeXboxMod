using System;

public class RecipeEntryCalloutCondition : AttentionCalloutCondition
{
	public override bool Evaluate(AttentionCallout callout)
	{
		return !(this.recipeData == null) && callout.recipeEntry.RecipeData.name == this.recipeData.name && GameManager.Instance.QuestManager.GetIsQuestStepActive(this.activeQuestStepData.name);
	}

	public RecipeData recipeData;

	public QuestStepData activeQuestStepData;
}
