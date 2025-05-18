using System;

public class EmptyCondition : CompletedGridCondition
{
	public override bool Evaluate(SerializableGrid grid)
	{
		return grid.spatialItems.Count == 0;
	}
}
