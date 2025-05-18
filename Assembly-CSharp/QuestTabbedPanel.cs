using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestTabbedPanel : TabbedPanel
{
	private void Awake()
	{
		this.quests = base.GetComponentsInChildren<QuestEntryUI>(true).ToList<QuestEntryUI>();
	}

	private void OnEnable()
	{
		this.RefreshQuestUnseenStates();
	}

	public override void ShowStart()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged += this.RefreshQuestUnseenStates;
		if (this.defaultSelectable == null)
		{
			QuestEntryUI questEntryUI = this.quests.Find((QuestEntryUI q) => q.ButtonWrapper.gameObject.activeSelf);
			if (questEntryUI)
			{
				this.defaultSelectable = questEntryUI.ButtonWrapper.Button;
			}
		}
		if (this.defaultSelectable)
		{
			this.mainControllerFocusGrabber.SetSelectable(this.defaultSelectable);
			this.mainControllerFocusGrabber.SelectSelectable();
		}
		base.ShowStart();
	}

	public override void HideFinish()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.RefreshQuestUnseenStates;
		base.HideFinish();
	}

	private void RefreshQuestUnseenStates()
	{
		this.unseenItemIcon.SetActive(this.quests.Any((QuestEntryUI q) => q.GetHasUnseenUpdate()));
	}

	private List<QuestEntryUI> quests;

	[SerializeField]
	public GameObject unseenItemIcon;

	[SerializeField]
	private ControllerFocusGrabber mainControllerFocusGrabber;

	[SerializeField]
	private Selectable defaultSelectable;
}
