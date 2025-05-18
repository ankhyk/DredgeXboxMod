using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableGrid
{
	public GridCellData[,] Grid
	{
		get
		{
			return this.grid;
		}
	}

	public GridConfiguration GridConfiguration
	{
		get
		{
			return this.gridConfiguration;
		}
	}

	public int CellCount
	{
		get
		{
			return this.cellCount;
		}
	}

	public void Init(GridConfiguration gridConfiguration, bool clear = false)
	{
		this.gridConfiguration = gridConfiguration;
		if (clear)
		{
			this.Clear(false);
		}
		this.grid = new GridCellData[gridConfiguration.columns, gridConfiguration.rows];
		for (int i = 0; i < gridConfiguration.columns; i++)
		{
			for (int j = 0; j < gridConfiguration.rows; j++)
			{
				GridCellData gridCellData = new GridCellData
				{
					x = i,
					y = j
				};
				this.grid[i, j] = gridCellData;
				gridCellData.acceptedItemType = gridConfiguration.MainItemType;
				gridCellData.acceptedItemSubtype = gridConfiguration.MainItemSubtype;
				gridCellData.acceptedItemData = gridConfiguration.MainItemData;
				this.cellCount++;
			}
		}
		gridConfiguration.cellGroupConfigs.ForEach(new Action<CellGroupConfiguration>(this.ApplyCellGroupConfig));
		this.spatialItems.ForEach(delegate(SpatialItemInstance entry)
		{
			Vector3Int vector3Int = new Vector3Int(entry.x, entry.y, entry.z);
			if (entry.GetItemData<SpatialItemData>() != null)
			{
				this.GetCellsAffectedByObjectAtPosition(entry.GetItemData<SpatialItemData>().dimensions, vector3Int).ForEach(delegate(GridCellData gc)
				{
					gc.spatialItemInstance = entry;
				});
			}
		});
		if (this.spatialUnderlayItems != null)
		{
			this.spatialUnderlayItems.ForEach(delegate(SpatialItemInstance entry)
			{
				Vector3Int vector3Int2 = new Vector3Int(entry.x, entry.y, entry.z);
				if (entry.GetItemData<SpatialItemData>() != null)
				{
					this.GetCellsAffectedByObjectAtPosition(entry.GetItemData<SpatialItemData>().dimensions, vector3Int2).ForEach(delegate(GridCellData gc)
					{
						gc.underlaySpatialItemInstance = entry;
						if (gc.spatialItemInstance != null && entry.GetItemData<SpatialItemData>().itemType == ItemType.DAMAGE)
						{
							gc.spatialItemInstance.SetIsOnDamagedCell(true);
						}
					});
				}
			});
		}
		this.TriggerRefreshEvent();
	}

	public SpatialItemInstance GetItemAtPosition(Vector3Int pos)
	{
		return this.grid[pos.x, pos.y].spatialItemInstance;
	}

	public int GetFilledCells(ItemSubtype itemSubtype)
	{
		int filledCells = 0;
		this.spatialItems.ForEach(delegate(SpatialItemInstance item)
		{
			if (item.x < this.grid.GetLength(0) && item.y < this.grid.GetLength(1) && !this.grid[item.x, item.y].IsHidden && (itemSubtype == ItemSubtype.ALL || item.GetItemData<SpatialItemData>().itemSubtype.HasFlag(itemSubtype)))
			{
				filledCells += item.GetItemData<SpatialItemData>().GetSize();
			}
		});
		return filledCells;
	}

	public int GetFilledCells(ItemSubtype itemSubtype, ItemSubtype excludeType)
	{
		int filledCells = 0;
		this.spatialItems.ForEach(delegate(SpatialItemInstance item)
		{
			if (item.x < this.grid.GetLength(0) && item.y < this.grid.GetLength(1))
			{
				SpatialItemData itemData = item.GetItemData<SpatialItemData>();
				bool flag = itemData.itemSubtype.HasFlag(itemSubtype);
				bool flag2 = excludeType != ItemSubtype.NONE && itemData.itemSubtype.HasFlag(excludeType);
				if ((itemSubtype == ItemSubtype.ALL || flag) && !flag2)
				{
					filledCells += item.GetItemData<SpatialItemData>().GetSize();
				}
			}
		});
		return filledCells;
	}

	public int GetFilledCells(ItemType itemType)
	{
		int filledCells = 0;
		this.spatialItems.ForEach(delegate(SpatialItemInstance item)
		{
			if (!this.grid[item.x, item.y].IsHidden && (itemType == ItemType.ALL || item.GetItemData<SpatialItemData>().itemType.HasFlag(itemType)))
			{
				filledCells += item.GetItemData<SpatialItemData>().GetSize();
			}
		});
		return filledCells;
	}

	public float GetFillProportional(ItemSubtype itemSubtype)
	{
		return (float)this.GetFilledCells(itemSubtype) / (float)this.GetCountNonHiddenCells();
	}

	public float GetFillProportional(ItemSubtype itemSubtype, ItemSubtype excludeType)
	{
		return (float)this.GetFilledCells(itemSubtype, excludeType) / (float)this.GetCountNonHiddenCells();
	}

	public int GetCountNonHiddenCells()
	{
		int num = 0;
		for (int i = 0; i < this.gridConfiguration.rows; i++)
		{
			for (int j = 0; j < this.gridConfiguration.columns; j++)
			{
				if (this.grid[j, i] != null && !this.grid[j, i].IsHidden)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetCountCellsAcceptingType(ItemType itemType)
	{
		int num = 0;
		for (int i = 0; i < this.gridConfiguration.rows; i++)
		{
			for (int j = 0; j < this.gridConfiguration.columns; j++)
			{
				if (this.grid[j, i] != null && !this.grid[j, i].IsHidden && this.grid[j, i].acceptedItemType.HasFlag(itemType))
				{
					num++;
				}
			}
		}
		return num;
	}

	public void ApplyCellGroupConfig(CellGroupConfiguration cgc)
	{
		cgc.cells.ForEach(delegate(Vector2Int coord)
		{
			GridCellData gridCellData = this.grid[coord.x, coord.y];
			if (gridCellData != null)
			{
				gridCellData.acceptedItemType = cgc.ItemType;
				gridCellData.acceptedItemSubtype = cgc.ItemSubtype;
				gridCellData.IsHidden = cgc.IsHidden;
				gridCellData.damageImmune = cgc.DamageImmune;
			}
		});
	}

	public void TriggerRefreshEvent()
	{
		Action onGridRefreshed = this.OnGridRefreshed;
		if (onGridRefreshed == null)
		{
			return;
		}
		onGridRefreshed();
	}

	public void Clear(bool reInit)
	{
		if (this.spatialItems != null)
		{
			this.spatialItems.Clear();
		}
		if (this.spatialUnderlayItems != null)
		{
			this.spatialUnderlayItems.Clear();
		}
		for (int i = 0; i < this.grid.GetLength(1); i++)
		{
			for (int j = 0; j < this.grid.GetLength(0); j++)
			{
				if (this.grid[j, i] != null)
				{
					this.grid[j, i].spatialItemInstance = null;
				}
			}
		}
		if (reInit)
		{
			this.Init(this.gridConfiguration, false);
		}
	}

	public bool FindSpaceAndAddObjectToGridData(SpatialItemData itemData, bool dispatchEvent, Action<GridObject> PostCreateAction = null)
	{
		Vector3Int zero = Vector3Int.zero;
		bool flag = this.FindPositionForObject(itemData, out zero, 0, true);
		if (flag)
		{
			SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemData);
			this.AddObjectToGridData(spatialItemInstance, zero, dispatchEvent, PostCreateAction);
		}
		return flag;
	}

	public bool FindSpaceAndAddObjectToGridData(SpatialItemData itemData, bool dispatchEvent, out SpatialItemInstance spatialItemInstance, Action<GridObject> PostCreateAction = null)
	{
		Vector3Int zero = Vector3Int.zero;
		spatialItemInstance = null;
		bool flag = this.FindPositionForObject(itemData, out zero, 0, true);
		if (flag)
		{
			spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemData);
			this.AddObjectToGridData(spatialItemInstance, zero, dispatchEvent, PostCreateAction);
		}
		return flag;
	}

	public bool FindRandomSpaceAndAddObjectToGridData(SpatialItemData itemData, bool dispatchEvent, Action<GridObject> PostCreateAction = null)
	{
		Vector3Int zero = Vector3Int.zero;
		bool flag = this.FindRandomPositionForObject(itemData, out zero);
		if (flag)
		{
			SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemData);
			this.AddObjectToGridData(spatialItemInstance, zero, dispatchEvent, PostCreateAction);
		}
		return flag;
	}

	public void AddObjectToGridData(SpatialItemInstance spatialItemInstance, Vector3Int pos, bool dispatchEvent, Action<GridObject> PostCreateAction = null)
	{
		SpatialItemData spatialItemData = spatialItemInstance.GetItemData<SpatialItemData>();
		if (spatialItemData == null)
		{
			return;
		}
		this.GetCellsAffectedByObjectAtPosition(spatialItemData.dimensions, pos).ForEach(delegate(GridCellData gc)
		{
			if (spatialItemData.isUnderlayItem)
			{
				gc.underlaySpatialItemInstance = spatialItemInstance;
				return;
			}
			gc.spatialItemInstance = spatialItemInstance;
		});
		spatialItemInstance.x = pos.x;
		spatialItemInstance.y = pos.y;
		spatialItemInstance.z = pos.z;
		if (spatialItemData.isUnderlayItem)
		{
			this.spatialUnderlayItems.Add(spatialItemInstance);
		}
		else
		{
			this.spatialItems.Add(spatialItemInstance);
		}
		if (dispatchEvent)
		{
			Action<SpatialItemInstance, Action<GridObject>> onItemAdded = this.OnItemAdded;
			if (onItemAdded != null)
			{
				onItemAdded(spatialItemInstance, PostCreateAction);
			}
			GameEvents.Instance.TriggerItemAddedEvent(spatialItemInstance, this.gridConfiguration.ItemsInThisBelongToPlayer);
		}
		Action onContentsUpdated = this.OnContentsUpdated;
		if (onContentsUpdated == null)
		{
			return;
		}
		onContentsUpdated();
	}

	public void ForceTriggerItemAddEvent(SpatialItemInstance spatialItemInstance)
	{
		Action<SpatialItemInstance, Action<GridObject>> onItemAdded = this.OnItemAdded;
		if (onItemAdded != null)
		{
			onItemAdded(spatialItemInstance, null);
		}
		GameEvents.Instance.TriggerItemAddedEvent(spatialItemInstance, this.gridConfiguration.ItemsInThisBelongToPlayer);
	}

	public void RemoveObjectFromGridData(SpatialItemInstance spatialItemInstanceToRemove, bool notify)
	{
		Vector3Int vector3Int = new Vector3Int(spatialItemInstanceToRemove.x, spatialItemInstanceToRemove.y, spatialItemInstanceToRemove.z);
		if (spatialItemInstanceToRemove.GetItemData<SpatialItemData>().isUnderlayItem)
		{
			this.GetCellsAffectedByObjectAtPosition(spatialItemInstanceToRemove.GetItemData<SpatialItemData>().dimensions, vector3Int).ForEach(delegate(GridCellData gcd)
			{
				gcd.underlaySpatialItemInstance = null;
			});
			this.spatialUnderlayItems.Remove(spatialItemInstanceToRemove);
		}
		else
		{
			this.GetCellsAffectedByObjectAtPosition(spatialItemInstanceToRemove.GetItemData<SpatialItemData>().dimensions, vector3Int).ForEach(delegate(GridCellData gcd)
			{
				gcd.spatialItemInstance = null;
			});
			this.spatialItems.Remove(spatialItemInstanceToRemove);
		}
		if (notify)
		{
			Action<SpatialItemInstance> onItemRemoved = this.OnItemRemoved;
			if (onItemRemoved != null)
			{
				onItemRemoved(spatialItemInstanceToRemove);
			}
		}
		Action onContentsUpdated = this.OnContentsUpdated;
		if (onContentsUpdated == null)
		{
			return;
		}
		onContentsUpdated();
	}

	public GridCellData GetRootCellForObject(SpatialItemInstance spatialItemInstanceToRemove)
	{
		new Vector3Int(spatialItemInstanceToRemove.x, spatialItemInstanceToRemove.y, spatialItemInstanceToRemove.z);
		return this.grid[spatialItemInstanceToRemove.x, spatialItemInstanceToRemove.y];
	}

	public bool FindRandomPositionForObject(SpatialItemData spatialItemData, out Vector3Int foundPosition)
	{
		foundPosition = default(Vector3Int);
		if (spatialItemData == null)
		{
			return false;
		}
		GridCellData gridCellData = null;
		int num = 0;
		int num2 = 100;
		int columns = this.gridConfiguration.columns;
		int rows = this.gridConfiguration.rows;
		int num3 = 0;
		List<int> list = new List<int> { 0, 90, 180, 270 };
		while (gridCellData == null && num < num2)
		{
			int i = global::UnityEngine.Random.Range(0, columns);
			int j = global::UnityEngine.Random.Range(0, rows);
			num3 = list.PickRandom<int>();
			this.grid[i, j];
			if (this.grid[i, j] != null)
			{
				if (this.TestPos(spatialItemData, i, j, num3, out foundPosition))
				{
					return true;
				}
				num++;
			}
		}
		if (gridCellData == null)
		{
			for (int i = 0; i < this.gridConfiguration.columns; i++)
			{
				for (int j = 0; j < this.gridConfiguration.rows; j++)
				{
					if (this.grid[i, j] != null && this.TestPos(spatialItemData, i, j, num3, out foundPosition))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool FindPositionForObject(SpatialItemData spatialItemData, out Vector3Int foundPosition, int startZ = 0, bool prioritizeRotation = false)
	{
		foundPosition = default(Vector3Int);
		if (spatialItemData == null)
		{
			return false;
		}
		if (prioritizeRotation)
		{
			int num = startZ;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < this.gridConfiguration.rows; j++)
				{
					for (int k = 0; k < this.gridConfiguration.columns; k++)
					{
						if (this.grid[k, j] != null && this.TestPos(spatialItemData, k, j, num, out foundPosition))
						{
							return true;
						}
					}
				}
				num = (num + 90) % 360;
			}
		}
		else
		{
			for (int l = 0; l < this.gridConfiguration.rows; l++)
			{
				for (int m = 0; m < this.gridConfiguration.columns; m++)
				{
					if (this.grid[m, l] != null)
					{
						int num = startZ;
						for (int n = 0; n < 4; n++)
						{
							if (this.TestPos(spatialItemData, m, l, num, out foundPosition))
							{
								return true;
							}
							num = (num + 90) % 360;
						}
					}
				}
			}
		}
		return false;
	}

	public bool TestPos(SpatialItemData spatialItemData, int x, int y, int z, out Vector3Int foundPosition)
	{
		Vector3Int vector3Int = new Vector3Int(x, y, z);
		foundPosition = default(Vector3Int);
		List<GridCellData> cellsAffectedByObjectAtPosition = this.GetCellsAffectedByObjectAtPosition(spatialItemData.dimensions, vector3Int);
		bool isValid = true;
		if (cellsAffectedByObjectAtPosition.Count != spatialItemData.GetSize())
		{
			isValid = false;
		}
		cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gcd)
		{
			if (gcd == null)
			{
				isValid = false;
				return;
			}
			if (gcd.ContainsObject)
			{
				isValid = false;
				return;
			}
			if (gcd.underlaySpatialItemInstance != null && gcd.underlaySpatialItemInstance.GetItemData<SpatialItemData>().itemType == ItemType.DAMAGE)
			{
				isValid = false;
				return;
			}
			if (!gcd.DoesCellAcceptItem(spatialItemData))
			{
				isValid = false;
				return;
			}
		});
		if (isValid)
		{
			foundPosition = vector3Int;
			return true;
		}
		return false;
	}

	public List<GridCellData> GetCellsAffectedByObjectAtPosition(List<Vector2Int> dimensions, Vector3Int pos)
	{
		if (this.cellsAffectedByQuery == null)
		{
			this.cellsAffectedByQuery = new List<GridCellData>();
		}
		else
		{
			this.cellsAffectedByQuery.Clear();
		}
		dimensions.ForEach(delegate(Vector2Int offset)
		{
			int num = pos.x + offset.x;
			int num2 = pos.y + offset.y;
			if (pos.z == 90)
			{
				num = pos.x + offset.y;
				num2 = pos.y - offset.x;
			}
			else if (pos.z == 180)
			{
				num = pos.x - offset.x;
				num2 = pos.y - offset.y;
			}
			else if (pos.z == 270)
			{
				num = pos.x - offset.y;
				num2 = pos.y + offset.x;
			}
			if (num >= 0 && num < this.gridConfiguration.columns && num2 >= 0 && num2 < this.gridConfiguration.rows && this.grid[num, num2] != null)
			{
				this.cellsAffectedByQuery.Add(this.grid[num, num2]);
			}
		});
		return this.cellsAffectedByQuery;
	}

	public List<T> GetAllItemsOfType<T>(ItemType itemTypes, ItemSubtype itemSubtypes = ItemSubtype.NONE) where T : SpatialItemInstance
	{
		List<T> items = new List<T>();
		this.spatialItems.ForEach(delegate(SpatialItemInstance spatialItemInstance)
		{
			SpatialItemData itemData = spatialItemInstance.GetItemData<SpatialItemData>();
			if (itemSubtypes == ItemSubtype.NONE)
			{
				if (itemData.itemType.HasFlag(itemTypes))
				{
					items.Add((T)((object)spatialItemInstance));
					return;
				}
			}
			else if (itemData.itemType.HasFlag(itemTypes) && itemData.itemSubtype.HasFlag(itemSubtypes))
			{
				items.Add((T)((object)spatialItemInstance));
			}
		});
		return items;
	}

	[SerializeField]
	public List<SpatialItemInstance> spatialUnderlayItems = new List<SpatialItemInstance>();

	[SerializeField]
	public List<SpatialItemInstance> spatialItems = new List<SpatialItemInstance>();

	[NonSerialized]
	private GridCellData[,] grid;

	[NonSerialized]
	private GridConfiguration gridConfiguration;

	[NonSerialized]
	public Action<SpatialItemInstance, Action<GridObject>> OnItemAdded;

	[NonSerialized]
	public Action<SpatialItemInstance> OnItemRemoved;

	[NonSerialized]
	public Action OnContentsUpdated;

	[NonSerialized]
	public Action OnGridRefreshed;

	[NonSerialized]
	private List<GridCellData> cellsAffectedByQuery = new List<GridCellData>();

	[NonSerialized]
	private int cellCount;
}
