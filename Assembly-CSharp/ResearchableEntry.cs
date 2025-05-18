using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResearchableEntry : MonoBehaviour
{
	private void SetVisuals()
	{
		this.mainImage.sprite = this.spatialItemData.sprite;
		Vector2 vector = new Vector2(this.pixelsPerSquare * (float)this.spatialItemData.GetWidth(), this.pixelsPerSquare * (float)this.spatialItemData.GetHeight());
		this.mainImageContainer.sizeDelta = vector;
		(this.mainImage.transform as RectTransform).sizeDelta = vector;
		(this.backplateImage.transform as RectTransform).sizeDelta = vector;
		this.preLine.anchoredPosition = new Vector2(-(vector.x * 0.5f), this.preLine.anchoredPosition.y);
		this.postLine.anchoredPosition = new Vector2(vector.x * 0.5f, this.postLine.anchoredPosition.y);
		foreach (Transform transform in this.researchNotchContainer.Cast<Transform>().ToList<Transform>())
		{
			global::UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		for (int i = 0; i < this.spatialItemData.ResearchPointsRequired; i++)
		{
		}
	}

	private void Awake()
	{
		this.researchableSelectable.promptCompleteAction = new Action(this.OnResearchableEntryClicked);
		this.spatialItemTooltipRequester.SpatialItemData = this.spatialItemData;
	}

	private void OnEnable()
	{
		this.researchNotches.Clear();
		this.researchNotches = this.researchNotchContainer.GetComponentsInChildren<ResearchNotch>().ToList<ResearchNotch>();
		this.researchNotches.ForEach(delegate(ResearchNotch n)
		{
			n.OnAnimationCompleteAction = (Action)Delegate.Combine(n.OnAnimationCompleteAction, new Action(this.OnNotchAnimationComplete));
		});
		this.RefreshUI(null);
		GameEvents.Instance.OnResearchCompleted += new Action<SpatialItemData>(this.RefreshUI);
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnResearchCompleted -= new Action<SpatialItemData>(this.RefreshUI);
		this.researchNotches.ForEach(delegate(ResearchNotch n)
		{
			n.OnAnimationCompleteAction = (Action)Delegate.Remove(n.OnAnimationCompleteAction, new Action(this.OnNotchAnimationComplete));
		});
	}

	private void RefreshUI(ItemData itemData)
	{
		this.isResearched = GameManager.Instance.SaveData.GetIsItemResearched(this.spatialItemData);
		this.areResearchPrerequisitesMet = this.spatialItemData.ResearchPrerequisites.TrueForAll((ResearchedItemResearchablePrerequisite x) => x.IsPrerequisiteMet());
		this.areOwnedItemPrerequisitesMet = this.spatialItemData.ItemOwnPrerequisites.TrueForAll((OwnedItemResearchablePrerequisite x) => x.IsPrerequisiteMet());
		this.areAllPrerequisitesMet = this.areResearchPrerequisitesMet && this.areOwnedItemPrerequisitesMet;
		this.researchableSelectable.CanBeResearched = !this.isResearched && this.areAllPrerequisitesMet;
		bool flag = this.isResearched || this.areAllPrerequisitesMet;
		this.mainImage.material = (flag ? null : this.silhouetteMaterial);
		this.spatialItemTooltipRequester.TooltipMode = (flag ? TooltipUI.TooltipMode.RESEARCH_PREVIEW : TooltipUI.TooltipMode.MYSTERY);
		this.backplateImage.color = (this.isResearched ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
		this.linesToColorWhenPrerequisitesAreMet.ForEach(delegate(Image l)
		{
			l.color = (this.areResearchPrerequisitesMet ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
		});
		this.linesToColorWhenThisIsResearched.ForEach(delegate(Image l)
		{
			l.color = (this.isResearched ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
		});
		this.researchNotchContainer.gameObject.SetActive(!this.isResearched);
		this.researchCompleteContainer.SetActive(this.isResearched);
		if (!this.isResearched)
		{
			int researchProgress = GameManager.Instance.SaveData.GetResearchProgress(this.spatialItemData);
			for (int i = 0; i < this.researchNotches.Count; i++)
			{
				this.researchNotches[i].SetFilled(i < researchProgress);
			}
		}
		this.RefreshAttentionCallout();
	}

	private void RefreshAttentionCallout()
	{
		if (this.attentionCallout)
		{
			this.attentionCallout.SetActive(this.highlightConditions.Any((HighlightCondition h) => h.ShouldHighlight()));
		}
	}

	private void OnResearchableEntryClicked()
	{
		if (!this.isResearched && this.areAllPrerequisitesMet)
		{
			if (!GameManager.Instance.ResearchHelper.SpendResearchItem())
			{
				GameManager.Instance.UI.ResearchWindow.FlashResearchItemCount();
				CustomDebug.EditorLogError("[ResearchableEntry] OnResearchableEntryClicked() failed to spend research item.");
				return;
			}
			int num = GameManager.Instance.SaveData.AdjustResearchProgress(this.spatialItemData, 1);
			if (this.researchNotches.Count >= num)
			{
				this.researchNotches[num - 1].AnimateFill();
			}
		}
	}

	private void OnNotchAnimationComplete()
	{
		this.RefreshUI(this.spatialItemData);
	}

	[SerializeField]
	private SpatialItemData spatialItemData;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private SpatialItemTooltipRequester spatialItemTooltipRequester;

	[SerializeField]
	private ResearchableSelectable researchableSelectable;

	[SerializeField]
	private BasicButtonWrapper basicButtonWrapper;

	[SerializeField]
	private RectTransform mainImageContainer;

	[SerializeField]
	private Image backplateImage;

	[SerializeField]
	private Image mainImage;

	[SerializeField]
	private List<Image> linesToColorWhenPrerequisitesAreMet;

	[SerializeField]
	private List<Image> linesToColorWhenThisIsResearched;

	[SerializeField]
	private Material silhouetteMaterial;

	[SerializeField]
	private float pixelsPerSquare;

	[SerializeField]
	private GameObject researchNotchPrefab;

	[SerializeField]
	private RectTransform preLine;

	[SerializeField]
	private RectTransform postLine;

	[SerializeField]
	private GameObject researchCompleteContainer;

	[SerializeField]
	private Transform researchNotchContainer;

	[SerializeField]
	private GameObject attentionCallout;

	[SerializeField]
	private List<HighlightCondition> highlightConditions = new List<HighlightCondition>();

	private List<ResearchNotch> researchNotches = new List<ResearchNotch>();

	private bool isResearched;

	private bool areResearchPrerequisitesMet;

	private bool areOwnedItemPrerequisitesMet;

	private bool areAllPrerequisitesMet;
}
