using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShopRestocker : SerializedMonoBehaviour
{
	private void Awake()
	{
		this.itemIdsToKeep = this.itemsToKeepInStock.Select((SpatialItemData data) => data.id).ToList<string>();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnDayChanged += this.OnDayChanged;
		GameEvents.Instance.OnResearchCompleted += new Action<SpatialItemData>(this.OnResearchCompleted);
		GameEvents.Instance.OnShopRestockRequested += this.OnShopRestockRequested;
		this.shopDataGridConfigs.ForEach(delegate(ShopRestocker.ShopDataGridConfig config)
		{
			if (GameManager.Instance.SaveData.GetGridByKey(config.gridKey).spatialItems.Count == 0)
			{
				this.dueForRefresh = true;
			}
		});
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnDayChanged -= this.OnDayChanged;
		GameEvents.Instance.OnResearchCompleted -= new Action<SpatialItemData>(this.OnResearchCompleted);
		GameEvents.Instance.OnShopRestockRequested -= this.OnShopRestockRequested;
	}

	private void OnDayChanged(int day)
	{
		this.dueForRefresh = true;
		this.timeUntilTryRestock = 0f;
	}

	private void OnResearchCompleted(ItemData itemData)
	{
		this.OnShopRestockRequested();
	}

	public void OnShopRestockRequested()
	{
		this.dueForRefresh = true;
		this.timeUntilTryRestock = 0f;
	}

	private void Update()
	{
		if (this.dueForRefresh)
		{
			this.timeUntilTryRestock -= Time.deltaTime;
			if (this.timeUntilTryRestock < 0f)
			{
				this.TryRefreshShops();
			}
		}
	}

	private void TryRefreshShops()
	{
		if (GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.MARKET) || GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.SHIPYARD))
		{
			this.timeUntilTryRestock = 5f;
			return;
		}
		this.shopDataGridConfigs.ForEach(new Action<ShopRestocker.ShopDataGridConfig>(this.TryRefreshShop));
		this.dueForRefresh = false;
	}

	private void TryRefreshShop(ShopRestocker.ShopDataGridConfig config)
	{
		SerializableGrid grid = GameManager.Instance.SaveData.GetGridByKey(config.gridKey);
		IEnumerable<SpatialItemData> newStock = config.shopData.GetNewStock();
		List<SpatialItemInstance> itemsToRemove = new List<SpatialItemInstance>();
		grid.spatialItems.ForEach(delegate(SpatialItemInstance item)
		{
			if (!this.itemIdsToKeep.Contains(item.id))
			{
				itemsToRemove.Add(item);
			}
		});
		itemsToRemove.ForEach(delegate(SpatialItemInstance item)
		{
			grid.RemoveObjectFromGridData(item, false);
		});
		newStock.OrderBy(delegate(SpatialItemData x)
		{
			if (x.itemSubtype == ItemSubtype.ROD)
			{
				return 1;
			}
			if (x.itemSubtype == ItemSubtype.ENGINE)
			{
				return 2;
			}
			if (x.itemSubtype != ItemSubtype.NET)
			{
				return 4;
			}
			return 3;
		}).ToList<SpatialItemData>().ForEach(delegate(SpatialItemData newItem)
		{
			this.didFindPosition = grid.FindPositionForObject(newItem, out this.position, 0, true);
			if (this.didFindPosition)
			{
				SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(newItem);
				grid.AddObjectToGridData(spatialItemInstance, this.position, false, null);
			}
		});
	}

	[SerializeField]
	private List<ShopRestocker.ShopDataGridConfig> shopDataGridConfigs = new List<ShopRestocker.ShopDataGridConfig>();

	[SerializeField]
	private List<SpatialItemData> itemsToKeepInStock = new List<SpatialItemData>();

	private List<string> itemIdsToKeep;

	private bool dueForRefresh;

	private float timeUntilTryRestock;

	private Vector3Int position = Vector3Int.zero;

	private bool didFindPosition;

	private class ShopDataGridConfig
	{
		public ShopData shopData;

		public GridKey gridKey;
	}
}
