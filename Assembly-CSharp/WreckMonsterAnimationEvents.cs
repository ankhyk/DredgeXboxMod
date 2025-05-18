using System;
using UnityEngine;

public class WreckMonsterAnimationEvents : AnimationEvents
{
	private void Awake()
	{
		this.audioSource.spatialBlend = 1f;
		this.audioSource.maxDistance = 100f;
		this.audioSource.minDistance = 25f;
		this.audioSource.dopplerLevel = 0f;
	}

	public void PlaySFXEmerge()
	{
		this.audioSource.PlayOneShot(this.emergeSFX);
	}

	public void PlaySFXSlamAttack()
	{
		this.audioSource.PlayOneShot(this.slamAttackSFX);
	}

	public void PlaySFXSwipeAttack()
	{
		this.audioSource.PlayOneShot(this.swipeAttackSFX);
	}

	public void PlaySFXRetreat()
	{
		this.audioSource.PlayOneShot(this.retreatSFX);
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip emergeSFX;

	[SerializeField]
	private AudioClip slamAttackSFX;

	[SerializeField]
	private AudioClip swipeAttackSFX;

	[SerializeField]
	private AudioClip retreatSFX;
}
