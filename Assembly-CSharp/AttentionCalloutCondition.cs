using System;

[Serializable]
public abstract class AttentionCalloutCondition
{
	public abstract bool Evaluate(AttentionCallout callout);
}
