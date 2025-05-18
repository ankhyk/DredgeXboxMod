using System;
using UnityEngine;

public abstract class WorldEvent : MonoBehaviour
{
	public WorldEventData worldEventData { get; set; }

	public virtual void Activate()
	{
		this.finishRequested = false;
	}

	public virtual void RequestEventFinish()
	{
		this.finishRequested = true;
	}

	public virtual void EventFinished()
	{
		Action<WorldEvent> onEventFinished = this.OnEventFinished;
		if (onEventFinished == null)
		{
			return;
		}
		onEventFinished(this);
	}

	public Action<WorldEvent> OnEventFinished;

	protected bool finishRequested;
}
