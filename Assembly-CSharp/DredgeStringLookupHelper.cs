using System;
using UnityEngine;

public class DredgeStringLookupHelper : MonoBehaviour
{
	public static string ColorSwappedString(string inputString)
	{
		return inputString.Replace("[C0]", "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.EMPHASIS) + ">").Replace("[C1]", "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE) + ">").Replace("[C2]", "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE) + ">")
			.Replace("[C3]", "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL) + ">")
			.Replace("[/C0]", "</color>")
			.Replace("[/C1]", "</color>")
			.Replace("[/C2]", "</color>")
			.Replace("[/C3]", "</color>");
	}

	public string _LumberSteelPoint
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.STEEL_POINT_MATERIALS).spatialItems.FindAll((SpatialItemInstance i) => i.id == "lumber").Count.ToString();
		}
	}

	public string _ScrapSteelPoint
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.STEEL_POINT_MATERIALS).spatialItems.FindAll((SpatialItemInstance i) => i.id == "scrap").Count.ToString();
		}
	}

	public string _LumberTIRBase
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.TIR_RIG_BASE).spatialItems.FindAll((SpatialItemInstance i) => i.id == "lumber").Count.ToString();
		}
	}

	public string _ScrapTIRBase
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.TIR_RIG_BASE).spatialItems.FindAll((SpatialItemInstance i) => i.id == "scrap").Count.ToString();
		}
	}

	public string _WireTIRBase
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.TIR_RIG_BASE).spatialItems.FindAll((SpatialItemInstance i) => i.id == "wire").Count.ToString();
		}
	}

	public string _CratesTIRBase
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.TIR_RIG_BASE).spatialItems.FindAll((SpatialItemInstance i) => i.id == "crate").Count.ToString();
		}
	}

	public string _OctopusSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_FIRST_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "glowing-octopus").Count.ToString();
		}
	}

	public string _SquidSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_FIRST_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "firefly-squid").Count.ToString();
		}
	}

	public string _JellySB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_FIRST_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "aurora-jellyfish").Count.ToString();
		}
	}

	public string _AnglerfishSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_SECOND_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "anglerfish").Count.ToString();
		}
	}

	public string _LoosejawSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_SECOND_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "stoplight-loosejaw").Count.ToString();
		}
	}

	public string _SnailfishSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_SECOND_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "snailfish").Count.ToString();
		}
	}

	public string _AmphipodSB
	{
		get
		{
			return GameManager.Instance.SaveData.GetGridByKey(GridKey.RESEARCH_SECOND_SET).spatialItems.FindAll((SpatialItemInstance i) => i.id == "giant-amphipod").Count.ToString();
		}
	}

	public string _Icebreaker
	{
		get
		{
			int num = 0;
			if (GameManager.Instance.SaveData.historyOfItemsOwned.Contains("quest-icebreaker-1"))
			{
				num++;
			}
			if (GameManager.Instance.SaveData.historyOfItemsOwned.Contains("quest-icebreaker-2"))
			{
				num++;
			}
			if (GameManager.Instance.SaveData.historyOfItemsOwned.Contains("quest-icebreaker-3"))
			{
				num++;
			}
			return num.ToString();
		}
	}

	public string _NarwhalFish
	{
		get
		{
			int c = 0;
			GameManager.Instance.SaveData.GetGridByKey(GridKey.DLC1_MONSTER_FEEDING_STATION_1).spatialItems.ForEach(delegate(SpatialItemInstance i)
			{
				c += i.GetItemData<SpatialItemData>().dimensions.Count;
			});
			return c.ToString();
		}
	}

	public string _Flames
	{
		get
		{
			return GameManager.Instance.SaveData.GetIntVariable("flames-lit", 0).ToString();
		}
	}

	public string _TabletsReturned
	{
		get
		{
			return GameManager.Instance.SaveData.GetIntVariable("tablets-returned", 0).ToString();
		}
	}
}
