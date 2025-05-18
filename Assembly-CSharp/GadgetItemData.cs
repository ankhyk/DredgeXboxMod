using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GadgetItemData", menuName = "Dredge/GadgetItemData", order = 0)]
public class GadgetItemData : SpatialItemData
{
	public decimal EffectMagnitude
	{
		get
		{
			return this.effectMagnitude;
		}
	}

	public GadgetEffect EffectType
	{
		get
		{
			return this.effectType;
		}
	}

	public GadgetItemData()
	{
		this.itemSubtype = ItemSubtype.GADGET;
	}

	[SerializeField]
	private decimal effectMagnitude;

	[SerializeField]
	private GadgetEffect effectType;
}
