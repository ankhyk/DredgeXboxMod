using System;
using System.Collections.Generic;

public class QuestStepCompleteCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		QuestManager questManager = GameManager.Instance.QuestManager;
		return this.questSteps.TrueForAll((QuestStepData q) => questManager.GetIsQuestStepCompleted(q.name));
	}

	public override string Print()
	{
		int count = this.questSteps.FindAll((QuestStepData q) => GameManager.Instance.QuestManager.GetIsQuestStepCompleted(q.name)).Count;
		return string.Format("QuestStepCompleteCondition: {0} [{1} / {2}]", count >= this.questSteps.Count, count, this.questSteps.Count);
	}

	public List<QuestStepData> questSteps = new List<QuestStepData>();
}
