using System;
using UnityEngine;

public class PlayerProximityMonitor : MonoBehaviour
{
	public float CurrentProximity
	{
		get
		{
			return this.currentProximity;
		}
	}

	private void OnEnable()
	{
		this.UpdateProximity();
	}

	private void UpdateProximity()
	{
		if (GameManager.Instance.Player)
		{
			this.currentProximity = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
		}
	}

	private void Update()
	{
		this.timeSinceProximityUpdated += Time.deltaTime;
		if (this.timeSinceProximityUpdated > this.updateFrequencySec)
		{
			this.UpdateProximity();
			this.timeSinceProximityUpdated = 0f;
		}
	}

	[SerializeField]
	private float updateFrequencySec;

	private float currentProximity;

	private float timeSinceProximityUpdated = float.PositiveInfinity;

	private Vector2 samplePos;
}
