using System;
using System.Collections.Generic;
using Cinemachine;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.Audio;

public class IceBlockController : MonoBehaviour
{
	public void OnIceBlockShatter(Transform t)
	{
		this.shatterEffect.gameObject.transform.position = t.position;
		this.shatterEffect.Emit(12);
		GameManager.Instance.AudioPlayer.PlaySFX(this.shatterClips.PickRandom<AudioClip>(), t.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, this.audioCloseDistance, this.audioFarDistance, false, false);
		this.impulseSource.gameObject.transform.position = t.position;
		this.impulseSource.GenerateImpulse();
		GameManager.Instance.VibrationManager.Vibrate(this.vibrationData, VibrationRegion.WholeBody, true).Run();
	}

	[SerializeField]
	private VibrationData vibrationData;

	[SerializeField]
	private CinemachineImpulseSource impulseSource;

	[SerializeField]
	private ParticleSystem shatterEffect;

	[SerializeField]
	private List<AudioClip> shatterClips = new List<AudioClip>();

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private float audioCloseDistance;

	[SerializeField]
	private float audioFarDistance;
}
