using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TooltipSectionDeployableDetails : TooltipSection<DeployableItemData>
{
	public override void Init<T>(T deployableItemData, SpatialItemInstance spatialItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.tooltipMode = tooltipMode;
		this.spatialItemInstance = spatialItemInstance;
		if (spatialItemInstance == null)
		{
			this.Init(deployableItemData, deployableItemData.MaxDurabilityDays);
			return;
		}
		this.Init(deployableItemData, spatialItemInstance.durability);
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
		this.Init<DeployableItemData>(this.deployableItemData, this.spatialItemInstance, TooltipUI.TooltipMode.HOVER);
		this.RefreshUI();
	}

	private void Init(DeployableItemData deployableItemData, float currentDurability)
	{
		this.deployableItemData = deployableItemData;
		this.currentDurability = currentDurability;
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.isLayedOut = false;
		if (this.tooltipMode == TooltipUI.TooltipMode.MYSTERY)
		{
			this.expectedCatchContainer.SetActive(false);
			this.capacityContainer.SetActive(false);
			this.durabilityContainer.SetActive(false);
			this.aberrationBonusContainer.SetActive(false);
		}
		else if (this.deployableItemData.id == "tir-net1")
		{
			GameManager.Instance.LanguageManager.FormatTimeStringForDurability(this.currentDurability, this.deployableItemData.MaxDurabilityDays, this.localizedDurabilityTextField, "tooltip.deployable.durability-value");
			this.localizedDurabilityTextField.enabled = true;
			this.durabilityBar.fillAmount = this.currentDurability / this.deployableItemData.MaxDurabilityDays;
			this.durabilityContainer.SetActive(true);
			this.expectedCatchContainer.SetActive(false);
			this.capacityContainer.SetActive(false);
			this.aberrationBonusContainer.SetActive(false);
		}
		else
		{
			GameManager.Instance.LanguageManager.FormatTimeStringForDurability(this.currentDurability, this.deployableItemData.MaxDurabilityDays, this.localizedDurabilityTextField, "tooltip.deployable.durability-value");
			this.localizedDurabilityTextField.enabled = true;
			this.durabilityBar.fillAmount = this.currentDurability / this.deployableItemData.MaxDurabilityDays;
			float num = 1f / this.deployableItemData.TimeBetweenCatchRolls * this.deployableItemData.CatchRate;
			string text;
			if (num % 1f > 0.1f)
			{
				text = string.Format("{0} - {1}", Mathf.FloorToInt(num), Mathf.CeilToInt(num));
			}
			else
			{
				text = string.Format("~{0}", Mathf.RoundToInt(num));
			}
			this.localizedExpectedCatchTextField.StringReference.Arguments = new string[] { text };
			this.localizedExpectedCatchTextField.enabled = true;
			this.expectedCatchContainer.SetActive(true);
			this.capacityTextField.text = string.Format("{0}x{1}", this.deployableItemData.GridConfig.columns, this.deployableItemData.GridConfig.rows);
			if (this.deployableItemData.aberrationBonus > 0f)
			{
				this.aberrationBonusTextField.text = "+" + (this.deployableItemData.aberrationBonus * 100f).ToString(".#") + "%";
				this.aberrationBonusContainer.SetActive(true);
			}
			else
			{
				this.aberrationBonusContainer.SetActive(false);
			}
			this.expectedCatchContainer.SetActive(true);
			this.capacityContainer.SetActive(true);
			this.durabilityContainer.SetActive(true);
		}
		HarvestableType[] harvestableTypes = this.deployableItemData.harvestableTypes;
		if (harvestableTypes.Length == 0)
		{
			this.harvestTypesContainer.SetActive(false);
		}
		else
		{
			this.harvestTypesContainer.SetActive(true);
			this.tags.ForEach(delegate(HarvestableTypeTagUI t)
			{
				t.gameObject.SetActive(false);
			});
			int num2 = 0;
			while (num2 < harvestableTypes.Length && num2 < this.tags.Count)
			{
				HarvestableType harvestableType = harvestableTypes[num2];
				this.tags[num2].Init(harvestableType, false);
				this.tags[num2].gameObject.SetActive(true);
				num2++;
			}
		}
		float height = this.heightPerTagRow;
		if (harvestableTypes.Length > 2)
		{
			height = this.heightPerTagRow * 2f;
		}
		this.containersToAdjustHeightByRow.ForEach(delegate(RectTransform rt)
		{
			Vector2 sizeDelta = rt.sizeDelta;
			sizeDelta.y = height;
			rt.sizeDelta = sizeDelta;
		});
		this.isLayedOut = true;
	}

	[SerializeField]
	private List<RectTransform> containersToAdjustHeightByRow;

	[SerializeField]
	private float heightPerTagRow;

	[SerializeField]
	private LocalizeStringEvent localizedDurabilityTextField;

	[SerializeField]
	private LocalizeStringEvent localizedExpectedCatchTextField;

	[SerializeField]
	private Image durabilityBar;

	[SerializeField]
	private TextMeshProUGUI capacityTextField;

	[SerializeField]
	private TextMeshProUGUI aberrationBonusTextField;

	[SerializeField]
	private GameObject harvestTypesContainer;

	[SerializeField]
	private GameObject expectedCatchContainer;

	[SerializeField]
	private GameObject capacityContainer;

	[SerializeField]
	private GameObject durabilityContainer;

	[SerializeField]
	private GameObject aberrationBonusContainer;

	[SerializeField]
	private List<HarvestableTypeTagUI> tags;

	private SpatialItemInstance spatialItemInstance;

	private DeployableItemData deployableItemData;

	private float currentDurability;

	private TooltipUI.TooltipMode tooltipMode;
}
