using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "AbilityRecipeData", menuName = "Dredge/AbilityRecipeData", order = 0)]
public class AbilityRecipeData : RecipeData
{
	public override Sprite GetSprite()
	{
		return this.abilityData.icon;
	}

	public override int GetWidth()
	{
		return 2;
	}

	public override int GetHeight()
	{
		return 2;
	}

	public override LocalizedString GetItemNameKey()
	{
		return this.abilityData.nameKey;
	}

	public override int GetQuantityProduced()
	{
		return 1;
	}

	public AbilityData abilityData;
}
