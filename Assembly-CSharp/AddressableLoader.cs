using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableLoader : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.enable)
		{
			this.LoadAddressable();
		}
	}

	private void Awake()
	{
		if (this.awake)
		{
			this.LoadAddressable();
		}
	}

	private void LoadAddressable()
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(this.assetReference);
		handle.Completed += delegate(AsyncOperationHandle<GameObject> result)
		{
			if (result.Status == AsyncOperationStatus.Succeeded)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(handle.Result, this.transform);
			}
		};
	}

	[SerializeField]
	private AssetReference assetReference;

	[SerializeField]
	private bool awake;

	[SerializeField]
	private bool enable;
}
