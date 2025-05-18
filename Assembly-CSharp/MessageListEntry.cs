using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MessageListEntry : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IPointerClickHandler
{
	public Button Button
	{
		get
		{
			return this.button;
		}
	}

	public MessageItemData MessageItemData
	{
		get
		{
			return this.messageItemData;
		}
		set
		{
			this.messageItemData = value;
		}
	}

	public void OnEnable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged += this.RefreshUI;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.RefreshUI;
	}

	public void Init(NonSpatialItemInstance itemInstance)
	{
		this.messageItemInstance = itemInstance;
		this.messageItemData = GameManager.Instance.ItemManager.GetItemDataById<MessageItemData>(itemInstance.id);
		this.unseenImage.SetActive(itemInstance.isNew);
		this.localizedTitleField.StringReference = this.messageItemData.itemNameKey;
		this.localizedTitleField.StringReference.RefreshString();
		this.localizedDescriptionField.StringReference = this.messageItemData.messageBodyKey;
		this.localizedDescriptionField.StringReference.RefreshString();
	}

	private void RefreshUI()
	{
		this.unseenImage.SetActive(this.messageItemInstance != null && this.messageItemInstance.isNew);
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action<MessageListEntry> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected == null)
		{
			return;
		}
		onEntrySelected(this);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action<MessageListEntry> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted != null)
		{
			onEntrySubmitted(this);
		}
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.messageItemInstance);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Action<MessageListEntry> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted != null)
		{
			onEntrySubmitted(this);
		}
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.messageItemInstance);
	}

	[SerializeField]
	private LocalizeStringEvent localizedTitleField;

	[SerializeField]
	private LocalizeStringEvent localizedDescriptionField;

	[SerializeField]
	private GameObject unseenImage;

	[SerializeField]
	private Button button;

	[Header("Config")]
	public Action<MessageListEntry> OnEntrySelected;

	public Action<MessageListEntry> OnEntrySubmitted;

	private NonSpatialItemInstance messageItemInstance;

	private MessageItemData messageItemData;
}
