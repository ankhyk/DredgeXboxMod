using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResolutionDropdown : MonoBehaviour
{
	private void Awake()
	{
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnResolutionSelected));
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
		this.resolutions = ResolutionHelper.GetSupportedResolutionsDescending();
		int num = 0;
		List<string> list = new List<string>();
		for (int i = 0; i < this.resolutions.Count; i++)
		{
			string text = this.resolutions[i].width.ToString() + " x " + this.resolutions[i].height.ToString();
			list.Add(text);
			if (this.resolutions[i].width == Screen.width && this.resolutions[i].height == Screen.height)
			{
				num = i;
			}
		}
		this.dropdown.ClearOptions();
		this.dropdown.AddOptions(list);
		this.dropdown.SetValueWithoutNotify(num);
		this.dropdownSettingInput.CurrentIndex = num;
	}

	public void OnResolutionSelected(int index)
	{
		Screen.SetResolution(this.resolutions[index].width, this.resolutions[index].height, Screen.fullScreen);
		GameManager.Instance.SettingsSaveData.resx = this.resolutions[index].width;
		GameManager.Instance.SettingsSaveData.resy = this.resolutions[index].height;
		GameManager.Instance.SaveManager.SaveSettings();
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TMP_Dropdown dropdown;

	private List<Resolution> resolutions;
}
