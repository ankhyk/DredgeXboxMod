using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditionalContentButton : MonoBehaviour
{
	private void Awake()
	{
		if (!this.GetShouldShow())
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private bool GetShouldShow()
	{
		return !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DELUXE) || !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1) || !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2);
	}

	private void OnDLCInstalled(Entitlement arg0)
	{
		if (!this.GetShouldShow())
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			if (SceneManager.GetActiveScene().name == "Title")
			{
				GameObject gameObject = GameObject.Find("ButtonContainer");
				if (gameObject != null)
				{
					gameObject.GetComponent<ControllerFocusGrabber>().SelectSelectable();
				}
			}
		}
	}
}
