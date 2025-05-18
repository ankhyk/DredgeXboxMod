using System;
using UnityEngine;

[CreateAssetMenu(fileName = "OozeEventData", menuName = "Dredge/OozeEventData", order = 0)]
public class OozeEventData : ScriptableObject
{
	public int GetMaxNumForPhase(int currentPhase)
	{
		if (currentPhase < this.minPhase)
		{
			return 0;
		}
		return this.baseCountAtMinPhase + this.extraCountPerPhaseBeyondMin * (currentPhase - this.minPhase);
	}

	public bool GetCanSpawnBySanity(float currentSanity)
	{
		return currentSanity <= this.maxSanityToSpawn;
	}

	[SerializeField]
	public GameObject prefab;

	[SerializeField]
	public int minPhase;

	[SerializeField]
	public int baseCountAtMinPhase;

	[SerializeField]
	public int extraCountPerPhaseBeyondMin;

	[SerializeField]
	public float maxSanityToSpawn;

	[SerializeField]
	public float spawnIntervalSec;

	[SerializeField]
	public float spawnThisCloseToPlayer;
}
