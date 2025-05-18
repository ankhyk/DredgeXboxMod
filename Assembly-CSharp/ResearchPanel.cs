using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResearchPanel : TabbedPanel
{
	public override void ShowFinish()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
		base.ShowFinish();
	}

	public override void HideStart()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
		base.HideStart();
	}

	private void OnTimeForcefullyPassingChanged(bool isTimePassing, string str, TimePassageMode mode)
	{
		if (isTimePassing)
		{
			if (EventSystem.current.currentSelectedGameObject)
			{
				BasicButtonWrapper component = EventSystem.current.currentSelectedGameObject.GetComponent<BasicButtonWrapper>();
				if (component)
				{
					component.SetSelectable(this.controllerFocusGrabber);
				}
			}
			this.controllerFocusGrabber.enabled = false;
			this.canvasGroupDisabler.Disable();
			return;
		}
		this.controllerFocusGrabber.enabled = true;
		this.canvasGroupDisabler.Enable();
	}

	[SerializeField]
	private CanvasGroupDisabler canvasGroupDisabler;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;
}
