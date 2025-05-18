using System;
using UnityEngine;

public class MapMarkerLocation : MonoBehaviour
{
	private void SetMapMarkerData()
	{
		this.mapMarkerData.x = base.transform.position.x;
		this.mapMarkerData.z = base.transform.position.z;
	}

	[SerializeField]
	private MapMarkerData mapMarkerData;
}
