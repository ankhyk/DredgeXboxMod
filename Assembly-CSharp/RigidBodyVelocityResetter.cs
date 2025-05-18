using System;
using UnityEngine;

public class RigidBodyVelocityResetter : MonoBehaviour
{
	private void OnDisable()
	{
		this.rigidBody.velocity = Vector3.zero;
		this.rigidBody.transform.localEulerAngles = this.baseRotation;
		this.rigidBody.transform.localPosition = this.basePosition;
	}

	private void OnValidate()
	{
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		this.basePosition = this.rigidBody.transform.localPosition;
		this.baseRotation = this.rigidBody.transform.localEulerAngles;
	}

	[SerializeField]
	private Rigidbody rigidBody;

	[SerializeField]
	private Vector3 basePosition;

	[SerializeField]
	private Vector3 baseRotation;
}
