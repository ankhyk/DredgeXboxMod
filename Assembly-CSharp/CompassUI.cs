using System;
using System.Collections.Generic;
using UnityEngine;

public class CompassUI : MonoBehaviour
{
	private void Awake()
	{
		this.cam = Camera.main.transform;
	}

	private void Update()
	{
		if (this.cam != null)
		{
			Vector3 zero = Vector3.zero;
			if (this.skewX)
			{
				zero.x = 55f - this.cam.eulerAngles.x;
			}
			else
			{
				zero.x = this.compassFace.eulerAngles.x;
			}
			zero.z = this.cam.eulerAngles.y;
			this.compassFace.eulerAngles = zero;
			this.markerObjects.ForEach(delegate(Transform o)
			{
				o.transform.eulerAngles = Vector3.zero;
			});
		}
	}

	[SerializeField]
	private List<Transform> markerObjects;

	[SerializeField]
	private Transform compassFace;

	[SerializeField]
	private bool skewX;

	private Transform cam;
}
