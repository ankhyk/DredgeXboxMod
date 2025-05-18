using System;
using UnityEngine;

public class GameModeResponder : SettingResponder
{
	protected override void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.GAME_MODE)
		{
			this.Refresh();
		}
	}

	protected override void Refresh()
	{
		if (GameManager.Instance.SettingsSaveData != null)
		{
			GameMode gameMode = GameMode.NORMAL;
			if (GameManager.Instance.SettingsSaveData.gameMode == 1)
			{
				gameMode = GameMode.PASSIVE;
			}
			else if (GameManager.Instance.SettingsSaveData.gameMode == 2)
			{
				gameMode = GameMode.NIGHTMARE;
			}
			this.linkedResponder.OnGameModeChanged(gameMode);
		}
	}

	[SerializeField]
	private IGameModeResponder linkedResponder;
}
