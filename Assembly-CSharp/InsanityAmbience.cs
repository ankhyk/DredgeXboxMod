using System;
using UnityEngine;

public class InsanityAmbience : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnGameStartable += this.OnGameStartable;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnGameStartable -= this.OnGameStartable;
	}

	private void OnGameStartable()
	{
		this.playerSanityRef = GameManager.Instance.Player.Sanity;
		this.canUpdate = true;
	}

	private void Update()
	{
		if (!this.canUpdate)
		{
			return;
		}
		this.targetMasterVolumeProp = 1f - Mathf.InverseLerp(this.maxInsanityDrainVal, 0f, Mathf.Min(-0.0001f, this.playerSanityRef.RateOfChange));
		this.thisMasterVolumeProp = Mathf.Lerp(this.thisMasterVolumeProp, this.targetMasterVolumeProp, Time.deltaTime * this.volumeChangeSpeed);
		GameManager.Instance.AudioPlayer.AudioMixer.SetFloat("insanitySfxVolume", Mathf.Log10(this.thisMasterVolumeProp) * this.insanityMixerCoefficient);
		this.currentSanity = this.playerSanityRef.CurrentSanity;
		float num = 1f - this.playerSanityRef.CurrentSanity;
		this.audioSource[0].volume = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0f, 0.25f, num));
		this.audioSource[1].volume = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0f, 0.5f, num));
		this.audioSource[2].volume = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.25f, 0.75f, num));
		this.audioSource[3].volume = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.5f, 1f, num));
	}

	[SerializeField]
	private AudioSource[] audioSource;

	[SerializeField]
	private float insanityMixerCoefficient;

	[SerializeField]
	private float maxInsanityDrainVal;

	[SerializeField]
	private float minInsanityVolumeVal;

	[SerializeField]
	private float volumeChangeSpeed;

	private PlayerSanity playerSanityRef;

	private bool canUpdate;

	private float sanityLost;

	private float prop;

	private float range = 0.25f;

	private float targetMasterVolumeProp;

	private float thisMasterVolumeProp;

	private float currentSanity;
}
