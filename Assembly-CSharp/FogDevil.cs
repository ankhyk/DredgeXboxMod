using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FogDevil : MonoBehaviour, IGameModeResponder
{
	private bool SuppressSpawns { get; set; }

	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnWorldPhaseChanged += this.OnWorldPhaseChanged;
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnWorldPhaseChanged -= this.OnWorldPhaseChanged;
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool active)
	{
		if (abilityData.name == this.lightAbilityData.name)
		{
			this.RefreshAggroState(active);
		}
	}

	public void RefreshAggroState(bool active)
	{
		if (active && this.currentGameMode != GameMode.PASSIVE)
		{
			if (this.currentState == FogDevil.FogDevilState.SPAWNED)
			{
				this.ps.Emit(3);
				this.aggroParticles.Emit(15);
			}
			if (this.colorFadeTween != null)
			{
				this.colorFadeTween.Kill(false);
			}
			this.colorFadeTween = this.fogDevilMaterial.DOFloat(0f, "_NeutralAmount", 0.1f);
			this.colorFadeTween.OnComplete(delegate
			{
				this.colorFadeTween = null;
			});
			this.isAggro = true;
			return;
		}
		if (this.colorFadeTween != null)
		{
			this.colorFadeTween.Kill(false);
		}
		this.colorFadeTween = this.fogDevilMaterial.DOFloat(1f, "_NeutralAmount", 1f);
		this.colorFadeTween.OnComplete(delegate
		{
			this.colorFadeTween = null;
		});
		this.isAggro = false;
	}

	private void OnWorldPhaseChanged(int worldPhase)
	{
		this.worldPhase = worldPhase;
	}

	private void OnFinaleVoyageStarted()
	{
		this.SuppressSpawns = true;
		this.currentState = FogDevil.FogDevilState.DESPAWNING;
		this.despawnCoroutine = base.StartCoroutine(this.Despawn());
	}

	private void Start()
	{
		this.worldPhase = GameManager.Instance.SaveData.WorldPhase;
		this.spawnPos = base.transform.position;
		this.ma = this.ps.main;
		this.no = this.ps.noise;
		this.em = this.ps.emission;
		this.audioSource.volume = 0f;
		this.em.enabled = false;
		this.sanityMod.SetActive(false);
	}

	private void Update()
	{
		if (!this.player && GameManager.Instance.Player)
		{
			this.player = GameManager.Instance.Player.gameObject.transform;
		}
		if (!this.player)
		{
			return;
		}
		if (this.SuppressSpawns)
		{
			return;
		}
		if (this.currentState == FogDevil.FogDevilState.NOT_SPAWNED && this.currentGameMode != GameMode.PASSIVE && this.worldPhase >= this.worldPhaseForThisToSpawn && (GameManager.Instance.Time.Time < this.disappearTime || GameManager.Instance.Time.Time > this.appearTime))
		{
			this.currentState = FogDevil.FogDevilState.ATTEMPTING_TO_SPAWN;
		}
		if (this.currentState == FogDevil.FogDevilState.ATTEMPTING_TO_SPAWN && Time.time > this.timeOfLastSpawnAttempt + this.timeBetweenSpawnAttemptsSec)
		{
			this.timeOfLastSpawnAttempt = Time.time;
			this.DoTrySpawn();
		}
		if ((this.currentState == FogDevil.FogDevilState.SPAWNED || this.currentState == FogDevil.FogDevilState.ATTEMPTING_TO_SPAWN) && ((GameManager.Instance.Time.Time > this.disappearTime && GameManager.Instance.Time.Time < this.appearTime) || this.currentGameMode == GameMode.PASSIVE))
		{
			this.currentState = FogDevil.FogDevilState.DESPAWNING;
			this.despawnCoroutine = base.StartCoroutine(this.Despawn());
		}
		if (this.currentState == FogDevil.FogDevilState.SPAWNED)
		{
			this.AdjustPosition();
		}
	}

	private void AdjustPosition()
	{
		bool isDocked = GameManager.Instance.Player.IsDocked;
		Vector3 position = base.transform.position;
		Vector3 vector = this.player.position - base.transform.position;
		float magnitude = vector.magnitude;
		Vector3.Distance(base.transform.position, this.spawnPos);
		float num = Vector3.Distance(this.player.position, this.spawnPos);
		if (this.isAggro && !isDocked && num < this.chaseDistance)
		{
			this.sanityMod.SetActive(true);
			this.ma.startColor = this.chaseColor;
			this.ma.startLifetimeMultiplier = 0.1f;
			this.ma.maxParticles = 5;
			float num2 = Mathf.InverseLerp(this.chaseDistance, 0f, magnitude);
			this.no.strength = num2 * 3f;
			vector.y = 0f;
			float num3 = GameManager.Instance.PlayerStats.MovementSpeedModifier * this.speed;
			base.transform.position = position + vector.normalized * Time.deltaTime * num3 * num2;
		}
		else
		{
			this.sanityMod.SetActive(false);
			this.ma.startLifetimeMultiplier = 1f;
			this.ma.maxParticles = 2;
			this.no.strength = 0f;
			this.ma.startColor = this.idleColor;
			vector = this.spawnPos - base.transform.position;
			base.transform.position = base.transform.position + vector.normalized * Time.deltaTime * 2f;
		}
		if (magnitude > this.spawnDistance + this.spawnDistanceRandomOffset)
		{
			this.currentState = FogDevil.FogDevilState.ATTEMPTING_TO_SPAWN;
		}
	}

	private IEnumerator Despawn()
	{
		this.em.enabled = false;
		DOTween.To(() => this.audioSource.volume, delegate(float x)
		{
			this.audioSource.volume = x;
		}, 0f, this.ma.startLifetime.constantMin);
		yield return new WaitForSeconds(this.ma.startLifetime.constantMin);
		this.sanityMod.SetActive(false);
		this.currentState = FogDevil.FogDevilState.NOT_SPAWNED;
		this.despawnCoroutine = null;
		yield break;
	}

	private void OnDestroy()
	{
		if (this.despawnCoroutine != null)
		{
			base.StopCoroutine(this.despawnCoroutine);
			this.despawnCoroutine = null;
		}
	}

	private void DoTrySpawn()
	{
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			return;
		}
		Vector3 position = this.player.position;
		Vector3 forward = this.player.forward;
		Vector3 vector = this.player.position + this.player.forward * (this.spawnDistance + global::UnityEngine.Random.Range(-this.spawnDistanceRandomOffset, this.spawnDistanceRandomOffset));
		vector = this.RotatePointAroundPivot(vector, this.player.position, new Vector3(0f, global::UnityEngine.Random.Range(-this.spawnArcRadius, this.spawnArcRadius), 0f));
		vector.y = 0f;
		if (GameManager.Instance.WaveController.SampleWaveSteepnessAtPosition(vector) < this.minimumDepthSpawn)
		{
			return;
		}
		this.aggroParticles.gameObject.SetActive(false);
		base.transform.position = vector;
		this.currentState = FogDevil.FogDevilState.SPAWNED;
		this.audioSource.volume = this.audioVolume;
		this.spawnPos = vector;
		this.sanityMod.SetActive(true);
		this.em.enabled = true;
		this.aggroParticles.gameObject.SetActive(true);
		this.RefreshAggroState(GameManager.Instance.PlayerAbilities.GetIsAbilityActive(this.lightAbilityData));
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	[SerializeField]
	private float speed;

	[SerializeField]
	private float chaseDistance;

	[SerializeField]
	private float spawnDistance;

	[SerializeField]
	private float spawnDistanceRandomOffset;

	[SerializeField]
	private float spawnArcRadius;

	[SerializeField]
	private GameObject sanityMod;

	[SerializeField]
	private GameObject particles;

	[SerializeField]
	private Color idleColor;

	[SerializeField]
	private Color chaseColor;

	[SerializeField]
	private float appearTime;

	[SerializeField]
	private float disappearTime;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float audioVolume;

	[SerializeField]
	private float minimumDepthSpawn;

	[SerializeField]
	private float timeBetweenSpawnAttemptsSec;

	[SerializeField]
	private int worldPhaseForThisToSpawn;

	[SerializeField]
	private AbilityData lightAbilityData;

	[SerializeField]
	private Material fogDevilMaterial;

	[SerializeField]
	private ParticleSystem aggroParticles;

	[SerializeField]
	private ParticleSystem ps;

	private float timeOfLastSpawnAttempt;

	private ParticleSystem.MainModule ma;

	private ParticleSystem.NoiseModule no;

	private ParticleSystem.EmissionModule em;

	private Vector3 spawnPos;

	private Transform player;

	private FogDevil.FogDevilState currentState;

	private Coroutine despawnCoroutine;

	private int worldPhase;

	private bool isAggro;

	private Tween colorFadeTween;

	private GameMode currentGameMode;

	private enum FogDevilState
	{
		NOT_SPAWNED,
		ATTEMPTING_TO_SPAWN,
		SPAWNED,
		DESPAWNING
	}
}
