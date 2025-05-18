using System;
using UnityEngine;

public class ZoneCollider : MonoBehaviour
{
	public ZoneEnum Zone
	{
		get
		{
			return this.zone;
		}
	}

	[SerializeField]
	private ZoneEnum zone;
}
