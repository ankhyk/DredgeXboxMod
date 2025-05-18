using System;
using UnityEngine;

public class OwnedItemResearchablePrerequisite : ResearchPrerequisite
{
	public override bool IsPrerequisiteMet()
	{
		return GameManager.Instance.SaveData.historyOfItemsOwned.Contains(this.itemData.id);
	}

	[SerializeField]
	public ItemData itemData;
}
