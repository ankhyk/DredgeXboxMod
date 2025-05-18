using System;
using System.Linq;
using CommandTerminal;
using UnityEngine;

public class CinematicHelper : MonoBehaviour
{
	private void Start()
	{
		Terminal.Shell.AddCommand("cam", new Action<CommandArg[]>(this.SetCam), 1, 3, "'cam 1 2 3' plays camera '1' with a speed of 2 and a 3 second delay.");
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("cam");
	}

	private void SetCam(CommandArg[] args)
	{
		CinematicCamera cinematicCamera = global::UnityEngine.Object.FindObjectsOfType<CinematicCamera>().ToList<CinematicCamera>().Find((CinematicCamera c) => c.id == args[0].String);
		if (cinematicCamera == null)
		{
			CustomDebug.EditorLogError("No cam found with id " + args[0].String + ". Ignoring.");
			return;
		}
		if (this.prevCam != null)
		{
			this.prevCam.VirtualCamera.enabled = false;
		}
		this.prevCam = cinematicCamera;
		if (args.Length > 2)
		{
			cinematicCamera.Play(args[1].Float, args[2].Float);
			return;
		}
		if (args.Length > 1)
		{
			cinematicCamera.Play(args[1].Float, 0f);
			return;
		}
		cinematicCamera.Play(0f, 0f);
	}

	private CinematicCamera prevCam;
}
