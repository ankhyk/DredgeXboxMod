using System;
using UnityEngine;

public class FogPropertyModifier : MonoBehaviour
{
	public FogProperty FogProperty
	{
		get
		{
			return this.fogProperty;
		}
	}

	public float GetProportionStrengthForPoint(Vector3 position)
	{
		float num = Vector3.Distance(position, base.transform.position);
		if (num > this.partialValueRadius)
		{
			return 0f;
		}
		if (num < this.fullValueRadius)
		{
			return 1f;
		}
		return 1f - Mathf.InverseLerp(this.fullValueRadius, this.partialValueRadius, num);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.fullValueRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.partialValueRadius);
	}

	[SerializeField]
	private FogProperty fogProperty;

	[SerializeField]
	private float partialValueRadius;

	[SerializeField]
	private float fullValueRadius;
}
