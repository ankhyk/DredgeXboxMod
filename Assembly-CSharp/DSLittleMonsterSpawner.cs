using System;
using System.Collections.Generic;
using UnityEngine;

public class DSLittleMonsterSpawner : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnDismissAndSuppressDSLittleMonsters += this.DismissAndSuppressAllMonsters;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnDismissAndSuppressDSLittleMonsters -= this.DismissAndSuppressAllMonsters;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name)
		{
			this.isBanishActive = enabled;
			if (this.isBanishActive)
			{
				this.DespawnAllMonsters(false, true);
			}
		}
	}

	private void DismissAndSuppressAllMonsters(float duration)
	{
		this.monsters.ForEach(delegate(DSLittleMonster m)
		{
			if (!m.IsDespawning)
			{
				m.DismissAndSuppress(duration);
			}
		});
	}

	private void Update()
	{
		if (this.player == null)
		{
			this.player = GameManager.Instance.Player;
			return;
		}
		if (Time.time > this.timeOfLastSpawnCheck + this.timeBetweenSpawnChecksSec)
		{
			this.timeOfLastSpawnCheck = Time.time;
			float num = Vector3.Distance(base.transform.position, this.player.transform.position);
			if (!this.isBanishActive && num < this.spawnDistance && this.monsters.Count < this.spawnConfigs.Count && Time.time > this.timeOfLastSpawn + this.timeBetweenSpawnsSec)
			{
				this.SpawnMonster();
				return;
			}
			if (num > this.despawnDistance && this.monsters.Count > 0)
			{
				this.DespawnAllMonsters(true, false);
			}
		}
	}

	private void DespawnAllMonsters(bool ignoreIfChasing, bool isBanish)
	{
		this.monsters.ForEach(delegate(DSLittleMonster m)
		{
			if (!m.IsDespawning)
			{
				m.Despawn(ignoreIfChasing, isBanish);
			}
		});
	}

	private void SpawnMonster()
	{
		this.timeOfLastSpawn = Time.time;
		DSLittleMonster component = global::UnityEngine.Object.Instantiate<GameObject>(this.monsterPrefab, base.transform.position, Quaternion.identity).GetComponent<DSLittleMonster>();
		DSMonsterSpawnConfig dsmonsterSpawnConfig = this.spawnConfigs[this.monsters.Count];
		component.speed = dsmonsterSpawnConfig.speed;
		component.turnSpeed = dsmonsterSpawnConfig.turnSpeed;
		component.wiggleAmount = dsmonsterSpawnConfig.wiggleAmount;
		component.wiggleSpeed = dsmonsterSpawnConfig.wiggleSpeed;
		component.viewDistance = dsmonsterSpawnConfig.viewDistance;
		component.circleSpeed = dsmonsterSpawnConfig.circleSpeed;
		component.circleDistance = dsmonsterSpawnConfig.circleDistance;
		component.lookFrequencySec = dsmonsterSpawnConfig.lookFrequencySec;
		component.player = this.player;
		component.playerAnchor = this.player.DevilsSpineMonsterAttachPoint;
		component.playerDetectTransform = this.player.ColliderCenter;
		component.homeAnchor = this.homeAnchor;
		component.maxRange = this.maxRange;
		component.canLosePlayerBySight = this.canLosePlayerBySight;
		component.attachDistanceThreshold = this.attachDistanceThreshold;
		component.displacedDespawnDistance = this.displacedDespawnDistance;
		component.name = string.Format("DSLittleMonster-{0}-{1}", this.monsters.Count, base.name);
		this.monsters.Add(component);
		DSLittleMonster dslittleMonster = component;
		dslittleMonster.OnDespawn = (Action<DSLittleMonster>)Delegate.Combine(dslittleMonster.OnDespawn, new Action<DSLittleMonster>(this.OnMonsterDespawned));
	}

	private void OnMonsterDespawned(DSLittleMonster monsterLogic)
	{
		monsterLogic.OnDespawn = (Action<DSLittleMonster>)Delegate.Remove(monsterLogic.OnDespawn, new Action<DSLittleMonster>(this.OnMonsterDespawned));
		this.monsters.Remove(monsterLogic);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		float num = 0f;
		for (int i = 0; i < this.spawnConfigs.Count; i++)
		{
			if (num < this.spawnConfigs[i].viewDistance)
			{
				num = this.spawnConfigs[i].viewDistance;
			}
		}
		Gizmos.DrawWireSphere(base.transform.position, num);
	}

	[SerializeField]
	private AbilityData banishAbilityData;

	[SerializeField]
	private GameObject monsterPrefab;

	[SerializeField]
	private Transform homeAnchor;

	[SerializeField]
	private List<DSMonsterSpawnConfig> spawnConfigs = new List<DSMonsterSpawnConfig>();

	[SerializeField]
	public bool canLosePlayerBySight;

	[SerializeField]
	public float attachDistanceThreshold;

	[SerializeField]
	public float timeBetweenSpawnsSec;

	[SerializeField]
	public float timeBetweenSpawnChecksSec;

	[SerializeField]
	public float spawnDistance;

	[SerializeField]
	public float despawnDistance;

	[SerializeField]
	public float maxRange;

	[SerializeField]
	public float displacedDespawnDistance;

	private Player player;

	private bool isBanishActive;

	private List<DSLittleMonster> monsters = new List<DSLittleMonster>();

	private float timeOfLastSpawn;

	private float timeOfLastSpawnCheck;
}
