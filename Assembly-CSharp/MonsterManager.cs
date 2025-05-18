using System;
using System.Collections.Generic;
using CommandTerminal;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonsterManager : SerializedMonoBehaviour
{
	private bool SuppressSpawns { get; set; }

	public GCMonsterManager GaleCliffsMonsterManager
	{
		get
		{
			return this.galeCliffsMonsterManager;
		}
	}

	private void OnEnable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Combine(instance.OnGameEnded, new Action(this.OnGameEnded));
		GameEvents.Instance.OnPlayerHitByMonster += this.OnPlayerHitByMonster;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerHitByMonster -= this.OnPlayerHitByMonster;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Remove(instance.OnGameEnded, new Action(this.OnGameEnded));
		this.OnGameEnded();
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name)
		{
			this.isBanishActive = enabled;
		}
	}

	private void OnPlayerHitByMonster()
	{
		this.canSpawn = false;
	}

	private void OnGameEnded()
	{
		this.canSpawn = false;
	}

	private void OnFinaleVoyageStarted()
	{
		this.SuppressSpawns = true;
		this.galeCliffsMonsterManager.SuppressSpawns = true;
		this.devilsSpineMonsterManager.SuppressSpawns = true;
		this.twistedStrandMonsterManager.SuppressSpawns = true;
		this.marrowMonsters.ForEach(delegate(MarrowMonster m)
		{
			m.Despawn();
		});
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	private void Start()
	{
		GameManager.Instance.MonsterManager = this;
		this.monsterCounts = new int[this.monsterConfigs.Length];
		for (int i = 0; i < this.monsterConfigs.Length; i++)
		{
			this.monsterCounts[i] = 0;
		}
		this.canSpawn = true;
	}

	private void Update()
	{
		if (this.SuppressSpawns)
		{
			return;
		}
		if (!this.canSpawn && this.prevGameTime < GameManager.Instance.Time.DuskTime && GameManager.Instance.Time.Time > GameManager.Instance.Time.DuskTime)
		{
			this.canSpawn = true;
		}
		this.timeUntilNextCheck -= Time.deltaTime;
		if (this.timeUntilNextCheck <= 0f && this.canSpawn && !this.isBanishActive && GameManager.Instance.Player)
		{
			this.TrySpawnMonsters();
			this.timeUntilNextCheck = this.secondsBetweenChecks;
		}
		this.prevGameTime = GameManager.Instance.Time.Time;
	}

	private void TrySpawnMonsters()
	{
		for (int i = 0; i < this.monsterConfigs.Length; i++)
		{
			this.TrySpawnMonster(i);
		}
	}

	private void TrySpawnMonster(int configIndex)
	{
		MonsterConfig monsterConfig = this.monsterConfigs[configIndex];
		MonsterData monsterData = monsterConfig.monsterData;
		int num = this.monsterCounts[configIndex];
		if (num >= monsterConfig.maxSpawned)
		{
			return;
		}
		if ((float)GameManager.Instance.SaveData.WorldPhase < monsterData.worldPhaseMin)
		{
			return;
		}
		float time = GameManager.Instance.Time.Time;
		if (time <= monsterData.spawnTime && time >= monsterData.despawnTime)
		{
			return;
		}
		Vector3 vector = monsterConfig.path[4].position;
		int num2 = 0;
		while (num2 < 10 && Vector3.Distance(GameManager.Instance.Player.transform.position, vector) < monsterData.spawnMinDistance)
		{
			vector = monsterConfig.path[global::UnityEngine.Random.Range(0, monsterConfig.path.Length)].position;
			num2++;
		}
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(monsterData.prefab, vector, Quaternion.identity);
		MarrowMonster monster = gameObject.GetComponent<MarrowMonster>();
		monster.Init(monsterData, GameManager.Instance.Player, monsterConfig.path);
		this.marrowMonsters.Add(monster);
		this.monsterCounts[configIndex] = num + 1;
		int cachedConfigIndex = configIndex;
		MarrowMonster monster2 = monster;
		monster2.OnDespawned = (Action)Delegate.Combine(monster2.OnDespawned, new Action(delegate
		{
			this.OnMonsterDespawned(cachedConfigIndex, monster);
		}));
	}

	private void OnMonsterDespawned(int configIndex, MarrowMonster monster)
	{
		int num = this.monsterCounts[configIndex];
		this.monsterCounts[configIndex] = Mathf.Max(0, num - 1);
		this.marrowMonsters.Remove(monster);
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("world.phase", new Action<CommandArg[]>(this.SetWorldPhase), 1, 1, "Sets world phase e.g. 'world.phase 1'");
			Terminal.Shell.AddCommand("world.phase-tpr", new Action<CommandArg[]>(this.SetWorldPhaseTPR), 1, 1, "Sets TPR world phase e.g. 'world.phase-tpr 1'");
			Terminal.Shell.AddCommand("world.phase-tir", new Action<CommandArg[]>(this.SetWorldPhaseTIR), 1, 1, "Sets TIR world phase e.g. 'world.phase-tir 1'");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("world.phase");
			Terminal.Shell.RemoveCommand("world.phase-tpr");
			Terminal.Shell.RemoveCommand("world.phase-tir");
		}
	}

	private void SetWorldPhase(CommandArg[] args)
	{
		GameManager.Instance.SaveData.WorldPhase = args[0].Int;
		GameEvents.Instance.TriggerWorldPhaseChanged(GameManager.Instance.SaveData.WorldPhase);
	}

	private void SetWorldPhaseTPR(CommandArg[] args)
	{
		GameManager.Instance.SaveData.TPRWorldPhase = args[0].Int;
	}

	private void SetWorldPhaseTIR(CommandArg[] args)
	{
		GameManager.Instance.SaveData.TIRWorldPhase = args[0].Int;
		GameEvents.Instance.TriggerTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
	}

	[SerializeField]
	private GCMonsterManager galeCliffsMonsterManager;

	[SerializeField]
	private DSMonsterManager devilsSpineMonsterManager;

	[SerializeField]
	private TwistedStrandMonsterManager twistedStrandMonsterManager;

	[SerializeField]
	private float secondsBetweenChecks;

	[SerializeField]
	private MonsterConfig[] monsterConfigs;

	[SerializeField]
	public int[] monsterCounts;

	[SerializeField]
	private AbilityData banishAbilityData;

	private List<MarrowMonster> marrowMonsters = new List<MarrowMonster>();

	private bool canSpawn;

	private bool isBanishActive;

	private float timeUntilNextCheck;

	private float prevGameTime;
}
