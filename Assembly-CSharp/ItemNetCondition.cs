using System;

public class ItemNetCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.SaveData.GetNumItemInGridById(this.itemId, GameManager.Instance.SaveData.TrawlNet) > 0;
	}

	public string itemId;
}
