using System;
using UnityEngine;

public class Spinner : MonoBehaviour
{
	private void Update()
	{
		base.transform.eulerAngles = new Vector3(0f, 0f, base.transform.eulerAngles.z + this.degreesPerSecond * Time.unscaledDeltaTime);
	}

	[SerializeField]
	private float degreesPerSecond;
}
