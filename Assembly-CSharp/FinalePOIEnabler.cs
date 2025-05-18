using System;
using System.Collections.Generic;
using UnityEngine;

public class FinalePOIEnabler : MonoBehaviour
{
	private void Start()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler("EnableBadFinalePOI", new Action(this.EnableBadFinalePOI));
		GameManager.Instance.DialogueRunner.AddCommandHandler("EnableGoodFinalePOI", new Action(this.EnableGoodFinalePOI));
	}

	private void EnableBadFinalePOI()
	{
		this.badEnableObjects.ForEach(delegate(GameObject go)
		{
			go.SetActive(true);
		});
	}

	private void EnableGoodFinalePOI()
	{
		this.goodEnableObjects.ForEach(delegate(GameObject go)
		{
			go.SetActive(true);
		});
	}

	[SerializeField]
	private List<GameObject> badEnableObjects;

	[SerializeField]
	private List<GameObject> goodEnableObjects;
}
