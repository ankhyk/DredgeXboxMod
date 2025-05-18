using System;
using System.Collections.Generic;

public struct GridObjectPlacementResult
{
	public List<GridObject> objects;

	public List<GridCell> cells;

	public bool placementUnobstructed;

	public bool placementCellsValid;

	public bool placementCellsAcceptObject;

	public bool placementCellsAreUndamaged;

	public bool placementCellsAreInStorageTray;
}
