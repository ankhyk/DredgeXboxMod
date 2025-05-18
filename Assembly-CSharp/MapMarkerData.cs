using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMarkerData", menuName = "Dredge/MapMarkerData", order = 0)]
public class MapMarkerData : ScriptableObject
{
	public float x;

	public float z;

	public MapMarkerType mapMarkerType;
}
