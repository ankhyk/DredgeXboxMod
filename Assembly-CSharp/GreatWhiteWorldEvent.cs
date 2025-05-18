using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AI;

public class GreatWhiteWorldEvent : WorldEvent
{
	private void Awake()
	{
		this.model.localPosition = new Vector3(0f, this.downY, 0f);
		this.prevPos = this.model.position;
	}

	public override void Activate()
	{
		base.Activate();
		this.navMeshAgent.speed = this.idleSpeed;
		Transform transform = GameManager.Instance.Player.transform;
		for (int i = 0; i < base.worldEventData.depthTestPath.Count - 1; i++)
		{
			Vector3 vector = base.worldEventData.depthTestPath[i];
			Vector3 vector2 = transform.position + vector.x * transform.right + vector.z * transform.forward;
			vector2.y = 0f;
			this.path.Add(vector2);
		}
		this.currentState = GreatWhiteWorldEvent.GreatWhiteState.SWIMMING;
		this.model.DOLocalMoveY(0f, 2f, false).SetEase(Ease.OutSine);
		this.navMeshAgent.SetDestination(this.path[0]);
		this.audioSource.DOFade(1f, 1f).From(0f, true, false);
	}

	protected virtual void Update()
	{
		if (this.currentState == GreatWhiteWorldEvent.GreatWhiteState.SWIMMING)
		{
			if (Vector3.Distance(base.transform.position, this.path[this.pathIndex]) < this.waypointDistanceThreshold)
			{
				this.pathIndex++;
				if (this.pathIndex < this.path.Count)
				{
					this.navMeshAgent.SetDestination(this.path[this.pathIndex]);
				}
				else
				{
					this.currentState = GreatWhiteWorldEvent.GreatWhiteState.SEEKING_PLAYER;
					this.navMeshAgent.speed = this.pursueSpeed;
				}
			}
		}
		else if (this.currentState == GreatWhiteWorldEvent.GreatWhiteState.SEEKING_PLAYER && Time.time > this.timeOfLastPathUpdate + this.timeBetweenPathUpdates)
		{
			this.navMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
			this.timeOfLastPathUpdate = Time.time;
		}
		if (this.currentState == GreatWhiteWorldEvent.GreatWhiteState.SEEKING_PLAYER)
		{
			this.timeSpentSeeking += Time.deltaTime;
			if (this.timeSpentSeeking > this.maxSeekTimeSec)
			{
				this.ForceDespawn();
			}
		}
		float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
		if ((this.currentState == GreatWhiteWorldEvent.GreatWhiteState.SWIMMING || this.currentState == GreatWhiteWorldEvent.GreatWhiteState.SEEKING_PLAYER) && num < this.playerDistanceThreshold)
		{
			float num2 = 1f - Mathf.InverseLerp(0f, this.playerDistanceThreshold, num);
			float num3 = this.playerProximityDepth.Evaluate(num2) * this.downY;
			this.model.localPosition = new Vector3(0f, Mathf.Lerp(this.model.localPosition.y, num3, Time.deltaTime), 0f);
			this.audioSource.volume = 1f - num2;
		}
		if (num < this.despawnDistanceThreshold)
		{
			this.RequestEventFinish();
		}
		this.model.rotation = Quaternion.Slerp(this.model.rotation, Quaternion.LookRotation(this.model.position - this.prevPos), Time.deltaTime * this.rotationSpeed);
		this.prevPos = this.model.position;
	}

	private void ForceDespawn()
	{
		this.currentState = GreatWhiteWorldEvent.GreatWhiteState.DESPAWNING;
		float num = 3f;
		this.audioSource.DOFade(0f, num);
		this.model.DOLocalMoveY(this.downY, num, false).SetEase(Ease.InSine).OnComplete(delegate
		{
			this.RequestEventFinish();
		});
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.EventFinished();
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[SerializeField]
	private float waypointDistanceThreshold;

	[SerializeField]
	private float playerDistanceThreshold;

	[SerializeField]
	private float despawnDistanceThreshold;

	[SerializeField]
	private float downY;

	[SerializeField]
	private float idleSpeed;

	[SerializeField]
	private float pursueSpeed;

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private Transform model;

	[SerializeField]
	private float maxSeekTimeSec;

	[SerializeField]
	private AnimationCurve playerProximityDepth;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private AudioSource audioSource;

	private GreatWhiteWorldEvent.GreatWhiteState currentState;

	private List<Vector3> path = new List<Vector3>();

	private int pathIndex;

	private float timeBetweenPathUpdates = 0.5f;

	private float timeOfLastPathUpdate;

	private float timeSpentSeeking;

	private Vector3 prevPos;

	private enum GreatWhiteState
	{
		SWIMMING,
		SEEKING_PLAYER,
		DESPAWNING
	}
}
