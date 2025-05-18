using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SpeakerButton : MonoBehaviour
{
	public BasicButtonWrapper BasicButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	public void Init(SpeakerData speakerData, string yarnRootNode, bool highlightThis)
	{
		this.speakerData = speakerData;
		this.yarnRootNode = yarnRootNode;
		this.questAttentionCallout.SetActive(highlightThis);
		Sprite sprite = speakerData.smallPortraitSprite;
		if (speakerData.portraitOverrideConditions.Count > 0)
		{
			PortraitOverride portraitOverride = speakerData.portraitOverrideConditions.Find((PortraitOverride po) => po.nodesVisited.All((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n)));
			if (portraitOverride.smallPortraitSprite)
			{
				sprite = portraitOverride.smallPortraitSprite;
			}
		}
		this.speakerPortraitImage.sprite = sprite;
		this.localizedSpeakerNameField.StringReference.SetReference(LanguageManager.CHARACTER_TABLE, speakerData.speakerNameKey);
	}

	private void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	private void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	public void OnClick()
	{
		Action<string, SpeakerData> onSubmitAction = this.OnSubmitAction;
		if (onSubmitAction == null)
		{
			return;
		}
		onSubmitAction(this.yarnRootNode, this.speakerData);
	}

	[SerializeField]
	private LocalizeStringEvent localizedSpeakerNameField;

	[SerializeField]
	private Image speakerPortraitImage;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private GameObject questAttentionCallout;

	private SpeakerData speakerData;

	private string yarnRootNode;

	public Action<string, SpeakerData> OnSubmitAction;
}
