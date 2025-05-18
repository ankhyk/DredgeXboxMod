using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LanguageManager : MonoBehaviour
{
	public SupportedLocaleData SupportedLocaleData
	{
		get
		{
			return this.supportedLocaleData;
		}
	}

	public bool IsInit
	{
		get
		{
			return this.isInit;
		}
		set
		{
			this.isInit = value;
		}
	}

	private void Awake()
	{
		if (GameManager.Instance.SettingsSaveData != null)
		{
			this.Init();
			return;
		}
		ApplicationEvents.Instance.OnSaveManagerInitialized += this.Init;
	}

	private void Init()
	{
		ApplicationEvents.Instance.OnSaveManagerInitialized -= this.Init;
		if (!string.IsNullOrEmpty(GameManager.Instance.SettingsSaveData.localeId))
		{
			this.SetLocale(GameManager.Instance.SettingsSaveData.localeId);
		}
		Addressables.LoadAssetAsync<SupportedLocaleData>(this.supportedLocaleDataRef).Completed += delegate(AsyncOperationHandle<SupportedLocaleData> res)
		{
			if (res.Status == AsyncOperationStatus.Succeeded)
			{
				this.supportedLocaleData = res.Result;
			}
		};
		this.RefreshColors();
		this.IsInit = true;
		Action onInit = this.OnInit;
		if (onInit == null)
		{
			return;
		}
		onInit();
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnLocaleChanged += this.OnLocaleChanged;
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnLocaleChanged -= this.OnLocaleChanged;
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.COLOR_EMPHASIS || settingType == SettingType.COLOR_POSITIVE || settingType == SettingType.COLOR_NEGATIVE || settingType == SettingType.COLOR_CRITICAL)
		{
			this.RefreshColors();
		}
	}

	public void OnLocaleChanged(Locale locale)
	{
		LocalizationSettings.SelectedLocale = locale;
		GameManager.Instance.SettingsSaveData.localeId = locale.Identifier.Code;
	}

	public void SetLocale(string identifierCode)
	{
		if (!LocalizationSettings.InitializationOperation.IsDone)
		{
			this.queuedLocaleChange = identifierCode;
			LocalizationSettings.InitializationOperation.Completed += this.OnLocalizationInitializationCompleted;
			return;
		}
		Locale locale = LocalizationSettings.AvailableLocales.Locales.Find((Locale l) => l.Identifier.Code == identifierCode);
		if (locale)
		{
			this.OnLocaleChanged(locale);
		}
	}

	public Locale GetLocale()
	{
		return LocalizationSettings.SelectedLocale;
	}

	private void OnLocalizationInitializationCompleted(AsyncOperationHandle<LocalizationSettings> handle)
	{
		this.SetLocale(this.queuedLocaleChange);
		this.queuedLocaleChange = "";
	}

	public void FormatTimeStringForDurability(float currentDurabilityDays, float maxDurabilityDays, LocalizeStringEvent stringField, string localizationKey)
	{
		if (currentDurabilityDays <= 0f)
		{
			stringField.StringReference.SetReference(LanguageManager.STRING_TABLE, "tooltip.deployable.durability-broken");
			return;
		}
		currentDurabilityDays *= 1f + GameManager.Instance.PlayerStats.ResearchedEquipmentMaintenanceModifier;
		int num = Mathf.RoundToInt(currentDurabilityDays);
		int num2 = Mathf.RoundToInt(currentDurabilityDays * 24f);
		int num3;
		LocalizedString localizedString;
		if (num < 1)
		{
			num3 = num2;
			localizedString = this.localizedHourString;
		}
		else
		{
			num3 = num;
			localizedString = this.localizedDayString;
		}
		num3 = Mathf.Max(num3, 1);
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedString.TableEntryReference, null, FallbackBehavior.UseProjectSettings, new object[] { num3 }).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded && op.Status == AsyncOperationStatus.Succeeded)
			{
				stringField.enabled = false;
				stringField.StringReference.SetReference(LanguageManager.STRING_TABLE, localizationKey);
				stringField.StringReference.Arguments = new object[] { op.Result };
				stringField.RefreshString();
				stringField.enabled = true;
			}
		};
	}

	public string FormatControlPromptString(LocalizedString localizedString, List<DredgeControlEnum> controls)
	{
		List<string> arguments = new List<string>();
		controls.ForEach(delegate(DredgeControlEnum dce)
		{
			PlayerAction playerAction = GameManager.Instance.Input.Controls.GetPlayerAction(dce);
			ControlIconData controlIconForActionWithDefault = GameManager.Instance.Input.GetControlIconForActionWithDefault(playerAction);
			arguments.Add(" <sprite name=\"" + controlIconForActionWithDefault.upSpriteName + "\"> ");
		});
		LocalizedStringDatabase stringDatabase = LocalizationSettings.StringDatabase;
		TableEntryReference tableEntryReference = localizedString.TableEntryReference;
		Locale selectedLocale = LocalizationSettings.SelectedLocale;
		FallbackBehavior fallbackBehavior = FallbackBehavior.UseProjectSettings;
		object[] array = arguments.ToArray();
		return stringDatabase.GetLocalizedString(tableEntryReference, selectedLocale, fallbackBehavior, array);
	}

	public string FormatDistanceString(float distance)
	{
		bool flag = true;
		if (GameManager.Instance)
		{
			flag = GameManager.Instance.SettingsSaveData.units == 0;
		}
		if (flag)
		{
			double num = Math.Round((double)distance, 1);
			return string.Format("{0} m", num);
		}
		int num2 = (int)(distance * 3.281f);
		return string.Format("{0} ft", num2);
	}

	public string FormatLongDistanceString(float unityUnits)
	{
		bool flag = true;
		float num = unityUnits * 20f;
		if (GameManager.Instance)
		{
			flag = GameManager.Instance.SettingsSaveData.units == 0;
		}
		if (flag)
		{
			double num2 = Math.Round((double)num * 0.001, 1);
			return string.Format("{0} km", num2);
		}
		double num3 = Math.Round((double)(num * 0.000621371f), 1);
		return string.Format("{0} mi", num3);
	}

	public Color GetColor(DredgeColorTypeEnum colorType)
	{
		return this.colors[(int)colorType];
	}

	public string GetColorCode(DredgeColorTypeEnum colorType)
	{
		return this.colorCodes[(int)colorType];
	}

	public void RefreshColors()
	{
		this.colors = new List<Color>();
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorNeutral]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorEmphasis]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorPositive]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorNegative]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorCritical]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorWarning]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorValuable]);
		this.colors.Add(GameManager.Instance.GameConfigData.Colors[GameManager.Instance.SettingsSaveData.colorDisabled]);
		this.colorCodes = new List<string>();
		this.colors.ForEach(delegate(Color c)
		{
			this.colorCodes.Add(ColorUtility.ToHtmlStringRGB(c));
		});
	}

	public DredgeColorTypeEnum GetColorTypeEnumFromSettingType(SettingType settingType)
	{
		switch (settingType)
		{
		case SettingType.COLOR_NEUTRAL:
			return DredgeColorTypeEnum.NEUTRAL;
		case SettingType.COLOR_EMPHASIS:
			return DredgeColorTypeEnum.EMPHASIS;
		case SettingType.COLOR_POSITIVE:
			return DredgeColorTypeEnum.POSITIVE;
		case SettingType.COLOR_NEGATIVE:
			return DredgeColorTypeEnum.NEGATIVE;
		case SettingType.COLOR_CRITICAL:
			return DredgeColorTypeEnum.CRITICAL;
		case SettingType.COLOR_WARNING:
			return DredgeColorTypeEnum.WARNING;
		case SettingType.COLOR_VALUABLE:
			return DredgeColorTypeEnum.VALUABLE;
		case SettingType.COLOR_DISABLED:
			return DredgeColorTypeEnum.DISABLED;
		default:
			return DredgeColorTypeEnum.NEUTRAL;
		}
	}

	public static string STRING_TABLE = "Strings";

	public static string YARN_TABLE = "Yarn";

	public static string CHARACTER_TABLE = "Characters";

	public static string ITEM_TABLE = "Items";

	private string queuedLocaleChange = "";

	public List<Color> colors;

	private List<string> colorCodes;

	[SerializeField]
	private LocalizedString localizedHourString;

	[SerializeField]
	private LocalizedString localizedDayString;

	[SerializeField]
	private AssetReference supportedLocaleDataRef;

	private SupportedLocaleData supportedLocaleData;

	private bool isInit;

	public Action OnInit;
}
