using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class NonSpatialGridEntryUI : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISelectHandler, IPointerClickHandler, IDeselectHandler, IPointerEnterHandler
{
	public Button Button
	{
		get
		{
			return this.button;
		}
	}

	public NonSpatialItemInstance ItemInstance
	{
		get
		{
			return this.itemInstance;
		}
	}

	public virtual void Init(NonSpatialItemInstance itemInstance)
	{
		this.itemInstance = itemInstance;
		this.tooltipRequester.NonSpatialItemInstance = itemInstance;
		this.unseenItemIcon.gameObject.SetActive(itemInstance.isNew);
		if (this.itemInstance.GetItemData<NonSpatialItemData>())
		{
			this.InitUI();
			this.RefreshUI();
		}
	}

	protected virtual void InitUI()
	{
		this.localizedNameTextField.StringReference = this.itemInstance.GetItemData<NonSpatialItemData>().itemNameKey;
		this.localizedNameTextField.RefreshString();
		this.localizedNameTextField.enabled = true;
	}

	public virtual void RefreshUI()
	{
		this.unseenItemIcon.gameObject.SetActive(this.itemInstance.isNew);
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action<NonSpatialGridEntryUI> onEntrySelected = this.OnEntrySelected;
		if (onEntrySelected != null)
		{
			onEntrySelected(this);
		}
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.itemInstance);
		this.RefreshUI();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action<NonSpatialGridEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted != null)
		{
			onEntrySubmitted(this);
		}
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.itemInstance);
		this.RefreshUI();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Action<NonSpatialGridEntryUI> onEntrySubmitted = this.OnEntrySubmitted;
		if (onEntrySubmitted != null)
		{
			onEntrySubmitted(this);
		}
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.itemInstance);
		this.RefreshUI();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.itemInstance);
		this.RefreshUI();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.Instance.ItemManager.MarkNonSpatialItemAsSeen(this.itemInstance);
		this.RefreshUI();
	}

	[SerializeField]
	private NonSpatialItemTooltipRequester tooltipRequester;

	[SerializeField]
	protected Image unseenItemIcon;

	[SerializeField]
	private Button button;

	[SerializeField]
	protected LocalizeStringEvent localizedNameTextField;

	[SerializeField]
	protected LocalizeStringEvent localizedDescriptionTextField;

	protected NonSpatialItemInstance itemInstance;

	public Action<NonSpatialGridEntryUI> OnEntrySelected;

	public Action<NonSpatialGridEntryUI> OnEntrySubmitted;
}
