using System;
using UnityEngine;

public class GCMonsterSpawnTrigger : MonoBehaviour
{
	public RouteDirection Direction
	{
		get
		{
			return this.direction;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			GameManager.Instance.MonsterManager.GaleCliffsMonsterManager.TrySpawnFromTrigger(this.spawnRoute, this.direction);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			GameManager.Instance.MonsterManager.GaleCliffsMonsterManager.TrySpawnFromTrigger(this.spawnRoute, this.direction);
		}
	}

	[SerializeField]
	private EntityPath spawnRoute;

	[SerializeField]
	private RouteDirection direction;
}
