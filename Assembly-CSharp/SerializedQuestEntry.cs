using System;
using System.Collections.Generic;

[Serializable]
public class SerializedQuestEntry
{
	public string id;

	public QuestState state;

	public string activeStepId;

	public List<string> completedStepIds;

	public int resolutionIndex;

	public bool hasUnseenUpdate;
}
