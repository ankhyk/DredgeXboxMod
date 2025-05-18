using System;
using UnityEngine;

public class StoreLinkButton : MonoBehaviour
{
	public void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
	}

	public void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnButtonClicked));
	}

	private void OnButtonClicked()
	{
		GameManager.Instance.EntitlementManager.entitlementStrategy.OpenEntitlementList();
	}

	[SerializeField]
	private bool showAllDLC;

	[SerializeField]
	private StoreSKUData storeSKUData;

	[SerializeField]
	private BasicButtonWrapper button;
}
