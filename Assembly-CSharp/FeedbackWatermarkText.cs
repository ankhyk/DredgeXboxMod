using System;
using CommandTerminal;
using TMPro;
using UnityEngine;

public class FeedbackWatermarkText : MonoBehaviour
{
	private void Start()
	{
		Terminal.Shell.AddCommand("fwmark", new Action<CommandArg[]>(this.ToggleWatermark), 1, 1, "Display feedback watermark [0 = off | 1 = on]");
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("fwmark");
	}

	private void ToggleWatermark(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		this.text.gameObject.SetActive(flag);
	}

	[SerializeField]
	private TextMeshProUGUI text;
}
