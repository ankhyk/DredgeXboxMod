using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BanishAbility : Ability
{
	public override bool Activate()
	{
		this.vfxParticles.main.startLifetime = this.abilityData.duration;
		this.mainSFX.volume = 1f;
		this.mainSFX.Play();
		this.vfx.SetActive(true);
		if (this.delayedDeactivateCoroutine != null)
		{
			base.StopCoroutine(this.delayedDeactivateCoroutine);
		}
		this.delayedDeactivateCoroutine = base.StartCoroutine(this.DelayedDeactivate());
		GameManager.Instance.Player.Sanity.ChangeSanity(this.sanityLossOnActivate);
		this.isActive = true;
		base.Activate();
		return true;
	}

	private IEnumerator DelayedDeactivate()
	{
		yield return new WaitForSeconds(this.abilityData.duration);
		GameManager.Instance.AudioPlayer.PlaySFX(this.endSFX, AudioLayer.SFX_PLAYER, this.endSFXVolume, 1f);
		this.mainSFX.DOFade(0f, 0.5f).OnComplete(delegate
		{
			this.mainSFX.Stop();
		});
		yield return new WaitForSeconds(this.animationEndDuration);
		this.Deactivate();
		yield break;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		this.vfx.SetActive(false);
	}

	private void Update()
	{
		if (this.isActive && !GameManager.Instance.VibrationManager.IsPlayingVibration())
		{
			GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, false);
		}
	}

	[SerializeField]
	private GameObject vfx;

	[SerializeField]
	private float animationEndDuration;

	[SerializeField]
	private ParticleSystem vfxParticles;

	[SerializeField]
	private Coroutine delayedDeactivateCoroutine;

	[SerializeField]
	private float sanityLossOnActivate;

	[SerializeField]
	private AudioSource mainSFX;

	[SerializeField]
	private AssetReference endSFX;

	[SerializeField]
	private float endSFXVolume;
}
