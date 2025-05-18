using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DockLabel : MonoBehaviour
{
	private void OnEnable()
	{
		bool boolVariable = GameManager.Instance.SaveData.GetBoolVariable("has-visited-dock-" + this.dockData.Id, false);
		this.textField.enabled = boolVariable;
		if (boolVariable)
		{
			this.localizedStringEvent.StringReference = this.dockData.DockNameKey;
		}
	}

	[SerializeField]
	private DockData dockData;

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private LocalizeStringEvent localizedStringEvent;
}
