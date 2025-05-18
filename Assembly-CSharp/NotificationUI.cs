using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
	public float AppearTime
	{
		get
		{
			return this.appearTime;
		}
	}

	public bool IsAnimatingOut
	{
		get
		{
			return this.isAnimatingOut;
		}
	}

	public void Init(NotificationType notificationType, string notificationString, float holdTimeSec, int index, Dictionary<NotificationType, DredgeColorTypeEnum> colorMap)
	{
		this.textField.text = notificationString;
		this.holdTimeSec = holdTimeSec;
		this.index = index;
		this.appearTime = Time.time;
		DredgeColorTypeEnum dredgeColorTypeEnum;
		if (colorMap.TryGetValue(notificationType, out dredgeColorTypeEnum))
		{
			Color color = GameManager.Instance.LanguageManager.GetColor(dredgeColorTypeEnum);
			this.textCoverImage.color = color;
			this.backgroundImage.color = color;
		}
	}

	private void Awake()
	{
		this.textCover.gameObject.SetActive(true);
		this.textCoverTween = this.textCover.DOScaleX(0f, this.uncoverTimeSec).OnComplete(delegate
		{
			this.textCover.gameObject.SetActive(false);
		}).SetUpdate(this.useUnscaledTime);
	}

	public void SetUseUnscaledTime(bool useUnscaledTime)
	{
		this.useUnscaledTime = useUnscaledTime;
		if (this.textCoverTween != null)
		{
			this.textCoverTween.SetUpdate(useUnscaledTime);
		}
	}

	private void Update()
	{
		if (!this.isAnimatingOut)
		{
			if (this.useUnscaledTime)
			{
				this.holdTimeSec -= Time.unscaledDeltaTime;
			}
			else
			{
				this.holdTimeSec -= Time.deltaTime;
			}
			if (this.holdTimeSec <= 0f)
			{
				this.Hide();
			}
		}
	}

	public void Hide()
	{
		this.isAnimatingOut = true;
		this.canvasGroup.DOFade(0f, this.fadeOutTimeSec).OnComplete(delegate
		{
			Action<NotificationUI, int> onHideComplete = this.OnHideComplete;
			if (onHideComplete == null)
			{
				return;
			}
			onHideComplete(this, this.index);
		}).SetUpdate(this.useUnscaledTime);
	}

	[SerializeField]
	private TextMeshProUGUI textField;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private float uncoverTimeSec;

	[SerializeField]
	private float fadeOutTimeSec;

	[SerializeField]
	private RectTransform textCover;

	[SerializeField]
	private Image textCoverImage;

	[SerializeField]
	private Image backgroundImage;

	private bool useUnscaledTime;

	public Action<NotificationUI, int> OnHideComplete;

	private float holdTimeSec;

	private bool isAnimatingOut;

	private int index;

	private float appearTime;

	private Tweener textCoverTween;
}
