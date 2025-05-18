using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	public void DestroySelf()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void AnimationComplete()
	{
		Action onComplete = this.OnComplete;
		if (onComplete == null)
		{
			return;
		}
		onComplete();
	}

	public void FireSignal()
	{
		Action onSignalFired = this.OnSignalFired;
		if (onSignalFired == null)
		{
			return;
		}
		onSignalFired();
	}

	public Action OnComplete;

	public Action OnSignalFired;
}
