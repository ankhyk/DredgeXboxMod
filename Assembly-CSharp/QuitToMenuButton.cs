using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class QuitToMenuButton : MonoBehaviour
{
	private void OnEnable()
	{
		this.willSave = GameManager.Instance.Player.IsDocked && !GameManager.Instance.SaveData.ForbidSave;
		this.basicButtonWrapper.LocalizedString.StringReference = (this.willSave ? this.saveAndQuitString : this.quitString);
	}

	public void OnClick()
	{
		UnityEvent onPopupShown = this.OnPopupShown;
		if (onPopupShown != null)
		{
			onPopupShown.Invoke();
		}
		ApplicationEvents.Instance.TriggerSettingsConfirmationToggled(true);
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = (this.willSave ? "popup.confirm-save-and-quit" : "popup.confirm-quit");
		object[] array = new string[] { TimeHelper.FormattedTimeString(GameManager.Instance.SaveData.lastSavedTime) };
		dialogOptions.textArguments = array;
		dialogOptions.disableGameCanvas = false;
		DialogButtonOptions dialogButtonOptions = default(DialogButtonOptions);
		dialogButtonOptions.buttonString = "prompt.cancel";
		dialogButtonOptions.id = 0;
		dialogButtonOptions.hideOnButtonPress = true;
		dialogButtonOptions.isBackOption = true;
		DialogButtonOptions dialogButtonOptions2 = new DialogButtonOptions
		{
			buttonString = "prompt.confirm",
			id = 1,
			hideOnButtonPress = false
		};
		dialogOptions.buttonOptions = new DialogButtonOptions[] { dialogButtonOptions, dialogButtonOptions2 };
		GameManager.Instance.CanUnpause = false;
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnConfirmationResult));
	}

	private void OnConfirmationResult(DialogButtonOptions options)
	{
		GameManager.Instance.CanUnpause = true;
		if (options.id != 1)
		{
			UnityEvent onPopupDismissed = this.OnPopupDismissed;
			if (onPopupDismissed != null)
			{
				onPopupDismissed.Invoke();
			}
			ApplicationEvents.Instance.TriggerSettingsConfirmationToggled(false);
			EventSystem.current.SetSelectedGameObject(base.gameObject);
			return;
		}
		if (this.willSave)
		{
			SaveManager saveManager = GameManager.Instance.SaveManager;
			saveManager.OnSaveComplete = (Action)Delegate.Combine(saveManager.OnSaveComplete, new Action(this.OnSaveComplete));
			GameManager.Instance.Save();
			return;
		}
		GameManager.Instance.Loader.LoadTitleFromGame();
	}

	private void OnSaveComplete()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveComplete = (Action)Delegate.Remove(saveManager.OnSaveComplete, new Action(this.OnSaveComplete));
		GameManager.Instance.Loader.LoadTitleFromGame();
	}

	public LocalizedString quitString;

	public LocalizedString saveAndQuitString;

	public BasicButtonWrapper basicButtonWrapper;

	public UnityEvent OnPopupShown;

	public UnityEvent OnPopupDismissed;

	private bool willSave;
}
