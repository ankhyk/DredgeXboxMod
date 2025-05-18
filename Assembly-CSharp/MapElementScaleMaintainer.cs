using System;
using System.Collections.Generic;
using UnityEngine;

public class MapElementScaleMaintainer : MonoBehaviour
{
	public bool IsDirty { get; set; }

	private void OnEnable()
	{
		this.IsDirty = true;
	}

	private void LateUpdate()
	{
		if (this.IsDirty)
		{
			this.newScale = this.fixedScale / this.mapTransform.localScale.x;
			Vector3 vector = new Vector3(this.newScale, this.newScale, this.newScale);
			foreach (Transform transform in this.transforms)
			{
				if (transform)
				{
					transform.localScale = vector;
				}
			}
			this.IsDirty = false;
		}
	}

	public void AddElement(RectTransform transform)
	{
		this.transforms.Add(transform);
		this.IsDirty = true;
	}

	public void RemoveElement(RectTransform transform)
	{
		this.transforms.Remove(transform);
		this.IsDirty = true;
	}

	[SerializeField]
	private List<RectTransform> transforms;

	[SerializeField]
	private RectTransform mapTransform;

	[SerializeField]
	private float fixedScale = 1f;

	private float newScale = 1f;
}
