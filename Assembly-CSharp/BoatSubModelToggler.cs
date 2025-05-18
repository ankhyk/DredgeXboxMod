using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoatSubModelToggler : SerializedMonoBehaviour
{
	private void OnEnable()
	{
		this.hasAdvancedLights = GameManager.Instance.SaveData.unlockedAbilities.Contains("lights-advanced");
		GameEvents.Instance.OnPlayerStatsChanged += this.OnPlayerStatsChanged;
		GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		inventory.OnContentsUpdated = (Action)Delegate.Combine(inventory.OnContentsUpdated, new Action(this.RefreshFishContainers));
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
		GameEvents.Instance.OnIcebreakerEquipChanged += this.OnIcebreakerEquipChanged;
		this.RefreshFishContainers();
		this.OnPlayerStatsChanged();
		this.OnPlayerDamageChanged();
		this.RefreshIcebreaker();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerStatsChanged -= this.OnPlayerStatsChanged;
		GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		inventory.OnContentsUpdated = (Action)Delegate.Remove(inventory.OnContentsUpdated, new Action(this.RefreshFishContainers));
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
		GameEvents.Instance.OnIcebreakerEquipChanged -= this.OnIcebreakerEquipChanged;
	}

	private void OnPlayerDamageChanged()
	{
		int numberOfDamagedSlots = GameManager.Instance.SaveData.GetNumberOfDamagedSlots();
		bool flag = false;
		int num;
		if (numberOfDamagedSlots == 0)
		{
			num = 0;
		}
		else if (numberOfDamagedSlots == GameManager.Instance.PlayerStats.DamageThreshold)
		{
			flag = true;
			num = this.boatModelProxy.DamageStateMeshes.Count - 1;
		}
		else
		{
			num = Mathf.CeilToInt((float)numberOfDamagedSlots / 2f);
		}
		num = Mathf.Clamp(num, 0, this.boatModelProxy.DamageStateMeshes.Count - 1);
		Mesh mesh = this.boatModelProxy.DamageStateMeshes[num];
		this.meshFilter.mesh = mesh;
		this.hullCriticalEffects.SetActive(flag);
	}

	private void RefreshFishContainers()
	{
		float num = GameManager.Instance.SaveData.Inventory.GetFillProportional(ItemSubtype.FISH) + GameManager.Instance.SaveData.Inventory.GetFillProportional(ItemSubtype.TRINKET);
		int num2 = Mathf.CeilToInt((float)this.fishContainers.Length * num);
		for (int i = 0; i < this.fishContainers.Length; i++)
		{
			this.fishContainers[i].SetActive(i < num2);
		}
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		if (abilityData.name == "lights-advanced")
		{
			this.hasAdvancedLights = true;
			this.OnPlayerStatsChanged();
		}
	}

	private void OnPlayerStatsChanged()
	{
		for (int l = 0; l < this.subModelConfigs.Length; l++)
		{
			BoatSubModelConfig thisConfig = this.subModelConfigs[l];
			if (thisConfig.itemSubtype == ItemSubtype.NET)
			{
				this.OnNetEquipChanged();
			}
			else
			{
				int count = (from i in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, thisConfig.itemSubtype)
					where thisConfig.showDamagedItems || !i.GetIsOnDamagedCell()
					select i).ToList<SpatialItemInstance>().Count;
				for (int j = 0; j < thisConfig.gameObjects.Length; j++)
				{
					thisConfig.gameObjects[j].SetActive(j < count);
				}
				if (thisConfig.itemSubtype == ItemSubtype.LIGHT && this.hasAdvancedLights)
				{
					for (int k = 0; k < this.advancedLightModels.Length; k++)
					{
						this.advancedLightModels[k].SetActive(k < count);
					}
				}
			}
		}
	}

	private void OnNetEquipChanged()
	{
		SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
		List<Net> list = base.GetComponentsInChildren<Net>(true).ToList<Net>();
		if (spatialItemInstance != null)
		{
			NetType netType = NetType.REGULAR;
			if (spatialItemInstance.id == "tir-net1")
			{
				netType = NetType.SIPHON;
			}
			else if (spatialItemInstance.id == "tir-net2")
			{
				netType = NetType.MATERIAL;
			}
			list.ForEach(delegate(Net n)
			{
				n.gameObject.SetActive(n.NetType == netType);
			});
			return;
		}
		list.ForEach(delegate(Net n)
		{
			n.gameObject.SetActive(false);
		});
	}

	private void OnIcebreakerEquipChanged()
	{
		this.RefreshIcebreaker();
	}

	private void RefreshIcebreaker()
	{
		this.icebreaker.SetActive(GameManager.Instance.SaveData.GetIsIcebreakerEquipped());
	}

	[SerializeField]
	private BoatSubModelConfig[] subModelConfigs;

	[SerializeField]
	private GameObject[] advancedLightModels;

	[SerializeField]
	private GameObject[] fishContainers;

	[SerializeField]
	private BoatModelProxy boatModelProxy;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private GameObject hullCriticalEffects;

	[SerializeField]
	private GameObject icebreaker;

	private bool hasAdvancedLights;

	public static string ICEBREAKER_EQUIP_STRING_KEY = "is-icebreaker-equipped";
}
