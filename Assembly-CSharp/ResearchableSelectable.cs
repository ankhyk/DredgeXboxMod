using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResearchableSelectable : UISelectable
{
	public bool CanBeResearched { get; set; }

	public Action promptCompleteAction { get; set; }

	public override void OnSelect(BaseEventData eventData)
	{
		if (this.CanBeResearched)
		{
			GameManager.Instance.UI.ResearchWindow.ShowResearchPrompt(this.promptCompleteAction, this.actionLayer);
		}
		else
		{
			GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		}
		base.OnSelect(eventData);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (this.CanBeResearched)
		{
			GameManager.Instance.UI.ResearchWindow.ShowResearchPrompt(this.promptCompleteAction, this.actionLayer);
		}
		else
		{
			GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		}
		base.OnPointerEnter(eventData);
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		base.OnDeselect(eventData);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		base.OnPointerExit(eventData);
	}

	protected override void OnDestroy()
	{
		GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		base.OnDestroy();
	}

	protected override void OnDisable()
	{
		GameManager.Instance.UI.ResearchWindow.HideResearchPrompt(this.actionLayer);
		base.OnDisable();
	}

	[SerializeField]
	private ActionLayer actionLayer = ActionLayer.POPUP_WINDOW;
}
