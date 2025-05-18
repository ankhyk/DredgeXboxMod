using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlMappingContainer : MonoBehaviour
{
	private void Start()
	{
		if (this.allowRebinding)
		{
			this.unbindAction = new DredgePlayerActionPress("prompt.unbind", GameManager.Instance.Input.Controls.UnbindControl);
			this.unbindAction.showInControlArea = true;
			this.unbindAction.evaluateWhenPaused = true;
			this.unbindAction.priority = 1;
			DredgePlayerActionPress dredgePlayerActionPress = this.unbindAction;
			dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnUnbindPressComplete));
		}
	}

	private void OnEnable()
	{
		this.allowRebinding = GameManager.Instance.GameConfigData.PlatformsSupportingControlRebindings.Contains(RuntimePlatform.WindowsPlayer);
		this.PopulateGrid();
		this.itemEntryContainer.position = new Vector2(this.itemEntryContainer.position.x, this.itemEntryContainer.sizeDelta.y * -0.5f);
		this.UpdateFooter();
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		if (this.allowRebinding)
		{
			ApplicationEvents.Instance.OnPlayerActionBindingChanged += this.OnPlayerActionBindingChanged;
			ApplicationEvents.Instance.OnPlayerActionBindingEnded += this.OnPlayerActionBindingEnded;
			BasicButtonWrapper basicButtonWrapper = this.resetAllButton;
			basicButtonWrapper.OnClick = (Action)Delegate.Combine(basicButtonWrapper.OnClick, new Action(this.OnResetAllClicked));
		}
		Navigation navigation = this.resumeButton.Button.navigation;
		Navigation navigation2 = this.resetAllButton.Button.navigation;
		Navigation navigation3 = this.saveAndQuitButton.Button.navigation;
		Navigation navigation4 = this.resetAllSettingsButton.Button.navigation;
		navigation.mode = Navigation.Mode.Explicit;
		navigation2.mode = Navigation.Mode.Explicit;
		navigation3.mode = Navigation.Mode.Explicit;
		navigation4.mode = Navigation.Mode.Explicit;
		navigation2.selectOnDown = this.resumeButton.Button;
		navigation.selectOnUp = this.resetAllButton.Button;
		navigation3.selectOnUp = this.resetAllButton.Button;
		navigation4.selectOnUp = this.resetAllButton.Button;
		navigation4.selectOnRight = this.resumeButton.Button;
		navigation.selectOnRight = this.saveAndQuitButton.Button;
		navigation.selectOnLeft = this.resetAllSettingsButton.Button;
		navigation3.selectOnLeft = this.resumeButton.Button;
		this.resumeButton.Button.navigation = navigation;
		this.resetAllButton.Button.navigation = navigation2;
		this.saveAndQuitButton.Button.navigation = navigation3;
		this.resetAllSettingsButton.Button.navigation = navigation4;
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		ApplicationEvents.Instance.OnPlayerActionBindingChanged -= this.OnPlayerActionBindingChanged;
		ApplicationEvents.Instance.OnPlayerActionBindingEnded -= this.OnPlayerActionBindingEnded;
		BasicButtonWrapper basicButtonWrapper = this.resetAllButton;
		basicButtonWrapper.OnClick = (Action)Delegate.Remove(basicButtonWrapper.OnClick, new Action(this.OnResetAllClicked));
		this.UpdateLastHoveredBindingEntry(null);
		Navigation navigation = this.resumeButton.Button.navigation;
		Navigation navigation2 = this.saveAndQuitButton.Button.navigation;
		Navigation navigation3 = this.resetAllSettingsButton.Button.navigation;
		navigation.mode = Navigation.Mode.Automatic;
		navigation2.mode = Navigation.Mode.Automatic;
		navigation3.mode = Navigation.Mode.Automatic;
		this.resumeButton.Button.navigation = navigation;
		this.saveAndQuitButton.Button.navigation = navigation2;
		this.resetAllSettingsButton.Button.navigation = navigation3;
		this.Clear();
	}

	private void OnPlayerActionBindingEnded(PlayerAction playerAction)
	{
		this.UpdateFooter();
	}

	private void OnUnbindPressComplete()
	{
		if (this.lastHoveredBindingEntry)
		{
			BindingSource bindingForAction = GameManager.Instance.Input.GetBindingForAction(this.lastHoveredBindingEntry.PlayerAction, this.lastHoveredBindingEntry.BindingSourceType, false);
			if (bindingForAction != null)
			{
				this.lastHoveredBindingEntry.PlayerAction.RemoveBinding(bindingForAction);
				base.StartCoroutine(this.DelayedRefreshBinding(this.lastHoveredBindingEntry));
			}
		}
	}

	private IEnumerator DelayedRefreshBinding(ControlBindingEntryUI entry)
	{
		yield return new WaitForEndOfFrame();
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.CONTROL_BINDINGS);
		entry.Refresh();
		yield break;
	}

	private void UpdateFooter()
	{
		if (this.idleFooter)
		{
			this.idleFooter.SetActive(!GameManager.Instance.Input.Controls.IsListeningForBinding);
		}
		if (this.listeningFooter)
		{
			this.listeningFooter.SetActive(GameManager.Instance.Input.Controls.IsListeningForBinding);
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.controlEntries.ForEach(delegate(ControlEntryUI c)
		{
			c.Refresh();
		});
		if (this.lerpCoroutine != null)
		{
			base.StopCoroutine(this.lerpCoroutine);
			this.lerpCoroutine = null;
		}
		if (this.controllerFocusGrabber == null || this.controllerFocusGrabber.selectThis == null)
		{
			this.SelectFirstGridEntry();
		}
	}

	private void PopulateGrid()
	{
		this.Clear();
		int num = 0;
		Color color = new Color(0f, 0f, 0f, 0f);
		for (int i = 0; i < GameManager.Instance.Input.Controls.Actions.Count; i++)
		{
			PlayerAction playerAction = GameManager.Instance.Input.Controls.Actions[i];
			if (!GameManager.Instance.Input.Controls.hidden.Contains(playerAction))
			{
				bool rebindable = this.allowRebinding && !GameManager.Instance.Input.Controls.unRebindable.Contains(playerAction);
				bool flag = this.allowRebinding && !GameManager.Instance.Input.Controls.unUnbindable.Contains(playerAction);
				bool rebindable2 = rebindable;
				ControlEntryUI componentInChildren = global::UnityEngine.Object.Instantiate<GameObject>(this.controlEntryPrefab, this.itemEntryContainer).GetComponentInChildren<ControlEntryUI>();
				if (componentInChildren)
				{
					if (num % 2 == 0)
					{
						Image component = componentInChildren.GetComponent<Image>();
						if (component)
						{
							component.color = color;
						}
					}
					componentInChildren.Init(playerAction, rebindable, flag);
					componentInChildren.ControlBindingEntryUIs.ForEach(delegate(ControlBindingEntryUI c)
					{
						c.OnEntrySelected = (Action<ControlBindingEntryUI>)Delegate.Combine(c.OnEntrySelected, new Action<ControlBindingEntryUI>(this.OnEntrySelected));
						if (rebindable)
						{
							c.OnEntrySubmitted = (Action<ControlBindingEntryUI>)Delegate.Combine(c.OnEntrySubmitted, new Action<ControlBindingEntryUI>(this.OnEntrySubmitted));
						}
					});
					this.controlEntries.Add(componentInChildren);
					if (i == 0)
					{
						this.controllerFocusGrabber.SetSelectable(componentInChildren.ControlBindingEntryUIs[0].ButtonWrapper.Button);
					}
				}
				if (this.allowRebinding && componentInChildren.ResetEntryUI)
				{
					componentInChildren.ResetEntryUI.Init(playerAction, rebindable);
					this.resetEntries.Add(componentInChildren.ResetEntryUI);
					ResetControlEntryUI resetEntryUI = componentInChildren.ResetEntryUI;
					resetEntryUI.OnEntrySelected = (Action<ResetControlEntryUI>)Delegate.Combine(resetEntryUI.OnEntrySelected, new Action<ResetControlEntryUI>(this.OnResetEntrySelected));
					ResetControlEntryUI resetEntryUI2 = componentInChildren.ResetEntryUI;
					resetEntryUI2.OnEntrySubmitted = (Action<ResetControlEntryUI>)Delegate.Combine(resetEntryUI2.OnEntrySubmitted, new Action<ResetControlEntryUI>(this.OnResetEntrySubmitted));
				}
				num++;
			}
		}
		this.LinkControlEntries(this.allowRebinding);
		base.StartCoroutine(this.OnLoaded());
	}

	private void LinkControlEntries(bool withRebinding)
	{
		for (int i = 0; i < this.controlEntries.Count; i++)
		{
			ControlEntryUI controlEntryUI = this.controlEntries[i];
			if (i < this.controlEntries.Count - 1)
			{
				ControlEntryUI controlEntryUI2 = this.controlEntries[i + 1];
				for (int j = 0; j < controlEntryUI.ControlBindingEntryUIs.Count; j++)
				{
					Navigation navigation = controlEntryUI.ControlBindingEntryUIs[j].ButtonWrapper.Button.navigation;
					navigation.selectOnDown = controlEntryUI2.ControlBindingEntryUIs[j].ButtonWrapper.Button;
					controlEntryUI.ControlBindingEntryUIs[j].ButtonWrapper.Button.navigation = navigation;
				}
				if (withRebinding)
				{
					ResetControlEntryUI resetControlEntryUI = this.resetEntries[i];
					Navigation navigation2 = resetControlEntryUI.ButtonWrapper.Button.navigation;
					ResetControlEntryUI resetControlEntryUI2 = this.resetEntries[i + 1];
					navigation2.selectOnDown = resetControlEntryUI2.ButtonWrapper.Button;
					resetControlEntryUI.ButtonWrapper.Button.navigation = navigation2;
				}
			}
			if (i > 0)
			{
				ControlEntryUI controlEntryUI3 = this.controlEntries[i - 1];
				for (int k = 0; k < controlEntryUI.ControlBindingEntryUIs.Count; k++)
				{
					Navigation navigation3 = controlEntryUI.ControlBindingEntryUIs[k].ButtonWrapper.Button.navigation;
					navigation3.selectOnUp = controlEntryUI3.ControlBindingEntryUIs[k].ButtonWrapper.Button;
					controlEntryUI.ControlBindingEntryUIs[k].ButtonWrapper.Button.navigation = navigation3;
				}
				if (withRebinding)
				{
					ResetControlEntryUI resetControlEntryUI3 = this.resetEntries[i];
					Navigation navigation4 = resetControlEntryUI3.ButtonWrapper.Button.navigation;
					ResetControlEntryUI resetControlEntryUI4 = this.resetEntries[i - 1];
					navigation4.selectOnUp = resetControlEntryUI4.ButtonWrapper.Button;
					resetControlEntryUI3.ButtonWrapper.Button.navigation = navigation4;
				}
			}
			if (i == this.controlEntries.Count - 1)
			{
				for (int l = 0; l < controlEntryUI.ControlBindingEntryUIs.Count; l++)
				{
					Navigation navigation5 = controlEntryUI.ControlBindingEntryUIs[l].ButtonWrapper.Button.navigation;
					navigation5.selectOnDown = this.resetAllButton.Button;
					controlEntryUI.ControlBindingEntryUIs[l].ButtonWrapper.Button.navigation = navigation5;
				}
				if (withRebinding)
				{
					Navigation navigation6 = controlEntryUI.ResetEntryUI.ButtonWrapper.Button.navigation;
					Navigation navigation7 = this.resetAllButton.Button.navigation;
					navigation6.selectOnDown = this.resetAllButton.Button;
					navigation7.selectOnUp = controlEntryUI.ResetEntryUI.ButtonWrapper.Button;
					controlEntryUI.ResetEntryUI.ButtonWrapper.Button.navigation = navigation6;
					this.resetAllButton.Button.navigation = navigation7;
				}
			}
		}
	}

	private IEnumerator OnLoaded()
	{
		yield return new WaitForEndOfFrame();
		EventSystem.current.SetSelectedGameObject(this.controlEntries[0].ControlBindingEntryUIs[0].ButtonWrapper.Button.gameObject);
		yield break;
	}

	public void SelectFirstGridEntry()
	{
		EventSystem.current.SetSelectedGameObject(this.controlEntries[0].ControlBindingEntryUIs[0].ButtonWrapper.Button.gameObject);
		this.controlEntries[0].ControlBindingEntryUIs[0].ButtonWrapper.Button.Select();
		if (!GameManager.Instance.Input.IsUsingController)
		{
			this.scrollRect.content.anchoredPosition = this.scrollRect.GetSnapToPositionToBringChildIntoView(this.controlEntries[0].gameObject.transform as RectTransform);
		}
	}

	private void OnPlayerActionBindingChanged(PlayerAction playerAction)
	{
		ControlEntryUI controlEntryUI = this.controlEntries.Find((ControlEntryUI c) => c.PlayerAction == playerAction);
		if (controlEntryUI)
		{
			controlEntryUI.Refresh();
		}
	}

	private IEnumerator LerpToDestinationPos(Vector2 destinationPos)
	{
		Canvas.ForceUpdateCanvases();
		this.isLerpingToDestinationPos = true;
		float scrollTime = 0f;
		float maxScrollTimeSec = 0.15f;
		float distanceThreshold = 10f;
		while (this.isLerpingToDestinationPos)
		{
			scrollTime += Time.unscaledDeltaTime;
			float num = Mathf.Min(10f * Time.unscaledDeltaTime, 1f);
			this.scrollRect.content.anchoredPosition = Vector2.Lerp(this.scrollRect.content.anchoredPosition, destinationPos, num);
			if (scrollTime >= maxScrollTimeSec || Vector2.SqrMagnitude(this.scrollRect.content.anchoredPosition - destinationPos) < distanceThreshold)
			{
				this.scrollRect.content.anchoredPosition = destinationPos;
				this.isLerpingToDestinationPos = false;
			}
			yield return null;
		}
		this.lerpCoroutine = null;
		yield break;
	}

	private void OnEntrySelected(ControlBindingEntryUI controlEntryUI)
	{
		this.UpdateLastHoveredBindingEntry(controlEntryUI);
		if (controlEntryUI)
		{
			this.OnSomethingSelected(controlEntryUI.ButtonWrapper.Button, controlEntryUI.gameObject.transform as RectTransform);
		}
	}

	private void OnResetEntrySelected(ResetControlEntryUI resetControlEntryUI)
	{
		this.UpdateLastHoveredBindingEntry(null);
		this.OnSomethingSelected(resetControlEntryUI.ButtonWrapper.Button, resetControlEntryUI.gameObject.transform as RectTransform);
	}

	private void UpdateLastHoveredBindingEntry(ControlBindingEntryUI controlEntryUI)
	{
		this.lastHoveredBindingEntry = controlEntryUI;
		if (this.allowRebinding)
		{
			DredgePlayerActionBase[] array;
			if (this.lastHoveredBindingEntry && this.lastHoveredBindingEntry.Unbindable)
			{
				DredgeInputManager input = GameManager.Instance.Input;
				array = new DredgePlayerActionPress[] { this.unbindAction };
				input.AddActionListener(array, ActionLayer.SYSTEM);
				return;
			}
			DredgeInputManager input2 = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.unbindAction };
			input2.RemoveActionListener(array, ActionLayer.SYSTEM);
		}
	}

	private void OnSomethingSelected(Selectable selectable, RectTransform rectTransform)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			if (this.lerpCoroutine != null)
			{
				base.StopCoroutine(this.lerpCoroutine);
			}
			Vector2 snapToPositionToBringChildIntoView = this.scrollRect.GetSnapToPositionToBringChildIntoView(rectTransform);
			this.lerpCoroutine = base.StartCoroutine(this.LerpToDestinationPos(snapToPositionToBringChildIntoView));
		}
		this.controllerFocusGrabber.SetSelectable(selectable);
	}

	private void OnEntrySubmitted(ControlBindingEntryUI controlEntryUI)
	{
		if (controlEntryUI.Rebindable && !controlEntryUI.PlayerAction.IsListeningForBinding)
		{
			controlEntryUI.PlayerAction.ListenForBinding();
			this.UpdateFooter();
		}
	}

	private void OnResetEntrySubmitted(ResetControlEntryUI resetControlEntryUI)
	{
		if (resetControlEntryUI.Rebindable)
		{
			GameManager.Instance.Input.ResetBinding(resetControlEntryUI.PlayerAction);
		}
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.CONTROL_BINDINGS);
	}

	private void OnResetAllClicked()
	{
		GameManager.Instance.Input.ResetAllBindings();
	}

	private void Clear()
	{
		foreach (object obj in this.itemEntryContainer)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		this.controlEntries.Clear();
		this.resetEntries.Clear();
	}

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private GameObject controlEntryPrefab;

	[SerializeField]
	private RectTransform itemEntryContainer;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private GameObject idleFooter;

	[SerializeField]
	private GameObject listeningFooter;

	[SerializeField]
	private BasicButtonWrapper resetAllSettingsButton;

	[SerializeField]
	private BasicButtonWrapper resetAllButton;

	[SerializeField]
	private BasicButtonWrapper resumeButton;

	[SerializeField]
	private BasicButtonWrapper saveAndQuitButton;

	private List<ControlEntryUI> controlEntries = new List<ControlEntryUI>();

	private List<ResetControlEntryUI> resetEntries = new List<ResetControlEntryUI>();

	private Coroutine lerpCoroutine;

	private bool isLerpingToDestinationPos;

	private bool allowRebinding;

	private DredgePlayerActionPress unbindAction;

	private ControlBindingEntryUI lastHoveredBindingEntry;
}
