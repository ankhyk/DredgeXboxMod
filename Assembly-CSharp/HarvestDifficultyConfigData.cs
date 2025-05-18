using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestDifficultyConfigData", menuName = "Dredge/HarvestDifficultyConfigData", order = 0)]
public class HarvestDifficultyConfigData : SerializedScriptableObject
{
	[Header("General")]
	public float targetValue;

	public float secondsToPassivelyCatch;

	[Header("Radial")]
	public float rotationSpeed;

	public int minTargets;

	public int maxTargets;

	public int minTargetWidth;

	public int maxTargetWidth;

	public int specialTargetWidth;

	[Header("Pendulum")]
	public int numPendulumSegments;

	[Header("Ball Catcher")]
	public List<List<BallCatcherBallConfig>> ballCatcherPatterns;

	public float valueFactor;

	public float speedFactor;

	public float ballTrophySpeedFactor;

	public float targetZoneDegrees;

	[Header("Diamond")]
	public float diamondRotation;

	public float diamondScaleUpTimeSec;

	public float diamondTrophySpeedFactor;

	public float timeBetweenDiamondTargetsMin;

	public float timeBetweenDiamondTargetsMax;

	[Header("Spiral")]
	public float spiralRotationSpeed;

	public int spiralNumNotches;

	public float spiralMinNotchWidth;

	public float spiralMaxNotchWidth;

	public float spiralValueFactor;
}
