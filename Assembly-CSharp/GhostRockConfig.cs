using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostRockConfig", menuName = "Dredge/GhostRockConfig", order = 0)]
public class GhostRockConfig : ScriptableObject
{
	[SerializeField]
	public float sanityThresholdMax;

	[SerializeField]
	public float sanityThresholdMin;

	[SerializeField]
	public float spawnStartTime;

	[SerializeField]
	public float spawnEndTime;

	[SerializeField]
	public float minDistanceThreshold;

	[SerializeField]
	public float maxDistanceThreshold;
}
