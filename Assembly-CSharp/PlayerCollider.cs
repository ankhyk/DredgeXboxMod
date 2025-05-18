using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		bool flag = other.gameObject.CompareTag(this.safeColliderTag);
		if ((this.iceLayer.value & (1 << other.gameObject.layer)) > 0 && GameManager.Instance.SaveData.GetIsIcebreakerEquipped())
		{
			flag = true;
		}
		this.ProcessHit(flag, other.gameObject.CompareTag(this.monsterTag), other.gameObject.CompareTag(this.uniqueVibrationTag));
	}

	public void ProcessHit(bool isSafeCollider, bool isMonster, bool hasUniqueVibration)
	{
		if (!isSafeCollider)
		{
			if (Time.time > this.timeOfLastCollision + this.invulnerabilityTimeInSeconds)
			{
				UnityEvent onCollisionEvent = this.OnCollisionEvent;
				if (onCollisionEvent != null)
				{
					onCollisionEvent.Invoke();
				}
				this.timeOfLastCollision = Time.time;
				if (!hasUniqueVibration)
				{
					Action<bool> onCollisionVibrationEvent = this.OnCollisionVibrationEvent;
					if (onCollisionVibrationEvent == null)
					{
						return;
					}
					onCollisionVibrationEvent(isMonster);
				}
			}
			return;
		}
		UnityEvent onSafeCollisionEvent = this.OnSafeCollisionEvent;
		if (onSafeCollisionEvent == null)
		{
			return;
		}
		onSafeCollisionEvent.Invoke();
	}

	private float timeOfLastCollision;

	[SerializeField]
	private float invulnerabilityTimeInSeconds;

	[SerializeField]
	private string safeColliderTag;

	[SerializeField]
	private string monsterTag;

	[SerializeField]
	public string uniqueVibrationTag;

	public UnityEvent OnCollisionEvent;

	public UnityEvent OnSafeCollisionEvent;

	public Action<bool> OnCollisionVibrationEvent;

	public LayerMask iceLayer;
}
