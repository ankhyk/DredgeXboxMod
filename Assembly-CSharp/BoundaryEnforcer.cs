using System;
using UnityEngine;

public class BoundaryEnforcer : MonoBehaviour
{
	private void Update()
	{
		if (this.teleportInProgress && Time.time > this.teleportCastTime + this.teleportCooldownSec)
		{
			this.teleportInProgress = false;
		}
		if (GameManager.Instance.Player == null || this.teleportInProgress)
		{
			return;
		}
		if (Time.time > this.timeOfLastCheck + this.checkIntervalSec)
		{
			this.timeOfLastCheck = Time.time;
			this.DoBoundaryCheck();
		}
	}

	private void DoBoundaryCheck()
	{
		float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
		if (num > this.mainHardBoundaryRadius && !this.hasSpawnedLeviathan && (GameManager.Instance.WorldEventManager.CurrentEvent == null || GameManager.Instance.WorldEventManager.CurrentEvent.name != this.leviathanAttackEvent.name))
		{
			GameManager.Instance.WorldEventManager.DoEvent(this.leviathanAttackEvent);
			this.hasSpawnedLeviathan = true;
		}
		if (num < this.mainSoftBoundaryRadius)
		{
			this.hasSpawnedLeviathan = false;
		}
		this.outOfBoundsWarning.SetActive(num > this.mainSoftBoundaryRadius);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.mainSoftBoundaryRadius);
		Gizmos.DrawWireSphere(base.transform.position, this.demoSoftBoundaryRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.mainHardBoundaryRadius);
		Gizmos.DrawWireSphere(base.transform.position, this.demoHardBoundaryRadius);
	}

	[SerializeField]
	private GameObject outOfBoundsWarning;

	[SerializeField]
	private float demoSoftBoundaryRadius;

	[SerializeField]
	private float demoHardBoundaryRadius;

	[SerializeField]
	private float mainSoftBoundaryRadius;

	[SerializeField]
	private float mainHardBoundaryRadius;

	[SerializeField]
	private AbilityData teleportAbility;

	[SerializeField]
	private WorldEventData leviathanAttackEvent;

	[SerializeField]
	private float checkIntervalSec;

	[SerializeField]
	private float teleportCooldownSec;

	private float teleportCastTime;

	private bool hasSpawnedLeviathan;

	private bool teleportInProgress;

	private float timeOfLastCheck;
}
