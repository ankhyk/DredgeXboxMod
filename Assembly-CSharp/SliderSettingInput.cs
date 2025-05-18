using System;
using InControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SliderSettingInput : MonoBehaviour, ISubmitHandler, IEventSystemHandler, ISettingsRefreshable
{
	private void OnEnable()
	{
		this.RefreshInteractionState();
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		SliderDisabler sliderDisabler = this.sliderDisabler;
		sliderDisabler.SliderDeselected = (Action)Delegate.Combine(sliderDisabler.SliderDeselected, new Action(this.OnSliderDeselected));
		SliderDisabler sliderDisabler2 = this.sliderDisabler;
		sliderDisabler2.SliderSubmitted = (Action)Delegate.Combine(sliderDisabler2.SliderSubmitted, new Action(this.OnSliderSubmitted));
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		SliderDisabler sliderDisabler = this.sliderDisabler;
		sliderDisabler.SliderDeselected = (Action)Delegate.Remove(sliderDisabler.SliderDeselected, new Action(this.OnSliderDeselected));
		SliderDisabler sliderDisabler2 = this.sliderDisabler;
		sliderDisabler2.SliderSubmitted = (Action)Delegate.Remove(sliderDisabler2.SliderSubmitted, new Action(this.OnSliderSubmitted));
	}

	public void ForceRefresh()
	{
		if (this.retrieveSelectedValue)
		{
			this.RefreshSlider();
			ApplicationEvents.Instance.TriggerSettingChanged(this.settingType);
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.RefreshInteractionState();
	}

	private void OnSliderDeselected()
	{
		this.sliderFocusButton.interactable = true;
		Action<SliderSettingInput, bool> sliderFocusChanged = this.SliderFocusChanged;
		if (sliderFocusChanged == null)
		{
			return;
		}
		sliderFocusChanged(this, false);
	}

	public void ForceSliderDeselect()
	{
		this.slider.interactable = false;
		EventSystem.current.SetSelectedGameObject(base.gameObject);
		this.sliderFocusButton.Select();
	}

	private void OnSliderSubmitted()
	{
		this.slider.interactable = false;
		EventSystem.current.SetSelectedGameObject(base.gameObject);
		this.sliderFocusButton.Select();
		Action<SliderSettingInput, bool> sliderFocusChanged = this.SliderFocusChanged;
		if (sliderFocusChanged == null)
		{
			return;
		}
		sliderFocusChanged(this, false);
	}

	private void RefreshInteractionState()
	{
		bool flag = !GameManager.Instance.Input.IsUsingController;
		this.slider.interactable = flag;
		this.sliderFocusButton.interactable = !flag;
		this.uiSelectable.enabled = !flag;
	}

	void ISubmitHandler.OnSubmit(BaseEventData eventData)
	{
		this.slider.interactable = !this.slider.interactable;
		this.sliderFocusButton.interactable = !this.slider.interactable;
		if (this.slider.interactable)
		{
			EventSystem.current.SetSelectedGameObject(this.slider.gameObject);
			this.slider.Select();
			Action<SliderSettingInput, bool> sliderFocusChanged = this.SliderFocusChanged;
			if (sliderFocusChanged == null)
			{
				return;
			}
			sliderFocusChanged(this, true);
		}
	}

	private void Awake()
	{
		LocalizedString localizedString = this.localizedString;
		LocalizedString localizedString2 = this.tooltipDescriptionString;
		if (this.SKUSpecificLocalizedTitleString)
		{
			LocalizedString stringOverride = this.SKUSpecificLocalizedTitleString.GetStringOverride();
			if (stringOverride != null)
			{
				localizedString = stringOverride;
			}
		}
		if (this.SKUSpecificLocalizedDescriptionString)
		{
			LocalizedString stringOverride2 = this.SKUSpecificLocalizedDescriptionString.GetStringOverride();
			if (stringOverride2 != null)
			{
				localizedString2 = stringOverride2;
			}
		}
		this.localizedStringField.StringReference = localizedString;
		if (localizedString2.IsEmpty)
		{
			this.textTooltipRequester.enabled = false;
		}
		else
		{
			this.textTooltipRequester.LocalizedTitleKey = localizedString;
			this.textTooltipRequester.LocalizedDescriptionKey = localizedString2;
			this.textTooltipRequester.enabled = true;
		}
		if (this.retrieveSelectedValue)
		{
			this.RefreshSlider();
		}
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
	}

	private void RefreshSlider()
	{
		float num = 0.5f;
		SettingType settingType = this.settingType;
		switch (settingType)
		{
		case SettingType.CAMERA_SENSITIVITY_X:
			num = GameManager.Instance.SettingsSaveData.cameraSensitivityX;
			break;
		case SettingType.CAMERA_SENSITIVITY_Y:
			num = GameManager.Instance.SettingsSaveData.cameraSensitivityY;
			break;
		case SettingType.CAMERA_FOLLOW_MODE:
		case (SettingType)8:
			break;
		case SettingType.CAMERA_SHAKE_AMOUNT:
			num = GameManager.Instance.SettingsSaveData.cameraShakeAmount;
			break;
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_X:
			num = GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityX;
			break;
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_Y:
			num = GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityY;
			break;
		default:
			if (settingType != SettingType.TURNING_DEADZONE_X)
			{
				if (settingType == SettingType.MOTION_SICKNESS_AMOUNT)
				{
					num = GameManager.Instance.SettingsSaveData.motionSicknessAmount;
				}
			}
			else
			{
				num = GameManager.Instance.SettingsSaveData.turningDeadzoneX;
			}
			break;
		}
		this.slider.SetValueWithoutNotify(num);
	}

	private void OnValueChanged(float val)
	{
		SettingType settingType = this.settingType;
		switch (settingType)
		{
		case SettingType.CAMERA_SENSITIVITY_X:
			GameManager.Instance.SettingsSaveData.cameraSensitivityX = val;
			break;
		case SettingType.CAMERA_SENSITIVITY_Y:
			GameManager.Instance.SettingsSaveData.cameraSensitivityY = val;
			break;
		case SettingType.CAMERA_FOLLOW_MODE:
		case (SettingType)8:
			break;
		case SettingType.CAMERA_SHAKE_AMOUNT:
			GameManager.Instance.SettingsSaveData.cameraShakeAmount = val;
			break;
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_X:
			GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityX = val;
			break;
		case SettingType.SPYGLASS_CAMERA_SENSITIVITY_Y:
			GameManager.Instance.SettingsSaveData.spyglassCameraSensitivityY = val;
			break;
		default:
			if (settingType != SettingType.TURNING_DEADZONE_X)
			{
				if (settingType == SettingType.MOTION_SICKNESS_AMOUNT)
				{
					GameManager.Instance.SettingsSaveData.motionSicknessAmount = val;
				}
			}
			else
			{
				GameManager.Instance.SettingsSaveData.turningDeadzoneX = val;
			}
			break;
		}
		ApplicationEvents.Instance.TriggerSettingChanged(this.settingType);
	}

	[SerializeField]
	private SettingType settingType;

	[SerializeField]
	private LocalizedString localizedString;

	[SerializeField]
	private LocalizedString tooltipDescriptionString;

	[SerializeField]
	private LocalizeStringEvent localizedStringField;

	[SerializeField]
	private SKUSpecificLocalizedString SKUSpecificLocalizedTitleString;

	[SerializeField]
	private SKUSpecificLocalizedString SKUSpecificLocalizedDescriptionString;

	[SerializeField]
	private TextTooltipRequester textTooltipRequester;

	[SerializeField]
	private UISelectable uiSelectable;

	[SerializeField]
	private Slider slider;

	[SerializeField]
	private Button sliderFocusButton;

	[SerializeField]
	private SliderDisabler sliderDisabler;

	[SerializeField]
	public Action<SliderSettingInput, bool> SliderFocusChanged;

	[SerializeField]
	public bool retrieveSelectedValue = true;
}
