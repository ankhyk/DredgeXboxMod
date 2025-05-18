using System;
using System.Diagnostics;
using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
	private void Start()
	{
		BasicButtonWrapper basicButtonWrapper = this.button;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnClick));
	}

	public void OnClick()
	{
		if (!this.hasBeenClicked)
		{
			this.hasBeenClicked = true;
			BasicButtonWrapper basicButtonWrapper = this.button;
			basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnClick));
			if (!Application.isEditor)
			{
				Process.GetCurrentProcess().Kill();
			}
		}
	}

	[SerializeField]
	private BasicButtonWrapper button;

	private bool hasBeenClicked;
}
