using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherData", menuName = "Dredge/WeatherData", order = 0)]
public class WeatherData : ScriptableObject
{
	public WeatherParameters Parameters
	{
		get
		{
			return this.parameters;
		}
	}

	[SerializeField]
	private WeatherParameters parameters;

	public ZoneEnum permittedZones = ZoneEnum.ALL;

	public bool day = true;

	public bool night = true;
}
