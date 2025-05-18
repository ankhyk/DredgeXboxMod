using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestableItemData", menuName = "Dredge/HarvestableItemData", order = 0)]
public class HarvestableItemData : SpatialItemData
{
	public DepthEnum MinDepth
	{
		get
		{
			return this.minDepth;
		}
	}

	public DepthEnum MaxDepth
	{
		get
		{
			return this.maxDepth;
		}
	}

	public bool HasMinDepth
	{
		get
		{
			return this.hasMinDepth;
		}
	}

	public bool HasMaxDepth
	{
		get
		{
			return this.hasMaxDepth;
		}
	}

	public string GetDepthString()
	{
		string text = "";
		Vector2 vector = GameManager.Instance.GameConfigData.DepthBands[this.minDepth];
		Vector2 vector2 = GameManager.Instance.GameConfigData.DepthBands[this.maxDepth];
		if (this.hasMinDepth && this.hasMaxDepth)
		{
			string formattedDepthString = GameManager.Instance.ItemManager.GetFormattedDepthString(vector.x);
			string formattedDepthString2 = GameManager.Instance.ItemManager.GetFormattedDepthString(vector2.y);
			text = formattedDepthString + " - " + formattedDepthString2;
		}
		else if (this.hasMinDepth)
		{
			text = GameManager.Instance.ItemManager.GetFormattedDepthString(vector.x) + " +";
		}
		else if (this.hasMaxDepth)
		{
			string formattedDepthString3 = GameManager.Instance.ItemManager.GetFormattedDepthString(vector2.y);
			text = "0 - " + formattedDepthString3;
		}
		return text;
	}

	public bool CanCatchByDepth(float depth, GameConfigData gameConfigData)
	{
		bool flag = true;
		if (!this.hasMinDepth && !this.hasMaxDepth)
		{
			return true;
		}
		Vector2 vector = Vector2.zero;
		if (this.hasMinDepth)
		{
			vector = gameConfigData.DepthBands[this.minDepth];
		}
		Vector2 vector2 = Vector2.zero;
		if (this.hasMaxDepth)
		{
			vector2 = gameConfigData.DepthBands[this.maxDepth];
		}
		if (flag && this.hasMinDepth && depth < vector.x)
		{
			flag = false;
		}
		if (flag && this.hasMaxDepth && depth > vector2.y)
		{
			flag = false;
		}
		return flag;
	}

	[SerializeField]
	public HarvestMinigameType harvestMinigameType;

	public int perSpotMin = 1;

	public int perSpotMax = 1;

	public float harvestItemWeight = 1f;

	public bool regenHarvestSpotOnDestroy;

	public HarvestPOICategory harvestPOICategory;

	public HarvestableType harvestableType;

	public bool requiresAdvancedEquipment;

	public HarvestDifficulty harvestDifficulty;

	public bool canBeReplacedWithResearchItem;

	public bool canBeCaughtByRod;

	public bool canBeCaughtByPot;

	public bool canBeCaughtByNet;

	public bool affectedByFishingSustain = true;

	[SerializeField]
	private bool hasMinDepth;

	[SerializeField]
	private DepthEnum minDepth;

	[SerializeField]
	private bool hasMaxDepth;

	[SerializeField]
	private DepthEnum maxDepth;

	public ZoneEnum zonesFoundIn;
}
