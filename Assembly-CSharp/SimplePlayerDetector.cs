using System;
using UnityEngine;

public class SimplePlayerDetector : PlayerDetector
{
	protected override void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action onPlayerDetected = this.OnPlayerDetected;
			if (onPlayerDetected == null)
			{
				return;
			}
			onPlayerDetected();
		}
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Action onPlayerDetected = this.OnPlayerDetected;
			if (onPlayerDetected == null)
			{
				return;
			}
			onPlayerDetected();
		}
	}
}
