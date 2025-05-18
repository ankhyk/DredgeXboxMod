using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class WaterspoutWorldEvent : WorldEvent
{
	private void Awake()
	{
		bool flag = false;
		float num = 5f;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(base.transform.position, out navMeshHit, num, 1))
		{
			RaycastHit[] array = Physics.RaycastAll(new Vector3(navMeshHit.position.x, 999f, navMeshHit.position.z), Vector3.down, 999f, this.forbiddenSpawnLayers, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < array.Length; i++)
			{
			}
			if (array.Length == 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.navMeshAgent.enabled = false;
			base.transform.position = navMeshHit.position;
			this.navMeshAgent.enabled = true;
			this.audioSource.DOFade(1f, 1f).From(0f, true, false);
			return;
		}
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void Activate()
	{
		this.spawnTime = Time.time;
		this.particleSystems.ForEach(delegate(ParticleSystem ps)
		{
			if (ps.main.startLifetime.constantMax > this.finishDelaySec)
			{
				this.finishDelaySec = ps.main.startLifetime.constantMax;
			}
		});
		if (this.navMeshAgent != null)
		{
			float num = this.moveSpeedScalar * this.moveSpeedProportionalToPlayer * GameManager.Instance.GameConfigData.BaseMovementSpeedModifier * GameManager.Instance.PlayerStats.MovementSpeedModifier;
			this.freeSpeed = Mathf.Min(this.maxSpeed, num);
			this.speedWhenHarvesting = this.freeSpeed * 0.1f;
			this.navMeshAgent.speed = this.freeSpeed;
			this.navMeshAgent.acceleration = this.freeSpeed * this.accelerationFactor;
		}
		if (this.waterspoutMode == WaterspoutWorldEvent.WaterspoutMode.MOVING)
		{
			this.PickDestination();
		}
		else if (this.waterspoutMode == WaterspoutWorldEvent.WaterspoutMode.CHASING)
		{
			this.SeekPlayer();
		}
		else if (this.waterspoutMode == WaterspoutWorldEvent.WaterspoutMode.STATIC)
		{
			this.navMeshAgent.enabled = false;
		}
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.OnPlayerHit));
	}

	private void OnPlayerHit()
	{
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerHit));
		GameManager.Instance.AudioPlayer.PlaySFX(this.collisionSFXAsset, base.transform.position, 1f, this.audioSource.outputAudioMixerGroup, AudioRolloffMode.Linear, 50f, 100f, false, true);
		global::UnityEngine.Object.Instantiate<GameObject>(this.impactPrefab, GameManager.Instance.Player.transform.position, Quaternion.identity);
		PlayerCollider collider = GameManager.Instance.Player.Collider;
		collider.ProcessHit(false, false, base.gameObject.CompareTag(collider.uniqueVibrationTag));
		GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
		if (!(GameManager.Instance.Player == null))
		{
			bool isAlive = GameManager.Instance.Player.IsAlive;
		}
		if (global::UnityEngine.Random.value < this.itemAddChance && GameManager.Instance.Player != null && GameManager.Instance.Player.IsAlive)
		{
			SpatialItemData spatialItemData = this.itemPool.PickRandom<SpatialItemData>();
			SpatialItemInstance spatialItemInstance = null;
			if (GameManager.Instance.SaveData.Inventory.FindSpaceAndAddObjectToGridData(spatialItemData, true, out spatialItemInstance, null))
			{
				GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ITEM_ADDED, "notification.waterspout-item-added", spatialItemData.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
				GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance);
				GameEvents.Instance.TriggerFishCaught();
			}
		}
		this.RequestEventFinish();
	}

	private void SeekPlayer()
	{
		Vector3 position = GameManager.Instance.Player.transform.position;
		if (this.navMeshAgent.isOnNavMesh)
		{
			this.navMeshAgent.SetDestination(position);
		}
	}

	private void PickDestination()
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 10;
		while (num2 < num3 && num < this.maxTravelDistance * 0.5f)
		{
			this.destination = this.RandomNavMeshLocation(this.maxTravelDistance);
			num = Vector3.Distance(base.transform.position, this.destination);
		}
		if (this.navMeshAgent.isOnNavMesh)
		{
			this.navMeshAgent.SetDestination(this.destination);
		}
	}

	private Vector3 RandomNavMeshLocation(float radius)
	{
		Vector3 vector = global::UnityEngine.Random.onUnitSphere.normalized * radius + base.transform.position;
		Vector3 vector2 = Vector3.zero;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(vector, out navMeshHit, radius, 1))
		{
			vector2 = navMeshHit.position;
		}
		return vector2;
	}

	private void Update()
	{
		if (this.capSpeedWhenHarvesting)
		{
			this.navMeshAgent.speed = Mathf.Lerp(this.navMeshAgent.speed, GameManager.Instance.UI.IsHarvesting ? this.speedWhenHarvesting : this.freeSpeed, Time.deltaTime);
		}
		if (!this.finishRequested && (Time.time > this.spawnTime + base.worldEventData.durationSec || Vector3.Distance(base.transform.position, this.destination) < 1f))
		{
			this.RequestEventFinish();
		}
		if (!this.finishRequested && this.waterspoutMode == WaterspoutWorldEvent.WaterspoutMode.CHASING && Time.time > this.timeOfLastRepathToPlayer + this.repathToPlayerInterval)
		{
			this.SeekPlayer();
			this.timeOfLastRepathToPlayer = Time.time;
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.playerCollider.enabled = false;
			this.particleSystems.ForEach(delegate(ParticleSystem ps)
			{
				ps.Stop();
			});
			this.audioSource.DOFade(0f, this.finishDelaySec);
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.finishDelaySec);
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private List<ParticleSystem> particleSystems;

	[SerializeField]
	private AssetReference collisionSFXAsset;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float maxTravelDistance;

	[SerializeField]
	private float itemAddChance;

	[SerializeField]
	private List<SpatialItemData> itemPool;

	[SerializeField]
	private Collider playerCollider;

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private GameObject impactPrefab;

	[SerializeField]
	private WaterspoutWorldEvent.WaterspoutMode waterspoutMode;

	[SerializeField]
	private float moveSpeedScalar;

	[SerializeField]
	private float moveSpeedProportionalToPlayer;

	[SerializeField]
	private float accelerationFactor;

	[SerializeField]
	private float maxSpeed;

	[SerializeField]
	private float repathToPlayerInterval = 0.5f;

	[SerializeField]
	private bool capSpeedWhenHarvesting;

	[SerializeField]
	private LayerMask forbiddenSpawnLayers;

	private Vector3 destination;

	private float finishDelaySec;

	private float spawnTime;

	private float timeOfLastRepathToPlayer;

	private float freeSpeed;

	private float speedWhenHarvesting;

	private enum WaterspoutMode
	{
		STATIC,
		MOVING,
		CHASING
	}
}
