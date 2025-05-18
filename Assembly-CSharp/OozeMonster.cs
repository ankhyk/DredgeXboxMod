using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class OozeMonster : OozeEvent
{
	private void Awake()
	{
		this.spawnPosition = base.transform.position;
		this.lifetimeSec = global::UnityEngine.Random.Range(this.lifetimeMinSec, this.lifetimeMaxSec);
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Combine(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		this.vocalAudioSource.PlayOneShot(this.spawnSFX.PickRandom<AudioClip>());
		GameManager.Instance.VibrationManager.Vibrate(this.spawnVibrationData, VibrationRegion.WholeBody, true);
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
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
	}

	private void OnPlayerHit()
	{
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		if (this.hasHitPlayer)
		{
			return;
		}
		this.hasHitPlayer = true;
		global::UnityEngine.Object.Instantiate<GameObject>(this.playerHitVFX, GameManager.Instance.Player.transform.position, Quaternion.identity);
		this.impulseSource.GenerateImpulse();
		this.hitVFX.SetActive(true);
	}

	private void OnSpawnComplete()
	{
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		this.spawnTime = Time.time;
		this.isSpawnComplete = true;
		this.navMeshAgent.speed = this.baseSpeed;
		this.UpdateChasePlayerLogic();
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (enabled && ability.name == this.banishAbility.name && !this.isQueuedToDespawn)
		{
			this.OnBanishCast();
		}
	}

	private void Update()
	{
		if (this.isSpawnComplete && !this.isQueuedToDespawn && Time.time > this.spawnTime + this.lifetimeSec)
		{
			this.DoDespawn(false);
		}
		if (!this.isQueuedToDespawn && Time.time > this.timeOfLastRepath + this.repathIntervalSec)
		{
			this.UpdateChasePlayerLogic();
		}
		if (this.isWaitingForSpawnToFinishBeforeCanDespawn && this.isSpawnComplete)
		{
			this.isWaitingForSpawnToFinishBeforeCanDespawn = false;
			this.DoDespawn(false);
		}
	}

	private void DoDespawnVFX()
	{
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnSignalFired = (Action)Delegate.Remove(animationEvents.OnSignalFired, new Action(this.DoDespawnVFX));
		global::UnityEngine.Object.Instantiate<GameObject>(this.despawnVFX, this.vfxSpawnLocation.position, Quaternion.identity);
	}

	private void UpdateChasePlayerLogic()
	{
		this.timeOfLastRepath = Time.time;
		if (GameManager.Instance.Player)
		{
			float num = Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position);
			if (num < this.proximityForAchievement)
			{
				this.hasBeenCloseEnoughToPlayerForAchievement = true;
			}
			if (this.isSpawnComplete && num <= this.lungeRange)
			{
				this.DoDespawn(true);
			}
			this.navMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
		}
	}

	private void DoDespawn(bool withLungeAcceleration)
	{
		if (this.isQueuedToDespawn)
		{
			return;
		}
		this.isQueuedToDespawn = true;
		if (withLungeAcceleration)
		{
			this.lungeTween = DOTween.To(() => this.navMeshAgent.speed, delegate(float x)
			{
				this.navMeshAgent.speed = x;
			}, this.lungeSpeed, this.lungeAccelerationTimeSec);
			this.vocalAttenuateTween = DOTween.To(() => this.vocalAudioSource.volume, delegate(float x)
			{
				this.vocalAudioSource.volume = x;
			}, 0f, this.lungeAccelerationTimeSec);
			this.swimAttenuateTween = DOTween.To(() => this.swimAudioSource.volume, delegate(float x)
			{
				this.swimAudioSource.volume = x;
			}, 0f, this.lungeAccelerationTimeSec);
		}
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		AnimationEvents animationEvents2 = this.animationEvent;
		animationEvents2.OnComplete = (Action)Delegate.Combine(animationEvents2.OnComplete, new Action(this.OnDespawnComplete));
		AnimationEvents animationEvents3 = this.animationEvent;
		animationEvents3.OnSignalFired = (Action)Delegate.Combine(animationEvents3.OnSignalFired, new Action(this.DoDespawnVFX));
		this.animator.SetTrigger("despawn");
		this.attackAudioSource.PlayOneShot(this.attackSFX);
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			if (this.isSpawnComplete)
			{
				this.DoDespawn(false);
				base.RequestEventFinish();
				return;
			}
			this.isWaitingForSpawnToFinishBeforeCanDespawn = true;
		}
	}

	private void OnBanishCast()
	{
		if (this.isQueuedToDespawn)
		{
			return;
		}
		this.isQueuedToDespawn = true;
		this.animator.SetTrigger("banish");
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnDespawnComplete));
		GameEvents.Instance.TriggerThreatBanished(true);
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
		if (this.hasBeenCloseEnoughToPlayerForAchievement && !this.hasHitPlayer)
		{
			GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.DLC_4_12, true);
		}
		DOTween.Kill(this.lungeTween, false);
		DOTween.Kill(this.vocalAttenuateTween, false);
		DOTween.Kill(this.swimAttenuateTween, false);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.lungeRange);
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
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float repathIntervalSec;

	[SerializeField]
	private float lungeRange;

	[SerializeField]
	private float baseSpeed;

	[SerializeField]
	private float lungeSpeed;

	[SerializeField]
	private float lungeAccelerationTimeSec;

	[SerializeField]
	private GameObject despawnVFX;

	[SerializeField]
	private Transform vfxSpawnLocation;

	[SerializeField]
	private GameObject playerHitVFX;

	[SerializeField]
	private VariablePlayerDamager variablePlayerDamager;

	[SerializeField]
	private CinemachineImpulseSource impulseSource;

	[SerializeField]
	private GameObject hitVFX;

	[SerializeField]
	private float proximityForAchievement;

	[SerializeField]
	private AudioSource swimAudioSource;

	[SerializeField]
	private AudioSource vocalAudioSource;

	[SerializeField]
	private AudioSource attackAudioSource;

	[SerializeField]
	private List<AudioClip> spawnSFX = new List<AudioClip>();

	[SerializeField]
	private AudioClip attackSFX;

	[SerializeField]
	private VibrationData spawnVibrationData;

	private Vector3 spawnPosition;

	private bool isSpawnComplete;

	private bool isQueuedToDespawn;

	private float lifetimeSec;

	private float spawnTime;

	private float timeOfLastRepath;

	private float timeOfLastSlapCheck;

	private Tween lungeTween;

	private Tween vocalAttenuateTween;

	private Tween swimAttenuateTween;

	private bool hasHitPlayer;

	private bool hasBeenCloseEnoughToPlayerForAchievement;

	private bool isWaitingForSpawnToFinishBeforeCanDespawn;
}
