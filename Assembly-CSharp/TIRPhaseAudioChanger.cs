using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TIRPhaseAudioChanger : SerializedMonoBehaviour
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
		newPhase = Mathf.Min(newPhase, 6);
		this.audioSource.clip = this.phaseSpecificAudio[newPhase];
		this.audioSource.Play();
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Dictionary<int, AudioClip> phaseSpecificAudio = new Dictionary<int, AudioClip>
	{
		{ 0, null },
		{ 1, null },
		{ 2, null },
		{ 3, null },
		{ 4, null },
		{ 5, null },
		{ 6, null }
	};
}
