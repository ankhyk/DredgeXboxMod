using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntry : MonoBehaviour
{
	public RecipeData RecipeData
	{
		get
		{
			return this.recipeData;
		}
		set
		{
			this.recipeData = value;
		}
	}

	private void Awake()
	{
		BasicButtonWrapper basicButtonWrapper = this.basicButtonWrapper;
		basicButtonWrapper.OnSelectAction = (Action)Delegate.Combine(basicButtonWrapper.OnSelectAction, new Action(this.OnSelected));
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnResearchCompleted += this.RefreshUI;
		GameEvents.Instance.OnQuestStepCompleted += this.OnQuestStepCompleted;
		this.RefreshUI();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnResearchCompleted -= this.RefreshUI;
		GameEvents.Instance.OnQuestStepCompleted -= this.OnQuestStepCompleted;
		this.researchNotches.ForEach(delegate(ResearchNotch n)
		{
			n.OnAnimationCompleteAction = (Action)Delegate.Remove(n.OnAnimationCompleteAction, new Action(this.RefreshUI));
		});
	}

	private void OnQuestStepCompleted(QuestStepData questStepData)
	{
		this.RefreshUI();
	}

	private void OnSelected()
	{
		GameManager.Instance.GridManager.ClearLastSelectedCell();
		GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(ScreenSide.LEFT);
	}

	public void AnimateIn()
	{
		this.canvasGroup.DOFade(1f, this.imageFadeDurationSec);
		this.childImageContainer.DOScale(Vector3.one, this.scaleDurationSec).From(Vector3.zero, true, false).SetEase(this.scaleEase);
	}

	public void Init(RecipeData recipeData)
	{
		this.recipeData = recipeData;
		foreach (Transform transform in this.researchNotchContainer.Cast<Transform>().ToList<Transform>())
		{
			global::UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		this.researchNotches = new List<ResearchNotch>();
		for (int i = 0; i < recipeData.researchRequired; i++)
		{
			ResearchNotch component = global::UnityEngine.Object.Instantiate<GameObject>(this.researchNotchPrefab, this.researchNotchContainer).GetComponent<ResearchNotch>();
			ResearchNotch researchNotch = component;
			researchNotch.OnAnimationCompleteAction = (Action)Delegate.Combine(researchNotch.OnAnimationCompleteAction, new Action(this.RefreshUI));
			this.researchNotches.Add(component);
		}
		this.mainImage.sprite = recipeData.GetSprite();
		this.targetSize = new Vector2(this.pixelsPerSquare * (float)recipeData.GetWidth(), this.pixelsPerSquare * (float)recipeData.GetHeight());
		this.mainImageContainer.sizeDelta = this.targetSize;
		if (recipeData is ItemRecipeData)
		{
			this.spatialItemTooltipRequester.enabled = true;
			this.spatialItemTooltipRequester.TooltipMode = this.tooltipMode;
			this.spatialItemTooltipRequester.SpatialItemData = (recipeData as ItemRecipeData).itemProduced;
			this.spatialItemTooltipRequester.RecipeData = recipeData;
			this.abilityTooltipRequester.enabled = false;
			this.upgradeTooltipRequester.enabled = false;
		}
		else if (recipeData is AbilityRecipeData)
		{
			this.spatialItemTooltipRequester.enabled = false;
			this.upgradeTooltipRequester.enabled = false;
			this.abilityTooltipRequester.enabled = true;
			this.abilityTooltipRequester.abilityData = (recipeData as AbilityRecipeData).abilityData;
			this.abilityTooltipRequester.RecipeData = recipeData;
		}
		this.canvasGroup.alpha = 0f;
		this.RefreshUI();
	}

	private void RefreshUI(SpatialItemData spatialItemData)
	{
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		this.needsResearching = this.recipeData != null && this.recipeData.researchRequired != 0 && !GameManager.Instance.SaveData.GetIsItemResearched(this.recipeData);
		this.researchNotchContainer.gameObject.SetActive(this.needsResearching);
		if (this.needsResearching)
		{
			int researchProgress = GameManager.Instance.SaveData.GetResearchProgress(this.recipeData);
			for (int i = 0; i < this.researchNotches.Count; i++)
			{
				this.researchNotches[i].SetFilled(i < researchProgress);
			}
		}
		this.isOneTimeAndAlreadyOwned = false;
		if (this.recipeData != null && this.recipeData is AbilityRecipeData)
		{
			this.isOneTimeAndAlreadyOwned = GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked((this.recipeData as AbilityRecipeData).abilityData);
		}
		else if (this.recipeData != null && this.recipeData is HullRecipeData)
		{
			this.isOneTimeAndAlreadyOwned = GameManager.Instance.SaveData.HullTier >= (this.recipeData as HullRecipeData).hullUpgradeData.tier;
		}
		this.alreadyOwnedIcon.SetActive(this.isOneTimeAndAlreadyOwned);
		if (this.needsResearching)
		{
			this.researchableSelectable.CanBeResearched = true;
			this.researchableSelectable.promptCompleteAction = new Action(this.AddResearchPart);
			this.basicButtonWrapper.OnClick = delegate
			{
			};
		}
		else
		{
			this.researchableSelectable.CanBeResearched = false;
			this.basicButtonWrapper.OnClick = new Action(this.OnRecipeEntryClicked);
		}
		if (this.quantityText && this.recipeData)
		{
			this.quantityText.text = string.Format("x{0}", this.recipeData.quantityProduced);
			this.quantityText.gameObject.SetActive(this.recipeData.quantityProduced > 1);
		}
		this.mainImage.material = (this.needsResearching ? this.silhouetteMaterial : null);
		if (this.attentionCallout != null && this.recipeData != null)
		{
			this.attentionCallout.Evaluate();
		}
	}

	private void OnRecipeEntryClicked()
	{
		if (this.recipeData is AbilityRecipeData && GameManager.Instance.PlayerAbilities.GetIsAbilityUnlocked((this.recipeData as AbilityRecipeData).abilityData))
		{
			GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ERROR, "notification.already-researched-ability", (this.recipeData as AbilityRecipeData).abilityData.nameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.EMPHASIS));
			return;
		}
		if (GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			return;
		}
		Action<RecipeData> onRecipeSelected = this.OnRecipeSelected;
		if (onRecipeSelected == null)
		{
			return;
		}
		onRecipeSelected(this.recipeData);
	}

	private void AddResearchPart()
	{
		if (this.needsResearching)
		{
			if (!GameManager.Instance.ResearchHelper.SpendResearchItem())
			{
				Action onRecipeFailedToSpendResearch = this.OnRecipeFailedToSpendResearch;
				if (onRecipeFailedToSpendResearch == null)
				{
					return;
				}
				onRecipeFailedToSpendResearch();
				return;
			}
			else
			{
				int num = GameManager.Instance.SaveData.AdjustResearchProgress(this.recipeData, 1);
				if (this.researchNotches.Count >= num)
				{
					this.researchNotches[num - 1].AnimateFill();
				}
				if (num >= this.recipeData.researchRequired)
				{
					Action<RecipeData> onRecipeSelected = this.OnRecipeSelected;
					if (onRecipeSelected == null)
					{
						return;
					}
					onRecipeSelected(this.recipeData);
				}
			}
		}
	}

	[SerializeField]
	public Selectable selectable;

	[SerializeField]
	private SpatialItemTooltipRequester spatialItemTooltipRequester;

	[SerializeField]
	private AbilityTooltipRequester abilityTooltipRequester;

	[SerializeField]
	private UpgradeTooltipRequester upgradeTooltipRequester;

	[SerializeField]
	private ResearchableSelectable researchableSelectable;

	[SerializeField]
	private TooltipUI.TooltipMode tooltipMode;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private RectTransform mainImageContainer;

	[SerializeField]
	private RectTransform backgroundContainer;

	[SerializeField]
	private RectTransform childImageContainer;

	[SerializeField]
	private Image backplateImage;

	[SerializeField]
	private Image mainImage;

	[SerializeField]
	private float pixelsPerSquare;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private GameObject alreadyOwnedIcon;

	[SerializeField]
	private TextMeshProUGUI quantityText;

	[SerializeField]
	private Material silhouetteMaterial;

	[Header("Research Config")]
	[SerializeField]
	private GameObject researchNotchPrefab;

	[SerializeField]
	private Transform researchNotchContainer;

	[Header("Animation Config")]
	[SerializeField]
	private float imageFadeDurationSec;

	[SerializeField]
	private Ease scaleEase;

	[SerializeField]
	private float scaleDurationSec;

	[SerializeField]
	private AttentionCallout attentionCallout;

	private RecipeData recipeData;

	public Action<RecipeData> OnRecipeSelected;

	public Action OnRecipeFailedToSpendResearch;

	private bool needsResearching;

	private bool isOneTimeAndAlreadyOwned;

	private List<ResearchNotch> researchNotches = new List<ResearchNotch>();

	private Vector2 targetSize;
}
