using System;
using DG.Tweening;
using UnityEngine;

public class SavingIcon : MonoBehaviour
{
	private void Awake()
	{
		this.canvasGroup.alpha = 0f;
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveStart = (Action)Delegate.Combine(saveManager.OnSaveStart, new Action(this.OnSaveStart));
		SaveManager saveManager2 = GameManager.Instance.SaveManager;
		saveManager2.OnSaveComplete = (Action)Delegate.Combine(saveManager2.OnSaveComplete, new Action(this.OnSaveComplete));
	}

	private void OnDestroy()
	{
		SaveManager saveManager = GameManager.Instance.SaveManager;
		saveManager.OnSaveStart = (Action)Delegate.Remove(saveManager.OnSaveStart, new Action(this.OnSaveStart));
		SaveManager saveManager2 = GameManager.Instance.SaveManager;
		saveManager2.OnSaveComplete = (Action)Delegate.Remove(saveManager2.OnSaveComplete, new Action(this.OnSaveComplete));
	}

	private void OnSaveStart()
	{
		if (this.fadeTween != null)
		{
			this.fadeTween.Kill(false);
			this.fadeTween = null;
		}
		this.fadeTween = this.canvasGroup.DOFade(1f, 0.35f);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.OnComplete(delegate
		{
			this.fadeTween = null;
		});
	}

	private void OnSaveComplete()
	{
		this.fadeTween = this.canvasGroup.DOFade(0f, 0.35f);
		this.fadeTween.SetUpdate(true);
		this.fadeTween.SetDelay(1f);
		this.fadeTween.OnComplete(delegate
		{
			this.fadeTween = null;
		});
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	private Tweener fadeTween;
}
