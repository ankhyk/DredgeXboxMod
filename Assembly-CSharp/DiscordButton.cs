using System;
using UnityEngine;

public class DiscordButton : MonoBehaviour
{
	public void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.discordButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnDiscordButtonClicked));
	}

	public void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.discordButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnDiscordButtonClicked));
	}

	private void OnDiscordButtonClicked()
	{
		Application.OpenURL("https://bit.ly/3ObpPdx");
	}

	[SerializeField]
	private BasicButtonWrapper discordButton;
}
