using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class PlayerStatsUI : SerializedMonoBehaviour
{
	public string _BoatSpeed { get; set; }

	public string _BoatSpeedProjectedChange { get; set; }

	public string _FishingSpeed { get; set; }

	public string _FishingSpeedProjectedChange { get; set; }

	public string _LightPower { get; set; }

	public string _LightPowerProjectedChange { get; set; }

	public string _AberrationBonus { get; set; }

	public string _AberrationBonusProjectedChange { get; set; }

	private void Awake()
	{
		this.harvestableTypeTags.Keys.ToList<HarvestableType>().ForEach(delegate(HarvestableType h)
		{
			this.harvestableTypeTags[h].Init(h, false);
		});
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerStatsChanged += this.OnPlayerStatsChanged;
		GameEvents.Instance.OnCanInstallHoveredItemChanged += this.OnCanInstallHoveredItemChanged;
		this.OnPlayerStatsChanged();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerStatsChanged -= this.OnPlayerStatsChanged;
		GameEvents.Instance.OnCanInstallHoveredItemChanged -= this.OnCanInstallHoveredItemChanged;
	}

	private void OnCanInstallHoveredItemChanged(bool canInstall, bool freeOfDamage)
	{
		this.boatSpeedProjectedChange = 0f;
		this.fishingSpeedProjectedChange = 0f;
		this.lightPowerProjectedChange = 0f;
		this.aberrationBonusProjectedChange = 0f;
		if (canInstall)
		{
			GridObject currentlyHeldObject = GameManager.Instance.GridManager.CurrentlyHeldObject;
			GridCell lastSelectedCell = GameManager.Instance.GridManager.LastSelectedCell;
			GridObject gridObject = ((lastSelectedCell != null) ? lastSelectedCell.OccupyingObject : null);
			if (currentlyHeldObject)
			{
				SpatialItemData itemData = currentlyHeldObject.ItemData;
				if (itemData.itemSubtype == ItemSubtype.ENGINE && freeOfDamage)
				{
					this.boatSpeedProjectedChange += (itemData as EngineItemData).speedBonus;
				}
				else if (itemData.itemSubtype == ItemSubtype.ROD && freeOfDamage)
				{
					this.fishingSpeedProjectedChange += (itemData as RodItemData).fishingSpeedModifier;
				}
				else if (itemData.itemSubtype == ItemSubtype.LIGHT && freeOfDamage)
				{
					this.lightPowerProjectedChange += (itemData as LightItemData).lumens;
				}
				if (itemData is HarvesterItemData)
				{
					this.aberrationBonusProjectedChange += (itemData as HarvesterItemData).aberrationBonus;
				}
			}
			if (gridObject)
			{
				SpatialItemData itemData2 = gridObject.ItemData;
				if (itemData2.itemSubtype == ItemSubtype.ENGINE && !gridObject.SpatialItemInstance.GetIsOnDamagedCell())
				{
					this.boatSpeedProjectedChange -= (itemData2 as EngineItemData).speedBonus;
				}
				else if (itemData2.itemSubtype == ItemSubtype.ROD && !gridObject.SpatialItemInstance.GetIsOnDamagedCell())
				{
					this.fishingSpeedProjectedChange -= (itemData2 as RodItemData).fishingSpeedModifier;
				}
				else if (itemData2.itemSubtype == ItemSubtype.LIGHT && !gridObject.SpatialItemInstance.GetIsOnDamagedCell())
				{
					this.lightPowerProjectedChange -= (itemData2 as LightItemData).lumens;
				}
				if (itemData2 is HarvesterItemData)
				{
					this.aberrationBonusProjectedChange += (itemData2 as HarvesterItemData).aberrationBonus;
				}
			}
		}
		this.RefreshUI();
	}

	private void OnPlayerStatsChanged()
	{
		this.boatSpeedProjectedChange = 0f;
		this.fishingSpeedProjectedChange = 0f;
		this.lightPowerProjectedChange = 0f;
		this.aberrationBonusProjectedChange = 0f;
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		HashSet<HarvestableType> activeTypes = GameManager.Instance.PlayerStats.HarvestableTypes;
		HashSet<HarvestableType> activeAdvancedTypes = GameManager.Instance.PlayerStats.AdvancedHarvestableTypes;
		this.harvestableTypeTags.Keys.ToList<HarvestableType>().ForEach(delegate(HarvestableType h)
		{
			this.harvestableTypeTags[h].gameObject.SetActive(activeTypes.Contains(h));
			this.harvestableTypeTags[h].SetAdvancedState(activeAdvancedTypes.Contains(h));
		});
		this.boatSpeedProjectedChange *= 1f + GameManager.Instance.PlayerStats.ResearchedMovementSpeedModifier;
		string text = GameManager.Instance.PlayerStats.MovementSpeedModifier.ToString(".#");
		this._BoatSpeed = text + " kn";
		if (this.boatSpeedProjectedChange == 0f)
		{
			this._BoatSpeedProjectedChange = "";
		}
		else
		{
			string text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			string text3 = "+";
			if (this.boatSpeedProjectedChange < 0f)
			{
				text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
				text3 = "";
			}
			string text4 = this.boatSpeedProjectedChange.ToString(".#");
			this._BoatSpeedProjectedChange = string.Concat(new string[] { "(<color=#", text2, ">", text3, text4, " kn</color>)" });
		}
		this.fishingSpeedProjectedChange *= 1f + GameManager.Instance.PlayerStats.ResearchedFishingSpeedModifier;
		this._FishingSpeed = string.Format("{0}%", Mathf.RoundToInt(GameManager.Instance.PlayerStats.FishingSpeedModifier * 100f));
		if (this.fishingSpeedProjectedChange == 0f)
		{
			this._FishingSpeedProjectedChange = "";
		}
		else
		{
			string text5 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			string text6 = "+";
			if (this.fishingSpeedProjectedChange < 0f)
			{
				text5 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
				text6 = "";
			}
			int num = Mathf.RoundToInt(this.fishingSpeedProjectedChange * 100f);
			this._FishingSpeedProjectedChange = string.Format("(<color=#{0}>{1}{2}%</color>)", text5, text6, num);
		}
		string text7 = GameManager.Instance.PlayerStats.LightLumens.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
		this._LightPower = text7 + " lm";
		if (this.lightPowerProjectedChange == 0f)
		{
			this._LightPowerProjectedChange = "";
		}
		else
		{
			string text8 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			string text9 = "+";
			if (this.lightPowerProjectedChange < 0f)
			{
				text8 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
				text9 = "";
			}
			string text10 = this.lightPowerProjectedChange.ToString();
			this._LightPowerProjectedChange = string.Concat(new string[] { "(<color=#", text8, ">", text9, text10, " lm</color>)" });
		}
		string text11 = (GameManager.Instance.PlayerStats.TotalAberrationCatchModifier * 100f).ToString("0.#####");
		this._AberrationBonus = text11 + "%";
		if (this.aberrationBonusProjectedChange == 0f)
		{
			this._AberrationBonusProjectedChange = "";
		}
		else
		{
			string text12 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			string text13 = "+";
			if (this.aberrationBonusProjectedChange < 0f)
			{
				text12 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
				text13 = "";
			}
			string text14 = (this.aberrationBonusProjectedChange * 100f).ToString("0.#####");
			this._AberrationBonusProjectedChange = string.Concat(new string[] { "(<color=#", text12, ">", text13, text14, "%</color>)" });
		}
		this.boatSpeedTextField.StringReference.Arguments = new string[] { this._BoatSpeed, this._BoatSpeedProjectedChange };
		this.fishingSpeedTextField.StringReference.Arguments = new string[] { this._FishingSpeed, this._FishingSpeedProjectedChange };
		this.lightPowerTextField.StringReference.Arguments = new string[] { this._LightPower, this._LightPowerProjectedChange };
		this.aberrationBonusTextField.StringReference.Arguments = new string[] { this._AberrationBonus, this._AberrationBonusProjectedChange };
		this.fishingSpeedTextField.RefreshString();
		this.boatSpeedTextField.RefreshString();
		this.lightPowerTextField.RefreshString();
		this.aberrationBonusTextField.RefreshString();
		this.boatSpeedTextField.enabled = true;
		this.fishingSpeedTextField.enabled = true;
		this.lightPowerTextField.enabled = true;
		this.aberrationBonusTextField.enabled = true;
		this.aberrationBonusTextGameObject.SetActive(GameManager.Instance.PlayerStats.TotalAberrationCatchModifier != 0f || this.aberrationBonusProjectedChange != 0f);
	}

	[SerializeField]
	private LocalizeStringEvent boatSpeedTextField;

	[SerializeField]
	private LocalizeStringEvent fishingSpeedTextField;

	[SerializeField]
	private LocalizeStringEvent lightPowerTextField;

	[SerializeField]
	private LocalizeStringEvent aberrationBonusTextField;

	[SerializeField]
	private GameObject aberrationBonusTextGameObject;

	[SerializeField]
	private Dictionary<HarvestableType, HarvestableTypeTagUI> harvestableTypeTags = new Dictionary<HarvestableType, HarvestableTypeTagUI>();

	private float boatSpeedProjectedChange;

	private float fishingSpeedProjectedChange;

	private float lightPowerProjectedChange;

	private float aberrationBonusProjectedChange;
}
