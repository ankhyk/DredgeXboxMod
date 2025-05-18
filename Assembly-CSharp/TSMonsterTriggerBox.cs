using System;
using System.Collections.Generic;
using UnityEngine;

public class TSMonsterTriggerBox : MonoBehaviour
{
	public int AssociatedMonsterId
	{
		get
		{
			return this.associatedMonsterId;
		}
	}

	public List<Transform> SpawnPointCandidates
	{
		get
		{
			return this.spawnPointCandidates;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action<TSMonsterTriggerBox> onPlayerDetected = this.OnPlayerDetected;
			if (onPlayerDetected == null)
			{
				return;
			}
			onPlayerDetected(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action<TSMonsterTriggerBox> onPlayerDetected = this.OnPlayerDetected;
			if (onPlayerDetected == null)
			{
				return;
			}
			onPlayerDetected(this);
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action onPlayerExitDetected = this.OnPlayerExitDetected;
			if (onPlayerExitDetected == null)
			{
				return;
			}
			onPlayerExitDetected();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action onPlayerExitDetected = this.OnPlayerExitDetected;
			if (onPlayerExitDetected == null)
			{
				return;
			}
			onPlayerExitDetected();
		}
	}

	private void OnDrawGizmos()
	{
		int hashCode = base.name.GetHashCode();
		int num = (hashCode & 16711680) >> 16;
		int num2 = (hashCode & 65280) >> 8;
		int num3 = hashCode & 255;
		Gizmos.color = new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f);
		for (int i = 0; i < this.spawnPointCandidates.Count; i++)
		{
			Gizmos.DrawLine(base.transform.position, this.spawnPointCandidates[i].transform.position);
		}
	}

	public Action<TSMonsterTriggerBox> OnPlayerDetected;

	public Action OnPlayerExitDetected;

	[SerializeField]
	private List<Transform> spawnPointCandidates;

	[SerializeField]
	private int associatedMonsterId;
}
