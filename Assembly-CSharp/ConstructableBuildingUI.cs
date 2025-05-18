using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ConstructableBuildingUI : MonoBehaviour, IConstructableObjectUI<BuildingRecipeData>
{
	public void Init(BuildingRecipeData buildingRecipe)
	{
		this.headerContainerTransform.DOAnchorPosX(0f, this.slideOutDurationSec, false).From(new Vector2(this.slideOutX, 0f), true, false).SetEase(this.slideOutEase);
		this.textCanvasGroup.alpha = 0f;
		this.buildingNameText.StringReference = buildingRecipe.buildingNameKey;
		this.unlockDetailsText.StringReference = buildingRecipe.buildingDescriptionKey;
		base.gameObject.SetActive(true);
		this.background.SetActive(true);
		this.textCanvasGroup.DOFade(1f, this.fadeTextDurationSec);
	}

	public void Close()
	{
		base.gameObject.SetActive(false);
		this.background.SetActive(false);
	}

	[SerializeField]
	private RectTransform headerContainerTransform;

	[SerializeField]
	private CanvasGroup textCanvasGroup;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private LocalizeStringEvent buildingNameText;

	[SerializeField]
	private LocalizeStringEvent unlockDetailsText;

	[SerializeField]
	private Image buildingIcon;

	[Header("Animation Config")]
	[SerializeField]
	private float fadeTextDurationSec;

	[SerializeField]
	private float slideOutX;

	[SerializeField]
	private float slideOutDurationSec;

	[SerializeField]
	private Ease slideOutEase;
}
