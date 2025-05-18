using System;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
	private void Start()
	{
		this.currentPos = base.transform.position;
	}

	private void LateUpdate()
	{
		this.UpdateTransform();
	}

	private void OnEnable()
	{
		this.currentPos = base.transform.position;
	}

	private void UpdateTransform()
	{
		Vector3 vector = this.RotatePointAroundPivot(this.parent.position + this.transformOffset, this.parent.position, this.parent.eulerAngles);
		this.currentPos = Vector3.Lerp(this.currentPos, vector, Time.deltaTime * this.speed);
		Vector3 vector2 = (this.currentPos - this.parent.position).normalized * this.originalDistance;
		base.transform.position = this.parent.position + vector2;
		base.transform.LookAt(this.parent, Vector3.up);
		base.transform.eulerAngles += this.rotationOffset;
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}

	private void SetOffsets()
	{
		this.originalDistance = (base.transform.position - this.parent.position).magnitude;
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.transformOffset = base.transform.position - this.parent.position;
		this.rotationOffset = base.transform.localEulerAngles;
		base.transform.LookAt(this.parent, Vector3.up);
		this.rotationOffset -= base.transform.localEulerAngles;
		base.transform.eulerAngles = eulerAngles;
	}

	private void OnValidate()
	{
		this.SetOffsets();
	}

	[SerializeField]
	private Transform parent;

	[SerializeField]
	private float speed = 8f;

	[SerializeField]
	private Vector3 transformOffset;

	[SerializeField]
	private Vector3 rotationOffset;

	private Vector3 currentPos;

	[SerializeField]
	private float originalDistance;
}
