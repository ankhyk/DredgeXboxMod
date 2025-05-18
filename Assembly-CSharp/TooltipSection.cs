using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class TooltipSection<ItemData> : SerializedMonoBehaviour, ILayoutable
{
	public abstract void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode) where T : ItemData;

	public virtual void Init<T>(T itemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode) where T : ItemData
	{
	}

	public virtual void Init()
	{
	}

	public virtual void Init(UpgradeData upgradeData)
	{
	}

	public virtual void Init(IUpgradeCost upgradeCost)
	{
	}

	public virtual void Init(TextTooltipRequester textTooltipRequester)
	{
	}

	public virtual void Init(AbilityTooltipRequester abilityTooltipRequester)
	{
	}

	public bool IsLayedOut
	{
		get
		{
			return this.isLayedOut;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	protected bool isLayedOut;
}
