using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreshnessCoroutine : MonoBehaviour
{
	private void OnEnable()
	{
		this.coroutine = base.StartCoroutine(this.ItemFreshnessLoop(this.secondsBetweenChecks));
	}

	private void OnDisable()
	{
		if (this.coroutine != null)
		{
			base.StopCoroutine(this.coroutine);
			this.coroutine = null;
		}
	}

	private IEnumerator ItemFreshnessLoop(float secondsBetweenUpdates)
	{
		while (GameManager.Instance.IsPlaying)
		{
			float prevGameTime = GameManager.Instance.Time.TimeAndDay;
			yield return new WaitForSeconds(secondsBetweenUpdates);
			this.AdjustItemFreshness(GameManager.Instance.Time.TimeAndDay - prevGameTime);
		}
		yield break;
	}

	private void AdjustItemFreshness(float proportionOfDayJustElapsed)
	{
		if (proportionOfDayJustElapsed > 0f)
		{
			this.AdjustFreshnessForGrid(GameManager.Instance.SaveData.Inventory, proportionOfDayJustElapsed);
			this.AdjustFreshnessForGrid(GameManager.Instance.SaveData.Storage, proportionOfDayJustElapsed);
		}
	}

	private void AdjustFreshnessForGrid(SerializableGrid grid, float proportionOfDayJustElapsed)
	{
		List<SpatialItemInstance> list = grid.spatialItems.Where((SpatialItemInstance i) => i.GetItemData<SpatialItemData>().id.Contains("ice-block")).ToList<SpatialItemInstance>();
		int cooledCells = 0;
		list.ForEach(delegate(SpatialItemInstance i)
		{
			cooledCells += i.GetItemData<SpatialItemData>().dimensions.Count;
		});
		float coolingChange;
		float fishChange;
		if (cooledCells == 0)
		{
			fishChange = proportionOfDayJustElapsed * GameManager.Instance.GameConfigData.FreshnessLossPerDay;
			coolingChange = 0f;
		}
		else
		{
			float num = Mathf.InverseLerp(0f, GameManager.Instance.GameConfigData.CellsForMaxFreshnessLossReduction, (float)cooledCells);
			float num2 = GameManager.Instance.GameConfigData.FreshnessLossReductionCurve.Evaluate(num) * GameManager.Instance.GameConfigData.MaxFreshnessLossReduction;
			float num3 = 1f - num2;
			coolingChange = proportionOfDayJustElapsed * num3;
			fishChange = coolingChange * GameManager.Instance.GameConfigData.FreshnessLossPerDay;
		}
		list.ForEach(delegate(SpatialItemInstance i)
		{
			i.durability -= coolingChange;
		});
		if (list.Any((SpatialItemInstance i) => i.durability <= 0f))
		{
			list.Where((SpatialItemInstance i) => i.durability <= 0f).ToList<SpatialItemInstance>().ForEach(delegate(SpatialItemInstance i)
			{
				grid.RemoveObjectFromGridData(i, true);
			});
		}
		FishItemData fishItemData;
		grid.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).ForEach(delegate(FishItemInstance itemInstance)
		{
			fishItemData = itemInstance.GetItemData<FishItemData>();
			itemInstance.freshness = Mathf.Max(itemInstance.freshness - fishChange * fishItemData.RotCoefficient, 0f);
			if (itemInstance.freshness <= 0f)
			{
				GameManager.Instance.ItemManager.ReplaceFishWithRot(itemInstance, grid, false);
			}
		});
	}

	[SerializeField]
	private float secondsBetweenChecks;

	private List<FishItemInstance> fish = new List<FishItemInstance>();

	private Coroutine coroutine;
}
