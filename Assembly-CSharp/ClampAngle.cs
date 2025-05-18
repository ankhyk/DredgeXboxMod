using System;
using UnityEngine;

public class ClampAngle : MonoBehaviour
{
	private void FixedUpdate()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		if (eulerAngles.x > 180f && eulerAngles.x < 360f - this.range)
		{
			eulerAngles.x = 360f - this.range;
		}
		if (eulerAngles.x < 180f && eulerAngles.x > this.range)
		{
			eulerAngles.x = this.range;
		}
		if (eulerAngles.z > 180f && eulerAngles.z < 360f - this.range)
		{
			eulerAngles.z = 360f - this.range;
		}
		if (eulerAngles.z < 180f && eulerAngles.z > this.range)
		{
			eulerAngles.z = this.range;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
	}

	[SerializeField]
	private float range;
}
