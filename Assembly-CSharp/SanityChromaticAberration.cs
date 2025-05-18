using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityChromaticAberration : MonoBehaviour
{
	private void OnEnable()
	{
		this.volumeProfile.TryGet<ChromaticAberration>(out this.chromaticAberration);
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.GetAreVisualsEnabled();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.PANIC_VISUALS)
		{
			this.GetAreVisualsEnabled();
		}
	}

	private void GetAreVisualsEnabled()
	{
		this.panicVisualsEnabled = GameManager.Instance.SettingsSaveData.panicVisuals == 1;
	}

	private void Update()
	{
		if (GameManager.Instance.IsPlaying && GameManager.Instance.Player && this.panicVisualsEnabled)
		{
			this.thisMagnitude = this._chromaticAberrationCurve.Evaluate(1f - GameManager.Instance.Player.Sanity.CurrentSanity);
		}
		else
		{
			this.thisMagnitude = 0f;
		}
		if (this.previousMagnitude != this.thisMagnitude)
		{
			this.chromaticAberration.intensity.Override(this.thisMagnitude);
			this.previousMagnitude = this.thisMagnitude;
		}
	}

	[SerializeField]
	private AnimationCurve _chromaticAberrationCurve;

	[SerializeField]
	private VolumeProfile volumeProfile;

	private ChromaticAberration chromaticAberration;

	private bool panicVisualsEnabled;

	private float previousMagnitude;

	private float thisMagnitude;
}
