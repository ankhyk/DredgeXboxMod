using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class DropdownSettingInput : MonoBehaviour, ISettingsRefreshable
{
	public int CurrentIndex
	{
		get
		{
			return this.currentIndex;
		}
		set
		{
			this.currentIndex = value;
		}
	}

	private void Awake()
	{
		LocalizedString localizedString = this.localizedString;
		LocalizedString localizedString2 = this.tooltipDescriptionString;
		if (this.SKUSpecificLocalizedTitleString)
		{
			LocalizedString stringOverride = this.SKUSpecificLocalizedTitleString.GetStringOverride();
			if (stringOverride != null)
			{
				localizedString = stringOverride;
			}
		}
		if (this.SKUSpecificLocalizedDescriptionString)
		{
			LocalizedString stringOverride2 = this.SKUSpecificLocalizedDescriptionString.GetStringOverride();
			if (stringOverride2 != null)
			{
				localizedString2 = stringOverride2;
			}
		}
		this.localizedStringField.StringReference = localizedString;
		if (localizedString2.IsEmpty)
		{
			this.textTooltipRequester.enabled = false;
		}
		else
		{
			this.textTooltipRequester.LocalizedTitleKey = localizedString;
			this.textTooltipRequester.LocalizedDescriptionKey = localizedString2;
			this.textTooltipRequester.enabled = true;
		}
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnValueChanged));
	}

	private void OnComponentSubmitted()
	{
		if (this.scrollToSelectedItem)
		{
			List<ScrollRectMagnet> list = base.transform.GetComponentsInChildren<ScrollRectMagnet>().ToList<ScrollRectMagnet>();
			if (this.currentIndex < list.Count)
			{
				list[this.currentIndex].ScrollToThis();
			}
		}
	}

	private void OnEnable()
	{
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Combine(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
		ApplicationEvents.Instance.OnLocaleChanged += this.OnLocaleChanged;
		this.RefreshDropdown();
	}

	private void OnDisable()
	{
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Remove(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
		ApplicationEvents.Instance.OnLocaleChanged -= this.OnLocaleChanged;
	}

	private void OnLocaleChanged(Locale l)
	{
		this.RefreshDropdownStrings();
	}

	public void RefreshDropdownStrings()
	{
		if (this.populateOptions)
		{
			this.dropdown.options.Clear();
			this.optionStrings.ForEach(delegate(LocalizedString str)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
				optionData.text = LocalizationSettings.StringDatabase.GetLocalizedString(str.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
				this.dropdown.options.Add(optionData);
			});
			this.selectedValueTextField.text = LocalizationSettings.StringDatabase.GetLocalizedString(this.optionStrings[this.currentIndex].TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		}
	}

	private void RefreshDropdown()
	{
		this.RefreshDropdownStrings();
		if (this.retrieveSelectedIndex)
		{
			SettingType settingType = this.settingType;
			switch (settingType)
			{
			case SettingType.CAMERA_FREELOOK:
				this.currentIndex = GameManager.Instance.SettingsSaveData.cameraFreelook;
				break;
			case SettingType.CAMERA_RECENTER:
				this.currentIndex = GameManager.Instance.SettingsSaveData.cameraRecenter;
				break;
			case SettingType.CAMERA_INVERT_X:
				this.currentIndex = GameManager.Instance.SettingsSaveData.cameraInvertX;
				break;
			case SettingType.CAMERA_INVERT_Y:
				this.currentIndex = GameManager.Instance.SettingsSaveData.cameraInvertY;
				break;
			case SettingType.CAMERA_SENSITIVITY_X:
			case SettingType.CAMERA_SENSITIVITY_Y:
				break;
			case SettingType.CAMERA_FOLLOW_MODE:
				this.currentIndex = GameManager.Instance.SettingsSaveData.cameraFollow;
				break;
			default:
				switch (settingType)
				{
				case SettingType.RADIAL_TRIGGER_MODE:
					this.currentIndex = GameManager.Instance.SettingsSaveData.radialTriggerMode;
					break;
				case SettingType.NO_FAIL_BEHAVIOUR:
					this.currentIndex = GameManager.Instance.SettingsSaveData.noFailBehaviour;
					break;
				case SettingType.TURNING_DEADZONE_X:
				case SettingType.MOTION_SICKNESS_AMOUNT:
				case SettingType.LANGUAGE:
					break;
				case SettingType.CONSTRAIN_CURSOR:
					this.currentIndex = GameManager.Instance.SettingsSaveData.constrainCursor;
					break;
				case SettingType.PANIC_VISUALS:
					this.currentIndex = GameManager.Instance.SettingsSaveData.panicVisuals;
					break;
				case SettingType.NOTIFICATION_DURATION:
					this.currentIndex = GameManager.Instance.SettingsSaveData.notificationDuration;
					break;
				case SettingType.TEXT_SPEED:
					this.currentIndex = GameManager.Instance.SettingsSaveData.textSpeed;
					break;
				case SettingType.HASTE_VFX:
					this.currentIndex = GameManager.Instance.SettingsSaveData.hasteVFX;
					break;
				case SettingType.GAME_MODE:
					this.currentIndex = GameManager.Instance.SettingsSaveData.gameMode;
					break;
				case SettingType.TUTORIALS:
					this.currentIndex = GameManager.Instance.SettingsSaveData.tutorials;
					break;
				case SettingType.UNITS:
					this.currentIndex = GameManager.Instance.SettingsSaveData.units;
					break;
				case SettingType.CLOCK_STYLE:
					this.currentIndex = GameManager.Instance.SettingsSaveData.clockStyle;
					break;
				case SettingType.ANTI_ALIASING:
					this.currentIndex = GameManager.Instance.SettingsSaveData.antiAliasing;
					break;
				case SettingType.SHADOW_QUALITY:
					this.currentIndex = GameManager.Instance.SettingsSaveData.shadowQuality;
					break;
				case SettingType.REFLECTIONS:
					this.currentIndex = GameManager.Instance.SettingsSaveData.reflections;
					break;
				case SettingType.PAUSE_ON_FOCUS_LOSS:
					this.currentIndex = GameManager.Instance.SettingsSaveData.pauseOnFocusLoss;
					break;
				case SettingType.VSYNC:
					this.currentIndex = GameManager.Instance.SettingsSaveData.vsync;
					break;
				case SettingType.CONTROL_ICON_STYLE:
					this.currentIndex = GameManager.Instance.SettingsSaveData.controlIconStyle;
					break;
				case SettingType.TITLE_THEME:
					this.currentIndex = GameManager.Instance.SettingsSaveData.titleTheme;
					break;
				default:
					if (settingType == SettingType.HEARTBEAT_SFX)
					{
						this.currentIndex = GameManager.Instance.SettingsSaveData.heartbeatSFX;
					}
					break;
				}
				break;
			}
			this.dropdown.SetValueWithoutNotify(this.currentIndex);
		}
	}

	private void OnValueChanged(int value)
	{
		this.currentIndex = value;
		SettingType settingType = this.settingType;
		switch (settingType)
		{
		case SettingType.CAMERA_FREELOOK:
			GameManager.Instance.SettingsSaveData.cameraFreelook = value;
			break;
		case SettingType.CAMERA_RECENTER:
			GameManager.Instance.SettingsSaveData.cameraRecenter = value;
			break;
		case SettingType.CAMERA_INVERT_X:
			GameManager.Instance.SettingsSaveData.cameraInvertX = value;
			break;
		case SettingType.CAMERA_INVERT_Y:
			GameManager.Instance.SettingsSaveData.cameraInvertY = value;
			break;
		case SettingType.CAMERA_SENSITIVITY_X:
		case SettingType.CAMERA_SENSITIVITY_Y:
		case (SettingType)8:
		case SettingType.CAMERA_SHAKE_AMOUNT:
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_X:
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_Y:
		case (SettingType)12:
		case (SettingType)13:
		case (SettingType)14:
		case (SettingType)15:
		case (SettingType)16:
		case (SettingType)17:
		case (SettingType)18:
		case (SettingType)19:
		case (SettingType)20:
		case (SettingType)29:
		case (SettingType)30:
		case SettingType.TURNING_DEADZONE_X:
		case SettingType.MOTION_SICKNESS_AMOUNT:
		case SettingType.LANGUAGE:
			break;
		case SettingType.CAMERA_FOLLOW_MODE:
			GameManager.Instance.SettingsSaveData.cameraFollow = value;
			break;
		case SettingType.COLOR_NEUTRAL:
			GameManager.Instance.SettingsSaveData.colorNeutral = value;
			break;
		case SettingType.COLOR_EMPHASIS:
			GameManager.Instance.SettingsSaveData.colorEmphasis = value;
			break;
		case SettingType.COLOR_POSITIVE:
			GameManager.Instance.SettingsSaveData.colorPositive = value;
			break;
		case SettingType.COLOR_NEGATIVE:
			GameManager.Instance.SettingsSaveData.colorNegative = value;
			break;
		case SettingType.COLOR_CRITICAL:
			GameManager.Instance.SettingsSaveData.colorCritical = value;
			break;
		case SettingType.COLOR_WARNING:
			GameManager.Instance.SettingsSaveData.colorWarning = value;
			break;
		case SettingType.COLOR_VALUABLE:
			GameManager.Instance.SettingsSaveData.colorValuable = value;
			break;
		case SettingType.COLOR_DISABLED:
			GameManager.Instance.SettingsSaveData.colorDisabled = value;
			break;
		case SettingType.RADIAL_TRIGGER_MODE:
			GameManager.Instance.SettingsSaveData.radialTriggerMode = value;
			break;
		case SettingType.NO_FAIL_BEHAVIOUR:
			GameManager.Instance.SettingsSaveData.noFailBehaviour = value;
			break;
		case SettingType.CONSTRAIN_CURSOR:
			GameManager.Instance.SettingsSaveData.constrainCursor = value;
			break;
		case SettingType.PANIC_VISUALS:
			GameManager.Instance.SettingsSaveData.panicVisuals = value;
			break;
		case SettingType.NOTIFICATION_DURATION:
			GameManager.Instance.SettingsSaveData.notificationDuration = value;
			break;
		case SettingType.TEXT_SPEED:
			GameManager.Instance.SettingsSaveData.textSpeed = value;
			break;
		case SettingType.HASTE_VFX:
			GameManager.Instance.SettingsSaveData.hasteVFX = value;
			break;
		case SettingType.GAME_MODE:
			GameManager.Instance.SettingsSaveData.gameMode = value;
			break;
		case SettingType.TUTORIALS:
			GameManager.Instance.SettingsSaveData.tutorials = value;
			break;
		case SettingType.UNITS:
			GameManager.Instance.SettingsSaveData.units = value;
			break;
		case SettingType.CLOCK_STYLE:
			GameManager.Instance.SettingsSaveData.clockStyle = value;
			break;
		case SettingType.ANTI_ALIASING:
			GameManager.Instance.SettingsSaveData.antiAliasing = value;
			break;
		case SettingType.SHADOW_QUALITY:
			GameManager.Instance.SettingsSaveData.shadowQuality = value;
			break;
		case SettingType.REFLECTIONS:
			GameManager.Instance.SettingsSaveData.reflections = value;
			break;
		case SettingType.PAUSE_ON_FOCUS_LOSS:
			GameManager.Instance.SettingsSaveData.pauseOnFocusLoss = value;
			break;
		case SettingType.VSYNC:
			GameManager.Instance.SettingsSaveData.vsync = value;
			break;
		case SettingType.CONTROL_ICON_STYLE:
			GameManager.Instance.SettingsSaveData.controlIconStyle = value;
			break;
		case SettingType.TITLE_THEME:
			GameManager.Instance.SettingsSaveData.titleTheme = value;
			GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme = true;
			break;
		default:
			if (settingType == SettingType.HEARTBEAT_SFX)
			{
				GameManager.Instance.SettingsSaveData.heartbeatSFX = value;
			}
			break;
		}
		ApplicationEvents.Instance.TriggerSettingChanged(this.settingType);
	}

	public void ForceRefresh()
	{
		ApplicationEvents.Instance.TriggerSettingChanged(this.settingType);
		this.RefreshDropdown();
	}

	[SerializeField]
	public SettingType settingType;

	[SerializeField]
	private TextMeshProUGUI selectedValueTextField;

	[SerializeField]
	private LocalizedString localizedString;

	[SerializeField]
	private LocalizedString tooltipDescriptionString;

	[SerializeField]
	private SKUSpecificLocalizedString SKUSpecificLocalizedTitleString;

	[SerializeField]
	private SKUSpecificLocalizedString SKUSpecificLocalizedDescriptionString;

	[SerializeField]
	private LocalizeStringEvent localizedStringField;

	[SerializeField]
	private TextTooltipRequester textTooltipRequester;

	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private SettingsUIComponentEventNotifier dropdownEventNotifier;

	[SerializeField]
	private List<LocalizedString> optionStrings;

	[SerializeField]
	private bool populateOptions = true;

	[SerializeField]
	private bool retrieveSelectedIndex = true;

	[SerializeField]
	private bool scrollToSelectedItem = true;

	private int currentIndex;
}
