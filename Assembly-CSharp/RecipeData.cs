using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Dredge/RecipeData", order = 0)]
public abstract class RecipeData : SerializedScriptableObject, IUpgradeCost, IDisplayableRecipe
{
	GridKey IUpgradeCost.GetGridKey()
	{
		return this.questGridConfig.gridKey;
	}

	decimal IUpgradeCost.GetMonetaryCost()
	{
		return this.cost;
	}

	List<ItemCountCondition> IUpgradeCost.GetItemCost()
	{
		return this.questGridConfig.completeConditions.OfType<ItemCountCondition>().ToList<ItemCountCondition>();
	}

	public abstract Sprite GetSprite();

	public abstract int GetWidth();

	public abstract int GetHeight();

	public abstract LocalizedString GetItemNameKey();

	public abstract int GetQuantityProduced();

	public string recipeId;

	public QuestGridConfig questGridConfig;

	public decimal cost;

	public string onRecipeShownDialogueNodeName;

	public string onRecipeBuiltDialogueNodeName;

	public int researchRequired;

	public int quantityProduced = 1;
}
