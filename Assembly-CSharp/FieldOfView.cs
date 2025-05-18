using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	public GameObject TrackedObject
	{
		get
		{
			return this.trackedObject;
		}
		set
		{
			this.trackedObject = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Time.time > this.timeOfLastSearch + this.timeBetweenSearchesSec)
		{
			this.FindTarget();
			this.timeOfLastSearch = Time.time;
		}
	}

	private void FindTarget()
	{
		this.trackedObject = this.FindTargetForMask(this.playerMask);
	}

	private GameObject FindTargetForMask(LayerMask mask)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.viewRadius, mask);
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i].transform;
			Vector3 normalized = (transform.position - base.transform.position).normalized;
			if (Vector3.Angle(base.transform.forward, normalized) < this.viewAngle / 2f)
			{
				float num = Vector3.Distance(base.transform.position, transform.position);
				if (!Physics.Raycast(base.transform.position, normalized, num, this.obstacleMask))
				{
					return transform.gameObject;
				}
			}
		}
		return null;
	}

	private void DrawFieldOfView()
	{
		int num = Mathf.RoundToInt(this.viewAngle * this.meshResolution);
		float num2 = this.viewAngle / (float)num;
		List<Vector3> list = new List<Vector3>();
		FieldOfView.ViewCastInfo viewCastInfo = default(FieldOfView.ViewCastInfo);
		for (int i = 0; i < num; i++)
		{
			float num3 = base.transform.eulerAngles.y - this.viewAngle / 2f + num2 * (float)i;
			FieldOfView.ViewCastInfo viewCastInfo2 = this.ViewCast(num3);
			if (i > 0)
			{
				bool flag = Mathf.Abs(viewCastInfo.distance - viewCastInfo2.distance) > this.edgeDistanceThreshold;
				if (viewCastInfo.hit != viewCastInfo2.hit || (viewCastInfo.hit && viewCastInfo2.hit && flag))
				{
					FieldOfView.EdgeInfo edgeInfo = this.FindEdge(viewCastInfo, viewCastInfo2);
					if (edgeInfo.pointA != Vector3.zero)
					{
						list.Add(edgeInfo.pointA);
					}
					if (edgeInfo.pointB != Vector3.zero)
					{
						list.Add(edgeInfo.pointB);
					}
				}
			}
			list.Add(viewCastInfo2.point);
			viewCastInfo = viewCastInfo2;
		}
		int num4 = list.Count + 1;
		Vector3[] array = new Vector3[num4];
		int[] array2 = new int[(num4 - 2) * 3];
		array[0] = Vector3.zero;
		for (int j = 0; j < num4 - 1; j++)
		{
			array[j + 1] = base.transform.InverseTransformPoint(list[j]);
			if (j < num4 - 2)
			{
				array2[j * 3] = 0;
				array2[j * 3 + 1] = j + 1;
				array2[j * 3 + 2] = j + 2;
			}
		}
	}

	private FieldOfView.EdgeInfo FindEdge(FieldOfView.ViewCastInfo minViewCast, FieldOfView.ViewCastInfo maxViewCast)
	{
		float num = minViewCast.angle;
		float num2 = maxViewCast.angle;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < this.edgeResolveIterations; i++)
		{
			float num3 = (num + num2) / 2f;
			FieldOfView.ViewCastInfo viewCastInfo = this.ViewCast(num3);
			bool flag = Mathf.Abs(minViewCast.distance - viewCastInfo.distance) > this.edgeDistanceThreshold;
			if (viewCastInfo.hit == minViewCast.hit && !flag)
			{
				num = num3;
				vector = viewCastInfo.point;
			}
			else
			{
				num2 = num3;
				vector2 = viewCastInfo.point;
			}
		}
		return new FieldOfView.EdgeInfo(vector, vector2);
	}

	private FieldOfView.ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 vector = this.DirFromAngle(globalAngle, true);
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, vector, out raycastHit, this.viewRadius, this.obstacleMask))
		{
			return new FieldOfView.ViewCastInfo(true, raycastHit.point, raycastHit.distance, globalAngle);
		}
		return new FieldOfView.ViewCastInfo(false, base.transform.position + vector * this.viewRadius, this.viewRadius, globalAngle);
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += base.transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * 0.017453292f), 0f, Mathf.Cos(angleInDegrees * 0.017453292f));
	}

	[SerializeField]
	private float timeBetweenSearchesSec = 0.25f;

	public float viewRadius;

	[Range(0f, 360f)]
	public float viewAngle;

	public LayerMask playerMask;

	public LayerMask obstacleMask;

	public float meshResolution;

	public int edgeResolveIterations;

	public float edgeDistanceThreshold;

	private GameObject trackedObject;

	private float timeOfLastSearch;

	public struct ViewCastInfo
	{
		public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
		{
			this.hit = hit;
			this.point = point;
			this.distance = distance;
			this.angle = angle;
		}

		public bool hit;

		public Vector3 point;

		public float distance;

		public float angle;
	}

	public struct EdgeInfo
	{
		public EdgeInfo(Vector3 pointA, Vector3 pointB)
		{
			this.pointA = pointA;
			this.pointB = pointB;
		}

		public Vector3 pointA;

		public Vector3 pointB;
	}
}
