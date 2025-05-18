using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldEventData", menuName = "Dredge/WorldEventData", order = 0)]
public class WorldEventData : SerializedScriptableObject
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(InventoryCondition));
	}

	private void OnValidate()
	{
		if (this.forbidOoze && !this.hasMinDepth)
		{
			CustomDebug.EditorLogError("[WorldEventData] OnValidate() ooze is forbidden for event " + base.name + " but no depth checks will occur.");
		}
	}

	public WorldEventType eventType;

	public GameObject prefab;

	public bool dispelByBanish;

	public bool dispelByFoghorn;

	public float foghornDispelTime;

	public float foghornDispelCount;

	public int minWorldPhase;

	[Range(0f, 1f)]
	public float minSanity;

	[Range(0f, 1f)]
	public float maxSanity;

	public Dictionary<GameMode, float> repeatDelay = new Dictionary<GameMode, float>();

	public float weight;

	[Range(0f, 1f)]
	public float spawnStartTime;

	[Range(0f, 1f)]
	public float spawnEndTime;

	public bool hasDuration;

	public float durationSec;

	public bool hasMinDepth;

	public float minDepth;

	public List<Vector3> depthTestPath;

	public bool isPath;

	public float depthPathNumChecks = 5f;

	public Vector3 playerSpawnOffset;

	public Vector3 zoneTestOffset;

	public bool doSafeZoneHitCheck;

	public Vector3 safeZoneHitCheckOffset;

	public ZoneEnum forbiddenZones;

	public List<InventoryCondition> itemInventoryConditions = new List<InventoryCondition>();

	public bool forbidOoze = true;

	public bool allowInPassiveMode;
}
