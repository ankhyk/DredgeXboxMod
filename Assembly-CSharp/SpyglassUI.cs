using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SpyglassUI : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		this.hasAdvancedSpyglass = GameManager.Instance.SaveData.unlockedAbilities.Contains(this.advancedSpyglassAbilityData.name);
		this.togglePinAction = new DredgePlayerActionPress("prompt.toggle-pin", GameManager.Instance.Input.Controls.TagSpyglassMarker);
		this.togglePinAction.showInControlArea = false;
		this.togglePinAction.evaluateWhenPaused = false;
		DredgePlayerActionPress dredgePlayerActionPress = this.togglePinAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPressTogglePin));
		this.togglePinControlPrompt.Init(this.togglePinAction);
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		if (abilityData.name == this.advancedSpyglassAbilityData.name)
		{
			GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
			this.hasAdvancedSpyglass = true;
		}
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (this.spyglassAbilityData.name == abilityData.name)
		{
			this.isAbilityActive = enabled;
			if (!this.isAbilityActive)
			{
				this.focusedHarvestPOI = null;
				this.container.SetActive(false);
				this.animator.SetBool("showing", false);
			}
			this.crosshairContainer.SetActive(this.isAbilityActive);
			this.crosshairXImage.sprite = (this.hasAdvancedSpyglass ? this.advancedCrosshairImage : this.basicCrosshairImage);
			this.crosshairYImage.sprite = (this.hasAdvancedSpyglass ? this.advancedCrosshairImage : this.basicCrosshairImage);
			if (this.isAbilityActive)
			{
				this.AddStamps();
				return;
			}
			this.RemoveAllStamps();
		}
	}

	private void AddStamps()
	{
		GameManager.Instance.SaveData.mapStamps.ForEach(new Action<SerializedMapStamp>(this.AddMapStamp));
		GameManager.Instance.SaveData.harvestPOIMapMarkers.ForEach(delegate(string id)
		{
			this.AddHarvestPOIMarker(id);
		});
		this.stampCanvasGroup.DOKill(false);
		this.stampCanvasGroup.DOFade(1f, 1f).From(0f, true, false);
		this.stampContainer.gameObject.SetActive(true);
	}

	private void AddMapStamp(SerializedMapStamp stampData)
	{
		SpyglassMapStamp component = global::UnityEngine.Object.Instantiate<GameObject>(this.stampPrefab, this.stampContainer).GetComponent<SpyglassMapStamp>();
		component.Init(this.config.stampSprites[stampData.stampType], GameManager.Instance.LanguageManager.GetColor(this.config.stampColors[stampData.stampType]), stampData);
		this.instantiatedStamps.Add(component);
	}

	private SpyglassMapStamp AddHarvestPOIMarker(string id)
	{
		SpyglassMapStamp spyglassMapStamp = null;
		HarvestPOI harvestPOI;
		if (GameManager.Instance.HarvestPOIManager.harvestPOILookup.TryGetValue(id, out harvestPOI))
		{
			spyglassMapStamp = global::UnityEngine.Object.Instantiate<GameObject>(this.stampPrefab, this.stampContainer).GetComponent<SpyglassMapStamp>();
			spyglassMapStamp.Init(harvestPOI);
			this.instantiatedStamps.Add(spyglassMapStamp);
		}
		return spyglassMapStamp;
	}

	private void RemoveHarvestPOIMarker(string id)
	{
		SpyglassMapStamp spyglassMapStamp = this.instantiatedStamps.Find((SpyglassMapStamp s) => id == s.HarvestPOIID);
		if (spyglassMapStamp)
		{
			this.instantiatedStamps.Remove(spyglassMapStamp);
			global::UnityEngine.Object.Destroy(spyglassMapStamp.gameObject);
			GameManager.Instance.AudioPlayer.PlaySFX(this.advancedSpyglassRemoveMarker, AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	private void RemoveAllStamps()
	{
		this.stampContainer.gameObject.SetActive(false);
		this.instantiatedStamps.ForEach(delegate(SpyglassMapStamp stamp)
		{
			if (stamp)
			{
				global::UnityEngine.Object.Destroy(stamp.gameObject);
			}
		});
		this.instantiatedStamps.Clear();
	}

	public void SetFocusedHarvestPOI(HarvestPOI focusedHarvestPOI)
	{
		this.focusedHarvestPOI = focusedHarvestPOI;
		if (focusedHarvestPOI)
		{
			this.focusedHarvestPOIID = focusedHarvestPOI.Harvestable.GetId();
			this.isHarvestPOICurrentlyPinned = GameManager.Instance.SaveData.harvestPOIMapMarkers.Contains(this.focusedHarvestPOIID);
			SpyglassMapStamp spyglassMapStamp = this.instantiatedStamps.Find((SpyglassMapStamp s) => this.focusedHarvestPOIID == s.HarvestPOIID);
			if (spyglassMapStamp)
			{
				this.conflictingStamp = spyglassMapStamp;
				this.conflictingStamp.gameObject.SetActive(false);
			}
		}
		else
		{
			this.focusedHarvestPOIID = "";
			this.isHarvestPOICurrentlyPinned = false;
			if (this.conflictingStamp)
			{
				this.conflictingStamp.gameObject.SetActive(true);
				this.conflictingStamp = null;
			}
		}
		this.RefreshUI();
	}

	private void Update()
	{
		if (this.isAbilityActive && this.focusedHarvestPOI)
		{
			float num = Vector3.Distance(this.focusedHarvestPOI.transform.position, GameManager.Instance.Player.transform.position);
			float num2 = Mathf.InverseLerp(this.closeThreshold, this.farThreshold, num);
			float num3 = Mathf.Lerp(this.scaleMax, this.scaleMin, num2);
			this.container.transform.localScale = new Vector3(num3, num3, 1f);
		}
	}

	private void RefreshUI()
	{
		if (!this.focusedHarvestPOI)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.togglePinAction }, ActionLayer.BASE);
			this.animator.SetBool("showing", false);
			return;
		}
		HarvestableItemData activeFirstHarvestableItem = this.focusedHarvestPOI.Harvestable.GetActiveFirstHarvestableItem();
		if (!activeFirstHarvestableItem)
		{
			this.animator.SetBool("showing", false);
			return;
		}
		if (GameManager.Instance.SaveData.GetCaughtCountById(activeFirstHarvestableItem.id) > 0)
		{
			this.itemNameString.StringReference = activeFirstHarvestableItem.itemNameKey;
		}
		else
		{
			this.itemNameString.StringReference = this.obscuredString;
		}
		this.itemNameString.StringReference.RefreshString();
		this.itemImage.sprite = ((activeFirstHarvestableItem.itemSubtype == ItemSubtype.TRINKET) ? this.hiddenItemSprite : activeFirstHarvestableItem.sprite);
		this.harvestableTypeTagUI.Init(activeFirstHarvestableItem.harvestableType, activeFirstHarvestableItem.requiresAdvancedEquipment);
		this.invalidEquipmentImage.gameObject.SetActive(!GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(activeFirstHarvestableItem.harvestableType, activeFirstHarvestableItem.requiresAdvancedEquipment));
		float num = Vector3.Distance(this.focusedHarvestPOI.transform.position, GameManager.Instance.Player.transform.position);
		float num2 = Mathf.InverseLerp(this.closeThreshold, this.farThreshold, num);
		float num3 = Mathf.Lerp(this.scaleMax, this.scaleMin, num2);
		this.container.transform.localScale = new Vector3(num3, num3, 1f);
		if (this.hasAdvancedSpyglass)
		{
			this.RefreshPinImage();
		}
		this.advancedBehaviourContainer.SetActive(this.hasAdvancedSpyglass && !this.focusedHarvestPOI.IsBaitPOI);
		this.container.SetActive(true);
		this.animator.SetBool("showing", true);
		if (this.hasAdvancedSpyglass)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.togglePinAction }, ActionLayer.BASE);
		}
		GameManager.Instance.SaveData.SetHasSpiedHarvestCategory(activeFirstHarvestableItem.harvestableType, true);
		GameManager.Instance.AchievementManager.EvaluateAchievement(DredgeAchievementId.ABILITY_SPYGLASS);
	}

	private void OnPressTogglePin()
	{
		if (this.focusedHarvestPOIID == null || this.focusedHarvestPOIID == "")
		{
			return;
		}
		if (this.focusedHarvestPOI)
		{
			if (this.isHarvestPOICurrentlyPinned)
			{
				GameManager.Instance.SaveData.RemoveHarvestPOIMarker(this.focusedHarvestPOIID);
				this.RemoveHarvestPOIMarker(this.focusedHarvestPOIID);
				this.isHarvestPOICurrentlyPinned = false;
				this.conflictingStamp = null;
			}
			else if ((float)GameManager.Instance.SaveData.GetTotalUserPlacedMapMarkers() >= GameManager.Instance.GameConfigData.MaxNumMapMarkers)
			{
				GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.map-marker-limit");
			}
			else
			{
				this.isHarvestPOICurrentlyPinned = true;
				GameManager.Instance.SaveData.AddHarvestPOIMarker(this.focusedHarvestPOIID);
				this.conflictingStamp = this.AddHarvestPOIMarker(this.focusedHarvestPOIID);
				GameManager.Instance.AudioPlayer.PlaySFX(this.advancedSpyglassAddMarker, AudioLayer.SFX_UI, 1f, 1f);
				if (this.conflictingStamp)
				{
					this.conflictingStamp.gameObject.SetActive(false);
				}
			}
			this.RefreshPinImage();
		}
	}

	private void RefreshPinImage()
	{
		this.togglePinImage.sprite = (this.isHarvestPOICurrentlyPinned ? this.unpinImage : this.pinImage);
	}

	private void LateUpdate()
	{
		if (this.focusedHarvestPOI)
		{
			this.container.transform.position = Camera.main.WorldToScreenPoint(this.focusedHarvestPOI.transform.position);
		}
	}

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject advancedBehaviourContainer;

	[SerializeField]
	private ControlPromptIcon togglePinControlPrompt;

	[SerializeField]
	private Image togglePinImage;

	[SerializeField]
	private Sprite pinImage;

	[SerializeField]
	private Sprite unpinImage;

	[SerializeField]
	private GameObject crosshairContainer;

	[SerializeField]
	private Image crosshairXImage;

	[SerializeField]
	private Image crosshairYImage;

	[SerializeField]
	private Sprite basicCrosshairImage;

	[SerializeField]
	private Sprite advancedCrosshairImage;

	[SerializeField]
	private LocalizeStringEvent itemNameString;

	[SerializeField]
	private Image itemImage;

	[SerializeField]
	private Image invalidEquipmentImage;

	[SerializeField]
	private Sprite hiddenItemSprite;

	[SerializeField]
	private HarvestableTypeTagUI harvestableTypeTagUI;

	[SerializeField]
	private AbilityData spyglassAbilityData;

	[SerializeField]
	private AbilityData advancedSpyglassAbilityData;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private LocalizedString obscuredString;

	[SerializeField]
	private float scaleMin;

	[SerializeField]
	private float scaleMax;

	[SerializeField]
	private float closeThreshold;

	[SerializeField]
	private float farThreshold;

	[SerializeField]
	private CanvasGroup stampCanvasGroup;

	[SerializeField]
	private Transform stampContainer;

	[SerializeField]
	private GameObject stampPrefab;

	[SerializeField]
	private MapStampConfig config;

	[SerializeField]
	private AssetReference advancedSpyglassAddMarker;

	[SerializeField]
	private AssetReference advancedSpyglassRemoveMarker;

	private bool isAbilityActive;

	private DredgePlayerActionPress togglePinAction;

	private List<SpyglassMapStamp> instantiatedStamps = new List<SpyglassMapStamp>();

	private HarvestPOI focusedHarvestPOI;

	private string focusedHarvestPOIID;

	private bool hasAdvancedSpyglass;

	private bool isHarvestPOICurrentlyPinned;

	private SpyglassMapStamp conflictingStamp;
}
