using System;
using CommandTerminal;
using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
	public float CurrentSanity
	{
		get
		{
			return this._sanity;
		}
	}

	public float RateOfChange { get; private set; }

	public float AbilitySanityValue { get; set; }

	public Player PlayerRef { get; set; }

	private void Awake()
	{
		this._sanity = GameManager.Instance.SaveData.Sanity;
	}

	private void Update()
	{
		if (GameManager.Instance.Time.IsTimePassing() || this.PlayerRef.SanityModifierDetector.IgnoreTimescale)
		{
			float num = 1f;
			if (!this.PlayerRef.SanityModifierDetector.IgnoreTimescale)
			{
				num = GameManager.Instance.Time.GetTimePassageModifier();
			}
			if (GameManager.Instance.Time.CurrentTimePassageMode == TimePassageMode.SLEEP)
			{
				this.RateOfChange = GameManager.Instance.GameConfigData.SleepingSanityModifier;
			}
			else
			{
				float num2 = this.PlayerRef.SanityModifierDetector.GetTotalModifierValue();
				num2 += this.AbilitySanityValue;
				float num3 = (GameManager.Instance.Time.IsDaytime ? GameManager.Instance.GameConfigData.DaySanityModifier : GameManager.Instance.GameConfigData.NightSanityModifier);
				this.RateOfChange = num3 + num2;
			}
			this.RateOfChange *= GameManager.Instance.GameConfigData.GlobalSanityModifier;
			if (this.RateOfChange < 0f)
			{
				this.RateOfChange *= 1f - GameManager.Instance.PlayerStats.ResearchedSanityModifier;
			}
			this._sanity += this.RateOfChange * Time.deltaTime * num;
			this._sanity = Mathf.Clamp01(this._sanity);
			GameManager.Instance.SaveData.Sanity = this._sanity;
		}
	}

	private void Start()
	{
		this.AddTerminalCommands();
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	public void ChangeSanity(float changeAmount)
	{
		this._sanity = Mathf.Clamp01(this.CurrentSanity + changeAmount);
		GameManager.Instance.SaveData.Sanity = this._sanity;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("player.sanity", new Action<CommandArg[]>(this.SetSanity), 0, 1, "Sets player's sanity value 0-1");
	}

	private void SetSanity(CommandArg[] args)
	{
		if (args.Length == 1)
		{
			this._sanity = Mathf.Clamp01(args[0].Float);
			GameManager.Instance.SaveData.Sanity = this._sanity;
		}
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("player.sanity");
	}

	private float _sanity;
}
