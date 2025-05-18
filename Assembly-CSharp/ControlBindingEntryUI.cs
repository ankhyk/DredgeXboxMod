using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlBindingEntryUI : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public BindingSourceType BindingSourceType
	{
		get
		{
			return this.bindingSourceType;
		}
	}

	public PlayerAction PlayerAction { get; set; }

	public bool Rebindable { get; set; }

	public bool Unbindable { get; set; }

	public BasicButtonWrapper ButtonWrapper
	{
		get
		{
			return this.basicButtonWrapper;
		}
	}

	public void Init(PlayerAction playerAction, bool rebindable, bool unbindable)
	{
		this.Rebindable = rebindable;
		this.Unbindable = unbindable;
		this.PlayerAction = playerAction;
		ControlIconData controlIconForAction = GameManager.Instance.Input.GetControlIconForAction(playerAction, this.bindingSourceType, false);
		if (controlIconForAction != null)
		{
			this.bindingImage.gameObject.SetActive(true);
			this.backupTextField.gameObject.SetActive(false);
			this.bindingImage.sprite = controlIconForAction.upSprite;
			return;
		}
		this.bindingImage.gameObject.SetActive(false);
		this.backupTextField.gameObject.SetActive(true);
		this.backupTextField.text = GameManager.Instance.Input.GetControlStringForAction(playerAction, this.bindingSourceType);
	}

	public void Refresh()
	{
		this.Init(this.PlayerAction, this.Rebindable, this.Unbindable);
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action<ControlBindingEntryUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(this);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action<ControlBindingEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted == null)
		{
			return;
		}
		onEntrySubmitted(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Action<ControlBindingEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
			if (onEntrySubmitted == null)
			{
				return;
			}
			onEntrySubmitted(this);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Action<ControlBindingEntryUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		Action<ControlBindingEntryUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(null);
	}

	[SerializeField]
	private BindingSourceType bindingSourceType;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private TextMeshProUGUI backupTextField;

	[SerializeField]
	private Image bindingImage;

	public Action<ControlBindingEntryUI> OnEntrySelected;

	public Action<ControlBindingEntryUI> OnEntrySubmitted;
}
