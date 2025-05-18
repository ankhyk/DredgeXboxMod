using System;
using System.Collections.Generic;

public class NodeVisitedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		DredgeDialogueRunner dialogueRunner = GameManager.Instance.DialogueRunner;
		return this.nodeNames.TrueForAll((string n) => dialogueRunner.GetHasVisitedNode(n));
	}

	public override string Print()
	{
		int count = this.nodeNames.FindAll((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n)).Count;
		return string.Format("NodeVisitedCondition: {0} [{1} / {2}]", count >= this.nodeNames.Count, count, this.nodeNames.Count);
	}

	public List<string> nodeNames = new List<string>();
}
