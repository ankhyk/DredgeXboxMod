using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapStampConfig", menuName = "Dredge/MapStampConfig", order = 0)]
public class MapStampConfig : ScriptableObject
{
	[SerializeField]
	public List<Sprite> stampSprites;

	[SerializeField]
	public List<DredgeColorTypeEnum> stampColors;
}
