using System;
using UnityEngine;

public class DummyTimeProxy : TimeProxy
{
	public override decimal GetTimeAndDay()
	{
		return (decimal)this.fakeTime;
	}

	public override void SetTimeAndDay(decimal time)
	{
	}

	[SerializeField]
	[Range(0f, 1f)]
	public float fakeTime;
}
