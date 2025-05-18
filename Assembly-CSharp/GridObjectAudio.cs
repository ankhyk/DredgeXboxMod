using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GridObjectAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemQuickMoved += this.OnItemQuickMoved;
		GameEvents.Instance.OnItemDestroyed += this.OnItemDestroyed;
		GameEvents.Instance.OnItemRotated += this.OnItemRotated;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemQuickMoved -= this.OnItemQuickMoved;
		GameEvents.Instance.OnItemDestroyed -= this.OnItemDestroyed;
		GameEvents.Instance.OnItemRotated -= this.OnItemRotated;
	}

	private void OnItemPickedUp(GridObject obj)
	{
		GridObjectAudioOverride gridObjectAudioOverride = this.objectAudioOverrides.Find((GridObjectAudioOverride o) => o.itemData.id == obj.ItemData.id);
		AssetReference assetReference;
		if (gridObjectAudioOverride != null && gridObjectAudioOverride.hasPickupOverride)
		{
			assetReference = gridObjectAudioOverride.pickUpAssetReferences.PickRandom<AssetReference>();
		}
		else if (this.IsAberration(obj.ItemData))
		{
			assetReference = this.aberrationPickupSFX;
		}
		else if (this.IsOrganic(obj.ItemData))
		{
			assetReference = this.organicPickupSFX;
		}
		else if (this.IsEquipment(obj))
		{
			assetReference = this.equipmentUninstallSFX;
		}
		else if (this.IsTrinket(obj.ItemData))
		{
			assetReference = this.trinketPickupSFX;
		}
		else
		{
			assetReference = this.inorganicPickupSFX;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, 1f);
	}

	private void OnItemPlaceComplete(GridObject obj, bool success)
	{
		AssetReference assetReference;
		if (success)
		{
			GridObjectAudioOverride gridObjectAudioOverride = this.objectAudioOverrides.Find((GridObjectAudioOverride o) => o.itemData.id == obj.ItemData.id);
			if (gridObjectAudioOverride != null && gridObjectAudioOverride.hasPlaceOverride)
			{
				assetReference = gridObjectAudioOverride.placeAssetReferences.PickRandom<AssetReference>();
			}
			else if (this.IsOrganic(obj.ItemData))
			{
				assetReference = this.organicPlaceSFX;
			}
			else if (this.IsEquipment(obj))
			{
				assetReference = this.equipmentInstallSFX;
			}
			else if (this.IsTrinket(obj.ItemData))
			{
				assetReference = this.trinketPlaceSFX;
			}
			else
			{
				assetReference = this.inorganicPlaceSFX;
			}
		}
		else
		{
			assetReference = this.placeErrorSFX;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, 1f);
	}

	private void OnItemQuickMoved(GridObject gridObject)
	{
		this.OnItemPlaceComplete(gridObject, true);
	}

	private void OnItemDestroyed(SpatialItemData itemData, bool playerDestroyed)
	{
		if (!playerDestroyed)
		{
			return;
		}
		GridObjectAudioOverride gridObjectAudioOverride = this.objectAudioOverrides.Find((GridObjectAudioOverride o) => o.itemData.id == itemData.id);
		AssetReference assetReference;
		if (gridObjectAudioOverride != null && gridObjectAudioOverride.hasDestroyOverride)
		{
			assetReference = gridObjectAudioOverride.destroyAssetReferences.PickRandom<AssetReference>();
		}
		else if (this.IsOrganic(itemData))
		{
			assetReference = this.organicDropSFX;
		}
		else if (this.IsEquipment(itemData))
		{
			assetReference = this.equipmentUninstallSFX;
		}
		else if (this.IsTrinket(itemData))
		{
			assetReference = this.trinketDropSFX;
		}
		else
		{
			assetReference = this.inorganicDropSFX;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, 1f);
	}

	private bool IsOrganic(ItemData itemData)
	{
		return itemData.itemSubtype == ItemSubtype.FISH;
	}

	private bool IsAberration(ItemData itemData)
	{
		return itemData is FishItemData && (itemData as FishItemData).IsAberration;
	}

	private bool IsTrinket(ItemData itemData)
	{
		return itemData.itemSubtype == ItemSubtype.TRINKET;
	}

	private bool IsEquipment(GridObject obj)
	{
		return obj.ItemData.itemType == ItemType.EQUIPMENT && obj.ItemData.moveMode == MoveMode.INSTALL && obj.state == GridObjectState.IN_INVENTORY;
	}

	private bool IsEquipment(SpatialItemData itemData)
	{
		return itemData.itemType == ItemType.EQUIPMENT && itemData.moveMode == MoveMode.INSTALL;
	}

	private void OnItemRotated(GridObject obj)
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.rotateSFX, AudioLayer.SFX_UI, 1f, 1f);
	}

	[SerializeField]
	private AssetReference rotateSFX;

	[SerializeField]
	private AssetReference organicPickupSFX;

	[SerializeField]
	private AssetReference organicPlaceSFX;

	[SerializeField]
	private AssetReference organicDropSFX;

	[SerializeField]
	private AssetReference inorganicPickupSFX;

	[SerializeField]
	private AssetReference inorganicPlaceSFX;

	[SerializeField]
	private AssetReference inorganicDropSFX;

	[SerializeField]
	private AssetReference trinketPickupSFX;

	[SerializeField]
	private AssetReference trinketPlaceSFX;

	[SerializeField]
	private AssetReference trinketDropSFX;

	[SerializeField]
	private AssetReference equipmentInstallSFX;

	[SerializeField]
	private AssetReference equipmentUninstallSFX;

	[SerializeField]
	private AssetReference aberrationPickupSFX;

	[SerializeField]
	private AssetReference placeErrorSFX;

	[SerializeField]
	private List<GridObjectAudioOverride> objectAudioOverrides;
}
