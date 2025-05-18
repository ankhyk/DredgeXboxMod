using System;
using CommandTerminal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSanityUI : MonoBehaviour
{
	private void Update()
	{
		if (GameManager.Instance.Player)
		{
			this.barFill.fillAmount = GameManager.Instance.Player.Sanity.CurrentSanity;
			this.rateText.text = GameManager.Instance.Player.Sanity.RateOfChange.ToString("n4");
		}
	}

	private void Start()
	{
		Terminal.Shell.AddCommand("sanity", new Action<CommandArg[]>(this.ToggleSanityUI), 1, 1, "Display sanity UI [0 = off | 1 = on]");
	}

	private void ToggleSanityUI(CommandArg[] args)
	{
		bool flag = args[0].Int == 1;
		this.container.SetActive(flag);
	}

	private void OnDestroy()
	{
		Terminal.Shell.RemoveCommand("sanity");
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private Image barFill;

	[SerializeField]
	private TextMeshProUGUI rateText;
}
