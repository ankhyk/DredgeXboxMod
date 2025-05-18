using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class OozeTumor : OozeEvent
{
	private void Awake()
	{
		this.lifetimeSec = global::UnityEngine.Random.Range(this.lifetimeMinSec, this.lifetimeMaxSec);
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnExplodeComplete));
		AnimationEvents animationEvents2 = this.animationEvent;
		animationEvents2.OnSignalFired = (Action)Delegate.Combine(animationEvents2.OnSignalFired, new Action(this.OnSignalFired));
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.QueueExplode));
		this.spawnTime = Time.time;
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		AnimationEvents animationEvents = this.animationEvent;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnExplodeComplete));
		AnimationEvents animationEvents2 = this.animationEvent;
		animationEvents2.OnSignalFired = (Action)Delegate.Remove(animationEvents2.OnSignalFired, new Action(this.OnSignalFired));
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if (enabled && ability.name == this.banishAbility.name && !this.isTriggered)
		{
			this.isTriggered = true;
			this.animator.SetTrigger("banish");
		}
	}

	private void Update()
	{
		if (!this.isTriggered && Time.time > this.spawnTime + this.lifetimeSec)
		{
			this.QueueExplode();
		}
	}

	public override void RequestEventFinish()
	{
		this.QueueExplode();
		base.RequestEventFinish();
	}

	private void QueueExplode()
	{
		if (this.isTriggered)
		{
			return;
		}
		this.isTriggered = true;
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.QueueExplode));
		this.animator.SetTrigger("explode");
	}

	private void OnExplodeComplete()
	{
		Action<OozeEvent> onOozeEventComplete = this.OnOozeEventComplete;
		if (onOozeEventComplete != null)
		{
			onOozeEventComplete(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnSignalFired()
	{
		global::UnityEngine.Object.Instantiate<GameObject>(this.oozeSplashVFX, base.transform.position, Quaternion.identity);
		GameManager.Instance.AudioPlayer.PlaySFX(this.popSFX.PickRandom<AssetReference>(), base.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, 20f, 50f, false, false);
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float lifetimeMinSec;

	[SerializeField]
	private float lifetimeMaxSec;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private GameObject explosionCollider;

	[SerializeField]
	private AnimationEvents animationEvent;

	[SerializeField]
	private GameObject oozeSplashVFX;

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private List<AssetReference> popSFX = new List<AssetReference>();

	private bool isTriggered;

	private float lifetimeSec;

	private float spawnTime;
}
