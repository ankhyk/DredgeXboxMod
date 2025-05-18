using System;
using UnityEngine;

public class SanityUI : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.playerSanity)
		{
			this.sanityAnimator.SetFloat("Sanity", this.playerSanity.CurrentSanity);
		}
	}

	private void Update()
	{
		if (this.playerSanity == null)
		{
			if (GameManager.Instance.Player != null)
			{
				this.playerSanity = GameManager.Instance.Player.Sanity;
				return;
			}
		}
		else
		{
			this.sanityAnimator.SetFloat("Sanity", this.playerSanity.CurrentSanity);
			float num = Mathf.Clamp01(7f * (0f - this.playerSanity.RateOfChange));
			ParticleSystem.EmissionModule emission = this.sanityIncreasingEffect.emission;
			ParticleSystem.MainModule main = this.sanityIncreasingEffect.main;
			emission.rateOverTime = Mathf.Lerp(0f, this.maxParticleIntensity, num);
			main.startSize = Mathf.Lerp(this.minSize, this.maxSize, num);
			main.startLifetime = Mathf.Lerp(this.minParticleLifetime, this.maxParticleLifetime, num);
		}
	}

	[SerializeField]
	private Animator sanityAnimator;

	[SerializeField]
	private ParticleSystem sanityIncreasingEffect;

	[SerializeField]
	private float maxParticleIntensity = 20f;

	[SerializeField]
	private float minSize = 90f;

	[SerializeField]
	private float maxSize = 140f;

	[SerializeField]
	private float minParticleLifetime = 1f;

	[SerializeField]
	private float maxParticleLifetime = 0.5f;

	private PlayerSanity playerSanity;
}
