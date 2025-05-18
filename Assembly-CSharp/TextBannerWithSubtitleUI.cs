using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class TextBannerWithSubtitleUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.canvasGroup.alpha = 0f;
	}

	public void AnimateIn()
	{
		this.CleanUp();
		base.gameObject.SetActive(true);
		this.canvasGroupTweener = this.canvasGroup.DOFade(1f, 0.35f).From(0f, true, false).OnComplete(delegate
		{
			this.canvasGroupTweener = null;
		});
		this.titleTweener = this.title.DOAnchorPosX(1f, 0.35f, false).From(new Vector2(250f, 0f), true, false).OnComplete(delegate
		{
			this.titleTweener = null;
		});
		this.subtitleTweener = this.subtitle.DOAnchorPosX(1f, 0.35f, false).From(new Vector2(-250f, 0f), true, false).OnComplete(delegate
		{
			this.subtitleTweener = null;
		});
	}

	public void AnimateOut()
	{
		this.CleanUp();
		this.canvasGroupTweener = this.canvasGroup.DOFade(0f, 0.35f).OnComplete(delegate
		{
			base.gameObject.SetActive(false);
			this.canvasGroupTweener = null;
		});
		this.titleTweener = this.title.DOAnchorPosX(250f, 0.35f, false).OnComplete(delegate
		{
			this.titleTweener = null;
		});
		this.subtitleTweener = this.subtitle.DOAnchorPosX(-250f, 0.35f, false).OnComplete(delegate
		{
			this.subtitleTweener = null;
		});
	}

	private void CleanUp()
	{
		if (this.canvasGroupTweener != null)
		{
			this.canvasGroupTweener.Kill(false);
			this.canvasGroupTweener = null;
		}
		if (this.titleTweener != null)
		{
			this.titleTweener.Kill(false);
			this.titleTweener = null;
		}
		if (this.subtitleTweener != null)
		{
			this.subtitleTweener.Kill(false);
			this.subtitleTweener = null;
		}
	}

	[SerializeField]
	private RectTransform title;

	[SerializeField]
	private RectTransform subtitle;

	[SerializeField]
	private CanvasGroup canvasGroup;

	private Tweener canvasGroupTweener;

	private Tweener titleTweener;

	private Tweener subtitleTweener;
}
