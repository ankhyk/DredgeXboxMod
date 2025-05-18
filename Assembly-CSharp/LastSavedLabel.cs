using System;
using UnityEngine;
using UnityEngine.Localization.Components;

public class LastSavedLabel : MonoBehaviour
{
	public string _LastSavedTime
	{
		get
		{
			return TimeHelper.FormattedTimeString(GameManager.Instance.SaveData.lastSavedTime);
		}
	}

	private void Awake()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveStart = (Action)Delegate.Combine(saveManager.OnSaveStart, new Action(this.OnSaveStarted));
		SaveManager saveManager2 = GameManager.Instance.SaveManager;
		saveManager2.OnSaveComplete = (Action)Delegate.Combine(saveManager2.OnSaveComplete, new Action(this.OnSaveCompleted));
	}

	private void OnEnable()
	{
		this.UpdateText();
	}

	private void OnDestroy()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveStart = (Action)Delegate.Remove(saveManager.OnSaveStart, new Action(this.OnSaveStarted));
		SaveManager saveManager2 = GameManager.Instance.SaveManager;
		saveManager2.OnSaveComplete = (Action)Delegate.Remove(saveManager2.OnSaveComplete, new Action(this.OnSaveCompleted));
	}

	private void OnSaveStarted()
	{
	}

	private void OnSaveCompleted()
	{
		this.UpdateText();
	}

	private void UpdateText()
	{
		this.localizedTextField.StringReference.Arguments = new object[] { this._LastSavedTime };
		this.localizedTextField.RefreshString();
		this.localizedTextField.enabled = true;
	}

	[SerializeField]
	private LocalizeStringEvent localizedTextField;
}
