using System;
using UnityEngine;

public class HeavyWavesAudio : MonoBehaviour
{
	private void Update()
	{
		this.audioSource.volume = Mathf.Lerp(this.minVolume, this.maxVolume, Mathf.InverseLerp(this.minSteepness, this.maxSteepness, this.buoyantObject.CurrentWaveSteepness));
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private BuoyantObject buoyantObject;

	[SerializeField]
	private float minSteepness;

	[SerializeField]
	private float maxSteepness = 1f;

	[SerializeField]
	private float minVolume;

	[SerializeField]
	private float maxVolume = 1f;
}
