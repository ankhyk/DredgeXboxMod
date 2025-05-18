using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoneDetector : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerEnter(other, base.gameObject, new ReliableOnTriggerExit._OnTriggerExit(this.OnTriggerExit));
		ZoneCollider component = other.gameObject.GetComponent<ZoneCollider>();
		if (component && !this.currentZoneColliders.Contains(component))
		{
			this.currentZoneColliders.Add(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ReliableOnTriggerExit.NotifyTriggerExit(other, base.gameObject);
		ZoneCollider component = other.gameObject.GetComponent<ZoneCollider>();
		if (component && this.currentZoneColliders.Contains(component))
		{
			this.currentZoneColliders.Remove(component);
		}
	}

	public ZoneEnum GetCurrentZone()
	{
		if (this.currentZoneColliders.Count == 0)
		{
			return ZoneEnum.OPEN_OCEAN;
		}
		return this.currentZoneColliders[0].Zone;
	}

	public static ZoneEnum GetZoneForPoint(Vector3 point)
	{
		point.y = 9999f;
		foreach (RaycastHit raycastHit in Physics.RaycastAll(point, Vector3.down, 99999f, 1 << LayerMask.NameToLayer("ZoneCollider")))
		{
			ZoneCollider component = raycastHit.collider.gameObject.GetComponent<ZoneCollider>();
			if (component)
			{
				return component.Zone;
			}
		}
		return ZoneEnum.OPEN_OCEAN;
	}

	public List<ZoneCollider> currentZoneColliders = new List<ZoneCollider>();
}
