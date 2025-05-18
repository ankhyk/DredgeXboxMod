using System;
using System.Collections;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerFocusGrabber : MonoBehaviour
{
	protected void OnEnable()
	{
		if (!this.hasAddedInputListener)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
			this.hasAddedInputListener = true;
		}
		if (!this.hasAddedDialogListener)
		{
			ApplicationEvents.Instance.OnDialogToggled += this.OnDialogToggled;
			this.hasAddedDialogListener = true;
		}
		base.StartCoroutine(this.DoSelectSelectable());
	}

	private void OnDisable()
	{
		if (this.hasAddedInputListener)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
			this.hasAddedInputListener = false;
		}
		if (this.hasAddedDialogListener)
		{
			ApplicationEvents.Instance.OnDialogToggled -= this.OnDialogToggled;
			this.hasAddedDialogListener = false;
		}
	}

	private void OnDialogToggled(bool visible)
	{
		if (visible && this.hasAddedInputListener)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
			return;
		}
		if (!visible && !this.hasAddedInputListener)
		{
			DredgeInputManager input2 = GameManager.Instance.Input;
			input2.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input2.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		base.StartCoroutine(this.DoSelectSelectable());
	}

	public void SelectSelectable()
	{
		base.StartCoroutine(this.DoSelectSelectable());
	}

	private IEnumerator DoSelectSelectable()
	{
		if (GameManager.Instance.Input.IsUsingController && this.selectThis)
		{
			EventSystem.current.SetSelectedGameObject(null);
			yield return new WaitForEndOfFrame();
			if (this.selectThis)
			{
				EventSystem.current.SetSelectedGameObject(this.selectThis.gameObject);
			}
		}
		yield break;
	}

	public void SetSelectable(Selectable newSelectable)
	{
		this.selectThis = newSelectable;
	}

	[SerializeField]
	public Selectable selectThis;

	private bool hasAddedInputListener;

	private bool hasAddedDialogListener;
}
