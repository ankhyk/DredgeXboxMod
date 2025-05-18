using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeployableItemData", menuName = "Dredge/DeployableItemData", order = 0)]
public class DeployableItemData : HarvesterItemData
{
	public float TimeBetweenCatchRolls
	{
		get
		{
			return this.timeBetweenCatchRolls;
		}
	}

	public float MaxDurabilityDays
	{
		get
		{
			return this.maxDurabilityDays;
		}
	}

	public float CatchRate
	{
		get
		{
			return this.catchRate;
		}
	}

	public GridConfiguration GridConfig
	{
		get
		{
			return this.gridConfig;
		}
	}

	public GridKey GridKey
	{
		get
		{
			return this.gridKey;
		}
	}

	[SerializeField]
	private float timeBetweenCatchRolls;

	[SerializeField]
	private float catchRate;

	[SerializeField]
	private float maxDurabilityDays;

	[SerializeField]
	private GridConfiguration gridConfig;

	[SerializeField]
	private GridKey gridKey;
}
