using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TooltipSectionDurabilityDetails : TooltipSection<DurableItemData>
{
	public override void Init<T>(T durableItemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.tooltipMode = tooltipMode;
		this.spatialItemInstance = spatialItemInstance;
		if (spatialItemInstance == null)
		{
			this.Init(durableItemData, durableItemData.MaxDurabilityDays);
			return;
		}
		this.Init(durableItemData, spatialItemInstance.durability);
	}

	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.Init(itemData, itemData.MaxDurabilityDays);
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
	}

	private void OnItemsRepaired()
	{
		this.Init<DurableItemData>(this.durableItemData, this.spatialItemInstance, TooltipUI.TooltipMode.HOVER);
		this.RefreshUI();
	}

	private void Init(DurableItemData durableItemData, float currentDurability)
	{
		this.durableItemData = durableItemData;
		this.currentDurability = currentDurability;
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.isLayedOut = false;
		if (this.tooltipMode == TooltipUI.TooltipMode.MYSTERY)
		{
			this.durabilityContainer.SetActive(false);
		}
		else
		{
			if (this.durableItemData.displayDurabilityAsPercentage)
			{
				this.localizedDurabilityLabel.StringReference = this.durabilityAsPercentLabelString;
				this.localizedDurabilityTextField.enabled = false;
				int num = Mathf.RoundToInt(this.currentDurability / this.durableItemData.MaxDurabilityDays * 100f);
				this.durabilityTextField.text = string.Format("{0} %", num);
			}
			else
			{
				this.localizedDurabilityLabel.StringReference = this.durabilityAsTimeLabelString;
				GameManager.Instance.LanguageManager.FormatTimeStringForDurability(this.currentDurability, this.durableItemData.MaxDurabilityDays, this.localizedDurabilityTextField, "tooltip.deployable.durability-value");
				this.localizedDurabilityTextField.enabled = true;
			}
			this.durabilityBar.fillAmount = this.currentDurability / this.durableItemData.MaxDurabilityDays;
			this.durabilityContainer.SetActive(true);
		}
		this.isLayedOut = true;
	}

	[SerializeField]
	private LocalizedString durabilityAsTimeLabelString;

	[SerializeField]
	private LocalizedString durabilityAsPercentLabelString;

	[SerializeField]
	private LocalizeStringEvent localizedDurabilityLabel;

	[SerializeField]
	private LocalizeStringEvent localizedDurabilityTextField;

	[SerializeField]
	private TextMeshProUGUI durabilityTextField;

	[SerializeField]
	private Image durabilityBar;

	[SerializeField]
	private GameObject durabilityContainer;

	private SpatialItemInstance spatialItemInstance;

	private DurableItemData durableItemData;

	private float currentDurability;

	private TooltipUI.TooltipMode tooltipMode;
}
