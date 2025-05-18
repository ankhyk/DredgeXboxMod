using System;
using UnityEngine;

public class DockPOI : POI
{
	[SerializeField]
	public Transform[] dockSlots;

	[SerializeField]
	public Dock dock;
}
