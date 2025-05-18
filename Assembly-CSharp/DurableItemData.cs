using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DurableItemData", menuName = "Dredge/DurableItemData", order = 0)]
public class DurableItemData : SpatialItemData
{
	public float MaxDurabilityDays
	{
		get
		{
			return this.maxDurabilityDays;
		}
	}

	[SerializeField]
	private float maxDurabilityDays;

	[SerializeField]
	public bool displayDurabilityAsPercentage;
}
