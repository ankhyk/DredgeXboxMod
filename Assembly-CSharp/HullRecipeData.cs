using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "HullRecipeData", menuName = "Dredge/HullRecipeData", order = 0)]
public class HullRecipeData : RecipeData
{
	public override Sprite GetSprite()
	{
		return this.hullUpgradeData.hullSprite;
	}

	public override int GetWidth()
	{
		return 3;
	}

	public override int GetHeight()
	{
		return 3;
	}

	public override LocalizedString GetItemNameKey()
	{
		return this.hullUpgradeData.TitleKey;
	}

	public override int GetQuantityProduced()
	{
		return 1;
	}

	public HullUpgradeData hullUpgradeData;
}
