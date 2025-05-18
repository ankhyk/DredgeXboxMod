using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TooltipSectionUpgradeCost : TooltipSection<IUpgradeCost>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		throw new NotImplementedException();
	}

	public override void Init(IUpgradeCost upgradeCost)
	{
		SerializableGrid grid = GameManager.Instance.SaveData.GetGridByKey(upgradeCost.GetGridKey());
		this.isLayedOut = false;
		foreach (object obj in this.upgradeCostContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		upgradeCost.GetItemCost().ForEach(delegate(ItemCountCondition uc)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.upgradeCostIconPrefab, this.upgradeCostContainer).GetComponent<TooltipUpgradeCostIcon>().Init(uc.item, uc.CountItems(grid), uc.count);
		});
		string text = upgradeCost.GetMonetaryCost().ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		this.monetaryCostText.text = "$" + text;
		this.monetaryCostText.color = ((GameManager.Instance.SaveData.Funds >= upgradeCost.GetMonetaryCost()) ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
		this.isLayedOut = true;
	}

	[SerializeField]
	private List<TooltipUpgradeCostIcon> upgradeCostIcons = new List<TooltipUpgradeCostIcon>();

	[SerializeField]
	private TextMeshProUGUI monetaryCostText;

	[SerializeField]
	private Transform upgradeCostContainer;

	[SerializeField]
	private GameObject upgradeCostIconPrefab;
}
