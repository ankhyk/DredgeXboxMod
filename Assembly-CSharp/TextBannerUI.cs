using System;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class TextBannerUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.canvasGroup.alpha = 0f;
	}

	public void AnimateIn()
	{
		base.gameObject.SetActive(true);
		this.canvasGroup.DOFade(1f, 0.35f).From(0f, true, false);
		this.rectTransform.DOAnchorPosX(1f, 0.35f, false).From(new Vector2(250f, 0f), true, false);
	}

	public void AnimateOut()
	{
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private CanvasGroup canvasGroup;
}
