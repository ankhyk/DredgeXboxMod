using System;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ControlPromptEntryUI : MonoBehaviour
{
	public void Init(DredgePlayerActionBase dredgePlayerAction, ControlPromptUI.ControlPromptMode controlPromptMode)
	{
		this.dredgePlayerAction = dredgePlayerAction;
		this.controlPromptMode = controlPromptMode;
		this.alertColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		this.backplate.enabled = controlPromptMode == ControlPromptUI.ControlPromptMode.BOTTOM_RIGHT || controlPromptMode == ControlPromptUI.ControlPromptMode.CUSTOM;
		if (controlPromptMode != ControlPromptUI.ControlPromptMode.CUSTOM)
		{
			this.localizedString.StringReference.Arguments = dredgePlayerAction.LocalizationArguments;
			this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, dredgePlayerAction.promptString);
			this.localizedString.RefreshString();
		}
		if (this.hasInit)
		{
			this.Unsubscribe();
		}
		dredgePlayerAction.OnPromptArgumentsChanged = (Action)Delegate.Combine(dredgePlayerAction.OnPromptArgumentsChanged, new Action(this.OnPromptArgumentsChanged));
		dredgePlayerAction.OnEnableStatusChanged = (Action<bool>)Delegate.Combine(dredgePlayerAction.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
		dredgePlayerAction.OnPromptStringChanged = (Action<string>)Delegate.Combine(dredgePlayerAction.OnPromptStringChanged, new Action<string>(this.OnPromptStringChanged));
		this.OnEnableStatusChanged(dredgePlayerAction.Enabled);
		PlayerAction primaryPlayerAction = dredgePlayerAction.GetPrimaryPlayerAction();
		PlayerAction secondaryPlayerAction = dredgePlayerAction.GetSecondaryPlayerAction();
		bool flag = primaryPlayerAction != null && GameManager.Instance.Input.GetHasBindingForAction(primaryPlayerAction, GameManager.Instance.Input.CurrentBindingSource, true);
		bool flag2 = secondaryPlayerAction != null && GameManager.Instance.Input.GetHasBindingForAction(secondaryPlayerAction, GameManager.Instance.Input.CurrentBindingSource, true);
		this.shouldShowAction1 = flag || !flag2;
		this.controlPromptIcon1.gameObject.SetActive(this.shouldShowAction1);
		if (this.shouldShowAction1)
		{
			this.controlPromptIcon1.Init(dredgePlayerAction, primaryPlayerAction);
		}
		this.shouldShowAction2 = flag2;
		this.controlPromptIcon2.gameObject.SetActive(this.shouldShowAction2);
		if (this.shouldShowAction2)
		{
			this.controlPromptIcon2.Init(dredgePlayerAction, secondaryPlayerAction);
		}
		if (controlPromptMode != ControlPromptUI.ControlPromptMode.CUSTOM)
		{
			Canvas.ForceUpdateCanvases();
			this.mainHorizontalLayoutGroup.enabled = false;
			this.mainHorizontalLayoutGroup.enabled = true;
			this.iconHorizontalLayoutGroup.enabled = false;
			this.iconHorizontalLayoutGroup.enabled = true;
		}
		if (dredgePlayerAction is DredgePlayerActionHold)
		{
			this.updateDelegate = new ControlPromptEntryUI.UpdateDelegate(this.PlayerActionHoldUpdate);
		}
		this.hasInit = true;
	}

	private void Unsubscribe()
	{
		if (this.dredgePlayerAction != null)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.dredgePlayerAction;
			dredgePlayerActionBase.OnEnableStatusChanged = (Action<bool>)Delegate.Remove(dredgePlayerActionBase.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
			DredgePlayerActionBase dredgePlayerActionBase2 = this.dredgePlayerAction;
			dredgePlayerActionBase2.OnPromptStringChanged = (Action<string>)Delegate.Remove(dredgePlayerActionBase2.OnPromptStringChanged, new Action<string>(this.OnPromptStringChanged));
			DredgePlayerActionBase dredgePlayerActionBase3 = this.dredgePlayerAction;
			dredgePlayerActionBase3.OnPromptArgumentsChanged = (Action)Delegate.Remove(dredgePlayerActionBase3.OnPromptArgumentsChanged, new Action(this.OnPromptArgumentsChanged));
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.controlPromptIcon1.gameObject.SetActive(this.shouldShowAction1);
		this.controlPromptIcon2.gameObject.SetActive(this.shouldShowAction2);
	}

	private void OnEnable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnPointerDownAction = (Action)Delegate.Combine(basicButtonWrapper.OnPointerDownAction, new Action(this.OnPointerDown));
		BasicButtonWrapper basicButtonWrapper2 = this.basicButtonWrapper;
		basicButtonWrapper2.OnPointerUpAction = (Action)Delegate.Combine(basicButtonWrapper2.OnPointerUpAction, new Action(this.OnPointerUp));
		BasicButtonWrapper basicButtonWrapper3 = this.basicButtonWrapper;
		basicButtonWrapper3.OnPointerExitAction = (Action)Delegate.Combine(basicButtonWrapper3.OnPointerExitAction, new Action(this.OnPointerUp));
		this.localizedString.OnUpdateString.AddListener(new UnityAction<string>(this.OnStringUpdated));
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		this.Unsubscribe();
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnPointerDownAction = (Action)Delegate.Remove(basicButtonWrapper.OnPointerDownAction, new Action(this.OnPointerDown));
		BasicButtonWrapper basicButtonWrapper2 = this.basicButtonWrapper;
		basicButtonWrapper2.OnPointerUpAction = (Action)Delegate.Remove(basicButtonWrapper2.OnPointerUpAction, new Action(this.OnPointerUp));
		BasicButtonWrapper basicButtonWrapper3 = this.basicButtonWrapper;
		basicButtonWrapper3.OnPointerExitAction = (Action)Delegate.Remove(basicButtonWrapper3.OnPointerExitAction, new Action(this.OnPointerUp));
		this.localizedString.OnUpdateString.RemoveListener(new UnityAction<string>(this.OnStringUpdated));
	}

	private void OnPointerDown()
	{
		if (this.dredgePlayerAction != null)
		{
			this.dredgePlayerAction.ForcePointerDown();
		}
	}

	private void OnPointerUp()
	{
		if (this.dredgePlayerAction != null)
		{
			this.dredgePlayerAction.ForcePointerUp();
		}
	}

	private void OnEnableStatusChanged(bool enabled)
	{
		this.controlTextField.color = (enabled ? this.enabledColor : this.disabledColor);
	}

	private void OnPromptStringChanged(string promptString)
	{
		if (this.controlPromptMode != ControlPromptUI.ControlPromptMode.CUSTOM)
		{
			this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, this.dredgePlayerAction.promptString);
			this.localizedString.RefreshString();
		}
	}

	private void OnStringUpdated(string str)
	{
		Canvas.ForceUpdateCanvases();
		if (this.mainHorizontalLayoutGroup)
		{
			this.mainHorizontalLayoutGroup.enabled = false;
			this.mainHorizontalLayoutGroup.enabled = true;
		}
	}

	private void OnPromptArgumentsChanged()
	{
		if (this.controlPromptMode != ControlPromptUI.ControlPromptMode.CUSTOM)
		{
			this.localizedString.StringReference.Arguments = this.dredgePlayerAction.LocalizationArguments;
			this.localizedString.RefreshString();
		}
	}

	private void Update()
	{
		ControlPromptEntryUI.UpdateDelegate updateDelegate = this.updateDelegate;
		if (updateDelegate == null)
		{
			return;
		}
		updateDelegate();
	}

	private void PlayerActionHoldUpdate()
	{
		DredgePlayerActionHold dredgePlayerActionHold = this.dredgePlayerAction as DredgePlayerActionHold;
		if (dredgePlayerActionHold.showAlertOnHold)
		{
			if (dredgePlayerActionHold.IsHeld())
			{
				float num = Mathf.PingPong(dredgePlayerActionHold.currentHoldTime, 0.1f) * 10f;
				this.controlTextField.color = Color.Lerp(this.enabledColor, this.alertColor, num);
				return;
			}
			if (dredgePlayerActionHold.WasReleased())
			{
				this.controlTextField.color = this.enabledColor;
			}
		}
	}

	[SerializeField]
	private HorizontalLayoutGroup mainHorizontalLayoutGroup;

	[SerializeField]
	private HorizontalLayoutGroup iconHorizontalLayoutGroup;

	[SerializeField]
	private Image backplate;

	[SerializeField]
	private ControlPromptIcon controlPromptIcon1;

	[SerializeField]
	private ControlPromptIcon controlPromptIcon2;

	[SerializeField]
	private TextMeshProUGUI controlTextField;

	[SerializeField]
	private LocalizeStringEvent localizedString;

	[SerializeField]
	private DredgePlayerActionBase dredgePlayerAction;

	[SerializeField]
	private Color enabledColor;

	[SerializeField]
	private Color disabledColor;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	private ControlPromptUI.ControlPromptMode controlPromptMode;

	private bool shouldShowAction1;

	private bool shouldShowAction2;

	private bool hasInit;

	private Color alertColor;

	private ControlPromptEntryUI.UpdateDelegate updateDelegate;

	private delegate void UpdateDelegate();
}
