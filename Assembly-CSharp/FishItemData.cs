using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishItemData", menuName = "Dredge/FishItemData", order = 0)]
public class FishItemData : HarvestableItemData
{
	public float MinSizeCentimeters
	{
		get
		{
			return this.minSizeCentimeters;
		}
	}

	public float MaxSizeCentimeters
	{
		get
		{
			return this.maxSizeCentimeters;
		}
	}

	public List<FishItemData> Aberrations
	{
		get
		{
			return this.aberrations;
		}
		set
		{
			this.aberrations = value;
		}
	}

	public bool IsAberration
	{
		get
		{
			return this.isAberration;
		}
		set
		{
			this.isAberration = value;
		}
	}

	public FishItemData NonAberrationParent
	{
		get
		{
			return this.nonAberrationParent;
		}
		set
		{
			this.nonAberrationParent = value;
		}
	}

	public int MinWorldPhaseRequired
	{
		get
		{
			return this.minWorldPhaseRequired;
		}
	}

	public bool LocationHiddenUntilCaught
	{
		get
		{
			return this.locationHiddenUntilCaught;
		}
	}

	public bool Day
	{
		get
		{
			return this.day;
		}
	}

	public bool Night
	{
		get
		{
			return this.night;
		}
	}

	public bool CanAppearInBaitBalls
	{
		get
		{
			return this.canAppearInBaitBalls;
		}
	}

	public bool CanBeInfected
	{
		get
		{
			return this.canBeInfected && !this.isAberration;
		}
	}

	public float RotCoefficient
	{
		get
		{
			return this.rotCoefficient;
		}
	}

	public int TIRPhase
	{
		get
		{
			return this.tirPhase;
		}
	}

	public QuestData QuestCompleteRequired
	{
		get
		{
			return this.questCompleteRequired;
		}
	}

	public float BaitChanceOverride
	{
		get
		{
			return this.baitChanceOverride;
		}
	}

	public FishItemData()
	{
		this.itemType = ItemType.GENERAL;
		this.itemSubtype = ItemSubtype.FISH;
		this.canBeDiscardedByPlayer = true;
	}

	[SerializeField]
	private float minSizeCentimeters;

	[SerializeField]
	private float maxSizeCentimeters;

	[SerializeField]
	private List<FishItemData> aberrations;

	[SerializeField]
	private bool isAberration;

	[SerializeField]
	private FishItemData nonAberrationParent;

	[SerializeField]
	private int minWorldPhaseRequired;

	[SerializeField]
	private bool locationHiddenUntilCaught;

	[SerializeField]
	private bool day;

	[SerializeField]
	private bool night;

	[SerializeField]
	private bool canAppearInBaitBalls = true;

	[SerializeField]
	private QuestData questCompleteRequired;

	[SerializeField]
	private float baitChanceOverride = -1f;

	[SerializeField]
	private bool canBeInfected = true;

	[SerializeField]
	public List<Vector2Int> cellsExcludedFromDisplayingInfection;

	[SerializeField]
	private float rotCoefficient = 1f;

	[SerializeField]
	private int tirPhase;
}
