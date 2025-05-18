using System;
using CommandTerminal;
using TMPro;
using UnityEngine;

public class WatermarkText : MonoBehaviour
{
	private void Start()
	{
		Terminal.Shell.AddCommand("wmark", new Action<CommandArg[]>(this.ToggleWatermark), 1, 1, "Display watermark [0 = off | 1 = on]");
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("wmark");
	}

	private void ToggleWatermark(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		this.text.gameObject.SetActive(flag);
	}

	[SerializeField]
	private TextMeshProUGUI text;
}
