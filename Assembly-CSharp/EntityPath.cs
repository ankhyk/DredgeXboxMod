using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityPath : MonoBehaviour
{
	public List<Transform> Route
	{
		get
		{
			return this.route;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		for (int i = 0; i < this.route.Count - 1; i++)
		{
			Gizmos.DrawLine(this.route[i].position, this.route[i + 1].position);
		}
		if (this.loopGizmos)
		{
			Gizmos.DrawLine(this.route[this.route.Count - 1].position, this.route[0].position);
		}
	}

	[SerializeField]
	private List<Transform> route;

	[SerializeField]
	private bool loopGizmos;
}
