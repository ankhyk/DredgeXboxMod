using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
	public IConsoleManagementStrategy CurrentConsole { get; private set; }

	public bool HasConsoleManager
	{
		get
		{
			return this.CurrentConsole != null;
		}
	}

	public bool Initialized { get; private set; }

	public async Task Init()
	{
		try
		{
			global::UnityEngine.Debug.Log("Started setup of console manager");
			Stopwatch timer = new Stopwatch();
			timer.Start();
			this.CurrentConsole = new GDKPCConsoleStrategy();
			global::UnityEngine.Debug.Log("Started console CurrentConsole InitConsole");
			await this.CurrentConsole.InitConsole();
			global::UnityEngine.Debug.Log(string.Format("Console manager init completed in {0}ms", timer.ElapsedMilliseconds));
			global::UnityEngine.Debug.Log("Started console login");
			await this.CurrentConsole.LoginUser();
			global::UnityEngine.Debug.Log("Completed console login");
			this.Initialized = true;
			timer = null;
		}
		catch (Exception ex)
		{
			global::UnityEngine.Debug.LogError("Failed to setup console manager");
			global::UnityEngine.Debug.LogError(ex.Message ?? "");
		}
	}

	public void Update()
	{
		if (this.CurrentConsole == null)
		{
			return;
		}
		this.CurrentConsole.UpdateConsole();
	}

	public static Action<string> OnActiveUserSignedOut;
}
