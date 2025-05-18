using System;
using UnityEngine;

[Serializable]
public class WeatherParameters
{
	public float durationHours;

	public float cloudiness;

	public float cloudDarkness;

	public float auroraAmount;

	public float waveSteepness;

	public float foamAmount;

	public float lightningDelayMin;

	public float lightningDelayMax;

	public float weight;

	public AudioClip sfx;

	public float sfxVolume;

	public AnimationCurve sfxEnterCurve;

	public AnimationCurve sfxExitCurve;

	public bool forbidStingers;

	public bool hasRain;

	public float rainSpeed;

	public float rainRate;

	public float splashChance;

	public float dropletHeightMin;

	public float dropletHeightMax;

	public AnimationCurve rainEnterCurve;

	public AnimationCurve rainExitCurve;

	public bool hasSnow;

	public float snowSpeed;

	public float snowRate;

	public AnimationCurve snowEnterCurve;

	public AnimationCurve snowExitCurve;
}
