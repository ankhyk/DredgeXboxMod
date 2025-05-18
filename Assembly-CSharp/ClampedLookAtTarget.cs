using System;
using UnityEngine;

public class ClampedLookAtTarget : MonoBehaviour
{
	public Transform Target { get; set; }

	public float Speed { get; set; }

	private void Start()
	{
		this.originalRotation = base.transform.rotation;
	}

	private void Update()
	{
		Quaternion rotation = base.transform.rotation;
		if (this.Target == null)
		{
			base.transform.rotation = this.originalRotation;
		}
		else
		{
			base.transform.LookAt(this.Target);
		}
		Quaternion rotation2 = base.transform.rotation;
		base.transform.rotation = rotation;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation2, this.Speed * Time.deltaTime);
	}

	private Quaternion originalRotation;
}
