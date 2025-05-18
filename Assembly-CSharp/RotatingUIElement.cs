using System;
using UnityEngine;

public class RotatingUIElement : MonoBehaviour
{
	private void Update()
	{
		if (this.ignorePause)
		{
			base.transform.Rotate(new Vector3(0f, 0f, -(this.zDegPerSec * Time.unscaledDeltaTime)));
			return;
		}
		base.transform.Rotate(new Vector3(0f, 0f, -(this.zDegPerSec * Time.deltaTime)));
	}

	[SerializeField]
	private float zDegPerSec;

	[SerializeField]
	private bool ignorePause;
}
