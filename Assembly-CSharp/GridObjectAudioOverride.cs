using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[Serializable]
public class GridObjectAudioOverride
{
	public ItemData itemData;

	public bool hasPickupOverride;

	public List<AssetReference> pickUpAssetReferences;

	public bool hasPlaceOverride;

	public List<AssetReference> placeAssetReferences;

	public bool hasDestroyOverride;

	public List<AssetReference> destroyAssetReferences;
}
