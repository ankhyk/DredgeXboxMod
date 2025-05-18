using System;
using System.Collections.Generic;
using UnityEngine;

public class GiantIceMonsterHeartbeat : MonoBehaviour
{
	private void Awake()
	{
		this.timeUntilNextBeat = global::UnityEngine.Random.Range(this.timeBetweenBeatsMin, this.timeBetweenBeatsMax);
	}

	private void Update()
	{
		this.timeUntilNextBeat -= Time.deltaTime;
		if (this.timeUntilNextBeat <= 0f)
		{
			this.TryPlayHeartbeat();
			this.timeUntilNextBeat = global::UnityEngine.Random.Range(this.timeBetweenBeatsMin, this.timeBetweenBeatsMax);
		}
	}

	private void TryPlayHeartbeat()
	{
		if (GameManager.Instance.SettingsSaveData.heartbeatSFX == 1)
		{
			return;
		}
		int tprworldPhase = GameManager.Instance.SaveData.TPRWorldPhase;
		AudioClip audioClip = this.heartbeatsNormal.PickRandom<AudioClip>();
		if (tprworldPhase >= 4)
		{
			audioClip = null;
			this.timeUntilNextBeat = float.PositiveInfinity;
		}
		else if (tprworldPhase >= 2)
		{
			audioClip = this.heartbeatsTense.PickRandom<AudioClip>();
		}
		else if (tprworldPhase >= 1)
		{
			audioClip = this.heartbeatsMedium.PickRandom<AudioClip>();
		}
		if (audioClip != null)
		{
			this.heartbeatAudioSource.clip = audioClip;
			this.heartbeatAudioSource.Play();
		}
	}

	[SerializeField]
	private AudioSource heartbeatAudioSource;

	[SerializeField]
	private List<AudioClip> heartbeatsNormal = new List<AudioClip>();

	[SerializeField]
	private List<AudioClip> heartbeatsMedium = new List<AudioClip>();

	[SerializeField]
	private List<AudioClip> heartbeatsTense = new List<AudioClip>();

	[SerializeField]
	private float timeBetweenBeatsMin;

	[SerializeField]
	private float timeBetweenBeatsMax;

	private float timeUntilNextBeat;
}
