using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TooltipSectionFishDetails : TooltipSection<FishItemInstance>
{
	public override void Init<T>(T fishItemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.freshnessValueRaw = fishItemInstance.freshness;
		string text;
		if (fishItemInstance.isInfected)
		{
			this.freshnessColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL);
			text = "freshness.infected";
		}
		else if (this.freshnessValueRaw > GameManager.Instance.GameConfigData.MaxFreshness - 1f)
		{
			this.freshnessColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
			text = "freshness.fresh";
		}
		else if (this.freshnessValueRaw < 1f)
		{
			this.freshnessColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			text = "freshness.rotting";
		}
		else
		{
			this.freshnessColor = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
			text = "freshness.stale";
		}
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(LanguageManager.STRING_TABLE, text, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += this.OnFreshnessStringLoaded;
		FishItemData itemData = fishItemInstance.GetItemData<FishItemData>();
		string text2 = "";
		if (fishItemInstance.size >= GameManager.Instance.GameConfigData.TrophyMaxSize)
		{
			text2 = "<color=#FFD104><sprite name=\"TrophyIcon\" tint=1></color>";
		}
		text2 += GameManager.Instance.ItemManager.GetFormattedFishSizeString(fishItemInstance.size, itemData);
		this.sizeString.text = text2;
		if (itemData.harvestableType == HarvestableType.NONE)
		{
			this.harvestableTypeContainer.SetActive(false);
			this.catchDepthContainer.SetActive(true);
			this.catchDepthString.text = itemData.GetDepthString();
		}
		else
		{
			this.harvestableTypeContainer.SetActive(true);
			this.catchDepthContainer.SetActive(false);
			HarvestableType harvestableType = itemData.harvestableType;
			this.harvestableTypeTagUI.Init(harvestableType, itemData.requiresAdvancedEquipment);
		}
		this.isLayedOut = true;
	}

	private void OnFreshnessStringLoaded(AsyncOperationHandle<string> op)
	{
		string text = ColorUtility.ToHtmlStringRGB(this.freshnessColor);
		string text2 = string.Concat(new string[] { "<color=#", text, ">", op.Result, "</color>" });
		if (this.showFreshnessRaw)
		{
			string text3 = this.freshnessValueRaw.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
			text2 = text2 + "(" + text3 + ")";
		}
		this.freshnessString.text = text2;
	}

	[SerializeField]
	private TextMeshProUGUI sizeString;

	[SerializeField]
	private TextMeshProUGUI freshnessString;

	[SerializeField]
	private GameObject catchDepthContainer;

	[SerializeField]
	private TextMeshProUGUI catchDepthString;

	[SerializeField]
	private GameObject harvestableTypeContainer;

	[SerializeField]
	private HarvestableTypeTagUI harvestableTypeTagUI;

	[SerializeField]
	private bool showFreshnessRaw;

	private float freshnessValueRaw;

	private Color freshnessColor;
}
