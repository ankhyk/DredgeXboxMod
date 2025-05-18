using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestStepTriggerer : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnQuestStepCompleted += this.OnQuestStepCompleted;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnQuestStepCompleted -= this.OnQuestStepCompleted;
	}

	private void OnQuestStepCompleted(QuestStepData questStepData)
	{
		QuestStepTriggerable questStepTriggerable = this.questStepTriggerables.Find((QuestStepTriggerable c) => c.QuestStepData.name == questStepData.name);
		if (questStepTriggerable != null)
		{
			questStepTriggerable.Trigger();
		}
	}

	[SerializeField]
	private List<QuestStepTriggerable> questStepTriggerables = new List<QuestStepTriggerable>();
}
