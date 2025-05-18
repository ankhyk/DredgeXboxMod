using System;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class ExplodingMushrooms : MonoBehaviour
{
	private void Awake()
	{
		this.growAudioSource.pitch = global::UnityEngine.Random.Range(this.pitchMin, this.pitchMax);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && GameManager.Instance.Player.Sanity.CurrentSanity < this.sanityThreshold)
		{
			this.growAudioSource.Play();
			this.mushroomAnimator.SetTrigger("Explode");
		}
	}

	private void OnExplode()
	{
		AudioSource component = global::UnityEngine.Object.Instantiate<GameObject>(this.explodeVFX, base.transform.position, base.transform.rotation).GetComponent<AudioSource>();
		if (component)
		{
			component.pitch = this.growAudioSource.pitch;
		}
		GameManager.Instance.VibrationManager.Vibrate(this.explodeVibration, VibrationRegion.WholeBody, true).Run();
	}

	private void OnReset()
	{
		this.growAudioSource.Stop();
	}

	[SerializeField]
	private Animator mushroomAnimator;

	[SerializeField]
	private float sanityThreshold;

	[SerializeField]
	private AudioSource growAudioSource;

	[SerializeField]
	private GameObject explodeVFX;

	[SerializeField]
	private float pitchMin;

	[SerializeField]
	private float pitchMax;

	[SerializeField]
	private VibrationData explodeVibration;
}
