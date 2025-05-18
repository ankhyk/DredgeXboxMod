using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfectionHelper : MonoBehaviour
{
	private void OnEnable()
	{
		this.gameTimeOfLastInfect = GameManager.Instance.SaveData.LastInfectTime;
	}

	private void Update()
	{
		float timeAndDay = GameManager.Instance.Time.TimeAndDay;
		if (timeAndDay - this.gameTimeOfLastInfect > GameManager.Instance.GameConfigData.ItemInfectionSpreadIntervalDays)
		{
			this.gameTimeOfLastInfect = timeAndDay;
			GameManager.Instance.SaveData.LastInfectTime = this.gameTimeOfLastInfect;
			this.TryInfect();
		}
	}

	private void TryInfect()
	{
		this.TryInfectGrid(GameManager.Instance.SaveData.Storage, true);
		this.TryInfectGrid(GameManager.Instance.SaveData.TrawlNet, true);
		int count = (from f in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH)
			where f.isInfected
			select f).ToList<FishItemInstance>().Count;
		if (this.TryInfectGrid(GameManager.Instance.SaveData.Inventory, true))
		{
			int count2 = (from f in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH)
				where f.isInfected
				select f).ToList<FishItemInstance>().Count;
			if (count == 0 && count2 > 0)
			{
				CustomDebug.EditorLogError("[InfectionCoroutine] TryInfect() infection somehow spread despite there being no infected present.");
				return;
			}
			if (count == 1 && count2 > 1)
			{
				GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.infection-spread-1", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
				return;
			}
			if (count > 1 && count2 > count)
			{
				GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.infection-spread-n", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
			}
		}
	}

	private bool TryInfectGrid(SerializableGrid grid, bool canBecomeAberration)
	{
		bool flag = false;
		int num = grid.GridConfiguration.columns - 1;
		int num2 = grid.GridConfiguration.rows - 1;
		List<FishItemInstance> list = (from i in grid.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH)
			where i.isInfected
			select i).ToList<FishItemInstance>();
		for (int m = list.Count - 1; m >= 0; m--)
		{
			FishItemInstance fishItemInstance = list[m];
			List<GridCellData> cellsAffectedByObjectAtPosition = grid.GetCellsAffectedByObjectAtPosition(fishItemInstance.GetItemData<FishItemData>().dimensions, fishItemInstance.GetPosition());
			for (int j = cellsAffectedByObjectAtPosition.Count - 1; j >= 0; j--)
			{
				GridCellData gridCellData = cellsAffectedByObjectAtPosition[j];
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						if (k != 0 || l != 0)
						{
							int num3 = gridCellData.x + k;
							int num4 = gridCellData.y + l;
							if (num3 >= 0 && num3 <= num && num4 >= 0 && num4 <= num2)
							{
								GridCellData gridCellData2 = grid.Grid[num3, num4];
								if (!cellsAffectedByObjectAtPosition.Contains(gridCellData2) && gridCellData2 != null && gridCellData2.spatialItemInstance != null && gridCellData2.spatialItemInstance is FishItemInstance && !(gridCellData2.spatialItemInstance as FishItemInstance).isInfected && (gridCellData2.spatialItemInstance as FishItemInstance).GetItemData<FishItemData>().CanBeInfected && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.ItemInfectionSpreadChance && GameManager.Instance.GridManager.InfectItem(gridCellData2.spatialItemInstance as FishItemInstance, grid, canBecomeAberration))
								{
									flag = true;
								}
							}
						}
					}
				}
			}
		}
		return flag;
	}

	private float gameTimeOfLastInfect;
}
