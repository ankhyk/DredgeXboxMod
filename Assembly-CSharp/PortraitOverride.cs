using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PortraitOverride
{
	public GameObject portraitPrefab;

	public Sprite smallPortraitSprite;

	public bool useManualState;

	public string stateName;

	public int stateValue;

	public List<string> nodesVisited;
}
