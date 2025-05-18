using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class RepairAllButton : MonoBehaviour, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
{
	private void OnEnable()
	{
		this.localizedButtonText.enabled = false;
		this.EvaluateState();
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
		GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
		GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
	}

	private void OnPlayerFundsChanged(decimal total, decimal change)
	{
		this.EvaluateState();
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.selectable.interactable = false;
		this.tooltipRequester.enabled = false;
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.selectable.interactable = true;
		this.tooltipRequester.enabled = true;
		this.EvaluateState();
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		if (result)
		{
			this.selectable.interactable = true;
			this.tooltipRequester.enabled = true;
			this.EvaluateState();
		}
	}

	private void OnItemInventoryChanged(SpatialItemData itemData)
	{
		this.EvaluateState();
	}

	private void OnItemsRepaired()
	{
		this.EvaluateState();
	}

	private void EvaluateState()
	{
		this.repairAllCost = GameManager.Instance.ItemManager.GetRepairAllCost();
		this.localizedButtonText.StringReference.Arguments = new object[] { RepairActionHandler.GetFormattedCostString(this.repairAllCost) ?? "" };
		this.localizedButtonText.RefreshString();
		this.localizedButtonText.enabled = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		this.DoRepairAll();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		this.DoRepairAll();
	}

	private void DoRepairAll()
	{
		GameManager.Instance.ItemManager.RepairHullDamage(false);
		GameManager.Instance.ItemManager.RepairAllItemDurability();
	}

	[SerializeField]
	private TooltipRequester tooltipRequester;

	[SerializeField]
	private Selectable selectable;

	[SerializeField]
	private BasicButton basicButton;

	[SerializeField]
	private LocalizeStringEvent localizedButtonText;

	private decimal repairAllCost;
}
