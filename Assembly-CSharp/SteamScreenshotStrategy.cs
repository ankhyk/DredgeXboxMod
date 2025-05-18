using System;
using System.Collections;
using Steamworks;
using UnityEngine;

public class SteamScreenshotStrategy : IScreenshotStrategy
{
	public void Init()
	{
		this.m_screenshotReady = Callback<ScreenshotReady_t>.Create(new Callback<ScreenshotReady_t>.DispatchDelegate(this.OnScreenshotReady));
	}

	public void UnInit()
	{
		this.m_screenshotReady = null;
	}

	public void Update()
	{
	}

	public IEnumerator DoTakePhoto()
	{
		if (SteamManager.Initialized)
		{
			this.waitingForScreenshot = true;
			this.shotTime = Time.unscaledTime;
			SteamScreenshots.TriggerScreenshot();
			yield return new WaitUntil(() => !this.waitingForScreenshot || Time.unscaledTime > this.shotTime + this.timeoutTime);
			bool flag = this.waitingForScreenshot;
		}
		yield break;
	}

	private void OnScreenshotReady(ScreenshotReady_t pCallback)
	{
		this.waitingForScreenshot = false;
	}

	private bool waitingForScreenshot;

	private float shotTime;

	private float timeoutTime = 3f;

	protected Callback<ScreenshotReady_t> m_screenshotReady;
}
