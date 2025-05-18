using System;
using UnityEngine;

public class Cullable : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
		Gizmos.DrawSphere(base.transform.position + this.sphereOffset, this.sphereRadius);
	}

	[SerializeField]
	public float sphereRadius;

	[SerializeField]
	public Vector3 sphereOffset;

	[SerializeField]
	public GameObject replacerObject;

	[SerializeField]
	public CullingGroupType cullingGroupType;
}
