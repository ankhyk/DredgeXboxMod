using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestDetailWindow : PopupWindow
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void Show()
	{
		GameEvents.Instance.TriggerDetailPopupShowChange(true);
		this.containerCanvasGroup.alpha = 0f;
		base.Show();
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		base.Hide(windowHideMode);
		this.localizedTitleString.enabled = false;
		this.localizedSummaryString.enabled = false;
		GameEvents.Instance.TriggerDetailPopupShowChange(false);
	}

	public void Init(QuestData questData)
	{
		this.taskEntriesUsed = 0;
		this.localizedTitleString.StringReference = questData.titleKey;
		this.localizedTitleString.RefreshString();
		this.localizedTitleString.enabled = true;
		this.localizedSummaryString.StringReference = questData.summaryKey;
		this.localizedSummaryString.RefreshString();
		this.localizedSummaryString.enabled = true;
		this.ShowQuestData(questData);
		questData.subquests.ForEach(new Action<QuestData>(this.ShowQuestData));
		for (int i = this.taskEntriesUsed; i < this.taskUIs.Count; i++)
		{
			this.taskUIs[i].gameObject.SetActive(false);
		}
		GameEvents.Instance.TriggerHasUnseenItemsChanged();
		base.StartCoroutine(this.FlickLayoutGroups());
	}

	private void ShowQuestData(QuestData questData)
	{
		SerializedQuestEntry questEntryById = GameManager.Instance.QuestManager.GetQuestEntryById(questData.name);
		if (questEntryById == null || questEntryById.state == QuestState.UNAVAILABLE)
		{
			return;
		}
		for (int i = 0; i < questEntryById.completedStepIds.Count; i++)
		{
			string text = questEntryById.completedStepIds[i];
			QuestStepData questStepData = GameManager.Instance.QuestManager.GetQuestStepDataByStepId(text);
			if (!questStepData.hiddenWhenComplete && (!(questStepData.hideIfThisStepIsComplete != null) || !GameManager.Instance.QuestManager.GetIsQuestStepCompleted(questStepData.hideIfThisStepIsComplete.name)))
			{
				this.taskUIs[this.taskEntriesUsed].Init(questStepData.completedKey, true);
				this.taskUIs[this.taskEntriesUsed].gameObject.SetActive(true);
				this.taskEntriesUsed++;
			}
		}
		if (questEntryById.state == QuestState.STARTED)
		{
			QuestStepData questStepData = GameManager.Instance.QuestManager.GetQuestStepDataByStepId(questEntryById.activeStepId);
			if (!questStepData.hiddenWhenActive)
			{
				this.taskUIs[this.taskEntriesUsed].Init(questStepData.longActiveKey, false);
				this.taskUIs[this.taskEntriesUsed].gameObject.SetActive(true);
				this.taskEntriesUsed++;
			}
		}
		else if (questEntryById.state == QuestState.COMPLETED && questEntryById.resolutionIndex != 0)
		{
			this.taskUIs[this.taskEntriesUsed].Init(questData.resolutionKeys[questEntryById.resolutionIndex], true);
			this.taskUIs[this.taskEntriesUsed].gameObject.SetActive(true);
			this.taskEntriesUsed++;
		}
		questEntryById.hasUnseenUpdate = false;
	}

	private IEnumerator FlickLayoutGroups()
	{
		yield return new WaitForEndOfFrame();
		this.subLayoutGroups.ForEach(delegate(VerticalLayoutGroup l)
		{
			l.enabled = false;
		});
		this.subLayoutGroups.ForEach(delegate(VerticalLayoutGroup l)
		{
			l.enabled = true;
		});
		yield return new WaitForEndOfFrame();
		this.mainLayoutGroup.enabled = false;
		this.mainLayoutGroup.enabled = true;
		yield return new WaitForEndOfFrame();
		this.containerCanvasGroup.alpha = 1f;
		yield break;
	}

	[SerializeField]
	private List<VerticalLayoutGroup> subLayoutGroups;

	[SerializeField]
	private VerticalLayoutGroup mainLayoutGroup;

	[SerializeField]
	private LocalizeStringEvent localizedTitleString;

	[SerializeField]
	private LocalizeStringEvent localizedSummaryString;

	[SerializeField]
	private List<TaskEntryUI> taskUIs;

	[SerializeField]
	private CanvasGroup containerCanvasGroup;

	private int taskEntriesUsed;
}
