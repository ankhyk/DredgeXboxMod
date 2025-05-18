using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestHelper", menuName = "Dredge/HarvestHelper", order = 0)]
public class HarvestHelper : ScriptableObject
{
	[SerializeField]
	public int count;
}
