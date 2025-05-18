using System;
using UnityEngine;

public class GridCellData
{
	public bool ContainsObject
	{
		get
		{
			return this.spatialItemInstance != null;
		}
	}

	public bool IsHidden { get; set; }

	public bool DoesCellAcceptItem(ItemData itemData)
	{
		if (!(this.acceptedItemData == null))
		{
			return itemData.id == this.acceptedItemData.id;
		}
		if (this.acceptedItemType == ItemType.NONE)
		{
			return false;
		}
		if (itemData.itemSubtype == ItemSubtype.NONE)
		{
			return this.acceptedItemType.HasFlag(itemData.itemType);
		}
		return this.acceptedItemSubtype.HasFlag(itemData.itemSubtype);
	}

	public bool CanDamageThisCell()
	{
		bool flag = true;
		if (flag && this.spatialItemInstance != null && this.spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.NONE)
		{
			flag = false;
		}
		if (flag && this.underlaySpatialItemInstance != null && this.underlaySpatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.NONE)
		{
			flag = false;
		}
		if (flag && this.acceptedItemType == ItemType.NONE)
		{
			flag = false;
		}
		if (flag && this.damageImmune)
		{
			flag = false;
		}
		if (flag && this.acceptedItemType.HasFlag(ItemType.EQUIPMENT) && !GameManager.Instance.SaveData.availableDestinations.Contains(GameManager.Instance.GridManager.ShipwrightDestinationKey))
		{
			Debug.LogWarning("[GridCellData] CanDamageThisCell() can't damage as cell that holds equipment before unlocking the shipwright.");
			flag = false;
		}
		return flag;
	}

	public int x;

	public int y;

	public bool damageImmune;

	public SpatialItemInstance spatialItemInstance;

	public SpatialItemInstance underlaySpatialItemInstance;

	public ItemType acceptedItemType = ItemType.GENERAL;

	public ItemSubtype acceptedItemSubtype;

	public ItemData acceptedItemData;
}
