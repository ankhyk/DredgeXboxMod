using System;
using Cinemachine;
using CommandTerminal;
using UnityEngine;

public class DemoEndLogic : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnDemoEndToggled += this.OnDemoEndToggled;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
		ApplicationEvents.Instance.OnDemoEndToggled -= this.OnDemoEndToggled;
	}

	private void OnDemoEndToggled(bool ended)
	{
		this.virtualCamera.enabled = ended;
		GameManager.Instance.CanPause = false;
		GameManager.Instance.CanUnpause = false;
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("demo.end", new Action<CommandArg[]>(this.DemoEnd), 0, 0, "Ends the demo.");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("demo.end");
		}
	}

	private void DemoEnd(CommandArg[] args)
	{
		ApplicationEvents.Instance.TriggerDemoEndToggled(true);
	}

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
}
