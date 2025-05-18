using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;

public class WorldEventManager : MonoBehaviour, IGameModeResponder
{
	private bool SuppressSpawns { get; set; }

	public WorldEvent CurrentEvent
	{
		get
		{
			return this.currentEvent;
		}
	}

	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && this.currentEvent != null && !this.currentEvent.worldEventData.allowInPassiveMode && this.currentEvent.worldEventData.dispelByBanish)
		{
			this.currentEvent.RequestEventFinish();
		}
		this.rollFrequency = GameManager.Instance.GameConfigData.WorldEventRollFrequency[this.currentGameMode];
	}

	public void RegisterStaticWorldEvent(WorldEventType worldEventType, WorldEvent worldEvent)
	{
		if (!this.staticWorldEvents.ContainsKey(worldEventType))
		{
			this.staticWorldEvents.Add(worldEventType, new List<WorldEvent>());
		}
		this.staticWorldEvents[worldEventType].Add(worldEvent);
	}

	private void OnEnable()
	{
		GameManager.Instance.WorldEventManager = this;
		this.timeOfLastRoll = GameManager.Instance.Time.TimeAndDay;
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
		ApplicationEvents.Instance.OnDemoEndToggled += this.OnDemoEndToggled;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameManager.Instance.WorldEventManager = null;
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
		ApplicationEvents.Instance.OnDemoEndToggled -= this.OnDemoEndToggled;
		this.RemoveTerminalCommands();
	}

	private void OnFinaleVoyageStarted()
	{
		this.SuppressSpawns = true;
	}

	private void OnDemoEndToggled(bool started)
	{
		this.DoEvent(this.leviathanCallWorldEventData);
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name)
		{
			this.isBanishActive = enabled;
			if (this.currentEvent != null && this.currentEvent.worldEventData.dispelByBanish && enabled)
			{
				GameEvents.Instance.TriggerThreatBanished(true);
				this.currentEvent.RequestEventFinish();
				return;
			}
		}
		else if (abilityData.name == this.foghornAbilityData.name)
		{
			this.isFoghornActive = enabled;
			if (enabled)
			{
				this.foghornBlastCount++;
			}
		}
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock && this.currentEvent)
		{
			this.currentEvent.RequestEventFinish();
		}
	}

	private void Update()
	{
		if (!this.SuppressSpawns && GameManager.Instance.IsPlaying && GameManager.Instance.Time.TimeAndDay > this.timeOfLastRoll + this.rollFrequency && this.currentEvent == null)
		{
			this.timeOfLastRoll = GameManager.Instance.Time.TimeAndDay;
			this.RollForEvent();
		}
		if (this.isFoghornActive)
		{
			this.foghornHoldTime += Time.deltaTime;
			if (this.currentEvent != null && this.currentEvent.worldEventData.dispelByFoghorn && ((float)this.foghornBlastCount >= this.currentEvent.worldEventData.foghornDispelCount || this.foghornHoldTime >= this.currentEvent.worldEventData.foghornDispelTime))
			{
				this.currentEvent.RequestEventFinish();
				return;
			}
		}
		else
		{
			this.foghornHoldTime -= Time.deltaTime;
			this.foghornHoldTime = Mathf.Max(this.foghornHoldTime, 0f);
		}
	}

	private void RollForEvent()
	{
		bool flag = true;
		if (GameManager.Instance.Player.IsDocked)
		{
			flag = false;
		}
		if (GameManager.Instance.Player.IsFishing)
		{
			flag = false;
		}
		if (GameManager.Instance.Time.IsTimePassingForcefully())
		{
			flag = false;
		}
		if (flag && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.WorldEventChance)
		{
			WorldEventData worldEventData = this.SelectInsanityEvent();
			if (worldEventData != null)
			{
				this.DoEvent(worldEventData);
			}
		}
	}

	public void DoEvent(WorldEventData worldEventData)
	{
		switch (worldEventData.eventType)
		{
		case WorldEventType.SPAWN_PREFAB:
		{
			Vector3 position = GameManager.Instance.Player.transform.position;
			Vector3 vector = new Vector3(position.x, position.y, position.z);
			vector += GameManager.Instance.Player.transform.right * worldEventData.playerSpawnOffset.x;
			vector += GameManager.Instance.Player.transform.forward * worldEventData.playerSpawnOffset.z;
			vector.y = worldEventData.playerSpawnOffset.y;
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(worldEventData.prefab, vector, Quaternion.identity);
			this.currentEvent = gameObject.GetComponent<WorldEvent>();
			break;
		}
		case WorldEventType.EYE_PARTICLES:
			this.currentEvent = this.eyeParticleWorldEvent;
			break;
		case WorldEventType.FOG_GHOST:
		{
			float minDistance = float.PositiveInfinity;
			float thisDistance = 0f;
			List<WorldEvent> list = new List<WorldEvent>();
			if (this.staticWorldEvents.TryGetValue(worldEventData.eventType, out list))
			{
				list.ForEach(delegate(WorldEvent worldEvent)
				{
					thisDistance = Vector3.Distance(GameManager.Instance.Player.transform.position, worldEvent.transform.position);
					if (thisDistance < minDistance)
					{
						minDistance = thisDistance;
						this.currentEvent = worldEvent;
					}
				});
			}
			break;
		}
		case WorldEventType.PLAYER_CHILD:
		{
			GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(worldEventData.prefab, GameManager.Instance.Player.transform);
			this.currentEvent = gameObject2.GetComponent<WorldEvent>();
			break;
		}
		}
		if (this.currentEvent == null)
		{
			return;
		}
		this.currentEvent.worldEventData = worldEventData;
		WorldEvent worldEvent2 = this.currentEvent;
		worldEvent2.OnEventFinished = (Action<WorldEvent>)Delegate.Combine(worldEvent2.OnEventFinished, new Action<WorldEvent>(this.OnEventFinished));
		this.currentEvent.Activate();
		this.foghornHoldTime = 0f;
		this.foghornBlastCount = 0;
		this.AddWorldEventToHistory(worldEventData, this.timeOfLastRoll);
	}

	public void AddWorldEventToHistory(WorldEventData worldEventData, float time)
	{
		if (GameManager.Instance.SaveData.eventHistory.ContainsKey(worldEventData.name))
		{
			GameManager.Instance.SaveData.eventHistory[worldEventData.name] = time;
			return;
		}
		GameManager.Instance.SaveData.eventHistory.Add(worldEventData.name, time);
	}

	public bool PassesRepeatDelayForWorldEvent(WorldEventData e, float time)
	{
		float negativeInfinity = float.NegativeInfinity;
		if (GameManager.Instance.SaveData.eventHistory.ContainsKey(e.name))
		{
			GameManager.Instance.SaveData.eventHistory.TryGetValue(e.name, out negativeInfinity);
		}
		return time > negativeInfinity + e.repeatDelay[this.currentGameMode];
	}

	public void OnEventSpawnAborted(WorldEventData worldEventData)
	{
		GameManager.Instance.SaveData.eventHistory.Remove(worldEventData.name);
	}

	private void OnEventFinished(WorldEvent worldEvent)
	{
		if (worldEvent == this.currentEvent)
		{
			WorldEvent worldEvent2 = this.currentEvent;
			worldEvent2.OnEventFinished = (Action<WorldEvent>)Delegate.Remove(worldEvent2.OnEventFinished, new Action<WorldEvent>(this.OnEventFinished));
			this.currentEvent = null;
		}
		else
		{
			Debug.LogWarning(string.Format("[WorldEventManager] OnEventFinished({0}) the event that finished wasn't one we were listening for. Ignoring.", worldEvent));
		}
		this.foghornHoldTime = 0f;
		this.foghornBlastCount = 0;
	}

	private WorldEventData SelectInsanityEvent()
	{
		WorldEventData worldEventData = null;
		List<WorldEventData> list = new List<WorldEventData>();
		for (int i = 0; i < GameManager.Instance.DataLoader.allWorldEvents.Count; i++)
		{
			WorldEventData worldEventData2 = GameManager.Instance.DataLoader.allWorldEvents[i];
			if (this.TestWorldEvent(worldEventData2, true))
			{
				list.Add(worldEventData2);
			}
		}
		if (list.Count > 0)
		{
			int randomWeightedIndex = MathUtil.GetRandomWeightedIndex(list.Select((WorldEventData e) => e.weight).ToArray<float>());
			worldEventData = list[randomWeightedIndex];
		}
		else
		{
			Debug.LogWarning("[WorldEventManager] SelectInsanityEvent() no candidate events found.");
		}
		worldEventData;
		return worldEventData;
	}

	public bool TestWorldEvent(WorldEventData e, bool exitEarly)
	{
		if (this.showWorldEventTestMarkers)
		{
			foreach (object obj in this.testPointContainer.transform)
			{
				global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
		}
		bool flag = true;
		float currentSanity = GameManager.Instance.Player.Sanity.CurrentSanity;
		float timeAndDay = GameManager.Instance.Time.TimeAndDay;
		float time = GameManager.Instance.Time.Time;
		float num = (float)GameManager.Instance.SaveData.WorldPhase;
		if (this.currentGameMode == GameMode.PASSIVE && !e.allowInPassiveMode)
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (num < (float)e.minWorldPhase)
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (currentSanity < e.minSanity || currentSanity > e.maxSanity)
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (this.isBanishActive && e.dispelByBanish)
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (e.spawnStartTime < e.spawnEndTime)
		{
			if (time < e.spawnStartTime || time > e.spawnEndTime)
			{
				flag = false;
				if (exitEarly)
				{
					return flag;
				}
			}
		}
		else if (time < e.spawnStartTime && time > e.spawnEndTime)
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (!this.PassesRepeatDelayForWorldEvent(e, timeAndDay))
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (e.doSafeZoneHitCheck)
		{
			Vector3 vector = GameManager.Instance.Player.transform.position;
			if (this.DoesHitSafeZone(vector))
			{
				flag = false;
				if (exitEarly)
				{
					return flag;
				}
			}
			vector += GameManager.Instance.Player.transform.right * e.safeZoneHitCheckOffset.x;
			vector += GameManager.Instance.Player.transform.up * e.safeZoneHitCheckOffset.y;
			vector += GameManager.Instance.Player.transform.forward * e.safeZoneHitCheckOffset.z;
			if (this.DoesHitSafeZone(vector))
			{
				flag = false;
				if (exitEarly)
				{
					return flag;
				}
			}
		}
		if (!e.itemInventoryConditions.All((InventoryCondition e) => e.Evaluate()))
		{
			flag = false;
			if (exitEarly)
			{
				return flag;
			}
		}
		if (e.hasMinDepth)
		{
			if (e.isPath)
			{
				if (!this.CheckDepthPath(e.depthTestPath, e.depthPathNumChecks, e.minDepth, e.forbidOoze))
				{
					flag = false;
					if (exitEarly)
					{
						return flag;
					}
				}
			}
			else
			{
				Transform playerTransform = GameManager.Instance.Player.transform;
				if (e.depthTestPath.Any((Vector3 p) => !this.CheckDepthRelativePoint(p, playerTransform, e.minDepth, e.forbidOoze)))
				{
					flag = false;
					if (exitEarly)
					{
						return flag;
					}
				}
			}
		}
		if (e.forbiddenZones != ZoneEnum.NONE)
		{
			ZoneEnum zoneForPoint = PlayerZoneDetector.GetZoneForPoint(GameManager.Instance.Player.transform.position + GameManager.Instance.Player.transform.right * e.zoneTestOffset.x + GameManager.Instance.Player.transform.up * e.zoneTestOffset.y + GameManager.Instance.Player.transform.forward * e.zoneTestOffset.z);
			if (e.forbiddenZones.HasFlag(zoneForPoint))
			{
				return false;
			}
		}
		return flag;
	}

	public bool DoesHitSafeZone(Vector3 point)
	{
		point.y = 9999f;
		return Physics.RaycastAll(point, Vector3.down, 99999f, this.safeZoneHitCheckLayerMask).Length != 0;
	}

	private bool CheckDepthPath(List<Vector3> depthPath, float depthPathNumChecks, float minDepth, bool forbidOoze)
	{
		Transform transform = GameManager.Instance.Player.transform;
		WaveController waveController = GameManager.Instance.WaveController;
		if (depthPath.Count == 0)
		{
			return true;
		}
		if (depthPath.Count == 1)
		{
			if (!this.CheckDepthRelativePoint(depthPath[0], transform, minDepth, forbidOoze))
			{
				return false;
			}
		}
		else
		{
			float num = 1f / depthPathNumChecks;
			for (int i = 0; i < depthPath.Count - 1; i++)
			{
				Vector3 vector = depthPath[i];
				Vector3 vector2 = depthPath[i + 1];
				int num2 = 0;
				while ((float)num2 <= depthPathNumChecks)
				{
					Vector3 vector3 = Vector3.Lerp(vector, vector2, num * (float)num2);
					if (!this.CheckDepthRelativePoint(vector3, transform, minDepth, forbidOoze))
					{
						return false;
					}
					num2++;
				}
			}
		}
		return true;
	}

	private bool CheckDepthRelativePoint(Vector3 testPos, Transform playerTransform, float minDepth, bool forbidOoze)
	{
		Vector3 vector = playerTransform.position;
		vector += playerTransform.right * testPos.x;
		vector += playerTransform.up * testPos.y;
		vector += playerTransform.forward * testPos.z;
		vector.y = 0f;
		float num = GameManager.Instance.WaveController.SampleWaterDepthAtPosition(vector);
		bool flag = false;
		if (forbidOoze && GameManager.Instance.OozePatchManager)
		{
			flag = GameManager.Instance.OozePatchManager.SampleOozeAtPosition(vector) > 0f;
		}
		bool flag2 = this.DoesHitSafeZone(vector);
		bool flag3 = num > minDepth && !flag && !flag2;
		if (this.showWorldEventTestMarkers)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(flag3 ? this.testPointPrefab : this.testPointFailedPrefab, vector, Quaternion.identity).transform.SetParent(this.testPointContainer, true);
		}
		return flag3;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("world.event.list", new Action<CommandArg[]>(this.ListWorldEvents), 1, 2, "Lists all world events.");
		Terminal.Shell.AddCommand("world.event", new Action<CommandArg[]>(this.SpawnWorldEvent), 1, 2, "world.event <id> <delay>. Ignores all conditions. Optional delay.");
		Terminal.Shell.AddCommand("world.event.test", new Action<CommandArg[]>(this.TestWorldEvent), 1, 1, "world.event.test <id>. Tests conditions for an event. Does not spawn it.");
		Terminal.Shell.AddCommand("world.event.test-toggle-markers", new Action<CommandArg[]>(this.ToggleWorldEventTestMarkers), 1, 1, "world.event.test-toggle-markers [0-1]. Toggles whether world event test markers should be visually displayed in the scene.");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("world.event.list");
		Terminal.Shell.RemoveCommand("world.event");
		Terminal.Shell.RemoveCommand("world.event.test");
		Terminal.Shell.RemoveCommand("world.event.test-toggle-markers");
	}

	private void ToggleWorldEventTestMarkers(CommandArg[] args)
	{
		this.showWorldEventTestMarkers = args[0].Int == 1;
	}

	private void ListWorldEvents(CommandArg[] args)
	{
		string events = "";
		GameManager.Instance.DataLoader.allWorldEvents.ForEach(delegate(WorldEventData i)
		{
			events = events + i.name + ", ";
		});
	}

	private void SpawnWorldEvent(CommandArg[] args)
	{
		string name = args[0].String.ToLower();
		float num = 0f;
		if (args.Length >= 2)
		{
			num = args[1].Float;
		}
		WorldEventData worldEventData = GameManager.Instance.DataLoader.allWorldEvents.Find((WorldEventData x) => x.name.ToLower() == name);
		if (worldEventData)
		{
			base.StartCoroutine(this.SpawnWorldEventDelayed(worldEventData, num));
		}
	}

	private void TestWorldEvent(CommandArg[] args)
	{
		string name = args[0].String.ToLower();
		WorldEventData worldEventData = GameManager.Instance.DataLoader.allWorldEvents.Find((WorldEventData x) => x.name.ToLower() == name);
		if (worldEventData)
		{
			this.TestWorldEvent(worldEventData, false);
		}
	}

	private IEnumerator SpawnWorldEventDelayed(WorldEventData data, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.DoEvent(data);
		yield break;
	}

	[SerializeField]
	private EyeParticlesWorldEvent eyeParticleWorldEvent;

	[SerializeField]
	private Dictionary<WorldEventType, List<WorldEvent>> staticWorldEvents = new Dictionary<WorldEventType, List<WorldEvent>>();

	[SerializeField]
	private AbilityData banishAbilityData;

	[SerializeField]
	private AbilityData foghornAbilityData;

	[SerializeField]
	private WorldEventData leviathanCallWorldEventData;

	[SerializeField]
	private LayerMask safeZoneHitCheckLayerMask;

	[SerializeField]
	private GameObject testPointPrefab;

	[SerializeField]
	private GameObject testPointFailedPrefab;

	[SerializeField]
	private Transform testPointContainer;

	private bool isBanishActive;

	private bool isFoghornActive;

	private float rollFrequency;

	private float timeOfLastRoll;

	private float foghornHoldTime;

	private int foghornBlastCount;

	private WorldEvent currentEvent;

	private GameMode currentGameMode;

	private bool showWorldEventTestMarkers;
}
