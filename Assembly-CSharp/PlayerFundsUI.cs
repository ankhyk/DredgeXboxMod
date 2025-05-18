using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PlayerFundsUI : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
		this.OnPlayerFundsChanged(GameManager.Instance.SaveData.Funds, 0m);
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
	}

	private void OnPlayerFundsChanged(decimal newTotal, decimal changeAmount)
	{
		this._textField.text = "$" + newTotal.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
	}

	[SerializeField]
	private TextMeshProUGUI _textField;
}
