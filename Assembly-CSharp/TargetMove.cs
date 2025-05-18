using System;
using UnityEngine;
using UnityEngine.AI;

public class TargetMove : MonoBehaviour
{
	public void Init(GameObject target)
	{
		this.targetPosition = target.transform.position;
		this.navMeshAgent.SetDestination(this.targetPosition);
	}

	private void Update()
	{
		if (Vector3.Distance(base.transform.position, this.targetPosition) < this.waypointDistanceThreshold)
		{
			Action onPathComplete = this.OnPathComplete;
			if (onPathComplete == null)
			{
				return;
			}
			onPathComplete();
		}
	}

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float waypointDistanceThreshold;

	public Action OnPathComplete;

	private Vector3 targetPosition;
}
