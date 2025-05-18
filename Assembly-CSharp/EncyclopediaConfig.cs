using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "EncyclopediaConfig", menuName = "Dredge/EncyclopediaConfig", order = 0)]
public class EncyclopediaConfig : SerializedScriptableObject
{
	[SerializeField]
	public Dictionary<ZoneEnum, LocalizedString> zoneStrings = new Dictionary<ZoneEnum, LocalizedString>();

	[SerializeField]
	public Dictionary<ZoneEnum, Sprite> zoneIconSprites = new Dictionary<ZoneEnum, Sprite>();
}
