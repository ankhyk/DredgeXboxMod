using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;

public class SteamAchievementStrategy : IAchievementStrategy
{
	public bool GetAchievement(AchievementData achievementData)
	{
		bool flag = false;
		if (SteamManager.Initialized)
		{
			SteamUserStats.GetAchievement(achievementData.steamId, out flag);
		}
		return flag;
	}

	public Task Init(List<AchievementData> achievements)
	{
		return Task.CompletedTask;
	}

	public void SetAchievement(AchievementData achievementData, bool earned)
	{
		if (SteamManager.Initialized)
		{
			if (earned)
			{
				SteamUserStats.SetAchievement(achievementData.steamId);
			}
			else
			{
				SteamUserStats.ClearAchievement(achievementData.steamId);
			}
			SteamUserStats.StoreStats();
		}
	}
}
