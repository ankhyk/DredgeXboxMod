using System;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
	public void Fade(bool show, bool animate = true)
	{
		this._hasAnimatedLoadingScreen = false;
		if (animate)
		{
			this.loadingScreenAnimator.SetTrigger(show ? "show" : "hide");
			return;
		}
		this.loadingScreenCanvasGroup.alpha = (show ? 1f : 0f);
	}

	private void OnLoadingScreenAnimationComplete()
	{
		this._hasAnimatedLoadingScreen = true;
	}

	public bool HasAnimatedLoadingScreen()
	{
		return this._hasAnimatedLoadingScreen;
	}

	[SerializeField]
	private CanvasGroup loadingScreenCanvasGroup;

	[SerializeField]
	private Animator loadingScreenAnimator;

	[SerializeField]
	private GameObject loadingScreen;

	private bool _hasAnimatedLoadingScreen;
}
