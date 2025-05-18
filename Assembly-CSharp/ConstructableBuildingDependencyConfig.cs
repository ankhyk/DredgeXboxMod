using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructableBuildingDependencyConfig", menuName = "Dredge/ConstructableBuildingDependencyConfig", order = 0)]
public class ConstructableBuildingDependencyConfig : SerializedScriptableObject
{
	[SerializeField]
	public List<ConstructableBuildingDependencyData> data = new List<ConstructableBuildingDependencyData>();
}
