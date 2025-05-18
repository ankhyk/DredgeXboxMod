using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeSettingInput : MonoBehaviour, ISettingsRefreshable
{
	private void Awake()
	{
		this.Refresh();
	}

	public void ForceRefresh()
	{
		this.Refresh();
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.VOLUME);
	}

	private void Refresh()
	{
		float num = 0.5f;
		switch (this.audioLayer)
		{
		case AudioLayer.MASTER:
			num = GameManager.Instance.SettingsSaveData.masterVolume;
			break;
		case AudioLayer.MUSIC:
			num = GameManager.Instance.SettingsSaveData.musicVolume;
			break;
		case AudioLayer.SFX:
		case AudioLayer.SFX_PLAYER:
		case AudioLayer.SFX_WORLD:
			num = GameManager.Instance.SettingsSaveData.sfxVolume;
			break;
		case AudioLayer.SFX_UI:
			num = GameManager.Instance.SettingsSaveData.uiVolume;
			break;
		case AudioLayer.SFX_VOCALS:
			num = GameManager.Instance.SettingsSaveData.voiceVolume;
			break;
		}
		this.slider.SetValueWithoutNotify(num);
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
	}

	private void OnValueChanged(float val)
	{
		if (val <= 0f)
		{
			val = 0.001f;
		}
		switch (this.audioLayer)
		{
		case AudioLayer.MASTER:
			GameManager.Instance.SettingsSaveData.masterVolume = val;
			break;
		case AudioLayer.MUSIC:
			GameManager.Instance.SettingsSaveData.musicVolume = val;
			break;
		case AudioLayer.SFX:
		case AudioLayer.SFX_PLAYER:
		case AudioLayer.SFX_WORLD:
			GameManager.Instance.SettingsSaveData.sfxVolume = val;
			break;
		case AudioLayer.SFX_UI:
			GameManager.Instance.SettingsSaveData.uiVolume = val;
			break;
		case AudioLayer.SFX_VOCALS:
			GameManager.Instance.SettingsSaveData.voiceVolume = val;
			break;
		}
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.VOLUME);
	}

	[SerializeField]
	private AudioLayer audioLayer;

	[SerializeField]
	private Slider slider;
}
