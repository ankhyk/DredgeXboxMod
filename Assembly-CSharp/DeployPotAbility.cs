using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DeployPotAbility : Ability
{
	public override void Init()
	{
		base.Init();
	}

	public override bool Activate()
	{
		bool flag = false;
		List<SpatialItemInstance> list = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.POT);
		list = list.Where((SpatialItemInstance instance) => instance.id == base.CurrentlySelectedItem.id).ToList<SpatialItemInstance>();
		if (list.Count > 0)
		{
			float maxDurability = 0f;
			SpatialItemInstance bestPot = null;
			list.ForEach(delegate(SpatialItemInstance potInstance)
			{
				float durability = potInstance.durability;
				if (durability > maxDurability && durability > 0f)
				{
					maxDurability = durability;
					bestPot = potInstance;
				}
			});
			if (bestPot != null)
			{
				if (DeployPotAbility.DeployCrabPot(bestPot))
				{
					GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(bestPot, true);
					GameEvents.Instance.TriggerItemInventoryChanged(bestPot.GetItemData<SpatialItemData>());
					this.isActive = true;
					flag = true;
				}
			}
			else
			{
				GameManager.Instance.UI.ShowNotificationWithItemName(NotificationType.ERROR, "notification.deploy-pot.none-with-durability", base.CurrentlySelectedItem.itemNameKey, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
			}
			GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, true);
		}
		else
		{
			Debug.LogWarning("[DeployPotAbility] Activate() found no pots to deploy. Shouldn't have been able to activate this ability.");
		}
		base.Activate();
		this.Deactivate();
		return flag;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	protected override void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		if (this.abilityData.allowItemCycling && (spatialItemData == null || spatialItemData.itemSubtype == ItemSubtype.POT))
		{
			this.RefreshItemCyclingCollection();
		}
	}

	protected override void RefreshItemCyclingCollection()
	{
		List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.POT);
		this.uniqueItemDatasUsedByAbility = (from i in allItemsOfType.Select((SpatialItemInstance i) => i.GetItemData<SpatialItemData>()).Distinct<SpatialItemData>()
			orderby i.id
			select i).ToList<SpatialItemData>();
		base.CycleItem(0);
	}

	public static bool DeployCrabPot(SpatialItemInstance potInstance)
	{
		SerializedCrabPotPOIData serializedCrabPotPOIData = new SerializedCrabPotPOIData(potInstance, GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.x, GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.z, GameManager.Instance.Player.BoatModelProxy.DeployPosition.eulerAngles.y);
		serializedCrabPotPOIData.lastUpdate = GameManager.Instance.Time.TimeAndDay;
		GameManager.Instance.SaveData.serializedCrabPotPOIs.Add(serializedCrabPotPOIData);
		GameSceneInitializer.Instance.CreatePlacedHarvestPOI(serializedCrabPotPOIData);
		DeployableItemData itemData = potInstance.GetItemData<DeployableItemData>();
		float currentDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPlayerPosition();
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(itemData.itemNameKey.TableReference, itemData.itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			string formattedDepthString = GameManager.Instance.ItemManager.GetFormattedDepthString(currentDepth);
			GameManager.Instance.UI.ShowNotification(NotificationType.CRAB_POT_DEPLOYED, "notification.crab-pot-deployed", new object[]
			{
				string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
					">",
					op.Result,
					"</color>"
				}),
				formattedDepthString
			});
		};
		return true;
	}

	private static bool CheckIsPotItem(HarvestableItemData itemData)
	{
		return itemData.canBeCaughtByPot;
	}
}
