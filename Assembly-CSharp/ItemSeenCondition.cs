using System;

public class ItemSeenCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.SaveData.historyOfItemsOwned.Contains(this.itemId);
	}

	public string itemId;
}
