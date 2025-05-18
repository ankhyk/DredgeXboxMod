using System;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
	private void Bark()
	{
		this.audioSource.clip = this.barkClips.PickRandom<AudioClip>();
		this.audioSource.Play();
	}

	[SerializeField]
	private List<AudioClip> barkClips;

	[SerializeField]
	private AudioSource audioSource;
}
