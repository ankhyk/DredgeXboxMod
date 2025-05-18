using System;
using UnityEngine;
using UnityEngine.AI;

public class DSBigMonster : MonoBehaviour
{
	public Transform AnchorTransform
	{
		get
		{
			return this.anchorTransform;
		}
		set
		{
			this.anchorTransform = value;
		}
	}

	public float DeaggroRange
	{
		get
		{
			return this.deaggroRange;
		}
		set
		{
			this.deaggroRange = value;
		}
	}

	private void Awake()
	{
		this.targetFollow.enabled = false;
		this.simplePathFollow.enabled = false;
	}

	public void Init(RouteConfig routeConfig)
	{
		this.simplePathFollow.Init(routeConfig);
	}

	private void OnEnable()
	{
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Combine(targetFollow.OnPathError, new Action(this.OnTargetFollowError));
		DSBigMonsterAnimationEvents dsbigMonsterAnimationEvents = this.animationEvents;
		dsbigMonsterAnimationEvents.OnLungeStart = (Action)Delegate.Combine(dsbigMonsterAnimationEvents.OnLungeStart, new Action(this.OnLungeStart));
		DSBigMonsterAnimationEvents dsbigMonsterAnimationEvents2 = this.animationEvents;
		dsbigMonsterAnimationEvents2.OnAttackComplete = (Action)Delegate.Combine(dsbigMonsterAnimationEvents2.OnAttackComplete, new Action(this.OnAttackComplete));
		GameEvents.Instance.OnMonsterAttachedToPlayer += this.OnMonsterAttachedToPlayer;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		this.timeUntilNextIdleCall = global::UnityEngine.Random.Range(this.timeBetweenIdleCallsMin, this.timeBetweenIdleCallsMax);
	}

	private void OnDisable()
	{
		TargetFollow targetFollow = this.targetFollow;
		targetFollow.OnPathError = (Action)Delegate.Remove(targetFollow.OnPathError, new Action(this.OnTargetFollowError));
		DSBigMonsterAnimationEvents dsbigMonsterAnimationEvents = this.animationEvents;
		dsbigMonsterAnimationEvents.OnLungeStart = (Action)Delegate.Remove(dsbigMonsterAnimationEvents.OnLungeStart, new Action(this.OnLungeStart));
		DSBigMonsterAnimationEvents dsbigMonsterAnimationEvents2 = this.animationEvents;
		dsbigMonsterAnimationEvents2.OnAttackComplete = (Action)Delegate.Remove(dsbigMonsterAnimationEvents2.OnAttackComplete, new Action(this.OnAttackComplete));
		GameEvents.Instance.OnMonsterAttachedToPlayer -= this.OnMonsterAttachedToPlayer;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (ability.name == this.banishAbility.name)
		{
			this.isBanishActive = enabled;
			if (this.isBanishActive && this.currentMode == DSBigMonster.DSBigMonsterMode.CHASING)
			{
				GameEvents.Instance.TriggerThreatBanished(true);
				this.ReturnToPath();
			}
		}
	}

	private void OnMonsterAttachedToPlayer()
	{
		if (Time.time > this.timeOfLastResponseCall + this.timeBetweenResponseCalls && this.timeUntilResponseCall > 1.5f)
		{
			this.timeUntilResponseCall = 1.5f;
		}
	}

	private void OnTargetFollowError()
	{
		this.ReturnToPath();
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
		this.boatHitEffects.SetActive(false);
		this.timeOfLastAttack = Time.time;
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.OnPlayerHit));
		this.animator.SetTrigger("attack");
	}

	private void OnLungeStart()
	{
		GameEvents.Instance.TriggerDismissAndSuppressDSLittleMonsters(this.littleMonsterSuppressionDuration);
	}

	private void OnAttackComplete()
	{
		this.timeOfLastAttack = Time.time;
		this.navMeshAgent.enabled = true;
		this.ReturnToPath();
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerHit));
	}

	private void OnPlayerHit()
	{
		this.boatHitEffects.SetActive(true);
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerHit));
		this.audioSource.PlayOneShot(this.attackAudioClip);
		GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
		this.navMeshAgent.enabled = false;
		if (this.instaKill)
		{
			GameManager.Instance.Player.Die();
			return;
		}
		int num = this.damagePoints;
		if (GameManager.Instance.SettingsSaveData.CurrentGameMode() == GameMode.NIGHTMARE)
		{
			num++;
		}
		GameManager.Instance.GridManager.AddDamageToInventory(num, -1, -1);
	}

	private void Update()
	{
		if (this.player == null)
		{
			this.player = GameManager.Instance.Player;
			if (this.player)
			{
				this.targetFollow.Init(this.player.gameObject);
			}
			return;
		}
		this.timeUntilResponseCall -= Time.deltaTime;
		if (this.timeUntilResponseCall <= 0f)
		{
			this.timeUntilResponseCall = float.PositiveInfinity;
			this.timeOfLastResponseCall = Time.time;
			this.audioSource.pitch = global::UnityEngine.Random.Range(this.audioClipPitchMin, this.audioClipPitchMax);
			this.audioSource.PlayOneShot(this.responseCallAudioClip);
		}
		if (this.currentMode == DSBigMonster.DSBigMonsterMode.PATROLLING || this.currentMode == DSBigMonster.DSBigMonsterMode.CHASING)
		{
			this.timeUntilNextIdleCall -= Time.deltaTime;
			if (this.timeUntilNextIdleCall < 0f && Time.time > this.timeOfLastResponseCall + this.timeBetweenResponseCalls)
			{
				this.timeUntilNextIdleCall = global::UnityEngine.Random.Range(this.timeBetweenIdleCallsMin, this.timeBetweenIdleCallsMax);
				this.audioSource.pitch = global::UnityEngine.Random.Range(this.audioClipPitchMin, this.audioClipPitchMax);
				this.audioSource.PlayOneShot(this.idleCallAudioClip);
			}
		}
		if (!this.isBanishActive && this.currentMode == DSBigMonster.DSBigMonsterMode.CHASING && Time.time > this.timeOfLastAttack + this.attackDelay && Vector3.Distance(base.transform.position, this.player.transform.position) < this.attackDistanceThreshold)
		{
			this.TryDoAttack();
		}
		if (this.currentMode == DSBigMonster.DSBigMonsterMode.NONE)
		{
			this.ReturnToPath();
		}
		else if (!this.isBanishActive && this.currentMode == DSBigMonster.DSBigMonsterMode.PATROLLING && GameManager.Instance.PlayerStats.AttachedMonsterCount > 0 && this.proximityMonitor.CurrentProximity < this.playerDetectionThreshold && Time.time > this.timeOfLastAttack + this.attackDelay)
		{
			this.BeginChasing();
		}
		else if (this.currentMode == DSBigMonster.DSBigMonsterMode.CHASING && GameManager.Instance.PlayerStats.AttachedMonsterCount <= 0)
		{
			this.ReturnToPath();
		}
		this.navMeshAgent.speed = Mathf.Lerp(this.navMeshAgent.speed, this.currentSpeedTarget, Time.deltaTime);
	}

	private void BeginChasing()
	{
		if (Vector3.Distance(this.targetFollow.Target.transform.position, this.AnchorTransform.position) < this.deaggroRange)
		{
			this.simplePathFollow.enabled = false;
			this.targetFollow.enabled = true;
			this.currentMode = DSBigMonster.DSBigMonsterMode.CHASING;
			this.currentSpeedTarget = this.chaseSpeed;
		}
	}

	private void ReturnToPath()
	{
		this.targetFollow.enabled = false;
		this.simplePathFollow.enabled = true;
		this.simplePathFollow.MoveToClosestPathPoint();
		this.currentMode = DSBigMonster.DSBigMonsterMode.PATROLLING;
		this.currentSpeedTarget = this.patrolSpeed;
	}

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private SimplePathFollow simplePathFollow;

	[SerializeField]
	private TargetFollow targetFollow;

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private float patrolSpeed;

	[SerializeField]
	private float chaseSpeed;

	[SerializeField]
	private float playerDetectionThreshold;

	[SerializeField]
	private PlayerProximityMonitor proximityMonitor;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip idleCallAudioClip;

	[SerializeField]
	private AudioClip responseCallAudioClip;

	[SerializeField]
	private AudioClip attackAudioClip;

	[SerializeField]
	private GameObject boatHitEffects;

	[SerializeField]
	private float audioClipPitchMin;

	[SerializeField]
	private float audioClipPitchMax;

	[SerializeField]
	private float timeBetweenResponseCalls;

	[SerializeField]
	private float timeBetweenIdleCallsMin;

	[SerializeField]
	private float timeBetweenIdleCallsMax;

	[SerializeField]
	private float littleMonsterSuppressionDuration;

	[SerializeField]
	private float attackDelay;

	[SerializeField]
	private float attackDistanceThreshold;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private VibrationData aggroVibration;

	[SerializeField]
	private bool instaKill;

	[SerializeField]
	private LayerMask viewLayerMask;

	[SerializeField]
	private int damagePoints;

	[SerializeField]
	private DSBigMonsterAnimationEvents animationEvents;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private DSLittleMonsterSpawner spawner;

	[SerializeField]
	private AbilityData banishAbility;

	private float timeOfLastResponseCall;

	private float timeUntilNextIdleCall;

	private DSBigMonster.DSBigMonsterMode currentMode;

	private RouteConfig routeConfig;

	private Transform anchorTransform;

	private float deaggroRange;

	private Player player;

	private float currentSpeedTarget;

	private float timeOfLastAttack;

	private bool isBanishActive;

	private float timeUntilResponseCall = float.PositiveInfinity;

	private enum DSBigMonsterMode
	{
		NONE,
		PATROLLING,
		CHASING
	}
}
