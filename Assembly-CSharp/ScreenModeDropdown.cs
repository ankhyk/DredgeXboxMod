using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScreenModeDropdown : MonoBehaviour
{
	private void Awake()
	{
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnScreenModeSelected));
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.DelayedRefresh());
	}

	private IEnumerator DelayedRefresh()
	{
		yield return new WaitForEndOfFrame();
		this.RefreshFullScreenModes();
		yield break;
	}

	private void RefreshFullScreenModes()
	{
		FullScreenMode fullScreenMode = Screen.fullScreenMode;
		int num = 0;
		for (int i = 0; i < ScreenModeDropdown.fullscreenModes.Length; i++)
		{
			if (ScreenModeDropdown.fullscreenModes[i] == fullScreenMode)
			{
				num = i;
				break;
			}
		}
		this.dropdown.SetValueWithoutNotify(num);
		this.dropdownSettingInput.CurrentIndex = num;
	}

	private void OnScreenModeSelected(int index)
	{
		FullScreenMode fullScreenMode = ScreenModeDropdown.fullscreenModes[index];
		if (Screen.fullScreenMode != fullScreenMode)
		{
			if (fullScreenMode == FullScreenMode.Windowed)
			{
				Screen.fullScreenMode = fullScreenMode;
			}
			else
			{
				DisplayInfo mainWindowDisplayInfo = Screen.mainWindowDisplayInfo;
				Screen.SetResolution(mainWindowDisplayInfo.width, mainWindowDisplayInfo.height, fullScreenMode);
			}
		}
		GameManager.Instance.SettingsSaveData.FullScreenMode = (int)fullScreenMode;
		GameManager.Instance.SaveManager.SaveSettings();
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TMP_Dropdown dropdown;

	private static readonly FullScreenMode[] fullscreenModes = new FullScreenMode[]
	{
		FullScreenMode.ExclusiveFullScreen,
		FullScreenMode.FullScreenWindow,
		FullScreenMode.Windowed
	};
}
