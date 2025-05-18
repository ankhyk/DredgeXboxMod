using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cetacean : MonoBehaviour
{
	public bool CanJump
	{
		get
		{
			return this.canJump;
		}
		set
		{
			this.canJump = value;
		}
	}

	public Animator Animator
	{
		get
		{
			return this.animator;
		}
	}

	private void Awake()
	{
		this.timeUntilNextJump = global::UnityEngine.Random.Range(this.timeBetweenJumpsMin, this.timeBetweenJumpsMax);
		this.animator.speed = this.animatorSpeed;
		if (this.animationEvents)
		{
			AnimationEvents animationEvents = this.animationEvents;
			animationEvents.OnSignalFired = (Action)Delegate.Combine(animationEvents.OnSignalFired, new Action(this.OnJumpSignalFired));
		}
	}

	private void OnDestroy()
	{
		if (this.animationEvents)
		{
			AnimationEvents animationEvents = this.animationEvents;
			animationEvents.OnSignalFired = (Action)Delegate.Remove(animationEvents.OnSignalFired, new Action(this.OnJumpSignalFired));
		}
	}

	private void OnJumpSignalFired()
	{
		if (this.jumpParticles)
		{
			this.jumpParticles.Play();
		}
	}

	private void Update()
	{
		if (this.isAllowedToJump && this.canJump)
		{
			this.timeUntilNextJump -= Time.deltaTime;
			if (this.timeUntilNextJump <= 0f)
			{
				this.timeUntilNextJump = global::UnityEngine.Random.Range(this.timeBetweenJumpsMin, this.timeBetweenJumpsMax);
				this.animator.SetTrigger("jump");
				if (this.jumpAudio)
				{
					base.StartCoroutine(this.DelayedJumpSound());
				}
			}
		}
	}

	private IEnumerator DelayedJumpSound()
	{
		yield return new WaitForSeconds(this.jumpAudioDelaySec);
		this.jumpAudio.PlayOneShot(this.jumpClips.PickRandom<AudioClip>());
		yield break;
	}

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private ParticleSystem jumpParticles;

	[SerializeField]
	private AudioSource jumpAudio;

	[SerializeField]
	private List<AudioClip> jumpClips;

	[SerializeField]
	private float jumpAudioDelaySec;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float timeBetweenJumpsMin;

	[SerializeField]
	private float timeBetweenJumpsMax;

	[SerializeField]
	private float animatorSpeed = 1f;

	[SerializeField]
	private bool isAllowedToJump = true;

	private bool canJump;

	private float timeUntilNextJump;
}
