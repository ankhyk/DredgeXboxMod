using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructableDestinationData", menuName = "Dredge/ConstructableDestinationData", order = 0)]
public class ConstructableDestinationData : SerializedScriptableObject
{
	[SerializeField]
	public List<BaseDestinationTier> tiers = new List<BaseDestinationTier>();

	[SerializeField]
	public QuestGridConfig productQuestGrid;

	[SerializeField]
	public string itemProductPickupReminderDialogueNodeName;
}
