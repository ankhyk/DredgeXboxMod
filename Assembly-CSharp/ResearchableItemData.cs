using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ResearchableItemData", menuName = "Dredge/ResearchableItemData", order = 0)]
public class ResearchableItemData : NonSpatialItemData
{
	public float daysToResearch;

	public ResearchBenefitType researchBenefitType;

	public float researchBenefitValue;

	public LocalizedString completedDescriptionKey;
}
