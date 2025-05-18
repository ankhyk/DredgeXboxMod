using System;
using UnityEngine;
using UnityEngine.Audio;

public class TimeOfDayAudioBlender : MonoBehaviour
{
	private void Update()
	{
		this.cachedTimeOfDay = GameManager.Instance.Time.Time;
		this.helperVar = Mathf.Max(0.001f, this.dayVolumeModifier.Evaluate(this.cachedTimeOfDay));
		this.masterAudioMixer.SetFloat("dayAmbienceVolume", Mathf.Log10(this.helperVar * this.dayVolumeBase) * 20f);
		this.helperVar = Mathf.Max(0.001f, this.nightVolumeModifier.Evaluate(this.cachedTimeOfDay));
		this.masterAudioMixer.SetFloat("nightAmbienceVolume", Mathf.Log10(this.helperVar * this.nightVolumeBase) * 20f);
	}

	[SerializeField]
	private AudioMixer masterAudioMixer;

	[SerializeField]
	private AnimationCurve dayVolumeModifier;

	[SerializeField]
	private AnimationCurve nightVolumeModifier;

	[SerializeField]
	private float dayVolumeBase;

	[SerializeField]
	private float nightVolumeBase;

	private float cachedTimeOfDay;

	private float helperVar;

	private float helperVar2;
}
