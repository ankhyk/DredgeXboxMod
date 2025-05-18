using System;
using UnityEngine;

public class CanvasGroupDisabler : MonoBehaviour
{
	public void Disable()
	{
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
	}

	public void Enable()
	{
		this.canvasGroup.interactable = true;
		this.canvasGroup.blocksRaycasts = true;
	}

	[SerializeField]
	private CanvasGroup canvasGroup;
}
