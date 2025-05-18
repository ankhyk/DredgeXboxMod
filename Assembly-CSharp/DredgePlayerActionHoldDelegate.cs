using System;
using InControl;
using UnityEngine;

public class DredgePlayerActionHoldDelegate : DredgePlayerActionBase
{
	public DredgePlayerActionHoldDelegate(string promptString, PlayerAction playerAction)
		: base(promptString, playerAction)
	{
	}

	public override void Update()
	{
		base.Update();
		if (!this.enabled)
		{
			return;
		}
		if (!this.canProcessEvents)
		{
			return;
		}
		if (!this.hasDispatchedPressBeginEvent && !this.allowPreholding)
		{
			return;
		}
		if (this.playerAction.IsPressed || this.forcePressed)
		{
			this.currentHoldTime += Time.deltaTime;
		}
		else
		{
			this.currentHoldTime = 0f;
		}
		if (this.hasBeenResolved)
		{
			Action onPressComplete = this.OnPressComplete;
			if (onPressComplete != null)
			{
				onPressComplete();
			}
			this.forcePressed = false;
		}
	}

	public void Resolve()
	{
		this.hasBeenResolved = true;
	}

	public override void Reset()
	{
		base.Reset();
		this.currentHoldTime = 0f;
		this.hasBeenResolved = false;
	}

	public override PlayerAction GetPrimaryPlayerAction()
	{
		return this.playerAction as PlayerAction;
	}

	public override PlayerAction GetSecondaryPlayerAction()
	{
		return null;
	}

	public bool hasBeenResolved;

	public bool hasDelegatedResolution = true;

	public float currentHoldTime;
}
