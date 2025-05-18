using System;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerableTimeline : MonoBehaviour
{
	private void Awake()
	{
		GameEvents.Instance.OnAnimationStartRequested += this.OnAnimationTriggered;
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnAnimationStartRequested -= this.OnAnimationTriggered;
	}

	private void OnAnimationTriggered(string animationId)
	{
		if (animationId == this.animationId)
		{
			this.Play();
		}
	}

	private void Play()
	{
		this.director.Play();
	}

	public void OnTimelineComplete()
	{
		GameEvents.Instance.TriggerAnimationCompleted(this.animationId);
	}

	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private string animationId;
}
