using System;
using UnityEngine;

public class DSMonsterManager : MonoBehaviour
{
	public bool SuppressSpawns { get; set; }

	private void SpawnBigMonster()
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.bigMonsterPrefab, base.transform.position, Quaternion.identity);
		this.dsBigMonster = gameObject.GetComponent<DSBigMonster>();
		this.dsBigMonster.AnchorTransform = this.anchorTransform;
		this.dsBigMonster.DeaggroRange = this.deaggroRange;
		RouteConfig routeConfig = default(RouteConfig);
		routeConfig.route = this.path.Route;
		routeConfig.direction = RouteDirection.FORWARDS;
		this.dsBigMonster.Init(routeConfig);
	}

	private void DespawnBigMonster()
	{
		global::UnityEngine.Object.Destroy(this.dsBigMonster.gameObject);
		this.dsBigMonster = null;
	}

	private void Update()
	{
		if (this.player == null)
		{
			this.player = GameManager.Instance.Player;
			return;
		}
		if (Time.time > this.timeOfLastSpawnCheck + this.timeBetweenSpawnChecks)
		{
			this.timeOfLastSpawnCheck = Time.time;
			if (!this.SuppressSpawns && this.dsBigMonster == null && Vector3.Distance(this.player.transform.position, base.transform.position) < this.spawnRange)
			{
				this.SpawnBigMonster();
				return;
			}
			if (this.dsBigMonster != null && Vector3.Distance(this.player.transform.position, this.dsBigMonster.transform.position) > this.despawnRange)
			{
				this.DespawnBigMonster();
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.spawnRange);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.despawnRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.anchorTransform.position, this.deaggroRange);
	}

	[SerializeField]
	private EntityPath path;

	[SerializeField]
	private GameObject bigMonsterPrefab;

	[SerializeField]
	private float spawnRange;

	[SerializeField]
	private float despawnRange;

	[SerializeField]
	private float deaggroRange;

	[SerializeField]
	private float timeBetweenSpawnChecks;

	[SerializeField]
	private Transform anchorTransform;

	private float timeOfLastSpawnCheck;

	private Player player;

	private DSBigMonster dsBigMonster;
}
