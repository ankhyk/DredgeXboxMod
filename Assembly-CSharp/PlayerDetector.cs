using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	protected virtual void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.collidersHit++;
			if (this.collidersHit > 0 && this.cachedCollidersHit == 0)
			{
				Action onPlayerDetected = this.OnPlayerDetected;
				if (onPlayerDetected != null)
				{
					onPlayerDetected();
				}
			}
			this.cachedCollidersHit = this.collidersHit;
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.collidersHit++;
			if (this.collidersHit > 0 && this.cachedCollidersHit == 0)
			{
				Action onPlayerDetected = this.OnPlayerDetected;
				if (onPlayerDetected != null)
				{
					onPlayerDetected();
				}
			}
			this.cachedCollidersHit = this.collidersHit;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.collidersHit--;
			if (this.collidersHit == 0 && this.cachedCollidersHit > 0)
			{
				Action onPlayerExitDetected = this.OnPlayerExitDetected;
				if (onPlayerExitDetected != null)
				{
					onPlayerExitDetected();
				}
			}
			this.collidersHit = Mathf.Max(this.collidersHit, 0);
			this.cachedCollidersHit = this.collidersHit;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.collidersHit--;
			if (this.collidersHit == 0 && this.cachedCollidersHit > 0)
			{
				Action onPlayerExitDetected = this.OnPlayerExitDetected;
				if (onPlayerExitDetected != null)
				{
					onPlayerExitDetected();
				}
			}
			this.collidersHit = Mathf.Max(this.collidersHit, 0);
			this.cachedCollidersHit = this.collidersHit;
		}
	}

	public void Reset()
	{
		this.collidersHit = 0;
		this.cachedCollidersHit = 0;
		Action onPlayerExitDetected = this.OnPlayerExitDetected;
		if (onPlayerExitDetected == null)
		{
			return;
		}
		onPlayerExitDetected();
	}

	public Action OnPlayerDetected;

	public Action OnPlayerExitDetected;

	private int collidersHit;

	private int cachedCollidersHit;
}
