using System;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "MessageItemData", menuName = "Dredge/MessageItemData", order = 0)]
public class MessageItemData : NonSpatialItemData
{
	public LocalizedString messageBodyKey;

	public int chronologicalOrder;

	public int set;
}
