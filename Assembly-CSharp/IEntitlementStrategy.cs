using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEntitlementStrategy
{
	Task Init(List<EntitlementData> entitlements);

	bool GetHasEntitlement(EntitlementData entitlementData);

	void OpenEntitlementList();
}
