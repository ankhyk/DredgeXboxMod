using System;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioUtil
{
	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, AudioMixerGroup group, AudioRolloffMode mode, float min, float max, bool use2DVolume, bool ignorePause)
	{
		if (clip == null)
		{
			return;
		}
		GameObject gameObject = new GameObject("One shot audio");
		gameObject.transform.position = position;
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		if (group != null)
		{
			audioSource.outputAudioMixerGroup = group;
		}
		audioSource.clip = clip;
		if (use2DVolume)
		{
			Volume2D volume2D = gameObject.AddComponent<Volume2D>();
			volume2D.AttenuationDistance = max;
			volume2D.CloseDist = min;
			volume2D.CloseVolume = volume;
			audioSource.spatialBlend = 0f;
		}
		else
		{
			audioSource.dopplerLevel = 0f;
			audioSource.spatialBlend = 1f;
			audioSource.volume = volume;
			audioSource.rolloffMode = mode;
			audioSource.minDistance = min;
			audioSource.maxDistance = max;
		}
		audioSource.ignoreListenerPause = ignorePause;
		audioSource.Play();
		global::UnityEngine.Object.Destroy(gameObject, clip.length);
	}
}
