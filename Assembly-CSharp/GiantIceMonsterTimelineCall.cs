using System;
using System.Collections.Generic;
using UnityEngine;

public class GiantIceMonsterTimelineCall : MonoBehaviour
{
	private void OnEnable()
	{
		int tprworldPhase = GameManager.Instance.SaveData.TPRWorldPhase;
		this.audioSource.PlayOneShot(this.callClips[tprworldPhase - 1]);
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> callClips = new List<AudioClip>();
}
