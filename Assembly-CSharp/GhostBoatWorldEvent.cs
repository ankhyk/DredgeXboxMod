using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GhostBoatWorldEvent : WorldEvent
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
			return;
		}
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void Activate()
	{
		this.animator.Play("FogGhost_FadeIn");
		base.Activate();
		this.spawnTime = Time.time;
		this.PickDestination();
		this.foghornCoroutine = base.StartCoroutine(this.DelayedFoghornEnd(global::UnityEngine.Random.Range(this.foghornDurationMin, this.foghornDurationMax)));
	}

	private IEnumerator DelayedFoghornEnd(float duration)
	{
		yield return new WaitForSeconds(duration);
		this.FoghornEnd();
		yield break;
	}

	private void FoghornEnd()
	{
		this.foghornAudioSource.Stop();
		this.foghornAudioSource.PlayOneShot(this.foghornEndClip);
		this.foghornCoroutine = null;
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
		base.transform.rotation = Quaternion.LookRotation(this.destination);
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
		if (!this.finishRequested && (Time.time > this.spawnTime + base.worldEventData.durationSec || Vector3.Distance(base.transform.position, this.destination) < this.despawnDestinationProximity || Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < this.despawnPlayerProximity))
		{
			this.RequestEventFinish();
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.animator.Play("FogGhost_FadeOut");
			if (this.foghornCoroutine != null)
			{
				this.foghornCoroutine = null;
				this.FoghornEnd();
			}
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
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float maxTravelDistance;

	[SerializeField]
	private float despawnPlayerProximity;

	[SerializeField]
	private float despawnDestinationProximity;

	[SerializeField]
	private GameObject ghostObject;

	[SerializeField]
	private Renderer ghostRenderer;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioSource foghornAudioSource;

	[SerializeField]
	private AudioClip foghornEndClip;

	[SerializeField]
	private float finishDelaySec;

	[SerializeField]
	private float foghornDurationMin;

	[SerializeField]
	private float foghornDurationMax;

	[SerializeField]
	private LayerMask forbiddenSpawnLayers;

	private Vector3 destination;

	private float spawnTime;

	private Coroutine foghornCoroutine;
}
