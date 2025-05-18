using System;

public class OtherQuestCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return this.state == GameManager.Instance.QuestManager.GetQuestStateById(this.quest.name);
	}

	public QuestData quest;

	public QuestState state;
}
