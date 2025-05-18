using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggleWorldStateAction : WorldStateAction
{
	public override void Do()
	{
		if (this.gameObjects != null)
		{
			this.gameObjects.ForEach(delegate(GameObject go)
			{
				if (go != null)
				{
					go.SetActive(this.enabledIfMet);
				}
			});
		}
	}

	public List<GameObject> gameObjects;

	public bool enabledIfMet;
}
