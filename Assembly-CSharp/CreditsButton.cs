using System;
using UnityEngine;

public class CreditsButton : MonoBehaviour
{
	private void Awake()
	{
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	public void OnClick()
	{
		global::UnityEngine.Object.Instantiate<GameObject>(this.creditsPrefab).GetComponent<CreditsController>().SetCreditsMode(CreditsMode.SHOWING_ON_MENU);
	}

	[SerializeField]
	private BasicButtonWrapper button;

	[SerializeField]
	private GameObject creditsPrefab;
}
