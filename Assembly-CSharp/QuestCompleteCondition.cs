using System;
using System.Collections.Generic;

public class QuestCompleteCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		QuestManager questManager = GameManager.Instance.QuestManager;
		return this.quests.TrueForAll((QuestData q) => questManager.IsQuestCompleted(q.name));
	}

	public override string Print()
	{
		int count = this.quests.FindAll((QuestData q) => GameManager.Instance.QuestManager.IsQuestCompleted(q.name)).Count;
		return string.Format("QuestCompleteCondition: {0} [{1} / {2}]", count >= this.quests.Count, count, this.quests.Count);
	}

	public List<QuestData> quests = new List<QuestData>();
}
