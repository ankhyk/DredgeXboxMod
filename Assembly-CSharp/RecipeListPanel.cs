using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class RecipeListPanel : TabbedPanel
{
	private void Awake()
	{
		this.recipeEntries.ForEach(delegate(RecipeEntry re)
		{
			re.OnRecipeSelected = (Action<RecipeData>)Delegate.Combine(re.OnRecipeSelected, new Action<RecipeData>(this.InternalOnRecipeSelected));
			re.OnRecipeFailedToSpendResearch = (Action)Delegate.Combine(re.OnRecipeFailedToSpendResearch, new Action(this.OnRecipeFailedToSpendResearch));
		});
	}

	public void Init(RecipeListDestinationTier recipeList, bool exitOnFulfilled = true)
	{
		this.recipeList = recipeList;
		this.localizedTitleString.StringReference = this.recipeList.recipeListStringKey;
		this.researchCounterContainer.SetActive(recipeList.recipes.Any((RecipeData r) => r.researchRequired > 0));
		this.textCanvasGroup.alpha = 0f;
		this.headerTransform.DOAnchorPosX(0f, this.slideOutDurationSec, false).From(new Vector2(this.slideOutX, 0f), true, false).SetEase(this.slideOutEase);
		this.textCanvasGroup.DOFade(1f, this.fadeTextDurationSec).SetDelay(this.fadeTextDelaySec);
	}

	private void InternalOnRecipeSelected(RecipeData recipeData)
	{
		Action<RecipeData> onRecipeSelected = this.OnRecipeSelected;
		if (onRecipeSelected == null)
		{
			return;
		}
		onRecipeSelected(recipeData);
	}

	private void OnRecipeFailedToSpendResearch()
	{
		this.researchCounterAnimator.SetTrigger("failure");
	}

	public override void ShowStart()
	{
		this.bottomRow.gameObject.SetActive(false);
		for (int i = 0; i < this.recipeEntries.Count; i++)
		{
			if (this.recipeList.recipes.Count > i)
			{
				this.recipeEntries[i].gameObject.SetActive(true);
				this.recipeEntries[i].Init(this.recipeList.recipes[i]);
				this.recipeEntries[i].AnimateIn();
				if (i == this.entriesPerRow)
				{
					this.bottomRow.gameObject.SetActive(true);
				}
			}
			else
			{
				this.recipeEntries[i].gameObject.SetActive(false);
			}
		}
		GameManager.Instance.Player.CanMoveInstalledItems = true;
		base.ShowStart();
	}

	public override void ShowFinish()
	{
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		base.ShowFinish();
	}

	public override void HideStart()
	{
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
		base.HideStart();
	}

	public override void HideFinish()
	{
		base.HideFinish();
	}

	public override void SwitchToSide()
	{
		Selectable selectable = this.recipeEntries[0].selectable;
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(selectable.gameObject);
		selectable.Select();
	}

	private void OnEnable()
	{
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.RegisterSwitchResponder(this, this.screenSide);
		}
		GameEvents.Instance.OnItemInventoryChanged += this.OnInventoryItemChanged;
		this.UpdateResearchItemCount();
	}

	private void OnDisable()
	{
		if (this.screenSide != ScreenSide.NONE)
		{
			GameManager.Instance.ScreenSideSwitcher.UnregisterSwitchResponder(this, this.screenSide);
		}
		GameEvents.Instance.OnItemInventoryChanged -= this.OnInventoryItemChanged;
	}

	private void OnInventoryItemChanged(SpatialItemData spatialItemData)
	{
		if (spatialItemData != null && spatialItemData.id == this.researchItemData.id)
		{
			this.UpdateResearchItemCount();
		}
	}

	private void UpdateResearchItemCount()
	{
		this.researchItemCount.text = string.Format("x{0}", GameManager.Instance.ResearchHelper.TotalPartCount);
		this.researchItemCountCopy.text = this.researchItemCount.text;
	}

	public override bool GetCanSwitchToThisIfHoldingItem()
	{
		return false;
	}

	[SerializeField]
	private ScreenSide screenSide;

	[SerializeField]
	private RectTransform headerTransform;

	[SerializeField]
	private UIWindowType windowType;

	[SerializeField]
	private HorizontalLayoutGroup topRow;

	[SerializeField]
	private HorizontalLayoutGroup bottomRow;

	[SerializeField]
	private List<RecipeEntry> recipeEntries = new List<RecipeEntry>();

	[SerializeField]
	private LocalizeStringEvent localizedTitleString;

	[SerializeField]
	private int entriesPerRow;

	[SerializeField]
	private CanvasGroup textCanvasGroup;

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
	private float fadeTextDelaySec;

	private RecipeListDestinationTier recipeList;

	public Action<RecipeData> OnRecipeSelected;

	[Header("Researchable Recipes")]
	[SerializeField]
	private GameObject researchCounterContainer;

	[SerializeField]
	private Animator researchCounterAnimator;

	[SerializeField]
	private TextMeshProUGUI researchItemCount;

	[SerializeField]
	private TextMeshProUGUI researchItemCountCopy;

	[SerializeField]
	private SpatialItemData researchItemData;
}
