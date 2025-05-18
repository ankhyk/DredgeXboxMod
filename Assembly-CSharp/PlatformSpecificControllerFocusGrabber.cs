using System;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSpecificControllerFocusGrabber : ControllerFocusGrabber
{
	protected new void OnEnable()
	{
		base.OnEnable();
	}

	[SerializeField]
	private Selectable consoleSelectable;
}
