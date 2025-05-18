using System;
using UnityEngine;

public class TutorialStepDockCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
	}

	public override void Unsubscribe()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		base.Unsubscribe();
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		this.Evaluate();
	}

	public override void Evaluate()
	{
		if (GameManager.Instance.Player)
		{
			Dock currentDock = GameManager.Instance.Player.CurrentDock;
			base.SetConditionMet((currentDock && this.triggerOnDock) || (currentDock == null && this.triggerOnUndock));
		}
	}

	[SerializeField]
	public bool triggerOnDock;

	[SerializeField]
	public bool triggerOnUndock;
}
