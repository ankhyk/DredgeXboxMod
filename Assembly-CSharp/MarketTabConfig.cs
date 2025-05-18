using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class MarketTabConfig
{
	public GridKey gridKey;

	public Sprite tabSprite;

	public LocalizedString titleKey;

	public bool isUnlockedBasedOnDialogue;

	public List<string> unlockDialogueNodes;
}
