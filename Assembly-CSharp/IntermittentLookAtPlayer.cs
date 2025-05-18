using System;
using UnityEngine;

public class IntermittentLookAtPlayer : MonoBehaviour
{
	private void Awake()
	{
		this.defaultRotation = base.transform.rotation;
		this.targetTransform = Camera.main.transform;
		this.Look();
	}

	private void Update()
	{
		this.timeUntilLook -= Time.deltaTime;
		if (this.timeUntilLook <= 0f)
		{
			this.Look();
			this.timeUntilLook = global::UnityEngine.Random.Range(this.timeUntilLookMin, this.timeUntilLookMax);
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, this.speed * Time.deltaTime);
	}

	private void Look()
	{
		if (this.targetTransform && Vector3.Distance(base.transform.position, this.targetTransform.position) < this.maxDistance)
		{
			this.proposedNewTargetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
		}
		else
		{
			this.proposedNewTargetRotation = this.defaultRotation;
		}
		if (Quaternion.Angle(this.proposedNewTargetRotation, this.defaultRotation) <= this.maxAngle)
		{
			this.targetRotation = this.proposedNewTargetRotation;
		}
		this.targetRotation *= Quaternion.Euler(new Vector3(this.eulerOffset.x + global::UnityEngine.Random.Range(-this.randomness.x, this.randomness.x), this.eulerOffset.y + global::UnityEngine.Random.Range(-this.randomness.y, this.randomness.y), this.eulerOffset.z + global::UnityEngine.Random.Range(-this.randomness.z, this.randomness.z)));
	}

	[SerializeField]
	private float timeUntilLookMin;

	[SerializeField]
	private float timeUntilLookMax;

	[SerializeField]
	private Vector3 eulerOffset;

	[SerializeField]
	private Vector3 randomness;

	[SerializeField]
	private float maxDistance;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float maxAngle;

	private Transform targetTransform;

	private float timeUntilLook;

	private Quaternion defaultRotation;

	private Quaternion targetRotation;

	private Quaternion proposedNewTargetRotation;
}
