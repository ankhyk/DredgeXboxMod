using System;

public class DestroyItemEvent : QuestStepEvent
{
	public override bool OnItemDestroyed(string itemId)
	{
		return this.itemId == itemId;
	}

	public string itemId;
}
