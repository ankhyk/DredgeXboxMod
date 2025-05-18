using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OozeEye : OozeEvent
{
	private void Awake()
	{
		this.spawnPosition = base.transform.position;
		this.idleDestination = this.spawnPosition;
		this.lifetimeSec = global::UnityEngine.Random.Range(this.lifetimeMinSec, this.lifetimeMaxSec);
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		AnimationEvents animationEvents2 = this.animationEvent;
		animationEvents2.OnComplete = (Action)Delegate.Remove(animationEvents2.OnComplete, new Action(this.OnDespawnComplete));
		AnimationEvents animationEvents3 = this.animationEvent;
		animationEvents3.OnSignalFired = (Action)Delegate.Remove(animationEvents3.OnSignalFired, new Action(this.DoDespawnVFX));
	}

	private void OnSpawnComplete()
	{
		this.spawnTime = Time.time;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnSignalFired = (Action)Delegate.Remove(animationEvents.OnSignalFired, new Action(this.DoDespawnVFX));
		AnimationEvents animationEvents2 = this.animationEvent;
		animationEvents2.OnComplete = (Action)Delegate.Combine(animationEvents2.OnComplete, new Action(this.OnDespawnComplete));
		this.isSpawnComplete = true;
		this.canSeekPlayer = true;
		this.PlayIdleCallSFX();
	}

	private void PlayAmbientCallSFX()
	{
		this.audioSource.PlayOneShot(this.ambientCallAudioClips.PickRandom<AudioClip>());
	}

	private void PlayIdleCallSFX()
	{
		this.audioSource.PlayOneShot(this.idleAudioClips.PickRandom<AudioClip>());
		this.timeOfLastIdleClip = Time.time;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (enabled && ability.name == this.banishAbility.name && !this.finishRequested)
		{
			this.QueueDespawn();
		}
	}

	private void Update()
	{
		if (this.canSeekPlayer && Time.time > this.timeOfLastSeekCheck + this.seekCheckIntervalSec)
		{
			this.TrySeekPlayer();
		}
		if (!this.finishRequested && this.isSpawnComplete && Time.time > this.spawnTime + this.lifetimeSec)
		{
			this.QueueDespawn();
		}
		if (!this.finishRequested && this.isWaitingForDespawnTimer && Time.time > this.timeOfReachedPlayer + this.timeUntilDespawn)
		{
			this.QueueDespawn();
		}
	}

	private void TrySeekPlayer()
	{
		this.timeOfLastSeekCheck = Time.time;
		float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
		float num2 = Vector3.Distance(this.spawnPosition, GameManager.Instance.Player.transform.position);
		if (num < this.seekRange && num2 < this.maxDistanceAwayFromSpawnPosition && !GameManager.Instance.Player.IsDocked)
		{
			this.navMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
			this.navMeshAgent.stoppingDistance = this.seekStoppingDistance;
			if (!this.isSeekingPlayer)
			{
				this.PlayAmbientCallSFX();
			}
			this.isSeekingPlayer = true;
			if (num <= this.navMeshAgent.stoppingDistance + 1f)
			{
				this.OnReachedPlayer();
			}
		}
		else
		{
			this.navMeshAgent.stoppingDistance = 0f;
			if (this.isSeekingPlayer)
			{
				this.idleDestination = this.spawnPosition;
			}
			if (Vector3.Distance(base.transform.position, this.idleDestination) < this.wanderArriveThreshold)
			{
				if (!this.isHoldingAtWanderDestination)
				{
					if (Time.time > this.timeOfLastIdleClip + this.timeBetweenIdleClips)
					{
						this.PlayIdleCallSFX();
					}
					this.wanderHoldDurationSec = global::UnityEngine.Random.Range(this.wanderHoldDurationSecMin, this.wanderHoldDurationSecMax);
					this.isHoldingAtWanderDestination = true;
					this.timeArrivedAtWanderDestination = Time.time;
				}
				Vector3 vector;
				if (Time.time > this.timeArrivedAtWanderDestination + this.wanderHoldDurationSec && OozePatchEvents.GetPositionInOoze(this.spawnPosition, this.wanderDistanceAwayFromSpawnPosition, 1f, out vector, false))
				{
					this.idleDestination = vector;
					this.isHoldingAtWanderDestination = false;
				}
			}
			this.isSeekingPlayer = false;
			this.navMeshAgent.SetDestination(this.idleDestination);
		}
		this.animator.SetBool("seeking", this.isSeekingPlayer);
	}

	private void OnReachedPlayer()
	{
		this.hasReachedPlayer = true;
		this.timeOfReachedPlayer = Time.time;
		this.ScheduleDespawn();
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			this.QueueDespawn();
			base.RequestEventFinish();
		}
	}

	private void ScheduleDespawn()
	{
		this.timeUntilDespawn = global::UnityEngine.Random.Range(this.quickDespawnTimeSecMin, this.quickDespawnTimeSecMax);
		this.isWaitingForDespawnTimer = true;
		this.canSeekPlayer = false;
	}

	private void QueueDespawn()
	{
		this.finishRequested = true;
		this.canSeekPlayer = false;
		this.sanityModifier.SetActive(false);
		this.vFXVolumeFader.BlendDurationSec = this.despawnDurationSec;
		base.StartCoroutine(this.vFXVolumeFader.FadeOut());
		this.audioSource.PlayOneShot(this.attackAudioClips.PickRandom<AudioClip>());
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnSignalFired = (Action)Delegate.Combine(animationEvents.OnSignalFired, new Action(this.DoDespawnVFX));
		this.animator.SetTrigger("banish");
	}

	private void DoDespawnVFX()
	{
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnSignalFired = (Action)Delegate.Remove(animationEvents.OnSignalFired, new Action(this.DoDespawnVFX));
		global::UnityEngine.Object.Instantiate<GameObject>(this.despawnVFX, this.vfxSpawnLocation.position, Quaternion.identity);
	}

	private void OnDespawnComplete()
	{
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnDespawnComplete));
		Action<OozeEvent> onOozeEventComplete = this.OnOozeEventComplete;
		if (onOozeEventComplete != null)
		{
			onOozeEventComplete(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float lifetimeMinSec;

	[SerializeField]
	private float lifetimeMaxSec;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private AnimationEvents animationEvent;

	[SerializeField]
	private GameObject despawnVFX;

	[SerializeField]
	private Transform vfxSpawnLocation;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float maxDistanceAwayFromSpawnPosition;

	[SerializeField]
	private float wanderDistanceAwayFromSpawnPosition;

	[SerializeField]
	private float wanderArriveThreshold;

	[SerializeField]
	private float wanderHoldDurationSecMin;

	[SerializeField]
	private float wanderHoldDurationSecMax;

	[SerializeField]
	private float seekStoppingDistance;

	[SerializeField]
	private float seekRange;

	[SerializeField]
	private float seekCheckIntervalSec;

	[SerializeField]
	private float despawnDurationSec;

	[SerializeField]
	private GameObject sanityModifier;

	[SerializeField]
	private VFXVolumeFader vFXVolumeFader;

	[SerializeField]
	private float chanceOfBeingQuickDespawner;

	[SerializeField]
	private float quickDespawnTimeSecMin;

	[SerializeField]
	private float quickDespawnTimeSecMax;

	[SerializeField]
	private float timeBetweenIdleClips;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> attackAudioClips = new List<AudioClip>();

	[SerializeField]
	private List<AudioClip> ambientCallAudioClips = new List<AudioClip>();

	[SerializeField]
	private List<AudioClip> idleAudioClips = new List<AudioClip>();

	private bool canCall;

	private bool isWaitingForDespawnTimer;

	private float timeOfReachedPlayer;

	private float timeUntilDespawn;

	private bool hasReachedPlayer;

	private Vector3 spawnPosition;

	private Vector3 idleDestination;

	private bool isSpawnComplete;

	private bool canSeekPlayer;

	private bool isSeekingPlayer;

	private float lifetimeSec;

	private float spawnTime;

	private float timeOfLastSeekCheck;

	private float timeArrivedAtWanderDestination;

	private float wanderHoldDurationSec;

	private bool isHoldingAtWanderDestination;

	private float timeOfLastIdleClip;
}
