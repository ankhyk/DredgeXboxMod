using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialStepItemCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
	}

	public override void Unsubscribe()
	{
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
		base.Unsubscribe();
	}

	private void OnPlayerDamageChanged()
	{
		this.Evaluate();
	}

	private void OnItemInventoryChanged(SpatialItemData itemData)
	{
		this.Evaluate();
	}

	public override void Evaluate()
	{
		bool flag = GameManager.Instance.SaveData.Inventory.spatialItems.Any((SpatialItemInstance i) => (this.allowItemsWithZeroDurability || i.durability > 0f) && this.items.Contains(i.GetItemData<SpatialItemData>()));
		base.SetConditionMet(flag);
	}

	[SerializeField]
	public List<ItemData> items;

	[SerializeField]
	public bool allowItemsWithZeroDurability;
}
