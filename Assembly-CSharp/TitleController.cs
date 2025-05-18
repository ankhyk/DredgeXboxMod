using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.SYSTEM);
		ApplicationEvents.Instance.OnToggleSettings += this.OnToggleSettings;
		ApplicationEvents.Instance.OnCreditsToggled += this.OnCreditsToggled;
		PopupWindow popupWindow = this.saveSlotWindow;
		popupWindow.OnShowComplete = (Action)Delegate.Combine(popupWindow.OnShowComplete, new Action(this.OnSaveSlotWindowShowComplete));
		PopupWindow popupWindow2 = this.saveSlotWindow;
		popupWindow2.OnHideComplete = (Action)Delegate.Combine(popupWindow2.OnHideComplete, new Action(this.OnSaveSlotWindowHideComplete));
		this.CheckDidDeleteCorruptData();
		this.SetupUserSwitch().Run();
	}

	private void CheckDidDeleteCorruptData()
	{
		if (GameManager.Instance.SaveManager.DidDeleteCorruptData)
		{
			this.continueOrNewButton.SuppressFocusGain = true;
			GameManager.Instance.SaveManager.DidDeleteCorruptData = false;
			this.ShowCorruptDataDialog();
		}
	}

	private async Task SetupUserSwitch()
	{
		await Awaiters.NextFrame;
		await Awaiters.MainThread;
		this.GameCoreName.text = GameManager.Instance.ConsoleManager.CurrentConsole.GetUserName();
		this.GameCoreName.gameObject.SetActive(true);
		this.SwitchUserGameCorePrompt.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnToggleSettings -= this.OnToggleSettings;
		ApplicationEvents.Instance.OnCreditsToggled -= this.OnCreditsToggled;
		PopupWindow popupWindow = this.saveSlotWindow;
		popupWindow.OnShowComplete = (Action)Delegate.Remove(popupWindow.OnShowComplete, new Action(this.OnSaveSlotWindowShowComplete));
		PopupWindow popupWindow2 = this.saveSlotWindow;
		popupWindow2.OnHideComplete = (Action)Delegate.Remove(popupWindow2.OnHideComplete, new Action(this.OnSaveSlotWindowHideComplete));
	}

	private void OnSaveSlotWindowShowComplete()
	{
		EventSystem.current.SetSelectedGameObject(null);
		this.controllerFocusGrabber.enabled = false;
		this.buttonGroupCanvas.Disable();
	}

	private void OnSaveSlotWindowHideComplete()
	{
		this.buttonGroupCanvas.Enable();
		this.controllerFocusGrabber.enabled = true;
		this.controllerFocusGrabber.selectThis = this.loadButton;
		this.controllerFocusGrabber.SelectSelectable();
	}

	private void OnCreditsToggled(bool enabled)
	{
		if (enabled)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		else
		{
			this.controllerFocusGrabber.selectThis = this.creditsButton;
		}
		this.menuCanvas.enabled = !enabled;
		this.buttonGroup.SetActive(!enabled);
		this.controllerFocusGrabber.enabled = !enabled;
	}

	private void OnToggleSettings(bool show)
	{
		if (show)
		{
			this.DisableMenuCanvas();
			return;
		}
		this.controllerFocusGrabber.selectThis = this.settingsButton;
		this.EnableMenuCanvas();
	}

	private void DisableMenuCanvas()
	{
		this.menuCanvasGroup.interactable = false;
		this.menuCanvasGroup.blocksRaycasts = false;
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void EnableMenuCanvas()
	{
		this.menuCanvasGroup.interactable = true;
		this.menuCanvasGroup.blocksRaycasts = true;
	}

	private void SwitchUserGameCore()
	{
		TaskUtil.Run(new Func<Task>(this.SwitchUserGameCoreAsync));
	}

	private async Task SwitchUserGameCoreAsync()
	{
		await GameManager.Instance.ConsoleManager.CurrentConsole.SwitchUser();
	}

	public void ShowCorruptDataDialog()
	{
		this.DisableMenuCanvas();
		this.controllerFocusGrabber.enabled = false;
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = "popup.corrupt-saves-deleted";
		dialogOptions.buttonOptions = new DialogButtonOptions[]
		{
			new DialogButtonOptions
			{
				buttonString = "prompt.confirm",
				id = 1,
				hideOnButtonPress = true
			}
		};
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, new Action<DialogButtonOptions>(this.OnCorruptDataDialogConfirmationResult));
	}

	private void OnCorruptDataDialogConfirmationResult(DialogButtonOptions options)
	{
		this.controllerFocusGrabber.enabled = true;
		this.controllerFocusGrabber.selectThis = this.startButton;
		this.EnableMenuCanvas();
		this.controllerFocusGrabber.SelectSelectable();
	}

	public IEnumerator CheckIsSaveAllowedToBeLoaded(SaveData s, Selectable selectable, Action<bool> callback)
	{
		bool result = true;
		if (s == null || s == null)
		{
			result = false;
			yield return this.ShowLoadFailedWithIssueDialog(selectable, "popup.corrupt-save-identified");
		}
		if (result && ((s.GetSaveUsesEntitlement(Entitlement.DLC_1) && !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1)) || (s.GetSaveUsesEntitlement(Entitlement.DLC_2) && !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2))))
		{
			result = false;
			string text = "popup.incompatible-dlc";
			yield return this.ShowLoadFailedWithIssueDialog(selectable, text);
		}
		callback(result);
		yield break;
	}

	public IEnumerator ShowLoadFailedWithIssueDialog(Selectable s, string localizationKey)
	{
		this.DisableMenuCanvas();
		this.controllerFocusGrabber.enabled = false;
		DialogOptions dialogOptions = default(DialogOptions);
		dialogOptions.text = localizationKey;
		dialogOptions.buttonOptions = new DialogButtonOptions[]
		{
			new DialogButtonOptions
			{
				buttonString = "prompt.confirm",
				id = 1,
				hideOnButtonPress = true
			}
		};
		bool responded = false;
		GameManager.Instance.DialogManager.ShowDialog(dialogOptions, delegate(DialogButtonOptions options)
		{
			this.controllerFocusGrabber.enabled = true;
			this.controllerFocusGrabber.selectThis = s;
			this.EnableMenuCanvas();
			this.controllerFocusGrabber.SelectSelectable();
			responded = true;
		});
		yield return new WaitUntil(() => responded);
		yield break;
	}

	[SerializeField]
	private Canvas menuCanvas;

	[SerializeField]
	private CanvasGroup menuCanvasGroup;

	[SerializeField]
	private ContinueOrNewButton continueOrNewButton;

	[SerializeField]
	private Selectable startButton;

	[SerializeField]
	private Selectable loadButton;

	[SerializeField]
	private Selectable settingsButton;

	[SerializeField]
	private Selectable creditsButton;

	[SerializeField]
	private GameObject buttonGroup;

	[SerializeField]
	private PopupWindow saveSlotWindow;

	[SerializeField]
	private CanvasGroupDisabler buttonGroupCanvas;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private ControlPromptEntryUI SwitchUserGameCorePrompt;

	[SerializeField]
	private TMP_Text GameCoreName;

	private DredgePlayerActionHold switchUserAction;

	private GameObject cachedSelectedObject;
}
