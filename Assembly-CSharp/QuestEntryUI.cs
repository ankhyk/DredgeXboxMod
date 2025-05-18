using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
	public QuestData QuestData
	{
		get
		{
			return this.questData;
		}
	}

	public BasicButtonWrapper ButtonWrapper
	{
		get
		{
			return this.buttonWrapper;
		}
	}

	public void Init(JournalWindow journalWindow)
	{
		this.journalWindow = journalWindow;
		BasicButtonWrapper basicButtonWrapper = this.buttonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(delegate
		{
			this.journalWindow.OnQuestEntryClicked(this);
		}));
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged += this.RefreshUI;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.RefreshUI;
	}

	public bool GetHasUnseenUpdate()
	{
		bool flag = false;
		if (this.entry != null)
		{
			flag = this.entry.hasUnseenUpdate;
		}
		return flag;
	}

	public void RefreshUI()
	{
		this.questState = GameManager.Instance.QuestManager.GetQuestStateById(this.questData.name);
		this.entry = GameManager.Instance.QuestManager.GetQuestEntryById(this.questData.name);
		if (this.questState == QuestState.STARTED || this.questState == QuestState.COMPLETED)
		{
			this.container.SetActive(true);
			this.unseenIcon.SetActive(this.entry.hasUnseenUpdate);
			this.localizedTitleStringField.StringReference = this.questData.titleKey;
			this.localizedTitleStringField.RefreshString();
			this.localizedActiveStepStringField.StringReference = GameManager.Instance.QuestManager.GetShortActiveStepKeyByQuestId(this.questData.name);
			this.localizedActiveStepStringField.RefreshString();
			this.completedTickImage.enabled = this.questState == QuestState.COMPLETED;
			if (this.questState == QuestState.COMPLETED)
			{
				this.headerBackplateImage.color = this.completedHeaderBackplateColor;
				this.activeStepBackplateImage.color = this.completedActiveStepBackplateColor;
				this.activeStepStringField.color = this.completedTextColor;
				this.titleStringField.color = this.completedTextColor;
			}
			return;
		}
		this.container.SetActive(false);
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private QuestData questData;

	[SerializeField]
	private LocalizeStringEvent localizedTitleStringField;

	[SerializeField]
	private LocalizeStringEvent localizedActiveStepStringField;

	[SerializeField]
	private TextMeshProUGUI titleStringField;

	[SerializeField]
	private TextMeshProUGUI activeStepStringField;

	[SerializeField]
	private BasicButtonWrapper buttonWrapper;

	[SerializeField]
	private Image headerBackplateImage;

	[SerializeField]
	private Image activeStepBackplateImage;

	[SerializeField]
	private Image completedTickImage;

	[SerializeField]
	private GameObject unseenIcon;

	[Header("Config")]
	[SerializeField]
	private Color completedHeaderBackplateColor;

	[SerializeField]
	private Color completedActiveStepBackplateColor;

	[SerializeField]
	private Color completedTextColor;

	private JournalWindow journalWindow;

	private string activeStepId;

	private QuestState questState;

	private SerializedQuestEntry entry;
}
