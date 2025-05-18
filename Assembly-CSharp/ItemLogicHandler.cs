using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogicHandler : MonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.ItemLogicHandler = this;
	}

	private void Start()
	{
		GameEvents.Instance.OnItemSeen += this.OnItemSeen;
		GameEvents.Instance.OnItemAdded += this.OnItemAdded;
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
		GameEvents.Instance.OnSpecialItemHandlerRequested += this.OnSpecialItemHandlerRequested;
	}

	private void OnDestroy()
	{
		GameEvents.Instance.OnItemSeen -= this.OnItemSeen;
		GameEvents.Instance.OnItemAdded -= this.OnItemAdded;
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
		GameEvents.Instance.OnSpecialItemHandlerRequested -= this.OnSpecialItemHandlerRequested;
	}

	private void OnItemSeen(SpatialItemInstance itemInstance)
	{
		if (itemInstance == null)
		{
			return;
		}
		SpatialItemData itemData = itemInstance.GetItemData<SpatialItemData>();
		if (this.baitItems.Contains(itemData))
		{
			GameManager.Instance.PlayerAbilities.UnlockAbility(this.baitAbilityData.name);
		}
		if (itemData.itemSubtype == ItemSubtype.RELIC)
		{
			AutoSplitterData.relicsAcquired++;
		}
	}

	private void OnItemAdded(SpatialItemInstance itemInstance, bool belongsToPlayer)
	{
		if (belongsToPlayer)
		{
			if (itemInstance.GetItemData<ItemData>().itemSubtype == ItemSubtype.NET)
			{
				GameManager.Instance.PlayerAbilities.UnlockAbility(this.trawlAbilityData.name);
			}
			if (itemInstance.GetItemData<ItemData>().itemSubtype == ItemSubtype.POT)
			{
				GameManager.Instance.PlayerAbilities.UnlockAbility(this.potAbilityData.name);
			}
		}
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		if (upgradeData == this.netUnlockUpgradeData)
		{
			GameManager.Instance.SaveData.AdjustResearchProgress(this.netItemData, this.netItemData.ResearchPointsRequired);
		}
	}

	private void OnSpecialItemHandlerRequested(SpatialItemData itemData)
	{
		if (itemData.id == this.teleportAnchorItemData.id)
		{
			this.CreateTeleportAnchor(GameManager.Instance.Player.transform.position, true);
			return;
		}
		if (itemData.id == this.repairBoatItemData.id)
		{
			GameManager.Instance.ItemManager.UseRepairKit();
			GameManager.Instance.VibrationManager.Vibrate(this.repairBoatVibrationData, VibrationRegion.WholeBody, true);
			return;
		}
		if (itemData.id == this.repairDurabilityItemData.id)
		{
			GameManager.Instance.ItemManager.RepairAllItemDurability();
			GameManager.Instance.UI.OccasionalGridPanel.TryRepairCurrentCrabPot();
			GameManager.Instance.UI.ShowNotification(NotificationType.ANY_REPAIR_KIT_USED, "notification.durability-repaired");
			GameManager.Instance.VibrationManager.Vibrate(this.repairDurabilityVibrationData, VibrationRegion.WholeBody, true);
			return;
		}
		if (itemData.id == this.repairSanityItemData.id)
		{
			GameManager.Instance.Player.Sanity.ChangeSanity(1f);
			GameManager.Instance.UI.ShowNotification(NotificationType.ANY_REPAIR_KIT_USED, "notification.panic-repaired");
			GameManager.Instance.VibrationManager.Vibrate(this.repairSanityVibrationData, VibrationRegion.WholeBody, true);
		}
	}

	public void CreateTeleportAnchor(Vector3 position, bool showNotification)
	{
		global::UnityEngine.Object.Instantiate<GameObject>(this.teleportAnchorPOIPrefab, position, Quaternion.identity).GetComponent<TeleportAnchorPOI>().CanBeRetrieved = true;
		GameEvents.Instance.TriggerTeleportAnchorAdded();
		if (showNotification)
		{
			GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.TELEPORT_ANCHOR_PLACED, "notification.teleport-anchor-placed", this.teleportAnchorItemData.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.CRITICAL));
		}
		GameManager.Instance.SaveData.SetHasTeleportAnchor(true);
		GameManager.Instance.SaveData.SetTeleportAnchorPosition(position.x, position.z);
	}

	[SerializeField]
	private UpgradeData netUnlockUpgradeData;

	[SerializeField]
	private SpatialItemData netItemData;

	[SerializeField]
	private List<SpatialItemData> baitItems;

	[SerializeField]
	private SpatialItemData teleportAnchorItemData;

	[SerializeField]
	private SpatialItemData repairBoatItemData;

	[SerializeField]
	private SpatialItemData repairDurabilityItemData;

	[SerializeField]
	private SpatialItemData repairSanityItemData;

	[SerializeField]
	private VibrationData repairBoatVibrationData;

	[SerializeField]
	private VibrationData repairDurabilityVibrationData;

	[SerializeField]
	private VibrationData repairSanityVibrationData;

	[SerializeField]
	private AbilityData trawlAbilityData;

	[SerializeField]
	private AbilityData potAbilityData;

	[SerializeField]
	private AbilityData baitAbilityData;

	[SerializeField]
	private GameObject teleportAnchorPOIPrefab;
}
