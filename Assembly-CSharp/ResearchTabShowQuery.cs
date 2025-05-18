using System;
using UnityEngine;

public class ResearchTabShowQuery : TabShowQuery
{
	public override bool GetCanNavigate()
	{
		bool flag = true;
		if (this.dependentAbility != null)
		{
			flag = flag && GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked(this.dependentAbility);
		}
		if (this.dependentUpgrade != null)
		{
			flag = flag && GameManager.Instance.SaveData.GetIsUpgradeOwned(this.dependentUpgrade);
		}
		return flag;
	}

	[SerializeField]
	private UpgradeData dependentUpgrade;

	[SerializeField]
	private AbilityData dependentAbility;
}
