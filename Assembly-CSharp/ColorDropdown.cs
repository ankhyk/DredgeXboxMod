using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ColorDropdown : MonoBehaviour, ISettingsRefreshable
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnLocaleChanged += this.OnLocaleChanged;
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Combine(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
		this.RefreshDropdown();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnLocaleChanged -= this.OnLocaleChanged;
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Remove(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
	}

	private void OnLocaleChanged(Locale l)
	{
		this.RefreshDropdown();
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == this.dropdownSettingInput.settingType)
		{
			this.RefreshLabelsColor();
		}
	}

	public void ForceRefresh()
	{
		this.RefreshDropdown();
	}

	private void RefreshDropdown()
	{
		List<string> list = new List<string>();
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(this.labelLocalizedString.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		this.dropdown.ClearOptions();
		for (int i = 0; i < GameManager.Instance.GameConfigData.Colors.Count; i++)
		{
			string text = localizedString;
			list.Add(text);
		}
		this.dropdown.AddOptions(list);
		int num = 0;
		switch (this.dropdownSettingInput.settingType)
		{
		case SettingType.COLOR_NEUTRAL:
			num = GameManager.Instance.SettingsSaveData.colorNeutral;
			this.colorType = DredgeColorTypeEnum.NEUTRAL;
			break;
		case SettingType.COLOR_EMPHASIS:
			num = GameManager.Instance.SettingsSaveData.colorEmphasis;
			this.colorType = DredgeColorTypeEnum.EMPHASIS;
			break;
		case SettingType.COLOR_POSITIVE:
			num = GameManager.Instance.SettingsSaveData.colorPositive;
			this.colorType = DredgeColorTypeEnum.POSITIVE;
			break;
		case SettingType.COLOR_NEGATIVE:
			num = GameManager.Instance.SettingsSaveData.colorNegative;
			this.colorType = DredgeColorTypeEnum.NEGATIVE;
			break;
		case SettingType.COLOR_CRITICAL:
			num = GameManager.Instance.SettingsSaveData.colorCritical;
			this.colorType = DredgeColorTypeEnum.CRITICAL;
			break;
		case SettingType.COLOR_WARNING:
			num = GameManager.Instance.SettingsSaveData.colorWarning;
			this.colorType = DredgeColorTypeEnum.WARNING;
			break;
		case SettingType.COLOR_VALUABLE:
			num = GameManager.Instance.SettingsSaveData.colorValuable;
			this.colorType = DredgeColorTypeEnum.VALUABLE;
			break;
		case SettingType.COLOR_DISABLED:
			num = GameManager.Instance.SettingsSaveData.colorDisabled;
			this.colorType = DredgeColorTypeEnum.DISABLED;
			break;
		}
		this.dropdownSettingInput.CurrentIndex = num;
		this.dropdown.SetValueWithoutNotify(num);
		this.RefreshLabelsColor();
	}

	private void RefreshLabelsColor()
	{
		Color color = GameManager.Instance.LanguageManager.GetColor(this.colorType);
		this.textField.color = color;
	}

	private void OnComponentSubmitted()
	{
		List<DropdownItem> list = base.transform.GetComponentsInChildren<DropdownItem>().ToList<DropdownItem>();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].background.color = GameManager.Instance.GameConfigData.Colors[i];
			bool flag = i < this.columns;
			bool flag2 = (float)i > Mathf.Ceil((float)(list.Count / this.columns)) * (float)this.columns;
			Selectable toggle = list[i].toggle;
			Navigation navigation = toggle.navigation;
			if (!flag2 && i + this.columns < list.Count)
			{
				Selectable toggle2 = list[i + this.columns].toggle;
				navigation.selectOnDown = toggle2;
			}
			if (!flag && i - this.columns >= 0)
			{
				Selectable toggle3 = list[i - this.columns].toggle;
				navigation.selectOnUp = toggle3;
			}
			toggle.navigation = navigation;
		}
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private SettingsUIComponentEventNotifier dropdownEventNotifier;

	[SerializeField]
	private LocalizedString labelLocalizedString;

	[SerializeField]
	private int columns;

	private DredgeColorTypeEnum colorType;
}
