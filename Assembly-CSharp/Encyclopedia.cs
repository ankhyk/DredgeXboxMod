using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class Encyclopedia : MonoBehaviour
{
	private void Awake()
	{
		this.currentFishList = this.allFish;
		this.prevPageAction = new DredgePlayerActionPress("prompt.tab-left", GameManager.Instance.Input.Controls.TabLeft);
		DredgePlayerActionPress dredgePlayerActionPress = this.prevPageAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnPrevPageActionPressed));
		this.prevPageAction.evaluateWhenPaused = true;
		this.prevPageControlPromptIcon.Init(this.prevPageAction, GameManager.Instance.Input.Controls.TabLeft);
		this.nextPageAction = new DredgePlayerActionPress("prompt.tab-right", GameManager.Instance.Input.Controls.TabRight);
		DredgePlayerActionPress dredgePlayerActionPress2 = this.nextPageAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnNextPageActionPressed));
		this.nextPageAction.evaluateWhenPaused = true;
		this.nextPageControlPromptIcon.Init(this.nextPageAction, GameManager.Instance.Input.Controls.TabRight);
		EncyclopediaPage encyclopediaPage = this.leftPage;
		encyclopediaPage.PageLinkRequest = (Action<FishItemData>)Delegate.Combine(encyclopediaPage.PageLinkRequest, new Action<FishItemData>(this.OnPageLinkRequested));
		EncyclopediaPage encyclopediaPage2 = this.rightPage;
		encyclopediaPage2.PageLinkRequest = (Action<FishItemData>)Delegate.Combine(encyclopediaPage2.PageLinkRequest, new Action<FishItemData>(this.OnPageLinkRequested));
		if (GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1))
		{
			this.iceHarvestTypeButton.gameObject.SetActive(true);
			this.harvestTypes.Add(HarvestableType.ICE);
			this.harvestTypeButtons.Add(this.iceHarvestTypeButton);
			this.harvestTypeTags.Add(this.iceHarvestTypeTag);
			this.dlc1ZoneButton.gameObject.SetActive(true);
			this.zones.Add(ZoneEnum.PALE_REACH);
			this.zoneButtons.Insert(this.zoneButtons.Count - 2, this.dlc1ZoneButton);
			this.SetZoneButtonPositions();
			this.numZoneButtons++;
			this.exoticTabIndex++;
			this.aberrationTabIndex++;
		}
		else
		{
			this.currentFishList.RemoveAll((FishItemData f) => f.entitlementsRequired.Contains(Entitlement.DLC_1));
		}
		if (!GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2))
		{
			this.currentFishList.RemoveAll((FishItemData f) => f.entitlementsRequired.Contains(Entitlement.DLC_2));
		}
		for (int i = 0; i < this.harvestTypes.Count; i++)
		{
			this.harvestTypeTags[i].Init(this.harvestTypes[i], false);
		}
		this.ResetHarvestableTypesFilter();
	}

	private void SetZoneButtonPositions()
	{
		float num = 50f;
		float num2 = 5f;
		float num3 = num + num2;
		for (int i = 0; i < this.zoneButtons.Count; i++)
		{
			RectTransform component = this.zoneButtons[i].GetComponent<RectTransform>();
			component.anchoredPosition = new Vector3(component.anchoredPosition.x, -(num3 * (float)i));
		}
	}

	private void OnEnable()
	{
		this.canShowAberrations = true;
		this.zoneButtons[this.exoticTabIndex].gameObject.SetActive(this.canShowAberrations);
		for (int i = 0; i < this.zoneButtons.Count; i++)
		{
			int capture2 = i;
			BasicButtonWrapper basicButtonWrapper = this.zoneButtons[i];
			basicButtonWrapper.OnSubmitAction = (Action)Delegate.Combine(basicButtonWrapper.OnSubmitAction, new Action(delegate
			{
				this.OnZoneButtonClicked(capture2);
			}));
			RectTransform rectTransform = this.zoneButtons[i].transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, rectTransform.anchoredPosition.y);
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = rectTransform.DOAnchorPosX(0f, 0.35f, false).SetUpdate(true);
			tweenerCore.SetDelay(0.1f * (float)i);
			tweenerCore.SetEase(Ease.OutExpo);
		}
		for (int j = 0; j < this.harvestTypeButtons.Count; j++)
		{
			HarvestableType capture = this.harvestTypes[j];
			BasicButtonWrapper basicButtonWrapper2 = this.harvestTypeButtons[j];
			basicButtonWrapper2.OnSubmitAction = (Action)Delegate.Combine(basicButtonWrapper2.OnSubmitAction, new Action(delegate
			{
				this.OnHarvestTypeButtonClicked(capture);
			}));
			RectTransform rectTransform2 = this.harvestTypeButtons[j].transform as RectTransform;
			rectTransform2.anchoredPosition = new Vector2(rectTransform2.rect.width, rectTransform2.anchoredPosition.y);
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore2 = rectTransform2.DOAnchorPosX(0f, 0.35f, false).SetUpdate(true);
			tweenerCore2.SetDelay(0.1f * (float)j);
			tweenerCore2.SetEase(Ease.OutExpo);
		}
		string lastCaughtSpecies = GameManager.Instance.SaveData.LastUnseenCaughtSpecies;
		if (lastCaughtSpecies != "")
		{
			this.ResetHarvestableTypesFilter();
			this.FilterFishList();
			this.currentIndex = this.allFish.FindIndex((FishItemData itemData) => itemData.id == lastCaughtSpecies);
		}
		else
		{
			this.FilterFishList();
		}
		this.RefreshUI();
	}

	private void ResetHarvestableTypesFilter()
	{
		this.filteredTypes.Clear();
		for (int i = 0; i < this.harvestTypes.Count; i++)
		{
			this.filteredTypes.Add(this.harvestTypes[i]);
		}
		this.RefreshHarvestTypeButtons();
	}

	private void RefreshHarvestTypeButtons()
	{
		for (int i = 0; i < this.harvestTypeButtons.Count; i++)
		{
			this.harvestTypeButtons[i].Button.colors = (this.filteredTypes.Contains(this.harvestTypes[i]) ? this.harvestTypeActiveColorBlock : this.harvestTypeInactiveColorBlock);
		}
		this.filteredTypes.Sort();
		string text = "";
		for (int j = 0; j < this.filteredTypes.Count; j++)
		{
			text += string.Format("{0}, ", this.filteredTypes[j]);
		}
	}

	private void FilterFishList()
	{
		FishItemData fishItemData = this.currentFishList[this.currentIndex];
		this.currentFishList = this.allFish.Where((FishItemData fish) => this.filteredTypes.Count == this.harvestTypes.Count || this.filteredTypes.Contains(fish.harvestableType)).ToList<FishItemData>();
		this.currentIndex = this.currentFishList.IndexOf(fishItemData);
		string text = "";
		for (int i = 0; i < this.currentFishList.Count; i++)
		{
			text = text + this.currentFishList[i].id + ", ";
		}
	}

	private void OnPageLinkRequested(FishItemData fishItemData)
	{
		int num = this.currentFishList.IndexOf(fishItemData);
		this.currentIndex = num;
		this.RefreshUI();
		base.StartCoroutine(this.SelectFirstZoneButton());
	}

	private IEnumerator SelectFirstZoneButton()
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(this.zoneButtons[0].gameObject);
			this.zoneButtons[0].ManualOnSelect();
		}
		yield break;
	}

	private void OnZoneButtonClicked(int i)
	{
		if (i <= this.numZoneButtons)
		{
			ZoneEnum zone = this.zones[i];
			this.currentIndex = this.currentFishList.FindIndex((FishItemData fish) => fish.zonesFoundIn.HasFlag(zone));
		}
		else if (i == this.exoticTabIndex)
		{
			this.currentIndex = this.currentFishList.FindIndex((FishItemData fish) => fish.LocationHiddenUntilCaught);
		}
		else if (i == this.aberrationTabIndex)
		{
			this.currentIndex = this.currentFishList.FindIndex((FishItemData fish) => fish.IsAberration);
		}
		this.RefreshUI();
	}

	private void OnHarvestTypeButtonClicked(HarvestableType htype)
	{
		if (this.filteredTypes.Count == this.harvestTypes.Count)
		{
			this.filteredTypes.Clear();
		}
		if (this.filteredTypes.Contains(htype))
		{
			this.filteredTypes.Remove(htype);
		}
		else
		{
			this.filteredTypes.Add(htype);
		}
		if (this.filteredTypes.Count == 0)
		{
			this.ResetHarvestableTypesFilter();
		}
		else
		{
			this.RefreshHarvestTypeButtons();
		}
		this.FilterFishList();
		this.RefreshUI();
	}

	private void OnDisable()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.prevPageAction, this.nextPageAction }, ActionLayer.POPUP_WINDOW);
		this.zoneButtons.ForEach(delegate(BasicButtonWrapper b)
		{
			b.OnSubmitAction = null;
			DOTween.Kill(b.transform as RectTransform, false);
		});
		this.harvestTypeButtons.ForEach(delegate(BasicButtonWrapper b)
		{
			b.OnSubmitAction = null;
			DOTween.Kill(b.transform as RectTransform, false);
		});
	}

	public void OnPrevPageActionPressed()
	{
		this.currentIndex -= 2;
		base.StartCoroutine(this.SelectFirstZoneButton());
		this.RefreshUI();
		GameManager.Instance.AudioPlayer.PlaySFX(this.turnSFX.PickRandom<AssetReference>(), AudioLayer.SFX_UI, 1f, 1f);
	}

	public void OnNextPageActionPressed()
	{
		this.currentIndex += 2;
		base.StartCoroutine(this.SelectFirstZoneButton());
		this.RefreshUI();
		GameManager.Instance.AudioPlayer.PlaySFX(this.turnSFX.PickRandom<AssetReference>(), AudioLayer.SFX_UI, 1f, 1f);
	}

	private void RefreshUI()
	{
		GameManager.Instance.SaveData.LastUnseenCaughtSpecies = "";
		GameEvents.Instance.TriggerHasUnseenItemsChanged();
		this.totalItemCount = this.currentFishList.Count;
		this.totalPageCount = Mathf.CeilToInt((float)this.totalItemCount * 0.5f);
		this.totalNumToDiscover = 0;
		this.discoveredCount = 0;
		this.currentFishList.ForEach(delegate(FishItemData fishData)
		{
			this.totalNumToDiscover++;
			if (GameManager.Instance.SaveData.GetCaughtCountById(fishData.id) > 0)
			{
				this.discoveredCount++;
			}
		});
		if (this.currentIndex % 2 == 1)
		{
			this.currentIndex--;
		}
		this.currentIndex = Mathf.Clamp(this.currentIndex, 0, this.currentFishList.Count - 1);
		float num = (float)this.currentIndex;
		int num2 = this.currentIndex + 1;
		this.pageCountLocalizedText.enabled = false;
		int num3 = Mathf.CeilToInt((num + (float)1) * 0.5f);
		this.pageCountLocalizedText.StringReference.Arguments = new object[] { num3, this.totalPageCount };
		this.pageCountLocalizedText.StringReference.RefreshString();
		this.pageCountLocalizedText.enabled = true;
		this.discoveryCountLocalizedText.enabled = false;
		this.discoveryCountLocalizedText.StringReference.Arguments = new object[] { this.discoveredCount, this.totalNumToDiscover };
		this.discoveryCountLocalizedText.StringReference.RefreshString();
		this.discoveryCountLocalizedText.enabled = true;
		FishItemData fishItemData = this.currentFishList[this.currentIndex];
		int num4 = this.allFish.IndexOf(fishItemData);
		this.leftPage.RefreshUI(fishItemData, num4);
		if (num2 <= this.currentFishList.Count - 1)
		{
			FishItemData fishItemData2 = this.currentFishList[this.currentIndex + 1];
			int num5 = this.allFish.IndexOf(fishItemData2);
			this.rightPage.RefreshUI(fishItemData2, num5);
		}
		else
		{
			this.rightPage.RefreshUI(null, -1);
		}
		this.RefreshZoneButtonPopoutPosition();
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.prevPageAction, this.nextPageAction }, ActionLayer.POPUP_WINDOW);
		if (this.currentIndex > 0)
		{
			this.prevPageGroup.SetActive(true);
			this.prevPageAction.Enable();
		}
		else
		{
			this.prevPageGroup.SetActive(false);
			this.prevPageAction.Disable(true);
		}
		if (num2 < this.currentFishList.Count - 1)
		{
			this.nextPageGroup.SetActive(true);
			this.nextPageAction.Enable();
			return;
		}
		this.nextPageGroup.SetActive(false);
		this.nextPageAction.Disable(true);
	}

	private int GetZoneButtonIndexByFishData(FishItemData currentFishData)
	{
		if (currentFishData.IsAberration)
		{
			return this.aberrationTabIndex;
		}
		if (currentFishData.LocationHiddenUntilCaught)
		{
			return this.exoticTabIndex;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.THE_MARROWS))
		{
			return 0;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.GALE_CLIFFS))
		{
			return 1;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.STELLAR_BASIN))
		{
			return 2;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.TWISTED_STRAND))
		{
			return 3;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.DEVILS_SPINE))
		{
			return 4;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.OPEN_OCEAN))
		{
			return 5;
		}
		if (currentFishData.zonesFoundIn.HasFlag(ZoneEnum.PALE_REACH))
		{
			return 6;
		}
		return -1;
	}

	private void RefreshZoneButtonPopoutPosition()
	{
		int zoneButtonIndexByFishData = this.GetZoneButtonIndexByFishData(this.currentFishList[this.currentIndex]);
		int num = zoneButtonIndexByFishData;
		if (this.currentIndex + 1 < this.currentFishList.Count)
		{
			num = this.GetZoneButtonIndexByFishData(this.currentFishList[this.currentIndex + 1]);
		}
		for (int i = 0; i < this.zoneButtons.Count; i++)
		{
			RectTransform rectTransform = this.zoneButtons[i].gameObject.transform as RectTransform;
			rectTransform.sizeDelta = new Vector2((zoneButtonIndexByFishData == i || num == i) ? this.zoneButtonWidthSelected : this.zoneButtonWidthIdle, rectTransform.sizeDelta.y);
		}
	}

	[SerializeField]
	private AssetLabelReference itemDataAssetLabelReference;

	[SerializeField]
	private List<AssetReference> turnSFX;

	[SerializeField]
	private List<FishItemData> allFish;

	[SerializeField]
	private EncyclopediaPage leftPage;

	[SerializeField]
	private EncyclopediaPage rightPage;

	[SerializeField]
	private LocalizeStringEvent pageCountLocalizedText;

	[SerializeField]
	private LocalizeStringEvent discoveryCountLocalizedText;

	[SerializeField]
	private GameObject prevPageGroup;

	[SerializeField]
	private GameObject nextPageGroup;

	[SerializeField]
	private ControlPromptIcon prevPageControlPromptIcon;

	[SerializeField]
	private ControlPromptIcon nextPageControlPromptIcon;

	[SerializeField]
	private float zoneButtonWidthIdle;

	[SerializeField]
	private float zoneButtonWidthSelected;

	[SerializeField]
	private List<BasicButtonWrapper> zoneButtons;

	[SerializeField]
	private List<BasicButtonWrapper> harvestTypeButtons;

	[SerializeField]
	private List<HarvestableTypeTagUI> harvestTypeTags;

	[Header("DLC1")]
	[SerializeField]
	private BasicButtonWrapper iceHarvestTypeButton;

	[SerializeField]
	private HarvestableTypeTagUI iceHarvestTypeTag;

	[SerializeField]
	private BasicButtonWrapper dlc1ZoneButton;

	[Header("Harvest Type Active Color Block")]
	[SerializeField]
	private ColorBlock harvestTypeActiveColorBlock;

	[Header("Harvest Type Inactive Color Block")]
	[SerializeField]
	private ColorBlock harvestTypeInactiveColorBlock;

	private List<FishItemData> currentFishList;

	private List<ZoneEnum> zones = new List<ZoneEnum>
	{
		ZoneEnum.THE_MARROWS,
		ZoneEnum.GALE_CLIFFS,
		ZoneEnum.STELLAR_BASIN,
		ZoneEnum.TWISTED_STRAND,
		ZoneEnum.DEVILS_SPINE,
		ZoneEnum.OPEN_OCEAN
	};

	private List<HarvestableType> harvestTypes = new List<HarvestableType>
	{
		HarvestableType.COASTAL,
		HarvestableType.SHALLOW,
		HarvestableType.OCEANIC,
		HarvestableType.ABYSSAL,
		HarvestableType.HADAL,
		HarvestableType.MANGROVE,
		HarvestableType.VOLCANIC,
		HarvestableType.CRAB
	};

	private List<HarvestableType> filteredTypes = new List<HarvestableType>();

	private DredgePlayerActionPress prevPageAction;

	private DredgePlayerActionPress nextPageAction;

	private int currentIndex;

	private int numZoneButtons = 5;

	private int exoticTabIndex = 6;

	private int aberrationTabIndex = 7;

	private int totalItemCount;

	private int totalPageCount;

	private int discoveredCount;

	private int totalNumToDiscover;

	private bool canShowAberrations;
}
