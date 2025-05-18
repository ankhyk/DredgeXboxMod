using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetReferenceOverride
{
	public AssetReference assetReference;

	public List<string> nodesVisited;

	public List<string> boolValues;

	public int tirWorldPhase;
}
