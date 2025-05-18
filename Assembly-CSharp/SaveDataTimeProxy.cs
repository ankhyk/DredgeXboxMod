using System;
using UnityEngine;

public class SaveDataTimeProxy : TimeProxy
{
	public override decimal GetTimeAndDay()
	{
		if (GameManager.Instance != null && GameManager.Instance.SaveData != null)
		{
			return GameManager.Instance.SaveData.Time;
		}
		return this._backupTimeAndDay;
	}

	public override void SetTimeAndDay(decimal time)
	{
		if (GameManager.Instance != null && GameManager.Instance.SaveData != null)
		{
			GameManager.Instance.SaveData.Time = time;
		}
	}

	[Range(0f, 1f)]
	[SerializeField]
	[Tooltip("This value is used in editor (or when a save can't be loaded)")]
	private decimal _backupTimeAndDay = 0.5m;
}
