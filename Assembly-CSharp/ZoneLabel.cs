using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class ZoneLabel : MonoBehaviour
{
	private void OnEnable()
	{
	}

	[SerializeField]
	private LocalizedString obscuredLabelKey;

	[SerializeField]
	private LocalizeStringEvent localizedStringEvent;

	[SerializeField]
	private bool availableInDemo;
}
