using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SpeakerParalinguisticConfig", menuName = "Dredge/SpeakerParalinguisticConfig", order = 0)]
public class SpeakerParalinguisticConfig : SerializedScriptableObject
{
	public Dictionary<ParalinguisticType, List<AssetReference>> paralinguistics = new Dictionary<ParalinguisticType, List<AssetReference>>();
}
