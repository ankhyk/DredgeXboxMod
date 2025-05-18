using System;
using UnityEngine;

public class POISpawner : MonoBehaviour
{
	public string Id
	{
		get
		{
			return this.uniqueId.Id;
		}
	}

	public float StartStock
	{
		get
		{
			return this.startStock;
		}
	}

	[SerializeField]
	protected UniqueId uniqueId;

	[SerializeField]
	protected float startStock;
}
