using System;
using CommandTerminal;
using UnityEngine;

[ExecuteInEditMode]
[DefaultExecutionOrder(-900)]
public class TimeController : MonoBehaviour
{
	public float TimeAndDay
	{
		get
		{
			return (float)this._timeAndDay;
		}
	}

	public float Time
	{
		get
		{
			return (float)this._time;
		}
	}

	public bool IsDaytime
	{
		get
		{
			return this._isDaytime;
		}
	}

	public int Day
	{
		get
		{
			return this._day;
		}
	}

	public bool IsPlayerPassingTimeManually { get; set; }

	public float SceneLightness { get; private set; }

	public TimePassageMode CurrentTimePassageMode
	{
		get
		{
			return this.currentTimePassageMode;
		}
	}

	public float DuskTime
	{
		get
		{
			return this.duskTime;
		}
	}

	public float DawnTime
	{
		get
		{
			return this.dawnTime;
		}
	}

	private void Awake()
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.Time = this;
		}
		this._timeAndDay = this.timeProxy.GetTimeAndDay();
		Shader.SetGlobalFloat("_TimeOfDay", this.TimeAndDay);
		this.AddTerminalCommands();
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	public float GetDurationOfDayInSeconds()
	{
		return GameManager.Instance.GameConfigData.HourDurationInSeconds * 24f;
	}

	public bool IsTimePassing()
	{
		return this.IsTimePassingViaMovement() || this.IsTimePassingViaFishing() || this.IsPlayerPassingTimeManually || this.IsTimePassingForcefully();
	}

	public bool IsTimePassingViaFishing()
	{
		return GameManager.Instance.Player && (GameManager.Instance.Player && GameManager.Instance.Player.IsAlive) && GameManager.Instance.Player.IsFishing;
	}

	public bool IsTimePassingViaMovement()
	{
		return GameManager.Instance.Player && (GameManager.Instance.Player && GameManager.Instance.Player.IsAlive) && GameManager.Instance.Player.Controller.IsMoving;
	}

	public void ForcefullyPassTime(float hours, string reasonKey, TimePassageMode mode)
	{
		GameEvents.Instance.TriggerTimeForcefullyPassingChanged(true, reasonKey, mode);
		this.timeRemainingToForcefullyPass = (decimal)hours * 0.0416666666666666666666666667m;
		this.totalTimeToForcefullyPass = this.timeRemainingToForcefullyPass;
		this.currentTimePassageMode = mode;
	}

	public void StopForcefullyPassingTime()
	{
		this.currentTimePassageMode = TimePassageMode.NONE;
		this.timeRemainingToForcefullyPass = 0m;
		GameEvents.Instance.TriggerTimeForcefullyPassingChanged(false, "", this.currentTimePassageMode);
	}

	public float GetProportionForcefullyPassedTimeRemaining()
	{
		return (float)(this.timeRemainingToForcefullyPass / this.totalTimeToForcefullyPass);
	}

	public bool IsTimePassingForcefully()
	{
		return this.timeRemainingToForcefullyPass > 0m;
	}

	public float GetTimePassageModifier()
	{
		if (this.IsTimePassingForcefully())
		{
			return GameManager.Instance.GameConfigData.ForcedTimePassageSpeedModifier;
		}
		if (this.IsTimePassingViaFishing())
		{
			return GameManager.Instance.GameConfigData.FishingTimePassageSpeedModifier;
		}
		if (this.IsPlayerPassingTimeManually)
		{
			return 1f;
		}
		if (this.IsTimePassingViaMovement())
		{
			return GameManager.Instance.Player.Controller.InputMagnitude;
		}
		return 0f;
	}

	public decimal GetTimeChangeThisFrame()
	{
		return (decimal)(1f / this.GetDurationOfDayInSeconds() * global::UnityEngine.Time.deltaTime * this.GetTimePassageModifier());
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			this.wasDaytimeHelperVar = this.IsDaytime;
			decimal num = this.GetTimeChangeThisFrame();
			if (this.IsTimePassingForcefully())
			{
				this.timeRemainingToForcefullyPass -= num;
				if (this.timeRemainingToForcefullyPass <= 0m)
				{
					this.currentTimePassageMode = TimePassageMode.NONE;
					GameEvents.Instance.TriggerTimeForcefullyPassingChanged(false, "", this.currentTimePassageMode);
				}
				if (this._freezeTime)
				{
					num = 0m;
				}
				if (num > 0m)
				{
					this.timeProxy.SetTimeAndDay(this._timeAndDay + num);
				}
			}
			else
			{
				if (this._freezeTime)
				{
					num = 0m;
				}
				if (num > 0m)
				{
					this.timeProxy.SetTimeAndDay(this._timeAndDay + num / 2m);
				}
			}
		}
		this._timeAndDay = this.timeProxy.GetTimeAndDay();
		this._time = this._timeAndDay % 1m;
		this._day = (int)Math.Floor(this._timeAndDay);
		this._isDaytime = this.Time > this.dawnTime && this.Time < this.duskTime;
		if (this.wasDaytimeHelperVar != this.IsDaytime && GameEvents.Instance)
		{
			GameEvents.Instance.TriggerDayNightChanged();
		}
		if (this._lastDay < this.Day)
		{
			GameEvents.Instance.TriggerDayChanged(this.Day);
			this._lastDay = this.Day;
		}
		this.SceneLightness = this.RecalculateSceneLightness();
		Shader.SetGlobalFloat("_SceneLightness", this.SceneLightness);
		Shader.SetGlobalFloat("_TimeOfDay", this.Time);
		float num2 = this.lightAngleMin + 360f * this.Time;
		this.directionalLight.transform.eulerAngles = new Vector3(num2, -90f, 0f);
		this.directionalLight.color = this.sunColour.Evaluate(this.Time);
		RenderSettings.ambientLight = this.ambientLightColor.Evaluate(this.Time);
		if (this.playerProxy)
		{
			Vector3 playerPosition = this.playerProxy.GetPlayerPosition();
			Shader.SetGlobalVector("_FogCenter", new Vector4(playerPosition.x, playerPosition.y, playerPosition.z, 0f));
			return;
		}
		Camera current = Camera.current;
		if (current != null)
		{
			Shader.SetGlobalVector("_FogCenter", new Vector4(current.transform.position.x, current.transform.position.y, current.transform.position.z, 0f));
		}
	}

	private float RecalculateSceneLightness()
	{
		float num = 0f;
		if (GameManager.Instance && GameManager.Instance.WeatherController)
		{
			num = GameManager.Instance.WeatherController.CloudDarkness + GameManager.Instance.WeatherController.Cloudiness;
		}
		if (num > this.cloudLightEnableThreshold)
		{
			return 1f;
		}
		if (!this.IsDaytime)
		{
			return 1f;
		}
		return 0f;
	}

	public string GetTimeFormatted(float timeToFormat)
	{
		float num = timeToFormat * 24f;
		float num2 = (float)Mathf.FloorToInt(num);
		int num3 = Mathf.FloorToInt((num - num2) * 60f);
		string text = "";
		string text2 = "";
		string text3 = num3.ToString("00.##");
		if (GameManager.Instance.SettingsSaveData.clockStyle == 0)
		{
			if (num2 < 12f)
			{
				text = " AM";
			}
			if (num2 >= 12f)
			{
				num2 %= 12f;
				text = " PM";
			}
			if (num2 == 0f)
			{
				num2 = 12f;
			}
			text2 = num2.ToString();
		}
		else if (GameManager.Instance.SettingsSaveData.clockStyle == 1)
		{
			text2 = num2.ToString("00.##");
		}
		return string.Concat(new string[] { "<mspace=13>", text2, "</mspace>:<mspace=13>", text3, "</mspace>", text });
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("time.set", new Action<CommandArg[]>(this.SetTime), 1, 1, "Sets time e.g. 'time.set 1.5' would be halfway through the second day");
			Terminal.Shell.AddCommand("time.stop", new Action<CommandArg[]>(this.StopTime), 0, 0, "Stops the passage of time");
			Terminal.Shell.AddCommand("time.start", new Action<CommandArg[]>(this.StartTime), 0, 0, "Resumes the passage of time");
			Terminal.Shell.AddCommand("time.pass", new Action<CommandArg[]>(this.PassTime), 1, 3, "Forcefully passes a number of hours e.g. 'time.pass 6' passes 6 hours");
			Terminal.Shell.AddCommand("time.wrap", new Action<CommandArg[]>(this.TimeWrap), 0, -1, "Sets the time used by shaders and waves to 5 seconds before the wrap");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("time.set");
			Terminal.Shell.RemoveCommand("time.stop");
			Terminal.Shell.RemoveCommand("time.start");
			Terminal.Shell.RemoveCommand("time.pass");
			Terminal.Shell.RemoveCommand("time.wrap");
		}
	}

	private void SetTime(CommandArg[] args)
	{
		this.timeProxy.SetTimeAndDay((decimal)args[0].Float);
	}

	private void PassTime(CommandArg[] args)
	{
		float @float = args[0].Float;
		if (args.Length > 1)
		{
			float float2 = args[1].Float;
			GameManager.Instance.GameConfigData.ForcedTimePassageSpeedModifier = float2;
		}
		if (args.Length > 2)
		{
			float float3 = args[2].Float;
			GameManager.Instance.GameConfigData.HourDurationInSeconds = float3;
		}
		this.ForcefullyPassTime(@float, "feedback.pass-time-generic", TimePassageMode.OTHER);
	}

	private void StopTime(CommandArg[] args)
	{
		this.ToggleFreezeTime(true);
	}

	private void StartTime(CommandArg[] args)
	{
		this.ToggleFreezeTime(false);
	}

	public void ToggleFreezeTime(bool freeze)
	{
		this._freezeTime = freeze;
	}

	private void TimeWrap(CommandArg[] args)
	{
		GameManager.Instance.gameTime = 995f;
	}

	[SerializeField]
	private TimeProxy timeProxy;

	[SerializeField]
	private PlayerProxy playerProxy;

	[SerializeField]
	private Light directionalLight;

	[SerializeField]
	private Gradient sunColour;

	[SerializeField]
	private float lightAngleMin;

	[SerializeField]
	private float cloudLightEnableThreshold;

	[SerializeField]
	private Gradient ambientLightColor;

	[SerializeField]
	private AnimationCurve sceneLights;

	[SerializeField]
	private float dawnTime;

	[SerializeField]
	private float duskTime;

	private bool _freezeTime;

	private decimal _timeAndDay;

	private decimal _time;

	private int _day;

	private bool _isDaytime;

	private int _lastDay;

	private decimal timeRemainingToForcefullyPass;

	private TimePassageMode currentTimePassageMode;

	private bool wasDaytimeHelperVar;

	private decimal totalTimeToForcefullyPass;
}
