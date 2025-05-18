using System;
using CommandTerminal;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PlayerDepthMonitor : MonoBehaviour
{
	private void Start()
	{
		this.AddTerminalCommands();
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	private void Update()
	{
		this.timeSinceDepthUpdated += Time.deltaTime;
		if (this.loggingEnabled && this.timeSinceDepthUpdated > 1f)
		{
			this.timeSinceDepthUpdated = 0f;
			(this.depthMonitor.CurrentDepth * 100f).ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		}
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("depth.monitor", new Action<CommandArg[]>(this.ToggleDepthMonitoring), 1, 1, "Toggles the logging of player depth [0/1]");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("depth.monitor");
	}

	private void ToggleDepthMonitoring(CommandArg[] args)
	{
		this.loggingEnabled = args[0].Float == 1f;
		this.depthMonitor.enabled = this.loggingEnabled;
		if (this.loggingEnabled)
		{
			this.depthMonitor.UpdateDepth();
		}
	}

	[SerializeField]
	private DepthMonitor depthMonitor;

	private float timeSinceDepthUpdated = float.PositiveInfinity;

	private bool loggingEnabled;
}
