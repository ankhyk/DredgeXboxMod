using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;
using UnityEngine.Localization;

public class QuestManager : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.QuestManager = this;
		this.allQuests = GameManager.Instance.DataLoader.allQuests;
		this.allQuestSteps = GameManager.Instance.DataLoader.allQuestSteps;
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Combine(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Combine(instance2.OnGameEnded, new Action(this.OnGameEnded));
	}

	private void OnDisable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Remove(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Remove(instance2.OnGameEnded, new Action(this.OnGameEnded));
	}

	public void OnGameStarted()
	{
		this.AddTerminalCommands();
		GameEvents.Instance.OnItemDestroyed += new Action<SpatialItemData, bool>(this.OnItemDestroyed);
		GameEvents.Instance.OnItemSold += new Action<SpatialItemData>(this.OnItemSold);
		GameEvents.Instance.OnDayChanged += this.OnDayChanged;
		GameEvents.Instance.OnQuestStateChanged += this.OnQuestStateChanged;
		GameEvents.Instance.OnItemAdded += this.OnItemAdded;
		GameEvents.Instance.OnItemSeen += this.OnItemSeen;
		GameEvents.Instance.OnBuildingConstructed += this.OnBuildingConstructed;
	}

	public void OnGameEnded()
	{
		this.RemoveTerminalCommands();
		GameEvents.Instance.OnItemDestroyed -= new Action<SpatialItemData, bool>(this.OnItemDestroyed);
		GameEvents.Instance.OnItemSold -= new Action<SpatialItemData>(this.OnItemSold);
		GameEvents.Instance.OnDayChanged -= this.OnDayChanged;
		GameEvents.Instance.OnQuestStateChanged -= this.OnQuestStateChanged;
		GameEvents.Instance.OnItemAdded -= this.OnItemAdded;
		GameEvents.Instance.OnItemSeen -= this.OnItemSeen;
		GameEvents.Instance.OnBuildingConstructed -= this.OnBuildingConstructed;
	}

	private void EvaluateUnavailableQuests()
	{
		List<QuestData> list = (from qd in this.allQuests.Values.ToList<QuestData>()
			where this.GetQuestStateById(qd.name) == QuestState.UNAVAILABLE
			select qd).ToList<QuestData>();
		List<string> questsToOffer = new List<string>();
		list.ForEach(delegate(QuestData qd)
		{
			if (qd.canBeOfferedAutomatically)
			{
				if (qd.offerConditions.TrueForAll((QuestStepCondition c) => c.Evaluate()) && !questsToOffer.Contains(qd.name))
				{
					questsToOffer.Add(qd.name);
				}
			}
		});
		questsToOffer.ForEach(delegate(string id)
		{
			this.OfferQuest(id);
		});
	}

	private void OnBuildingConstructed(BuildingTierId buildingTierId)
	{
		this.EvaluateActiveQuestStepsForCompletion();
	}

	private void OnDayChanged(int day)
	{
		this.EvaluateUnavailableQuests();
	}

	private void OnQuestStateChanged(string questId, QuestState newState)
	{
		this.EvaluateUnavailableQuests();
		this.EvaluateActiveQuestStepsForCompletion();
	}

	private void OnItemAdded(SpatialItemInstance itemInstance, bool belongsToPlayer)
	{
		if (belongsToPlayer)
		{
			this.EvaluateActiveQuestStepsForCompletion();
		}
	}

	private void OnItemSeen(SpatialItemInstance itemInstance)
	{
		this.EvaluateActiveQuestStepsForCompletion();
	}

	private void EvaluateActiveQuestStepsForCompletion()
	{
		this.GetStepDataForAllActiveQuests().ForEach(delegate(QuestStepData qs)
		{
			if (qs.allowAutomaticCompletion && qs.completeConditions != null)
			{
				bool flag = false;
				bool flag2 = false;
				if (qs.conditionMode == ConditionMode.ALL)
				{
					flag2 = qs.completeConditions.TrueForAll((QuestStepCondition qe) => qe.Evaluate());
					flag = qs.completeConditions.TrueForAll((QuestStepCondition qe) => qe.silent);
				}
				else if (qs.conditionMode == ConditionMode.ANY)
				{
					QuestStepCondition questStepCondition = qs.completeConditions.FirstOrDefault((QuestStepCondition qe) => qe.Evaluate());
					if (questStepCondition != null)
					{
						flag2 = true;
						flag = questStepCondition.silent;
					}
				}
				if (flag2)
				{
					this.CompleteQuestStep(qs.name, flag, flag);
				}
			}
		});
	}

	private void OnItemDestroyed(ItemData itemData, bool playerDestroyed)
	{
		Func<QuestStepEvent, bool> <>9__1;
		this.GetStepDataForAllActiveQuests().ForEach(delegate(QuestStepData qs)
		{
			if (qs.canBeFailed && qs.failureEvents != null && qs.failureEvents.Count > 0)
			{
				IEnumerable<QuestStepEvent> failureEvents = qs.failureEvents;
				Func<QuestStepEvent, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (QuestStepEvent qe) => qe.OnItemDestroyed(itemData.id));
				}
				QuestStepEvent questStepEvent = failureEvents.FirstOrDefault(func);
				if (questStepEvent != null)
				{
					this.CompleteQuest(this.GetQuestDataByStepId(qs.name).name, questStepEvent.resolutionIndex, false);
				}
			}
		});
	}

	private void OnItemSold(ItemData itemData)
	{
		Func<QuestStepEvent, bool> <>9__1;
		this.GetStepDataForAllActiveQuests().ForEach(delegate(QuestStepData qs)
		{
			if (qs.canBeFailed && qs.failureEvents != null && qs.failureEvents.Count > 0)
			{
				IEnumerable<QuestStepEvent> failureEvents = qs.failureEvents;
				Func<QuestStepEvent, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (QuestStepEvent qe) => qe.OnItemSold(itemData.id));
				}
				QuestStepEvent questStepEvent = failureEvents.FirstOrDefault(func);
				if (questStepEvent != null)
				{
					this.CompleteQuest(this.GetQuestDataByStepId(qs.name).name, questStepEvent.resolutionIndex, false);
				}
			}
		});
	}

	public void OfferQuest(string questId)
	{
		QuestData questDataById = this.GetQuestDataById(questId);
		if (questDataById == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] OfferQuest(", questId, ") could not find valid quest with id: ", questId, ". Will not offer quest." }));
			return;
		}
		if (this.GetQuestStateById(questId) == QuestState.UNAVAILABLE)
		{
			SerializedQuestEntry serializedQuestEntry = new SerializedQuestEntry
			{
				id = questId,
				state = QuestState.OFFERED,
				completedStepIds = new List<string>()
			};
			if (questDataById.onOfferedQuestStep != null)
			{
				serializedQuestEntry.activeStepId = questDataById.onOfferedQuestStep.name;
			}
			GameManager.Instance.SaveData.questEntries.Add(questId, serializedQuestEntry);
			GameEvents.Instance.TriggerQuestStateChanged(questId, QuestState.OFFERED);
			return;
		}
	}

	public void StartQuest(string questId, bool silent = false)
	{
		QuestData questDataById = this.GetQuestDataById(questId);
		if (questDataById == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] StartQuest(", questId, ") could not find valid quest with id: ", questId, ". Will not start quest." }));
			return;
		}
		if (questDataById.steps == null || questDataById.steps.Count == 0)
		{
			CustomDebug.EditorLogError("[QuestManager] Quest has no steps. Will not start quest.");
			return;
		}
		if (this.IsQuestStarted(questId) || this.IsQuestCompleted(questId))
		{
			return;
		}
		SerializedQuestEntry serializedQuestEntry = this.GetQuestEntryById(questId);
		if (serializedQuestEntry == null)
		{
			serializedQuestEntry = new SerializedQuestEntry
			{
				id = questId,
				completedStepIds = new List<string>()
			};
			GameManager.Instance.SaveData.questEntries.Add(questId, serializedQuestEntry);
		}
		serializedQuestEntry.state = QuestState.STARTED;
		serializedQuestEntry.activeStepId = questDataById.steps[0].name;
		questDataById.steps[0].mapMarkersToAddOnStart.ForEach(delegate(MapMarkerData m)
		{
			GameManager.Instance.SaveData.AddMapMarker(m, false);
		});
		if (questDataById.showUnseenIndicators)
		{
			serializedQuestEntry.hasUnseenUpdate = true;
			GameEvents.Instance.TriggerHasUnseenItemsChanged();
		}
		GameEvents.Instance.TriggerQuestStateChanged(questId, QuestState.STARTED);
		if (!silent)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.QUEST_STARTED, "notification.quest-started");
			return;
		}
	}

	public void CompleteQuest(string questId, int resolutionIndex = 0, bool silent = false)
	{
		QuestData questDataById = this.GetQuestDataById(questId);
		if (questDataById == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] CompleteQuest(", questId, ") could not find valid quest with id: ", questId, ". Will not complete quest." }));
			return;
		}
		if (!this.IsQuestCompleted(questId))
		{
			SerializedQuestEntry serializedQuestEntry = this.GetQuestEntryById(questId);
			if (serializedQuestEntry == null)
			{
				serializedQuestEntry = new SerializedQuestEntry
				{
					id = questId,
					completedStepIds = new List<string>()
				};
				GameManager.Instance.SaveData.questEntries.Add(questId, serializedQuestEntry);
			}
			serializedQuestEntry.resolutionIndex = resolutionIndex;
			serializedQuestEntry.state = QuestState.COMPLETED;
			serializedQuestEntry.hasUnseenUpdate = false;
			questDataById.mapMarkersToRemoveOnCompletion.ForEach(delegate(MapMarkerData m)
			{
				GameManager.Instance.SaveData.RemoveMapMarker(m);
			});
			GameEvents.Instance.TriggerHasUnseenItemsChanged();
			GameEvents.Instance.TriggerQuestStateChanged(questId, QuestState.COMPLETED);
			if (!silent)
			{
				GameManager.Instance.UI.ShowNotification(NotificationType.QUEST_COMPLETED, "notification.quest-completed");
			}
			GameEvents.Instance.TriggerQuestCompleted(questDataById);
		}
	}

	public QuestData GetQuestDataByStepId(string questStepId)
	{
		Predicate<QuestStepData> <>9__1;
		return this.allQuests.Values.ToList<QuestData>().Find(delegate(QuestData q)
		{
			List<QuestStepData> steps = q.steps;
			Predicate<QuestStepData> predicate;
			if ((predicate = <>9__1) == null)
			{
				predicate = (<>9__1 = (QuestStepData qs) => qs.name == questStepId);
			}
			return steps.Find(predicate) != null || (q.onOfferedQuestStep != null && q.onOfferedQuestStep.name == questStepId);
		});
	}

	public QuestData GetQuestDataById(string questId)
	{
		QuestData questData = null;
		this.allQuests.TryGetValue(questId, out questData);
		return questData;
	}

	public QuestStepData GetQuestStepDataByStepId(string questStepId)
	{
		QuestStepData questStepData = null;
		this.allQuestSteps.TryGetValue(questStepId, out questStepData);
		return questStepData;
	}

	public LocalizedString GetShortActiveStepKeyByQuestId(string questId)
	{
		SerializedQuestEntry questEntryById = this.GetQuestEntryById(questId);
		QuestData questDataById = this.GetQuestDataById(questId);
		if (questEntryById.state == QuestState.COMPLETED && questDataById.resolutionKeys.Length != 0)
		{
			return questDataById.resolutionKeys[questEntryById.resolutionIndex];
		}
		return this.GetQuestStepDataByStepId(questEntryById.activeStepId).shortActiveKey;
	}

	public void CompleteQuestStep(string questStepId, bool suppressNotifications = false, bool suppressUnseenMarkers = false)
	{
		QuestData questDataByStepId = this.GetQuestDataByStepId(questStepId);
		QuestStepData questStepDataByStepId = this.GetQuestStepDataByStepId(questStepId);
		if (questDataByStepId == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] CompleteQuestStep(", questStepId, ") could not find valid quest for step id: ", questStepId, ". Will not progress step." }));
			return;
		}
		int num = questDataByStepId.steps.FindIndex((QuestStepData qs) => qs.name == questStepId);
		SerializedQuestEntry questEntry = this.GetQuestEntryById(questDataByStepId.name);
		if (questEntry == null)
		{
			CustomDebug.EditorLogError("[QuestManager] CompleteQuestStep(" + questStepId + ") can't find quest entry in save data.");
			return;
		}
		if (questEntry.state == QuestState.UNAVAILABLE)
		{
			return;
		}
		if (questEntry.state == QuestState.COMPLETED)
		{
			return;
		}
		if (questEntry.completedStepIds.Contains(questStepId))
		{
			return;
		}
		if (questDataByStepId.steps.FindIndex((QuestStepData qs) => qs.name == questEntry.activeStepId) > num)
		{
			return;
		}
		questEntry.completedStepIds.Add(questStepId);
		questStepDataByStepId.mapMarkersToDeleteOnCompletion.ForEach(delegate(MapMarkerData m)
		{
			GameManager.Instance.SaveData.RemoveMapMarker(m);
		});
		if (num < questDataByStepId.steps.Count - 1)
		{
			questEntry.activeStepId = questDataByStepId.steps[num + 1].name;
			this.GetQuestStepDataByStepId(questEntry.activeStepId).mapMarkersToAddOnStart.ForEach(delegate(MapMarkerData m)
			{
				GameManager.Instance.SaveData.RemoveMapMarker(m);
			});
			if (questDataByStepId.showUnseenIndicators && !suppressUnseenMarkers)
			{
				questEntry.hasUnseenUpdate = true;
			}
			if (!suppressNotifications)
			{
				GameEvents.Instance.TriggerHasUnseenItemsChanged();
				if (questEntry.state == QuestState.OFFERED)
				{
					GameManager.Instance.UI.ShowNotification(NotificationType.QUEST_STARTED, "notification.quest-started");
				}
				else if (questEntry.state == QuestState.STARTED)
				{
					GameManager.Instance.UI.ShowNotification(NotificationType.QUEST_UPDATED, "notification.quest-progressed");
				}
			}
			questEntry.state = QuestState.STARTED;
		}
		else
		{
			this.CompleteQuest(questDataByStepId.name, 0, suppressNotifications);
		}
		GameEvents.Instance.TriggerQuestStepCompleted(questStepDataByStepId);
	}

	public List<SerializedQuestEntry> GetQuestsByState(QuestState state)
	{
		return (from q in GameManager.Instance.SaveData.questEntries.Values.ToList<SerializedQuestEntry>()
			where q.state == state
			select q).ToList<SerializedQuestEntry>();
	}

	public List<QuestStepData> GetStepDataForAllActiveQuests()
	{
		List<QuestStepData> list = new List<QuestStepData>();
		List<SerializedQuestEntry> list2 = new List<SerializedQuestEntry>();
		list2.AddRange(this.GetQuestsByState(QuestState.OFFERED));
		list2.AddRange(this.GetQuestsByState(QuestState.STARTED));
		for (int i = 0; i < list2.Count; i++)
		{
			SerializedQuestEntry serializedQuestEntry = list2[i];
			global::UnityEngine.Object questDataById = this.GetQuestDataById(serializedQuestEntry.id);
			QuestStepData questStepDataByStepId = this.GetQuestStepDataByStepId(serializedQuestEntry.activeStepId);
			if (!(questDataById == null) && !(questStepDataByStepId == null))
			{
				list.Add(questStepDataByStepId);
			}
		}
		return list;
	}

	public SerializedQuestEntry GetQuestEntryById(string questId)
	{
		SerializedQuestEntry serializedQuestEntry = null;
		GameManager.Instance.SaveData.questEntries.TryGetValue(questId, out serializedQuestEntry);
		return serializedQuestEntry;
	}

	public QuestState GetQuestStateById(string questId)
	{
		SerializedQuestEntry questEntryById = this.GetQuestEntryById(questId);
		if (questEntryById == null)
		{
			return QuestState.UNAVAILABLE;
		}
		return questEntryById.state;
	}

	public bool IsQuestInactive(string questId)
	{
		return this.GetQuestStateById(questId) == QuestState.UNAVAILABLE;
	}

	public bool IsQuestAvailable(string questId)
	{
		return this.GetQuestStateById(questId) == QuestState.OFFERED;
	}

	public bool IsQuestStarted(string questId)
	{
		return this.GetQuestStateById(questId) == QuestState.STARTED;
	}

	public bool IsQuestCompleted(string questId)
	{
		return this.GetQuestStateById(questId) == QuestState.COMPLETED;
	}

	public bool IsIntroQuestCompleted()
	{
		return this.GetQuestStateById(this.introQuest.name) == QuestState.COMPLETED;
	}

	public bool GetIsQuestStepCompleted(string questStepId)
	{
		QuestData questDataByStepId = this.GetQuestDataByStepId(questStepId);
		if (questDataByStepId == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] GetIsQuestStepCompleted(", questStepId, ") could not find valid quest for step id: ", questStepId, "." }));
			return false;
		}
		SerializedQuestEntry questEntryById = this.GetQuestEntryById(questDataByStepId.name);
		return questEntryById != null && questEntryById.completedStepIds.Contains(questStepId);
	}

	public bool GetIsQuestStepActive(string questStepId)
	{
		QuestData questDataByStepId = this.GetQuestDataByStepId(questStepId);
		if (questDataByStepId == null)
		{
			CustomDebug.EditorLogError(string.Concat(new string[] { "[QuestManager] GetIsQuestStepActive(", questStepId, ") could not find valid quest for step id: ", questStepId, "." }));
			return false;
		}
		SerializedQuestEntry questEntryById = this.GetQuestEntryById(questDataByStepId.name);
		return questEntryById != null && questEntryById.activeStepId == questStepId && questEntryById.state != QuestState.COMPLETED;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("quest.list", new Action<CommandArg[]>(this.ListQuests), 0, 0, "Lists all quest ids.");
		Terminal.Shell.AddCommand("quest.steps.list", new Action<CommandArg[]>(this.ListQuestSteps), 0, 0, "Lists all quest step ids.");
		Terminal.Shell.AddCommand("quest.offer", new Action<CommandArg[]>(this.DebugOfferQuest), 1, 1, "Offers a quest e.g. 'quest.offer MyQuestId' marks the MyQuestId quest as offered.");
		Terminal.Shell.AddCommand("quest.start", new Action<CommandArg[]>(this.StartQuestDebug), 1, 1, "Starts a quest e.g. 'quest.start MyQuestId' starts the MyQuestId quest.");
		Terminal.Shell.AddCommand("quest.step", new Action<CommandArg[]>(this.CompleteQuestStepDebug), 1, 1, "Completes a quest step e.g. 'quest.step MyQuestStepId' completes the MyQuestStepId step");
		Terminal.Shell.AddCommand("quest.complete", new Action<CommandArg[]>(this.CompleteQuestDebug), 1, 1, "Completes a quest e.g. 'quest.start MyQuestId' completes the MyQuestId quest");
		Terminal.Shell.AddCommand("quest.states", new Action<CommandArg[]>(this.ListQuestsDebug), 0, 0, "Lists all quests in the player's save state");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("quest.list");
		Terminal.Shell.RemoveCommand("quest.steps.list");
		Terminal.Shell.RemoveCommand("quest.offer");
		Terminal.Shell.RemoveCommand("quest.start");
		Terminal.Shell.RemoveCommand("quest.step");
		Terminal.Shell.RemoveCommand("quest.complete");
		Terminal.Shell.RemoveCommand("quest.states");
	}

	private void ListQuests(CommandArg[] args)
	{
		string quests = "";
		this.allQuests.Values.ToList<QuestData>().ForEach(delegate(QuestData i)
		{
			quests = quests + i.name + ", ";
		});
	}

	private void ListQuestSteps(CommandArg[] args)
	{
		string steps = "";
		this.allQuestSteps.Values.ToList<QuestStepData>().ForEach(delegate(QuestStepData i)
		{
			steps = steps + i.name + ", ";
		});
	}

	private void DebugOfferQuest(CommandArg[] args)
	{
		string @string = args[0].String;
		this.OfferQuest(@string);
	}

	private void StartQuestDebug(CommandArg[] args)
	{
		string @string = args[0].String;
		this.StartQuest(@string, false);
	}

	private void CompleteQuestStepDebug(CommandArg[] args)
	{
		string @string = args[0].String;
		this.CompleteQuestStep(@string, false, false);
	}

	private void CompleteQuestDebug(CommandArg[] args)
	{
		string @string = args[0].String;
		this.CompleteQuest(@string, 0, false);
	}

	private void ListQuestsDebug(CommandArg[] args)
	{
		List<SerializedQuestEntry> questsByState = this.GetQuestsByState(QuestState.OFFERED);
		List<SerializedQuestEntry> questsByState2 = this.GetQuestsByState(QuestState.STARTED);
		List<SerializedQuestEntry> questsByState3 = this.GetQuestsByState(QuestState.COMPLETED);
		questsByState.ForEach(delegate(SerializedQuestEntry q)
		{
		});
		questsByState2.ForEach(delegate(SerializedQuestEntry q)
		{
		});
		questsByState3.ForEach(delegate(SerializedQuestEntry q)
		{
		});
	}

	public Dictionary<string, QuestData> allQuests;

	public Dictionary<string, QuestStepData> allQuestSteps;

	[SerializeField]
	private QuestData introQuest;
}
