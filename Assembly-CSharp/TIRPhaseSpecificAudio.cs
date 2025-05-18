using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TIRPhaseSpecificAudio : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
		this.OnTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
	}

	private void OnTIRWorldPhaseChanged(int newPhase)
	{
		bool flag = this.validTIRPhases.Contains(Mathf.Min(newPhase, 6));
		if (flag && !this.isPlaying)
		{
			this.audioSource.Play();
			this.isPlaying = true;
			return;
		}
		if (!flag && this.isPlaying)
		{
			this.audioSource.Stop();
			this.isPlaying = false;
		}
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<int> validTIRPhases;

	private bool isPlaying;
}
