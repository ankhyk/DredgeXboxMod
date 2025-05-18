using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TrawlNetAbility : Ability
{
	public override void Init()
	{
		base.Init();
	}

	private void OnDestroy()
	{
	}

	protected override void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		if (spatialItemData && spatialItemData.itemSubtype == ItemSubtype.NET)
		{
			SpatialItemInstance spatialItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
			bool flag = spatialItemInstance != this.trawlNetItemInstance;
			this.trawlNetItemInstance = spatialItemInstance;
			if (!flag)
			{
				return;
			}
			if (this.trawlNetItemInstance == null)
			{
				this.trawlNetItemData = null;
				GameManager.Instance.SaveData.TrawlNet.Clear(true);
			}
			else
			{
				this.trawlNetItemData = this.trawlNetItemInstance.GetItemData<DeployableItemData>();
				this.RefreshTimeUntilNextCatchRoll();
				this.TryToRemoveExcessItems();
			}
			if (this.isActive && (this.trawlNetItemInstance == null || (this.trawlNetItemInstance != null && this.trawlNetItemInstance.durability <= 0f)))
			{
				this.Deactivate();
			}
			this.RefreshNetFullness();
			this.RefreshDeploymentState();
			this.RefreshNetMode(this.trawlNetItemData);
			Action<int> itemCountChanged = this.ItemCountChanged;
			if (itemCountChanged != null)
			{
				itemCountChanged(GameManager.Instance.SaveData.TrawlNet.spatialItems.Count);
			}
		}
		this.RefreshItemCyclingCollection();
	}

	protected override void RefreshItemCyclingCollection()
	{
		this.uniqueItemDatasUsedByAbility = (from i in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.NET)
			select i.GetItemData<SpatialItemData>()).Distinct<SpatialItemData>().ToList<SpatialItemData>();
		base.CycleItem(0);
	}

	private void TryToRemoveExcessItems()
	{
		SerializableGrid netGrid = GameManager.Instance.SaveData.TrawlNet;
		List<SpatialItemInstance> list = new List<SpatialItemInstance>();
		list.AddRange(netGrid.spatialItems);
		netGrid.Clear(false);
		Vector3Int pos;
		list.ForEach(delegate(SpatialItemInstance i)
		{
			SpatialItemData itemData = i.GetItemData<SpatialItemData>();
			if (netGrid.FindPositionForObject(itemData, out pos, 0, false))
			{
				netGrid.AddObjectToGridData(i, pos, true, null);
			}
		});
	}

	private void RefreshTimeUntilNextCatchRoll()
	{
		if (GameManager.Instance.SaveData.NetFishCaught <= 0)
		{
			this.timeUntilNextCatchRoll = 0.0208333333333333333333333333m;
			return;
		}
		this.timeUntilNextCatchRoll = (decimal)this.trawlNetItemData.TimeBetweenCatchRolls / GameManager.Instance.PlayerStats.TrawlCatchRateGadgetModifier;
	}

	private void OnEnable()
	{
		this.AddTerminalCommands();
		GameEvents.Instance.OnBoatModelChanged += this.RefreshDeploymentState;
		GameEvents.Instance.OnTeleportComplete += this.OnTeleportComplete;
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
		this.trawlNetItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
		if (this.trawlNetItemInstance != null)
		{
			this.trawlNetItemData = this.trawlNetItemInstance.GetItemData<DeployableItemData>();
			this.RefreshNetMode(this.trawlNetItemData);
		}
		this.RefreshTrawlNetListeners();
		this.RefreshNetFullness();
	}

	private void OnDisable()
	{
		this.RemoveTerminalCommands();
		GameEvents.Instance.OnBoatModelChanged -= this.RefreshDeploymentState;
		GameEvents.Instance.OnTeleportComplete -= this.OnTeleportComplete;
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
		if (this.trawlNet != null)
		{
			SerializableGrid serializableGrid = this.trawlNet;
			serializableGrid.OnItemAdded = (Action<SpatialItemInstance, Action<GridObject>>)Delegate.Remove(serializableGrid.OnItemAdded, new Action<SpatialItemInstance, Action<GridObject>>(this.OnItemAdded));
			SerializableGrid serializableGrid2 = this.trawlNet;
			serializableGrid2.OnItemRemoved = (Action<SpatialItemInstance>)Delegate.Remove(serializableGrid2.OnItemRemoved, new Action<SpatialItemInstance>(this.OnItemRemoved));
			SerializableGrid serializableGrid3 = this.trawlNet;
			serializableGrid3.OnContentsUpdated = (Action)Delegate.Remove(serializableGrid3.OnContentsUpdated, new Action(this.OnContentsUpdated));
		}
	}

	private void OnFinaleVoyageStarted()
	{
		this.isInFinale = true;
	}

	private void RefreshTrawlNetListeners()
	{
		if (this.trawlNet != null)
		{
			SerializableGrid serializableGrid = this.trawlNet;
			serializableGrid.OnItemAdded = (Action<SpatialItemInstance, Action<GridObject>>)Delegate.Remove(serializableGrid.OnItemAdded, new Action<SpatialItemInstance, Action<GridObject>>(this.OnItemAdded));
			SerializableGrid serializableGrid2 = this.trawlNet;
			serializableGrid2.OnItemRemoved = (Action<SpatialItemInstance>)Delegate.Remove(serializableGrid2.OnItemRemoved, new Action<SpatialItemInstance>(this.OnItemRemoved));
			SerializableGrid serializableGrid3 = this.trawlNet;
			serializableGrid3.OnContentsUpdated = (Action)Delegate.Remove(serializableGrid3.OnContentsUpdated, new Action(this.OnContentsUpdated));
		}
		this.trawlNet = GameManager.Instance.SaveData.TrawlNet;
		if (this.trawlNet != null)
		{
			SerializableGrid serializableGrid4 = this.trawlNet;
			serializableGrid4.OnItemAdded = (Action<SpatialItemInstance, Action<GridObject>>)Delegate.Combine(serializableGrid4.OnItemAdded, new Action<SpatialItemInstance, Action<GridObject>>(this.OnItemAdded));
			SerializableGrid serializableGrid5 = this.trawlNet;
			serializableGrid5.OnItemRemoved = (Action<SpatialItemInstance>)Delegate.Combine(serializableGrid5.OnItemRemoved, new Action<SpatialItemInstance>(this.OnItemRemoved));
			SerializableGrid serializableGrid6 = this.trawlNet;
			serializableGrid6.OnContentsUpdated = (Action)Delegate.Combine(serializableGrid6.OnContentsUpdated, new Action(this.OnContentsUpdated));
		}
	}

	private void OnContentsUpdated()
	{
		Action<int> itemCountChanged = this.ItemCountChanged;
		if (itemCountChanged == null)
		{
			return;
		}
		itemCountChanged(GameManager.Instance.SaveData.TrawlNet.spatialItems.Count);
	}

	private void OnItemAdded(SpatialItemInstance spatialItemInstance, Action<GridObject> PostCreateAction = null)
	{
		this.RefreshNetFullness();
	}

	private void OnItemRemoved(SpatialItemInstance spatialItemInstance)
	{
		this.RefreshNetFullness();
	}

	private void OnTeleportComplete()
	{
		this.RefreshDeploymentState();
		this.RefreshNetFullness();
	}

	private void RefreshDeploymentState()
	{
		Animator trawlNetAnimator = this.playerRef.BoatModelProxy.GetTrawlNetAnimator();
		if (trawlNetAnimator != null)
		{
			trawlNetAnimator.SetBool("isDeployed", this.isActive);
		}
		this.playerRef.BoatModelProxy.TrawlNetOozeCollider.SetActive(this.isActive && this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE);
	}

	private void RefreshNetFullness()
	{
		float num = 0f;
		Animator trawlNetAnimator = this.playerRef.BoatModelProxy.GetTrawlNetAnimator();
		if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL)
		{
			num = this.trawlNet.GetFillProportional(ItemSubtype.FISH);
		}
		else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
		{
			num = this.trawlNet.GetFillProportional(ItemSubtype.MATERIAL) + this.trawlNet.GetFillProportional(ItemSubtype.TRINKET);
		}
		if (trawlNetAnimator == null)
		{
			return;
		}
		trawlNetAnimator.SetFloat("fullness", num);
	}

	private void RefreshNetMode(SpatialItemData netData)
	{
		if (netData != null)
		{
			if (netData.id == this.oozeNetData.id)
			{
				this.trawlMode = TrawlNetAbility.TrawlMode.TRAWL_OOZE;
			}
			else if (this.trawlNetItemData.id == this.materialNetData.id)
			{
				this.trawlMode = TrawlNetAbility.TrawlMode.TRAWL_MATERIAL;
			}
			else
			{
				this.trawlMode = TrawlNetAbility.TrawlMode.TRAWL;
			}
		}
		else
		{
			this.trawlMode = TrawlNetAbility.TrawlMode.NONE;
		}
		this.RefreshTrawlNetListeners();
	}

	public override bool Activate()
	{
		bool flag = false;
		if (this.isActive)
		{
			this.Deactivate();
		}
		else
		{
			this.trawlNetItemInstance = GameManager.Instance.SaveData.EquippedTrawlNetInstance();
			if (this.trawlNetItemInstance.durability > 0f)
			{
				this.isActive = true;
				flag = true;
				this.trawlNetItemData = this.trawlNetItemInstance.GetItemData<DeployableItemData>();
				this.RefreshNetMode(this.trawlNetItemData);
				this.RefreshTimeUntilNextCatchRoll();
				this.RefreshDeploymentState();
				GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, true);
				if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
				{
					this.oozeGatheringActiveAudioSource.Play();
					this.oozeGatheringPassiveAudioSource.Play();
				}
				GameManager.Instance.AudioPlayer.PlaySFX(this.GetDeploySFX(), AudioLayer.SFX_PLAYER, 1f, 1f);
				base.Activate();
			}
		}
		return flag;
	}

	private AssetReference GetDeploySFX()
	{
		AssetReference assetReference = this.deploySFX;
		if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
		{
			assetReference = this.materialNetDeploySFX;
		}
		else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
		{
			assetReference = this.oozeDeploySFX;
		}
		return assetReference;
	}

	private AssetReference GetCatchSFX()
	{
		AssetReference assetReference = this.catchSFX;
		if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
		{
			assetReference = this.materialNetCatchSFX;
		}
		else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
		{
			assetReference = null;
		}
		return assetReference;
	}

	private AssetReference GetEndSFX()
	{
		AssetReference assetReference = this.retractSFX;
		if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
		{
			assetReference = this.materialNetRetractSFX;
		}
		else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
		{
			assetReference = this.oozeRetractSFX;
		}
		return assetReference;
	}

	private AssetReference GetBreakSFX()
	{
		AssetReference assetReference = this.breakSFX;
		if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
		{
			assetReference = this.materialNetBreakSFX;
		}
		else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
		{
			assetReference = this.oozeBreakSFX;
		}
		return assetReference;
	}

	public override void Deactivate()
	{
		if (this.isActive)
		{
			if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
			{
				GameManager.Instance.AudioPlayer.PlaySFX(this.oozeEndSFX, AudioLayer.SFX_PLAYER, this.endSFXVolume, 1f);
			}
			else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL || this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
			{
				if (this.trawlNetItemInstance != null && this.trawlNetItemInstance.durability > 0f)
				{
					GameManager.Instance.AudioPlayer.PlaySFX(this.GetEndSFX(), AudioLayer.SFX_PLAYER, this.endSFXVolume, 1f);
				}
				else
				{
					GameManager.Instance.AudioPlayer.PlaySFX(this.GetBreakSFX(), AudioLayer.SFX_PLAYER, this.breakSFXVolume, 1f);
				}
			}
			GameManager.Instance.VibrationManager.Vibrate(this.abilityData.secondaryVibration, VibrationRegion.WholeBody, true);
		}
		this.oozeGatheringActiveAudioSource.Stop();
		this.oozeGatheringPassiveAudioSource.Stop();
		base.Deactivate();
		this.RefreshDeploymentState();
	}

	private void Update()
	{
		this.change = 0m;
		if (this.isActive && this.trawlNetItemInstance != null)
		{
			bool flag = this.trawlNetItemInstance.durability > 0f;
			bool isMoving = GameManager.Instance.Player.Controller.IsMoving;
			if (flag)
			{
				if (isMoving)
				{
					this.change = GameManager.Instance.Time.GetTimeChangeThisFrame();
					this.modifiedChange = this.change * (decimal)(1f - GameManager.Instance.PlayerStats.ResearchedEquipmentMaintenanceModifier);
					this.trawlNetItemInstance.ChangeDurability((float)(-this.modifiedChange));
				}
				if (this.trawlNetItemInstance.durability > 0f)
				{
					if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_OOZE)
					{
						if (GameManager.Instance.OozePatchManager.TryGetOozePatchAtPosition(base.transform.position, out this.currentOozePatch))
						{
							this.OnOozeGathered(this.currentOozePatch.CollectOoze());
						}
						bool isOozeNearToPlayer = GameManager.Instance.OozePatchManager.isOozeNearToPlayer;
						this.UpdateOozePassiveSFX(isOozeNearToPlayer);
						return;
					}
					if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL || this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
					{
						this.timeUntilNextCatchRoll -= this.change;
						if (this.timeUntilNextCatchRoll <= 0m)
						{
							if (global::UnityEngine.Random.value < this.trawlNetItemData.CatchRate)
							{
								this.AddTrawlItem();
							}
							this.RefreshTimeUntilNextCatchRoll();
							return;
						}
					}
				}
				else
				{
					this.Deactivate();
					GameEvents.Instance.TriggerItemInventoryChanged(this.trawlNetItemInstance.GetItemData<DeployableItemData>());
				}
			}
		}
	}

	private void OnOozeGathered(float amount)
	{
		GameManager.Instance.OozePatchManager.NotifyOozeGathered(amount * (float)GameManager.Instance.PlayerStats.TrawlCatchRateGadgetModifier);
	}

	private void UpdateOozePassiveSFX(bool isInOoze)
	{
		this.oozeGatheringActiveAudioSource.volume = Mathf.Lerp(this.oozeGatheringActiveAudioSource.volume, isInOoze ? 1f : 0f, Time.deltaTime * this.oozeGatheringChangeModifier);
		this.oozeGatheringPassiveAudioSource.volume = Mathf.Lerp(this.oozeGatheringPassiveAudioSource.volume, isInOoze ? 0f : 1f, Time.deltaTime * this.oozeGatheringChangeModifier);
	}

	private bool CheckCanBeCaughtByThisNet(HarvestableItemData itemData)
	{
		if (!itemData.canBeCaughtByNet)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < this.trawlNetItemData.harvestableTypes.Length; i++)
		{
			if (this.trawlNetItemData.harvestableTypes[i] == itemData.harvestableType)
			{
				flag = true;
			}
		}
		return flag;
	}

	private void AddTrawlItem()
	{
		if (this.isInFinale)
		{
			return;
		}
		float num = GameManager.Instance.WaveController.SampleWaterDepthAtPlayerPosition();
		List<string> harvestableItemIds = GameManager.Instance.Player.HarvestZoneDetector.GetHarvestableItemIds(new Func<HarvestableItemData, bool>(this.CheckCanBeCaughtByThisNet), num, GameManager.Instance.Time.IsDaytime);
		if (harvestableItemIds.Count == 0)
		{
			return;
		}
		float[] array = new float[harvestableItemIds.Count];
		for (int i = 0; i < harvestableItemIds.Count; i++)
		{
			array[i] = GameManager.Instance.ItemManager.GetItemDataById<HarvestableItemData>(harvestableItemIds[i]).harvestItemWeight;
		}
		int randomWeightedIndex = MathUtil.GetRandomWeightedIndex(array);
		string text = harvestableItemIds[randomWeightedIndex];
		HarvestableItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<HarvestableItemData>(text);
		if (itemDataById == null)
		{
			Debug.LogWarning("[TrawlNetAbility] AddTrawlItem() chosen item data could not be found");
			return;
		}
		Vector3Int vector3Int = default(Vector3Int);
		if (GameManager.Instance.SaveData.TrawlNet.FindPositionForObject(itemDataById, out vector3Int, 0, false))
		{
			if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL)
			{
				FishSizeGenerationMode fishSizeGenerationMode = FishSizeGenerationMode.NO_BIG_TROPHY;
				float num2 = 1f;
				if (!itemDataById.canBeCaughtByPot && !itemDataById.canBeCaughtByRod)
				{
					fishSizeGenerationMode = FishSizeGenerationMode.ANY;
					num2 = 2f;
				}
				FishItemInstance fishItemInstance = GameManager.Instance.ItemManager.CreateFishItem(text, FishAberrationGenerationMode.RANDOM_CHANCE, false, fishSizeGenerationMode, num2);
				GameManager.Instance.SaveData.TrawlNet.AddObjectToGridData(fishItemInstance, vector3Int, true, null);
				GameManager.Instance.ItemManager.SetItemSeen(fishItemInstance);
				GameEvents.Instance.TriggerFishCaught();
				GameManager.Instance.VibrationManager.Vibrate(this.fishAddedVibrationData, VibrationRegion.WholeBody, true).Run();
			}
			else if (this.trawlMode == TrawlNetAbility.TrawlMode.TRAWL_MATERIAL)
			{
				SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemDataById);
				GameManager.Instance.SaveData.TrawlNet.AddObjectToGridData(spatialItemInstance, vector3Int, true, null);
				GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance);
				GameManager.Instance.VibrationManager.Vibrate(this.materialAddedVibrationData, VibrationRegion.WholeBody, true).Run();
			}
			GameManager.Instance.AudioPlayer.PlaySFX(this.GetCatchSFX(), AudioLayer.SFX_PLAYER, this.catchSFXVolume, 1f);
			SaveData saveData = GameManager.Instance.SaveData;
			int netFishCaught = saveData.NetFishCaught;
			saveData.NetFishCaught = netFishCaught + 1;
			return;
		}
		Debug.LogWarning("[TrawlNetAbility] AddTrawlItem() failed to find space for item");
	}

	public override int GetItemCount()
	{
		return GameManager.Instance.SaveData.TrawlNet.spatialItems.Count;
	}

	private void AddTrawlItem(CommandArg[] args)
	{
		this.AddTrawlItem();
	}

	public void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("trawl.add", new Action<CommandArg[]>(this.AddTrawlItem), 0, 0, "Adds an item to the trawl net based on the active harvest zone(s)");
		}
	}

	public void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("time.set");
		}
	}

	[SerializeField]
	private Player playerRef;

	[SerializeField]
	private AssetReference endSFX;

	[SerializeField]
	private AssetReference breakSFX;

	[SerializeField]
	private AssetReference catchSFX;

	[SerializeField]
	private AssetReference deploySFX;

	[SerializeField]
	private AssetReference retractSFX;

	[SerializeField]
	private VibrationData fishAddedVibrationData;

	[SerializeField]
	private SpatialItemData oozeNetData;

	[SerializeField]
	private AssetReference oozeEndSFX;

	[SerializeField]
	private AssetReference oozeBreakSFX;

	[SerializeField]
	private AssetReference oozeDeploySFX;

	[SerializeField]
	private AssetReference oozeRetractSFX;

	[SerializeField]
	private AudioSource oozeGatheringActiveAudioSource;

	[SerializeField]
	private AudioSource oozeGatheringPassiveAudioSource;

	[SerializeField]
	private float oozeGatheringChangeModifier;

	[SerializeField]
	private VibrationData oozeGatheringVibrationData;

	[SerializeField]
	private VibrationData oozePassiveVibrationData;

	[SerializeField]
	private VibrationData oozeAddedVibrationData;

	[SerializeField]
	private float endSFXVolume;

	[SerializeField]
	private float breakSFXVolume;

	[SerializeField]
	private float catchSFXVolume;

	[SerializeField]
	private SpatialItemData materialNetData;

	[SerializeField]
	private VibrationData materialAddedVibrationData;

	[SerializeField]
	private AssetReference materialNetBreakSFX;

	[SerializeField]
	private AssetReference materialNetCatchSFX;

	[SerializeField]
	private AssetReference materialNetDeploySFX;

	[SerializeField]
	private AssetReference materialNetRetractSFX;

	private OozePatch currentOozePatch;

	private TrawlNetAbility.TrawlMode trawlMode;

	private SpatialItemInstance trawlNetItemInstance;

	private DeployableItemData trawlNetItemData;

	private decimal timeUntilNextCatchRoll;

	private SerializableGrid trawlNet;

	private bool isInFinale;

	private decimal change;

	private decimal modifiedChange;

	private enum TrawlMode
	{
		NONE,
		TRAWL,
		TRAWL_OOZE,
		TRAWL_MATERIAL
	}
}
