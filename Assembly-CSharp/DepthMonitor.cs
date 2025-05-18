using System;
using UnityEngine;

public class DepthMonitor : MonoBehaviour
{
	public float CurrentDepth
	{
		get
		{
			return this.currentDepth;
		}
	}

	public void UpdateDepth()
	{
		this.currentDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPosition(base.transform.position);
	}

	private void Update()
	{
		this.timeSinceDepthUpdated += Time.deltaTime;
		if (this.timeSinceDepthUpdated > this.depthUpdateFrequencySec)
		{
			this.UpdateDepth();
			this.timeSinceDepthUpdated = 0f;
		}
	}

	[SerializeField]
	private float depthUpdateFrequencySec;

	private float currentDepth;

	private float timeSinceDepthUpdated = float.PositiveInfinity;

	private Vector2 samplePos;
}
