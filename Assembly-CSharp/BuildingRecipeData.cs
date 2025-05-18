using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "BuildingRecipeData", menuName = "Dredge/BuildingRecipeData", order = 0)]
public class BuildingRecipeData : RecipeData
{
	public override int GetWidth()
	{
		return 1;
	}

	public override int GetHeight()
	{
		return 1;
	}

	public override LocalizedString GetItemNameKey()
	{
		return this.buildingNameKey;
	}

	public override int GetQuantityProduced()
	{
		return 1;
	}

	public override Sprite GetSprite()
	{
		return null;
	}

	[SerializeField]
	public LocalizedString buildingNameKey;

	[SerializeField]
	public LocalizedString buildingDescriptionKey;
}
