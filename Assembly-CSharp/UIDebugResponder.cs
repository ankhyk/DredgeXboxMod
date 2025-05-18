using System;
using UnityEngine;

public class UIDebugResponder : MonoBehaviour
{
	private void Awake()
	{
		ApplicationEvents.Instance.OnUIDebugToggled += this.OnUIDebugToggled;
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnUIDebugToggled -= this.OnUIDebugToggled;
	}

	private void OnUIDebugToggled(bool enabled)
	{
		this.canvasGroup.alpha = (float)(enabled ? 1 : 0);
	}

	[SerializeField]
	private CanvasGroup canvasGroup;
}
