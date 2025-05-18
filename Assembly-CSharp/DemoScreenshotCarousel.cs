using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class DemoScreenshotCarousel : MonoBehaviour
{
	private void OnEnable()
	{
		this.timeUntilSwitch = this.holdDuration;
		this.localizedStringField.StringReference = this.captions[this.currentIndex];
	}

	private void Update()
	{
		if (this.isSwitching)
		{
			return;
		}
		this.timeUntilSwitch -= Time.unscaledDeltaTime;
		if (this.timeUntilSwitch <= 0f)
		{
			this.isSwitching = true;
			this.tweener = this.image.DOColor(new Color(1f, 1f, 1f, 0f), this.fadeOutDuration);
			this.tweener.OnComplete(delegate
			{
				this.currentIndex++;
				if (this.currentIndex > this.sprites.Count - 1)
				{
					this.currentIndex = 0;
				}
				this.image.sprite = this.sprites[this.currentIndex];
				this.localizedStringField.StringReference = this.captions[this.currentIndex];
				this.tweener = this.image.DOColor(new Color(1f, 1f, 1f, 1f), this.fadeInDuration);
				this.tweener.OnComplete(delegate
				{
					this.timeUntilSwitch = this.holdDuration;
					this.isSwitching = false;
					this.tweener = null;
				});
			});
		}
	}

	private void OnDestroy()
	{
		if (this.tweener != null)
		{
			this.tweener.Kill(false);
		}
	}

	[SerializeField]
	private List<Sprite> sprites;

	[SerializeField]
	private List<LocalizedString> captions;

	[SerializeField]
	private Image image;

	[SerializeField]
	private LocalizeStringEvent localizedStringField;

	[SerializeField]
	private float holdDuration;

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private float fadeInDuration;

	private Tweener tweener;

	private int currentIndex;

	private float timeUntilSwitch;

	private bool isSwitching;
}
