using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestStepToggleTriggerable : QuestStepTriggerable
{
	public override void Trigger()
	{
		this.gameObjects.ForEach(delegate(GameObject go)
		{
			go.SetActive(this.shouldEnable);
		});
	}

	[SerializeField]
	private List<GameObject> gameObjects;

	[SerializeField]
	private bool shouldEnable;
}
