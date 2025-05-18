using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSettingResponder : SerializedMonoBehaviour
{
	public void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.RefreshColors();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.COLOR_NEUTRAL || settingType == SettingType.COLOR_EMPHASIS || settingType == SettingType.COLOR_POSITIVE || settingType == SettingType.COLOR_NEGATIVE || settingType == SettingType.COLOR_CRITICAL || settingType == SettingType.COLOR_WARNING || settingType == SettingType.COLOR_VALUABLE || settingType == SettingType.COLOR_DISABLED)
		{
			this.RefreshColors(settingType);
		}
	}

	private void RefreshColors(SettingType settingType)
	{
		DredgeColorTypeEnum colorTypeEnumFromSettingType = GameManager.Instance.LanguageManager.GetColorTypeEnumFromSettingType(settingType);
		Color c = GameManager.Instance.LanguageManager.GetColor(colorTypeEnumFromSettingType);
		if (this.images.ContainsKey(colorTypeEnumFromSettingType))
		{
			this.images[colorTypeEnumFromSettingType].ForEach(delegate(Image i)
			{
				i.color = c;
			});
		}
		if (this.text.ContainsKey(colorTypeEnumFromSettingType))
		{
			this.text[colorTypeEnumFromSettingType].ForEach(delegate(TextMeshProUGUI t)
			{
				t.color = c;
			});
		}
	}

	private void RefreshColors()
	{
		this.images.Keys.ToList<DredgeColorTypeEnum>().ForEach(delegate(DredgeColorTypeEnum e)
		{
			Color c = GameManager.Instance.LanguageManager.GetColor(e);
			this.images[e].ForEach(delegate(Image i)
			{
				i.color = c;
			});
		});
		this.text.Keys.ToList<DredgeColorTypeEnum>().ForEach(delegate(DredgeColorTypeEnum e)
		{
			Color c = GameManager.Instance.LanguageManager.GetColor(e);
			this.text[e].ForEach(delegate(TextMeshProUGUI i)
			{
				i.color = c;
			});
		});
	}

	[SerializeField]
	private Dictionary<DredgeColorTypeEnum, List<Image>> images = new Dictionary<DredgeColorTypeEnum, List<Image>>();

	[SerializeField]
	private Dictionary<DredgeColorTypeEnum, List<TextMeshProUGUI>> text = new Dictionary<DredgeColorTypeEnum, List<TextMeshProUGUI>>();
}
