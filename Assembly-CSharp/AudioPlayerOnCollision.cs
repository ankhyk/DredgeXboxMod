using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class AudioPlayerOnCollision : MonoBehaviour
{
	protected virtual void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.clips.PickRandom<AssetReference>(), base.transform.position, 1f, this.audioMixerGroup, this.rolloffMode, this.min, this.max, this.use2DVolume, false);
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.clips.PickRandom<AssetReference>(), base.transform.position, 1f, this.audioMixerGroup, this.rolloffMode, this.min, this.max, this.use2DVolume, false);
		}
	}

	[SerializeField]
	private List<AssetReference> clips = new List<AssetReference>();

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private AudioRolloffMode rolloffMode;

	[SerializeField]
	private float min;

	[SerializeField]
	private float max;

	[SerializeField]
	private bool use2DVolume;
}
