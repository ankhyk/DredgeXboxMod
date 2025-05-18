using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StingerAudio : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		this.timeUntilNextCheck = global::UnityEngine.Random.Range(this.delayBetweenStingersMinSec, this.delayBetweenStingersMaxSec);
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock == null)
		{
			this.canUpdate = true;
			this.timeUntilNextCheck = this.minTimeAfterUndockingToPlay;
			return;
		}
		this.canUpdate = false;
	}

	private void Update()
	{
		if (!this.canUpdate)
		{
			return;
		}
		if (GameManager.Instance.UI.IsInCutscene)
		{
			return;
		}
		if (!this.shouldQueueNextTrack)
		{
			this.timeUntilNextCheck -= Time.deltaTime;
			if (this.timeUntilNextCheck <= 0f)
			{
				bool flag = true;
				if (GameManager.Instance.AudioPlayer.IsPlayingStinger)
				{
					flag = false;
				}
				float time = GameManager.Instance.Time.Time;
				if (flag && (time < this.gameTimeMin || time > this.gameTimeMax))
				{
					flag = false;
				}
				if (flag && GameManager.Instance.WeatherController.CurrentWeatherData.Parameters.forbidStingers)
				{
					flag = false;
				}
				if (flag)
				{
					this.shouldQueueNextTrack = true;
					return;
				}
				this.timeUntilNextCheck = this.timeBetweenChecksSec;
			}
			return;
		}
		ZoneEnum currentZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
		if (!this.stingerAssetReferences.ContainsKey(currentZone))
		{
			return;
		}
		if (currentZone != this.lastZone)
		{
			this.lastIndex = -1;
		}
		List<AssetReference> list;
		if (global::UnityEngine.Random.value < this.weightingTowardsTIRStingers && this.tirStingerAssetReferences.ContainsKey(currentZone))
		{
			list = this.tirStingerAssetReferences[currentZone];
		}
		else
		{
			list = this.stingerAssetReferences[currentZone];
		}
		int num = global::UnityEngine.Random.Range(0, list.Count);
		if (num == this.lastIndex)
		{
			num = (int)Mathf.Repeat((float)(num + 1), (float)(list.Count - 1));
		}
		this.lastIndex = num;
		this.lastZone = currentZone;
		GameManager.Instance.AudioPlayer.PlayMusic(list[num], AudioLayer.MUSIC_STINGER);
		this.shouldQueueNextTrack = false;
		this.timeUntilNextCheck = this.assumedStingerDuration + global::UnityEngine.Random.Range(this.delayBetweenStingersMinSec, this.delayBetweenStingersMaxSec);
	}

	private void OnDestroy()
	{
		GameManager.Instance.AudioPlayer.RequestStingerStop();
	}

	[SerializeField]
	private Dictionary<ZoneEnum, List<AssetReference>> stingerAssetReferences = new Dictionary<ZoneEnum, List<AssetReference>>();

	[SerializeField]
	private Dictionary<ZoneEnum, List<AssetReference>> tirStingerAssetReferences = new Dictionary<ZoneEnum, List<AssetReference>>();

	[SerializeField]
	private float delayBetweenStingersMinSec;

	[SerializeField]
	private float delayBetweenStingersMaxSec;

	[SerializeField]
	private float assumedStingerDuration;

	[SerializeField]
	private float gameTimeMin;

	[SerializeField]
	private float gameTimeMax;

	[SerializeField]
	private float timeBetweenChecksSec;

	[SerializeField]
	private float minTimeAfterUndockingToPlay;

	[SerializeField]
	private float weightingTowardsTIRStingers;

	private bool shouldQueueNextTrack;

	private float timeUntilNextCheck;

	private ZoneEnum lastZone;

	private int lastIndex = -1;

	private bool canUpdate;
}
