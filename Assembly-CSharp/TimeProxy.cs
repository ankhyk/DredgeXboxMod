using System;
using UnityEngine;

public abstract class TimeProxy : MonoBehaviour
{
	public abstract decimal GetTimeAndDay();

	public abstract void SetTimeAndDay(decimal time);
}
