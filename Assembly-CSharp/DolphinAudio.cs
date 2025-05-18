using System;
using System.Collections.Generic;
using UnityEngine;

public class DolphinAudio : MonoBehaviour
{
	private void PlayJumpSFX()
	{
		this.audioSource.clip = this.jumpAudioClips.PickRandom<AudioClip>();
		this.audioSource.pitch = global::UnityEngine.Random.Range(0.95f, 1.05f);
		this.audioSource.Play();
	}

	[SerializeField]
	private List<AudioClip> jumpAudioClips;

	[SerializeField]
	private AudioSource audioSource;
}
