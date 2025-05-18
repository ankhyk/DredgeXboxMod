using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ItemRecipeData", menuName = "Dredge/ItemRecipeData", order = 0)]
public class ItemRecipeData : RecipeData
{
	public override Sprite GetSprite()
	{
		return this.itemProduced.sprite;
	}

	public override int GetWidth()
	{
		return this.itemProduced.GetWidth();
	}

	public override int GetHeight()
	{
		return this.itemProduced.GetHeight();
	}

	public override LocalizedString GetItemNameKey()
	{
		return this.itemProduced.itemNameKey;
	}

	public override int GetQuantityProduced()
	{
		return this.quantityProduced;
	}

	public SpatialItemData itemProduced;
}
