using System;

public class ItemInventoryCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.DialogueRunner.GetNumItemInInventoryById(this.itemId) > 0;
	}

	public string itemId;
}
