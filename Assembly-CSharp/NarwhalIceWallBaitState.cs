using System;
using System.Collections.Generic;
using UnityEngine;

public class NarwhalIceWallBaitState : MonoBehaviour
{
	private void OnEnable()
	{
		this.grid = GameManager.Instance.SaveData.GetGridByKey(this.linkedGridKey);
		SerializableGrid serializableGrid = this.grid;
		serializableGrid.OnContentsUpdated = (Action)Delegate.Combine(serializableGrid.OnContentsUpdated, new Action(this.OnContentsUpdated));
		this.OnContentsUpdated();
	}

	private void OnDisable()
	{
		SerializableGrid serializableGrid = this.grid;
		serializableGrid.OnContentsUpdated = (Action)Delegate.Remove(serializableGrid.OnContentsUpdated, new Action(this.OnContentsUpdated));
	}

	private void OnContentsUpdated()
	{
		List<SpatialItemInstance> allItemsOfType = this.grid.GetAllItemsOfType<SpatialItemInstance>(ItemType.GENERAL, ItemSubtype.FISH);
		int numCellsOfFishInGrid = 0;
		allItemsOfType.ForEach(delegate(SpatialItemInstance i)
		{
			numCellsOfFishInGrid += i.GetItemData<SpatialItemData>().dimensions.Count;
		});
		int num = 0;
		for (int j = 0; j < this.cellCounts.Count; j++)
		{
			if (numCellsOfFishInGrid >= this.cellCounts[j])
			{
				num = j;
			}
		}
		this.gameObjects.ForEach(delegate(GameObject o)
		{
			o.SetActive(false);
		});
		this.gameObjects[num].SetActive(true);
	}

	[SerializeField]
	private GridKey linkedGridKey;

	[SerializeField]
	private List<int> cellCounts = new List<int>();

	[SerializeField]
	private List<GameObject> gameObjects = new List<GameObject>();

	private SerializableGrid grid;
}
