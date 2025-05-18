using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class FadeAudio : MonoBehaviour
{
	private void Awake()
	{
		this.audioSource.Play();
		this.FadeUp();
	}

	public void FadeUp()
	{
		if (this.fadeTween != null)
		{
			return;
		}
		this.fadeTween = this.audioSource.DOFade(this.upVolume, this.durationSec).From(this.downVolume, true, false).OnComplete(delegate
		{
			this.isFadedIn = true;
			this.fadeTween = null;
		});
	}

	public void FadeDown()
	{
		this.FadeDown(this.durationSec);
	}

	public void FadeDown(float overrideDurationSec)
	{
		if (this.fadeTween != null)
		{
			return;
		}
		this.fadeTween = this.audioSource.DOFade(this.downVolume, overrideDurationSec).OnComplete(delegate
		{
			this.isFadedIn = false;
			this.fadeTween = null;
		});
	}

	private void Update()
	{
		if (this.fadeTween == null && this.autoFadeUpGameTime < this.autoFadeDownGameTime)
		{
			if (this.autoFadeUp && !this.isFadedIn && GameManager.Instance.Time.Time > this.autoFadeUpGameTime && GameManager.Instance.Time.Time < this.autoFadeDownGameTime)
			{
				this.FadeUp();
			}
			if (this.autoFadeDown && this.isFadedIn && GameManager.Instance.Time.Time > this.autoFadeDownGameTime)
			{
				this.FadeDown();
			}
		}
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float durationSec = 0.5f;

	[SerializeField]
	private float upVolume = 0.5f;

	[SerializeField]
	private float downVolume;

	[SerializeField]
	private bool autoFadeUp;

	[SerializeField]
	private float autoFadeUpGameTime;

	[SerializeField]
	private bool autoFadeDown;

	[SerializeField]
	private float autoFadeDownGameTime;

	private Tweener fadeTween;

	private bool isFadedIn;

	private bool isFadedDown;
}
