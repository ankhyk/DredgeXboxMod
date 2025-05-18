using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class MonsterRayWorldEvent : WorldEvent
{
	private void Awake()
	{
		this.navMeshAgent.updateRotation = false;
		this.prevPos = this.model.position;
		this.playerTransform = GameManager.Instance.Player.transform;
		this.lerpedSpeedValue = this.moveSpeed;
		base.transform.rotation = Quaternion.LookRotation(this.playerTransform.position - base.transform.position);
		this.materialCopy = new Material(this.material);
		this.meshRenderer.material = this.materialCopy;
		this.currentDissolveVal = 1f;
		this.OnDissolveUpdated();
	}

	public override void Activate()
	{
		base.Activate();
		this.swimAudio.DOFade(1f, 1f).From(0f, true, false);
		this.Spawn();
	}

	private void Spawn()
	{
		this.currentState = MonsterRayWorldEvent.MonsterRayState.SPAWNING;
		this.dissolveAudio.Play();
		this.LerpDissolveAmount(0f, this.spawnDurationSec, delegate
		{
			this.currentState = MonsterRayWorldEvent.MonsterRayState.FOLLOWING;
		});
	}

	private void Despawn()
	{
		this.currentState = MonsterRayWorldEvent.MonsterRayState.DESPAWNING;
		this.RequestEventFinish();
	}

	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, this.playerTransform.position);
		this.currentDestinationPos = this.playerTransform.position;
		if (this.currentState == MonsterRayWorldEvent.MonsterRayState.FOLLOWING)
		{
			if (num < this.attackRange)
			{
				this.timeWithinAttackRange += Time.deltaTime;
			}
			else
			{
				this.timeWithinAttackRange = 0f;
			}
			this.timeSpentFollowing += Time.deltaTime;
			if (this.timeSpentFollowing > this.maxFollowDurationSec)
			{
				this.Despawn();
			}
			else if (!this.hasHitPlayer && this.timeWithinAttackRange >= this.timeUntilAttack && Time.time > this.timeOfLastAttack + this.attackCooldownSec && (this.monsterRayType == MonsterRayWorldEvent.MonsterRayType.BITE_ATTACK || this.monsterRayType == MonsterRayWorldEvent.MonsterRayType.TAIL_ATTACK))
			{
				this.DoAttack();
			}
			else if (this.hasHitPlayer && Time.time > this.timeOfPlayerHit + this.despawnDelay)
			{
				this.Despawn();
			}
			else if (this.timeWithinAttackRange > this.timeUntilAttack && this.monsterRayType == MonsterRayWorldEvent.MonsterRayType.SHADOW)
			{
				this.Despawn();
			}
		}
		if (this.currentState == MonsterRayWorldEvent.MonsterRayState.DESPAWNING)
		{
			this.navMeshAgent.speed = 0f;
		}
		else
		{
			float num2 = Mathf.Clamp(this.moveSpeed * GameManager.Instance.PlayerStats.MovementSpeedModifier, this.boatSpeedMin, this.boatSpeedMax);
			float num3 = this.moveSpeedScalar * num2;
			this.lerpedSpeedValue = Mathf.Lerp(this.lerpedSpeedValue, num3, Time.deltaTime);
			this.navMeshAgent.speed = this.lerpedSpeedValue;
		}
		if (this.currentState != MonsterRayWorldEvent.MonsterRayState.DESPAWNING && num > this.navMeshAgent.stoppingDistance)
		{
			if (Time.time > this.timeOfLastDestinationSet + this.timeBetweenDestinationSetsSec)
			{
				this.navMeshAgent.SetDestination(this.currentDestinationPos);
				this.timeOfLastDestinationSet = Time.time;
				if (this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
				{
					this.Despawn();
				}
				if (GameManager.Instance.WorldEventManager.DoesHitSafeZone(this.currentDestinationPos))
				{
					this.Despawn();
				}
			}
			this.model.rotation = Quaternion.Slerp(this.model.rotation, Quaternion.LookRotation(this.model.position - this.prevPos), Time.deltaTime * this.rotationSpeed);
		}
		this.prevPos = this.model.position;
	}

	private void DoAttack()
	{
		this.currentState = MonsterRayWorldEvent.MonsterRayState.ATTACKING;
		this.variablePlayerDamager.AddListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Combine(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAttackAnimationComplete));
		if (this.monsterRayType == MonsterRayWorldEvent.MonsterRayType.BITE_ATTACK)
		{
			this.variablePlayerDamager.damagePoints = 1;
			this.animator.SetTrigger("bite-attack");
		}
		else if (this.monsterRayType == MonsterRayWorldEvent.MonsterRayType.TAIL_ATTACK)
		{
			this.variablePlayerDamager.damagePoints = 2;
			this.animator.SetTrigger("tail-attack");
		}
		this.attackAudio.clip = this.attackClips.PickRandom<AudioClip>();
		this.attackAudio.Play();
	}

	private void OnAttackAnimationComplete()
	{
		this.variablePlayerDamager.RemoveListeners();
		VariablePlayerDamager variablePlayerDamager = this.variablePlayerDamager;
		variablePlayerDamager.PlayerHit = (Action)Delegate.Remove(variablePlayerDamager.PlayerHit, new Action(this.OnPlayerHit));
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAttackAnimationComplete));
		this.timeWithinAttackRange = 0f;
		this.timeOfLastAttack = Time.time;
		this.currentState = MonsterRayWorldEvent.MonsterRayState.FOLLOWING;
	}

	private void OnPlayerHit()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
		this.hasHitPlayer = true;
		this.timeOfPlayerHit = Time.time;
		this.hitVFX.SetActive(true);
		GameManager.Instance.VibrationManager.Vibrate(this.hitVibration, VibrationRegion.WholeBody, true);
	}

	public override void RequestEventFinish()
	{
		this.currentState = MonsterRayWorldEvent.MonsterRayState.DESPAWNING;
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.sanityModifier.SetActive(false);
			this.swimAudio.DOFade(0f, this.despawnDurationSec);
			this.panicAudio.DOFade(0f, this.despawnDurationSec);
			this.dissolveAudio.Play();
			this.vFXVolumeFader.BlendDurationSec = this.despawnDurationSec;
			base.StartCoroutine(this.vFXVolumeFader.FadeOut());
			this.LerpDissolveAmount(1f, this.despawnDurationSec, delegate
			{
				this.EventFinished();
				global::UnityEngine.Object.Destroy(base.gameObject);
			});
		}
	}

	private Vector3 PickDespawnDestination()
	{
		float num = 0f;
		float num2 = 10f;
		int num3 = 0;
		int num4 = 10;
		Vector3 vector = Vector3.zero;
		while (num3 < num4 && num < num2 * 0.5f)
		{
			vector = this.RandomNavMeshLocation(num2);
			num = Vector3.Distance(base.transform.position, vector);
		}
		return vector;
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

	private void LerpDissolveAmount(float newDissolveAmount, float durationSec, Action onComplete)
	{
		if (this.dissolveTween != null)
		{
			this.dissolveTween.Kill(false);
			this.dissolveTween = null;
		}
		this.dissolveTween = DOTween.To(() => this.currentDissolveVal, delegate(float x)
		{
			this.currentDissolveVal = x;
		}, newDissolveAmount, durationSec).OnUpdate(new TweenCallback(this.OnDissolveUpdated)).OnComplete(delegate
		{
			this.OnDissolveUpdated();
			this.dissolveTween = null;
			if (onComplete != null)
			{
				Action onComplete2 = onComplete;
				if (onComplete2 == null)
				{
					return;
				}
				onComplete2();
			}
		});
	}

	private void OnDissolveUpdated()
	{
		this.materialCopy.SetFloat("_DissolveAmount", this.currentDissolveVal);
	}

	[SerializeField]
	private AudioSource swimAudio;

	[SerializeField]
	private AudioSource panicAudio;

	[SerializeField]
	private AudioSource dissolveAudio;

	[SerializeField]
	private AudioSource attackAudio;

	[SerializeField]
	private List<AudioClip> attackClips;

	[SerializeField]
	private AssetReference attackSFX;

	[SerializeField]
	private float swimAudioMinVolume;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private Transform model;

	[SerializeField]
	private VariablePlayerDamager variablePlayerDamager;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private MonsterRayWorldEvent.MonsterRayType monsterRayType;

	[SerializeField]
	private GameObject hitVFX;

	[SerializeField]
	private SkinnedMeshRenderer meshRenderer;

	[SerializeField]
	private Material material;

	[SerializeField]
	private GameObject sanityModifier;

	[SerializeField]
	private VFXVolumeFader vFXVolumeFader;

	[SerializeField]
	private float attackRange;

	[SerializeField]
	private float timeUntilAttack;

	[SerializeField]
	private float spawnDurationSec;

	[SerializeField]
	private float despawnDurationSec;

	[SerializeField]
	private float despawnDelay;

	[SerializeField]
	private float maxFollowDurationSec;

	[SerializeField]
	private float attackCooldownSec;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float moveSpeedScalar;

	[SerializeField]
	private float boatSpeedMin;

	[SerializeField]
	private float boatSpeedMax;

	[SerializeField]
	private float rotationSpeed;

	private Vector3 prevPos;

	private float timeOfPlayerHit;

	private bool hasHitPlayer;

	private MonsterRayWorldEvent.MonsterRayState currentState;

	private Transform playerTransform;

	private float timeBetweenDestinationSetsSec = 0.35f;

	private float timeOfLastDestinationSet;

	private float timeWithinAttackRange;

	private float timeSpentFollowing;

	private Vector3 currentDestinationPos;

	private Material materialCopy;

	private Tweener dissolveTween;

	private float currentDissolveVal;

	private float timeOfLastAttack;

	private float lerpedSpeedValue;

	private enum MonsterRayState
	{
		NONE,
		SPAWNING,
		FOLLOWING,
		ATTACKING,
		DESPAWNING
	}

	private enum MonsterRayType
	{
		SHADOW,
		BITE_ATTACK,
		TAIL_ATTACK
	}
}
