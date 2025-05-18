using System;
using System.Collections.Generic;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class TwistedStrandTrapAnimationEvents : MonoBehaviour
{
	public void MortarFire()
	{
		this.mortarFireParticles.Play();
		GameManager.Instance.VibrationManager.Vibrate(this.mortarFireVibration, VibrationRegion.WholeBody, true).Run();
	}

	public void MortarHit()
	{
		this.mortarHitParticles.Play();
		GameManager.Instance.VibrationManager.Vibrate(this.mortarHitVibration, VibrationRegion.WholeBody, true).Run();
	}

	public void StartTrap()
	{
		this.trapAnimator.SetBool("TrapActive", true);
		this.monsterAnimator.SetTrigger("TriggerTrap");
	}

	public void ResetTrap()
	{
		this.trapAnimator.SetBool("TrapActive", false);
	}

	public void SetDestroyed()
	{
		this.mainTrapAnimator.SetBool("IsDestroyed", true);
	}

	public void StartMonsterAnimations()
	{
		this.monsterAnimator.SetTrigger("Start");
	}

	public void StartBaitEat()
	{
		this.monsterAnimator.SetTrigger("Eat");
	}

	public void PlayEmergeSFX()
	{
		this.trapAudioSource.PlayOneShot(this.emergeSFXClips.PickRandom<AudioClip>(), 1f);
	}

	public void PlayEmergeCallSFX()
	{
		this.trapAudioSource.PlayOneShot(this.emergeCallSFXClips.PickRandom<AudioClip>(), 1f);
	}

	public void PlayEatingSFX()
	{
		this.trapAudioSource.PlayOneShot(this.eatingSFXClip, 1f);
	}

	public void PlayTriggerSFX()
	{
		this.trapAudioSource.PlayOneShot(this.triggerSFXClip, 1f);
	}

	public void PlayTrappedSFX()
	{
		this.trapAudioSource.PlayOneShot(this.trappedSFXClips.PickRandom<AudioClip>(), 1f);
	}

	public void PlayFireSFX()
	{
		this.mortarAudioSource.Play();
	}

	public void PlayExplodeSFX()
	{
		this.trapAudioSource.PlayOneShot(this.explodeSFXClip, 1f);
	}

	[SerializeField]
	private AudioSource trapAudioSource;

	[SerializeField]
	private AudioSource mortarAudioSource;

	[SerializeField]
	private List<AudioClip> emergeSFXClips;

	[SerializeField]
	private List<AudioClip> emergeCallSFXClips;

	[SerializeField]
	private AudioClip eatingSFXClip;

	[SerializeField]
	private AudioClip triggerSFXClip;

	[SerializeField]
	private AudioClip explodeSFXClip;

	[SerializeField]
	private List<AudioClip> trappedSFXClips;

	[SerializeField]
	private Animator mainTrapAnimator;

	[SerializeField]
	private Animator trapAnimator;

	[SerializeField]
	private Animator monsterAnimator;

	[SerializeField]
	private ParticleSystem mortarFireParticles;

	[SerializeField]
	private ParticleSystem mortarHitParticles;

	[SerializeField]
	private VibrationData mortarFireVibration;

	[SerializeField]
	private VibrationData mortarHitVibration;
}
