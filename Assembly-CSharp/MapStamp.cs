using System;
using UnityEngine;

public class MapStamp : SelectableMapMarker
{
	public SerializedMapStamp StampData
	{
		get
		{
			return this.stampData;
		}
	}

	public void Init(Sprite sprite, Color color, SerializedMapStamp stampData)
	{
		this.markerImage.sprite = sprite;
		this.markerImage.color = color;
		this.stampData = stampData;
	}

	public override void RemoveMarkerFromData()
	{
		GameManager.Instance.SaveData.mapStamps.Remove(this.stampData);
	}

	private SerializedMapStamp stampData;
}
