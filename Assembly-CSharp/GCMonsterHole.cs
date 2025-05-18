using System;
using UnityEngine;

public class GCMonsterHole : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Monster"))
		{
			GameManager.Instance.MonsterManager.GaleCliffsMonsterManager.OnMonsterReachedHole(this.linkedHole, this.linkedIsland);
		}
	}

	[SerializeField]
	private GaleCliffsMonsterHole linkedHole;

	[SerializeField]
	private GaleCliffsIsland linkedIsland;
}
