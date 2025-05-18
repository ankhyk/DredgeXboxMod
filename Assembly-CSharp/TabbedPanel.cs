using System;
using UnityEngine;

public class TabbedPanel : MonoBehaviour, IScreenSideSwitchResponder
{
	public ScreenSide ScreenSide { get; set; }

	public bool IsShowing()
	{
		return this._isShowing;
	}

	public virtual void ShowStart()
	{
		this._isShowing = true;
		this.container.gameObject.SetActive(true);
	}

	public virtual void ShowFinish()
	{
	}

	public virtual void HideStart()
	{
	}

	public virtual void HideFinish()
	{
		this._isShowing = false;
		this.container.gameObject.SetActive(false);
	}

	public virtual void SwitchToSide()
	{
	}

	public void ToggleSwitchIcon(bool show)
	{
		if (this.sideSwitchIcon)
		{
			this.sideSwitchIcon.SetActive(show);
		}
	}

	public virtual bool GetCanSwitchToThisIfHoldingItem()
	{
		return true;
	}

	[SerializeField]
	protected GameObject container;

	[SerializeField]
	private GameObject sideSwitchIcon;

	private bool _isShowing;
}
