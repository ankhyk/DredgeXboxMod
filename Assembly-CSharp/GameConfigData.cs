using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigData", menuName = "Dredge/GameConfigData", order = 0)]
public class GameConfigData : SerializedScriptableObject
{
	public SaveDataTemplate SaveDataTemplate
	{
		get
		{
			return this.saveDataTemplate;
		}
	}

	public SettingsSaveDataTemplate SettingsSaveDataTemplate
	{
		get
		{
			return this.settingsSaveDataTemplate;
		}
	}

	public List<RuntimePlatform> PlatformsSupportingControlRebindings
	{
		get
		{
			return this.platformsSupportingControlRebindings;
		}
	}

	public float WorldSize
	{
		get
		{
			return this.worldSize;
		}
	}

	public float DepthModifier
	{
		get
		{
			return this.depthModifier;
		}
	}

	public Dictionary<DepthEnum, Vector2> DepthBands
	{
		get
		{
			return this.depthBands;
		}
	}

	public int BasePlayerHealth
	{
		get
		{
			return this.basePlayerHealth;
		}
	}

	public int PlayerHealthPerHullTier
	{
		get
		{
			return this.playerHealthPerHullTier;
		}
	}

	public int MaxPlayerHealth
	{
		get
		{
			return this.maxPlayerHealth;
		}
	}

	public float BaseMovementSpeedModifier
	{
		get
		{
			return this.baseMovementSpeedModifier;
		}
	}

	public float BaseReverseSpeedModifier
	{
		get
		{
			return this.baseReverseSpeedModifier;
		}
	}

	public float BaseTurnSpeed
	{
		get
		{
			return this.baseTurnSpeed;
		}
	}

	public float NumAttachedMonstersToNullifyEngines
	{
		get
		{
			return (float)this.numAttachedMonstersToNullifyEngines;
		}
	}

	public float GlobalSanityModifier
	{
		get
		{
			return this.globalSanityModifier;
		}
	}

	public float NightSanityModifier
	{
		get
		{
			return this.nightSanityModifier;
		}
	}

	public float DaySanityModifier
	{
		get
		{
			return this.daySanityModifier;
		}
	}

	public float SleepingSanityModifier
	{
		get
		{
			return this.sleepingSanityModifier;
		}
	}

	public float MaxLightSanityModifier
	{
		get
		{
			return this.maxLightSanityModifier;
		}
	}

	public float LumensForMaxLightSanityModifier
	{
		get
		{
			return this.lumensForMaxLightSanityModifier;
		}
	}

	public float AdvancedLightsIntensityModifier
	{
		get
		{
			return this.advancedLightsIntensityModifier;
		}
	}

	public float AdvancedLightsRangeModifier
	{
		get
		{
			return this.advancedLightsRangeModifier;
		}
	}

	public int AberrationStartDay
	{
		get
		{
			return this.aberrationStartDay;
		}
	}

	public float BaseAberrationSpawnChance
	{
		get
		{
			return this.baseAberrationSpawnChance;
		}
	}

	public float NightAberrationSpawnChance
	{
		get
		{
			return this.nightAberrationSpawnChance;
		}
	}

	public float SpecialSpotAberrationSpawnBonus
	{
		get
		{
			return this.specialSpotAberrationSpawnBonus;
		}
	}

	public decimal SpawnChanceIncreasePerNonAberrationCaught
	{
		get
		{
			return this.spawnChanceIncreasePerNonAberrationCaught;
		}
	}

	public float MaxAberrationSpawnChance
	{
		get
		{
			return this.maxAberrationSpawnChance;
		}
	}

	public float ResearchItemDredgeSpotSpawnChance
	{
		get
		{
			return this.researchItemDredgeSpotSpawnChance;
		}
	}

	public float ResearchItemCrabPotSpawnChance
	{
		get
		{
			return this.researchItemCrabPotSpawnChance;
		}
	}

	public float TrophyMaxSize
	{
		get
		{
			return this.trophyMaxSize;
		}
	}

	public int MaxCrabPotCount
	{
		get
		{
			return this.maxCrabPotCount;
		}
	}

	public float SpecialSpotChanceDay
	{
		get
		{
			return this.specialSpotChanceDay;
		}
	}

	public float SpecialSpotChanceNight
	{
		get
		{
			return this.specialSpotChanceNight;
		}
	}

	public decimal MinSizeSaleModifier
	{
		get
		{
			return this.minSizeSaleModifier;
		}
	}

	public decimal MaxSizeSaleModifier
	{
		get
		{
			return this.maxSizeSaleModifier;
		}
	}

	public decimal[] FreshnessSaleModifiers
	{
		get
		{
			return this.freshnessSaleModifiers;
		}
	}

	public float FreshnessLossPerDay
	{
		get
		{
			return this.freshnessLossPerDay;
		}
	}

	public float MaxFreshness
	{
		get
		{
			return this.maxFreshness;
		}
	}

	public float MaxFreshnessLossReduction
	{
		get
		{
			return this.maxFreshnessLossReduction;
		}
	}

	public float CellsForMaxFreshnessLossReduction
	{
		get
		{
			return (float)this.cellsForMaxFreshnessLossReduction;
		}
	}

	public AnimationCurve FreshnessLossReductionCurve
	{
		get
		{
			return this.freshnessLossReductionCurve;
		}
	}

	public int NumCellsPerFishBait
	{
		get
		{
			return this.numCellsPerFishBait;
		}
	}

	public int NumFishInBaitBallMin
	{
		get
		{
			return this.numFishInBaitBallMin;
		}
	}

	public int NumFishInBaitBallMax
	{
		get
		{
			return this.numFishInBaitBallMax;
		}
	}

	public int NumFishSpeciesInBaitBall
	{
		get
		{
			return this.numFishSpeciesInBaitBall;
		}
	}

	public float HourDurationInSeconds
	{
		get
		{
			return this.hourDurationInSeconds;
		}
		set
		{
			this.hourDurationInSeconds = value;
		}
	}

	public float BasePlayerSpeed
	{
		get
		{
			return this.basePlayerSpeed;
		}
	}

	public float BaseFishingSpeedModifier
	{
		get
		{
			return this.baseFishingSpeedModifier;
		}
	}

	public float ForcedTimePassageSpeedModifier
	{
		get
		{
			return this.forcedTimePassageSpeedModifier;
		}
		set
		{
			this.forcedTimePassageSpeedModifier = value;
		}
	}

	public float FishingTimePassageSpeedModifier
	{
		get
		{
			return this.fishingTimePassageSpeedModifier;
		}
	}

	public float EquipmentInstallTimePerSquare
	{
		get
		{
			return this.equipmentInstallTimePerSquare;
		}
	}

	public decimal HullRepairCostPerSquare
	{
		get
		{
			return this.hullRepairCostPerSquare;
		}
	}

	public decimal PotRepairCostPerDay
	{
		get
		{
			return this.potRepairCostPerDay;
		}
	}

	public decimal NetRepairCostPerDay
	{
		get
		{
			return this.netRepairCostPerDay;
		}
	}

	public float DeployableDamagePerHitProportional
	{
		get
		{
			return this.deployableDamagePerHitProportional;
		}
	}

	public float AtrophyStockPenalty
	{
		get
		{
			return this.atrophyStockPenalty;
		}
	}

	public int AtrophyGuaranteedAberrationCount
	{
		get
		{
			return this.atrophyGuaranteedAberrationCount;
		}
	}

	public float AtrophyTotalParasiteChance
	{
		get
		{
			return this.atrophyTotalParasiteChance;
		}
	}

	public float AtrophyConditionMin
	{
		get
		{
			return this.atrophyConditionMin;
		}
	}

	public float AtrophyConditionMax
	{
		get
		{
			return this.atrophyConditionMax;
		}
	}

	public decimal GreaterMarrowDebt
	{
		get
		{
			return this.greaterMarrowDebt;
		}
	}

	public decimal GreaterMarrowDebtRepaymentProportion
	{
		get
		{
			return this.greaterMarrowDebtRepaymentProportion;
		}
	}

	public Dictionary<GameMode, float> WorldEventRollFrequency
	{
		get
		{
			return this.worldEventRollFrequency;
		}
	}

	public float WorldEventChance
	{
		get
		{
			return this.worldEventChance;
		}
	}

	public float TrophyNotchSpawnChance
	{
		get
		{
			return this.trophyNotchSpawnChance;
		}
	}

	public float BaitedTrophyNotchSpawnChance
	{
		get
		{
			return this.baitedTrophyNotchSpawnChance;
		}
	}

	public int FishToCatchBetweenTrophyNotches
	{
		get
		{
			return this.fishToCatchBetweenTrophyNotches;
		}
	}

	public float StockReplenishCoefficient
	{
		get
		{
			return this.stockReplenishCoefficient;
		}
	}

	public float MinStockReplenish
	{
		get
		{
			return this.minStockReplenish;
		}
	}

	public float InsaneTooltipThreshold
	{
		get
		{
			return this.insaneTooltipThreshold;
		}
	}

	public float BanishMachineDurationDays
	{
		get
		{
			return this.banishMachineDurationDays;
		}
	}

	public float ItemInfectionSpreadIntervalDays
	{
		get
		{
			return this.itemInfectionSpreadIntervalDays;
		}
	}

	public float ItemInfectionSpreadChance
	{
		get
		{
			return this.itemInfectionSpreadChance;
		}
	}

	public float InfectionAberrationSwapChance
	{
		get
		{
			return this.infectionAberrationSwapChance;
		}
	}

	public List<Color> Colors
	{
		get
		{
			return this.colors;
		}
	}

	public float CameraShakeScaleFactor
	{
		get
		{
			return this.cameraShakeScaleFactor;
		}
	}

	public float HasteHeatCap
	{
		get
		{
			return this.hasteHeatCap;
		}
	}

	public float HasteHeatGain
	{
		get
		{
			return this.hasteHeatGain;
		}
	}

	public float HasteHeatLoss
	{
		get
		{
			return this.hasteHeatLoss;
		}
	}

	public float HasteHeatCooldown
	{
		get
		{
			return this.hasteHeatCooldown;
		}
	}

	public float IceMonsterSpawnDelayPerCellOfFish
	{
		get
		{
			return this.iceMonsterSpawnDelayPerCellOfFish;
		}
	}

	public float IceMonsterSpawnDelayBase
	{
		get
		{
			return this.iceMonsterSpawnDelayBase;
		}
	}

	public float IceMonsterSpawnDelayOnFailedHunt
	{
		get
		{
			return this.iceMonsterSpawnDelayOnFailedHunt;
		}
	}

	public float OozeCollectionCoefficient
	{
		get
		{
			return this.oozeCollectionCoefficient;
		}
	}

	public float OozePatchProportionMinimum
	{
		get
		{
			return this.oozePatchProportionMinimum;
		}
	}

	public float MaxMovementPropInOoze
	{
		get
		{
			return this.maxMovementPropInOoze;
		}
	}

	public float MaxNumMapMarkers
	{
		get
		{
			return (float)this.maxNumMapMarkers;
		}
	}

	public float DarkSplashInfectionChance
	{
		get
		{
			return this.darkSplashInfectionChance;
		}
	}

	public GridConfiguration GetGridConfigForKey(GridKey key)
	{
		GridConfiguration gridConfiguration = null;
		this.gridConfigs.TryGetValue(key, out gridConfiguration);
		return gridConfiguration;
	}

	public GridConfiguration GetGridConfigForHullTier(int tier)
	{
		tier = Mathf.Clamp(tier, 1, 5);
		return this.hullTierGridConfigs[tier - 1];
	}

	[SerializeField]
	private SaveDataTemplate saveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate settingsSaveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate switchSettingsSaveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate pS4SettingsSaveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate pS5SettingsSaveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate gameCoreXboxOneSettingsSaveDataTemplate;

	[SerializeField]
	private SettingsSaveDataTemplate gameCoreXboxSeriesSettingsSaveDataTemplate;

	[SerializeField]
	private List<RuntimePlatform> platformsSupportingControlRebindings;

	[SerializeField]
	private Dictionary<GridKey, GridConfiguration> gridConfigs;

	[SerializeField]
	private List<GridConfiguration> hullTierGridConfigs;

	[Space(10f)]
	[SerializeField]
	private float worldSize;

	[SerializeField]
	private float depthModifier;

	[SerializeField]
	private Dictionary<DepthEnum, Vector2> depthBands;

	[SerializeField]
	private int basePlayerHealth;

	[SerializeField]
	private int playerHealthPerHullTier;

	[SerializeField]
	private int maxPlayerHealth;

	[Header("Movement Speeds")]
	[SerializeField]
	private float baseMovementSpeedModifier;

	[SerializeField]
	private float baseReverseSpeedModifier;

	[SerializeField]
	private float baseTurnSpeed;

	[SerializeField]
	private int numAttachedMonstersToNullifyEngines;

	[Header("Sanity Config")]
	[SerializeField]
	private float globalSanityModifier;

	[SerializeField]
	private float nightSanityModifier;

	[SerializeField]
	private float daySanityModifier;

	[SerializeField]
	private float sleepingSanityModifier;

	[SerializeField]
	private float maxLightSanityModifier;

	[SerializeField]
	private float lumensForMaxLightSanityModifier;

	[SerializeField]
	private float advancedLightsIntensityModifier;

	[SerializeField]
	private float advancedLightsRangeModifier;

	[Header("Aberration Config")]
	[SerializeField]
	private int aberrationStartDay;

	[SerializeField]
	private float baseAberrationSpawnChance;

	[SerializeField]
	private float nightAberrationSpawnChance;

	[SerializeField]
	private float specialSpotAberrationSpawnBonus;

	[SerializeField]
	private decimal spawnChanceIncreasePerNonAberrationCaught;

	[SerializeField]
	private float maxAberrationSpawnChance;

	[SerializeField]
	private float researchItemDredgeSpotSpawnChance;

	[SerializeField]
	private float researchItemCrabPotSpawnChance;

	[SerializeField]
	private float trophyMaxSize;

	[SerializeField]
	private int maxCrabPotCount;

	[Header("Special Fishing Spot Config")]
	[SerializeField]
	private float specialSpotChanceDay;

	[SerializeField]
	private float specialSpotChanceNight;

	[Header("Fish Sale Price Config")]
	[SerializeField]
	private decimal minSizeSaleModifier;

	[SerializeField]
	private decimal maxSizeSaleModifier;

	[SerializeField]
	private decimal[] freshnessSaleModifiers;

	[SerializeField]
	private float freshnessLossPerDay;

	[SerializeField]
	private float maxFreshness;

	[SerializeField]
	private float maxFreshnessLossReduction;

	[SerializeField]
	private int cellsForMaxFreshnessLossReduction;

	[SerializeField]
	private AnimationCurve freshnessLossReductionCurve;

	[SerializeField]
	private int numCellsPerFishBait;

	[SerializeField]
	private int numFishInBaitBallMin;

	[SerializeField]
	private int numFishInBaitBallMax;

	[SerializeField]
	private int numFishSpeciesInBaitBall;

	[SerializeField]
	private float hourDurationInSeconds;

	[SerializeField]
	private float basePlayerSpeed;

	[SerializeField]
	private float baseFishingSpeedModifier;

	[SerializeField]
	private float forcedTimePassageSpeedModifier;

	[SerializeField]
	private float fishingTimePassageSpeedModifier;

	[SerializeField]
	private float equipmentInstallTimePerSquare;

	[SerializeField]
	private decimal hullRepairCostPerSquare;

	[SerializeField]
	private decimal potRepairCostPerDay;

	[SerializeField]
	private decimal netRepairCostPerDay;

	[SerializeField]
	private float deployableDamagePerHitProportional;

	[SerializeField]
	private float atrophyStockPenalty;

	[SerializeField]
	private int atrophyGuaranteedAberrationCount;

	[SerializeField]
	private float atrophyTotalParasiteChance;

	[SerializeField]
	private float atrophyConditionMin;

	[SerializeField]
	private float atrophyConditionMax;

	[SerializeField]
	private decimal greaterMarrowDebt;

	[SerializeField]
	private decimal greaterMarrowDebtRepaymentProportion;

	[SerializeField]
	private Dictionary<GameMode, float> worldEventRollFrequency = new Dictionary<GameMode, float>();

	[SerializeField]
	private float worldEventChance;

	[SerializeField]
	public Dictionary<ResearchBenefitType, ResearchBenefitCalculationStrategy> ResearchBenefitCalculationStrategies;

	[Header("Harvest Minigame Difficulty")]
	[SerializeField]
	public Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> DredgingDifficultyConfigs;

	[SerializeField]
	public Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> FishingDifficultyConfigs;

	[SerializeField]
	private float trophyNotchSpawnChance;

	[SerializeField]
	private float baitedTrophyNotchSpawnChance;

	[SerializeField]
	private int fishToCatchBetweenTrophyNotches;

	[SerializeField]
	private float stockReplenishCoefficient;

	[SerializeField]
	private float minStockReplenish;

	[SerializeField]
	private float insaneTooltipThreshold;

	[SerializeField]
	private float banishMachineDurationDays;

	[SerializeField]
	private float itemInfectionSpreadIntervalDays;

	[SerializeField]
	private float itemInfectionSpreadChance;

	[SerializeField]
	private float infectionAberrationSwapChance;

	[SerializeField]
	private List<Color> colors;

	[SerializeField]
	private float cameraShakeScaleFactor;

	[SerializeField]
	private float hasteHeatCap;

	[SerializeField]
	private float hasteHeatGain;

	[SerializeField]
	private float hasteHeatLoss;

	[SerializeField]
	private float hasteHeatCooldown;

	[SerializeField]
	private float iceMonsterSpawnDelayPerCellOfFish;

	[SerializeField]
	private float iceMonsterSpawnDelayBase;

	[SerializeField]
	private float iceMonsterSpawnDelayOnFailedHunt;

	[SerializeField]
	private float oozeCollectionCoefficient;

	[SerializeField]
	private float oozePatchProportionMinimum;

	[SerializeField]
	private float maxMovementPropInOoze;

	[SerializeField]
	private int maxNumMapMarkers;

	[SerializeField]
	private float darkSplashInfectionChance;
}
