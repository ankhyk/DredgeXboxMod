using System;

public abstract class QuestStepCondition
{
	public abstract bool Evaluate();

	public bool silent;
}
