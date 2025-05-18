using System;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectByDialogueNodeVisit : MonoBehaviour
{
	private void OnEnable()
	{
		bool visited = GameManager.Instance.DialogueRunner.GetHasVisitedNode(this.dialogueNode);
		this.gameObjects.ForEach(delegate(GameObject go)
		{
			go.SetActive(visited ? this.stateIfVisited : (!this.stateIfVisited));
		});
	}

	[SerializeField]
	private string dialogueNode;

	[SerializeField]
	private bool stateIfVisited;

	[SerializeField]
	private List<GameObject> gameObjects;
}
