using System;
using System.Collections.Generic;
using UnityEngine;

public class GCMonsterManager : MonoBehaviour
{
	public bool SuppressSpawns { get; set; }

	public GCMonsterRouteReference RouteReferences
	{
		get
		{
			return this.routeReference;
		}
	}

	public void TrySpawnFromTrigger(EntityPath spawnRoute, RouteDirection direction)
	{
		if (this.isMonsterSpawned)
		{
			return;
		}
		if (this.SuppressSpawns)
		{
			return;
		}
		if (GameManager.Instance.PlayerAbilities.GetIsAbilityActive(this.banishAbilityData))
		{
			return;
		}
		this.SpawnMonster(spawnRoute, direction);
	}

	private void SpawnMonster(EntityPath spawnRoute, RouteDirection direction)
	{
		List<Transform> route = spawnRoute.Route;
		GCMonster component = global::UnityEngine.Object.Instantiate<GameObject>(this.monsterData.prefab, route[0].position, route[0].rotation).GetComponent<GCMonster>();
		if (component)
		{
			this.isMonsterSpawned = true;
			component.Init(this.monsterData, route, direction);
			GCMonster gcmonster = component;
			gcmonster.OnMonsterDespawned = (Action)Delegate.Combine(gcmonster.OnMonsterDespawned, new Action(this.OnMonsterDespawned));
			GameManager.Instance.SaveData.SetBoolVariable("spawned-gc-monster", true);
		}
	}

	public void OnMonsterReachedHole(GaleCliffsMonsterHole linkedHole, GaleCliffsIsland linkedIsland)
	{
		if (Time.time < this.timeOfLastRockFall + this.minTimeBetweenRockFallsSec)
		{
			return;
		}
		this.timeOfLastRockFall = Time.time;
		List<FallingRocks> list = this.allFallingRocks.FindAll((FallingRocks r) => r.Island == linkedIsland);
		List<FallingRocks> rocksByThisHole = list.FindAll((FallingRocks r) => r.AssociatedHole == linkedHole);
		list.RemoveAll((FallingRocks r) => rocksByThisHole.Contains(r));
		list.ForEach(delegate(FallingRocks r)
		{
			if (r.isActiveAndEnabled)
			{
				r.TriggerWarningDelayed(global::UnityEngine.Random.Range(0f, 2f));
			}
		});
		rocksByThisHole.ForEach(delegate(FallingRocks r)
		{
			if (r.isActiveAndEnabled)
			{
				r.TriggerRockfallDelayed(global::UnityEngine.Random.Range(0f, 2f));
			}
		});
	}

	private void OnMonsterDespawned()
	{
		this.isMonsterSpawned = false;
	}

	[SerializeField]
	private MonsterData monsterData;

	[SerializeField]
	private List<FallingRocks> allFallingRocks;

	[SerializeField]
	private GCMonsterRouteReference routeReference;

	[SerializeField]
	private float minTimeBetweenRockFallsSec;

	[SerializeField]
	private AbilityData banishAbilityData;

	private float timeOfLastRockFall = float.NegativeInfinity;

	private bool isMonsterSpawned;
}
