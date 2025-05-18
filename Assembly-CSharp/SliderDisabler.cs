using System;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderDisabler : MonoBehaviour, ISubmitHandler, IEventSystemHandler, IDeselectHandler, ISelectHandler
{
	private void OnEnable()
	{
		this.RefreshInteractionState();
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.RefreshInteractionState();
	}

	private void RefreshInteractionState()
	{
		this.slider.interactable = !GameManager.Instance.Input.IsUsingController;
	}

	public void OnSelect(BaseEventData eventData)
	{
		ApplicationEvents.Instance.TriggerSliderFocusToggled(true);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		Action sliderDeselected = this.SliderDeselected;
		if (sliderDeselected != null)
		{
			sliderDeselected();
		}
		this.RefreshInteractionState();
		ApplicationEvents.Instance.TriggerSliderFocusToggled(false);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action sliderSubmitted = this.SliderSubmitted;
		if (sliderSubmitted != null)
		{
			sliderSubmitted();
		}
		ApplicationEvents.Instance.TriggerSliderFocusToggled(false);
	}

	[SerializeField]
	private Button parentSelectable;

	[SerializeField]
	private Slider slider;

	[HideInInspector]
	public Action SliderDeselected;

	[HideInInspector]
	public Action SliderSubmitted;
}
