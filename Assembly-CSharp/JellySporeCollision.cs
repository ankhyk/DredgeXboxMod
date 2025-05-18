using System;
using UnityEngine;

public class JellySporeCollision : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (this.hasTriggered)
		{
			return;
		}
		if (other.gameObject.tag == "Player")
		{
			GameManager.Instance.GridManager.InfectRandomItemInInventory();
			this.hasTriggered = true;
		}
	}

	private bool hasTriggered;
}
