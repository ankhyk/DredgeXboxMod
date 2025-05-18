using System;
using UnityEngine;

public class DistanceScaler : MonoBehaviour
{
	private void Update()
	{
		float num = base.transform.position.x - Camera.main.transform.position.x;
		float num2 = base.transform.position.z - Camera.main.transform.position.z;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		Vector3 vector = Vector3.one;
		vector += Vector3.one * num3 * this.scale;
		base.transform.localScale = vector;
	}

	public float scale = 0.1f;
}
