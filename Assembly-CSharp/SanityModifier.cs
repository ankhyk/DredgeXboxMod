using System;
using UnityEngine;

public class SanityModifier : MonoBehaviour
{
	public float FullValue
	{
		get
		{
			if (!GameManager.Instance.Time.IsDaytime)
			{
				return this.fullValueNight;
			}
			return this.fullValueDay;
		}
	}

	public float PartialValueMin
	{
		get
		{
			if (!GameManager.Instance.Time.IsDaytime)
			{
				return this.partialValueMinNight;
			}
			return this.partialValueMinDay;
		}
	}

	public bool IgnoreTimescale
	{
		get
		{
			return this.ignoreTimescale;
		}
	}

	public void SetNightValue(float value)
	{
		this.fullValueNight = value;
	}

	public void SetDayValue(float value)
	{
		this.fullValueDay = value;
	}

	public float GetModifierValueForPoint(Vector3 position)
	{
		float num = Vector3.Distance(position, base.transform.position);
		if (num > this.partialValueRadius)
		{
			return 0f;
		}
		if (num < this.fullValueRadius)
		{
			return this.FullValue;
		}
		float num2 = 1f - Mathf.InverseLerp(this.fullValueRadius, this.partialValueRadius, num);
		return Mathf.Lerp(this.PartialValueMin, this.FullValue, num2);
	}

	private void OnDrawGizmos()
	{
		if (this.fullValueDay > 0f || this.fullValueNight > 0f)
		{
			Gizmos.color = Color.green;
		}
		else
		{
			Gizmos.color = Color.red;
		}
		Gizmos.DrawWireSphere(base.transform.position, this.fullValueRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.partialValueRadius);
	}

	[SerializeField]
	private float fullValueDay;

	[SerializeField]
	private float fullValueNight;

	[SerializeField]
	private float fullValueRadius;

	[SerializeField]
	private float partialValueMinDay;

	[SerializeField]
	private float partialValueMinNight;

	[SerializeField]
	private float partialValueRadius;

	[SerializeField]
	private bool ignoreTimescale;
}
