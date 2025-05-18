using System;
using UnityEngine;

public class WaterPropertyModifier : MonoBehaviour
{
	public WaterProperty WaterProperty
	{
		get
		{
			return this.waterProperty;
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
	private WaterProperty waterProperty;

	[SerializeField]
	private float partialValueRadius;

	[SerializeField]
	private float fullValueRadius;
}
