using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AI;

public class CrocodileWorldEvent : MonoBehaviour
{
	public void Init(RouteConfig routeConfig)
	{
		this.routeConfig = routeConfig;
		this.currentState = CrocodileWorldEvent.CrocodileState.SPAWNING;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		this.emergeAudio.Play();
		this.swimAudio.DOFade(1f, 1f).From(0f, true, false);
	}

	private void OnSpawnComplete()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnSpawnComplete));
		this.BeginIdling();
	}

	private void BeginIdling()
	{
		this.currentState = CrocodileWorldEvent.CrocodileState.IDLING;
		this.animator.SetTrigger("idle");
		this.idleTimeUntilSwim = this.idleDuration;
	}

	private void Update()
	{
		if (this.currentState == CrocodileWorldEvent.CrocodileState.IDLING)
		{
			this.idleTimeUntilSwim -= Time.deltaTime;
			if (this.idleTimeUntilSwim <= 0f)
			{
				this.BeginSwimming();
			}
		}
		float num = Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position);
		if (num < this.duckDistanceThreshold)
		{
			this.isDucked = true;
			float num2 = Mathf.InverseLerp(0f, this.duckDistanceThreshold, num);
			this.targetYPos = Mathf.Lerp(this.duckedY, this.upY, num2);
		}
		else
		{
			this.targetYPos = this.upY;
		}
		this.timeUntilNextCall -= Time.deltaTime;
		if (this.currentState == CrocodileWorldEvent.CrocodileState.SWIMMING && this.timeUntilNextCall <= 0f)
		{
			this.PlayCallSFX();
		}
		this.adjustedPos.x = this.rootObject.position.x;
		this.adjustedPos.y = this.targetYPos;
		this.adjustedPos.z = this.rootObject.position.z;
		this.rootObject.position = this.adjustedPos;
	}

	private void BeginSwimming()
	{
		this.currentState = CrocodileWorldEvent.CrocodileState.SWIMMING;
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
		this.animator.SetTrigger("swim");
		SimplePathFollow simplePathFollow = this.simplePathFollow;
		simplePathFollow.OnPathComplete = (Action)Delegate.Combine(simplePathFollow.OnPathComplete, new Action(this.OnPathComplete));
		this.simplePathFollow.Init(this.routeConfig);
	}

	private void OnPathComplete()
	{
		this.currentState = CrocodileWorldEvent.CrocodileState.DESPAWNING;
		this.submergeAudio.Play();
		this.swimAudio.DOFade(0f, 1f);
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnDespawnComplete));
		this.animator.SetTrigger("despawn");
	}

	public void PlayCallSFX()
	{
		this.callAudio.PlayOneShot(this.callClips.PickRandom<AudioClip>());
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
	}

	private void OnDespawnComplete()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private AudioSource emergeAudio;

	[SerializeField]
	private AudioSource submergeAudio;

	[SerializeField]
	private AudioSource swimAudio;

	[SerializeField]
	private AudioSource callAudio;

	[SerializeField]
	private List<AudioClip> callClips;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private SimplePathFollow simplePathFollow;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform rootObject;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private float idleDuration = 2.5f;

	[SerializeField]
	private float duckDistanceThreshold;

	[SerializeField]
	private float upY;

	[SerializeField]
	private float duckedY;

	[SerializeField]
	private float timeBetweenCallsMin;

	[SerializeField]
	private float timeBetweenCallsMax;

	private float targetYPos;

	private bool isDucked;

	private RouteConfig routeConfig;

	private CrocodileWorldEvent.CrocodileState currentState;

	private float idleTimeUntilSwim;

	private Vector3 adjustedPos;

	private float timeUntilNextCall;

	private enum CrocodileState
	{
		NONE,
		SPAWNING,
		IDLING,
		SWIMMING,
		DESPAWNING
	}
}
