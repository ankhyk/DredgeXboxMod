using System;
using InControl;

public class DredgePlayerActionPress : DredgePlayerActionBase
{
	public DredgePlayerActionPress(string promptName, PlayerAction action)
		: base(promptName, action)
	{
	}

	public override void Reset()
	{
		base.Reset();
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
		if (this.playerAction.WasPressed || this.forcePressed)
		{
			Action onPressComplete = this.OnPressComplete;
			if (onPressComplete != null)
			{
				onPressComplete();
			}
			this.forcePressed = false;
		}
	}

	public override PlayerAction GetPrimaryPlayerAction()
	{
		return this.playerAction as PlayerAction;
	}

	public override PlayerAction GetSecondaryPlayerAction()
	{
		return null;
	}
}
