using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SwitchEntitlementStrategy : IEntitlementStrategy
{
	public bool GetHasEntitlement(EntitlementData entitlementData)
	{
		return false;
	}

	public void OpenEntitlementList()
	{
	}

	public Task Init(List<EntitlementData> entitlements)
	{
		return Task.CompletedTask;
	}

	private static int NUM_DLC_ITEMS = 4;
}
