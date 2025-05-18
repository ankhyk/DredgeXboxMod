using System;
using UnityEngine;

public class OozeEvent : MonoBehaviour
{
	public virtual void RequestEventFinish()
	{
		this.finishRequested = true;
	}

	public Action<OozeEvent> OnOozeEventComplete;

	protected bool finishRequested;
}
