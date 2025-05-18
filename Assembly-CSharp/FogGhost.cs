using System;
using UnityEngine;

public class FogGhost : WorldEvent
{
	private void Start()
	{
		GameManager.Instance.WorldEventManager.RegisterStaticWorldEvent(WorldEventType.FOG_GHOST, this);
		base.gameObject.SetActive(false);
	}

	public override void Activate()
	{
		base.gameObject.SetActive(true);
		this.hasBeenSeen = false;
		this.fadingOut = false;
		this.animator.Play("FogGhost_FadeIn");
		base.Activate();
		this.RequestEventFinish();
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
	}

	private void OnFinaleVoyageStarted()
	{
		this.RequestEventFinish();
	}

	private void Update()
	{
		if (!this.hasBeenSeen && GameManager.Instance.Player)
		{
			float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
			if (this.ghostRenderer.isVisible && num < this.seenMaxDistanceThreshold)
			{
				this.hasBeenSeen = true;
				if (!this.fadingOut && this.manuallyFadeOutIfCloserThanSeenDistance)
				{
					this.ManuallyFadeOut();
				}
			}
		}
		if (!this.fadingOut && this.hasBeenSeen && !this.ghostRenderer.isVisible)
		{
			this.ManuallyFadeOut();
		}
	}

	private void ManuallyFadeOut()
	{
		this.fadingOut = true;
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Combine(animationEvents.OnComplete, new Action(this.OnFadeOutComplete));
		this.animator.Play("FogGhost_FadeOut");
	}

	private void OnFadeOutComplete()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnComplete = (Action)Delegate.Remove(animationEvents.OnComplete, new Action(this.OnFadeOutComplete));
		base.gameObject.SetActive(false);
	}

	public override void RequestEventFinish()
	{
		base.RequestEventFinish();
		this.EventFinished();
	}

	[SerializeField]
	private GameObject ghostObject;

	[SerializeField]
	private Renderer ghostRenderer;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private bool manuallyFadeOutIfCloserThanSeenDistance;

	[SerializeField]
	private float seenMaxDistanceThreshold;

	private bool hasBeenSeen;

	private bool fadingOut;
}
