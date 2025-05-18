using System;
using UnityEngine;
using UnityEngine.AI;

public class TargetFollow : MonoBehaviour
{
	public GameObject Target
	{
		get
		{
			return this.target;
		}
	}

	public void Init(GameObject target)
	{
		this.target = target;
		this.timeOfLastPathSet = float.NegativeInfinity;
	}

	private void Update()
	{
		if (!this.navMeshAgent.enabled)
		{
			return;
		}
		this.targetPositionHelper = this.target.transform.position;
		this.targetPositionHelper.y = base.transform.position.y;
		if (Vector3.Distance(this.targetPositionHelper, base.transform.position) < this.pathLockThreshold)
		{
			return;
		}
		if (Time.time > this.timeOfLastPathSet + this.timeBetweenPathRefreshesSec)
		{
			bool flag = false;
			int num = 4;
			NavMeshPath navMeshPath;
			do
			{
				this.targetPositionHelper = this.target.transform.position;
				this.targetPositionHelper.y = base.transform.position.y;
				Vector3 normalized = (this.targetPositionHelper - base.transform.position).normalized;
				this.targetPositionHelper += normalized * (float)num;
				navMeshPath = new NavMeshPath();
				this.navMeshAgent.CalculatePath(this.targetPositionHelper, navMeshPath);
				if (navMeshPath.corners.Length != 0)
				{
					flag = Vector3.Distance(this.targetPositionHelper, navMeshPath.corners[navMeshPath.corners.Length - 1]) < this.arriveThreshold;
				}
				num--;
			}
			while (!flag && num >= 0);
			if (flag)
			{
				this.navMeshAgent.SetPath(navMeshPath);
				this.timeOfLastPathSet = Time.time;
				return;
			}
			Action onPathError = this.OnPathError;
			if (onPathError == null)
			{
				return;
			}
			onPathError();
		}
	}

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	[SerializeField]
	private float arriveThreshold;

	[SerializeField]
	private float timeBetweenPathRefreshesSec = 0.25f;

	[SerializeField]
	private float overshootDistance;

	[SerializeField]
	private float pathLockThreshold;

	public Action OnPathError;

	private float timeOfLastPathSet;

	private GameObject target;

	private Vector3 targetPositionHelper;
}
