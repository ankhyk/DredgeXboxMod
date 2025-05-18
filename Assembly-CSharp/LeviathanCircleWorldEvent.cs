using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class LeviathanCircleWorldEvent : WorldEvent
{
	public override void Activate()
	{
		base.Activate();
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
		this.audioSource.PlayOneShot(this.leviathanAppearSFX, 1f);
		AudioClip audioClip = this.leviathanCallSFX.PickRandom<AudioClip>();
		this.audioSource.PlayOneShot(audioClip, 1f);
		this.rumbleAudioSource.DOFade(1f, 1f).From(0f, true, false);
		this.animationRunning = true;
	}

	public void Update()
	{
		if (this.animationRunning)
		{
			GameManager.Instance.VibrationManager.Vibrate(this.CircleVibration, VibrationRegion.WholeBody, false).Run();
		}
	}

	private void OnAnimationComplete()
	{
		this.animationRunning = false;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
		this.RequestEventFinish();
	}

	private void OnDestroy()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnAnimationComplete));
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			this.rumbleAudioSource.DOFade(0f, this.delayBeforeDestroying);
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.delayBeforeDestroying);
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioSource rumbleAudioSource;

	[SerializeField]
	private AudioClip leviathanAppearSFX;

	[SerializeField]
	private float delayBeforeDestroying;

	[SerializeField]
	private List<AudioClip> leviathanCallSFX;

	[SerializeField]
	private VibrationData CircleVibration;

	private bool animationRunning;
}
