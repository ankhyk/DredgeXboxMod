using System;
using System.Collections.Generic;

public class AchievementEarnedCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		AchievementManager achievementManager = GameManager.Instance.AchievementManager;
		return this.achievements.TrueForAll((AchievementData a) => achievementManager.GetAchievementState(a));
	}

	public override string Print()
	{
		int count = this.achievements.FindAll((AchievementData a) => GameManager.Instance.AchievementManager.GetAchievementState(a)).Count;
		return string.Format("AchievementEarnedCondition: {0} [{1} / {2}]", count >= this.achievements.Count, count, this.achievements.Count);
	}

	public List<AchievementData> achievements = new List<AchievementData>();
}
