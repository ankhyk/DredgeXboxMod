using System;

public class DayCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.Time.Day >= this.day;
	}

	public int day;
}
