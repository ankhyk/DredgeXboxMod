using System;
using UnityEngine;

public class AberrationEnabler : MonoBehaviour
{
	private void Awake()
	{
		if (!GameManager.Instance.SaveData.CanCatchAberrations)
		{
			GameEvents.Instance.OnDayChanged += this.OnDayChanged;
		}
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnDayChanged -= this.OnDayChanged;
	}

	private void OnDayChanged(int newDay)
	{
		if (newDay >= GameManager.Instance.GameConfigData.AberrationStartDay - 1)
		{
			GameManager.Instance.SaveData.CanCatchAberrations = true;
			GameEvents.Instance.OnDayChanged -= this.OnDayChanged;
			base.gameObject.SetActive(false);
		}
	}
}
