using System;
using System.Collections.Generic;
using UnityEngine;

public class PortraitVFXEnabler : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler<bool>("TogglePortraitVFX", new Action<bool>(this.TogglePortraitVFX));
	}

	private void OnDestroy()
	{
		GameManager.Instance.DialogueRunner.RemoveCommandHandler("TogglePortraitVFX");
	}

	private void TogglePortraitVFX(bool enabled)
	{
		this.portraitVFX.ForEach(delegate(GameObject g)
		{
			g.SetActive(enabled);
		});
	}

	[SerializeField]
	private List<GameObject> portraitVFX;
}
