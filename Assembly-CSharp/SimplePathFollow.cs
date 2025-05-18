using System;
using UnityEngine;
using UnityEngine.AI;

public class SimplePathFollow : MonoBehaviour
{
	public void Init(RouteConfig routeConfig)
	{
		this.routeConfig = routeConfig;
		this.pathIndex = routeConfig.startIndex;
		this.navMeshAgent.SetDestination(this.routeConfig.route[this.pathIndex].position);
		this.isInit = true;
	}

	public void MoveToClosestPathPoint()
	{
		if (!this.navMeshAgent.enabled)
		{
			return;
		}
		int num = 0;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < this.routeConfig.route.Count; i++)
		{
			float num3 = Vector3.Distance(base.transform.position, this.routeConfig.route[i].position);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		this.pathIndex = num;
		this.navMeshAgent.SetDestination(this.routeConfig.route[num].position);
	}

	protected virtual void Update()
	{
		if (!this.isInit)
		{
			return;
		}
		if (!this.navMeshAgent.enabled)
		{
			return;
		}
		if (!this.loop && ((this.routeConfig.direction == RouteDirection.FORWARDS && this.pathIndex >= this.routeConfig.route.Count) || (this.routeConfig.direction == RouteDirection.BACKWARDS && this.pathIndex < 0)))
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.routeConfig.route[this.pathIndex].position) < this.waypointDistanceThreshold)
		{
			if (this.routeConfig.direction == RouteDirection.FORWARDS)
			{
				this.pathIndex++;
			}
			else
			{
				this.pathIndex--;
			}
			if ((this.routeConfig.direction == RouteDirection.FORWARDS && this.pathIndex >= this.routeConfig.route.Count) || (this.routeConfig.direction == RouteDirection.BACKWARDS && this.pathIndex < 0))
			{
				if (this.loop)
				{
					if (this.routeConfig.direction == RouteDirection.FORWARDS)
					{
						this.pathIndex = 0;
					}
					else
					{
						this.pathIndex = this.routeConfig.route.Count - 1;
					}
					this.navMeshAgent.SetDestination(this.routeConfig.route[this.pathIndex].position);
					return;
				}
				Action onPathComplete = this.OnPathComplete;
				if (onPathComplete == null)
				{
					return;
				}
				onPathComplete();
				return;
			}
			else
			{
				this.navMeshAgent.SetDestination(this.routeConfig.route[this.pathIndex].position);
			}
		}
	}

	private void OnDrawGizmos()
	{
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
	public NavMeshAgent navMeshAgent;

	[SerializeField]
	private float waypointDistanceThreshold;

	[SerializeField]
	private bool loop;

	public Action OnPathComplete;

	private RouteConfig routeConfig;

	private int pathIndex;

	private bool isInit;
}
