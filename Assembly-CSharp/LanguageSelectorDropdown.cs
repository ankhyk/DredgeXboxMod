using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class LanguageSelectorDropdown : SerializedMonoBehaviour, ISettingsRefreshable
{
	private void Awake()
	{
		this.supportedLocaleData = GameManager.Instance.LanguageManager.SupportedLocaleData;
		int count = this.supportedLocaleData.locales.Count;
		this.mainBoxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)count * this.heightPerLocale);
		this.mainBoxRect.ForceUpdateRectTransforms();
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnLanguageOptionSelected));
	}

	public void ForceRefresh()
	{
		this.Refresh();
	}

	private void OnEnable()
	{
		this.Refresh();
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Combine(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
	}

	private void OnDisable()
	{
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Remove(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
	}

	private void Refresh()
	{
		this.dropdown.options.Clear();
		if (this.supportedLocaleData == null)
		{
			return;
		}
		this.supportedLocaleData.locales.ForEach(delegate(Locale locale)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
			string titleCaseNativeLanguage = this.GetTitleCaseNativeLanguage(locale);
			optionData.text = titleCaseNativeLanguage;
			this.dropdown.options.Add(optionData);
		});
		Locale locale2 = GameManager.Instance.LanguageManager.GetLocale();
		int num = this.supportedLocaleData.locales.IndexOf(locale2);
		if (locale2 == this.supportedLocaleData.testLocale)
		{
			num = this.supportedLocaleData.locales.Count;
		}
		if (num != -1)
		{
			this.dropdown.SetValueWithoutNotify(num);
		}
		this.dropdownSettingInput.CurrentIndex = num;
		this.RefreshSelectedValueTextField(num);
	}

	private string GetTitleCaseNativeLanguage(Locale locale)
	{
		CultureInfo cultureInfo = locale.Identifier.CultureInfo;
		string text;
		if (this.supportedLocaleData.nativeLanguageNameOverrides.ContainsKey(cultureInfo.Name))
		{
			text = this.supportedLocaleData.nativeLanguageNameOverrides.GetValueOrDefault(cultureInfo.Name);
		}
		else
		{
			text = (cultureInfo.IsNeutralCulture ? cultureInfo.NativeName : cultureInfo.Parent.NativeName);
		}
		return cultureInfo.TextInfo.ToTitleCase(text);
	}

	public void OnLanguageOptionSelected(int index)
	{
		this.GetLocaleSelectedByIndex(index);
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.LANGUAGE);
		ApplicationEvents.Instance.TriggerLocaleChange(this.GetLocaleSelectedByIndex(index));
		this.RefreshSelectedValueTextField(index);
	}

	private void RefreshSelectedValueTextField(int index)
	{
		Locale localeSelectedByIndex = this.GetLocaleSelectedByIndex(index);
		TMP_FontAsset tmp_FontAsset = null;
		if (this.supportedLocaleData.fonts.TryGetValue(localeSelectedByIndex, out tmp_FontAsset))
		{
			this.selectedValueTextField.font = tmp_FontAsset;
			return;
		}
		this.selectedValueTextField.font = this.defaultFont;
	}

	private void OnComponentSubmitted()
	{
		List<DropdownItem> list = base.transform.GetComponentsInChildren<DropdownItem>().ToList<DropdownItem>();
		for (int i = 0; i < list.Count; i++)
		{
			DropdownItem dropdownItem = list[i];
			TMP_FontAsset tmp_FontAsset = null;
			Locale localeSelectedByIndex = this.GetLocaleSelectedByIndex(i);
			if (this.supportedLocaleData.fonts.TryGetValue(localeSelectedByIndex, out tmp_FontAsset))
			{
				dropdownItem.textField.font = tmp_FontAsset;
			}
			dropdownItem.textField.enabled = true;
		}
	}

	private Locale GetLocaleSelectedByIndex(int index)
	{
		if (this.supportedLocaleData.locales.Count > index)
		{
			return this.supportedLocaleData.locales[index];
		}
		return this.supportedLocaleData.testLocale;
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private TMP_FontAsset defaultFont;

	[SerializeField]
	private RectTransform mainBoxRect;

	[SerializeField]
	private SettingsUIComponentEventNotifier dropdownEventNotifier;

	[SerializeField]
	private TextMeshProUGUI selectedValueTextField;

	[SerializeField]
	private float heightPerLocale;

	private SupportedLocaleData supportedLocaleData;
}
