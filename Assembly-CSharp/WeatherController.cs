using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityEngine;

[ExecuteInEditMode]
public class WeatherController : MonoBehaviour
{
	public float CloudDarkness
	{
		get
		{
			return this._cloudDarkness;
		}
		set
		{
			this._cloudDarkness = value;
		}
	}

	public float Cloudiness
	{
		get
		{
			return this._cloudiness;
		}
		set
		{
			this._cloudiness = value;
		}
	}

	public float TransitionDurationSec
	{
		get
		{
			return this.transitionDurationSec;
		}
		set
		{
			this.transitionDurationSec = value;
		}
	}

	public WeatherData CurrentWeatherData
	{
		get
		{
			return this.currentWeatherData;
		}
	}

	public WeatherData FallbackWeather
	{
		get
		{
			return this._fallbackWeather;
		}
		set
		{
			this._fallbackWeather = value;
		}
	}

	public Lightning LightningParticlesEmission
	{
		get
		{
			return this.lightningParticlesEmission;
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			GameManager.Instance.WeatherController = this;
			this.AddTerminalCommands();
			this.allWeather = GameManager.Instance.DataLoader.allWeather;
			WeatherData weatherData = this._fallbackWeather;
			if (GameManager.Instance != null && GameManager.Instance.SaveData != null)
			{
				string weather = GameManager.Instance.SaveData.Weather;
				WeatherData weatherDataByString = this.GetWeatherDataByString(weather);
				if (weatherDataByString != null)
				{
					weatherData = weatherDataByString;
				}
			}
			this.timeUntilNextZoneCheck = this.timeBetweenZoneChecks;
			if (this.rainParticles)
			{
				this.mainRainModule = this.rainParticles.main;
				this.emissionRainModule = this.rainParticles.emission;
				this.dropletSizeCurve = this.mainRainModule.startSizeY;
			}
			if (this.snowParticles)
			{
				this.mainSnowModule = this.snowParticles.main;
				this.emissionSnowModule = this.snowParticles.emission;
			}
			this._cloudiness = this._fallbackWeather.Parameters.cloudiness;
			this._cloudDarkness = this._fallbackWeather.Parameters.cloudDarkness;
			this._auroraAmount = this._fallbackWeather.Parameters.auroraAmount;
			this._waveSteepness = this._fallbackWeather.Parameters.waveSteepness;
			this._foamAmount = this._fallbackWeather.Parameters.foamAmount;
			this._lightningDelayMin = this._fallbackWeather.Parameters.lightningDelayMin;
			this._lightningDelayMax = this._fallbackWeather.Parameters.lightningDelayMax;
			this.currentWeather = this._fallbackWeather.Parameters;
			this.currentWeatherData = this._fallbackWeather;
			this.ChangeWeather(weatherData);
		}
	}

	private WeatherData GetWeatherDataByString(string name)
	{
		return this.allWeather.Find((WeatherData w) => w.name == name);
	}

	private void OnEnable()
	{
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnTeleportComplete += this.OnTeleportComplete;
		}
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
		if (GameEvents.Instance)
		{
			GameEvents.Instance.OnTeleportComplete -= this.OnTeleportComplete;
		}
	}

	private void OnTeleportComplete()
	{
		if (!this.CanCurrentWeatherExistInCurrentZone())
		{
			this.rainParticles.Clear();
			this._rainRate = 0f;
			this.snowParticles.Clear();
			this._snowRate = 0f;
			this._auroraAmount = 0f;
			Shader.SetGlobalFloat("_AuroraAmount", this._auroraAmount);
			this._prevSfxVolume = 0f;
			this.PickNewWeather(true);
		}
	}

	private void PickNewWeather(bool setImmediate = false)
	{
		ZoneEnum currentZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
		bool IsDaytime = GameManager.Instance.Time.IsDaytime;
		List<WeatherData> list = this.allWeather.Where((WeatherData w) => w.permittedZones.HasFlag(currentZone) && ((w.day & IsDaytime) || (w.night && !IsDaytime))).ToList<WeatherData>();
		string validWeathers = "Valid weathers: ";
		list.ForEach(delegate(WeatherData s)
		{
			validWeathers += string.Format("{0} [{1}], ", s.name, s.Parameters.weight);
		});
		this.weights = new float[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			this.weights[i] = list[i].Parameters.weight;
		}
		int randomWeightedIndex = MathUtil.GetRandomWeightedIndex(this.weights);
		WeatherData weatherData = list[randomWeightedIndex];
		this.ChangeWeather(weatherData);
		if (setImmediate)
		{
			this.weatherChangeTransitionStartTime = -99999f;
		}
	}

	private bool CanCurrentWeatherExistInCurrentZone()
	{
		return this.currentWeatherData.permittedZones.HasFlag(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone());
	}

	public void EmitLightning()
	{
		this.lightningParticlesEmission.Emit();
	}

	private void Update()
	{
		if (GameManager.Instance && GameManager.Instance.Player)
		{
			this.timeUntilNextZoneCheck -= Time.deltaTime;
			if (this.timeUntilNextZoneCheck <= 0f)
			{
				this.timeUntilNextZoneCheck = this.timeBetweenZoneChecks;
				if (!this.CanCurrentWeatherExistInCurrentZone())
				{
					this.PickNewWeather(false);
				}
			}
		}
		if (GameManager.Instance != null && GameManager.Instance.IsPlaying && GameManager.Instance.SaveData != null)
		{
			float weatherChangeTime = GameManager.Instance.SaveData.WeatherChangeTime;
			float num = this.currentWeather.durationHours * 0.041666668f;
			if (GameManager.Instance.Time.TimeAndDay > weatherChangeTime + num && !this.needsTransitioning)
			{
				this.PickNewWeather(false);
			}
		}
		if (this.needsTransitioning)
		{
			if (Time.time < this.weatherChangeTransitionStartTime + this.transitionDurationSec)
			{
				float num2 = (Time.time - this.weatherChangeTransitionStartTime) / this.transitionDurationSec;
				this._cloudiness = Mathf.Lerp(this.previousWeather.cloudiness, this.currentWeather.cloudiness, num2);
				this._cloudDarkness = Mathf.Lerp(this.previousWeather.cloudDarkness, this.currentWeather.cloudDarkness, num2);
				this._auroraAmount = Mathf.Lerp(this.previousWeather.auroraAmount, this.currentWeather.auroraAmount, num2);
				this._waveSteepness = Mathf.Lerp(this.previousWeather.waveSteepness, this.currentWeather.waveSteepness, num2);
				this._foamAmount = Mathf.Lerp(this.previousWeather.foamAmount, this.currentWeather.foamAmount, num2);
				this._lightningDelayMin = this.currentWeather.lightningDelayMin;
				this._lightningDelayMax = this.currentWeather.lightningDelayMax;
				this._splashChance = Mathf.Lerp(this.previousWeather.splashChance, this.currentWeather.splashChance, num2);
				if (this.previousWeather.dropletHeightMin > 0f)
				{
					this._dropletHeightMin = Mathf.Lerp(this.previousWeather.dropletHeightMin, this.currentWeather.dropletHeightMin, num2);
					this._dropletHeightMax = Mathf.Lerp(this.previousWeather.dropletHeightMax, this.currentWeather.dropletHeightMax, num2);
				}
				else
				{
					this._dropletHeightMin = this.currentWeather.dropletHeightMin;
					this._dropletHeightMax = this.currentWeather.dropletHeightMax;
				}
				if (this.currentWeather.hasRain && this.previousWeather.hasRain)
				{
					this._rainSpeed = Mathf.Lerp(this.previousWeather.rainSpeed, this.currentWeather.rainSpeed, this.currentWeather.rainEnterCurve.Evaluate(num2));
				}
				else if (this.currentWeather.hasRain)
				{
					this._rainSpeed = Mathf.Lerp(1f, this.currentWeather.rainSpeed, this.previousWeather.rainEnterCurve.Evaluate(num2));
				}
				else if (!this.currentWeather.hasRain)
				{
					this._rainSpeed = Mathf.Lerp(this._rainSpeed, 1f, num2);
				}
				if (this.currentWeather.hasRain)
				{
					this._rainRate = Mathf.Lerp(this.previousWeather.rainRate, this.currentWeather.rainRate, this.currentWeather.rainEnterCurve.Evaluate(num2));
				}
				else if (this.previousWeather.hasRain)
				{
					this._rainRate = Mathf.Lerp(this.previousWeather.rainRate, this.currentWeather.rainRate, this.previousWeather.rainExitCurve.Evaluate(num2));
				}
				else if (!this.currentWeather.hasRain)
				{
					this._rainRate = Mathf.Lerp(this._rainRate, 0f, num2);
				}
				if (this.currentWeather.hasSnow && this.previousWeather.hasSnow)
				{
					this._snowSpeed = Mathf.Lerp(this.previousWeather.snowSpeed, this.currentWeather.snowSpeed, this.currentWeather.snowEnterCurve.Evaluate(num2));
				}
				else if (this.currentWeather.hasSnow)
				{
					this._snowSpeed = Mathf.Lerp(1f, this.currentWeather.snowSpeed, this.previousWeather.snowEnterCurve.Evaluate(num2));
				}
				else if (!this.currentWeather.hasSnow)
				{
					this._snowSpeed = Mathf.Lerp(this._snowSpeed, 1f, num2);
				}
				if (this.currentWeather.hasSnow)
				{
					this._snowRate = Mathf.Lerp(this.previousWeather.snowRate, this.currentWeather.snowRate, this.currentWeather.snowEnterCurve.Evaluate(num2));
				}
				else if (this.previousWeather.hasSnow)
				{
					this._snowRate = Mathf.Lerp(this.previousWeather.snowRate, this.currentWeather.snowRate, this.previousWeather.snowExitCurve.Evaluate(num2));
				}
				else if (!this.currentWeather.hasSnow)
				{
					this._snowRate = Mathf.Lerp(this._snowRate, 0f, num2);
				}
				this._sfxVolume = Mathf.Lerp(0f, this.currentWeather.sfxVolume, this.currentWeather.sfxEnterCurve.Evaluate(num2));
				this._prevSfxVolume = Mathf.Lerp(this.previousWeather.sfxVolume, 0f, this.previousWeather.sfxExitCurve.Evaluate(num2));
			}
			else
			{
				this._sfxVolume = this.currentWeather.sfxVolume;
				this._cloudiness = this.currentWeather.cloudiness;
				this._cloudDarkness = this.currentWeather.cloudDarkness;
				this._auroraAmount = this.currentWeather.auroraAmount;
				this._waveSteepness = this.currentWeather.waveSteepness;
				this._foamAmount = this.currentWeather.foamAmount;
				this._lightningDelayMin = this.currentWeather.lightningDelayMin;
				this._lightningDelayMax = this.currentWeather.lightningDelayMax;
				this._rainRate = this.currentWeather.rainRate;
				this._rainSpeed = this.currentWeather.rainSpeed;
				this._splashChance = this.currentWeather.splashChance;
				this._dropletHeightMin = this.currentWeather.dropletHeightMin;
				this._dropletHeightMax = this.currentWeather.dropletHeightMax;
				this._snowRate = this.currentWeather.snowRate;
				this._snowSpeed = this.currentWeather.snowSpeed;
				this.needsTransitioning = false;
			}
			this._isDirty = true;
		}
		if (this._isDirty && Application.isPlaying)
		{
			if (this.rainParticles)
			{
				this.dropletSizeCurve.constantMin = this._dropletHeightMax;
				this.dropletSizeCurve.constantMax = this._dropletHeightMin;
				this.mainRainModule.startSizeY = this.dropletSizeCurve;
				this.mainRainModule.simulationSpeed = this._rainSpeed;
				this.emissionRainModule.rateOverTime = this._rainRate;
				this.rainParticles.subEmitters.SetSubEmitterEmitProbability(0, this._splashChance);
			}
			if (this.snowParticles)
			{
				this.mainSnowModule.simulationSpeed = this._snowSpeed;
				this.emissionSnowModule.rateOverTime = this._snowRate;
			}
			if (this.lightningParticles)
			{
				this.lightningParticlesEmission.SetLightningDelay(this._lightningDelayMin, this._lightningDelayMax);
			}
			if (this.currentWeatherAudioSource)
			{
				this.currentWeatherAudioSource.volume = this._sfxVolume;
			}
			if (this.prevWeatherAudioSource)
			{
				this.prevWeatherAudioSource.volume = this._prevSfxVolume;
			}
			Shader.SetGlobalFloat("_WindSpeed", this._wind);
			Shader.SetGlobalFloat("_Cloudiness", this._cloudiness);
			Shader.SetGlobalFloat("_CloudDarkness", this._cloudDarkness);
			Shader.SetGlobalFloat("_AuroraAmount", this._auroraAmount);
			Shader.SetGlobalFloat("_WaveSteepness", this._waveSteepness);
			GameManager.Instance.WaveController.Steepness = this._waveSteepness;
			Shader.SetGlobalFloat("_FoamAmount", this._foamAmount);
			this._isDirty = false;
		}
	}

	public void ChangeWeather(WeatherData weatherData)
	{
		this.weatherChangeTransitionStartTime = Time.time;
		this.needsTransitioning = true;
		if (this.currentWeather != null && this.currentWeather.sfx && this.prevWeatherAudioSource)
		{
			this.prevWeatherAudioSource.volume = this.currentWeatherAudioSource.volume;
			this.prevWeatherAudioSource.clip = this.currentWeather.sfx;
			this.prevWeatherAudioSource.time = this.currentWeatherAudioSource.time;
			this.prevWeatherAudioSource.Play();
		}
		if (weatherData.Parameters.sfx && this.currentWeatherAudioSource)
		{
			this.currentWeatherAudioSource.clip = weatherData.Parameters.sfx;
			this.currentWeatherAudioSource.volume = 0f;
			this.currentWeatherAudioSource.Play();
		}
		this.previousWeather = new WeatherParameters();
		if (this.currentWeather == null)
		{
			this.currentWeather = this._fallbackWeather.Parameters;
		}
		this.previousWeather.cloudiness = this._cloudiness;
		this.previousWeather.cloudDarkness = this._cloudDarkness;
		this.previousWeather.auroraAmount = this._auroraAmount;
		this.previousWeather.waveSteepness = this._waveSteepness;
		this.previousWeather.foamAmount = this._foamAmount;
		this.previousWeather.lightningDelayMin = this._lightningDelayMin;
		this.previousWeather.lightningDelayMax = this._lightningDelayMax;
		this.previousWeather.rainSpeed = this._rainSpeed;
		this.previousWeather.rainRate = this._rainRate;
		this.previousWeather.dropletHeightMin = this._dropletHeightMin;
		this.previousWeather.dropletHeightMax = this._dropletHeightMax;
		this.previousWeather.splashChance = this._splashChance;
		this.previousWeather.sfxVolume = this._sfxVolume;
		this.previousWeather.snowSpeed = this._snowSpeed;
		this.previousWeather.snowRate = this._snowRate;
		this.previousWeather.hasRain = this.currentWeather.hasRain;
		this.previousWeather.rainEnterCurve = this.currentWeather.rainEnterCurve;
		this.previousWeather.rainExitCurve = this.currentWeather.rainExitCurve;
		this.previousWeather.hasSnow = this.currentWeather.hasSnow;
		this.previousWeather.snowEnterCurve = this.currentWeather.snowEnterCurve;
		this.previousWeather.snowExitCurve = this.currentWeather.snowExitCurve;
		this.previousWeather.sfx = this.currentWeather.sfx;
		this.previousWeather.sfxEnterCurve = this.currentWeather.sfxEnterCurve;
		this.previousWeather.sfxExitCurve = this.currentWeather.sfxExitCurve;
		this.currentWeather = weatherData.Parameters;
		this.currentWeatherData = weatherData;
		if (GameManager.Instance != null && GameManager.Instance.IsPlaying && GameManager.Instance.SaveData != null)
		{
			GameManager.Instance.SaveData.Weather = weatherData.name;
			GameManager.Instance.SaveData.WeatherChangeTime = GameManager.Instance.Time.TimeAndDay;
		}
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("weather.list", new Action<CommandArg[]>(this.ListWeather), 0, 0, "Lists the weather ids");
			Terminal.Shell.AddCommand("weather.set", new Action<CommandArg[]>(this.SetWeather), 0, 1, "Options: clear, fine, cloudy, dark");
			Terminal.Shell.AddCommand("weather.change", new Action<CommandArg[]>(this.ChangeWeather), 0, 1, "Options: clear, fine, cloudy, dark");
			Terminal.Shell.AddCommand("sky.cloud", new Action<CommandArg[]>(this.SetCloud), 1, 1, "Sets cloudiness. 0 - 1 range");
			Terminal.Shell.AddCommand("sky.wind", new Action<CommandArg[]>(this.SetWind), 1, 1, "Sets wind speed. 0 - 3 range");
			Terminal.Shell.AddCommand("sky.dark", new Action<CommandArg[]>(this.SetCloudDarkness), 1, 1, "Sets cloud darkness. 0 - 1 range");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("weather.list");
			Terminal.Shell.RemoveCommand("weather.set");
			Terminal.Shell.RemoveCommand("weather.change");
			Terminal.Shell.RemoveCommand("sky.cloud");
			Terminal.Shell.RemoveCommand("sky.wind");
			Terminal.Shell.RemoveCommand("sky.dark");
		}
	}

	private void ListWeather(CommandArg[] args)
	{
		string weathers = "";
		this.allWeather.ForEach(delegate(WeatherData w)
		{
			weathers = weathers + w.name + ", ";
		});
	}

	private void SetWeather(CommandArg[] args)
	{
		this.rainParticles.Clear();
		this.snowParticles.Clear();
		this.ChangeWeather(args);
		this.weatherChangeTransitionStartTime = -99999f;
	}

	public void SetWeather(WeatherData weatherData)
	{
		this.rainParticles.Clear();
		this.snowParticles.Clear();
		this.ChangeWeather(weatherData);
		this.weatherChangeTransitionStartTime = -99999f;
	}

	private void ChangeWeather(CommandArg[] args)
	{
		string text = "";
		if (args.Length != 0)
		{
			text = args[0].String;
		}
		this.ChangeWeather(text);
	}

	public void ChangeWeather(string weatherName)
	{
		WeatherData weatherData = null;
		if (!string.IsNullOrEmpty(weatherName))
		{
			weatherData = this.DebugGetWeatherDataByString(weatherName);
		}
		if (weatherData == null)
		{
			this.PickNewWeather(false);
			return;
		}
		this.ChangeWeather(weatherData);
	}

	private WeatherData DebugGetWeatherDataByString(string key)
	{
		string text = char.ToUpper(key[0]).ToString() + key.Substring(1);
		return this.GetWeatherDataByString(text);
	}

	private void SetCloud(CommandArg[] args)
	{
		this._cloudiness = Mathf.Clamp01(args[0].Float);
	}

	private void SetWind(CommandArg[] args)
	{
		this._wind = Mathf.Clamp01(args[0].Float);
	}

	private void SetCloudDarkness(CommandArg[] args)
	{
		this._cloudDarkness = Mathf.Clamp01(args[0].Float);
	}

	[SerializeField]
	private WeatherData _fallbackWeather;

	[Range(0f, 3f)]
	[SerializeField]
	private float _wind;

	[Range(0f, 1f)]
	[SerializeField]
	private float _cloudiness;

	[Range(0f, 1f)]
	[SerializeField]
	private float _cloudDarkness;

	[Range(0f, 1f)]
	[SerializeField]
	private float _auroraAmount;

	[Range(0f, 1f)]
	[SerializeField]
	private float _waveSteepness;

	[Range(0f, 1f)]
	[SerializeField]
	private float _foamAmount;

	[SerializeField]
	private float _lightningDelayMin;

	[SerializeField]
	private float _lightningDelayMax;

	[Range(0f, 1.4f)]
	[SerializeField]
	private float _rainSpeed;

	[Range(0f, 2000f)]
	[SerializeField]
	private float _rainRate;

	[Range(0f, 1.4f)]
	[SerializeField]
	private float _snowSpeed;

	[Range(0f, 1000f)]
	[SerializeField]
	private float _snowRate;

	[Range(0f, 2f)]
	[SerializeField]
	private float _dropletHeightMin;

	[Range(0f, 2f)]
	[SerializeField]
	private float _dropletHeightMax;

	[Range(0f, 1f)]
	[SerializeField]
	private float _splashChance;

	[Range(0f, 1f)]
	[SerializeField]
	private float _sfxVolume;

	[Range(0f, 1f)]
	[SerializeField]
	private float _prevSfxVolume;

	[SerializeField]
	private float transitionDurationSec;

	[SerializeField]
	private ParticleSystem rainParticles;

	[SerializeField]
	private ParticleSystem snowParticles;

	[SerializeField]
	private ParticleSystem lightningParticles;

	[SerializeField]
	private AudioSource currentWeatherAudioSource;

	[SerializeField]
	private AudioSource prevWeatherAudioSource;

	[SerializeField]
	private Lightning lightningParticlesEmission;

	private ParticleSystem.MainModule mainRainModule;

	private ParticleSystem.EmissionModule emissionRainModule;

	private ParticleSystem.MainModule mainSnowModule;

	private ParticleSystem.EmissionModule emissionSnowModule;

	private ParticleSystem.MinMaxCurve dropletSizeCurve;

	private List<WeatherData> allWeather;

	private float[] weights;

	private WeatherParameters previousWeather;

	private WeatherParameters currentWeather;

	private WeatherData currentWeatherData;

	private float weatherChangeTransitionStartTime = -999999f;

	private bool needsTransitioning;

	private bool _isDirty;

	private float timeUntilNextZoneCheck;

	private float timeBetweenZoneChecks = 5f;
}
