using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class StringWithControlPromptGlyph : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.RefreshText();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CONTROL_BINDINGS)
		{
			this.RefreshText();
		}
	}

	public void RefreshText()
	{
		string text = GameManager.Instance.LanguageManager.FormatControlPromptString(this.localizedString, this.controls);
		this.textField.text = text;
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private LocalizedString localizedString;

	[SerializeField]
	private List<DredgeControlEnum> controls;
}
