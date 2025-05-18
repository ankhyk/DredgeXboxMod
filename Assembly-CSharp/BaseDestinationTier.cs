using System;
using UnityEngine;
using UnityEngine.Localization;

public class BaseDestinationTier
{
	[SerializeField]
	public BuildingTierId tierId;

	[SerializeField]
	public LocalizedString tierNameKey;

	[SerializeField]
	public RecipeData recipeToCreateThis;

	[SerializeField]
	public string descriptionDialogueNodeName;

	[SerializeField]
	public string postConstructionDialogueNodeName;

	[SerializeField]
	public ConstructableDestinationUI.ConstructionViewState viewAfterConstruction;
}
