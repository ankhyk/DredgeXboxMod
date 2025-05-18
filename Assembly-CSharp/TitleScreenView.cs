using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class TitleScreenView : MonoBehaviour
{
	private void Awake()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
	}

	private void OnEnable()
	{
		this.RefreshLayout();
		GameManager.Instance.EntitlementManager.OnDLCRuntimeInstall.AddListener(new UnityAction<Entitlement>(this.OnDLCRuntimeInstall));
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		this.RefreshLayout();
	}

	private void RefreshLayout()
	{
		int num = GameManager.Instance.SettingsSaveData.titleTheme;
		bool hasEntitlement = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1);
		bool hasEntitlement2 = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2);
		bool hasEntitlement3 = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DELUXE);
		if (GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme)
		{
			if (num == 3 && (!hasEntitlement || !hasEntitlement2 || !hasEntitlement3))
			{
				GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme = false;
			}
			if (num == 2 && !hasEntitlement2)
			{
				GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme = false;
			}
			if (num == 1 && !hasEntitlement)
			{
				GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme = false;
			}
		}
		if (!GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme)
		{
			if (hasEntitlement && hasEntitlement2 && hasEntitlement3)
			{
				num = 3;
			}
			else if (hasEntitlement2)
			{
				num = 2;
			}
			else if (hasEntitlement)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
		}
		if (this.currentTitleTheme == num)
		{
			return;
		}
		this.currentTitleTheme = num;
		switch (num)
		{
		case 0:
			this.SetNoDLC();
			return;
		case 1:
			this.SetDLC1();
			return;
		case 2:
			this.SetDLC2();
			return;
		case 3:
			this.SetAllDLC();
			return;
		default:
			return;
		}
	}

	private void SetNoDLC()
	{
		this.titleAudio.Stop();
		this.dlc1MapData.SetActive(false);
		this.dlc2MapData.SetActive(false);
		this.alldlcMapData.SetActive(false);
		this.dlc1ExpansionText.SetActive(false);
		this.dlc2ExpansionText.SetActive(false);
		this.alldlcExpansionText.SetActive(false);
		this.ocean.position = new Vector3(this.baseGameCamera.position.x, 0f, this.baseGameCamera.position.z);
		this.virtualCamera.position = this.baseGameCamera.position;
		CinemachineComposer cinemachineComponent = this.virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
		cinemachineComponent.m_ScreenX = this.baseGameCameraOffset.x;
		cinemachineComponent.m_ScreenY = this.baseGameCameraOffset.y;
		this.lookAt.position = this.baseGameLookAt.position;
		this.playerProxy.position = this.baseGamePlayerProxy.position;
		this.dummyTimeProxy.fakeTime = this.baseGameTime;
		this.weatherController.FallbackWeather = this.baseGameWeatherData;
		this.weatherController.SetWeather(this.baseGameWeatherData);
		this.fogController.defaultFogDensityOverDay = this.baseGameFogPropertyModifier.FogProperty.fogDensityOverDay;
		this.fogController.defaultFogHeight = this.baseGameFogPropertyModifier.FogProperty.fogHeight;
		this.fogController.defaultFogColorOverDay = this.baseGameFogPropertyModifier.FogProperty.fogColorOverDay;
		this.titleAudio.clip = this.baseGameTitleTheme;
		this.titleAudio.Play();
	}

	private void SetDLC1()
	{
		this.titleAudio.Stop();
		this.dlc1MapData.SetActive(true);
		this.dlc2MapData.SetActive(false);
		this.alldlcMapData.SetActive(false);
		this.dlc1ExpansionText.SetActive(true);
		this.dlc2ExpansionText.SetActive(false);
		this.alldlcExpansionText.SetActive(false);
		this.ocean.position = new Vector3(this.dlc1Camera.position.x, 0f, this.dlc1Camera.position.z);
		this.virtualCamera.position = this.dlc1Camera.position;
		CinemachineComposer cinemachineComponent = this.virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
		cinemachineComponent.m_ScreenX = this.dlc1CameraOffset.x;
		cinemachineComponent.m_ScreenY = this.dlc1CameraOffset.y;
		this.lookAt.position = this.dlc1LookAt.position;
		this.playerProxy.position = this.dlc1PlayerProxy.position;
		this.weatherController.FallbackWeather = this.dlc1WeatherData;
		this.weatherController.SetWeather(this.dlc1WeatherData);
		this.dummyTimeProxy.fakeTime = this.dlc1Time;
		this.fogController.defaultFogDensityOverDay = this.dlc1FogPropertyModifier.FogProperty.fogDensityOverDay;
		this.fogController.defaultFogHeight = this.dlc1FogPropertyModifier.FogProperty.fogHeight;
		this.fogController.defaultFogColorOverDay = this.dlc1FogPropertyModifier.FogProperty.fogColorOverDay;
		this.titleAudio.clip = this.dlc1TitleTheme;
		this.titleAudio.Play();
	}

	private void SetDLC2()
	{
		this.titleAudio.Stop();
		this.dlc1MapData.SetActive(false);
		this.dlc2MapData.SetActive(true);
		this.alldlcMapData.SetActive(false);
		this.dlc1ExpansionText.SetActive(false);
		this.dlc2ExpansionText.SetActive(true);
		this.alldlcExpansionText.SetActive(false);
		this.ocean.position = new Vector3(this.dlc2Camera.position.x, 0f, this.dlc2Camera.position.z);
		this.virtualCamera.position = this.dlc2Camera.position;
		CinemachineComposer cinemachineComponent = this.virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
		cinemachineComponent.m_ScreenX = this.dlc2CameraOffset.x;
		cinemachineComponent.m_ScreenY = this.dlc2CameraOffset.y;
		this.lookAt.position = this.dlc2LookAt.position;
		this.playerProxy.position = this.dlc2PlayerProxy.position;
		this.weatherController.FallbackWeather = this.dlc2WeatherData;
		this.weatherController.SetWeather(this.dlc2WeatherData);
		this.dummyTimeProxy.fakeTime = this.dlc2Time;
		this.fogController.defaultFogDensityOverDay = this.dlc2FogPropertyModifier.FogProperty.fogDensityOverDay;
		this.fogController.defaultFogHeight = this.dlc2FogPropertyModifier.FogProperty.fogHeight;
		this.fogController.defaultFogColorOverDay = this.dlc2FogPropertyModifier.FogProperty.fogColorOverDay;
		this.titleAudio.clip = this.dlc2TitleTheme;
		this.titleAudio.Play();
	}

	private void SetAllDLC()
	{
		this.titleAudio.Stop();
		this.dlc1MapData.SetActive(false);
		this.dlc2MapData.SetActive(false);
		this.alldlcMapData.SetActive(true);
		this.dlc1ExpansionText.SetActive(false);
		this.dlc2ExpansionText.SetActive(false);
		this.alldlcExpansionText.SetActive(true);
		this.ocean.position = new Vector3(this.alldlcCamera.position.x, 0f, this.alldlcCamera.position.z);
		this.virtualCamera.position = this.alldlcCamera.position;
		CinemachineComposer cinemachineComponent = this.virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineComposer>();
		cinemachineComponent.m_ScreenX = this.alldlcCameraOffset.x;
		cinemachineComponent.m_ScreenY = this.alldlcCameraOffset.y;
		this.lookAt.position = this.alldlcLookAt.position;
		this.playerProxy.position = this.alldlcPlayerProxy.position;
		this.weatherController.FallbackWeather = this.alldlcWeatherData;
		this.weatherController.SetWeather(this.alldlcWeatherData);
		this.dummyTimeProxy.fakeTime = this.alldlcTime;
		this.fogController.defaultFogDensityOverDay = this.alldlcFogPropertyModifier.FogProperty.fogDensityOverDay;
		this.fogController.defaultFogHeight = this.alldlcFogPropertyModifier.FogProperty.fogHeight;
		this.fogController.defaultFogColorOverDay = this.alldlcFogPropertyModifier.FogProperty.fogColorOverDay;
		this.titleAudio.clip = this.alldlcTitleTheme;
		this.titleAudio.Play();
	}

	private void OnDLCRuntimeInstall(Entitlement entitlement)
	{
		this.RefreshLayout();
	}

	[Header("Core Objects")]
	[SerializeField]
	private Transform virtualCamera;

	[SerializeField]
	private Transform lookAt;

	[SerializeField]
	private Transform playerProxy;

	[SerializeField]
	private Transform ocean;

	[SerializeField]
	private DummyTimeProxy dummyTimeProxy;

	[SerializeField]
	private WeatherController weatherController;

	[SerializeField]
	private FogController fogController;

	[SerializeField]
	private AudioSource titleAudio;

	[Header("NoDLC")]
	[SerializeField]
	private Transform baseGameCamera;

	[SerializeField]
	private Transform baseGameLookAt;

	[SerializeField]
	private Transform baseGamePlayerProxy;

	[SerializeField]
	private WeatherData baseGameWeatherData;

	[SerializeField]
	private float baseGameTime;

	[SerializeField]
	private FogPropertyModifier baseGameFogPropertyModifier;

	[SerializeField]
	private AudioClip baseGameTitleTheme;

	[SerializeField]
	private Vector2 baseGameCameraOffset;

	[Header("DLC1")]
	[SerializeField]
	private GameObject dlc1ExpansionText;

	[SerializeField]
	private Transform dlc1Camera;

	[SerializeField]
	private Transform dlc1LookAt;

	[SerializeField]
	private Transform dlc1PlayerProxy;

	[SerializeField]
	private WeatherData dlc1WeatherData;

	[SerializeField]
	private float dlc1Time;

	[SerializeField]
	private FogPropertyModifier dlc1FogPropertyModifier;

	[SerializeField]
	private AudioClip dlc1TitleTheme;

	[SerializeField]
	private GameObject dlc1MapData;

	[SerializeField]
	private Vector2 dlc1CameraOffset;

	[Header("DLC2")]
	[SerializeField]
	private GameObject dlc2ExpansionText;

	[SerializeField]
	private Transform dlc2Camera;

	[SerializeField]
	private Transform dlc2LookAt;

	[SerializeField]
	private Transform dlc2PlayerProxy;

	[SerializeField]
	private WeatherData dlc2WeatherData;

	[SerializeField]
	private float dlc2Time;

	[SerializeField]
	private FogPropertyModifier dlc2FogPropertyModifier;

	[SerializeField]
	private AudioClip dlc2TitleTheme;

	[SerializeField]
	private GameObject dlc2MapData;

	[SerializeField]
	private Vector2 dlc2CameraOffset;

	[Header("AllDLC")]
	[SerializeField]
	private GameObject alldlcExpansionText;

	[SerializeField]
	private Transform alldlcCamera;

	[SerializeField]
	private Transform alldlcLookAt;

	[SerializeField]
	private Transform alldlcPlayerProxy;

	[SerializeField]
	private WeatherData alldlcWeatherData;

	[SerializeField]
	private float alldlcTime;

	[SerializeField]
	private FogPropertyModifier alldlcFogPropertyModifier;

	[SerializeField]
	private AudioClip alldlcTitleTheme;

	[SerializeField]
	private GameObject alldlcMapData;

	[SerializeField]
	private Vector2 alldlcCameraOffset;

	private int currentTitleTheme = -1;
}
