using System;
using UnityEngine;

public class TutorialStepDialogueCondition : TutorialStepCondition
{
	public override void Subscribe(Action<TutorialStepData> callback, TutorialStepData linkedData)
	{
		base.Subscribe(callback, linkedData);
		GameEvents.Instance.OnNodeVisited += this.OnNodeVisited;
	}

	public override void Unsubscribe()
	{
		GameEvents.Instance.OnNodeVisited -= this.OnNodeVisited;
		base.Unsubscribe();
	}

	private void OnNodeVisited(string nodeName)
	{
		this.Evaluate();
	}

	public override void Evaluate()
	{
		base.SetConditionMet(GameManager.Instance.DialogueRunner.GetHasVisitedNode(this.nodeName));
	}

	[SerializeField]
	public string nodeName;
}
