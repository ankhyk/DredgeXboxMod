using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TitleThemeSelectorDropdown : SerializedMonoBehaviour, ISettingsRefreshable
{
	private void Awake()
	{
		this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnOptionSelected));
	}

	public void ForceRefresh()
	{
		this.Refresh();
	}

	private void OnEnable()
	{
		this.Refresh();
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Combine(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
		GameManager.Instance.EntitlementManager.OnDLCRuntimeInstall.AddListener(new UnityAction<Entitlement>(this.OnDLCRuntimeInstall));
		base.StartCoroutine(this.DoDelayedRefresh());
	}

	private void OnDisable()
	{
		SettingsUIComponentEventNotifier settingsUIComponentEventNotifier = this.dropdownEventNotifier;
		settingsUIComponentEventNotifier.OnComponentSubmitted = (Action)Delegate.Remove(settingsUIComponentEventNotifier.OnComponentSubmitted, new Action(this.OnComponentSubmitted));
		GameManager.Instance.EntitlementManager.OnDLCRuntimeInstall.RemoveListener(new UnityAction<Entitlement>(this.OnDLCRuntimeInstall));
	}

	private IEnumerator DoDelayedRefresh()
	{
		yield return new WaitForEndOfFrame();
		this.Refresh();
		yield break;
	}

	private void OnDLCRuntimeInstall(Entitlement e)
	{
		this.Refresh();
	}

	private void Refresh()
	{
		this.hasDLC1 = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1);
		this.hasDLC2 = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2);
		this.hasDeluxe = GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DELUXE);
		int num = 0;
		int titleTheme = GameManager.Instance.SettingsSaveData.titleTheme;
		if (!GameManager.Instance.SettingsSaveData.hasEverTouchedTitleTheme)
		{
			if (this.hasDLC1)
			{
				num = 1;
			}
			if (this.hasDLC2)
			{
				num = 2;
			}
			if (this.hasDLC1 && this.hasDLC2 && this.hasDeluxe)
			{
				num = 3;
			}
		}
		else
		{
			num = titleTheme;
		}
		this.dropdown.SetValueWithoutNotify(num);
		this.dropdownSettingInput.CurrentIndex = num;
		this.dropdownSettingInput.RefreshDropdownStrings();
		int num2 = 4;
		this.mainBoxRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)num2 * this.heightPerOption);
		this.mainBoxRect.ForceUpdateRectTransforms();
	}

	public void OnOptionSelected(int index)
	{
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.TITLE_THEME);
	}

	private void OnComponentSubmitted()
	{
		List<DropdownItem> list = base.transform.GetComponentsInChildren<DropdownItem>().ToList<DropdownItem>();
		for (int i = 0; i < list.Count; i++)
		{
			Toggle component = list[i].GetComponent<Toggle>();
			if (component)
			{
				bool flag = false;
				switch (i)
				{
				case 1:
					flag = !this.hasDLC1;
					break;
				case 2:
					flag = !this.hasDLC2;
					break;
				case 3:
					flag = !this.hasDLC1 || !this.hasDLC2 || !this.hasDeluxe;
					break;
				}
				if (flag)
				{
					component.interactable = false;
				}
				list[i].image.enabled = flag;
			}
		}
	}

	[SerializeField]
	private DropdownSettingInput dropdownSettingInput;

	[SerializeField]
	private TMP_Dropdown dropdown;

	[SerializeField]
	private RectTransform mainBoxRect;

	[SerializeField]
	private SettingsUIComponentEventNotifier dropdownEventNotifier;

	[SerializeField]
	private TextMeshProUGUI selectedValueTextField;

	[SerializeField]
	private float heightPerOption;

	private bool hasDLC1;

	private bool hasDLC2;

	private bool hasDeluxe;
}
