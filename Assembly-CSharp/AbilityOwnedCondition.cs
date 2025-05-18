using System;
using System.Collections.Generic;

public class AbilityOwnedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		return this.abilities.TrueForAll((AbilityData u) => saveData.unlockedAbilities.Contains(u.name.ToLower()));
	}

	public override string Print()
	{
		int count = this.abilities.FindAll((AbilityData u) => GameManager.Instance.SaveData.unlockedAbilities.Contains(u.name.ToLower())).Count;
		return string.Format("AbilityOwnedCondition: {0} [{1} / {2}]", count >= this.abilities.Count, count, this.abilities.Count);
	}

	public List<AbilityData> abilities = new List<AbilityData>();
}
