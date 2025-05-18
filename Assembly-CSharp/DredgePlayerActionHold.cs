using System;
using InControl;
using UnityEngine;

public class DredgePlayerActionHold : DredgePlayerActionBase
{
	public DredgePlayerActionHold(string promptString, PlayerAction playerAction, float holdTime)
		: base(promptString, playerAction)
	{
		this.holdTimeRequiredSec = holdTime;
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
		float num = this.currentHoldTime;
		if (this.playerAction.IsPressed || this.forcePressed)
		{
			this.currentHoldTime += Time.unscaledDeltaTime;
		}
		else if (!this.playerAction.IsPressed && !this.maintainHoldProgress)
		{
			this.currentHoldTime -= Time.unscaledDeltaTime;
		}
		if (this.usesHoldSFX && GameManager.Instance && GameManager.Instance.AudioPlayer)
		{
			if (this.playerAction.WasPressed)
			{
				GameManager.Instance.AudioPlayer.SetIsButtonHeld(true);
			}
			if (this.playerAction.WasReleased)
			{
				GameManager.Instance.AudioPlayer.SetIsButtonHeld(false);
			}
		}
		this.currentHoldTime = Mathf.Max(this.currentHoldTime, 0f);
		if ((this.currentHoldTime == 0f) & (num > 0f))
		{
			Action onPressHoldEmptied = this.OnPressHoldEmptied;
			if (onPressHoldEmptied != null)
			{
				onPressHoldEmptied();
			}
		}
		if (this.currentHoldTime >= this.holdTimeRequiredSec)
		{
			Action onPressComplete = this.OnPressComplete;
			if (onPressComplete != null)
			{
				onPressComplete();
			}
			this.forcePressed = false;
			if (GameManager.Instance && GameManager.Instance.AudioPlayer)
			{
				GameManager.Instance.AudioPlayer.SetIsButtonHeld(false);
				if (this.usesHoldSFX)
				{
					GameManager.Instance.AudioPlayer.OnHoldActionComplete();
				}
			}
		}
	}

	public override void Reset()
	{
		base.Reset();
		this.currentHoldTime = 0f;
		if (GameManager.Instance && GameManager.Instance.AudioPlayer)
		{
			GameManager.Instance.AudioPlayer.SetIsButtonHeld(false);
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

	public Action OnPressHoldEmptied;

	public float holdTimeRequiredSec;

	public bool usesHoldSFX = true;

	public float currentHoldTime;

	public bool maintainHoldProgress;

	public bool showAlertOnHold;
}
