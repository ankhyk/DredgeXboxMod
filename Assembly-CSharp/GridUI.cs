using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridUI : MonoBehaviour
{
	public GridKey LinkedGridKey
	{
		get
		{
			return this.gridKey;
		}
		set
		{
			this.gridKey = value;
		}
	}

	public GridObjectState DefaultGridObjectState
	{
		get
		{
			return this.defaultGridObjectState;
		}
	}

	public GridConfiguration GridConfiguration
	{
		get
		{
			return this.gridConfiguration;
		}
	}

	public bool OverrideGridCellColor
	{
		get
		{
			return this.overrideGridCellColor;
		}
		set
		{
			this.overrideGridCellColor = value;
		}
	}

	public DredgeColorTypeEnum GridCellColor
	{
		get
		{
			return this.gridCellColor;
		}
		set
		{
			this.gridCellColor = value;
		}
	}

	public ScreenSide ScreenSide
	{
		get
		{
			return this.screenSide;
		}
	}

	private void OnEnable()
	{
		this.Init();
		GameManager.Instance.GridManager.AddActiveGrid(this);
		if (this.presetGridMode != PresetGridMode.NONE)
		{
			this.ShowGridHints(this.hintItems, this.presetGridMode);
		}
	}

	private void OnDisable()
	{
		GameManager.Instance.GridManager.RemoveActiveGrid(this);
		if (this.allGridObjects.Contains(GameManager.Instance.GridManager.GetCurrentlyFocusedObject()))
		{
			GameEvents.Instance.TriggerItemHoveredChanged(null);
		}
		GameManager.Instance.GridManager.ReportGridShowing(this, false);
		this.Clear();
	}

	public void SetLinkedGrid(SerializableGrid grid)
	{
		this.linkedGrid = grid;
		this.retrieveLinkedGridOnInit = false;
		this.gridConfiguration = grid.GridConfiguration;
	}

	public void DisableInput()
	{
		this.canvasGroupDisabler.Disable();
	}

	public void EnableInput()
	{
		this.canvasGroupDisabler.Enable();
	}

	public void Init()
	{
		if (this.retrieveLinkedGridOnInit)
		{
			if (this.gridKey == GridKey.NONE)
			{
				Debug.LogWarning("[GridUI: " + base.name + "] Init() cannot initialize a grid with no gridKey or linkedGrid");
				return;
			}
			this.linkedGrid = GameManager.Instance.SaveData.GetGridByKey(this.gridKey);
		}
		GameManager.Instance.GridManager.ReportGridShowing(this, true);
		SerializableGrid serializableGrid = this.linkedGrid;
		serializableGrid.OnItemAdded = (Action<SpatialItemInstance, Action<GridObject>>)Delegate.Combine(serializableGrid.OnItemAdded, new Action<SpatialItemInstance, Action<GridObject>>(this.CreateObject));
		SerializableGrid serializableGrid2 = this.linkedGrid;
		serializableGrid2.OnItemRemoved = (Action<SpatialItemInstance>)Delegate.Combine(serializableGrid2.OnItemRemoved, new Action<SpatialItemInstance>(this.OnItemRemovedFromData));
		SerializableGrid serializableGrid3 = this.linkedGrid;
		serializableGrid3.OnGridRefreshed = (Action)Delegate.Combine(serializableGrid3.OnGridRefreshed, new Action(this.OnGridRefreshed));
		this.GenerateGrid();
		if (this.LinkedGridKey == GridKey.INVENTORY)
		{
			GameEvents.Instance.OnUpgradePreviewed += this.OnUpgradePreviewed;
			if (GameManager.Instance.GridManager.CurrentUpgradePreviewData != null)
			{
				this.OnUpgradePreviewed(GameManager.Instance.GridManager.CurrentUpgradePreviewData);
			}
		}
	}

	private void OnGridRefreshed()
	{
		this.Clear();
		this.Init();
	}

	public void OnUpgradePreviewed(UpgradeData upgradeData)
	{
		if (this.allCells != null)
		{
			for (int i = 0; i < this.allCells.GetLength(0); i++)
			{
				for (int j = 0; j < this.allCells.GetLength(1); j++)
				{
					this.allCells[i, j].ToggleUpgradePreviewImage(false, ItemSubtype.NONE);
				}
			}
		}
		if (upgradeData && upgradeData is SlotUpgradeData && upgradeData.tier == GameManager.Instance.SaveData.HullTier)
		{
			GridCell gridCell;
			(upgradeData as SlotUpgradeData).CellGroupConfigs.ForEach(delegate(CellGroupConfiguration cgc)
			{
				cgc.cells.ForEach(delegate(Vector2Int pos)
				{
					gridCell = this.allCells[pos.x, pos.y];
					if (gridCell)
					{
						gridCell.ToggleUpgradePreviewImage(true, cgc.ItemSubtype);
					}
				});
			});
		}
	}

	public void Clear()
	{
		SerializableGrid serializableGrid = this.linkedGrid;
		serializableGrid.OnItemAdded = (Action<SpatialItemInstance, Action<GridObject>>)Delegate.Remove(serializableGrid.OnItemAdded, new Action<SpatialItemInstance, Action<GridObject>>(this.CreateObject));
		SerializableGrid serializableGrid2 = this.linkedGrid;
		serializableGrid2.OnItemRemoved = (Action<SpatialItemInstance>)Delegate.Remove(serializableGrid2.OnItemRemoved, new Action<SpatialItemInstance>(this.OnItemRemovedFromData));
		SerializableGrid serializableGrid3 = this.linkedGrid;
		serializableGrid3.OnGridRefreshed = (Action)Delegate.Remove(serializableGrid3.OnGridRefreshed, new Action(this.OnGridRefreshed));
		if (this.LinkedGridKey == GridKey.INVENTORY)
		{
			GameEvents.Instance.OnUpgradePreviewed -= this.OnUpgradePreviewed;
		}
		if (this.allCells != null)
		{
			for (int i = 0; i < this.allCells.GetLength(0); i++)
			{
				for (int j = 0; j < this.allCells.GetLength(1); j++)
				{
					this.allCells[i, j].gameObject.Recycle();
				}
			}
		}
		this.allCells = null;
		foreach (GridObject gridObject in this.allGridObjects)
		{
			gridObject.gameObject.Recycle();
		}
		foreach (GridObject gridObject2 in this.allHintObjects)
		{
			gridObject2.gameObject.Recycle();
		}
		this.allGridObjects.Clear();
		this.allHintObjects.Clear();
	}

	private void GenerateGrid()
	{
		this.underlayContainer.gameObject.SetActive(this.linkedGrid.GridConfiguration.HasUnderlay);
		this.cellSize = GameManager.Instance.GridManager.cellSize;
		this.allCells = new GridCell[this.linkedGrid.GridConfiguration.columns, this.linkedGrid.GridConfiguration.rows];
		int num = 0;
		if (this.LinkedGridKey == GridKey.INVENTORY)
		{
			num = -1;
		}
		RectTransform component = base.GetComponent<RectTransform>();
		this.originPos = new Vector3(component.rect.width * 0.5f - (float)this.linkedGrid.GridConfiguration.columns * 0.5f * this.cellSize, -(component.rect.height * 0.5f) + ((float)this.linkedGrid.GridConfiguration.rows + (float)num) * 0.5f * this.cellSize, 0f);
		Color color = Color.white;
		if (this.overrideGridCellColor)
		{
			color = GameManager.Instance.LanguageManager.GetColor(this.gridCellColor);
		}
		GridCellData[,] grid = this.linkedGrid.Grid;
		int upperBound = grid.GetUpperBound(0);
		int upperBound2 = grid.GetUpperBound(1);
		for (int i = grid.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = grid.GetLowerBound(1); j <= upperBound2; j++)
			{
				GridCellData gridCellData = grid[i, j];
				GameObject gameObject = this.gridCellPrefab.Spawn(this.cellContainer);
				(gameObject.transform as RectTransform).anchoredPosition = new Vector2(this.originPos.x + (float)gridCellData.x * this.cellSize, this.originPos.y - (float)gridCellData.y * this.cellSize);
				(gameObject.transform as RectTransform).sizeDelta = new Vector2(this.cellSize, this.cellSize);
				GridCell component2 = gameObject.GetComponent<GridCell>();
				this.allCells[gridCellData.x, gridCellData.y] = component2;
				if (this.overrideGridCellColor)
				{
					component2.regularColor = color;
				}
				component2.Init(gridCellData, string.Format("[{0}, {1}]", gridCellData.x, gridCellData.y), this, this.defaultGridObjectState, gridCellData.acceptedItemType, gridCellData.acceptedItemSubtype);
			}
		}
		this.CreateObjects();
	}

	private void CreateObjects()
	{
		foreach (SpatialItemInstance spatialItemInstance in this.linkedGrid.spatialItems)
		{
			this.CreateObject(spatialItemInstance, null);
		}
		if (this.linkedGrid.GridConfiguration.HasUnderlay)
		{
			foreach (SpatialItemInstance spatialItemInstance2 in this.linkedGrid.spatialUnderlayItems)
			{
				this.CreateObject(spatialItemInstance2, null);
			}
		}
	}

	public void CreateObject(SpatialItemInstance entry, Action<GridObject> PostCreateAction = null)
	{
		SpatialItemData itemData = entry.GetItemData<SpatialItemData>();
		Vector3Int vector3Int = new Vector3Int(entry.x, entry.y, entry.z);
		GridCellData gridCellData = this.GetCellAtPos(vector3Int.x, vector3Int.y).GridCellData;
		bool flag = !itemData.dimensions.Contains(Vector2Int.zero);
		if (gridCellData.IsHidden && !flag)
		{
			return;
		}
		GameObject gameObject = this.gridObjectPrefab.Spawn(base.transform);
		gameObject.name = itemData.name;
		GridObject component = gameObject.GetComponent<GridObject>();
		component.Init(entry, this, this.defaultGridObjectState);
		this.PlaceObjectOnGrid(component, vector3Int, false, true);
		if (PostCreateAction != null)
		{
			PostCreateAction(component);
		}
	}

	private void OnItemRemovedFromData(SpatialItemInstance entry)
	{
		GridObject gridObject = this.allGridObjects.Find((GridObject go) => go.SpatialItemInstance == entry);
		if (gridObject)
		{
			this.RemoveObjectFromListAtCoord(gridObject, false);
			if (this.currentlyPickedUpObject == gridObject)
			{
				this.currentlyPickedUpObject = null;
			}
			GameManager.Instance.GridManager.OnItemRemoved(gridObject);
			this.allGridObjects.Remove(gridObject);
			global::UnityEngine.Object.Destroy(gridObject.gameObject);
			return;
		}
		Debug.LogWarning(string.Format("[GridUI: {0}] OnItemRemovedFromData({1}) couldn't find gridObject with matching entry", base.name, entry));
	}

	public void PlaceObjectOnGrid(GridObject o, Vector3Int pos, bool needsSaving, bool instant)
	{
		Transform transform = this.objectContainer;
		bool isUnderlayItem = o.ItemData.isUnderlayItem;
		if (isUnderlayItem)
		{
			transform = this.underlayContainer;
		}
		o.transform.SetParent(transform, true);
		if (needsSaving)
		{
			this.linkedGrid.AddObjectToGridData(o.SpatialItemInstance, pos, false, null);
		}
		if (this.linkedGrid.GridConfiguration.ItemsInThisBelongToPlayer)
		{
			GameManager.Instance.ItemManager.SetItemSeen(o.SpatialItemInstance);
		}
		o.SetRootCoord(pos);
		o.ParentGrid = this;
		o.CurrentRotation = pos.z;
		List<GridCellData> cellsAffectedByObjectAtPosition = this.linkedGrid.GetCellsAffectedByObjectAtPosition(o.ItemData.dimensions, pos);
		cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
		{
			if (isUnderlayItem)
			{
				this.allCells[gc.x, gc.y].OccupyingUnderlayObject = o;
				return;
			}
			this.allCells[gc.x, gc.y].SetOccupyingObject(o);
		});
		int minCellX = 999;
		int minCellY = 999;
		int maxCellX = 0;
		int maxCellY = 0;
		cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
		{
			if (gc.x < minCellX)
			{
				minCellX = gc.x;
			}
			if (gc.y < minCellY)
			{
				minCellY = gc.y;
			}
			if (gc.x > maxCellX)
			{
				maxCellX = gc.x;
			}
			if (gc.y > maxCellY)
			{
				maxCellY = gc.y;
			}
		});
		float num = (float)minCellX + (float)(maxCellX - minCellX) * 0.5f;
		float num2 = (float)minCellY + (float)(maxCellY - minCellY) * 0.5f;
		RectTransform rectTransform = o.transform as RectTransform;
		Vector2 vector = new Vector2((float)o.ItemData.GetWidth() * this.cellSize, (float)o.ItemData.GetHeight() * this.cellSize);
		rectTransform.sizeDelta = vector;
		rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
		o.SetRotation((float)pos.z, instant);
		float num3 = this.originPos.x + num * this.cellSize + this.cellSize * 0.5f;
		float num4 = -this.originPos.y + num2 * this.cellSize + this.cellSize * 0.5f;
		o.SetPosition(new Vector2(num3, -num4), instant);
		o.transform.SetParent(transform, false);
		o.state = this.defaultGridObjectState;
		this.allGridObjects.Add(o);
		this.CheckHintImages();
	}

	public void ClearGridHints()
	{
		this.hintItems = null;
		this.presetGridMode = PresetGridMode.NONE;
	}

	public void ShowGridHints(List<SpatialItemInstance> hintItems, PresetGridMode presetGridMode)
	{
		this.hintItems = hintItems;
		this.presetGridMode = presetGridMode;
		foreach (SpatialItemInstance spatialItemInstance in hintItems)
		{
			SpatialItemData itemData = spatialItemInstance.GetItemData<SpatialItemData>();
			Vector3Int vector3Int = new Vector3Int(spatialItemInstance.x, spatialItemInstance.y, spatialItemInstance.z);
			GameObject gameObject = this.gridObjectPrefab.Spawn(base.transform);
			gameObject.name = itemData.name;
			GridObject hintObject = gameObject.GetComponent<GridObject>();
			if (presetGridMode == PresetGridMode.SILHOUETTE)
			{
				hintObject.Init(spatialItemInstance, this, GridObjectState.IS_HINT);
			}
			else if (presetGridMode == PresetGridMode.MYSTERY)
			{
				hintObject.Init(spatialItemInstance, this, GridObjectState.MYSTERY);
			}
			hintObject.SetHintMode(presetGridMode);
			Transform transform = this.hintObjectContainer;
			hintObject.transform.SetParent(transform, true);
			hintObject.SetRootCoord(vector3Int);
			hintObject.ParentGrid = this;
			hintObject.CurrentRotation = vector3Int.z;
			List<GridCellData> cellsAffectedByObjectAtPosition = this.linkedGrid.GetCellsAffectedByObjectAtPosition(itemData.dimensions, vector3Int);
			cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
			{
				this.allCells[gc.x, gc.y].SetOccupyingHintObject(hintObject);
			});
			int minCellX = 999;
			int minCellY = 999;
			int maxCellX = 0;
			int maxCellY = 0;
			cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
			{
				if (gc.x < minCellX)
				{
					minCellX = gc.x;
				}
				if (gc.y < minCellY)
				{
					minCellY = gc.y;
				}
				if (gc.x > maxCellX)
				{
					maxCellX = gc.x;
				}
				if (gc.y > maxCellY)
				{
					maxCellY = gc.y;
				}
			});
			float num = (float)minCellX + (float)(maxCellX - minCellX) * 0.5f;
			float num2 = (float)minCellY + (float)(maxCellY - minCellY) * 0.5f;
			RectTransform rectTransform = hintObject.transform as RectTransform;
			Vector2 vector = new Vector2((float)itemData.GetWidth() * this.cellSize, (float)itemData.GetHeight() * this.cellSize);
			rectTransform.sizeDelta = vector;
			rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
			hintObject.SetRotation((float)vector3Int.z, true);
			float num3 = this.originPos.x + num * this.cellSize + this.cellSize * 0.5f;
			float num4 = -this.originPos.y + num2 * this.cellSize + this.cellSize * 0.5f;
			hintObject.SetPosition(new Vector2(num3, -num4), true);
			hintObject.transform.SetParent(transform, false);
			this.allHintObjects.Add(hintObject);
		}
		this.CheckHintImages();
	}

	private void CheckHintImages()
	{
		List<GridCellData> affectedCells;
		this.allHintObjects.ForEach(delegate(GridObject hintObject)
		{
			affectedCells = this.linkedGrid.GetCellsAffectedByObjectAtPosition(hintObject.ItemData.dimensions, hintObject.SpatialItemInstance.GetPosition());
			bool flag = affectedCells.Any((GridCellData c) => c.ContainsObject);
			hintObject.gameObject.SetActive(!flag);
		});
	}

	public void ClearAllCellEffects()
	{
		if (this.linkedGrid.GridConfiguration == null)
		{
			return;
		}
		for (int i = 0; i < this.linkedGrid.GridConfiguration.columns; i++)
		{
			for (int j = 0; j < this.linkedGrid.GridConfiguration.rows; j++)
			{
				if (this.allCells[i, j])
				{
					this.allCells[i, j].SetCellState(CellState.REGULAR);
				}
			}
		}
	}

	public void SelectFirstPlaceableCell()
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			if (GameManager.Instance.GridManager.GetIsGridShowing(this))
			{
				for (int i = 0; i < this.linkedGrid.GridConfiguration.columns; i++)
				{
					for (int j = 0; j < this.linkedGrid.GridConfiguration.rows; j++)
					{
						GridCell gridCell = this.allCells[i, j];
						if (gridCell && gridCell.GridCellData.acceptedItemType != ItemType.NONE)
						{
							EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
							gridCell.OnSelected();
							return;
						}
					}
				}
				return;
			}
		}
		else
		{
			GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(this.screenSide);
		}
	}

	public void SelectMostSuitableCellForObjectPlacement(SpatialItemInstance spatialItemInstance)
	{
		if (!base.isActiveAndEnabled)
		{
			Debug.LogWarning("[GridUI] SelectMostSuitableCellForObjectPlacement() handling case where grid is inactive.");
			return;
		}
		if (GameManager.Instance.Input.IsUsingController)
		{
			Vector2Int currentHeldObjectSelectionOffsetCoord = GameManager.Instance.GridManager.CurrentHeldObjectSelectionOffsetCoord;
			Vector3Int vector3Int;
			if (!this.linkedGrid.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out vector3Int, 0, false))
			{
				this.SelectFirstCellForItemType(spatialItemInstance.GetItemData<SpatialItemData>(), true, currentHeldObjectSelectionOffsetCoord);
				return;
			}
			Vector2Int vector2Int = new Vector2Int(Mathf.Clamp(vector3Int.x - currentHeldObjectSelectionOffsetCoord.x, 0, this.linkedGrid.GridConfiguration.columns - 1), Mathf.Clamp(vector3Int.y - currentHeldObjectSelectionOffsetCoord.y, 0, this.linkedGrid.GridConfiguration.rows - 1));
			GridCell cellAtPos = this.GetCellAtPos(vector2Int.x, vector2Int.y);
			if (cellAtPos)
			{
				EventSystem.current.SetSelectedGameObject(cellAtPos.gameObject);
				cellAtPos.OnSelected();
				return;
			}
		}
		else
		{
			GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(this.screenSide);
		}
	}

	public void SelectFirstCellForItemType(SpatialItemData itemData, bool preferEmpty, Vector2Int offset)
	{
		GridCell firstCellForItemType = this.GetFirstCellForItemType(itemData, preferEmpty);
		if (firstCellForItemType != null)
		{
			GridCell gridCell = this.allCells[firstCellForItemType.GridCellData.x - offset.x, firstCellForItemType.GridCellData.y - offset.y];
			if (gridCell)
			{
				EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
				gridCell.OnSelected();
			}
			return;
		}
	}

	public void SelectFirstCellContainingItemType(ItemType itemType)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			for (int i = 0; i < this.linkedGrid.GridConfiguration.columns; i++)
			{
				for (int j = 0; j < this.linkedGrid.GridConfiguration.rows; j++)
				{
					GridCell gridCell = this.allCells[i, j];
					if (gridCell && ((gridCell.OccupyingObject != null && gridCell.OccupyingObject.ItemData.itemType == itemType) || (gridCell.OccupyingUnderlayObject != null && gridCell.OccupyingUnderlayObject.ItemData.itemType == itemType)))
					{
						EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
						gridCell.OnSelected();
						return;
					}
				}
			}
			return;
		}
		GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(this.screenSide);
	}

	public GridCell GetCellAtPos(int x, int y)
	{
		return this.allCells[x, y];
	}

	public GridCell GetFirstCellForItemType(SpatialItemData itemData, bool preferEmpty)
	{
		GridCell gridCell = null;
		for (int i = 0; i < this.linkedGrid.GridConfiguration.columns; i++)
		{
			for (int j = 0; j < this.linkedGrid.GridConfiguration.rows; j++)
			{
				GridCell gridCell2 = this.allCells[i, j];
				if (gridCell2 && gridCell2.GridCellData.DoesCellAcceptItem(itemData))
				{
					if (gridCell == null)
					{
						gridCell = gridCell2;
					}
					if (!preferEmpty || (preferEmpty && gridCell2.OccupyingObject == null))
					{
						break;
					}
				}
			}
		}
		return gridCell;
	}

	public GridCell GetFirstEmptyCell(SpatialItemData itemData = null)
	{
		for (int i = 0; i < this.linkedGrid.GridConfiguration.columns; i++)
		{
			for (int j = 0; j < this.linkedGrid.GridConfiguration.rows; j++)
			{
				GridCell gridCell = this.allCells[i, j];
				if (gridCell && gridCell.OccupyingObject == null && (!(itemData != null) || gridCell.GridCellData.DoesCellAcceptItem(itemData)))
				{
					EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
					gridCell.OnSelected();
					return gridCell;
				}
			}
		}
		return null;
	}

	public void PickUpObject(GridObject objectPickedUp)
	{
		GameManager.Instance.GridManager.PendingHeldObject = null;
		this.currentlyPickedUpObject = objectPickedUp;
		this.currentlyPickedUpObject.SetIsPickedUp(true);
		objectPickedUp.transform.SetParent(GameManager.Instance.GridManager.heldItemContainer.transform, true);
		GameManager.Instance.GridManager.CurrentHeldObjectSelectionOffset = objectPickedUp.transform.position - GameManager.Instance.GridManager.CursorProxy.GetPosition();
		Vector2Int coord = GameManager.Instance.GridManager.LastSelectedCell.Coord;
		Vector2Int vector2Int = new Vector2Int(objectPickedUp.RootCoord.x - coord.x, objectPickedUp.RootCoord.y - coord.y);
		GameManager.Instance.GridManager.CurrentHeldObjectSelectionOffsetCoord = vector2Int;
		GameManager.Instance.GridManager.CursorProxy.MoveTo(objectPickedUp.transform.position);
		if (objectPickedUp.ParentGrid)
		{
			this.RemoveObjectFromListAtCoord(objectPickedUp, true);
		}
		objectPickedUp.ParentGrid = null;
		this.CheckHintImages();
		GameManager.Instance.GridManager.ObjectPickedUp(objectPickedUp);
	}

	public void TryPlaceObject(GridObject objectToPlace)
	{
		GridObjectPlacementResult placementResult = objectToPlace.GetPlacementResult();
		if (placementResult.placementUnobstructed && placementResult.placementCellsValid && placementResult.placementCellsAcceptObject && (placementResult.placementCellsAreUndamaged || objectToPlace.ItemData.ignoreDamageWhenPlacing))
		{
			this.currentlyPickedUpObject = objectToPlace;
			Vector3Int vector3Int;
			if (objectToPlace.GetCurrentRootPositionWithRotation(out vector3Int))
			{
				this.PlaceObjectOnGrid(objectToPlace, vector3Int, true, false);
			}
			else
			{
				CustomDebug.EditorLogError("[GridUI: {name}] TryPlaceObject() didn't find root cell.");
			}
			GameManager.Instance.GridManager.NotifyOfPlacementAttempt(objectToPlace, true);
			return;
		}
		if (placementResult.placementCellsAcceptObject && placementResult.placementCellsValid && placementResult.objects.Count == 1 && placementResult.objects[0].ItemData.GetCanBeMoved() && (placementResult.placementCellsAreUndamaged || objectToPlace.ItemData.ignoreDamageWhenPlacing))
		{
			this.currentlyPickedUpObject = objectToPlace;
			GridCell gridCell = placementResult.cells.Find((GridCell gc) => gc.OccupyingObject && gc.OccupyingObject != objectToPlace);
			if (gridCell)
			{
				GameManager.Instance.GridManager.LastSelectedCell = gridCell;
				EventSystem.current.SetSelectedGameObject(gridCell.gameObject, null);
			}
			GameManager.Instance.GridManager.PendingHeldObject = placementResult.objects[0];
			this.RemoveObjectFromListAtCoord(GameManager.Instance.GridManager.PendingHeldObject, true);
			GameManager.Instance.GridManager.PendingHeldObject.ParentGrid = null;
			Vector3Int vector3Int2;
			if (objectToPlace.GetCurrentRootPositionWithRotation(out vector3Int2))
			{
				this.PlaceObjectOnGrid(objectToPlace, vector3Int2, true, false);
			}
			else
			{
				CustomDebug.EditorLogError("[GridUI: {name}] TryPlaceObject() didn't find root cell.");
			}
			GameManager.Instance.GridManager.NotifyOfPlacementAttempt(objectToPlace, true);
			this.PickUpObject(GameManager.Instance.GridManager.PendingHeldObject);
			return;
		}
		string str = "";
		placementResult.cells.ForEach(delegate(GridCell c)
		{
			str = str + c.name + ", ";
		});
		GameManager.Instance.GridManager.NotifyOfPlacementAttempt(objectToPlace, false);
	}

	public void OnObjectPlaced(GridObject objectPlaced, bool result)
	{
		if (this.currentlyPickedUpObject == objectPlaced && result)
		{
			this.currentlyPickedUpObject.SetIsPickedUp(false);
			this.currentlyPickedUpObject = null;
		}
	}

	public void RemoveObjectFromListAtCoord(GridObject o, bool deleteFromData)
	{
		if (deleteFromData)
		{
			this.linkedGrid.RemoveObjectFromGridData(o.SpatialItemInstance, false);
		}
		this.allGridObjects.Remove(o);
		List<GridCellData> cellsAffectedByObjectAtPosition = this.linkedGrid.GetCellsAffectedByObjectAtPosition(o.ItemData.dimensions, o.RootCoord);
		if (o.ItemData.isUnderlayItem)
		{
			cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
			{
				this.allCells[gc.x, gc.y].OccupyingUnderlayObject = null;
			});
		}
		else
		{
			cellsAffectedByObjectAtPosition.ForEach(delegate(GridCellData gc)
			{
				this.allCells[gc.x, gc.y].SetOccupyingObject(null);
			});
		}
		this.CheckHintImages();
	}

	public GameObject gridObjectPrefab;

	public GameObject gridCellPrefab;

	[SerializeField]
	private GridObjectState defaultGridObjectState;

	[SerializeField]
	private GridKey gridKey;

	public SerializableGrid linkedGrid;

	private GridCell[,] allCells;

	[SerializeField]
	private Transform underlayContainer;

	[SerializeField]
	private Transform cellContainer;

	[SerializeField]
	private Transform hintObjectContainer;

	[SerializeField]
	private Transform objectContainer;

	[SerializeField]
	private CanvasGroup underlayCanvasGroup;

	[SerializeField]
	private CanvasGroup objectCanvasGroup;

	[SerializeField]
	private CanvasGroupDisabler canvasGroupDisabler;

	[SerializeField]
	private GridConfiguration gridConfiguration;

	[SerializeField]
	private ScreenSide screenSide;

	private bool overrideGridCellColor;

	private DredgeColorTypeEnum gridCellColor;

	private GridObject currentlyPickedUpObject;

	public List<GridObject> allGridObjects = new List<GridObject>();

	public List<GridObject> allHintObjects = new List<GridObject>();

	private float cellSize;

	private Vector2 originPos;

	private bool retrieveLinkedGridOnInit = true;

	[HideInInspector]
	public Action<bool> OnChangeActiveStatus;

	private List<SpatialItemInstance> hintItems;

	private PresetGridMode presetGridMode;
}
