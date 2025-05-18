using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BannerUI : MonoBehaviour
{
	public bool isShowing { get; set; }

	public bool isHiding { get; set; }

	private void Awake()
	{
		this.isHiding = false;
		this.isShowing = false;
	}

	public void ShowFishDiscovered(FishItemData fishItemData)
	{
		this.titleTextLocalized.enabled = false;
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleTextLocalized.StringReference.SetReference(fishItemData.itemNameKey.TableReference, fishItemData.itemNameKey.TableEntryReference);
		this.titleTextLocalized.RefreshString();
		this.titleTextLocalized.enabled = true;
		this.subtitleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.fish-discovered.subtitle");
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		if (fishItemData.GetSize() == 1)
		{
			this.image.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		else
		{
			this.image.transform.localScale = Vector3.one;
		}
		this.image.sprite = fishItemData.sprite;
		GameManager.Instance.AudioPlayer.PlaySFX(fishItemData.IsAberration ? this.aberrationSFX : this.fishSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowFishTrophy(FishItemData fishItemData, float size)
	{
		this.titleTextLocalized.enabled = false;
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.trophy-fish.title");
		this.titleTextLocalized.RefreshString();
		this.titleTextLocalized.enabled = true;
		string text = "<color=#FFD104><sprite name=\"TrophyIcon\" tint=1></color>";
		text += GameManager.Instance.ItemManager.GetFormattedFishSizeString(size, fishItemData);
		this.subtitleTextLocalized.enabled = false;
		this.subtitleText.text = text;
		if (fishItemData.GetSize() == 1)
		{
			this.image.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		else
		{
			this.image.transform.localScale = Vector3.one;
		}
		this.image.sprite = fishItemData.sprite;
		GameManager.Instance.AudioPlayer.PlaySFX(this.regularSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowRelic(SpatialItemData spatialItemData)
	{
		this.titleTextLocalized.enabled = false;
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL);
		this.titleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.relic-discovered.title");
		this.titleTextLocalized.RefreshString();
		this.titleTextLocalized.enabled = true;
		this.subtitleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.relic-discovered.subtitle");
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		this.image.transform.localScale = Vector3.one;
		this.image.sprite = spatialItemData.sprite;
		GameManager.Instance.AudioPlayer.PlaySFX(this.regularSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowAbility(AbilityData ability)
	{
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleTextLocalized.enabled = false;
		this.titleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.ability-unlocked.title");
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(ability.nameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		this.titleTextLocalized.StringReference.Arguments = new object[] { string.Concat(new string[]
		{
			"<color=#",
			GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.EMPHASIS),
			">",
			localizedString,
			"</color>"
		}) };
		this.titleTextLocalized.RefreshString();
		this.titleTextLocalized.enabled = true;
		this.subtitleTextLocalized.StringReference.SetReference(ability.shortDescriptionKey.TableReference, ability.shortDescriptionKey.TableEntryReference);
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		this.image.transform.localScale = Vector3.one;
		this.image.sprite = ability.icon;
		GameManager.Instance.AudioPlayer.PlaySFX(this.regularSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowResearch(SpatialItemData spatialItemData)
	{
		this.titleTextLocalized.enabled = false;
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.research-complete.title");
		this.titleTextLocalized.RefreshString();
		this.titleTextLocalized.enabled = true;
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(spatialItemData.itemNameKey.TableReference, spatialItemData.itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		this.subtitleTextLocalized.enabled = false;
		string text = "notification.research-complete.subtitle";
		if (spatialItemData.ResearchIsForRecipe)
		{
			text = "notification.craftable-research-complete.subtitle";
		}
		this.subtitleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, text);
		this.subtitleTextLocalized.StringReference.Arguments = new object[] { string.Concat(new string[]
		{
			"<color=#",
			GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.EMPHASIS),
			">",
			localizedString,
			"</color>"
		}) };
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		if (spatialItemData.GetSize() == 1)
		{
			this.image.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		}
		else
		{
			this.image.transform.localScale = Vector3.one;
		}
		this.image.sprite = spatialItemData.sprite;
		GameManager.Instance.AudioPlayer.PlaySFX(this.researchSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowBook(ResearchableItemData researchableItemData)
	{
		this.titleTextLocalized.enabled = false;
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(researchableItemData.itemNameKey.TableReference, researchableItemData.itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleText.text = localizedString;
		this.subtitleTextLocalized.enabled = false;
		this.subtitleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.book-complete.subtitle");
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		this.image.transform.localScale = Vector3.one;
		this.image.sprite = this.bookSprite;
		GameManager.Instance.AudioPlayer.PlaySFX(this.bookCompleteSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void ShowUpgrade(UpgradeData upgradeData)
	{
		this.titleTextLocalized.enabled = false;
		object[] array = null;
		if (upgradeData is HullUpgradeData)
		{
			array = new object[]
			{
				upgradeData.tier,
				upgradeData.GetNewCellCount()
			};
		}
		else if (upgradeData is SlotUpgradeData)
		{
			array = new object[] { upgradeData.GetNewCellCount() };
		}
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(upgradeData.TitleKey.TableReference, upgradeData.TitleKey.TableEntryReference, LocalizationSettings.SelectedLocale, FallbackBehavior.UseProjectSettings, array);
		this.titleText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
		this.titleText.text = localizedString;
		this.subtitleTextLocalized.enabled = false;
		this.subtitleTextLocalized.StringReference.SetReference(LanguageManager.STRING_TABLE, "notification.upgrade-added.subtitle");
		this.subtitleTextLocalized.RefreshString();
		this.subtitleTextLocalized.enabled = true;
		this.image.transform.localScale = Vector3.one;
		if (upgradeData is HullUpgradeData)
		{
			this.image.sprite = this.hullUpgradeSprites[upgradeData.tier - 2];
		}
		else
		{
			this.image.sprite = this.regularUpgradeSprite;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(this.regularSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isShowing = true;
		this.animator.SetBool("showing", true);
	}

	public void RequestHide()
	{
		this.isHiding = true;
		this.animator.SetBool("showing", false);
	}

	public void OnHideCompleteEventFired()
	{
		this.isShowing = false;
		this.isHiding = false;
		Action onHideComplete = this.OnHideComplete;
		if (onHideComplete == null)
		{
			return;
		}
		onHideComplete();
	}

	[SerializeField]
	private AssetReference regularSFX;

	[SerializeField]
	private AssetReference bookCompleteSFX;

	[SerializeField]
	private AssetReference fishSFX;

	[SerializeField]
	private AssetReference aberrationSFX;

	[SerializeField]
	private AssetReference researchSFX;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private LocalizeStringEvent titleTextLocalized;

	[SerializeField]
	private TextMeshProUGUI subtitleText;

	[SerializeField]
	private LocalizeStringEvent subtitleTextLocalized;

	[SerializeField]
	private Sprite bookSprite;

	[SerializeField]
	private Sprite regularUpgradeSprite;

	[SerializeField]
	private List<Sprite> hullUpgradeSprites;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Animator animator;

	public Action OnHideComplete;
}
