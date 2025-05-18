using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "QuestStepData", menuName = "Dredge/Quest/QuestStepData", order = 0)]
public class QuestStepData : SerializedScriptableObject
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(QuestStepCondition));
	}

	public IEnumerable<Type> GetEventType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(QuestStepEvent));
	}

	public bool CanBeShown()
	{
		if (this.showConditions != null && this.showConditions.Count > 0)
		{
			return this.showConditions.TrueForAll((QuestStepCondition c) => c.Evaluate());
		}
		return true;
	}

	[SerializeField]
	public List<MapMarkerData> mapMarkersToAddOnStart;

	[SerializeField]
	public List<MapMarkerData> mapMarkersToDeleteOnCompletion;

	[SerializeField]
	public bool hiddenWhenActive;

	[SerializeField]
	public bool hiddenWhenComplete;

	[Header("Localization Strings")]
	[SerializeField]
	public LocalizedString shortActiveKey;

	[SerializeField]
	public LocalizedString longActiveKey;

	[SerializeField]
	public LocalizedString completedKey;

	[SerializeField]
	public QuestStepData hideIfThisStepIsComplete;

	[Space(50f)]
	[Header("Speaker Button Configuration")]
	[SerializeField]
	public bool showAtDock;

	[SerializeField]
	public DockData stepDock;

	[SerializeField]
	public bool showAtSpeaker;

	[SerializeField]
	public SpeakerData stepSpeaker;

	[SerializeField]
	public string yarnRootNode;

	[SerializeField]
	public List<QuestStepCondition> showConditions = new List<QuestStepCondition>();

	[Space(50f)]
	[Header("Failure Conditions")]
	[SerializeField]
	public bool canBeFailed;

	[SerializeField]
	public List<QuestStepEvent> failureEvents = new List<QuestStepEvent>();

	[Space(50f)]
	[Header("Completion Conditions")]
	[SerializeField]
	public bool allowAutomaticCompletion;

	[SerializeField]
	public ConditionMode conditionMode = ConditionMode.ALL;

	[SerializeField]
	public List<QuestStepCondition> completeConditions = new List<QuestStepCondition>();
}
