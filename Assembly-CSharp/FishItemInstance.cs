using System;
using UnityEngine;

[Serializable]
public class FishItemInstance : SpatialItemInstance
{
	public decimal GetSaleModifierForSize()
	{
		return (decimal)Mathf.Lerp((float)GameManager.Instance.GameConfigData.MinSizeSaleModifier, (float)GameManager.Instance.GameConfigData.MaxSizeSaleModifier, this.size);
	}

	public bool IsTrophySize()
	{
		return this.size >= GameManager.Instance.GameConfigData.TrophyMaxSize;
	}

	public bool IsAberrant()
	{
		return this.GetItemData<FishItemData>().IsAberration;
	}

	public void Infect()
	{
		this.isInfected = true;
		this.freshness = 0f;
		Action onInfected = this.OnInfected;
		if (onInfected == null)
		{
			return;
		}
		onInfected();
	}

	public decimal GetSaleModifierForFreshness()
	{
		decimal num;
		if (this.freshness > GameManager.Instance.GameConfigData.MaxFreshness - 1f)
		{
			num = GameManager.Instance.GameConfigData.FreshnessSaleModifiers[2];
		}
		else if (this.freshness < 1f)
		{
			num = GameManager.Instance.GameConfigData.FreshnessSaleModifiers[0];
		}
		else
		{
			num = GameManager.Instance.GameConfigData.FreshnessSaleModifiers[1];
		}
		return num;
	}

	public float size;

	public float freshness;

	public bool isInfected;
}
