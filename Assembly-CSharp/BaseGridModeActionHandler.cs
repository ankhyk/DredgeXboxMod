using System;

public abstract class BaseGridModeActionHandler
{
	public BaseGridModeActionHandler()
	{
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemHoveredChanged += this.OnItemHoveredChanged;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnFocusedGridCellChanged += this.OnFocusedGridCellChanged;
		GameEvents.Instance.OnItemInventoryChanged += this.OnItemInventoryChanged;
	}

	protected virtual void OnItemPickedUp(GridObject gridObject)
	{
	}

	protected virtual void OnCanBePlacedChanged(GridObject gridObject, bool canBePlaced)
	{
	}

	protected virtual void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
	}

	protected virtual void OnItemRemovedFromCursor(GridObject gridObject)
	{
	}

	protected virtual void OnItemHoveredChanged(GridObject gridObject)
	{
	}

	protected virtual void OnFocusedGridCellChanged(GridCell gridCell)
	{
	}

	protected virtual void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
	}

	public virtual void Shutdown()
	{
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemHoveredChanged -= this.OnItemHoveredChanged;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnFocusedGridCellChanged -= this.OnFocusedGridCellChanged;
		GameEvents.Instance.OnItemInventoryChanged -= this.OnItemInventoryChanged;
	}

	public GridMode mode;

	public Action OnPickUpPressedCallback;

	public Action OnPlacePressedCallback;

	public Action<float> OnRotatePressedCallback;
}
