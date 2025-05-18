using System;
using UnityEngine;
using XGamingRuntime;

[CreateAssetMenu(fileName = "SettingsSaveDataTemplate", menuName = "Dredge/SettingsSaveDataTemplate", order = 0)]
public class SettingsSaveDataTemplate : ScriptableObject
{
	public SettingsSaveData GetData()
	{
		SettingsSaveData settingsSaveData = new SettingsSaveData();
		settingsSaveData.cameraSensitivityX = this.cameraSensitivityX;
		settingsSaveData.cameraSensitivityY = this.cameraSensitivityY;
		settingsSaveData.spyglassCameraSensitivityX = this.spyglassCameraSensitivityX;
		settingsSaveData.spyglassCameraSensitivityY = this.spyglassCameraSensitivityY;
		settingsSaveData.cameraInvertX = this.cameraInvertX;
		settingsSaveData.cameraInvertY = this.cameraInvertY;
		settingsSaveData.cameraFreelook = this.cameraFreelook;
		settingsSaveData.cameraRecenter = this.cameraRecenter;
		settingsSaveData.cameraShakeAmount = this.cameraShakeAmount;
		settingsSaveData.colorNeutral = this.colorNeutral;
		settingsSaveData.colorEmphasis = this.colorEmphasis;
		settingsSaveData.colorPositive = this.colorPositive;
		settingsSaveData.colorNegative = this.colorNegative;
		settingsSaveData.colorCritical = this.colorCritical;
		settingsSaveData.colorWarning = this.colorWarning;
		settingsSaveData.colorValuable = this.colorValuable;
		settingsSaveData.colorDisabled = this.colorDisabled;
		settingsSaveData.radialTriggerMode = this.radialBehaviour;
		settingsSaveData.noFailBehaviour = this.noFailBehaviour;
		settingsSaveData.turningDeadzoneX = this.turningDeadzoneX;
		settingsSaveData.motionSicknessAmount = this.motionSicknessAmount;
		settingsSaveData.constrainCursor = this.constrainCursor;
		settingsSaveData.panicVisuals = this.panicVisuals;
		settingsSaveData.notificationDuration = this.notificationDuration;
		settingsSaveData.textSpeed = this.textSpeed;
		settingsSaveData.tutorials = this.tutorials;
		settingsSaveData.antiAliasing = this.antiAliasing;
		settingsSaveData.units = this.units;
		settingsSaveData.clockStyle = this.clockStyle;
		settingsSaveData.shadowQuality = this.shadowQuality;
		settingsSaveData.reflections = this.reflections;
		settingsSaveData.vsync = this.vsync;
		settingsSaveData.controlIconStyle = this.controlIconStyle;
		settingsSaveData.masterVolume = this.masterVolume;
		settingsSaveData.musicVolume = this.musicVolume;
		settingsSaveData.sfxVolume = this.sfxVolume;
		settingsSaveData.uiVolume = this.uiVolume;
		settingsSaveData.voiceVolume = this.voiceVolume;
		string defaultLanguage = this.GetDefaultLanguage();
		settingsSaveData.localeId = defaultLanguage;
		return settingsSaveData;
	}

	public string GetDefaultLanguage()
	{
		string text;
		int num = SDK.XPackageGetUserLocale(out text);
		if (num == 0)
		{
			Debug.Log("GDKPC: " + text);
			uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num2 <= 1664981344U)
			{
				if (num2 <= 637978675U)
				{
					if (num2 != 83303646U)
					{
						if (num2 != 376747596U)
						{
							if (num2 == 637978675U)
							{
								if (text == "zh-CN")
								{
									return "zh-Hans";
								}
							}
						}
						else if (text == "fr-FR")
						{
							return "fr";
						}
					}
					else if (text == "ru-RU")
					{
						return "ru";
					}
				}
				else if (num2 != 1162757945U)
				{
					if (num2 != 1434653370U)
					{
						if (num2 == 1664981344U)
						{
							if (text == "pt-BR")
							{
								return "pt-BR";
							}
						}
					}
					else if (text == "it-IT")
					{
						return "it";
					}
				}
				else if (text == "pl")
				{
					return "pl";
				}
			}
			else if (num2 <= 2328506441U)
			{
				if (num2 != 2194893224U)
				{
					if (num2 != 2196609786U)
					{
						if (num2 == 2328506441U)
						{
							if (text == "ja-JP")
							{
								return "ja-JP";
							}
						}
					}
					else if (text == "de-DE")
					{
						return "de";
					}
				}
				else if (text == "es-ES")
				{
					return "es";
				}
			}
			else if (num2 != 2586248143U)
			{
				if (num2 != 2698194840U)
				{
					if (num2 == 3973517379U)
					{
						if (text == "zh-TW")
						{
							return "zh-Hant";
						}
					}
				}
				else if (text == "pl-PL")
				{
					return "pl";
				}
			}
			else if (text == "ko-KR")
			{
				return "ko-KR";
			}
			return "en";
		}
		Debug.LogWarning(string.Format("Failed to get user locale GDK: Error {0}", num));
		return string.Empty;
	}

	[Header("Camera")]
	[Range(0f, 1f)]
	[SerializeField]
	public float cameraSensitivityX;

	[Range(0f, 1f)]
	[SerializeField]
	public float cameraSensitivityY;

	[Range(0f, 1f)]
	[SerializeField]
	public float cameraShakeAmount;

	[SerializeField]
	public int cameraInvertX;

	[SerializeField]
	public int cameraInvertY;

	[SerializeField]
	public int cameraFreelook;

	[SerializeField]
	public int cameraRecenter;

	[Range(0.05f, 1f)]
	[SerializeField]
	public float spyglassCameraSensitivityX;

	[Range(0.05f, 1f)]
	[SerializeField]
	public float spyglassCameraSensitivityY;

	[Header("Accessibility")]
	[SerializeField]
	public int colorNeutral;

	[SerializeField]
	public int colorEmphasis;

	[SerializeField]
	public int colorPositive;

	[SerializeField]
	public int colorNegative;

	[SerializeField]
	public int colorCritical;

	[SerializeField]
	public int colorWarning;

	[SerializeField]
	public int colorValuable;

	[SerializeField]
	public int colorDisabled;

	[SerializeField]
	public int radialBehaviour;

	[SerializeField]
	public int noFailBehaviour;

	[Range(0f, 1f)]
	[SerializeField]
	public float turningDeadzoneX;

	[Range(0f, 1f)]
	[SerializeField]
	public float motionSicknessAmount;

	[SerializeField]
	public int constrainCursor;

	[SerializeField]
	public int panicVisuals;

	[SerializeField]
	public int notificationDuration;

	[SerializeField]
	public int textSpeed;

	[Header("Display")]
	[SerializeField]
	public int tutorials;

	[SerializeField]
	public int antiAliasing;

	[SerializeField]
	public int units;

	[SerializeField]
	public int clockStyle;

	[SerializeField]
	public int shadowQuality;

	[SerializeField]
	public int reflections;

	[SerializeField]
	public int vsync;

	[SerializeField]
	public int controlIconStyle;

	[Header("Audio")]
	[Range(0f, 1f)]
	[SerializeField]
	public float masterVolume;

	[Range(0f, 1f)]
	[SerializeField]
	public float musicVolume;

	[Range(0f, 1f)]
	[SerializeField]
	public float sfxVolume;

	[Range(0f, 1f)]
	[SerializeField]
	public float uiVolume;

	[Range(0f, 1f)]
	[SerializeField]
	public float voiceVolume;
}
