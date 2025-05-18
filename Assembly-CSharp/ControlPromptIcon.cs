using System;
using InControl;
using UnityEngine;
using UnityEngine.UI;

public class ControlPromptIcon : MonoBehaviour
{
	public void Init(DredgePlayerActionBase dredgePlayerAction)
	{
		this.Init(dredgePlayerAction, dredgePlayerAction.GetPrimaryPlayerAction());
	}

	public void Init(DredgePlayerActionBase dredgePlayerAction, PlayerAction playerAction)
	{
		if (this.dredgePlayerAction != null)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.dredgePlayerAction;
			dredgePlayerActionBase.OnPressBegin = (Action)Delegate.Remove(dredgePlayerActionBase.OnPressBegin, new Action(this.OnPressBegin));
			DredgePlayerActionBase dredgePlayerActionBase2 = this.dredgePlayerAction;
			dredgePlayerActionBase2.OnPressEnd = (Action)Delegate.Remove(dredgePlayerActionBase2.OnPressEnd, new Action(this.OnPressEnd));
			DredgePlayerActionBase dredgePlayerActionBase3 = this.dredgePlayerAction;
			dredgePlayerActionBase3.OnEnableStatusChanged = (Action<bool>)Delegate.Remove(dredgePlayerActionBase3.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
		}
		this.holdActionFill.gameObject.SetActive(false);
		this.holdActionBack.gameObject.SetActive(false);
		this.delegateActionFill.gameObject.SetActive(false);
		this.controlImage.gameObject.SetActive(false);
		this.dredgePlayerAction = dredgePlayerAction;
		this.playerAction = playerAction;
		this.OnInputChanged(GameManager.Instance.Input.CurrentBindingSource, GameManager.Instance.Input.CurrentDeviceStyle);
		if (this.dredgePlayerAction != null)
		{
			if (dredgePlayerAction is DredgePlayerActionHold)
			{
				this.updateDelegate = new ControlPromptIcon.UpdateDelegate(this.PlayerActionHoldUpdate);
				this.holdActionFill.gameObject.SetActive(true);
				this.holdActionBack.gameObject.SetActive(true);
				this.holdActionFill.fillAmount = 1f - (dredgePlayerAction as DredgePlayerActionHold).currentHoldTime;
			}
			else if (dredgePlayerAction is DredgePlayerActionHoldDelegate)
			{
				this.updateDelegate = new ControlPromptIcon.UpdateDelegate(this.PlayerActionHoldUpdateDelegate);
				this.delegateActionFill.gameObject.SetActive(true);
			}
			else
			{
				this.updateDelegate = null;
			}
			ControlPromptIcon.UpdateDelegate updateDelegate = this.updateDelegate;
			if (updateDelegate != null)
			{
				updateDelegate();
			}
			DredgePlayerActionBase dredgePlayerActionBase4 = this.dredgePlayerAction;
			dredgePlayerActionBase4.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionBase4.OnPressBegin, new Action(this.OnPressBegin));
			DredgePlayerActionBase dredgePlayerActionBase5 = this.dredgePlayerAction;
			dredgePlayerActionBase5.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionBase5.OnPressEnd, new Action(this.OnPressEnd));
			DredgePlayerActionBase dredgePlayerActionBase6 = this.dredgePlayerAction;
			dredgePlayerActionBase6.OnEnableStatusChanged = (Action<bool>)Delegate.Combine(dredgePlayerActionBase6.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
			this.hasAddedListeners = true;
			this.OnEnableStatusChanged(dredgePlayerAction.Enabled);
		}
	}

	private void OnEnableStatusChanged(bool enabled)
	{
		if (this && base.gameObject)
		{
			this.controlImage.color = (enabled ? this.enabledColor : this.disabledColor);
			this.holdActionFill.color = (enabled ? this.holdFillEnabledColor : this.disabledColor);
			this.holdActionBack.color = (enabled ? this.enabledColor : this.disabledColor);
			this.delegateActionFill.color = (enabled ? this.enabledColor : this.disabledColor);
		}
	}

	private void OnEnable()
	{
		if (this.dredgePlayerAction != null && !this.hasAddedListeners)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.dredgePlayerAction;
			dredgePlayerActionBase.OnEnableStatusChanged = (Action<bool>)Delegate.Combine(dredgePlayerActionBase.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
			DredgePlayerActionBase dredgePlayerActionBase2 = this.dredgePlayerAction;
			dredgePlayerActionBase2.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionBase2.OnPressBegin, new Action(this.OnPressBegin));
			DredgePlayerActionBase dredgePlayerActionBase3 = this.dredgePlayerAction;
			dredgePlayerActionBase3.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionBase3.OnPressEnd, new Action(this.OnPressEnd));
			this.hasAddedListeners = true;
			this.OnEnableStatusChanged(this.dredgePlayerAction.Enabled);
		}
		ApplicationEvents.Instance.OnPlayerActionBindingChanged += this.OnPlayerActionBindingChanged;
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		this.OnInputChanged(GameManager.Instance.Input.CurrentBindingSource, GameManager.Instance.Input.CurrentDeviceStyle);
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		if (this.dredgePlayerAction != null)
		{
			DredgePlayerActionBase dredgePlayerActionBase = this.dredgePlayerAction;
			dredgePlayerActionBase.OnEnableStatusChanged = (Action<bool>)Delegate.Remove(dredgePlayerActionBase.OnEnableStatusChanged, new Action<bool>(this.OnEnableStatusChanged));
			DredgePlayerActionBase dredgePlayerActionBase2 = this.dredgePlayerAction;
			dredgePlayerActionBase2.OnPressBegin = (Action)Delegate.Remove(dredgePlayerActionBase2.OnPressBegin, new Action(this.OnPressBegin));
			DredgePlayerActionBase dredgePlayerActionBase3 = this.dredgePlayerAction;
			dredgePlayerActionBase3.OnPressEnd = (Action)Delegate.Remove(dredgePlayerActionBase3.OnPressEnd, new Action(this.OnPressEnd));
		}
		ApplicationEvents.Instance.OnPlayerActionBindingChanged -= this.OnPlayerActionBindingChanged;
		this.hasAddedListeners = false;
	}

	private void OnPlayerActionBindingChanged(PlayerAction changedPlayerAction)
	{
		if (changedPlayerAction == this.playerAction)
		{
			this.Init(this.dredgePlayerAction, this.playerAction);
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		if (!this || this.playerAction == null)
		{
			return;
		}
		ControlIconData controlIconForActionWithDefault = GameManager.Instance.Input.GetControlIconForActionWithDefault(this.playerAction);
		if (controlIconForActionWithDefault != null)
		{
			this.upSprite = controlIconForActionWithDefault.upSprite;
			this.downSprite = controlIconForActionWithDefault.downSprite;
			this.controlImage.sprite = this.upSprite;
			this.controlImage.gameObject.SetActive(true);
			return;
		}
		base.gameObject.SetActive(false);
	}

	private void OnPressBegin()
	{
		if (this.controlImage)
		{
			this.controlImage.sprite = this.downSprite;
		}
	}

	private void OnPressEnd()
	{
		if (this.controlImage)
		{
			this.controlImage.sprite = this.upSprite;
		}
	}

	private void Update()
	{
		ControlPromptIcon.UpdateDelegate updateDelegate = this.updateDelegate;
		if (updateDelegate == null)
		{
			return;
		}
		updateDelegate();
	}

	private void PlayerActionHoldUpdate()
	{
		DredgePlayerActionHold dredgePlayerActionHold = this.dredgePlayerAction as DredgePlayerActionHold;
		this.holdActionFill.fillAmount = 1f - dredgePlayerActionHold.currentHoldTime / dredgePlayerActionHold.holdTimeRequiredSec;
	}

	private void PlayerActionHoldUpdateDelegate()
	{
		if (this.dredgePlayerAction.IsHeld() || this.dredgePlayerAction.IsForcePressed())
		{
			this.delegateActionFill.transform.eulerAngles += new Vector3(0f, 0f, -(this.delegateSpinSpeedDegPerSec * Time.deltaTime));
		}
	}

	[SerializeField]
	private Image controlImage;

	[SerializeField]
	private Image holdActionFill;

	[SerializeField]
	private Image holdActionBack;

	[SerializeField]
	private Image delegateActionFill;

	[SerializeField]
	private Color enabledColor;

	[SerializeField]
	private Color holdFillEnabledColor;

	[SerializeField]
	private Color disabledColor;

	[SerializeField]
	private float delegateSpinSpeedDegPerSec;

	private Sprite upSprite;

	private Sprite downSprite;

	private DredgePlayerActionBase dredgePlayerAction;

	private PlayerAction playerAction;

	private bool hasAddedListeners;

	private ControlPromptIcon.UpdateDelegate updateDelegate;

	private delegate void UpdateDelegate();
}
