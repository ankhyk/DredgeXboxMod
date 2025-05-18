using System;
using UnityEngine;

public class WeatherTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			ReliableOnTriggerExit.NotifyTriggerEnter(other, base.gameObject, new ReliableOnTriggerExit._OnTriggerExit(this.OnTriggerExit));
			if (GameManager.Instance.Time.TimeAndDay > this.timeOfLastTrigger + this.cooldownDays)
			{
				this.timeOfLastTrigger = GameManager.Instance.Time.TimeAndDay;
				if (global::UnityEngine.Random.value < this.chance)
				{
					GameManager.Instance.WeatherController.ChangeWeather(this.weather);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			ReliableOnTriggerExit.NotifyTriggerExit(other, base.gameObject);
		}
	}

	[SerializeField]
	private WeatherData weather;

	[SerializeField]
	private float cooldownDays;

	[Range(0f, 1f)]
	[SerializeField]
	private float chance = 1f;

	private float timeOfLastTrigger = float.NegativeInfinity;
}
