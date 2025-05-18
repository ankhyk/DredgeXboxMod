using System;
using UnityEngine;

public class TimeOfDayAudioSourceFader : MonoBehaviour
{
	private void Update()
	{
		this.helperVar = Mathf.Max(0.001f, this.volumeModifier.Evaluate(GameManager.Instance.Time.Time));
		this.audioSource.volume = this.helperVar * this.maxVolume;
	}

	[SerializeField]
	private AnimationCurve volumeModifier;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float maxVolume;

	private float cachedTimeOfDay;

	private float helperVar;
}
