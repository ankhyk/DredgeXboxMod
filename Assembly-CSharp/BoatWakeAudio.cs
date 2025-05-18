using System;
using UnityEngine;

public class BoatWakeAudio : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource.volume = this.minVolume;
	}

	private void Update()
	{
		float num = Mathf.Clamp01(this.playerControllerRef.Velocity.magnitude * this.velocityModifier);
		this.audioSource.volume = Mathf.Lerp(this.minVolume, this.maxVolume, num);
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private PlayerController playerControllerRef;

	[SerializeField]
	private float velocityModifier;

	[SerializeField]
	private float minVolume;

	[SerializeField]
	private float maxVolume;
}
