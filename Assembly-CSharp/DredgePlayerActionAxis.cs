using System;
using InControl;

public class DredgePlayerActionAxis : DredgePlayerActionBase
{
	public DredgePlayerActionAxis(string promptName, PlayerOneAxisAction action)
		: base(promptName, action)
	{
	}

	public override void Reset()
	{
		base.Reset();
	}

	public float Value
	{
		get
		{
			return (this.playerAction as PlayerOneAxisAction).Value;
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
		return (this.playerAction as PlayerOneAxisAction).negativeAction;
	}

	public override PlayerAction GetSecondaryPlayerAction()
	{
		return (this.playerAction as PlayerOneAxisAction).positiveAction;
	}
}
