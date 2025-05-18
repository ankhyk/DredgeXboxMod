using System;

public abstract class QuestStepEvent
{
	public virtual bool OnItemDestroyed(string itemId)
	{
		return false;
	}

	public virtual bool OnItemSold(string itemId)
	{
		return false;
	}

	public int resolutionIndex;
}
