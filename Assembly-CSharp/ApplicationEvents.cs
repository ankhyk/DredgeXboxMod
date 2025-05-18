using System;
using InControl;
using UnityEngine;
using UnityEngine.Localization;

[DefaultExecutionOrder(-2000)]
public class ApplicationEvents : MonoBehaviour
{
	private void Awake()
	{
		ApplicationEvents.Instance = this;
	}

	public event Action OnBuildInfoChanged;

	public void TriggerBuildInfoChanged()
	{
		Action onBuildInfoChanged = this.OnBuildInfoChanged;
		if (onBuildInfoChanged == null)
		{
			return;
		}
		onBuildInfoChanged();
	}

	public event Action OnSaveManagerInitialized;

	public void TriggerSaveManagerInitialized()
	{
		Action onSaveManagerInitialized = this.OnSaveManagerInitialized;
		if (onSaveManagerInitialized == null)
		{
			return;
		}
		onSaveManagerInitialized();
	}

	public event Action<SettingType> OnSettingChanged;

	public void TriggerSettingChanged(SettingType settingType)
	{
		Action<SettingType> onSettingChanged = this.OnSettingChanged;
		if (onSettingChanged == null)
		{
			return;
		}
		onSettingChanged(settingType);
	}

	public event Action<Locale> OnLocaleChanged;

	public void TriggerLocaleChange(Locale locale)
	{
		Action<Locale> onLocaleChanged = this.OnLocaleChanged;
		if (onLocaleChanged == null)
		{
			return;
		}
		onLocaleChanged(locale);
	}

	public event Action<bool> OnToggleSettings;

	public void TriggerToggleSettings(bool show)
	{
		Action<bool> onToggleSettings = this.OnToggleSettings;
		if (onToggleSettings == null)
		{
			return;
		}
		onToggleSettings(show);
	}

	public event Action<bool> OnDialogToggled;

	public void TriggerDialogToggled(bool visible)
	{
		Action<bool> onDialogToggled = this.OnDialogToggled;
		if (onDialogToggled == null)
		{
			return;
		}
		onDialogToggled(visible);
	}

	public event Action<ActionLayer> OnInputActionLayerChanged;

	public void TriggerInputActionLayerChanged(ActionLayer newActionLayer)
	{
		Action<ActionLayer> onInputActionLayerChanged = this.OnInputActionLayerChanged;
		if (onInputActionLayerChanged == null)
		{
			return;
		}
		onInputActionLayerChanged(newActionLayer);
	}

	public event Action<bool> OnUIDebugToggled;

	public void TriggerUIDebugToggled(bool visible)
	{
		Action<bool> onUIDebugToggled = this.OnUIDebugToggled;
		if (onUIDebugToggled == null)
		{
			return;
		}
		onUIDebugToggled(visible);
	}

	public event Action OnTitleClosed;

	public void TriggerTitleClose()
	{
		Action onTitleClosed = this.OnTitleClosed;
		if (onTitleClosed == null)
		{
			return;
		}
		onTitleClosed();
	}

	public event Action OnGameLoaded;

	public void TriggerGameLoaded()
	{
		Action onGameLoaded = this.OnGameLoaded;
		if (onGameLoaded == null)
		{
			return;
		}
		onGameLoaded();
	}

	public event Action OnGameUnloaded;

	public void TriggerGameUnloaded()
	{
		Action onGameUnloaded = this.OnGameUnloaded;
		if (onGameUnloaded == null)
		{
			return;
		}
		onGameUnloaded();
	}

	public event Action OnGameStartable;

	public void TriggerGameStartable()
	{
		Action onGameStartable = this.OnGameStartable;
		if (onGameStartable == null)
		{
			return;
		}
		onGameStartable();
	}

	public event Action<bool> OnSettingsConfirmationWindowToggled;

	public void TriggerSettingsConfirmationToggled(bool open)
	{
		Action<bool> onSettingsConfirmationWindowToggled = this.OnSettingsConfirmationWindowToggled;
		if (onSettingsConfirmationWindowToggled == null)
		{
			return;
		}
		onSettingsConfirmationWindowToggled(open);
	}

	public event Action<UIWindowType, bool> OnUIWindowToggled;

	public void TriggerUIWindowToggled(UIWindowType windowType, bool show)
	{
		Action<UIWindowType, bool> onUIWindowToggled = this.OnUIWindowToggled;
		if (onUIWindowToggled == null)
		{
			return;
		}
		onUIWindowToggled(windowType, show);
	}

	public event Action<TooltipRequester> OnUITooltipRequested;

	public void TriggerUITooltipRequested(TooltipRequester tooltipRequester)
	{
		Action<TooltipRequester> onUITooltipRequested = this.OnUITooltipRequested;
		if (onUITooltipRequested == null)
		{
			return;
		}
		onUITooltipRequested(tooltipRequester);
	}

	public event Action<TooltipRequester, bool> OnUITooltipClearRequested;

	public void TriggerUITooltipClearRequested(TooltipRequester tooltipRequester, bool force = false)
	{
		Action<TooltipRequester, bool> onUITooltipClearRequested = this.OnUITooltipClearRequested;
		if (onUITooltipClearRequested == null)
		{
			return;
		}
		onUITooltipClearRequested(tooltipRequester, force);
	}

	public event Action<PlayerAction> OnPlayerActionBindingChanged;

	public void TriggerPlayerActionBindingChanged(PlayerAction playerAction)
	{
		Action<PlayerAction> onPlayerActionBindingChanged = this.OnPlayerActionBindingChanged;
		if (onPlayerActionBindingChanged == null)
		{
			return;
		}
		onPlayerActionBindingChanged(playerAction);
	}

	public event Action<PlayerAction> OnPlayerActionBindingEnded;

	public void TriggerPlayerActionBindingEnded(PlayerAction playerAction)
	{
		Action<PlayerAction> onPlayerActionBindingEnded = this.OnPlayerActionBindingEnded;
		if (onPlayerActionBindingEnded == null)
		{
			return;
		}
		onPlayerActionBindingEnded(playerAction);
	}

	public event Action OnMonitorChanged;

	public void TriggerMonitorChanged()
	{
		Action onMonitorChanged = this.OnMonitorChanged;
		if (onMonitorChanged == null)
		{
			return;
		}
		onMonitorChanged();
	}

	public event Action<bool> OnCreditsToggled;

	public void TriggerCreditsToggled(bool showing)
	{
		Action<bool> onCreditsToggled = this.OnCreditsToggled;
		if (onCreditsToggled == null)
		{
			return;
		}
		onCreditsToggled(showing);
	}

	public event Action<bool> OnDemoEndToggled;

	public void TriggerDemoEndToggled(bool showing)
	{
		Action<bool> onDemoEndToggled = this.OnDemoEndToggled;
		if (onDemoEndToggled == null)
		{
			return;
		}
		onDemoEndToggled(showing);
	}

	public event Action<bool> OnFeedbackFormToggled;

	public void TriggerFeedbackFormToggled(bool showing)
	{
		Action<bool> onFeedbackFormToggled = this.OnFeedbackFormToggled;
		if (onFeedbackFormToggled == null)
		{
			return;
		}
		onFeedbackFormToggled(showing);
	}

	public event Action<bool> OnSliderFocusToggled;

	public void TriggerSliderFocusToggled(bool hasFocus)
	{
		Action<bool> onSliderFocusToggled = this.OnSliderFocusToggled;
		if (onSliderFocusToggled == null)
		{
			return;
		}
		onSliderFocusToggled(hasFocus);
	}

	public static ApplicationEvents Instance;
}
