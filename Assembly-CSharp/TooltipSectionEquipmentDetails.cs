using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class TooltipSectionEquipmentDetails : TooltipSection<SpatialItemInstance>
{
	public override void Init<T>(T spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.spatialItemInstance = spatialItemInstance;
		this.RefreshUI();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
	}

	private void OnPlayerDamageChanged()
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.isLayedOut = false;
		string text = GameManager.Instance.ItemManager.GetInstallTimeForItem(this.spatialItemInstance.GetItemData<SpatialItemData>()).ToString(".#");
		string text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
		this.installTimeTextField.text = string.Concat(new string[] { "<color=#", text2, ">", text, "h</color>" });
		this.installTimeContainer.SetActive(this.spatialItemInstance.GetItemData<SpatialItemData>().itemSubtype != ItemSubtype.GADGET);
		bool flag = false;
		if (this.spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY)
		{
			flag = this.spatialItemInstance.durability <= 0f;
		}
		else if (this.spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.OPERATION)
		{
			flag = this.spatialItemInstance.GetIsOnDamagedCell();
		}
		this.operationalStatusLocalizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, flag ? "equipment-status.damaged" : "equipment-status.operational");
		this.operationalStatusTextField.color = (flag ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL));
		this.isLayedOut = true;
	}

	[SerializeField]
	private GameObject installTimeContainer;

	[SerializeField]
	private TextMeshProUGUI installTimeTextField;

	[SerializeField]
	private TextMeshProUGUI operationalStatusTextField;

	[SerializeField]
	private LocalizeStringEvent operationalStatusLocalizedString;

	private SpatialItemInstance spatialItemInstance;
}
