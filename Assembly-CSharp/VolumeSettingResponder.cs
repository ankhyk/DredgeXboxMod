using System;
using UnityEngine;

public class VolumeSettingResponder : MonoBehaviour
{
	private void Awake()
	{
		if (GameManager.Instance.AudioPlayer.IsMixerLoaded)
		{
			this.OnMixerLoaded();
			return;
		}
		AudioPlayer audioPlayer = GameManager.Instance.AudioPlayer;
		audioPlayer.OnMixerLoaded = (Action)Delegate.Combine(audioPlayer.OnMixerLoaded, new Action(this.OnMixerLoaded));
	}

	private void OnDisable()
	{
		AudioPlayer audioPlayer = GameManager.Instance.AudioPlayer;
		audioPlayer.OnMixerLoaded = (Action)Delegate.Remove(audioPlayer.OnMixerLoaded, new Action(this.OnMixerLoaded));
	}

	private void OnMixerLoaded()
	{
		AudioPlayer audioPlayer = GameManager.Instance.AudioPlayer;
		audioPlayer.OnMixerLoaded = (Action)Delegate.Remove(audioPlayer.OnMixerLoaded, new Action(this.OnMixerLoaded));
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		ApplicationEvents.Instance.OnSaveManagerInitialized += this.Refresh;
		this.Refresh();
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnSaveManagerInitialized -= this.Refresh;
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.VOLUME)
		{
			this.Refresh();
		}
	}

	private void Refresh()
	{
		if (!GameManager.Instance.AudioPlayer.IsMixerLoaded)
		{
			Debug.LogWarning("[VolumeSettingResponder] Refresh() AudioMixer has not loaded yet.");
			return;
		}
		if (GameManager.Instance.SettingsSaveData != null)
		{
			this.SetVolume("masterVolume", GameManager.Instance.SettingsSaveData.masterVolume);
			this.SetVolume("musicVolume", GameManager.Instance.SettingsSaveData.musicVolume);
			this.SetVolume("sfxVolume", GameManager.Instance.SettingsSaveData.sfxVolume);
			this.SetVolume("uiVolume", GameManager.Instance.SettingsSaveData.uiVolume);
			this.SetVolume("voiceVolume", GameManager.Instance.SettingsSaveData.voiceVolume);
			return;
		}
		this.SetVolume("masterVolume", 0f);
		this.SetVolume("musicVolume", 0f);
		this.SetVolume("sfxVolume", 0f);
		this.SetVolume("uiVolume", 0f);
		this.SetVolume("voiceVolume", 0f);
	}

	private void SetVolume(string key, float volume)
	{
		GameManager.Instance.AudioPlayer.AudioMixer.SetFloat(key, Mathf.Log10(volume) * 20f);
	}
}
