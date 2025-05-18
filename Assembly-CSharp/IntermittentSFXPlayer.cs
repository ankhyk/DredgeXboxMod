using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class IntermittentSFXPlayer : MonoBehaviour
{
	private void Awake()
	{
		this.timeUntilNextPlay = global::UnityEngine.Random.Range(this.minDelaySec, this.maxDelaySec);
	}

	private void Update()
	{
		this.timeUntilNextPlay -= Time.deltaTime;
		if (this.timeUntilNextPlay <= 0f)
		{
			this.timeUntilNextPlay = global::UnityEngine.Random.Range(this.minDelaySec, this.maxDelaySec);
			if (this.affectedByDebugCommand && !IntermittentSFXPlayer.playSFXClips)
			{
				return;
			}
			GameManager.Instance.AudioPlayer.PlaySFX(this.assetReferences.PickRandom<AssetReference>(), base.transform.position, this.volumeScale, this.audioMixerGroup, this.audioRolloffMode, this.minDistance, this.maxDistance, false, false);
		}
	}

	public static bool playSFXClips = true;

	[SerializeField]
	private List<AssetReference> assetReferences;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private AudioRolloffMode audioRolloffMode;

	[SerializeField]
	private float volumeScale;

	[SerializeField]
	private float minDelaySec;

	[SerializeField]
	private float maxDelaySec;

	[SerializeField]
	private float minDistance;

	[SerializeField]
	private float maxDistance;

	[SerializeField]
	private bool affectedByDebugCommand;

	private float timeUntilNextPlay;
}
