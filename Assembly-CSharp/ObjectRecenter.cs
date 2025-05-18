using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRecenter : MonoBehaviour
{
	private void Recenter()
	{
		List<Transform> list = new List<Transform>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform);
		}
		Vector3 vector = Vector3.zero;
		foreach (Transform transform2 in list)
		{
			vector += transform2.position;
			transform2.parent = null;
		}
		vector /= (float)list.Count;
		base.transform.position = vector;
		foreach (Transform transform3 in list)
		{
			transform3.parent = base.transform;
		}
	}
}
