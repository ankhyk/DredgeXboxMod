using System;
using UnityEngine;

public class ConstantlyRotateOnY : MonoBehaviour
{
	private void Update()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.y += Time.deltaTime * (this.counterClockwise ? (-this.rotateSpeed) : this.rotateSpeed);
		base.transform.localEulerAngles = localEulerAngles;
	}

	[SerializeField]
	private float rotateSpeed = 5f;

	[SerializeField]
	public bool counterClockwise;
}
