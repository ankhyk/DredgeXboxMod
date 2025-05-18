using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;

public class SteamEntitlementStrategy : IEntitlementStrategy
{
	public bool GetHasEntitlement(EntitlementData entitlementData)
	{
		bool flag = false;
		if (SteamManager.Initialized)
		{
			flag = SteamApps.BIsSubscribedApp(new AppId_t(Convert.ToUInt32(entitlementData.steamId)));
		}
		return flag;
	}

	public void OpenEntitlementList()
	{
	}

	public Task Init(List<EntitlementData> entitlements)
	{
		return Task.CompletedTask;
	}
}
