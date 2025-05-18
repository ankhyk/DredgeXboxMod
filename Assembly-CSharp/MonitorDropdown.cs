using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MonitorDropdown : MonoBehaviour
{
	private void Awake()
	{
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDisplaySelected));
	}

	private void OnEnable()
	{
		this.RefreshDropdown();
		ApplicationEvents.Instance.OnMonitorChanged += this.RefreshDropdown;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnMonitorChanged -= this.RefreshDropdown;
	}

	private void RefreshDropdown()
	{
		Screen.GetDisplayLayout(this.displays);
		DisplayInfo mainWindowDisplayInfo = Screen.mainWindowDisplayInfo;
		int num = 0;
		List<string> list = new List<string>();
		for (int i = 0; i < this.displays.Count; i++)
		{
			DisplayInfo displayInfo = this.displays[i];
			string text = string.Format("{0} - {1}x{2}", i + 1, displayInfo.width, displayInfo.height);
			list.Add(text);
			if (displayInfo.Equals(mainWindowDisplayInfo))
			{
				num = i;
			}
		}
		this.dropdown.ClearOptions();
		this.dropdown.AddOptions(list);
		this.dropdown.SetValueWithoutNotify(num);
		this.dropdownSettingInput.CurrentIndex = num;
	}

	public void OnDisplaySelected(int index)
	{
		if (!this.movingWindowInProgress)
		{
			base.StartCoroutine(this.MoveToDisplay(index));
		}
	}

	private IEnumerator MoveToDisplay(int index)
	{
		this.movingWindowInProgress = true;
		try
		{
			DisplayInfo displayInfo = this.displays[index];
			if (Screen.fullScreenMode == FullScreenMode.Windowed)
			{
				Screen.SetResolution(displayInfo.width, displayInfo.height, Screen.fullScreenMode);
			}
			Vector2Int vector2Int = new Vector2Int(0, 0);
			AsyncOperation asyncOperation = Screen.MoveMainWindowTo(in displayInfo, vector2Int);
			yield return asyncOperation;
		}
		finally
		{
			this.movingWindowInProgress = false;
			GameManager.Instance.SettingsSaveData.DisplayIndex = index;
			GameManager.Instance.SaveManager.SaveSettings();
			ApplicationEvents.Instance.TriggerMonitorChanged();
		}
		yield break;
		yield break;
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TMP_Dropdown dropdown;

	private List<DisplayInfo> displays = new List<DisplayInfo>();

	private bool movingWindowInProgress;
}
