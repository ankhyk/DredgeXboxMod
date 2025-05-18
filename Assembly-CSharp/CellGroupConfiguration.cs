using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CellGroupConfiguration
{
	public ItemType ItemType
	{
		get
		{
			return this.itemType;
		}
	}

	public ItemSubtype ItemSubtype
	{
		get
		{
			return this.itemSubtype;
		}
	}

	public bool IsHidden
	{
		get
		{
			return this.isHidden;
		}
	}

	public bool DamageImmune
	{
		get
		{
			return this.damageImmune;
		}
	}

	[SerializeField]
	public List<Vector2Int> cells;

	[SerializeField]
	private ItemType itemType;

	[SerializeField]
	private ItemSubtype itemSubtype;

	[SerializeField]
	private bool isHidden;

	[SerializeField]
	private bool damageImmune;
}
