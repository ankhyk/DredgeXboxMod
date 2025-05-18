using System;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	private void Update()
	{
		if (this.target)
		{
			Quaternion quaternion = Quaternion.LookRotation(this.target.position - base.transform.position);
			quaternion *= Quaternion.Euler(this.eulerOffset);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.speed * Time.deltaTime);
		}
	}

	[SerializeField]
	private float speed;

	[SerializeField]
	private Vector3 eulerOffset;

	[SerializeField]
	public Transform target;
}
