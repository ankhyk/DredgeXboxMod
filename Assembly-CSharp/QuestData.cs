using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "QuestData", menuName = "Dredge/Quest/QuestData", order = 0)]
public class QuestData : SerializedScriptableObject
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(QuestStepCondition));
	}

	[SerializeField]
	public LocalizedString titleKey;

	[SerializeField]
	public LocalizedString summaryKey;

	[SerializeField]
	public LocalizedString[] resolutionKeys;

	[SerializeField]
	public List<MapMarkerData> mapMarkersToRemoveOnCompletion;

	[SerializeField]
	public bool showUnseenIndicators = true;

	[SerializeField]
	public List<QuestStepData> steps;

	[SerializeField]
	public List<QuestData> subquests;

	[SerializeField]
	public QuestStepData onOfferedQuestStep;

	[SerializeField]
	public bool canBeOfferedAutomatically;

	[SerializeField]
	public List<QuestStepCondition> offerConditions = new List<QuestStepCondition>();

	[SerializeField]
	public string PS5Subtask;
}
