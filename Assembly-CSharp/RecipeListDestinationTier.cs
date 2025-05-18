using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class RecipeListDestinationTier : BaseDestinationTier
{
	[SerializeField]
	public LocalizedString recipeListStringKey;

	[SerializeField]
	public List<RecipeData> recipes;
}
