using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PopupDialog : MonoBehaviour
{
	private void OnCloseActionPressed()
	{
		this.OnButtonPressComplete(this.closeActionId);
	}

	private void OnButtonPressComplete(int id)
	{
		if (this.callback != null)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.closeAction };
			input.RemoveActionListener(array, ActionLayer.SYSTEM);
			Action<DialogButtonOptions> action = this.callback;
			if (action != null)
			{
				action(this.dialogOptions.buttonOptions[id]);
			}
			if (this.dialogOptions.buttonOptions[id].hideOnButtonPress)
			{
				this.Hide();
			}
			this.callback = null;
		}
	}

	public void Show(DialogOptions dialogOptions, Action<DialogButtonOptions> callback)
	{
		this.cachedCanPause = GameManager.Instance.CanPause;
		GameManager.Instance.CanPause = false;
		GameManager.Instance.CanUnpause = false;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.SYSTEM);
		ApplicationEvents.Instance.TriggerDialogToggled(true);
		if (dialogOptions.showScrim)
		{
			this.scrimTransitionEffect.Show(true);
		}
		bool flag = false;
		for (int i = 0; i < dialogOptions.buttonOptions.Length; i++)
		{
			if (dialogOptions.buttonOptions[i].isBackOption)
			{
				flag = true;
				this.closeActionId = dialogOptions.buttonOptions[i].id;
			}
		}
		if (flag)
		{
			this.closeAction = new DredgePlayerActionPress("prompt.back", GameManager.Instance.Input.Controls.Back);
			this.closeAction.showInControlArea = true;
			this.closeAction.evaluateWhenPaused = true;
			DredgePlayerActionPress dredgePlayerActionPress = this.closeAction;
			dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnCloseActionPressed));
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.closeAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
		}
		this.dialogOptions = dialogOptions;
		this.callback = callback;
		List<BasicButtonWrapper> list = new List<BasicButtonWrapper>();
		for (int j = 0; j < dialogOptions.buttonOptions.Length; j++)
		{
			BasicButtonWrapper component = global::UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab, this.buttonContainer).GetComponent<BasicButtonWrapper>();
			if (component)
			{
				component.LocalizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, dialogOptions.buttonOptions[j].buttonString);
				component.LocalizedString.RefreshString();
				int cachedIndex = j;
				BasicButtonWrapper basicButtonWrapper = component;
				basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(delegate
				{
					this.OnButtonPressComplete(cachedIndex);
				}));
				list.Add(component);
			}
		}
		this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, dialogOptions.text);
		this.localizedString.StringReference.Arguments = dialogOptions.textArguments;
		this.localizedString.RefreshString();
		for (int k = 0; k < list.Count; k++)
		{
			BasicButtonWrapper basicButtonWrapper2 = list[k];
			Navigation navigation = basicButtonWrapper2.Navigation;
			navigation.mode = Navigation.Mode.Explicit;
			if (k > 0)
			{
				Button button = list[k - 1].Button;
				navigation.selectOnLeft = button;
			}
			if (k < list.Count - 1)
			{
				BasicButtonWrapper basicButtonWrapper3 = list[k + 1];
				navigation.selectOnRight = basicButtonWrapper3.Button;
			}
			basicButtonWrapper2.Button.navigation = navigation;
		}
		base.StartCoroutine(this.SelectFirstButton(list[0].gameObject));
	}

	private IEnumerator SelectFirstButton(GameObject gameObject)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			EventSystem.current.SetSelectedGameObject(null);
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(gameObject);
		}
		yield break;
	}

	private void OnDestroy()
	{
	}

	public void Hide()
	{
		ApplicationEvents.Instance.TriggerDialogToggled(false);
		GameManager.Instance.CanPause = this.cachedCanPause;
		GameManager.Instance.CanUnpause = true;
		this.closeAction = null;
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private LocalizeStringEvent localizedString;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private Transform buttonContainer;

	[SerializeField]
	private UITransitionEffect scrimTransitionEffect;

	private DredgePlayerActionPress closeAction;

	private int closeActionId;

	private DialogOptions dialogOptions;

	private Action<DialogButtonOptions> callback;

	private bool cachedCanPause;
}
