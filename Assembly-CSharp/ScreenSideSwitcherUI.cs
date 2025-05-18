using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSideSwitcherUI : MonoBehaviour
{
	private void OnEnable()
	{
		this.image.color = Color.white;
		if (!GameManager.Instance.ScreenSideSwitcher.HasUsedSideSwitchers)
		{
			this.tweener = this.image.DOColor(Color.grey, 0.35f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
	}

	private void OnDisable()
	{
		if (this.tweener != null)
		{
			this.tweener.Kill(false);
			this.tweener = null;
		}
	}

	[SerializeField]
	private Image image;

	private Tweener tweener;
}
