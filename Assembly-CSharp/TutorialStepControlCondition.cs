using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

public class TutorialStepControlCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		this.playerActions = new List<PlayerAction>();
		for (int i = 0; i < this.dismissControls.Count; i++)
		{
			DredgeControlEnum dredgeControlEnum = this.dismissControls[i];
			PlayerAction playerAction = GameManager.Instance.Input.Controls.GetPlayerAction(dredgeControlEnum);
			this.playerActions.Add(playerAction);
		}
	}

	public override void Unsubscribe()
	{
		this.playerActions.Clear();
		this.heldTime = 0f;
		base.Unsubscribe();
	}

	public override void Update(float deltaTime)
	{
		if (this.useHoldTime)
		{
			if (this.playerActions.Any((PlayerAction pa) => pa.IsPressed))
			{
				this.heldTime += deltaTime;
				if (this.heldTime > this.targetHoldTime)
				{
					base.SetConditionMet(true);
				}
			}
			else if (this.loseProgressOverTime)
			{
				this.heldTime -= deltaTime;
				this.heldTime = Mathf.Max(this.heldTime, 0f);
			}
		}
		if (this.useNumPresses)
		{
			if (this.playerActions.Any((PlayerAction pa) => pa.WasPressed))
			{
				this.numPresses++;
				if (this.numPresses >= this.numPressesRequired)
				{
					base.SetConditionMet(true);
				}
			}
		}
	}

	public override float GetProgress()
	{
		if (this.useHoldTime)
		{
			return this.heldTime / this.targetHoldTime;
		}
		if (this.useNumPresses)
		{
			return (float)this.numPresses / (float)this.numPressesRequired;
		}
		return 0f;
	}

	public override void Evaluate()
	{
	}

	[SerializeField]
	public bool useHoldTime;

	[SerializeField]
	public float targetHoldTime;

	[SerializeField]
	public bool loseProgressOverTime;

	[SerializeField]
	public bool useNumPresses;

	[SerializeField]
	public int numPressesRequired;

	[SerializeField]
	private List<DredgeControlEnum> dismissControls = new List<DredgeControlEnum>();

	private float heldTime;

	private int numPresses;

	private List<PlayerAction> playerActions;
}
