using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NonSpatialItemData", menuName = "Dredge/NonSpatialItemData", order = 0)]
public class NonSpatialItemData : ItemData
{
	[SerializeField]
	public bool showInCabin = true;
}
