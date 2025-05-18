using System;
using UnityEngine;

[Serializable]
public class SpatialItemInstance : ItemInstance
{
	public bool GetIsOnDamagedCell()
	{
		return this.isOnDamagedCell;
	}

	public void SetIsOnDamagedCell(bool value)
	{
		if (this.isOnDamagedCell != value)
		{
			this.isOnDamagedCell = value;
			Action onDamagedStatusChanged = this.OnDamagedStatusChanged;
			if (onDamagedStatusChanged == null)
			{
				return;
			}
			onDamagedStatusChanged();
		}
	}

	public void ChangeDurability(float changeAmount)
	{
		if (this.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY)
		{
			this.durability += changeAmount;
			this.durability = Mathf.Clamp(this.durability, 0f, this.GetItemData<DeployableItemData>().MaxDurabilityDays);
		}
	}

	public float GetMissingDurabilityAmount()
	{
		if (this.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY)
		{
			return this.GetItemData<DeployableItemData>().MaxDurabilityDays - this.durability;
		}
		return 0f;
	}

	public void RepairToFullDurability()
	{
		if (this.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY)
		{
			float maxDurabilityDays = this.GetItemData<DeployableItemData>().MaxDurabilityDays;
			if (this.durability < maxDurabilityDays)
			{
				this.durability = maxDurabilityDays;
				Action onDurabilityRepaired = this.OnDurabilityRepaired;
				if (onDurabilityRepaired == null)
				{
					return;
				}
				onDurabilityRepaired();
			}
		}
	}

	public Vector3Int GetPosition()
	{
		return new Vector3Int(this.x, this.y, this.z);
	}

	public static bool operator ==(SpatialItemInstance lhs, SpatialItemInstance rhs)
	{
		if (lhs == null)
		{
			return rhs == null;
		}
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SpatialItemInstance lhs, SpatialItemInstance rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object obj)
	{
		return this.Equals(obj as SpatialItemInstance);
	}

	public bool Equals(SpatialItemInstance p)
	{
		return p != null && (this == p || (!(base.GetType() != p.GetType()) && (this.x == p.x && this.y == p.y && this.z == p.z) && this.id == p.id));
	}

	public override int GetHashCode()
	{
		return this.x * 65536 + this.y + this.z;
	}

	private bool isOnDamagedCell;

	public int x;

	public int y;

	public int z;

	public float durability;

	public bool seen;

	[NonSerialized]
	public Action OnDamagedStatusChanged;

	[NonSerialized]
	public Action OnDurabilityRepaired;

	[NonSerialized]
	public Action OnInfected;
}
