using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-950)]
public class PlayerStats : MonoBehaviour
{
	public int DamageThreshold { get; private set; }

	public float MovementSpeedModifier
	{
		get
		{
			return this.EquipmentMovementSpeedModifier * (1f + this.ResearchedMovementSpeedModifier) * this.OozeMovementSpeedModifier;
		}
	}

	public float EquipmentMovementSpeedModifier { get; private set; }

	public float ResearchedMovementSpeedModifier { get; private set; }

	public float OozeMovementSpeedModifier { get; set; }

	public int AttachedMonsterCount { get; private set; }

	public float FishingSpeedModifier
	{
		get
		{
			return this.EquipmentFishingSpeedModifier * (1f + this.ResearchedFishingSpeedModifier) + (float)(this.FishingSpeedGadgetModifier - 1m);
		}
	}

	public float MinigameFishingSpeedModifier
	{
		get
		{
			if (this.FishingSpeedModifier <= 1f)
			{
				return this.FishingSpeedModifier;
			}
			return Mathf.Pow(this.FishingSpeedModifier, 0.45f);
		}
	}

	public float DredgingSpeedModifier
	{
		get
		{
			return 1f * (float)this.DredgingSpeedGadgetModifier;
		}
	}

	public float EquipmentFishingSpeedModifier { get; private set; }

	public float ResearchedFishingSpeedModifier { get; private set; }

	public float TotalAberrationCatchModifier
	{
		get
		{
			return this.EquipmentAberrationCatchModifier + this.ResearchedAberrationCatchModifier;
		}
	}

	private float EquipmentAberrationCatchModifier { get; set; }

	private float ResearchedAberrationCatchModifier { get; set; }

	public float LightLumens { get; private set; }

	public float LightRange { get; private set; }

	public float SanityModifier { get; private set; }

	public float ResearchedBarteringModifier { get; private set; }

	public float ResearchedFishingSustainModifier { get; private set; }

	public float ResearchedSanityModifier { get; private set; }

	public float ResearchedEquipmentMaintenanceModifier { get; private set; }

	public decimal TurningSpeedGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.TURN_SPEED);
		}
	}

	public decimal ReverseSpeedGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.REVERSE_SPEED);
		}
	}

	public decimal DredgingSpeedGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.DREDGE_SPEED);
		}
	}

	public decimal FishingSpeedGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.FISHING_SPEED);
		}
	}

	public decimal HeatSinkGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.HEAT_SINK);
		}
	}

	public decimal TrawlCatchRateGadgetModifier
	{
		get
		{
			return this.GetGadgetModifierForType(GadgetEffect.TRAWL_CATCH_RATE);
		}
	}

	public float AttachedMonsterMovementSpeedFactor
	{
		get
		{
			return Mathf.Lerp(1f, 0f, Mathf.Clamp01((float)this.AttachedMonsterCount / this.config.NumAttachedMonstersToNullifyEngines));
		}
	}

	public HashSet<HarvestableType> HarvestableTypes { get; private set; }

	public HashSet<HarvestableType> AdvancedHarvestableTypes { get; private set; }

	public bool GetHasEquipmentForHarvestType(HarvestableType hType, bool requireAdvancedHarvestType)
	{
		if (requireAdvancedHarvestType)
		{
			return this.AdvancedHarvestableTypes.Contains(hType);
		}
		return this.HarvestableTypes.Contains(hType);
	}

	public decimal GetGadgetModifierForType(GadgetEffect effectType)
	{
		if (this.gadgetMagnitudes.ContainsKey(effectType))
		{
			return 1m + this.gadgetMagnitudes[effectType];
		}
		return 1m;
	}

	private void Awake()
	{
		this.OozeMovementSpeedModifier = 1f;
		GameManager.Instance.PlayerStats = this;
		this.config = GameManager.Instance.GameConfigData;
		this.CalculateAllStats();
	}

	public void AttachMonsterToPlayer()
	{
		int attachedMonsterCount = this.AttachedMonsterCount;
		this.AttachedMonsterCount = attachedMonsterCount + 1;
		GameEvents.Instance.TriggerMonsterAttachedToPlayer();
	}

	public void RemoveMonsterFromPlayer()
	{
		int attachedMonsterCount = this.AttachedMonsterCount;
		this.AttachedMonsterCount = attachedMonsterCount - 1;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnItemAdded += this.OnItemAdded;
		GameEvents.Instance.OnItemPickedUp += this.OnItemChanged;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemChanged;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemChanged;
		GameEvents.Instance.OnBookReadCompleted += this.OnResearchCompleted;
		GameEvents.Instance.OnEquipmentDamageChanged += this.CalculateEquipmentStats;
		this.CalculateDamageThreshold();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemAdded -= this.OnItemAdded;
		GameEvents.Instance.OnItemPickedUp -= this.OnItemChanged;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemChanged;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemChanged;
		GameEvents.Instance.OnBookReadCompleted -= this.OnResearchCompleted;
		GameEvents.Instance.OnEquipmentDamageChanged -= this.CalculateEquipmentStats;
	}

	private void OnItemAdded(SpatialItemInstance spatialItemInstance, bool belongsToPlayer)
	{
		if (spatialItemInstance.GetItemData<SpatialItemData>().itemType == ItemType.EQUIPMENT)
		{
			this.CalculateEquipmentStats(spatialItemInstance.GetItemData<SpatialItemData>());
		}
	}

	private void OnItemChanged(GridObject gridObject, bool result)
	{
		if (result)
		{
			this.OnItemChanged(gridObject);
		}
	}

	private void OnItemChanged(GridObject gridObject)
	{
		if (gridObject.state == GridObjectState.IN_INVENTORY)
		{
			this.OnItemChanged(gridObject.SpatialItemInstance);
		}
	}

	private void OnItemChanged(SpatialItemInstance spatialItemInstance)
	{
		if (spatialItemInstance.GetItemData<SpatialItemData>().itemType == ItemType.EQUIPMENT)
		{
			this.CalculateEquipmentStats(spatialItemInstance.GetItemData<SpatialItemData>());
		}
	}

	private void OnResearchCompleted(ResearchableItemInstance researchableItemInstance)
	{
		this.CalculateResearchedBenefits();
	}

	public void CalculateDamageThreshold()
	{
		this.DamageThreshold = GameManager.Instance.GameConfigData.BasePlayerHealth + GameManager.Instance.SaveData.HullTier * GameManager.Instance.GameConfigData.PlayerHealthPerHullTier;
		this.DamageThreshold = Mathf.Min(this.DamageThreshold, GameManager.Instance.GameConfigData.MaxPlayerHealth);
	}

	private void CalculateAllStats()
	{
		this.CalculateEquipmentStats(null);
		this.CalculateResearchedBenefits();
	}

	private void CalculateEquipmentStats(SpatialItemData spatialItemData)
	{
		this.CalculateGadgetStats();
		this.CalculateRodStats();
		this.CalculateEngineStats();
		this.CalculateLightStats();
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
		if (spatialItemInstance != null)
		{
			GameManager.Instance.SaveData.TrawlNet.Init(spatialItemInstance.GetItemData<DeployableItemData>().GridConfig, false);
		}
		GameEvents.Instance.TriggerPlayerStatsChanged();
	}

	private void CalculateGadgetStats()
	{
		this.gadgetMagnitudes = new Dictionary<GadgetEffect, decimal>();
		GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.GADGET).ForEach(delegate(SpatialItemInstance gadget)
		{
			if (!gadget.GetIsOnDamagedCell())
			{
				GadgetItemData itemData = gadget.GetItemData<GadgetItemData>();
				if (!this.gadgetMagnitudes.ContainsKey(itemData.EffectType))
				{
					this.gadgetMagnitudes[itemData.EffectType] = 0m;
				}
				Dictionary<GadgetEffect, decimal> dictionary = this.gadgetMagnitudes;
				GadgetEffect effectType = itemData.EffectType;
				dictionary[effectType] += itemData.EffectMagnitude;
			}
		});
	}

	private void CalculateRodStats()
	{
		if (this.HarvestableTypes == null)
		{
			this.HarvestableTypes = new HashSet<HarvestableType>();
		}
		else
		{
			this.HarvestableTypes.Clear();
		}
		if (this.AdvancedHarvestableTypes == null)
		{
			this.AdvancedHarvestableTypes = new HashSet<HarvestableType>();
		}
		else
		{
			this.AdvancedHarvestableTypes.Clear();
		}
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD);
		List<SpatialItemInstance> allItemsOfType2 = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NET);
		float totalFishingSpeedModifier = GameManager.Instance.GameConfigData.BaseFishingSpeedModifier;
		float aberrationCatchBonus = 0f;
		allItemsOfType.ForEach(delegate(SpatialItemInstance r)
		{
			RodItemData itemData = r.GetItemData<RodItemData>();
			for (int i = 0; i < itemData.harvestableTypes.Length; i++)
			{
				this.HarvestableTypes.Add(itemData.harvestableTypes[i]);
				if (itemData.isAdvancedEquipment)
				{
					this.AdvancedHarvestableTypes.Add(itemData.harvestableTypes[i]);
				}
			}
			if (!r.GetIsOnDamagedCell())
			{
				totalFishingSpeedModifier += itemData.fishingSpeedModifier;
			}
			aberrationCatchBonus += itemData.aberrationBonus;
		});
		allItemsOfType2.ForEach(delegate(SpatialItemInstance n)
		{
			aberrationCatchBonus += n.GetItemData<HarvesterItemData>().aberrationBonus;
		});
		this.EquipmentFishingSpeedModifier = totalFishingSpeedModifier;
		this.EquipmentAberrationCatchModifier = aberrationCatchBonus;
		GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.DREDGE).ForEach(delegate(SpatialItemInstance d)
		{
			HarvesterItemData itemData2 = d.GetItemData<HarvesterItemData>();
			for (int j = 0; j < itemData2.harvestableTypes.Length; j++)
			{
				this.HarvestableTypes.Add(itemData2.harvestableTypes[j]);
			}
		});
	}

	private void CalculateEngineStats()
	{
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ENGINE);
		float totalSpeed = GameManager.Instance.GameConfigData.BasePlayerSpeed;
		allItemsOfType.ForEach(delegate(SpatialItemInstance e)
		{
			if (!e.GetIsOnDamagedCell())
			{
				EngineItemData itemData = e.GetItemData<EngineItemData>();
				totalSpeed += itemData.speedBonus;
			}
		});
		this.EquipmentMovementSpeedModifier = totalSpeed;
	}

	private void CalculateLightStats()
	{
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.LIGHT);
		float totalLumens = 0f;
		float maxRange = 0f;
		allItemsOfType.ForEach(delegate(SpatialItemInstance l)
		{
			if (!l.GetIsOnDamagedCell())
			{
				LightItemData itemData = l.GetItemData<LightItemData>();
				totalLumens += itemData.lumens;
				maxRange = Mathf.Max(maxRange, itemData.range);
			}
		});
		this.LightLumens = totalLumens;
		this.LightRange = maxRange;
		float num = Mathf.InverseLerp(0f, GameManager.Instance.GameConfigData.LumensForMaxLightSanityModifier, totalLumens);
		this.SanityModifier = Mathf.Lerp(0f, GameManager.Instance.GameConfigData.MaxLightSanityModifier, num);
	}

	private void CalculateResearchedBenefits()
	{
		List<ResearchableItemData> list = (from i in GameManager.Instance.SaveData.GetResearchableItemInstances(true)
			select i.GetItemData<ResearchableItemData>()).ToList<ResearchableItemData>();
		this.ResearchedBarteringModifier = 1f + this.GetResearchedBenefitValueForList(list, ResearchBenefitType.BARTER);
		this.ResearchedBarteringModifier = Mathf.Clamp(this.ResearchedBarteringModifier, 1f, 1.99f);
		this.ResearchedFishingSustainModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.FISHING_SUSTAIN);
		this.ResearchedFishingSpeedModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.FISHING_SPEED);
		this.ResearchedMovementSpeedModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.MOVEMENT_SPEED);
		this.ResearchedSanityModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.SANITY_RESILIENCE);
		this.ResearchedEquipmentMaintenanceModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.EQUIPMENT_MAINTENANCE);
		this.ResearchedAberrationCatchModifier = this.GetResearchedBenefitValueForList(list, ResearchBenefitType.ABERRATION_CATCH_BONUS);
		GameEvents.Instance.TriggerPlayerStatsChanged();
	}

	private float GetResearchedBenefitValueForList(List<ResearchableItemData> allItems, ResearchBenefitType benefitType)
	{
		ResearchBenefitCalculationStrategy strategy = GameManager.Instance.GameConfigData.ResearchBenefitCalculationStrategies[benefitType];
		List<ResearchableItemData> list = allItems.Where((ResearchableItemData i) => i.researchBenefitType == benefitType).ToList<ResearchableItemData>();
		float val = 0f;
		list.ForEach(delegate(ResearchableItemData r)
		{
			if (strategy == ResearchBenefitCalculationStrategy.CUMULATIVE)
			{
				val += r.researchBenefitValue;
				return;
			}
			if (strategy == ResearchBenefitCalculationStrategy.HIGHEST)
			{
				if (r.researchBenefitValue > val)
				{
					val = r.researchBenefitValue;
					return;
				}
			}
			else if (strategy == ResearchBenefitCalculationStrategy.LOWEST && r.researchBenefitValue < val)
			{
				val = r.researchBenefitValue;
			}
		});
		return val;
	}

	private Dictionary<GadgetEffect, decimal> gadgetMagnitudes = new Dictionary<GadgetEffect, decimal>();

	private GameConfigData config;
}
