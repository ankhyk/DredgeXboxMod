using System;
using UnityEngine;

public class ResearchedItemResearchablePrerequisite : ResearchPrerequisite
{
	public override bool IsPrerequisiteMet()
	{
		return GameManager.Instance.SaveData.GetIsItemResearched(this.itemData);
	}

	[SerializeField]
	public SpatialItemData itemData;
}
