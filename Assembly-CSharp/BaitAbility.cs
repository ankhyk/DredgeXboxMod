using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaitAbility : Ability
{
	public override void Init()
	{
		base.Init();
	}

	public override bool Activate()
	{
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.Inventory.spatialItems.Find((SpatialItemInstance i) => i.id == this.currentlySelectedItem.id);
		if (spatialItemInstance != null)
		{
			this.isActive = true;
			this.DeployBait(spatialItemInstance);
			GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, true);
		}
		else
		{
			Debug.LogWarning("[BaitAbility] Activate() found no bait to deploy. Shouldn't have been able to activate this ability.");
		}
		base.Activate();
		this.Deactivate();
		return true;
	}

	public override void Deactivate()
	{
		base.Deactivate();
	}

	protected override void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		if (this.abilityData.allowItemCycling && (spatialItemData == null || this.baitItems.Contains(spatialItemData)))
		{
			this.RefreshItemCyclingCollection();
		}
	}

	protected override void RefreshItemCyclingCollection()
	{
		List<SpatialItemInstance> list = GameManager.Instance.SaveData.Inventory.spatialItems.Where((SpatialItemInstance instance) => this.baitItems.Contains(instance.GetItemData<SpatialItemData>())).ToList<SpatialItemInstance>();
		this.uniqueItemDatasUsedByAbility = (from i in list.Select((SpatialItemInstance i) => i.GetItemData<SpatialItemData>()).Distinct<SpatialItemData>()
			orderby i.id
			select i).ToList<SpatialItemData>();
		base.CycleItem(0);
	}

	public static List<FishItemData> GetFishForBait(SpatialItemData spatialItemData)
	{
		if (spatialItemData == null)
		{
			return new List<FishItemData>();
		}
		ZoneEnum currentPlayerZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
		List<FishItemData> list = GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH).OfType<FishItemData>().ToList<FishItemData>();
		int tirPhase = GameManager.Instance.SaveData.TIRWorldPhase;
		List<FishItemData> list2;
		if (spatialItemData.id == "bait")
		{
			list2 = list.Where((FishItemData i) => !i.IsAberration && i.CanAppearInBaitBalls && i.canBeCaughtByRod && i.zonesFoundIn.HasFlag(currentPlayerZone) && i.TIRPhase <= tirPhase && GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(i.harvestableType, i.requiresAdvancedEquipment)).Shuffle<FishItemData>().ToList<FishItemData>();
		}
		else if (spatialItemData.id == "bait-exotic")
		{
			list2 = list.Where((FishItemData i) => i.LocationHiddenUntilCaught && i.canBeCaughtByRod && !i.IsAberration && i.zonesFoundIn.HasFlag(currentPlayerZone) && i.TIRPhase <= tirPhase).ToList<FishItemData>();
		}
		else if (spatialItemData.id == "bait-ab" && GameManager.Instance.SaveData.CanCatchAberrations)
		{
			int currentWorldPhase = GameManager.Instance.SaveData.WorldPhase;
			list2 = list.Where((FishItemData i) => i.IsAberration && !i.LocationHiddenUntilCaught && i.NonAberrationParent != null && i.NonAberrationParent.CanAppearInBaitBalls && i.canBeCaughtByRod && i.zonesFoundIn.HasFlag(currentPlayerZone) && i.TIRPhase <= tirPhase && GameManager.Instance.PlayerStats.GetHasEquipmentForHarvestType(i.harvestableType, i.requiresAdvancedEquipment) && GameManager.Instance.SaveData.GetCaughtCountById(i.NonAberrationParent.id) > 0 && currentWorldPhase >= i.MinWorldPhaseRequired).Shuffle<FishItemData>().ToList<FishItemData>();
		}
		else
		{
			if (!(spatialItemData.id == "bait-crab"))
			{
				return new List<FishItemData>();
			}
			list2 = list.Where((FishItemData i) => i.canBeCaughtByPot && !i.IsAberration && i.CanAppearInBaitBalls && i.TIRPhase <= tirPhase && i.zonesFoundIn.HasFlag(currentPlayerZone)).Shuffle<FishItemData>().ToList<FishItemData>();
		}
		return list2.GetRange(0, Mathf.Min(list2.Count, GameManager.Instance.GameConfigData.NumFishSpeciesInBaitBall));
	}

	public void DeployBait(SpatialItemInstance baitInstance)
	{
		BaitPOIDataModel baitPOIDataModel = new BaitPOIDataModel();
		SpatialItemData itemData = baitInstance.GetItemData<SpatialItemData>();
		baitPOIDataModel.doesRestock = false;
		List<FishItemData> list = BaitAbility.GetFishForBait(baitInstance.GetItemData<SpatialItemData>());
		if (list.Count == 0)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.bait-failed");
			return;
		}
		int num = global::UnityEngine.Random.Range(GameManager.Instance.GameConfigData.NumFishInBaitBallMin, GameManager.Instance.GameConfigData.NumFishInBaitBallMax);
		if (itemData.id == "bait-exotic")
		{
			num = 1;
		}
		if (itemData.id == "bait-ab" && GameManager.Instance.QuestManager.IsQuestCompleted(this.deepFormItemData.QuestCompleteRequired.name) && this.deepFormItemData.zonesFoundIn.HasFlag(GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone()) && global::UnityEngine.Random.value <= this.deepFormItemData.BaitChanceOverride)
		{
			num = 1;
			list = new List<FishItemData> { this.deepFormItemData };
		}
		int num2 = 0;
		Stack<HarvestableItemData> stack = new Stack<HarvestableItemData>();
		for (int i = 0; i < num; i++)
		{
			stack.Push(list[num2 % list.Count]);
			num2++;
		}
		baitPOIDataModel.itemStock = stack;
		baitPOIDataModel.startStock = (float)stack.Count;
		baitPOIDataModel.maxStock = baitPOIDataModel.startStock;
		baitPOIDataModel.usesTimeSpecificStock = false;
		Vector3 vector = new Vector3(GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.x, 0f, GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.z);
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.baitPOIPrefab, vector, Quaternion.identity, GameSceneInitializer.Instance.HarvestPoiContainer.transform);
		gameObject.transform.eulerAngles = new Vector3(0f, GameManager.Instance.Player.BoatModelProxy.DeployPosition.eulerAngles.y, 0f);
		HarvestPOI component = gameObject.GetComponent<HarvestPOI>();
		if (component)
		{
			component.Harvestable = baitPOIDataModel;
			component.HarvestPOIData = baitPOIDataModel;
			Cullable component2 = component.GetComponent<Cullable>();
			if (component2)
			{
				GameManager.Instance.CullingBrain.AddCullable(component2);
			}
		}
		if (list.Count >= 3 && stack.Count >= 3)
		{
			GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.ABILITY_BAIT, true);
		}
		if (baitInstance.GetItemData<SpatialItemData>().id == "bait-crab")
		{
			GameManager.Instance.Player.Harvester.CurrentHarvestPOI = component;
			GameManager.Instance.Player.Harvester.IsCrabBaitMode = true;
			GameManager.Instance.Player.Harvester.enabled = true;
			GameEvents.Instance.OnHarvestModeToggled += this.OnHarvestModeToggled;
		}
		GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(baitInstance, true);
		GameEvents.Instance.TriggerItemInventoryChanged(this.currentlySelectedItem);
	}

	private void OnHarvestModeToggled(bool enabled)
	{
		if (!enabled)
		{
			GameEvents.Instance.OnHarvestModeToggled -= this.OnHarvestModeToggled;
		}
	}

	[SerializeField]
	private GameObject baitPOIPrefab;

	[SerializeField]
	private List<SpatialItemData> baitItems = new List<SpatialItemData>();

	[SerializeField]
	private FishItemData deepFormItemData;

	private List<HarvestPOI> deployedPOIs = new List<HarvestPOI>();

	private int maxDeployedPOIs = 10;
}
