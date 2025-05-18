using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityEngine;
using XGamingRuntime;

public class GDKPCAchievementStrategy : IAchievementStrategy
{
	public async Task Init(List<AchievementData> achievements)
	{
		Debug.LogWarning("GameCoreAchievementStrategy Init");
		this.Unlocked = new List<string>();
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		if (gdkpcconsoleStrategy.xboxUser.XboxLiveContext == null)
		{
			Debug.LogWarning("Failed get achievements as no XboxLiveContext was found");
		}
		else
		{
			bool done = false;
			SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(gdkpcconsoleStrategy.xboxUser.XboxLiveContext, gdkpcconsoleStrategy.xboxUser.UserXUID, 1907796436U, XblAchievementType.All, true, XblAchievementOrderBy.DefaultOrder, 0U, 100U, delegate(int hresult, XblAchievementsResultHandle result)
			{
				if (hresult == 0)
				{
					XblAchievement[] array;
					int num = SDK.XBL.XblAchievementsResultGetAchievements(result, out array);
					if (num == 0 && array != null)
					{
						XblAchievement[] array2 = array;
						for (int j = 0; j < array2.Length; j++)
						{
							XblAchievement i = array2[j];
							this.Unlocked.Add(i.Id);
							Debug.Log(string.Format("Unlocked: {0}", achievements.Find((AchievementData x) => x.xboxId == i.Id)));
						}
						Debug.Log(string.Format("Unlocked count {0}/{1}", array.Length, achievements.Count));
					}
					else
					{
						Debug.Log(string.Format("Failed to GetAchievement list  HResult: {0} achievementListIsValid:{1}", num, array != null));
					}
				}
				else
				{
					Debug.LogError(string.Format("Failed to GetAchievement HResult: {0}", hresult));
				}
				done = true;
			});
			while (!done)
			{
				await Awaiters.NextFrame;
			}
			Debug.Log(string.Format("GameCoreAchievementStrategy Init completed : UnlockedCount = {0}", this.Unlocked.Count));
		}
	}

	public bool GetAchievement(AchievementData achievementData)
	{
		return this.Unlocked.Contains(achievementData.xboxId);
	}

	public void SetAchievement(AchievementData achievementData, bool earned)
	{
		Debug.Log("SetAchievement: " + achievementData.xboxId + " GameCore");
		if (!earned)
		{
			return;
		}
		if (!this.Unlocked.Contains(achievementData.xboxId))
		{
			this.Unlocked.Add(achievementData.xboxId);
		}
		GDKPCConsoleStrategy gdkpcconsoleStrategy = GameManager.Instance.ConsoleManager.CurrentConsole as GDKPCConsoleStrategy;
		if (gdkpcconsoleStrategy.xboxUser.XboxLiveContext == null)
		{
			Debug.LogWarning("Failed set achievement as no XboxLiveContext was found");
			return;
		}
		Debug.Log("XblAchievementsUpdateAchievementAsync: " + achievementData.xboxId + " GameCore");
		SDK.XBL.XblAchievementsUpdateAchievementAsync(gdkpcconsoleStrategy.xboxUser.XboxLiveContext, gdkpcconsoleStrategy.xboxUser.UserXUID, achievementData.xboxId, 100U, delegate(int hresult)
		{
			if (hresult == 0)
			{
				Debug.Log("Achievement with Id:" + achievementData.xboxId + " has been unlocked/updated");
				if (!this.Unlocked.Contains(achievementData.xboxId))
				{
					this.Unlocked.Add(achievementData.xboxId);
					return;
				}
			}
			else
			{
				if (this.Unlocked.Contains(achievementData.xboxId))
				{
					this.Unlocked.Remove(achievementData.xboxId);
				}
				Debug.Log(string.Format("Failed to unlock/update Achievement with Id:{0}. ErrorCode:{1}", achievementData.xboxId, hresult));
			}
		});
	}

	private List<string> Unlocked = new List<string>();
}
