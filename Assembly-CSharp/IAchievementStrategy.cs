using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAchievementStrategy
{
	Task Init(List<AchievementData> achievements);

	bool GetAchievement(AchievementData achievementData);

	void SetAchievement(AchievementData achievementData, bool earned);
}
