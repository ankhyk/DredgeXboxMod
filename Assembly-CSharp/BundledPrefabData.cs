using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BundledPrefabData", menuName = "Dredge/BundledPrefabData", order = 0)]
public class BundledPrefabData : ScriptableObject
{
	public List<PrefabData> prefabData;
}
