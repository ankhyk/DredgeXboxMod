using System;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.Video;

public class SplashController : MonoBehaviour
{
	private void OnEnable()
	{
		Debug.Log("SplashController - OnEnabled");
		TaskUtil.Run(new Func<Task>(this.Init));
	}

	private async Task Init()
	{
		Debug.Log("SplashController - Init");
		while (!GameManager.Instance.HasLoadedAsyncManagers)
		{
			await Awaiters.NextFrame;
		}
		if (GameManager.Instance.hasBankedId)
		{
			this.OnSplashComplete();
		}
		else
		{
			this.BeginSplashAnimation();
		}
	}

	public async Task FadeIn(CanvasGroup group, float duration)
	{
		bool done = false;
		group.DOFade(1f, duration).OnComplete(delegate
		{
			done = true;
		});
		while (!done)
		{
			await Awaiters.NextFrame;
		}
	}

	public async Task FadeOut(CanvasGroup group, float duration)
	{
		bool done = false;
		group.DOFade(0f, duration).OnComplete(delegate
		{
			done = true;
		});
		while (!done)
		{
			await Awaiters.NextFrame;
		}
	}

	public async Task PlayVideo(VideoPlayer video)
	{
		video.targetCamera = Camera.main;
		video.Play();
		bool done = false;
		video.loopPointReached += delegate(VideoPlayer source)
		{
			done = true;
		};
		while (!done)
		{
			await Awaiters.NextFrame;
		}
		video.Pause();
	}

	private async Task BeginSplashAnimation()
	{
		if (SystemInfo.operatingSystem.Contains("Windows 7"))
		{
			this.t17Video.clip = this.video2k;
		}
		else
		{
			this.t17Video.clip = this.video4k;
		}
		await this.FadeIn(this.bsgLogoCanvasGroup, this.logoFadeDuration);
		await this.FadeIn(this.legalCanvasGroup, 0.35f);
		await Awaiters.Seconds(this.logoHoldDuration);
		await this.FadeOut(this.bsgLogoCanvasGroup, this.logoFadeDuration);
		await this.PlayVideo(this.t17Video);
		this.t17Video.gameObject.SetActive(false);
		await this.FadeOut(this.legalCanvasGroup, 0.35f);
		await Awaiters.MainThread;
		this.OnSplashComplete();
	}

	private void OnSplashComplete()
	{
		GameManager.Instance.Loader.LoadStartupFromSplash();
	}

	[SerializeField]
	private CanvasGroup bsgLogoCanvasGroup;

	[SerializeField]
	private CanvasGroup unityLogoCanvasGroup;

	[SerializeField]
	private CanvasGroup legalCanvasGroup;

	[SerializeField]
	private VideoClip video4k;

	[SerializeField]
	private VideoClip video2k;

	[SerializeField]
	private VideoPlayer t17Video;

	[SerializeField]
	private CanvasGroup t17VideoCanvasGroup;

	[SerializeField]
	private float logoFadeDuration;

	[SerializeField]
	private float logoHoldDuration;
}
