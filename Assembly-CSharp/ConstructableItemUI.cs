using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ConstructableItemUI : MonoBehaviour, IConstructableObjectUI<IDisplayableRecipe>
{
	public void Init(IDisplayableRecipe itemRecipe)
	{
		this.containerTransform.DOAnchorPosX(0f, this.slideOutDurationSec, false).From(new Vector2(this.slideOutX, 0f), true, false).SetEase(this.slideOutEase);
		object[] array = new object[0];
		if (itemRecipe is HullRecipeData)
		{
			array = new object[] { (itemRecipe as HullRecipeData).hullUpgradeData.tier };
		}
		LocalizedString itemNameKey = itemRecipe.GetItemNameKey();
		string text = LocalizationSettings.StringDatabase.GetLocalizedString(itemNameKey.TableReference, itemNameKey.TableEntryReference, LocalizationSettings.SelectedLocale, FallbackBehavior.UseProjectSettings, array);
		int quantityProduced = itemRecipe.GetQuantityProduced();
		if (quantityProduced > 1)
		{
			text += string.Format(" x{0}", quantityProduced);
		}
		this.itemNameText.text = text;
		this.itemImage.sprite = itemRecipe.GetSprite();
		this.itemImageFill.sprite = this.itemImage.sprite;
		this.quantityText.text = string.Format("x{0}", quantityProduced);
		this.quantityText.gameObject.SetActive(quantityProduced > 1);
		this.layoutGroup.padding = new RectOffset(this.borderSize, this.borderSize, this.borderSize, this.borderSize);
		this.imageTransform.sizeDelta = new Vector2((float)itemRecipe.GetWidth() * this.cellSize, (float)itemRecipe.GetHeight() * this.cellSize);
		this.imageCanvasGroup.alpha = 0f;
		this.textCanvasGroup.alpha = 0f;
		this.targetFillAmount = 0f;
		this.itemImageFill.fillAmount = 0f;
		this.cells.SetActive(itemRecipe is ItemRecipeData);
		base.gameObject.SetActive(true);
		this.background.SetActive(true);
		this.textCanvasGroup.DOFade(1f, this.fadeTextDurationSec);
		this.imageCanvasGroup.DOFade(1f, this.fadeImageDurationSec).SetDelay(this.fadeImageDelaySec);
		base.StartCoroutine(this.WaitForImageSize());
	}

	private IEnumerator WaitForImageSize()
	{
		this.mainContentSizeFitter.enabled = false;
		yield return new WaitForEndOfFrame();
		this.mainContentSizeFitter.enabled = true;
		yield break;
	}

	public void Close()
	{
		base.gameObject.SetActive(false);
		this.background.SetActive(false);
	}

	private void OnEnable()
	{
		RecipeGridPanel recipeGridPanel = this.recipeGridPanel;
		recipeGridPanel.OnPercentageRecipeCompleteChanged = (Action<float>)Delegate.Combine(recipeGridPanel.OnPercentageRecipeCompleteChanged, new Action<float>(this.OnPercentageRecipeCompleteChanged));
	}

	private void OnDisable()
	{
		RecipeGridPanel recipeGridPanel = this.recipeGridPanel;
		recipeGridPanel.OnPercentageRecipeCompleteChanged = (Action<float>)Delegate.Remove(recipeGridPanel.OnPercentageRecipeCompleteChanged, new Action<float>(this.OnPercentageRecipeCompleteChanged));
	}

	private void OnPercentageRecipeCompleteChanged(float percent)
	{
		this.targetFillAmount = percent;
	}

	private void Update()
	{
		this.itemImageFill.fillAmount = Mathf.Lerp(this.itemImageFill.fillAmount, this.targetFillAmount, Time.deltaTime * this.fillSpeed);
	}

	[SerializeField]
	private RecipeGridPanel recipeGridPanel;

	[SerializeField]
	private RectTransform containerTransform;

	[SerializeField]
	private CanvasGroup imageCanvasGroup;

	[SerializeField]
	private CanvasGroup textCanvasGroup;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private TextMeshProUGUI itemNameText;

	[SerializeField]
	private Image itemImage;

	[SerializeField]
	private Image itemImageFill;

	[SerializeField]
	private TextMeshProUGUI quantityText;

	[SerializeField]
	private RectTransform imageTransform;

	[SerializeField]
	private GameObject cells;

	[SerializeField]
	private HorizontalLayoutGroup layoutGroup;

	[SerializeField]
	private ContentSizeFitter mainContentSizeFitter;

	[SerializeField]
	private HorizontalLayoutGroup mainLayoutGroup;

	[SerializeField]
	private float cellSize;

	[SerializeField]
	private int borderSize;

	[Header("Animation Config")]
	[SerializeField]
	private float slideOutX;

	[SerializeField]
	private float slideOutDurationSec;

	[SerializeField]
	private Ease slideOutEase;

	[SerializeField]
	private float fadeTextDurationSec;

	[SerializeField]
	private float fadeImageDurationSec;

	[SerializeField]
	private float fadeImageDelaySec;

	[SerializeField]
	private float fillSpeed;

	private float targetFillAmount;
}
