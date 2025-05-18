using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class PrefabData
{
	public AssetReference prefabReference;

	public Vector3 position;

	public Quaternion rotation;
}
