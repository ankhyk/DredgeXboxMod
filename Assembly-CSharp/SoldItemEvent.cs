using System;

public class SoldItemEvent : QuestStepEvent
{
	public override bool OnItemSold(string itemId)
	{
		return this.itemId == itemId;
	}

	public string itemId;
}
