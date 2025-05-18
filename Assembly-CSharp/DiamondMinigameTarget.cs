using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class DiamondMinigameTarget : MonoBehaviour
{
	public bool IsSpecial
	{
		get
		{
			return this.isSpecial;
		}
	}

	public bool IsInPlay
	{
		get
		{
			return this.isInPlay;
		}
	}

	public void Init(float timeToHitFullScale, bool isSpecial, float scaleLimit, float angleFrom)
	{
		base.transform.localScale = Vector3.zero;
		this.timeToHitFullScale = timeToHitFullScale;
		this.isSpecial = isSpecial;
		this.scaleLimit = scaleLimit;
		this.isInPlay = true;
		base.transform.DORotate(Vector3.zero, timeToHitFullScale, RotateMode.FastBeyond360).From(new Vector3(0f, 0f, -angleFrom), true, false);
		this.image.color = GameManager.Instance.LanguageManager.GetColor(isSpecial ? DredgeColorTypeEnum.VALUABLE : DredgeColorTypeEnum.POSITIVE);
	}

	public float GetCurrentScale()
	{
		return base.transform.localScale.x;
	}

	public void RemoveSpecialness()
	{
		this.isSpecial = false;
		this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
	}

	public void OnInputDisabled()
	{
		this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
	}

	public void OnInputEnabled()
	{
		if (this.isInPlay)
		{
			this.image.color = GameManager.Instance.LanguageManager.GetColor(this.isSpecial ? DredgeColorTypeEnum.VALUABLE : DredgeColorTypeEnum.POSITIVE);
		}
	}

	private void Update()
	{
		this.lifetime += Time.deltaTime;
		if (this.isInPlay)
		{
			float num = this.scaleCurve.Evaluate(Mathf.InverseLerp(0f, this.timeToHitFullScale, this.lifetime)) * this.scaleLimit;
			base.transform.localScale = new Vector3(num, num, num);
			if (base.transform.localScale.x >= this.scaleLimit)
			{
				this.isInPlay = false;
				Action<DiamondMinigameTarget> onScaleLimitReached = this.OnScaleLimitReached;
				if (onScaleLimitReached == null)
				{
					return;
				}
				onScaleLimitReached(this);
			}
		}
	}

	public void Dismiss(bool success)
	{
		if (!success)
		{
			this.image.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
		this.isInPlay = false;
		this.image.DOFade(0f, this.dismissFadeDurationSec);
		TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = base.transform.DOScale(new Vector3(this.dismissScale, this.dismissScale, this.dismissScale), this.dismissScaleDurationSec);
		tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate
		{
			Action<DiamondMinigameTarget> onDismissed = this.OnDismissed;
			if (onDismissed == null)
			{
				return;
			}
			onDismissed(this);
		}));
	}

	private void OnDestroy()
	{
		DOTween.Kill(base.transform, false);
		DOTween.Kill(this.image, false);
	}

	[SerializeField]
	private float dismissScale;

	[SerializeField]
	private float dismissScaleDurationSec;

	[SerializeField]
	private float dismissFadeDurationSec;

	[SerializeField]
	private Image image;

	[SerializeField]
	private float scaleLimit;

	[SerializeField]
	private float timeToHitFullScale;

	[SerializeField]
	private AnimationCurve scaleCurve;

	public Action<DiamondMinigameTarget> OnScaleLimitReached;

	public Action<DiamondMinigameTarget> OnDismissed;

	private bool isInPlay;

	private float lifetime;

	private bool isSpecial;
}
