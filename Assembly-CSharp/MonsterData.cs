using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Dredge/MonsterData", order = 0)]
public class MonsterData : ScriptableObject
{
	public GameObject prefab;

	public float worldPhaseMin;

	public float spawnTime;

	public float despawnTime;

	public float spawnMinDistance;

	public float spawnMaxDistance;

	public float despawnDistanceThreshold;

	public float idleDepth;

	public float disappearDepth;

	public float seekDelaySec;

	public float playerLostThreshold;

	public float patrolSpeed;

	public float huntSpeed;

	public float fleeSpeed;
}
