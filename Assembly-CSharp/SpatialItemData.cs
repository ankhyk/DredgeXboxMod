using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpatialItemData", menuName = "Dredge/SpatialItemData", order = 0)]
public class SpatialItemData : ItemData
{
	public bool UseIntenseAberratedUIShader
	{
		get
		{
			return this.useIntenseAberratedUIShader;
		}
	}

	public List<OwnedItemResearchablePrerequisite> ItemOwnPrerequisites
	{
		get
		{
			return this.itemOwnPrerequisites;
		}
	}

	public List<ResearchedItemResearchablePrerequisite> ResearchPrerequisites
	{
		get
		{
			return this.researchPrerequisites;
		}
	}

	public int ResearchPointsRequired
	{
		get
		{
			return this.researchPointsRequired;
		}
	}

	public bool BuyableWithoutResearch
	{
		get
		{
			return this.buyableWithoutResearch;
		}
	}

	public bool ResearchIsForRecipe
	{
		get
		{
			return this.researchIsForRecipe;
		}
	}

	public bool ForbidStorageTray
	{
		get
		{
			return this.forbidStorageTray;
		}
	}

	public Sprite GetSprite()
	{
		if (this.platformSpecificSpriteOverrides != null && this.platformSpecificSpriteOverrides.Count > 0)
		{
			foreach (KeyValuePair<Platform, Sprite> keyValuePair in this.platformSpecificSpriteOverrides)
			{
				if (keyValuePair.Key.HasFlag(Platform.PC_GDK))
				{
					return keyValuePair.Value;
				}
			}
		}
		return this.sprite;
	}

	public bool GetCanBeMoved()
	{
		return this.moveMode == MoveMode.FREE || (this.moveMode == MoveMode.INSTALL && GameManager.Instance.Player.CanMoveInstalledItems);
	}

	public int GetWidth()
	{
		int xMax = 0;
		this.dimensions.ForEach(delegate(Vector2Int cell)
		{
			if (cell.x > xMax)
			{
				xMax = cell.x;
			}
		});
		return xMax + 1;
	}

	public int GetHeight()
	{
		int yMax = 0;
		this.dimensions.ForEach(delegate(Vector2Int cell)
		{
			if (cell.y > yMax)
			{
				yMax = cell.y;
			}
		});
		return yMax + 1;
	}

	public int GetSize()
	{
		return this.dimensions.Count;
	}

	[SerializeField]
	public bool canBeSoldByPlayer = true;

	[SerializeField]
	public bool canBeSoldInBulkAction = true;

	[SerializeField]
	public decimal value;

	[SerializeField]
	public bool hasSellOverride;

	[SerializeField]
	public decimal sellOverrideValue;

	[SerializeField]
	public Sprite sprite;

	[SerializeField]
	private Dictionary<Platform, Sprite> platformSpecificSpriteOverrides;

	[SerializeField]
	public Color itemColor = new Color(65f, 65f, 65f, 255f);

	[SerializeField]
	public bool hasSpecialDiscardAction;

	[SerializeField]
	public string discardPromptOverride;

	[SerializeField]
	public bool canBeDiscardedByPlayer;

	[SerializeField]
	public bool showAlertOnDiscardHold;

	[SerializeField]
	public bool discardHoldTimeOverride;

	[SerializeField]
	public float discardHoldTimeSec;

	[SerializeField]
	public bool canBeDiscardedDuringQuestPickup = true;

	[SerializeField]
	public DamageMode damageMode;

	[SerializeField]
	public MoveMode moveMode;

	[SerializeField]
	public bool ignoreDamageWhenPlacing;

	[SerializeField]
	public bool isUnderlayItem;

	[SerializeField]
	private bool forbidStorageTray;

	[SerializeField]
	public List<Vector2Int> dimensions = new List<Vector2Int>();

	[SerializeField]
	public float squishFactor;

	[SerializeField]
	private List<OwnedItemResearchablePrerequisite> itemOwnPrerequisites = new List<OwnedItemResearchablePrerequisite>();

	[SerializeField]
	private List<ResearchedItemResearchablePrerequisite> researchPrerequisites = new List<ResearchedItemResearchablePrerequisite>();

	[SerializeField]
	private int researchPointsRequired;

	[SerializeField]
	private bool buyableWithoutResearch;

	[SerializeField]
	private bool researchIsForRecipe;

	[SerializeField]
	private bool useIntenseAberratedUIShader;
}
