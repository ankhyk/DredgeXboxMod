using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GridConfiguration", menuName = "Dredge/GridConfiguration", order = 0)]
public class GridConfiguration : SerializedScriptableObject
{
	public ItemType MainItemType
	{
		get
		{
			return this.mainItemType;
		}
		set
		{
			this.mainItemType = value;
		}
	}

	public ItemSubtype MainItemSubtype
	{
		get
		{
			return this.mainItemSubtype;
		}
		set
		{
			this.mainItemSubtype = value;
		}
	}

	public ItemData MainItemData
	{
		get
		{
			return this.mainItemData;
		}
	}

	public bool ItemsInThisBelongToPlayer
	{
		get
		{
			return this.itemsInThisBelongToPlayer;
		}
		set
		{
			this.itemsInThisBelongToPlayer = value;
		}
	}

	public bool CanAddItemsInQuestMode
	{
		get
		{
			return this.canAddItemsInQuestMode;
		}
		set
		{
			this.canAddItemsInQuestMode = value;
		}
	}

	public bool HasUnderlay
	{
		get
		{
			return this.hasUnderlay;
		}
	}

	public int GetSize()
	{
		return this.columns * this.rows;
	}

	[SerializeField]
	public List<CellGroupConfiguration> cellGroupConfigs = new List<CellGroupConfiguration>();

	[SerializeField]
	private ItemType mainItemType;

	[SerializeField]
	private ItemSubtype mainItemSubtype;

	[SerializeField]
	private ItemData mainItemData;

	[SerializeField]
	private bool itemsInThisBelongToPlayer;

	[SerializeField]
	private bool canAddItemsInQuestMode;

	[SerializeField]
	[Tooltip("Does this grid have an underlay (used for damage and reinforcement)")]
	private bool hasUnderlay;

	public int columns;

	public int rows;
}
