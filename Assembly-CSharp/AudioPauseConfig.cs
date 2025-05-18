using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioPauseConfig : MonoBehaviour
{
	private void Start()
	{
		this.audioSource.ForEach(delegate(AudioSource a)
		{
			a.ignoreListenerPause = this.ignorePause;
		});
	}

	[SerializeField]
	private bool ignorePause;

	[SerializeField]
	private List<AudioSource> audioSource;
}
