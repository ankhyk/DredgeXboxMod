using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestTypeTagConfig", menuName = "Dredge/HarvestTypeTagConfig", order = 0)]
public class HarvestTypeTagConfig : SerializedScriptableObject
{
	[SerializeField]
	public Dictionary<HarvestableType, string> stringLookup = new Dictionary<HarvestableType, string>();

	[SerializeField]
	public Dictionary<HarvestableType, Color> colorLookup = new Dictionary<HarvestableType, Color>();

	[SerializeField]
	public Dictionary<HarvestableType, Color> textColorLookup = new Dictionary<HarvestableType, Color>();
}
