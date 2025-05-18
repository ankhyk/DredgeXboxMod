using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class EncyclopediaPage : SerializedMonoBehaviour
{
	public void RefreshUI(FishItemData itemData, int index)
	{
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = this.canvasGroup.DOFade(0f, this.fadeDurationSec);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(delegate
		{
			this.PopulatePage(itemData, index);
			this.fadeTween = null;
		});
	}

	private void PopulatePage(FishItemData itemData, int index)
	{
		if (itemData == null)
		{
			this.pageContainer.SetActive(false);
			return;
		}
		this.pageContainer.SetActive(true);
		Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		Color color2 = this.harvestTypeTagConfig.textColorLookup[itemData.harvestableType];
		int caughtCountById = GameManager.Instance.SaveData.GetCaughtCountById(itemData.id);
		bool flag = caughtCountById > 0;
		string text = (flag ? itemData.itemNameKey.GetLocalizedString() : "???");
		this.itemNameText.text = string.Format("#{0} {1}", index + 1, text);
		this.itemImage.sprite = itemData.sprite;
		this.itemImage.color = (flag ? this.itemImageColorIdentified : this.itemImageColorUnidentified);
		this.itemImage.material = ((itemData.UseIntenseAberratedUIShader && flag) ? this.intenselyAberratedUIMaterial : null);
		this.itemImageGrid.sizeDelta = new Vector2((float)itemData.GetWidth() * 128f, (float)itemData.GetHeight() * 128f);
		this.undiscoveredItemImage.sprite = (itemData.IsAberration ? this.undiscoveredAberrationSprite : this.undiscoveredRegularSprite);
		this.undiscoveredItemImage.color = (itemData.IsAberration ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
		this.undiscoveredItemImage.gameObject.SetActive(!flag);
		if (this.harvestTypeTagConfig.colorLookup.ContainsKey(itemData.harvestableType))
		{
			color = this.harvestTypeTagConfig.colorLookup[itemData.harvestableType];
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[EncyclopediaPage] RefreshUI({0}) couldn't find color in config.", itemData.harvestableType));
		}
		this.harvestTypeBackplate.color = color;
		if (itemData.harvestableType == HarvestableType.NONE || itemData.harvestableType == HarvestableType.CRAB)
		{
			string depthString = itemData.GetDepthString();
			this.harvestTypeLocalizedText.enabled = false;
			this.harvestTypeLocalizedText.StringReference.SetReference(LanguageManager.STRING_TABLE, "encyclopedia.depth");
			this.harvestTypeLocalizedText.StringReference.Arguments = new object[] { depthString };
			this.harvestTypeLocalizedText.enabled = true;
		}
		else if (this.harvestTypeTagConfig.stringLookup.ContainsKey(itemData.harvestableType))
		{
			this.harvestTypeLocalizedText.StringReference.SetReference(LanguageManager.STRING_TABLE, this.harvestTypeTagConfig.stringLookup[itemData.harvestableType]);
		}
		else
		{
			CustomDebug.EditorLogError(string.Format("[EncyclopediaPage] RefreshUI({0}) couldn't find string in config.", itemData.harvestableType));
		}
		this.harvestTypeText.color = color2;
		if (caughtCountById > 0)
		{
			this.caughtCounterLocalizedText.enabled = false;
			this.caughtCounterLocalizedText.StringReference.SetReference(LanguageManager.STRING_TABLE, "encyclopedia.caught-some");
			this.caughtCounterLocalizedText.StringReference.Arguments = new object[] { caughtCountById };
			this.caughtCounterLocalizedText.enabled = true;
			this.caughtCounterText.color = Color.black;
			this.caughtCounterDiamond.color = Color.black;
		}
		else
		{
			this.caughtCounterLocalizedText.enabled = false;
			this.caughtCounterLocalizedText.StringReference.SetReference(LanguageManager.STRING_TABLE, "encyclopedia.caught-none");
			this.caughtCounterLocalizedText.enabled = true;
			this.caughtCounterText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			this.caughtCounterDiamond.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
		string text2 = (flag ? itemData.itemDescriptionKey.GetLocalizedString() : "???");
		this.descriptionLocalizedText.text = text2;
		this.descriptionLocalizedText.color = color2;
		this.descriptionBackplate.color = color;
		LocalizedString localizedString = this.encyclopediaConfig.zoneStrings[itemData.zonesFoundIn];
		Sprite sprite = this.encyclopediaConfig.zoneIconSprites[itemData.zonesFoundIn];
		if (itemData.LocationHiddenUntilCaught && caughtCountById == 0)
		{
			localizedString = this.encyclopediaConfig.zoneStrings[ZoneEnum.NONE];
			sprite = this.encyclopediaConfig.zoneIconSprites[ZoneEnum.NONE];
		}
		this.zoneImage.sprite = sprite;
		this.localizedZoneText.StringReference = localizedString;
		this.zoneText.HighlightedBackplateColor = color;
		this.zoneText.HighlightedTextColor = color2;
		this.zoneText.SetHighlighted(true);
		foreach (KeyValuePair<ItemSubtype, ToggleableTextWithBackplate> keyValuePair in this.catchTypeTexts)
		{
			keyValuePair.Value.HighlightedTextColor = color2;
			keyValuePair.Value.HighlightedBackplateColor = color;
			if (keyValuePair.Key == ItemSubtype.ROD)
			{
				keyValuePair.Value.SetHighlighted(itemData.canBeCaughtByRod);
			}
			if (keyValuePair.Key == ItemSubtype.POT)
			{
				keyValuePair.Value.SetHighlighted(itemData.canBeCaughtByPot);
			}
			if (keyValuePair.Key == ItemSubtype.NET)
			{
				keyValuePair.Value.SetHighlighted(itemData.canBeCaughtByNet);
			}
		}
		if (flag)
		{
			float largestFishRecordById = GameManager.Instance.SaveData.GetLargestFishRecordById(itemData.id);
			this.largestText.text = GameManager.Instance.ItemManager.GetFormattedFishSizeString(largestFishRecordById, itemData);
			this.trophyIcon.color = ((largestFishRecordById > GameManager.Instance.GameConfigData.TrophyMaxSize) ? (GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) * this.trophyIconColorMultiplier) : Color.black);
			this.trophyIcon.color = new Color(this.trophyIcon.color.r, this.trophyIcon.color.g, this.trophyIcon.color.b, 1f);
		}
		else
		{
			this.largestText.text = "-";
			this.trophyIcon.color = Color.black;
		}
		bool flag2 = GameManager.Instance.DialogueRunner.GetNumItemsSoldById(itemData.id) > 0;
		this.valueText.text = (flag2 ? itemData.value.ToString("n2", LocalizationSettings.SelectedLocale.Formatter) : "???");
		this.dayToggler.HighlightedBackplateColor = color;
		this.dayToggler.HighlightedImageColor = color2;
		this.nightToggler.HighlightedBackplateColor = color;
		this.nightToggler.HighlightedImageColor = color2;
		this.dayToggler.SetHighlighted(itemData.Day);
		this.nightToggler.SetHighlighted(itemData.Night);
		this.aberrationInfos.ForEach(delegate(AberrationInfoUI i)
		{
			i.gameObject.SetActive(false);
		});
		this.aberrationsContainer.SetActive(false);
		if (itemData.IsAberration)
		{
			FishItemData aberrationData2 = itemData.NonAberrationParent;
			if (aberrationData2 != null)
			{
				this.aberrationInfos[0].BasicButtonWrapper.OnSubmitAction = delegate
				{
					Action<FishItemData> pageLinkRequest = this.PageLinkRequest;
					if (pageLinkRequest == null)
					{
						return;
					}
					pageLinkRequest(aberrationData2);
				};
				this.aberrationInfos[0].SetData(aberrationData2);
				this.aberrationInfos[0].gameObject.SetActive(true);
				this.aberrationsContainer.SetActive(true);
				this.aberrationsHeaderText.StringReference = this.aberrationOfLocalizedString;
			}
		}
		else
		{
			int num = itemData.Aberrations.Count(delegate(FishItemData i)
			{
				if (i.entitlementsRequired.Count != 0 && i.entitlementsRequired[0] != Entitlement.NONE)
				{
					return i.entitlementsRequired.All((Entitlement e) => GameManager.Instance.EntitlementManager.GetHasEntitlement(e));
				}
				return true;
			});
			if (num > 0)
			{
				for (int j = 0; j < num; j++)
				{
					if (num > j)
					{
						FishItemData aberrationData = itemData.Aberrations[j];
						this.aberrationInfos[j].BasicButtonWrapper.OnSubmitAction = delegate
						{
							Action<FishItemData> pageLinkRequest2 = this.PageLinkRequest;
							if (pageLinkRequest2 == null)
							{
								return;
							}
							pageLinkRequest2(aberrationData);
						};
						this.aberrationInfos[j].SetData(aberrationData);
						this.aberrationInfos[j].gameObject.SetActive(true);
					}
				}
				this.aberrationsHeaderText.StringReference = this.aberrationsLocalizedString;
				this.aberrationsContainer.SetActive(true);
			}
			else
			{
				this.aberrationsContainer.SetActive(false);
			}
		}
		bool flag3 = itemData.entitlementsRequired.Contains(Entitlement.DLC_2);
		this.advancedTypeBackground.SetActive(flag3);
		this.advancedTypeTag.SetActive(itemData.requiresAdvancedEquipment);
		this.rodCatchTypeText.StringReference = (itemData.requiresAdvancedEquipment ? this.advancedRodString : this.regularRodString);
		this.fadeTween = this.canvasGroup.DOFade(1f, this.fadeDurationSec);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(delegate
		{
			this.fadeTween = null;
		});
	}

	[Header("Config")]
	[SerializeField]
	private float fadeDurationSec;

	[SerializeField]
	private HarvestTypeTagConfig harvestTypeTagConfig;

	[SerializeField]
	private EncyclopediaConfig encyclopediaConfig;

	[SerializeField]
	private Color itemImageColorIdentified;

	[SerializeField]
	private Color itemImageColorUnidentified;

	[SerializeField]
	private float trophyThreshold;

	[SerializeField]
	protected float trophyIconColorMultiplier;

	[Header("References")]
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private GameObject pageContainer;

	[SerializeField]
	private TextMeshProUGUI itemNameText;

	[SerializeField]
	private Image itemImage;

	[SerializeField]
	private RectTransform itemImageGrid;

	[SerializeField]
	private Image undiscoveredItemImage;

	[SerializeField]
	private Sprite undiscoveredAberrationSprite;

	[SerializeField]
	private Sprite undiscoveredRegularSprite;

	[SerializeField]
	private TextMeshProUGUI harvestTypeText;

	[SerializeField]
	private LocalizeStringEvent harvestTypeLocalizedText;

	[SerializeField]
	private Image harvestTypeBackplate;

	[SerializeField]
	private LocalizeStringEvent caughtCounterLocalizedText;

	[SerializeField]
	private TextMeshProUGUI caughtCounterText;

	[SerializeField]
	private Image caughtCounterDiamond;

	[SerializeField]
	private Image descriptionBackplate;

	[SerializeField]
	private TextMeshProUGUI descriptionLocalizedText;

	[SerializeField]
	private LocalizeStringEvent localizedZoneText;

	[SerializeField]
	private ToggleableTextWithBackplate zoneText;

	[SerializeField]
	private Image zoneImage;

	[SerializeField]
	private TextMeshProUGUI largestText;

	[SerializeField]
	private Image trophyIcon;

	[SerializeField]
	private TextMeshProUGUI valueText;

	[SerializeField]
	private ToggleableImageWithBackplate dayToggler;

	[SerializeField]
	private ToggleableImageWithBackplate nightToggler;

	[SerializeField]
	private Dictionary<ItemSubtype, ToggleableTextWithBackplate> catchTypeTexts;

	[SerializeField]
	private GameObject advancedTypeBackground;

	[SerializeField]
	private GameObject advancedTypeTag;

	[SerializeField]
	private LocalizeStringEvent rodCatchTypeText;

	[SerializeField]
	private LocalizedString regularRodString;

	[SerializeField]
	private LocalizedString advancedRodString;

	[SerializeField]
	private LocalizeStringEvent aberrationsHeaderText;

	[SerializeField]
	private LocalizedString aberrationOfLocalizedString;

	[SerializeField]
	private LocalizedString aberrationsLocalizedString;

	[SerializeField]
	private GameObject aberrationsContainer;

	[SerializeField]
	private List<AberrationInfoUI> aberrationInfos;

	[SerializeField]
	public Material intenselyAberratedUIMaterial;

	[HideInInspector]
	public Action<FishItemData> PageLinkRequest;

	private Tweener fadeTween;
}
