using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ActiveAbilityInfoPanel : MonoBehaviour
{
	private void Awake()
	{
		this.originAnchorX = this.containerRect.anchoredPosition.x;
		this.containerRect.anchoredPosition = new Vector2(this.originAnchorX - this.animateXAmount, this.containerRect.anchoredPosition.y);
		GameEvents.Instance.OnAbilityItemCycled += this.OnAbilityItemCycled;
		this.AddTerminalCommands();
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnAbilityItemCycled -= this.OnAbilityItemCycled;
		this.RemoveTerminalCommands();
	}

	private void OnEnable()
	{
		this.timeUntilNextUpdate = 0f;
		GameEvents.Instance.OnPlayerAbilitySelected += this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		if (GameManager.Instance.UI && GameManager.Instance.UI.AbilityBarUI)
		{
			this.OnPlayerAbilitySelected(GameManager.Instance.UI.AbilityBarUI.CurrentAbilityData);
		}
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilitySelected -= this.OnPlayerAbilitySelected;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool active)
	{
		this.isAbilityActive = active;
	}

	private void RefreshItemNameField(SpatialItemData spatialItemData)
	{
		this.currentlySelectedItemData = spatialItemData;
		this.itemNameField.StringReference = ((spatialItemData == null) ? this.noItemStringKey : spatialItemData.itemNameKey);
		this.itemNameField.RefreshString();
		this.itemNameTextField.color = GameManager.Instance.LanguageManager.GetColor((spatialItemData == null) ? DredgeColorTypeEnum.NEGATIVE : DredgeColorTypeEnum.NEUTRAL);
	}

	private void OnPlayerAbilitySelected(AbilityData abilityData)
	{
		if (abilityData == null)
		{
			return;
		}
		Ability abilityForData = GameManager.Instance.PlayerAbilities.GetAbilityForData(abilityData);
		if (abilityForData.AbilityData.allowItemCycling)
		{
			this.OnAbilityItemCycled(abilityForData.CurrentlySelectedItem, abilityForData.UniqueItemDatasUsedByAbility.Count);
			this.cycleAbilityItemPrevIcon.Init(abilityForData.CycleItemPrevAction);
			this.cycleAbilityItemNextIcon.Init(abilityForData.CycleItemNextAction);
		}
		else
		{
			this.RefreshUI(null, abilityData);
			this.cycleAbilityItemPrevIcon.gameObject.SetActive(false);
			this.cycleAbilityItemNextIcon.gameObject.SetActive(false);
		}
		this.timeUntilNextUpdate = 0f;
	}

	private void RefreshUI(SpatialItemData spatialItemData, AbilityData abilityData)
	{
		bool flag = false;
		if (abilityData.name == this.potAbility.name)
		{
			if (spatialItemData != null && spatialItemData.id == this.materialPotItemData.id)
			{
				this.qualityIcon.sprite = this.materialQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.POT_MATERIAL;
			}
			else
			{
				this.qualityIcon.sprite = this.potQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.POT;
			}
			flag = true;
		}
		else if (abilityData.name == this.baitAbility.name)
		{
			if (spatialItemData != null && spatialItemData.id == this.crabBaitItemData.id)
			{
				this.qualityIcon.sprite = this.baitCrabQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.BAIT_CRAB;
			}
			else if (spatialItemData != null && spatialItemData.id == this.exoticBaitItemData.id)
			{
				this.qualityIcon.sprite = this.baitExoticQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.BAIT_EXOTIC;
			}
			else if (spatialItemData != null && spatialItemData.id == this.aberratedBaitItemData.id)
			{
				this.qualityIcon.sprite = this.baitAberratedQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.BAIT_ABERRATED;
			}
			else
			{
				this.qualityIcon.sprite = this.baitRegularQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.BAIT;
			}
			flag = true;
		}
		else if (abilityData.name == this.trawlAbility.name)
		{
			if (spatialItemData != null && spatialItemData.id == this.oozeNetItemData.id)
			{
				this.qualityIcon.sprite = this.oozeQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE;
			}
			else if (spatialItemData != null && spatialItemData.id == this.materialNetItemData.id)
			{
				this.qualityIcon.sprite = this.materialQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.TRAWL_MATERIAL;
			}
			else
			{
				this.qualityIcon.sprite = this.trawlQualityIcon;
				this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.TRAWL;
			}
			flag = true;
		}
		else
		{
			this.abilityMode = ActiveAbilityInfoPanel.AbilityMode.NONE;
		}
		if (flag)
		{
			this.Show();
		}
		else
		{
			this.Hide();
		}
		this.RefreshItemNameField(spatialItemData);
		this.UpdateCatchableSpecies();
	}

	private void Show()
	{
		this.oozeFillGroup.SetActive(this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE);
		this.depthGroup.SetActive(this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE);
		if (this.isShowing)
		{
			return;
		}
		this.isShowing = true;
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		if (this.slideTween != null)
		{
			this.slideTween.Kill(false);
			this.slideTween = null;
		}
		this.containerRect.gameObject.SetActive(true);
		this.fadeTween = this.canvasGroup.DOFade(1f, this.animateDurationSec);
		this.slideTween = this.containerRect.DOAnchorPosX(this.originAnchorX, this.animateDurationSec, false);
		this.slideTween.SetEase(Ease.OutExpo);
		this.fadeTween.SetEase(Ease.OutExpo);
		this.slideTween.SetUpdate(true);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(new TweenCallback(this.OnShowComplete));
	}

	private void Hide()
	{
		if (!this.isShowing)
		{
			return;
		}
		this.isShowing = false;
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		if (this.slideTween != null)
		{
			this.slideTween.Kill(false);
			this.slideTween = null;
		}
		this.fadeTween = this.canvasGroup.DOFade(0f, this.animateDurationSec);
		this.slideTween = this.containerRect.DOAnchorPosX(this.originAnchorX - this.animateXAmount, this.animateDurationSec, false);
		this.slideTween.SetEase(Ease.OutExpo);
		this.fadeTween.SetEase(Ease.OutExpo);
		this.slideTween.SetUpdate(true);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(new TweenCallback(this.OnHideComplete));
	}

	private void OnShowComplete()
	{
		this.fadeTween = null;
		this.slideTween = null;
	}

	private void OnHideComplete()
	{
		this.fadeTween = null;
		this.slideTween = null;
		this.containerRect.gameObject.SetActive(false);
	}

	private void Update()
	{
		this.timeUntilNextUpdate -= Time.deltaTime;
		if (this.timeUntilNextUpdate <= 0f && this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.NONE)
		{
			this.timeUntilNextUpdate = this.timeBetweenUpdates;
			this.UpdateDepthText();
			this.UpdateCatchableSpecies();
		}
		if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE)
		{
			this.oozeFillImage.fillAmount = GameManager.Instance.OozePatchManager.TotalOozeCollected;
		}
	}

	private void UpdateDepthText()
	{
		float currentDepth = GameManager.Instance.Player.PlayerDepthMonitor.CurrentDepth;
		this.depthTextField.text = GameManager.Instance.ItemManager.GetFormattedDepthString(currentDepth);
	}

	private void OnAbilityItemCycled(SpatialItemData spatialItemData, int totalNumItemTypes)
	{
		this.RefreshUI(spatialItemData, GameManager.Instance.PlayerAbilities.CurrentlySelectedAbilityData);
		this.cycleAbilityItemPrevIcon.gameObject.SetActive(totalNumItemTypes > 1);
		this.cycleAbilityItemNextIcon.gameObject.SetActive(totalNumItemTypes > 1);
	}

	private void UpdateCatchableSpecies()
	{
		List<string> list = new List<string>();
		if (this.enableLogs)
		{
			this.debugStr = "Depth: " + this.depthTextField.text + " | ";
		}
		int num;
		if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.POT_MATERIAL || this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL_MATERIAL)
		{
			num = 2;
		}
		else if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.POT)
		{
			list = GameManager.Instance.Player.HarvestZoneDetector.GetHarvestableItemIds(new Func<HarvestableItemData, bool>(this.CheckCanBeCaughtByThisPot), GameManager.Instance.Player.PlayerDepthMonitor.CurrentDepth, true);
			num = list.Count;
		}
		else if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL)
		{
			list = GameManager.Instance.Player.HarvestZoneDetector.GetHarvestableItemIds(new Func<HarvestableItemData, bool>(this.CheckCanBeCaughtByThisNet), GameManager.Instance.Player.PlayerDepthMonitor.CurrentDepth, GameManager.Instance.Time.IsDaytime);
			num = list.Count;
			if (this.enableLogs)
			{
				if (this.currentlySelectedItemData)
				{
					this.debugStr = this.debugStr + "net: " + this.currentlySelectedItemData.id + " | ";
				}
				else
				{
					this.debugStr += "net: NONE | ";
				}
			}
		}
		else if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE)
		{
			num = (GameManager.Instance.OozePatchManager.isOozeNearToPlayer ? 1 : 0);
			this.oozeCanisterAnimator.SetBool("IsActive", GameManager.Instance.OozePatchManager.GetIsCurrentlyGatheringOoze());
		}
		else
		{
			if (this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.BAIT && this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.BAIT_ABERRATED && this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.BAIT_CRAB && this.abilityMode != ActiveAbilityInfoPanel.AbilityMode.BAIT_EXOTIC)
			{
				return;
			}
			list = (from s in BaitAbility.GetFishForBait(this.currentlySelectedItemData)
				select s.id).ToList<string>();
			num = list.Count;
		}
		if (this.enableLogs)
		{
			list.ForEach(delegate(string s)
			{
				this.debugStr = this.debugStr + s + ", ";
			});
		}
		if (this.abilityMode == ActiveAbilityInfoPanel.AbilityMode.TRAWL_OOZE)
		{
			if (num == 0)
			{
				this.localizedQualityString.StringReference = this.poorQualityStringKey;
				this.qualityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
				return;
			}
			this.localizedQualityString.StringReference = this.oozeQualityStringKey;
			this.qualityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL);
			return;
		}
		else
		{
			if (num == 0)
			{
				this.localizedQualityString.StringReference = this.poorQualityStringKey;
				this.qualityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
				return;
			}
			if (num == 1)
			{
				this.localizedQualityString.StringReference = this.okQualityStringKey;
				this.qualityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
				return;
			}
			if (num > 1)
			{
				this.localizedQualityString.StringReference = this.goodQualityStringKey;
				this.qualityTextField.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			}
			return;
		}
	}

	private bool CheckCanBeCaughtByThisPot(HarvestableItemData itemData)
	{
		return this.currentlySelectedItemData != null && itemData.canBeCaughtByPot && (this.currentlySelectedItemData as DeployableItemData).GridConfig.MainItemSubtype.HasFlag(itemData.itemSubtype);
	}

	private bool CheckCanBeCaughtByThisNet(HarvestableItemData itemData)
	{
		if (!itemData.canBeCaughtByNet)
		{
			return false;
		}
		if (this.currentlySelectedItemData == null)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < (this.currentlySelectedItemData as HarvesterItemData).harvestableTypes.Length; i++)
		{
			if ((this.currentlySelectedItemData as HarvesterItemData).harvestableTypes[i] == itemData.harvestableType)
			{
				flag = true;
			}
		}
		return flag;
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("logs.depth", new Action<CommandArg[]>(this.ToggleDepthLogs), 1, 1, "Enables or disables depth (and harvestable) logging. Requires net or pot ability to be selected.");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("logs.depth");
	}

	private void ToggleDepthLogs(CommandArg[] args)
	{
		this.enableLogs = args[0].Int == 1;
	}

	[SerializeField]
	private AbilityData potAbility;

	[SerializeField]
	private AbilityData trawlAbility;

	[SerializeField]
	private AbilityData baitAbility;

	[SerializeField]
	private float timeBetweenUpdates;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private RectTransform containerRect;

	[SerializeField]
	private TextMeshProUGUI depthTextField;

	[SerializeField]
	private TextMeshProUGUI qualityTextField;

	[SerializeField]
	private LocalizeStringEvent itemNameField;

	[SerializeField]
	private TextMeshProUGUI itemNameTextField;

	[SerializeField]
	private LocalizeStringEvent localizedQualityString;

	[SerializeField]
	private LocalizedString poorQualityStringKey;

	[SerializeField]
	private LocalizedString okQualityStringKey;

	[SerializeField]
	private LocalizedString goodQualityStringKey;

	[SerializeField]
	private LocalizedString oozeQualityStringKey;

	[SerializeField]
	private LocalizedString noItemStringKey;

	[SerializeField]
	private Image qualityIcon;

	[SerializeField]
	private Sprite potQualityIcon;

	[SerializeField]
	private Sprite trawlQualityIcon;

	[SerializeField]
	private Sprite oozeQualityIcon;

	[SerializeField]
	private Sprite materialQualityIcon;

	[SerializeField]
	private Sprite baitRegularQualityIcon;

	[SerializeField]
	private Sprite baitCrabQualityIcon;

	[SerializeField]
	private Sprite baitAberratedQualityIcon;

	[SerializeField]
	private Sprite baitExoticQualityIcon;

	[SerializeField]
	private GameObject oozeFillGroup;

	[SerializeField]
	private GameObject depthGroup;

	[SerializeField]
	private Image oozeFillImage;

	[SerializeField]
	private float animateXAmount;

	[SerializeField]
	private float animateDurationSec;

	[SerializeField]
	private SpatialItemData oozeNetItemData;

	[SerializeField]
	private SpatialItemData materialNetItemData;

	[SerializeField]
	private SpatialItemData materialPotItemData;

	[SerializeField]
	private SpatialItemData aberratedBaitItemData;

	[SerializeField]
	private SpatialItemData exoticBaitItemData;

	[SerializeField]
	private SpatialItemData crabBaitItemData;

	[SerializeField]
	private ControlPromptIcon cycleAbilityItemPrevIcon;

	[SerializeField]
	private ControlPromptIcon cycleAbilityItemNextIcon;

	[SerializeField]
	private Animator oozeCanisterAnimator;

	private SpatialItemData currentlySelectedItemData;

	private ActiveAbilityInfoPanel.AbilityMode abilityMode;

	private float timeUntilNextUpdate;

	private bool isShowing;

	private Tweener fadeTween;

	private Tweener slideTween;

	private float originAnchorX;

	private bool enableLogs;

	private bool isAbilityActive;

	private string debugStr;

	private enum AbilityMode
	{
		NONE,
		POT,
		POT_MATERIAL,
		TRAWL,
		TRAWL_OOZE,
		TRAWL_MATERIAL,
		BAIT,
		BAIT_ABERRATED,
		BAIT_EXOTIC,
		BAIT_CRAB
	}
}
