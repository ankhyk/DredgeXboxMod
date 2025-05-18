using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerDataLookup", menuName = "Dredge/SpeakerDataLookup", order = 0)]
public class SpeakerDataLookup : SerializedScriptableObject
{
	[SerializeField]
	public Dictionary<string, SpeakerData> lookupTable = new Dictionary<string, SpeakerData>();
}
