using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ResetAllSettingsButton : MonoBehaviour
{
	private void OnEnable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnResetAllButtonPressed));
	}

	private void OnDisable()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnResetAllButtonPressed));
	}

	private void OnResetAllButtonPressed()
	{
		UnityEvent onPopupShown = this.OnPopupShown;
		if (onPopupShown != null)
		{
			onPopupShown.Invoke();
		}
		ApplicationEvents.Instance.TriggerSettingsConfirmationToggled(true);
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "popup.confirm-restore-default-settings";
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
			hideOnButtonPress = true
		};
		dialogOptions.buttonOptions = new DialogButtonOptions[] { dialogButtonOptions, dialogButtonOptions2 };
		GameManager.Instance.CanUnpause = false;
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnResetAllSettingsConfirmationResult));
	}

	private void OnResetAllSettingsConfirmationResult(DialogButtonOptions options)
	{
		GameManager.Instance.CanUnpause = true;
		if (options.id == 1)
		{
			GameManager.Instance.SaveManager.CreateMetaData();
			global::UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISettingsRefreshable>().ToList<ISettingsRefreshable>()
				.ForEach(delegate(ISettingsRefreshable s)
				{
					s.ForceRefresh();
				});
			GameManager.Instance.Input.ResetAllBindings();
		}
		UnityEvent onPopupDismissed = this.OnPopupDismissed;
		if (onPopupDismissed != null)
		{
			onPopupDismissed.Invoke();
		}
		ApplicationEvents.Instance.TriggerSettingsConfirmationToggled(false);
		if (GameManager.Instance.Input.IsUsingController)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}
	}

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	public UnityEvent OnPopupShown;

	public UnityEvent OnPopupDismissed;
}
