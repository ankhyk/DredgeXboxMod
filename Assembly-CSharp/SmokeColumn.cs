using System;
using UnityEngine;

public class SmokeColumn : MonoBehaviour
{
	private void OnEnable()
	{
		this.InitialisePositions();
	}

	private void Update()
	{
		if (!this.hasInit)
		{
			this.InitialisePositions();
		}
		if (this.hasInit)
		{
			this.UpdatePositions();
		}
	}

	private void InitialisePositions()
	{
		this.smokeMaterial = this.line.material;
		this.distance += this.initialUvOffest;
		this.lerpSpeeds = new float[this.line.positionCount];
		for (int i = 0; i < this.line.positionCount; i++)
		{
			this.lerpSpeeds[i] = Mathf.Lerp(12f, 500f, Mathf.Pow(1f - (float)i / (float)this.line.positionCount, 20f));
			Vector3 vector = base.transform.position + (this.windDirection + Vector3.up) * ((float)i * this.positionSpacing);
			this.line.SetPosition(i, vector);
		}
		this.hasInit = true;
	}

	private void UpdatePositions()
	{
		this.distance += (base.transform.position - this.positionLast).magnitude * 0.75f;
		this.smokeMaterial.SetFloat("_Distance", this.distance);
		for (int i = 0; i < this.line.positionCount; i++)
		{
			Vector3 vector = Vector3.Lerp(this.line.GetPosition(i), base.transform.position + (this.windDirection + Vector3.up) * ((float)i * this.positionSpacing), Time.deltaTime * this.lerpSpeeds[i]);
			vector.y = (base.transform.position + (this.windDirection + Vector3.up) * ((float)i * this.positionSpacing)).y;
			this.line.SetPosition(i, vector);
		}
		this.positionLast = base.transform.position;
		base.transform.eulerAngles = Vector3.zero;
	}

	[SerializeField]
	private Vector3 windDirection;

	[SerializeField]
	private float positionSpacing = 1f;

	[SerializeField]
	private LineRenderer line;

	[SerializeField]
	private float initialUvOffest;

	private Material smokeMaterial;

	private float distance;

	private Vector3 positionLast;

	private float[] lerpSpeeds;

	private bool hasInit;
}
