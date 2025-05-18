using System;
using System.Collections.Generic;
using UnityEngine;

public class OozePatchEvents : MonoBehaviour, IGameModeResponder
{
	private void Awake()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
		this.OnTIRWorldPhaseChanged(GameManager.Instance.SaveData.TIRWorldPhase);
		OozePatchEvents.numberActiveTentacles = 0;
		OozePatchEvents.numberActiveMonsters = 0;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock != null)
		{
			this.activeTentacles.ForEach(delegate(OozeTentacle t)
			{
				t.RequestEventFinish();
			});
			this.activeMonsters.ForEach(delegate(OozeMonster t)
			{
				t.RequestEventFinish();
			});
		}
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
		OozePatchEvents.numberActiveTentacles = 0;
		OozePatchEvents.numberActiveMonsters = 0;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (ability.name == this.banishAbility.name)
		{
			this.isBanishActive = enabled;
		}
	}

	private void OnTIRWorldPhaseChanged(int phase)
	{
		this.currentTIRWorldPhase = phase;
		this.maxNumActiveTumors = this.tumorData.GetMaxNumForPhase(this.currentTIRWorldPhase);
		this.maxNumActiveEyes = this.eyeData.GetMaxNumForPhase(this.currentTIRWorldPhase);
		this.maxNumActiveTentacles = this.tentacleData.GetMaxNumForPhase(this.currentTIRWorldPhase);
		this.maxNumActiveMonsters = this.monsterData.GetMaxNumForPhase(this.currentTIRWorldPhase);
	}

	private void Update()
	{
		if (!this.isBanishActive && Time.time > this.timeOfLastSpawnCheck + this.spawnCheckIntervalSec && GameManager.Instance.Player != null)
		{
			this.timeOfLastSpawnCheck = Time.time;
			if (Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position) < this.playerProximityThreshold)
			{
				this.TrySpawnEvent();
			}
		}
	}

	private void TrySpawnEvent()
	{
		float currentSanity = GameManager.Instance.Player.Sanity.CurrentSanity;
		if (!this.forbidTumors && this.activeTumors.Count < this.maxNumActiveTumors && Time.time > this.timeOfLastTumorSpawn + this.tumorData.spawnIntervalSec && this.tumorData.GetCanSpawnBySanity(currentSanity))
		{
			this.TrySpawnTumor(false);
		}
		if (!this.forbidEyes && this.activeEyes.Count < this.maxNumActiveEyes && Time.time > this.timeOfLastEyeSpawn + this.eyeData.spawnIntervalSec && this.eyeData.GetCanSpawnBySanity(currentSanity) && this.currentGameMode == GameMode.NORMAL)
		{
			this.TrySpawnEye(false);
		}
		if (!this.forbidTentacles && OozePatchEvents.numberActiveTentacles == 0 && OozePatchEvents.numberActiveTentacles < this.maxNumActiveTentacles && OozePatchEvents.numberActiveMonsters == 0 && Time.time > this.timeOfLastTentacleSpawn + this.tentacleData.spawnIntervalSec && this.tentacleData.GetCanSpawnBySanity(currentSanity) && this.currentGameMode == GameMode.NORMAL && !GameManager.Instance.Player.IsDocked)
		{
			this.TrySpawnTentacle(false);
		}
		if (!this.forbidMonsters && OozePatchEvents.numberActiveMonsters == 0 && OozePatchEvents.numberActiveMonsters < this.maxNumActiveMonsters && OozePatchEvents.numberActiveTentacles == 0 && Time.time > this.timeOfLastMonsterSpawn + this.monsterData.spawnIntervalSec && this.monsterData.GetCanSpawnBySanity(currentSanity) && this.currentGameMode == GameMode.NORMAL && !GameManager.Instance.Player.IsDocked)
		{
			this.TrySpawnMonster(false);
		}
	}

	private void TrySpawnTumor(bool force = false)
	{
		Vector3 vector;
		if (OozePatchEvents.GetPositionInOoze(base.transform.position, this.oozePatch.PatchSize, 2f, out vector, false))
		{
			if (Vector3.Distance(vector, GameManager.Instance.Player.transform.position) < 5f)
			{
				return;
			}
			Quaternion quaternion = Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			OozeTumor component = global::UnityEngine.Object.Instantiate<GameObject>(this.tumorData.prefab, vector, quaternion).GetComponent<OozeTumor>();
			if (component != null)
			{
				OozeTumor oozeTumor = component;
				oozeTumor.OnOozeEventComplete = (Action<OozeEvent>)Delegate.Combine(oozeTumor.OnOozeEventComplete, new Action<OozeEvent>(this.OnOozeEventComplete));
				this.activeTumors.Add(component);
				this.timeOfLastTumorSpawn = Time.time;
				return;
			}
		}
		else if (force)
		{
			this.TrySpawnTumor(true);
		}
	}

	private void TrySpawnEye(bool force = false)
	{
		Vector3 vector;
		if (OozePatchEvents.GetPositionInOoze(base.transform.position, this.oozePatch.PatchSize, 1f, out vector, false))
		{
			Quaternion quaternion = Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			OozeEye component = global::UnityEngine.Object.Instantiate<GameObject>(this.eyeData.prefab, vector, quaternion).GetComponent<OozeEye>();
			if (component != null)
			{
				OozeEye oozeEye = component;
				oozeEye.OnOozeEventComplete = (Action<OozeEvent>)Delegate.Combine(oozeEye.OnOozeEventComplete, new Action<OozeEvent>(this.OnOozeEventComplete));
				this.activeEyes.Add(component);
				this.timeOfLastEyeSpawn = Time.time;
				return;
			}
		}
		else if (force)
		{
			this.TrySpawnEye(true);
		}
	}

	private void TrySpawnTentacle(bool force = false)
	{
		Vector3 vector;
		if (OozePatchEvents.GetPositionInOoze(GameManager.Instance.Player.transform.position, this.tentacleData.spawnThisCloseToPlayer, 3f, out vector, true))
		{
			OozeTentacle component = global::UnityEngine.Object.Instantiate<GameObject>(this.tentacleData.prefab, vector, Quaternion.identity).GetComponent<OozeTentacle>();
			if (component != null)
			{
				OozeTentacle oozeTentacle = component;
				oozeTentacle.OnOozeEventComplete = (Action<OozeEvent>)Delegate.Combine(oozeTentacle.OnOozeEventComplete, new Action<OozeEvent>(this.OnOozeEventComplete));
				this.activeTentacles.Add(component);
				OozePatchEvents.numberActiveTentacles++;
				this.timeOfLastTentacleSpawn = Time.time;
				return;
			}
		}
		else if (force)
		{
			this.TrySpawnTentacle(true);
		}
	}

	private void TrySpawnMonster(bool force = false)
	{
		Vector3 vector;
		if (GameManager.Instance.OozePatchManager.isOozeNearToPlayer && OozePatchEvents.GetPositionInOoze(base.transform.position, this.oozePatch.PatchSize, 5f, out vector, false))
		{
			Quaternion quaternion = Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			OozeMonster component = global::UnityEngine.Object.Instantiate<GameObject>(this.monsterData.prefab, vector, quaternion).GetComponent<OozeMonster>();
			if (component != null)
			{
				OozeMonster oozeMonster = component;
				oozeMonster.OnOozeEventComplete = (Action<OozeEvent>)Delegate.Combine(oozeMonster.OnOozeEventComplete, new Action<OozeEvent>(this.OnOozeEventComplete));
				this.activeMonsters.Add(component);
				OozePatchEvents.numberActiveMonsters++;
				this.timeOfLastMonsterSpawn = Time.time;
				return;
			}
		}
		else if (force)
		{
			this.TrySpawnMonster(true);
		}
	}

	private void OnOozeEventComplete(OozeEvent oozeEvent)
	{
		oozeEvent.OnOozeEventComplete = (Action<OozeEvent>)Delegate.Remove(oozeEvent.OnOozeEventComplete, new Action<OozeEvent>(this.OnOozeEventComplete));
		if (oozeEvent is OozeTumor)
		{
			this.activeTumors.Remove(oozeEvent as OozeTumor);
			return;
		}
		if (oozeEvent is OozeEye)
		{
			this.activeEyes.Remove(oozeEvent as OozeEye);
			return;
		}
		if (oozeEvent is OozeTentacle)
		{
			this.activeTentacles.Remove(oozeEvent as OozeTentacle);
			OozePatchEvents.numberActiveTentacles--;
			return;
		}
		if (oozeEvent is OozeMonster)
		{
			this.activeMonsters.Remove(oozeEvent as OozeMonster);
			OozePatchEvents.numberActiveMonsters--;
		}
	}

	public static bool GetPositionInOoze(Vector3 anchorPosition, float searchRadius, float sizeTolerance, out Vector3 position, bool searchOnRadius)
	{
		Vector2 vector;
		if (searchOnRadius)
		{
			vector = global::UnityEngine.Random.insideUnitCircle.normalized * searchRadius;
		}
		else
		{
			vector = global::UnityEngine.Random.insideUnitCircle * searchRadius;
		}
		position = new Vector3(anchorPosition.x + vector.x, 0f, anchorPosition.z + vector.y);
		return OozePatchEvents.IsOozePresentAtPositionWithSizeTolerance(position, sizeTolerance);
	}

	private static bool IsOozePresentAtPositionWithSizeTolerance(Vector3 position, float sizeTolerance)
	{
		bool flag = true;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				Vector3 vector = new Vector3(position.x + (float)i * sizeTolerance, position.y, position.z + (float)j * sizeTolerance);
				if (GameManager.Instance.OozePatchManager.SampleOozeAtPosition(vector) <= 0f)
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	public void OnGameModeChanged(GameMode gameMode)
	{
		this.currentGameMode = gameMode;
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			this.activeEyes.ForEach(delegate(OozeEye t)
			{
				t.RequestEventFinish();
			});
			this.activeTentacles.ForEach(delegate(OozeTentacle t)
			{
				t.RequestEventFinish();
			});
			this.activeMonsters.ForEach(delegate(OozeMonster t)
			{
				t.RequestEventFinish();
			});
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.playerProximityThreshold);
	}

	[SerializeField]
	private OozeEventData tumorData;

	[SerializeField]
	private OozeEventData eyeData;

	[SerializeField]
	private OozeEventData tentacleData;

	[SerializeField]
	private OozeEventData monsterData;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private OozePatch oozePatch;

	[SerializeField]
	private float spawnCheckIntervalSec;

	[SerializeField]
	private float playerProximityThreshold;

	[SerializeField]
	private bool forbidTumors;

	[SerializeField]
	private bool forbidEyes;

	[SerializeField]
	private bool forbidTentacles;

	[SerializeField]
	private bool forbidMonsters;

	private float timeOfLastSpawnCheck;

	private float timeOfLastTumorSpawn;

	private float timeOfLastEyeSpawn;

	private float timeOfLastTentacleSpawn;

	private float timeOfLastMonsterSpawn;

	private static int numberActiveMonsters;

	private static int numberActiveTentacles;

	private List<OozeTumor> activeTumors = new List<OozeTumor>();

	private List<OozeEye> activeEyes = new List<OozeEye>();

	private List<OozeTentacle> activeTentacles = new List<OozeTentacle>();

	private List<OozeMonster> activeMonsters = new List<OozeMonster>();

	private bool isBanishActive;

	private int currentTIRWorldPhase;

	private int maxNumActiveTumors;

	private int maxNumActiveEyes;

	private int maxNumActiveTentacles;

	private int maxNumActiveMonsters;

	private GameMode currentGameMode;
}
