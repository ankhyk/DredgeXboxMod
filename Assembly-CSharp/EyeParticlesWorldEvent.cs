using System;
using DG.Tweening;
using UnityEngine;

public class EyeParticlesWorldEvent : WorldEvent
{
	public override void Activate()
	{
		float num = Mathf.InverseLerp(base.worldEventData.maxSanity, base.worldEventData.minSanity, GameManager.Instance.Player.Sanity.CurrentSanity);
		this.duration = Mathf.Lerp(this.durationMin, this.durationMax, num);
		this.targetMaxParticleCount = Mathf.Lerp(this.particleCountMin, this.particleCountMax, num);
		if (this.particleTween != null)
		{
			this.particleTween.Kill(false);
			this.particleTween = null;
		}
		this.particleTween = DOTween.To(() => this.currentMaxParticleCount, delegate(float x)
		{
			this.currentMaxParticleCount = x;
		}, this.targetMaxParticleCount, this.eventFadeInDuration);
		this.particleTween.OnUpdate(new TweenCallback(this.TweenUpdate));
		this.audioSource.Play();
		this.audioSource.DOFade(this.volumeMax, this.volumeFadeInDuration);
		base.Activate();
	}

	private void TweenUpdate()
	{
		this.eyeParticles.main.maxParticles = (int)this.currentMaxParticleCount;
	}

	private void Update()
	{
		if (!this.finishRequested)
		{
			this.duration -= Time.deltaTime;
			if (this.duration <= 0f)
			{
				this.RequestEventFinish();
			}
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			if (this.particleTween != null)
			{
				this.particleTween.Kill(false);
				this.particleTween = null;
			}
			this.audioSource.DOFade(0f, this.eventFadeOutDuration);
			this.particleTween = DOTween.To(() => this.currentMaxParticleCount, delegate(float x)
			{
				this.currentMaxParticleCount = x;
			}, 0f, this.eventFadeOutDuration);
			this.particleTween.OnComplete(new TweenCallback(this.DelayedEventFinish));
			this.particleTween.OnUpdate(new TweenCallback(this.TweenUpdate));
		}
	}

	private void DelayedEventFinish()
	{
		this.particleTween.Kill(false);
		this.particleTween = null;
		this.eyeParticles.main.maxParticles = 0;
		this.audioSource.Stop();
		this.EventFinished();
	}

	[SerializeField]
	private float particleCountMax;

	[SerializeField]
	private float particleCountMin;

	[SerializeField]
	private float durationMax;

	[SerializeField]
	private float durationMin;

	[SerializeField]
	private ParticleSystem eyeParticles;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float volumeMax;

	[SerializeField]
	private float volumeFadeInDuration;

	[SerializeField]
	private float eventFadeInDuration;

	[SerializeField]
	private float eventFadeOutDuration;

	private float targetMaxParticleCount;

	private float currentMaxParticleCount;

	private Tweener particleTween;

	private float duration;
}
