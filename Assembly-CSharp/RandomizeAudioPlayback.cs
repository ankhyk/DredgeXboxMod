using System;
using UnityEngine;

public class RandomizeAudioPlayback : MonoBehaviour
{
	public void Awake()
	{
		if (this.audioSource.clip)
		{
			this.audioSource.time = global::UnityEngine.Random.value * this.audioSource.clip.length;
		}
	}

	[SerializeField]
	private AudioSource audioSource;
}
