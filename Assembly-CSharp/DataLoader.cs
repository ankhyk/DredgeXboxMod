using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataLoader : MonoBehaviour
{
	public bool HasLoaded()
	{
		return this._hasLoadedWeatherData && this._hasLoadedQuestData && this._hasLoadedGridConfigData && this._hasLoadedWorldEventData && this._hasLoadedMapMarkers && this._hasLoadedQuestGrids;
	}

	private void Awake()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Combine(instance.OnGameEnded, new Action(this.OnGameEnded));
	}

	private void OnDisable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Remove(instance.OnGameEnded, new Action(this.OnGameEnded));
	}

	public void OnGameEnded()
	{
		this.allWeather.Clear();
		this.allQuests.Clear();
		this.allQuestSteps.Clear();
		this.allGridConfigs.Clear();
		this.allWorldEvents.Clear();
		this.allMapMarkers.Clear();
		this.allQuestGrids.Clear();
		this._hasLoadedWeatherData = false;
		this._hasLoadedQuestData = false;
		this._hasLoadedGridConfigData = false;
		this._hasLoadedWorldEventData = false;
		this._hasLoadedMapMarkers = false;
		this._hasLoadedQuestGrids = false;
	}

	public void Load()
	{
		Addressables.LoadAssetsAsync<QuestData>(this.questDataAssetLabelReference, null).Completed += this.OnQuestDataAddressablesLoaded;
		Addressables.LoadAssetsAsync<WeatherData>(this.weatherDataAssetLabelReference, null).Completed += this.OnWeatherDataAddressablesLoaded;
		Addressables.LoadAssetsAsync<GridConfiguration>(this.gridConfigDataAssetLabelReference, null).Completed += this.OnGridConfigDataAddressablesLoaded;
		Addressables.LoadAssetsAsync<WorldEventData>(this.worldEventDataAssetLabelReference, null).Completed += this.OnWorldEventDataAddressablesLoaded;
		Addressables.LoadAssetsAsync<MapMarkerData>(this.mapMarkerDataAssetLabelReference, null).Completed += this.OnMapMarkerDataAddressablesLoaded;
		Addressables.LoadAssetsAsync<QuestGridConfig>(this.questGridDataAssetLabelReference, null).Completed += this.OnQuestGridDataAddressablesLoaded;
	}

	private void OnQuestDataAddressablesLoaded(AsyncOperationHandle<IList<QuestData>> handle)
	{
		foreach (QuestData questData in handle.Result)
		{
			this.allQuests.Add(questData.name, questData);
			questData.steps.ForEach(delegate(QuestStepData qs)
			{
				this.allQuestSteps.Add(qs.name, qs);
			});
			if (questData.onOfferedQuestStep != null && !this.allQuestSteps.ContainsKey(questData.onOfferedQuestStep.name))
			{
				this.allQuestSteps.Add(questData.onOfferedQuestStep.name, questData.onOfferedQuestStep);
			}
		}
		this._hasLoadedQuestData = true;
	}

	private void OnWeatherDataAddressablesLoaded(AsyncOperationHandle<IList<WeatherData>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (WeatherData weatherData in handle.Result)
			{
				this.allWeather.Add(weatherData);
			}
			this._hasLoadedWeatherData = true;
		}
	}

	private void OnGridConfigDataAddressablesLoaded(AsyncOperationHandle<IList<GridConfiguration>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (GridConfiguration gridConfiguration in handle.Result)
			{
				this.allGridConfigs.Add(gridConfiguration.name, gridConfiguration);
			}
			this._hasLoadedGridConfigData = true;
		}
	}

	private void OnWorldEventDataAddressablesLoaded(AsyncOperationHandle<IList<WorldEventData>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (WorldEventData worldEventData in handle.Result)
			{
				this.allWorldEvents.Add(worldEventData);
			}
			this._hasLoadedWorldEventData = true;
		}
	}

	private void OnMapMarkerDataAddressablesLoaded(AsyncOperationHandle<IList<MapMarkerData>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (MapMarkerData mapMarkerData in handle.Result)
			{
				this.allMapMarkers.Add(mapMarkerData);
			}
			this._hasLoadedMapMarkers = true;
		}
	}

	private void OnQuestGridDataAddressablesLoaded(AsyncOperationHandle<IList<QuestGridConfig>> handle)
	{
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			foreach (QuestGridConfig questGridConfig in handle.Result)
			{
				this.allQuestGrids.Add(questGridConfig);
			}
			this._hasLoadedQuestGrids = true;
		}
	}

	[SerializeField]
	private AssetLabelReference questDataAssetLabelReference;

	[SerializeField]
	private AssetLabelReference weatherDataAssetLabelReference;

	[SerializeField]
	private AssetLabelReference gridConfigDataAssetLabelReference;

	[SerializeField]
	private AssetLabelReference worldEventDataAssetLabelReference;

	[SerializeField]
	private AssetLabelReference mapMarkerDataAssetLabelReference;

	[SerializeField]
	private AssetLabelReference questGridDataAssetLabelReference;

	public List<WeatherData> allWeather = new List<WeatherData>();

	public Dictionary<string, QuestData> allQuests = new Dictionary<string, QuestData>();

	public Dictionary<string, GridConfiguration> allGridConfigs = new Dictionary<string, GridConfiguration>();

	public Dictionary<string, QuestStepData> allQuestSteps = new Dictionary<string, QuestStepData>();

	public List<WorldEventData> allWorldEvents = new List<WorldEventData>();

	public List<MapMarkerData> allMapMarkers = new List<MapMarkerData>();

	public List<QuestGridConfig> allQuestGrids = new List<QuestGridConfig>();

	private bool _hasLoadedWeatherData;

	private bool _hasLoadedQuestData;

	private bool _hasLoadedGridConfigData;

	private bool _hasLoadedWorldEventData;

	private bool _hasLoadedMapMarkers;

	private bool _hasLoadedQuestGrids;
}
