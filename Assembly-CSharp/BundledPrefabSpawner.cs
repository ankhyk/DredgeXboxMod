using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BundledPrefabSpawner : MonoBehaviour
{
	private void Awake()
	{
		this.LoadBundledPrefabAddressable();
	}

	private void LoadBundledPrefabAddressable()
	{
		Addressables.LoadAssetAsync<BundledPrefabData>(this.bundledPrefabDataReference).Completed += delegate(AsyncOperationHandle<BundledPrefabData> response)
		{
			if (response.Status == AsyncOperationStatus.Succeeded)
			{
				this.OnDataLoaded(response.Result);
			}
		};
	}

	private void OnDataLoaded(BundledPrefabData bundledPrefabData)
	{
		bundledPrefabData.prefabData.ForEach(delegate(PrefabData prefabData)
		{
			this.LoadPrefabAddressable(prefabData);
		});
	}

	private void LoadPrefabAddressable(PrefabData prefabData)
	{
		Addressables.LoadAssetAsync<GameObject>(prefabData.prefabReference).Completed += delegate(AsyncOperationHandle<GameObject> response)
		{
			if (response.Status == AsyncOperationStatus.Succeeded)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(response.Result, prefabData.position, prefabData.rotation);
			}
		};
	}

	public AssetReference bundledPrefabDataReference;
}
