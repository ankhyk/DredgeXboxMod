using System;
using UnityEngine;

public class ResearchHelper : MonoBehaviour
{
	public int TotalPartCount
	{
		get
		{
			return this.totalPartCount;
		}
		set
		{
			this.totalPartCount = value;
		}
	}

	public HarvestableItemData ResearchItemData
	{
		get
		{
			return this.researchItemData;
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.ResearchHelper = this;
		this.UpdatePartCount();
		ApplicationEvents.Instance.OnGameLoaded += this.UpdatePartCount;
		GameEvents.Instance.OnItemInventoryChanged += this.OnInventoryItemChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnItemInventoryChanged -= this.OnInventoryItemChanged;
		ApplicationEvents.Instance.OnGameLoaded -= this.UpdatePartCount;
	}

	public bool SpendResearchItem()
	{
		bool flag = true;
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.GetItemInstanceById<SpatialItemInstance>(new string[] { this.researchItemData.id }, false, GameManager.Instance.SaveData.Inventory);
		if (spatialItemInstance == null)
		{
			spatialItemInstance = GameManager.Instance.SaveData.GetItemInstanceById<SpatialItemInstance>(new string[] { this.researchItemData.id }, false, GameManager.Instance.SaveData.Storage);
			if (!(spatialItemInstance != null))
			{
				GameManager.Instance.AudioPlayer.PlaySFX(this.spendResearchFailureSFX, AudioLayer.SFX_UI, 1f, 1f);
				return false;
			}
			flag = false;
		}
		if (flag)
		{
			GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(spatialItemInstance, true);
		}
		else
		{
			GameManager.Instance.SaveData.Storage.RemoveObjectFromGridData(spatialItemInstance, true);
		}
		this.totalPartCount--;
		GameManager.Instance.AudioPlayer.PlaySFX(this.spendResearchSuccessSFX, AudioLayer.SFX_UI, 1f, 1f);
		GameEvents.Instance.TriggerItemInventoryChanged(this.researchItemData);
		return true;
	}

	private void OnInventoryItemChanged(SpatialItemData spatialItemData)
	{
		if (spatialItemData != null && spatialItemData.id == this.researchItemData.id)
		{
			this.UpdatePartCount();
		}
	}

	public void UpdatePartCount()
	{
		this.inventoryPartCount = GameManager.Instance.SaveData.GetNumItemInGridById(this.researchItemData.id, GameManager.Instance.SaveData.Inventory);
		this.storagePartCount = GameManager.Instance.SaveData.GetNumItemInGridById(this.researchItemData.id, GameManager.Instance.SaveData.Storage);
		this.totalPartCount = this.inventoryPartCount + this.storagePartCount;
	}

	[SerializeField]
	private HarvestableItemData researchItemData;

	private int inventoryPartCount;

	private int storagePartCount;

	private int totalPartCount;

	[SerializeField]
	private AudioClip spendResearchSuccessSFX;

	[SerializeField]
	private AudioClip spendResearchFailureSFX;
}
