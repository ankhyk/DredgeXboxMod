using System;
using UnityEngine;

public class ItemPOIHandler : MonoBehaviour
{
	public bool IsHandlerActive { get; set; }

	public ItemPOI ItemPOI
	{
		get
		{
			return this.itemPOI;
		}
		set
		{
			this.itemPOI = value;
		}
	}

	private void Awake()
	{
		this.collectItemAction = new DredgePlayerActionPress("prompt.collect-item", GameManager.Instance.Input.Controls.Interact);
		DredgePlayerActionPress dredgePlayerActionPress = this.collectItemAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
		this.collectItemAction.showInControlArea = true;
		this.collectItemAction.allowPreholding = true;
	}

	private void OnDestroy()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.collectItemAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressComplete));
	}

	public void Activate(ItemPOI itemPOI)
	{
		this.IsHandlerActive = true;
		this.itemPOI = itemPOI;
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.collectItemAction };
		input.AddActionListener(array, ActionLayer.BASE);
	}

	public void Deactivate()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.collectItemAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		this.itemPOI = null;
		this.IsHandlerActive = false;
	}

	private void OnPressComplete()
	{
		GameEvents.Instance.TriggerPlayerInteractedWithPOI();
		GameManager.Instance.ItemManager.AddItemById(this.itemPOI.Harvestable.GetFirstItem().id, GameManager.Instance.SaveData.Inventory, true);
		this.itemPOI.OnHarvested(true);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.collectItemAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
	}

	private ItemPOI itemPOI;

	private DredgePlayerActionPress collectItemAction;
}
