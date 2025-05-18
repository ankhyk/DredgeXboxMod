using System;
using System.Collections;
using System.Collections.Generic;
using ChromaSDK;
using UnityEngine;

public class ChromaManager : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.ChromaManager = this;
	}

	public IEnumerator Start()
	{
		if (!ChromaAnimationAPI.IsChromaSDKAvailable())
		{
			this._mResult = 6023;
			yield break;
		}
		ChromaAnimationAPI._sStreamingAssetPath = Application.streamingAssetsPath;
		APPINFOTYPE appinfotype = default(APPINFOTYPE);
		appinfotype.Title = "DREDGE";
		appinfotype.Description = "A cosmic horror fishing adventure.";
		appinfotype.Author_Name = "Black Salt Games";
		appinfotype.Author_Contact = "contact@blacksaltgames.com";
		appinfotype.SupportedDevice = 63U;
		appinfotype.Category = 1U;
		this._mResult = ChromaAnimationAPI.InitSDK(ref appinfotype);
		int mResult = this._mResult;
		if (mResult != 0)
		{
			if (mResult != 6023 && mResult != 6033)
			{
			}
		}
		else
		{
			this._mInitialized = true;
			this._mSupportsStreaming = ChromaAnimationAPI.CoreStreamSupportsStreaming();
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	public void OnApplicationQuit()
	{
		if (this._mResult == 0)
		{
			ChromaAnimationAPI.StopAll();
			ChromaAnimationAPI.CloseAll();
			bool flag = ChromaAnimationAPI.Uninit() != 0;
			ChromaAnimationAPI.UninitAPI();
			if (flag)
			{
				CustomDebug.EditorLogError("Failed to uninitialize Chroma!");
			}
		}
	}

	public void PlayAnimation(ChromaManager.DredgeChromaAnimation animationId)
	{
		if (this._mInitialized)
		{
			this.currentAnimation = animationId;
			this.animationSuffixes.ForEach(delegate(string suffix)
			{
				string text = string.Format("RazerChromaAnimations/{0}/{1}.chroma", animationId, suffix);
				ChromaAnimationAPI.CloseAnimationName(text);
				ChromaAnimationAPI.GetAnimation(text);
				ChromaAnimationAPI.PlayAnimationName(text, true);
			});
		}
	}

	public void StopAllAnimations()
	{
		this.PlayAnimation(ChromaManager.DredgeChromaAnimation.DEFAULT);
	}

	private bool _mInitialized;

	private int _mResult;

	private bool _mSupportsStreaming;

	private List<string> animationSuffixes = new List<string> { "Keyboard", "ChromaLink", "Headset", "Mousepad", "Mouse", "Keypad" };

	public ChromaManager.DredgeChromaAnimation currentAnimation;

	public enum DredgeChromaAnimation
	{
		DEFAULT,
		INTRO_1,
		INTRO_2,
		INTRO_3,
		FISHING,
		SAILING,
		ABILITY_SELECT,
		SPYGLASS,
		POPUP_WINDOW
	}
}
