using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class HarvestableTypeTagUI : SerializedMonoBehaviour
{
	public void Init(HarvestableType harvestableType, bool isAdvancedType)
	{
		if (this.harvestTypeTagConfig.stringLookup.ContainsKey(harvestableType))
		{
			this.localizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, this.harvestTypeTagConfig.stringLookup[harvestableType]);
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[HarvestableTypeTagUI] Init({0}) couldn't find string in config.", harvestableType));
		}
		if (this.harvestTypeTagConfig.colorLookup.ContainsKey(harvestableType))
		{
			this.tagImage.color = this.harvestTypeTagConfig.colorLookup[harvestableType];
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[HarvestableTypeTagUI] Init({0}) couldn't find color in config.", harvestableType));
		}
		if (this.harvestTypeTagConfig.textColorLookup.ContainsKey(harvestableType))
		{
			this.textField.color = this.harvestTypeTagConfig.textColorLookup[harvestableType];
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[HarvestableTypeTagUI] Init({0}) couldn't find text color in config.", harvestableType));
		}
		this.SetAdvancedState(isAdvancedType);
	}

	public void SetAdvancedState(bool isAdvancedType)
	{
		this.advancedTypeIcon.SetActive(isAdvancedType);
	}

	[SerializeField]
	private LocalizeStringEvent localizedString;

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private Image tagImage;

	[SerializeField]
	private GameObject advancedTypeIcon;

	[SerializeField]
	private HarvestTypeTagConfig harvestTypeTagConfig;
}
