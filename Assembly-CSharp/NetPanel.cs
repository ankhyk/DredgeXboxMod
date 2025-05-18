using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class NetPanel : GridPanel
{
	public override void ShowStart()
	{
		this.gridUI.LinkedGridKey = GameManager.Instance.SaveData.GetCurrentTrawlNetGridKey();
		base.ShowStart();
		this.RefreshUI();
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
	}

	public override void HideStart()
	{
		base.HideStart();
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
	}

	private void OnItemsRepaired()
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
		if (spatialItemInstance != null)
		{
			DeployableItemData itemData = spatialItemInstance.GetItemData<DeployableItemData>();
			this.localizedTitleTextField.StringReference = itemData.itemNameKey;
			this.localizedTitleTextField.RefreshString();
			this.durabilityBar.fillAmount = spatialItemInstance.durability / itemData.MaxDurabilityDays;
			GameManager.Instance.LanguageManager.FormatTimeStringForDurability(spatialItemInstance.durability, itemData.MaxDurabilityDays, this.localizedDurabilityTextField, "label.deployable.durability-value");
			return;
		}
		this.localizedDurabilityTextField.gameObject.SetActive(false);
		this.localizedTitleTextField.gameObject.SetActive(false);
	}

	[SerializeField]
	private LocalizeStringEvent localizedTitleTextField;

	[SerializeField]
	private LocalizeStringEvent localizedDurabilityTextField;

	[SerializeField]
	private Image durabilityBar;

	[SerializeField]
	private GridUI gridUI;
}
