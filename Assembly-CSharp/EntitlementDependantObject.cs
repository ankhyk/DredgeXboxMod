using System;
using UnityEngine;

public class EntitlementDependantObject : MonoBehaviour
{
	private void Awake()
	{
		if (!GameManager.Instance.EntitlementManager.GetHasEntitlement(this.entitlement))
		{
			if (this.destroyOnEntitlementNotOwned)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			base.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
	}

	[SerializeField]
	private Entitlement entitlement;

	[SerializeField]
	private bool destroyOnEntitlementNotOwned;
}
