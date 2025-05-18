using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class GDKPCScreenshotStrategy : IScreenshotStrategy
{
	public void Init()
	{
		Debug.Log("[GOGScreenshotStrategy] Init()");
	}

	public void UnInit()
	{
	}

	public void Update()
	{
	}

	public IEnumerator DoTakePhoto()
	{
		Debug.Log("[GDKPCScreenshotStrategy] DoTakePhoto()");
		this.waitingForScreenshot = true;
		this.shotTime = Time.unscaledTime;
		string text = DateTime.Now.ToString();
		text = text.Replace("/", "-").Replace(":", "-").Replace(" ", "-");
		string text2 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/DREDGE/Screenshots";
		Directory.CreateDirectory(text2);
		ScreenCapture.CaptureScreenshot(text2 + "/" + text + ".png");
		this.waitingForScreenshot = false;
		yield return new WaitUntil(() => !this.waitingForScreenshot || Time.unscaledTime > this.shotTime + this.timeoutTime);
		if (this.waitingForScreenshot)
		{
			Debug.LogWarning("[GDKPCScreenshotStrategy] DoTakePhoto() request timed out.");
		}
		yield break;
	}

	private bool waitingForScreenshot;

	private float shotTime;

	private float timeoutTime = 3f;
}
