using System;
using UnityEngine;
using UnityEngine.UI;

public class MapHarvestPOIMarker : SelectableMapMarker
{
	public HarvestPOI HarvestPOI
	{
		get
		{
			return this.harvestPOI;
		}
	}

	public void Init(HarvestPOI harvestPOI)
	{
		HarvestableItemData firstHarvestableItem = harvestPOI.Harvestable.GetFirstHarvestableItem();
		this.markerImage.sprite = firstHarvestableItem.sprite;
		this.harvestPOI = harvestPOI;
		this.id = harvestPOI.Harvestable.GetId();
		Color color;
		if (this.harvestTypeTagConfig.colorLookup.TryGetValue(firstHarvestableItem.harvestableType, out color))
		{
			this.outline.effectColor = color;
			this.outline.enabled = true;
		}
	}

	public override void RemoveMarkerFromData()
	{
		GameManager.Instance.SaveData.RemoveHarvestPOIMarker(this.id);
	}

	private HarvestPOI harvestPOI;

	[SerializeField]
	private Outline outline;

	[SerializeField]
	private HarvestTypeTagConfig harvestTypeTagConfig;

	private string id;
}
