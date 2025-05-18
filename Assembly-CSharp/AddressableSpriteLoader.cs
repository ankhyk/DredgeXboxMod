using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableSpriteLoader : MonoBehaviour
{
	private void OnEnable()
	{
		this.LoadAddressable();
	}

	private void LoadAddressable()
	{
		Addressables.LoadAssetAsync<Sprite>(this.assetReference).Completed += delegate(AsyncOperationHandle<Sprite> response)
		{
			if (response.Status == AsyncOperationStatus.Succeeded)
			{
				this.image.sprite = response.Result;
				this.image.enabled = true;
			}
		};
	}

	private void OnDisable()
	{
		this.ReleaseAddressable();
	}

	private void ReleaseAddressable()
	{
		try
		{
			Addressables.Release<Sprite>(this.image.sprite);
		}
		catch (Exception)
		{
		}
	}

	[SerializeField]
	private AssetReference assetReference;

	[SerializeField]
	private Image image;
}
