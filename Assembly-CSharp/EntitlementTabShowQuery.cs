using System;
using UnityEngine;

public class EntitlementTabShowQuery : TabShowQuery
{
	private void Awake()
	{
		this.cachedHasEntitlement = GameManager.Instance.EntitlementManager.GetHasEntitlement(this.dependantEntitlement);
	}

	public override bool GetCanNavigate()
	{
		return this.cachedHasEntitlement;
	}

	public override bool GetCanShow()
	{
		return this.cachedHasEntitlement;
	}

	[SerializeField]
	private Entitlement dependantEntitlement;

	private bool cachedHasEntitlement;
}
