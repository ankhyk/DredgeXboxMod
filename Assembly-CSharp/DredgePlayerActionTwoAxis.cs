using System;
using InControl;
using UnityEngine;

public class DredgePlayerActionTwoAxis : DredgePlayerActionBase
{
	public DredgePlayerActionTwoAxis(string promptName, PlayerTwoAxisAction action)
		: base(promptName, action)
	{
	}

	public override void Reset()
	{
		base.Reset();
	}

	public Vector2 Value
	{
		get
		{
			return (this.playerAction as PlayerTwoAxisAction).Value;
		}
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
		if (this.playerAction.WasPressed)
		{
			Action onPressComplete = this.OnPressComplete;
			if (onPressComplete == null)
			{
				return;
			}
			onPressComplete();
		}
	}

	public override PlayerAction GetPrimaryPlayerAction()
	{
		CustomDebug.EditorLogError("[DredgePlayerActionTwoAxis] Can't use GetPrimaryPlayerAction() on a two axis action");
		return null;
	}

	public override PlayerAction GetSecondaryPlayerAction()
	{
		CustomDebug.EditorLogError("[DredgePlayerActionTwoAxis] Can't use GetSecondaryPlayerAction() on a two axis action");
		return null;
	}
}
