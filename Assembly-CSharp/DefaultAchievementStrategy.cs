using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DefaultAchievementStrategy : IAchievementStrategy
{
	public Task Init(List<AchievementData> achievement)
	{
		return Task.CompletedTask;
	}

	public bool GetAchievement(AchievementData achievementData)
	{
		return false;
	}

	public void SetAchievement(AchievementData achievementData, bool earned)
	{
	}
}
