using System;
using UnityEngine;

[Serializable]
public class VibrationParams
{
	public VibrationParams(float largeMotorIntensity, float sm, float time, float postVibrateDelay, float damp)
	{
		this.LargeMotorIntensity = largeMotorIntensity;
		this.SmallMotorIntensity = sm;
		this.Time = time;
		this.PostVibrateDelay = postVibrateDelay;
		this.XboxDampening = damp;
	}

	[Range(0f, 1f)]
	public float LargeMotorIntensity;

	[Range(0f, 1f)]
	public float SmallMotorIntensity;

	public float Time;

	public float PostVibrateDelay;

	[Range(0f, 1f)]
	public float XboxDampening = 0.75f;
}
