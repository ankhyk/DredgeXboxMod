using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

public abstract class UpgradeData : SerializedScriptableObject, IUpgradeCost
{
	public LocalizedString DescriptionKey
	{
		get
		{
			return this.descriptionKey;
		}
	}

	public LocalizedString TitleKey
	{
		get
		{
			return this.titleKey;
		}
	}

	public decimal MonetaryCost
	{
		get
		{
			return this.monetaryCost;
		}
	}

	public bool AvailableInDemo
	{
		get
		{
			return this.availableInDemo;
		}
	}

	public QuestGridConfig GridConfig
	{
		get
		{
			return this.gridConfig;
		}
	}

	public abstract int GetNewCellCount();

	decimal IUpgradeCost.GetMonetaryCost()
	{
		return this.MonetaryCost;
	}

	List<ItemCountCondition> IUpgradeCost.GetItemCost()
	{
		return this.gridConfig.completeConditions.OfType<ItemCountCondition>().ToList<ItemCountCondition>();
	}

	GridKey IUpgradeCost.GetGridKey()
	{
		return this.gridConfig.gridKey;
	}

	public string id;

	public int tier;

	public Sprite sprite;

	[SerializeField]
	private QuestGridConfig gridConfig;

	[SerializeField]
	private LocalizedString titleKey;

	[SerializeField]
	private LocalizedString descriptionKey;

	[SerializeField]
	private decimal monetaryCost;

	[SerializeField]
	private bool availableInDemo;

	public List<UpgradeCost> upgradeCost = new List<UpgradeCost>();

	[SerializeField]
	public List<ItemCountCondition> itemCost = new List<ItemCountCondition>();

	public List<UpgradeData> prerequisiteUpgrades = new List<UpgradeData>();
}
