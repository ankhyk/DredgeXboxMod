using System;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetControlEntryUI : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IPointerClickHandler
{
	public BasicButtonWrapper ButtonWrapper
	{
		get
		{
			return this.buttonWrapper;
		}
	}

	public PlayerAction PlayerAction { get; set; }

	public bool Rebindable { get; set; }

	public void Init(PlayerAction playerAction, bool rebindable)
	{
		this.Rebindable = rebindable;
		this.PlayerAction = playerAction;
		this.unrebindableTooltipRequester.enabled = !rebindable;
		if (!this.Rebindable)
		{
			this.resetButtonImage.sprite = this.nonRebindableSprite;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action<ResetControlEntryUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(this);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action<ResetControlEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted == null)
		{
			return;
		}
		onEntrySubmitted(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Action<ResetControlEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted == null)
		{
			return;
		}
		onEntrySubmitted(this);
	}

	[SerializeField]
	private BasicButtonWrapper buttonWrapper;

	[SerializeField]
	private Sprite nonRebindableSprite;

	[SerializeField]
	private TextTooltipRequester unrebindableTooltipRequester;

	public Action<ResetControlEntryUI> OnEntrySelected;

	public Action<ResetControlEntryUI> OnEntrySubmitted;

	[SerializeField]
	private Image resetButtonImage;
}
