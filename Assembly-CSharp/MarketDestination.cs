using System;
using System.Collections.Generic;
using UnityEngine;

public class MarketDestination : BaseDestination
{
	public ItemType ItemTypesBought
	{
		get
		{
			return this.itemTypesBought;
		}
	}

	public ItemSubtype ItemSubtypesBought
	{
		get
		{
			return this.itemSubtypesBought;
		}
	}

	public ItemType BulkItemTypesBought
	{
		get
		{
			return this.bulkItemTypesBought;
		}
	}

	public ItemSubtype BulkItemSubtypesBought
	{
		get
		{
			return this.bulkItemSubtypesBought;
		}
	}

	public SpatialItemData[] SpecificItemsBought
	{
		get
		{
			return this.specificItemsBought;
		}
	}

	public float SellValueModifier
	{
		get
		{
			return this.sellValueModifier;
		}
	}

	public bool AllowSellIfGridFull
	{
		get
		{
			return this.allowSellIfGridFull;
		}
	}

	public bool AllowStorageAccess
	{
		get
		{
			return this.allowStorageAccess;
		}
	}

	public bool AllowRepairs
	{
		get
		{
			return this.allowRepairs;
		}
	}

	public bool AllowBulkSell
	{
		get
		{
			return this.allowBulkSell;
		}
	}

	public string BulkSellPromptString
	{
		get
		{
			return this.bulkSellPromptString;
		}
	}

	public string BulkSellNotificationString
	{
		get
		{
			return this.bulkSellNotificationString;
		}
	}

	public GridKey GetGridKeyForItemType(ItemType itemType, ItemSubtype itemSubtype)
	{
		MarketTabConfig marketTabConfig = this.marketTabs.Find(delegate(MarketTabConfig config)
		{
			GridConfiguration gridConfigForKey = GameManager.Instance.GameConfigData.GetGridConfigForKey(config.gridKey);
			return gridConfigForKey != null && gridConfigForKey.MainItemType.HasFlag(itemType) && gridConfigForKey.MainItemSubtype.HasFlag(itemSubtype);
		});
		if (marketTabConfig != null)
		{
			return marketTabConfig.gridKey;
		}
		return GridKey.NONE;
	}

	[SerializeField]
	private ItemType itemTypesBought;

	[SerializeField]
	private ItemSubtype itemSubtypesBought;

	[SerializeField]
	private ItemType bulkItemTypesBought;

	[SerializeField]
	private ItemSubtype bulkItemSubtypesBought;

	[SerializeField]
	private SpatialItemData[] specificItemsBought;

	[SerializeField]
	private float sellValueModifier;

	[SerializeField]
	private bool allowSellIfGridFull;

	[SerializeField]
	private bool allowStorageAccess;

	[SerializeField]
	private bool allowRepairs;

	[SerializeField]
	private bool allowBulkSell;

	[SerializeField]
	private string bulkSellPromptString;

	[SerializeField]
	private string bulkSellNotificationString;

	public List<MarketTabConfig> marketTabs = new List<MarketTabConfig>();
}
