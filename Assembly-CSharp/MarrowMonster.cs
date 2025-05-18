using System;
using System.Collections;
using System.Collections.Generic;
using SensorToolkit;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class MarrowMonster : MonoBehaviour, IGameModeResponder
{
	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			if (this.shouldSeekPlayer)
			{
				this.detectsPlayer = false;
				this.TransitionFromHuntingToPatrolling();
				return;
			}
		}
		else
		{
			this.sensor.enabled = false;
			this.sensor.enabled = true;
		}
	}

	public void Init(MonsterData monsterData, Player player, Transform[] pathPoints)
	{
		this.monsterData = monsterData;
		this.player = player;
		this.pathPoints = pathPoints;
		this.buoyantObject.objectDepth = monsterData.idleDepth;
		this.shouldFollowWaypoints = true;
		this.monsterCollider.enabled = false;
		this.path = new NavMeshPath();
		this.timeUntilNextCallAudio = global::UnityEngine.Random.Range(this.callAudioDelayMin, this.callAudioDelayMax);
		this.currentSpeedTarget = monsterData.patrolSpeed;
		this.lerpedSpeedValue = this.currentSpeedTarget;
		this.hasInit = true;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		this.hasInit = false;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	public void Despawn()
	{
		this.shouldDespawn = true;
		this.shouldFollowWaypoints = false;
		this.shouldSeekPlayer = false;
		this.shouldFleePlayer = true;
		this.detectsPlayer = false;
		this.isAttacking = false;
		base.StartCoroutine(this.volumeFader.FadeOut());
		this.buoyantObject.objectDepth = this.monsterData.disappearDepth;
		this.retreatAudio.Play();
		this.navMeshAgent.SetDestination(this.pathPoints[0].position);
		this.bubbles.Stop();
	}

	protected virtual void Update()
	{
		if (!this.hasInit)
		{
			return;
		}
		float num = this.monsterData.patrolSpeed;
		if (this.shouldDespawn)
		{
			this.despawnTimer += Time.deltaTime;
		}
		float time = GameManager.Instance.Time.Time;
		if (!this.shouldDespawn && time > this.monsterData.despawnTime && time < this.monsterData.spawnTime)
		{
			this.Despawn();
		}
		else
		{
			if (this.shouldSeekPlayer)
			{
				num = this.monsterData.huntSpeed;
				if (Time.time > this.playerLostTime + this.monsterData.playerLostThreshold)
				{
					this.TransitionFromHuntingToPatrolling();
				}
			}
			if (this.detectsPlayer && Time.time > this.playerTargetTime + this.playerTargetReevaluationInterval)
			{
				this.EvaluatePathToPlayer();
			}
			else if (this.shouldFleePlayer)
			{
				num = this.monsterData.fleeSpeed;
			}
			else if (this.shouldFollowWaypoints)
			{
				num = this.monsterData.patrolSpeed;
				if (this.pathIndex != -1 && Vector3.Distance(base.transform.position, this.pathPoints[this.pathIndex].position) < this.waypointDistanceThreshold)
				{
					this.pathIndex++;
					if (this.pathIndex >= this.pathPoints.Length)
					{
						this.pathIndex = 0;
					}
					this.hasWaypoint = false;
				}
				if (!this.hasWaypoint)
				{
					if (this.pathIndex == -1)
					{
						this.SeekClosestWaypoint();
					}
					this.navMeshAgent.SetDestination(this.pathPoints[this.pathIndex].position);
					this.targetPos = this.pathPoints[this.pathIndex].position;
					this.hasWaypoint = true;
				}
			}
			if (this.canDoDamage && !this.isAttacking && this.detectsPlayer && !this.shouldFleePlayer && Vector3.Distance(base.transform.position, this.player.transform.position) < this.attackDistanceThreshold)
			{
				this.TryDoAttack();
			}
		}
		float num2 = num * GameManager.Instance.PlayerStats.MovementSpeedModifier;
		float num3;
		if (this.isAttacking)
		{
			num2 *= this.attackSpeedBoostFactor;
			num3 = this.moveSpeedScalar * Mathf.Clamp(num2, this.boatSpeedMin, this.attackMaxBoatSpeed);
		}
		else
		{
			num3 = this.moveSpeedScalar * Mathf.Clamp(num2, this.boatSpeedMin, this.boatSpeedMax);
		}
		this.lerpedSpeedValue = Mathf.Lerp(this.lerpedSpeedValue, num3, Time.deltaTime);
		this.navMeshAgent.speed = this.lerpedSpeedValue;
		if (this.shouldDespawn && this.despawnTimer >= 5f && (Vector3.Distance(this.pathPoints[0].position, base.transform.position) < 5f || Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position) > this.monsterData.despawnDistanceThreshold))
		{
			Action onDespawned = this.OnDespawned;
			if (onDespawned != null)
			{
				onDespawned();
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		this.DoAmbientAudio();
		this.TryDoCallAudio();
	}

	private void TryDoAttack()
	{
		Vector3 normalized = (this.player.ColliderCenter.position - base.transform.position).normalized;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, normalized, out raycastHit, this.attackDistanceThreshold, this.viewLayerMask) && raycastHit.transform.gameObject.CompareTag("Player"))
		{
			this.DoAttack();
		}
	}

	private void DoAttack()
	{
		this.isAttacking = true;
		this.monsterCollider.enabled = true;
		this.variablePlayerDamager.AddListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Combine(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAttackAnimationComplete));
		this.stateAnimator.SetTrigger("attack");
	}

	private void OnAttackAnimationComplete()
	{
		this.variablePlayerDamager.RemoveListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAttackAnimationComplete));
		this.monsterCollider.enabled = false;
		this.isAttacking = false;
	}

	private void OnPlayerHit()
	{
		this.canDoDamage = false;
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
		this.hitVFX.SetActive(true);
		GameEvents.Instance.TriggerPlayerHitByMonster();
		base.StartCoroutine(this.DelayedDespawn());
		GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
	}

	private void DoAmbientAudio()
	{
		if (this.shouldFleePlayer)
		{
			this.idleLoopAudioVolume = Mathf.Lerp(this.idleLoopAudioVolume, 0f, Time.deltaTime);
			this.idleLoopAudio.volume = this.idleLoopAudioVolume;
			this.aggroLoopAudio.volume = this.idleLoopAudioVolume;
			return;
		}
		if (this.shouldBlendAmbientVolumes)
		{
			this.idleLoopAudioVolume = Mathf.Lerp(this.idleLoopAudioVolume, (float)(this.shouldSeekPlayer ? 0 : 1), Time.deltaTime);
			this.idleLoopAudio.volume = this.idleLoopAudioVolume;
			this.aggroLoopAudio.volume = 1f - this.idleLoopAudioVolume;
		}
	}

	private void TryDoCallAudio()
	{
		if (this.shouldFleePlayer)
		{
			return;
		}
		this.timeUntilNextCallAudio -= Time.deltaTime;
		if (this.timeUntilNextCallAudio <= 0f)
		{
			this.DoCallAudio();
		}
	}

	private void DoCallAudio()
	{
		if (this.shouldSeekPlayer)
		{
			this.aggroCallAudio.clip = this.aggroCallAudioClips.PickRandom<AudioClip>();
			this.aggroCallAudio.Play();
		}
		else
		{
			this.callAudio.clip = this.callAudioClips.PickRandom<AudioClip>();
			this.callAudio.Play();
		}
		this.timeUntilNextCallAudio = global::UnityEngine.Random.Range(this.callAudioDelayMin, this.callAudioDelayMax);
	}

	private void TransitionFromHuntingToPatrolling()
	{
		this.shouldSeekPlayer = false;
		this.shouldFollowWaypoints = true;
		this.hasWaypoint = false;
		this.SeekClosestWaypoint();
	}

	private void SeekClosestWaypoint()
	{
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < this.pathPoints.Length; i++)
		{
			float num3 = Vector3.Distance(base.transform.position, this.pathPoints[i].position);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		this.pathIndex = num2;
	}

	private void EvaluatePathToPlayer()
	{
		Vector3 vector = (this.detectsPlayer ? new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z) : this.cachedPlayerPosition) - base.transform.position;
		Vector3 vector2 = this.player.transform.position + vector.normalized * 2f;
		vector2.y = 0f;
		this.targetPos = vector2;
		this.playerTargetTime = Time.time;
		NavMesh.CalculatePath(base.transform.position, vector2, -1, this.path);
		if (this.path.status == NavMeshPathStatus.PathComplete)
		{
			this.shouldSeekPlayer = true;
			this.shouldFollowWaypoints = false;
			this.navMeshAgent.SetPath(this.path);
			return;
		}
		this.shouldSeekPlayer = false;
		this.hasWaypoint = false;
		this.shouldFollowWaypoints = true;
	}

	public void OnPlayerTargetDetected()
	{
		if (this.shouldFleePlayer)
		{
			return;
		}
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			return;
		}
		this.shouldSeekPlayer = true;
		this.shouldFollowWaypoints = false;
		this.detectsPlayer = true;
		this.playerLostTime = float.PositiveInfinity;
		if (Time.time > this.timeOfLastPlayerDetectionAudioClipPlayed + this.timeBetweenPlayerDetectionAudioClips)
		{
			this.DoCallAudio();
		}
	}

	public void OnPlayerTargetLost()
	{
		if (this.detectsPlayer)
		{
			this.detectsPlayer = false;
			this.playerLostTime = Time.time;
			this.cachedPlayerPosition = new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z);
		}
	}

	private IEnumerator DelayedDespawn()
	{
		yield return new WaitForSeconds(0.25f);
		this.monsterCollider.enabled = false;
		yield return new WaitForSeconds(0.25f);
		this.Despawn();
		yield break;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name && enabled)
		{
			GameEvents.Instance.TriggerThreatBanished(this.shouldSeekPlayer || this.isAttacking);
			this.monsterCollider.enabled = false;
			this.Despawn();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.targetPos, 1f);
		Gizmos.color = Color.red;
		if (this.navMeshAgent.path != null)
		{
			for (int i = 0; i < this.navMeshAgent.path.corners.Length; i++)
			{
				Gizmos.DrawSphere(this.navMeshAgent.path.corners[i], 0.5f);
				if (i < this.navMeshAgent.path.corners.Length - 1)
				{
					Gizmos.DrawLine(this.navMeshAgent.path.corners[i], this.navMeshAgent.path.corners[i + 1]);
				}
			}
		}
	}

	[SerializeField]
	private Animator stateAnimator;

	[SerializeField]
	private Animator positionAnimator;

	[SerializeField]
	private BuoyantObject buoyantObject;

	[SerializeField]
	private Collider monsterCollider;

	[SerializeField]
	public NavMeshAgent navMeshAgent;

	[SerializeField]
	private float waypointDistanceThreshold;

	[SerializeField]
	private float playerTargetReevaluationInterval;

	[SerializeField]
	private AudioSource idleLoopAudio;

	[SerializeField]
	private AudioSource aggroLoopAudio;

	[SerializeField]
	private AudioSource callAudio;

	[SerializeField]
	private AudioSource aggroCallAudio;

	[SerializeField]
	private List<AudioClip> callAudioClips;

	[SerializeField]
	private List<AudioClip> aggroCallAudioClips;

	[SerializeField]
	private AudioSource retreatAudio;

	[SerializeField]
	private float timeBetweenPlayerDetectionAudioClips;

	[SerializeField]
	private float callAudioDelayMin;

	[SerializeField]
	private float callAudioDelayMax;

	[SerializeField]
	private AbilityData banishAbilityData;

	[SerializeField]
	private VFXVolumeFader volumeFader;

	[SerializeField]
	private float moveSpeedScalar;

	[SerializeField]
	private float boatSpeedMin;

	[SerializeField]
	private float boatSpeedMax;

	[SerializeField]
	private float attackSpeedBoostFactor;

	[SerializeField]
	private float attackMaxBoatSpeed;

	[SerializeField]
	private float attackMovementSpeedMultiplier;

	[SerializeField]
	private float attackDistanceThreshold;

	[SerializeField]
	private AssetReference attackSFX;

	[SerializeField]
	private GameObject hitVFX;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private VariablePlayerDamager variablePlayerDamager;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private LayerMask viewLayerMask;

	[SerializeField]
	private ParticleSystem bubbles;

	[SerializeField]
	private RangeSensor sensor;

	[HideInInspector]
	public Action OnDespawned;

	private MonsterData monsterData;

	private bool isAttacking;

	private bool shouldDespawn;

	private bool shouldFollowWaypoints;

	private bool hasWaypoint;

	private bool shouldSeekPlayer;

	private bool shouldFleePlayer;

	private float playerLostTime = float.NegativeInfinity;

	private float playerTargetTime = float.NegativeInfinity;

	private float despawnTimer;

	private float timeOfLastPlayerDetectionAudioClipPlayed;

	private Player player;

	private NavMeshPath path;

	private Transform[] pathPoints;

	private Vector3 cachedPlayerPosition;

	private bool detectsPlayer;

	private int pathIndex = -1;

	private bool hasInit;

	private Vector3 targetPos = Vector3.zero;

	private bool shouldBlendAmbientVolumes = true;

	private float idleLoopAudioVolume;

	private float timeUntilNextCallAudio;

	private float currentSpeedTarget;

	private float lerpedSpeedValue;

	private bool canDoDamage = true;

	private GameMode currentGameMode;
}
