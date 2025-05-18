using System;
using UnityEngine;

public abstract class QuestStepTriggerable : MonoBehaviour
{
	public QuestStepData QuestStepData
	{
		get
		{
			return this.questStepData;
		}
	}

	private void Start()
	{
		if (this.checkTriggerOnStart && GameManager.Instance.QuestManager.GetIsQuestStepCompleted(this.questStepData.name))
		{
			this.Trigger();
		}
	}

	public abstract void Trigger();

	[SerializeField]
	private QuestStepData questStepData;

	public bool checkTriggerOnStart;
}
