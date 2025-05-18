using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestZoneDetector : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		HarvestZone component = other.gameObject.GetComponent<HarvestZone>();
		if (component && this.currentHarvestZones.IndexOf(component) == -1)
		{
			this.currentHarvestZones.Add(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		HarvestZone component = other.gameObject.GetComponent<HarvestZone>();
		this.currentHarvestZones.Remove(component);
	}

	public List<string> GetHarvestableItemIds(Func<HarvestableItemData, bool> func, float currentDepth, bool isDay)
	{
		this.harvestableItemIds.Clear();
		this.currentHarvestZones.ForEach(delegate(HarvestZone zone)
		{
			for (int i = 0; i < zone.HarvestableItems.Length; i++)
			{
				if ((func == null || func(zone.HarvestableItems[i])) && (!isDay || zone.Day) && (isDay || zone.Night) && zone.HarvestableItems[i].CanCatchByDepth(currentDepth, GameManager.Instance.GameConfigData) && !this.harvestableItemIds.Contains(zone.HarvestableItems[i].id))
				{
					this.harvestableItemIds.Add(zone.HarvestableItems[i].id);
				}
			}
		});
		return this.harvestableItemIds;
	}

	public List<HarvestZone> currentHarvestZones = new List<HarvestZone>();

	private List<string> harvestableItemIds = new List<string>();
}
