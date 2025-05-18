using System;
using Sirenix.OdinInspector;

public abstract class SettingResponder : SerializedMonoBehaviour
{
	private void Awake()
	{
		this.Refresh();
		ApplicationEvents.Instance.OnSaveManagerInitialized += this.Refresh;
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	protected abstract void OnSettingChanged(SettingType settingType);

	protected abstract void Refresh();
}
