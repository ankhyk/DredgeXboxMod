using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class GCMonster : MonoBehaviour, IGameModeResponder
{
	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && (this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST))
		{
			this.DoLosePlayer();
		}
	}

	private void Start()
	{
		SimplePathFollow simplePathFollow = this.pathFollow;
		simplePathFollow.OnPathComplete = (Action)Delegate.Combine(simplePathFollow.OnPathComplete, new Action(this.OnPathComplete));
		TargetMove targetMove = this.targetMove;
		targetMove.OnPathComplete = (Action)Delegate.Combine(targetMove.OnPathComplete, new Action(this.OnMoveToPlayerGhostComplete));
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Combine(targetFollow.OnPathError, new Action(this.OnTargetFollowError));
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbilityData.name && enabled)
		{
			GameEvents.Instance.TriggerThreatBanished(this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST || this.currentState == GaleCliffsMonsterMode.ATTACKING);
			this.banishAudio.Play();
			this.isBanishActive = enabled;
			this.canEyesSee = false;
			this.forceEyesShut = true;
			this.MoveToDespawn();
		}
	}

	public void Init(MonsterData monsterData, List<Transform> pathPoints, RouteDirection direction)
	{
		this.monsterData = monsterData;
		this.patrolRoute = default(RouteConfig);
		this.patrolRoute.startIndex = ((direction == RouteDirection.FORWARDS) ? 0 : (pathPoints.Count - 1));
		this.patrolRoute.route = pathPoints;
		this.patrolRoute.direction = direction;
		this.currentSpeedTarget = monsterData.patrolSpeed;
		this.lerpedSpeedValue = this.currentSpeedTarget;
		this.DoPatrolRoute(this.patrolRoute);
	}

	private void Update()
	{
		if (this.player == null)
		{
			this.player = GameManager.Instance.Player;
			return;
		}
		this.canEyesSee = !this.forceEyesShut && !this.isBanishActive && this.depthMonitor.CurrentDepth >= this.attackDepthThreshold;
		if (this.canEyesSee && this.currentGameMode != GameMode.PASSIVE && this.currentState != GaleCliffsMonsterMode.MOVING_TO_PLAYER && this.currentState != GaleCliffsMonsterMode.ATTACKING)
		{
			if (this.eyes.Any((FieldOfView e) => e.TrackedObject != null))
			{
				GameObject trackedObject = this.eyes.Find((FieldOfView e) => e.TrackedObject != null).TrackedObject;
				if (this.CanReachTarget(trackedObject))
				{
					this.OnPlayerDetected(trackedObject);
				}
			}
		}
		if (this.canEyesSee && this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER && this.losePlayerCoroutine == null)
		{
			if (!this.eyes.Any((FieldOfView e) => e.TrackedObject != null))
			{
				this.OnPlayerDetectionLost();
			}
		}
		if (this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST)
		{
			this.chaseDuration += Time.deltaTime;
			if (this.chaseDuration >= this.maxChaseTimeSec)
			{
				this.DoCallAudio();
				this.MoveToDespawn();
			}
		}
		if (this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER && Vector3.Distance(base.transform.position, this.player.transform.position) < this.attackDistanceThreshold)
		{
			this.TryDoAttack();
		}
		if (this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.ATTACKING)
		{
			float num = Mathf.InverseLerp(this.proximityAnimatorThresholdNear, this.proximityAnimatorThresholdFar, this.playerProximityMonitor.CurrentProximity);
			this.currentProximityTarget = 1f - num;
		}
		else
		{
			this.currentProximityTarget = (this.isBanishActive ? (-1f) : 0f);
		}
		this.lerpedProximityValue = Mathf.Lerp(this.lerpedProximityValue, this.currentProximityTarget, Time.deltaTime);
		this.playerProximityAnimator.SetFloat("Proximity", this.lerpedProximityValue);
		float num2;
		switch (this.currentState)
		{
		case GaleCliffsMonsterMode.MOVING_TO_PLAYER:
		case GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST:
			num2 = this.monsterData.huntSpeed;
			goto IL_0294;
		case GaleCliffsMonsterMode.MOVING_TO_DESPAWN:
			num2 = this.monsterData.fleeSpeed;
			goto IL_0294;
		case GaleCliffsMonsterMode.ATTACKING:
			num2 = this.monsterData.huntSpeed * this.attackMovementSpeedMultiplier;
			goto IL_0294;
		}
		num2 = this.monsterData.patrolSpeed;
		IL_0294:
		float num3;
		if (this.currentState == GaleCliffsMonsterMode.ATTACKING)
		{
			num3 = this.moveSpeedScalar * Mathf.Clamp(num2 * GameManager.Instance.PlayerStats.MovementSpeedModifier, this.boatSpeedMin, this.attackMaxBoatSpeed);
		}
		else
		{
			num3 = this.moveSpeedScalar * Mathf.Clamp(num2 * GameManager.Instance.PlayerStats.MovementSpeedModifier, this.boatSpeedMin, this.boatSpeedMax);
		}
		this.lerpedSpeedValue = Mathf.Lerp(this.lerpedSpeedValue, num3, Time.deltaTime);
		this.navMeshAgent.speed = this.lerpedSpeedValue;
		this.DoAmbientAudio();
		this.TryDoCallAudio();
	}

	private void DoAmbientAudio()
	{
		if (this.shouldBlendAmbientVolumes)
		{
			this.idleLoopAudioVolume = Mathf.Lerp(this.idleLoopAudioVolume, (float)(this.targetFollow.enabled ? 0 : 1), Time.deltaTime);
			this.idleLoopAudio.volume = this.idleLoopAudioVolume;
			this.aggroLoopAudio.volume = 1f - this.idleLoopAudioVolume;
		}
	}

	private void TryDoCallAudio()
	{
		if (this.currentState == GaleCliffsMonsterMode.PATROLLING || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST)
		{
			this.timeUntilNextCallAudio -= Time.deltaTime;
			if (this.timeUntilNextCallAudio <= 0f)
			{
				this.DoCallAudio();
			}
		}
	}

	private void DoCallAudio()
	{
		if (Time.time < this.timeOfLastCallAudioClipPlayed + this.callAudioDelayMin)
		{
			return;
		}
		if (this.currentState == GaleCliffsMonsterMode.PATROLLING)
		{
			this.callAudio.clip = this.idleCalls.PickRandom<AudioClip>();
		}
		else if (this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER || this.currentState == GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST)
		{
			this.callAudio.clip = this.aggroCalls.PickRandom<AudioClip>();
		}
		this.callAudio.pitch = global::UnityEngine.Random.Range(this.callAudioPitchMin, this.callAudioPitchMax);
		this.callAudio.Play();
		this.timeOfLastCallAudioClipPlayed = Time.time;
		this.timeUntilNextCallAudio = global::UnityEngine.Random.Range(this.callAudioDelayMin, this.callAudioDelayMax);
		GameManager.Instance.VibrationManager.Vibrate(this.callVibration, VibrationRegion.WholeBody, true);
	}

	private bool CanReachTarget(GameObject target)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		Vector3 position = target.transform.position;
		position.y = base.transform.position.y;
		this.navMeshAgent.CalculatePath(position, navMeshPath);
		bool flag = false;
		if (navMeshPath.corners.Length != 0)
		{
			flag = Vector3.Distance(position, navMeshPath.corners[navMeshPath.corners.Length - 1]) < this.arriveThreshold;
		}
		return flag;
	}

	public void OnPlayerDetected(GameObject gameObject)
	{
		if (this.losePlayerCoroutine != null)
		{
			base.StopCoroutine(this.losePlayerCoroutine);
			this.losePlayerCoroutine = null;
		}
		this.animator.SetBool("detectsPlayer", true);
		if (this.currentState == GaleCliffsMonsterMode.PATROLLING)
		{
			this.chaseDuration = 0f;
		}
		this.currentState = GaleCliffsMonsterMode.MOVING_TO_PLAYER;
		this.targetMove.enabled = false;
		this.targetFollow.enabled = true;
		this.targetFollow.Init(gameObject);
		if (Time.time > this.timeOfLastPlayerDetectionAudioClipPlayed + this.timeBetweenPlayerDetectionAudioClips)
		{
			this.timeOfLastPlayerDetectionAudioClipPlayed = Time.time;
			this.playerDetectedAudio.Play();
			this.DoCallAudio();
		}
	}

	private void OnPlayerDetectionLost()
	{
		this.losePlayerCoroutine = base.StartCoroutine(this.LosePlayerDelayed(this.monsterData.playerLostThreshold));
	}

	private IEnumerator LosePlayerDelayed(float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		this.DoLosePlayer();
		yield break;
	}

	private void OnTargetFollowError()
	{
		this.DoLosePlayer();
	}

	private void DoLosePlayer()
	{
		if (this.losePlayerCoroutine != null)
		{
			base.StopCoroutine(this.losePlayerCoroutine);
			this.losePlayerCoroutine = null;
		}
		this.targetFollow.enabled = false;
		this.animator.SetBool("detectsPlayer", false);
		GameObject target = this.targetFollow.Target;
		if (this.CanReachTarget(target))
		{
			this.currentState = GaleCliffsMonsterMode.MOVING_TO_PLAYER_GHOST;
			this.targetMove.enabled = true;
			this.targetMove.Init(target);
			return;
		}
		this.FindClosestPatrolRoute();
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
		this.currentState = GaleCliffsMonsterMode.ATTACKING;
		this.didJustHitPlayer = false;
		this.variablePlayerDamager.AddListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Combine(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		this.animator.SetTrigger("attack");
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAttackComplete));
	}

	private void OnAttackComplete()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAttackComplete));
		this.variablePlayerDamager.RemoveListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		if (!this.didJustHitPlayer)
		{
			this.currentState = GaleCliffsMonsterMode.MOVING_TO_PLAYER;
		}
	}

	private void OnPlayerHit()
	{
		this.didJustHitPlayer = true;
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
		this.hitVFX.SetActive(true);
		this.MoveToDespawn();
		GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
	}

	private void OnMoveToPlayerGhostComplete()
	{
		this.FindClosestPatrolRoute();
	}

	private void FindClosestPatrolRoute()
	{
		RouteConfig routeConfig = this.FindClosestRoute();
		if (routeConfig.route != null)
		{
			this.DoPatrolRoute(routeConfig);
			return;
		}
		Debug.LogWarning("[GaleCliffsMonster] OnMoveToPlayerGhostComplete() couldn't find any paths to return to. Attempting to move to a despawn location.");
		this.MoveToDespawn();
	}

	private void DoPatrolRoute(RouteConfig route)
	{
		this.DoCallAudio();
		this.currentState = GaleCliffsMonsterMode.PATROLLING;
		this.targetFollow.enabled = false;
		this.targetMove.enabled = false;
		this.pathFollow.enabled = true;
		this.pathFollow.Init(route);
	}

	private void OnPathComplete()
	{
		this.pathFollow.enabled = false;
		if (this.currentState == GaleCliffsMonsterMode.PATROLLING || this.currentState == GaleCliffsMonsterMode.MOVING_TO_DESPAWN)
		{
			this.Despawn();
		}
	}

	private void MoveToDespawn()
	{
		this.targetMove.enabled = false;
		this.targetFollow.enabled = false;
		this.forceEyesShut = true;
		this.animator.SetBool("detectsPlayer", false);
		RouteConfig routeConfig = this.FindClosestExitRoute();
		if (routeConfig.route != null)
		{
			this.currentState = GaleCliffsMonsterMode.MOVING_TO_DESPAWN;
			this.pathFollow.enabled = true;
			this.pathFollow.Init(routeConfig);
			return;
		}
		Debug.LogWarning("[GaleCliffsMonster] MoveToDespawn() couldn't find any exit routes. Destroying.");
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Despawn()
	{
		this.shouldBlendAmbientVolumes = false;
		this.currentState = GaleCliffsMonsterMode.NONE;
		this.volumeFader.BlendDurationSec = this.ambienceFadeDurationSec;
		base.StartCoroutine(this.volumeFader.FadeOut());
		Tweener volumeTween = DOTween.To(() => this.idleLoopAudio.volume, delegate(float x)
		{
			this.idleLoopAudio.volume = x;
		}, 0f, this.ambienceFadeDurationSec);
		volumeTween.OnComplete(delegate
		{
			volumeTween = null;
			global::UnityEngine.Object.Destroy(this.gameObject);
		});
	}

	private void OnDestroy()
	{
		SimplePathFollow simplePathFollow = this.pathFollow;
		simplePathFollow.OnPathComplete = (Action)Delegate.Remove(simplePathFollow.OnPathComplete, new Action(this.OnPathComplete));
		TargetMove targetMove = this.targetMove;
		targetMove.OnPathComplete = (Action)Delegate.Remove(targetMove.OnPathComplete, new Action(this.OnMoveToPlayerGhostComplete));
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Remove(targetFollow.OnPathError, new Action(this.OnTargetFollowError));
		Action onMonsterDespawned = this.OnMonsterDespawned;
		if (onMonsterDespawned == null)
		{
			return;
		}
		onMonsterDespawned();
	}

	public RouteConfig FindClosestExitRoute()
	{
		float bestScore = float.NegativeInfinity;
		List<Transform> bestRoute = null;
		RouteConfig routeConfig = default(RouteConfig);
		GameManager.Instance.MonsterManager.GaleCliffsMonsterManager.RouteReferences.ExitRoutes.ForEach(delegate(List<Transform> r)
		{
			Vector3 position = r[0].position;
			Vector3 vector = position - this.transform.position;
			float num = Vector3.Distance(position, this.transform.position);
			float num2 = Vector3.Dot(vector, this.transform.forward) / (num + 0.01f);
			if (num2 > bestScore)
			{
				bestScore = num2;
				bestRoute = r;
			}
		});
		if (bestRoute == null)
		{
			Debug.LogWarning("[GaleCliffsMonster] FindClosestExitRoute() could not find a route.");
		}
		else
		{
			routeConfig.route = bestRoute;
			routeConfig.startIndex = 0;
			routeConfig.direction = RouteDirection.FORWARDS;
		}
		return routeConfig;
	}

	private RouteConfig FindClosestRoute()
	{
		RouteConfig routeConfig = default(RouteConfig);
		float num = float.NegativeInfinity;
		int num2 = -1;
		EntityPath entityPath = null;
		List<EntityPath> routes = GameManager.Instance.MonsterManager.GaleCliffsMonsterManager.RouteReferences.Routes;
		for (int i = 0; i < routes.Count; i++)
		{
			EntityPath entityPath2 = routes[i];
			for (int j = 1; j < entityPath2.Route.Count - 1; j++)
			{
				Vector3 position = entityPath2.Route[j].position;
				Vector3 vector = position - base.transform.position;
				float num3 = Vector3.Distance(position, base.transform.position);
				float num4 = Vector3.Dot(vector, base.transform.forward) / (num3 + 0.01f);
				if (num4 > num)
				{
					num = num4;
					entityPath = entityPath2;
					num2 = j;
				}
			}
		}
		if (entityPath != null)
		{
			routeConfig.route = entityPath.Route;
			routeConfig.startIndex = num2;
			if (num2 > 0 && num2 < routeConfig.route.Count - 1)
			{
				Vector3 vector2 = routeConfig.route[num2].position - new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				Vector3 vector3 = routeConfig.route[num2 - 1].position - routeConfig.route[num2].position;
				Vector3 vector4 = routeConfig.route[num2 + 1].position - routeConfig.route[num2].position;
				float num5 = Vector3.Dot(vector2.normalized, vector4.normalized);
				float num6 = Vector3.Dot(vector2.normalized, vector3.normalized);
				routeConfig.direction = ((num5 > num6) ? RouteDirection.FORWARDS : RouteDirection.BACKWARDS);
			}
		}
		else
		{
			Debug.LogWarning("[GaleCliffsMonster] FindClosestRoute() could not find a route.");
		}
		return routeConfig;
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private SimplePathFollow pathFollow;

	[SerializeField]
	private TargetFollow targetFollow;

	[SerializeField]
	private TargetMove targetMove;

	[SerializeField]
	private VariablePlayerDamager variablePlayerDamager;

	[SerializeField]
	private GameObject hitVFX;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private VibrationData callVibration;

	[SerializeField]
	private List<FieldOfView> eyes;

	[SerializeField]
	private PlayerProximityMonitor playerProximityMonitor;

	[SerializeField]
	private Animator playerProximityAnimator;

	[SerializeField]
	private AssetReference attackSFX;

	[SerializeField]
	private AudioSource idleLoopAudio;

	[SerializeField]
	private AudioSource aggroLoopAudio;

	[SerializeField]
	private AudioSource callAudio;

	[SerializeField]
	private AudioSource banishAudio;

	[SerializeField]
	private AudioSource playerDetectedAudio;

	[SerializeField]
	private LayerMask viewLayerMask;

	[SerializeField]
	private List<AudioClip> aggroCalls;

	[SerializeField]
	private List<AudioClip> idleCalls;

	[SerializeField]
	private float attackMaxBoatSpeed;

	[SerializeField]
	private float attackMovementSpeedMultiplier;

	[SerializeField]
	private float attackDistanceThreshold;

	[SerializeField]
	private float maxChaseTimeSec;

	[SerializeField]
	private float timeBetweenPlayerDetectionAudioClips;

	[SerializeField]
	private float callAudioDelayMin;

	[SerializeField]
	private float callAudioDelayMax;

	[SerializeField]
	private float callAudioPitchMin;

	[SerializeField]
	private float callAudioPitchMax;

	[SerializeField]
	private float ambienceMaxVolume;

	[SerializeField]
	private float ambienceFadeDurationSec;

	[SerializeField]
	private float arriveThreshold;

	[SerializeField]
	private DepthMonitor depthMonitor;

	[SerializeField]
	private float attackDepthThreshold;

	[SerializeField]
	private float proximityAnimatorThresholdNear;

	[SerializeField]
	private float proximityAnimatorThresholdFar;

	[SerializeField]
	private float moveSpeedScalar;

	[SerializeField]
	private float boatSpeedMin;

	[SerializeField]
	private float boatSpeedMax;

	[SerializeField]
	private VFXVolumeFader volumeFader;

	[SerializeField]
	private AbilityData banishAbilityData;

	private float timeOfLastPlayerDetectionAudioClipPlayed;

	private float timeOfLastCallAudioClipPlayed;

	public Action OnMonsterDespawned;

	private MonsterData monsterData;

	private RouteConfig patrolRoute;

	private Coroutine losePlayerCoroutine;

	private Vector3 playerCachedPosition;

	private List<Transform> pathPoints;

	private GaleCliffsMonsterMode currentState;

	private bool canEyesSee;

	private bool forceEyesShut;

	private bool isBanishActive;

	private float currentProximityTarget;

	private float lerpedProximityValue;

	private float currentSpeedTarget;

	private float lerpedSpeedValue;

	private bool shouldBlendAmbientVolumes = true;

	private float idleLoopAudioVolume;

	private float timeUntilNextCallAudio;

	private float chaseDuration;

	private Player player;

	private bool didJustHitPlayer;

	private GameMode currentGameMode;
}
