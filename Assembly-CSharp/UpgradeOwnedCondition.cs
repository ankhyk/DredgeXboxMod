using System;
using System.Collections.Generic;

public class UpgradeOwnedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		return this.upgrades.TrueForAll((UpgradeData u) => saveData.GetIsUpgradeOwned(u));
	}

	public override string Print()
	{
		int count = this.upgrades.FindAll((UpgradeData u) => GameManager.Instance.SaveData.GetIsUpgradeOwned(u)).Count;
		return string.Format("UpgradeOwnedCondition: {0} [{1} / {2}]", count >= this.upgrades.Count, count, this.upgrades.Count);
	}

	public List<UpgradeData> upgrades = new List<UpgradeData>();
}
