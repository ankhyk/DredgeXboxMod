using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct HighlightCondition
{
	public bool ShouldHighlight()
	{
		bool flag = true;
		if (this.extraConditions)
		{
			flag = this.extraConditions.Evaluate();
		}
		if (this.ifTheseStepsActive.Count > 0)
		{
			if (this.ifTheseStepsActive.Any((QuestStepData s) => GameManager.Instance.QuestManager.GetIsQuestStepActive(s.name)))
			{
				return true;
			}
		}
		bool flag2 = false;
		if (this.highlightIfNodesUnvisited.Count > 0)
		{
			flag2 = this.highlightIfNodesUnvisited.Any((string s) => !GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
		}
		bool flag3 = this.andTheseNodesVisited.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
		bool flag4 = this.andTheseStepsNotCompleted.All((QuestStepData qs) => !GameManager.Instance.QuestManager.GetIsQuestStepCompleted(qs.name));
		return this.alwaysHighlight || (flag2 && flag3 && flag4 && flag);
	}

	public bool alwaysHighlight;

	public List<QuestStepData> ifTheseStepsActive;

	public List<string> highlightIfNodesUnvisited;

	public List<string> andTheseNodesVisited;

	public List<QuestStepData> andTheseStepsNotCompleted;

	public HighlightConditionExtraData extraConditions;
}
