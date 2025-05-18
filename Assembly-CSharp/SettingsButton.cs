using System;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
	private void Awake()
	{
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	public void OnClick()
	{
		ApplicationEvents.Instance.TriggerToggleSettings(true);
	}

	[SerializeField]
	private BasicButtonWrapper button;
}
