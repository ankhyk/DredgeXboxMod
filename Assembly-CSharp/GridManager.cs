using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[DefaultExecutionOrder(-900)]
public class GridManager : MonoBehaviour
{
	public CursorProxy CursorProxy
	{
		get
		{
			return this.cursorProxy;
		}
	}

	public GraphicRaycaster GraphicRaycaster
	{
		get
		{
			return this.graphicRaycaster;
		}
	}

	public GridCell LastSelectedCell
	{
		get
		{
			return this.lastSelectedCell;
		}
		set
		{
			this.lastSelectedCell = value;
		}
	}

	public GridObject CurrentlyHeldObject
	{
		get
		{
			return this.currentlyHeldObject;
		}
	}

	public GridObject CurrentlyHoveredObject
	{
		get
		{
			return this.currentlyHoveredObject;
		}
	}

	public GridObject PendingHeldObject
	{
		get
		{
			return this.pendingHeldObject;
		}
		set
		{
			this.pendingHeldObject = value;
		}
	}

	public Vector2 CurrentHeldObjectSelectionOffset
	{
		get
		{
			return this.currentHeldObjectSelectionOffset;
		}
		set
		{
			this.currentHeldObjectSelectionOffset = value;
		}
	}

	public Vector2Int CurrentHeldObjectSelectionOffsetCoord { get; set; }

	public float ScaledCellSize
	{
		get
		{
			return this.cellSize * this.canvasScaler.transform.localScale.x;
		}
	}

	public GridUI LastActiveGrid
	{
		get
		{
			return this.lastActiveGrid;
		}
		set
		{
			this.lastActiveGrid = value;
			GameManager.Instance.ScreenSideSwitcher.OnSideBecomeActive(this.lastActiveGrid.ScreenSide);
		}
	}

	public bool IsShowingGrid
	{
		get
		{
			return this.gridsShowing.Count > 0;
		}
	}

	public bool IsInRepairMode { get; set; }

	public UpgradeData CurrentUpgradePreviewData { get; private set; }

	public bool IsPreviewingSlotUpgrade { get; private set; }

	public string ShipwrightDestinationKey
	{
		get
		{
			return this.shipwrightDestinationKey;
		}
	}

	public void AddActiveGrid(GridUI gridUI)
	{
		if (!this.activeGrids.Contains(gridUI))
		{
			this.activeGrids.Add(gridUI);
		}
	}

	public void RemoveActiveGrid(GridUI gridUI)
	{
		this.activeGrids.Remove(gridUI);
	}

	public int GetNumActiveGrids()
	{
		return this.activeGrids.Count;
	}

	public bool IsCurrentlyHoldingObject()
	{
		return this.currentlyHeldObject != null || this.pendingHeldObject != null;
	}

	public GridObject GetCurrentlyFocusedObject()
	{
		if (this.CurrentlyHeldObject)
		{
			return this.CurrentlyHeldObject;
		}
		if (this.CurrentlyHoveredObject)
		{
			return this.CurrentlyHoveredObject;
		}
		return null;
	}

	private void Awake()
	{
		GameManager.Instance.GridManager = this;
	}

	private void OnEnable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnPlayerDamageChanged += this.OnPlayerDamageChanged;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemAdded += this.OnItemAdded;
		GameEvents.Instance.OnItemDestroyed += this.OnItemDestroyed;
		GameEvents.Instance.OnEquipmentDamageChanged += this.OnEquipmentDamageChanged;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnUpgradePreviewed += this.OnUpgradePreviewed;
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Combine(instance.OnGameEnded, new Action(this.OnGameEnded));
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnPlayerDamageChanged -= this.OnPlayerDamageChanged;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemAdded -= this.OnItemAdded;
		GameEvents.Instance.OnItemDestroyed -= this.OnItemDestroyed;
		GameEvents.Instance.OnEquipmentDamageChanged -= this.OnEquipmentDamageChanged;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnUpgradePreviewed -= this.OnUpgradePreviewed;
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Remove(instance.OnGameEnded, new Action(this.OnGameEnded));
		this.RemoveTerminalCommands();
	}

	private void OnGameEnded()
	{
		this.DiscardCurrentObject();
	}

	private void OnUpgradePreviewed(UpgradeData upgradeData)
	{
		this.CurrentUpgradePreviewData = upgradeData;
		if (upgradeData == null)
		{
			this.IsPreviewingSlotUpgrade = false;
			return;
		}
		this.IsPreviewingSlotUpgrade = upgradeData is SlotUpgradeData;
	}

	public void ReportGridShowing(GridUI gridUI, bool showing)
	{
		if (showing)
		{
			this.gridsShowing.Add(gridUI);
			return;
		}
		this.gridsShowing.Remove(gridUI);
	}

	public bool GetIsGridShowing(GridUI gridUI)
	{
		return this.gridsShowing.Contains(gridUI);
	}

	public void ToggleRepairMode()
	{
		this.SetRepairMode(!this.IsInRepairMode);
	}

	public void SetRepairMode(bool isActive)
	{
		if (this.IsInRepairMode == isActive)
		{
			return;
		}
		this.IsInRepairMode = isActive;
		GameEvents.Instance.TriggerMaintenanceModeToggled(this.IsInRepairMode);
		if (this.IsInRepairMode)
		{
			GameManager.Instance.UI.InventoryGrid.SelectFirstCellContainingItemType(ItemType.DAMAGE);
			return;
		}
		this.TrySelectPreviousCell();
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.TrySelectCellUnderCursor();
	}

	public BaseGridModeActionHandler AddGridActionHandler(GridMode mode)
	{
		if (this.activeActionHandlers.Find((BaseGridModeActionHandler a) => a.mode == mode) != null)
		{
			return null;
		}
		BaseGridModeActionHandler baseGridModeActionHandler = null;
		if (mode == GridMode.DEFAULT)
		{
			baseGridModeActionHandler = new DefaultActionHandler();
		}
		else if (mode == GridMode.STORAGE)
		{
			baseGridModeActionHandler = new StorageModeActionHandler();
		}
		else if (mode == GridMode.SELL_TYPE)
		{
			baseGridModeActionHandler = new TypeSellModeActionHandler();
		}
		else if (mode == GridMode.SELL_SPECIFIC)
		{
			baseGridModeActionHandler = new SpecificSellModeActionHandler();
		}
		else if (mode == GridMode.BUY)
		{
			baseGridModeActionHandler = new BuyModeActionHandler();
		}
		else if (mode == GridMode.EQUIPMENT)
		{
			baseGridModeActionHandler = new EquipmentModeActionHandler();
		}
		else if (mode == GridMode.MAINTENANCE)
		{
			baseGridModeActionHandler = new RepairActionHandler();
		}
		if (baseGridModeActionHandler != null)
		{
			BaseGridModeActionHandler baseGridModeActionHandler2 = baseGridModeActionHandler;
			baseGridModeActionHandler2.OnPlacePressedCallback = (Action)Delegate.Combine(baseGridModeActionHandler2.OnPlacePressedCallback, new Action(this.OnPlacePressed));
			BaseGridModeActionHandler baseGridModeActionHandler3 = baseGridModeActionHandler;
			baseGridModeActionHandler3.OnPickUpPressedCallback = (Action)Delegate.Combine(baseGridModeActionHandler3.OnPickUpPressedCallback, new Action(this.OnPickUpPressed));
			BaseGridModeActionHandler baseGridModeActionHandler4 = baseGridModeActionHandler;
			baseGridModeActionHandler4.OnRotatePressedCallback = (Action<float>)Delegate.Combine(baseGridModeActionHandler4.OnRotatePressedCallback, new Action<float>(this.OnRotatePressed));
		}
		this.activeActionHandlers.Add(baseGridModeActionHandler);
		return baseGridModeActionHandler;
	}

	public T GetActionHandlerOfType<T>() where T : BaseGridModeActionHandler
	{
		return this.activeActionHandlers.OfType<T>().FirstOrDefault<T>();
	}

	public void RemoveGridActionHander(GridMode mode)
	{
		BaseGridModeActionHandler baseGridModeActionHandler = this.activeActionHandlers.Find((BaseGridModeActionHandler a) => a.mode == mode);
		if (baseGridModeActionHandler != null)
		{
			this.activeActionHandlers.Remove(baseGridModeActionHandler);
			baseGridModeActionHandler.Shutdown();
			BaseGridModeActionHandler baseGridModeActionHandler2 = baseGridModeActionHandler;
			baseGridModeActionHandler2.OnPlacePressedCallback = (Action)Delegate.Remove(baseGridModeActionHandler2.OnPlacePressedCallback, new Action(this.OnPlacePressed));
			BaseGridModeActionHandler baseGridModeActionHandler3 = baseGridModeActionHandler;
			baseGridModeActionHandler3.OnPickUpPressedCallback = (Action)Delegate.Remove(baseGridModeActionHandler3.OnPickUpPressedCallback, new Action(this.OnPickUpPressed));
			BaseGridModeActionHandler baseGridModeActionHandler4 = baseGridModeActionHandler;
			baseGridModeActionHandler4.OnRotatePressedCallback = (Action<float>)Delegate.Remove(baseGridModeActionHandler4.OnRotatePressedCallback, new Action<float>(this.OnRotatePressed));
		}
	}

	public void ClearActionHandlers()
	{
		this.activeActionHandlers.ForEach(delegate(BaseGridModeActionHandler ah)
		{
			ah.Shutdown();
			BaseGridModeActionHandler baseGridModeActionHandler = ah;
			baseGridModeActionHandler.OnPlacePressedCallback = (Action)Delegate.Remove(baseGridModeActionHandler.OnPlacePressedCallback, new Action(this.OnPlacePressed));
			BaseGridModeActionHandler baseGridModeActionHandler2 = ah;
			baseGridModeActionHandler2.OnPickUpPressedCallback = (Action)Delegate.Remove(baseGridModeActionHandler2.OnPickUpPressedCallback, new Action(this.OnPickUpPressed));
			BaseGridModeActionHandler baseGridModeActionHandler3 = ah;
			baseGridModeActionHandler3.OnRotatePressedCallback = (Action<float>)Delegate.Remove(baseGridModeActionHandler3.OnRotatePressedCallback, new Action<float>(this.OnRotatePressed));
			ah = null;
		});
		this.activeActionHandlers.Clear();
	}

	public void ObjectPickedUp(GridObject o)
	{
		this.currentlyHeldObject = o;
		GameEvents.Instance.TriggerItemPickupEvent(o);
	}

	public void RefreshCurrentlySelectedCell()
	{
		GridCell gridCell = this.lastSelectedCell;
		this.lastSelectedCell = null;
		this.OnSelectUpdate(gridCell);
	}

	public void ClearLastSelectedCell()
	{
		this.OnSelectUpdate(null);
	}

	public void OnSelectUpdate(GridCell newSelectedCell)
	{
		if (this.suppressNextSelectUpdate)
		{
			return;
		}
		if (this.lastSelectedCell != newSelectedCell)
		{
			this.lastObject = null;
			this.newObject = null;
			if (this.lastSelectedCell)
			{
				this.lastObject = this.lastSelectedCell.OccupyingObject ?? this.lastSelectedCell.OccupyingHintObject;
				if (!this.lastObject || (this.IsInRepairMode && this.lastSelectedCell.OccupyingUnderlayObject != null))
				{
					this.lastObject = this.lastSelectedCell.OccupyingUnderlayObject;
				}
			}
			if (newSelectedCell)
			{
				this.newObject = newSelectedCell.OccupyingObject ?? newSelectedCell.OccupyingHintObject;
				if (!this.newObject || (this.IsInRepairMode && newSelectedCell.OccupyingUnderlayObject != null))
				{
					this.newObject = newSelectedCell.OccupyingUnderlayObject;
				}
				this.LastActiveGrid = newSelectedCell.ParentGrid;
				this.shouldTriggerNewCellFocus = true;
				this.newCellFocus = newSelectedCell;
			}
			else
			{
				this.shouldTriggerNewCellFocus = true;
				this.newCellFocus = null;
			}
			if (this.lastObject != this.newObject)
			{
				this.currentlyHoveredObject = this.newObject;
				GameEvents.Instance.TriggerItemHoveredChanged(this.currentlyHoveredObject);
				if (this.currentlyHoveredObject != null)
				{
					this.currentlyHoveredObject.OnHoverChanged(true);
				}
			}
			this.lastSelectedCell = newSelectedCell;
			this._isDirty = true;
		}
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool success)
	{
		if (success)
		{
			if (gridObject.state == GridObjectState.IN_INVENTORY && gridObject.ItemData.itemType == ItemType.EQUIPMENT && this.CheckForDamage(gridObject.SpatialItemInstance))
			{
				GameEvents.Instance.TriggerEquipmentDamageChanged(gridObject.ItemData);
			}
			GameEvents.Instance.TriggerItemInventoryChanged(gridObject.ItemData);
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		if (gridObject.ItemData.itemType == ItemType.EQUIPMENT)
		{
			gridObject.SpatialItemInstance.SetIsOnDamagedCell(false);
		}
		GameEvents.Instance.TriggerItemInventoryChanged(gridObject.ItemData);
	}

	private void OnItemAdded(SpatialItemInstance spatialItemInstance, bool belongsToPlayer)
	{
		GameEvents.Instance.TriggerItemInventoryChanged(spatialItemInstance.GetItemData<SpatialItemData>());
	}

	private void OnItemDestroyed(SpatialItemData spatialItemData, bool playerDestroyed)
	{
		GameEvents.Instance.TriggerItemInventoryChanged(spatialItemData);
	}

	private void OnEquipmentDamageChanged(SpatialItemData spatialItemData)
	{
		GameEvents.Instance.TriggerItemInventoryChanged(spatialItemData);
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		GameEvents.Instance.TriggerItemInventoryChanged(gridObject.ItemData);
	}

	public SpatialItemInstance GetDamageToRepairWithEquipmentPriority(List<ItemSubtype> targetSubtypes, out bool didFindEquipmentToRepair)
	{
		SpatialItemInstance spatialItemInstance = null;
		didFindEquipmentToRepair = false;
		int num = 0;
		while (num < targetSubtypes.Count && !didFindEquipmentToRepair)
		{
			int num2 = 0;
			while (num2 < GameManager.Instance.SaveData.Inventory.spatialUnderlayItems.Count && !didFindEquipmentToRepair)
			{
				SpatialItemInstance spatialItemInstance2 = GameManager.Instance.SaveData.Inventory.spatialUnderlayItems[num2];
				Vector3Int position = spatialItemInstance2.GetPosition();
				SpatialItemInstance itemAtPosition = GameManager.Instance.SaveData.Inventory.GetItemAtPosition(position);
				if (itemAtPosition != null && itemAtPosition.GetItemData<SpatialItemData>().itemSubtype.HasFlag(targetSubtypes[num]))
				{
					didFindEquipmentToRepair = true;
					spatialItemInstance = spatialItemInstance2;
					break;
				}
				num2++;
			}
			num++;
		}
		if (spatialItemInstance == null && GameManager.Instance.SaveData.Inventory.spatialUnderlayItems.Count > 0)
		{
			spatialItemInstance = GameManager.Instance.SaveData.Inventory.spatialUnderlayItems[0];
		}
		return spatialItemInstance;
	}

	public bool GetGridCoordWithDamageAndType(ItemType targetType, ItemSubtype targetSubtype, out Vector3Int pos)
	{
		bool flag = false;
		List<SpatialItemInstance> list = (from i in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(targetType, targetSubtype)
			where i.GetIsOnDamagedCell()
			select i).ToList<SpatialItemInstance>();
		if (list.Count > 0)
		{
			flag = true;
			pos = list.PickRandom<SpatialItemInstance>().GetPosition();
		}
		else
		{
			pos = Vector3Int.zero;
		}
		return flag;
	}

	public void AddDamageToInventory(ItemType targetType, ItemSubtype targetSubtype)
	{
		if (GameManager.Instance.Player.IsImmuneModeEnabled)
		{
			return;
		}
		List<SpatialItemInstance> list = (from i in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(targetType, targetSubtype)
			where !i.GetIsOnDamagedCell()
			select i).ToList<SpatialItemInstance>();
		bool flag = false;
		if (list.Count > 0)
		{
			Vector3Int position = list.PickRandom<SpatialItemInstance>().GetPosition();
			flag = this.AddDamageToInventory(1, position.x, position.y);
		}
		if (!flag)
		{
			this.AddDamageToInventory(1, -1, -1);
		}
	}

	public bool AddDamageToInventory(int numDamage = 1, int xParam = -1, int yParam = -1)
	{
		if (GameManager.Instance.Player.IsImmuneModeEnabled)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < numDamage; i++)
		{
			int j = xParam;
			int k = yParam;
			SpatialItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<SpatialItemData>("dmg");
			if (itemDataById == null)
			{
				return false;
			}
			SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(itemDataById);
			if (spatialItemInstance == null)
			{
				return false;
			}
			SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
			GridCellData gridCellData = null;
			if (j == -1 || k == -1)
			{
				int num = 0;
				int num2 = 100;
				int columns = inventory.GridConfiguration.columns;
				int rows = inventory.GridConfiguration.rows;
				j = 0;
				k = 0;
				while (gridCellData == null && num < num2)
				{
					j = global::UnityEngine.Random.Range(0, columns);
					k = global::UnityEngine.Random.Range(0, rows);
					GridCellData gridCellData2 = inventory.Grid[j, k];
					if (gridCellData2.CanDamageThisCell())
					{
						gridCellData = gridCellData2;
					}
					num++;
				}
				if (gridCellData == null)
				{
					for (j = 0; j < inventory.GridConfiguration.columns; j++)
					{
						for (k = 0; k < inventory.GridConfiguration.rows; k++)
						{
							GridCellData gridCellData3 = inventory.Grid[j, k];
							if (gridCellData3.CanDamageThisCell())
							{
								gridCellData = gridCellData3;
								break;
							}
						}
					}
				}
			}
			else
			{
				gridCellData = inventory.Grid[j, k];
			}
			if (gridCellData != null && gridCellData.CanDamageThisCell())
			{
				Vector3Int vector3Int = new Vector3Int(j, k, 0);
				if (gridCellData.spatialItemInstance != null)
				{
					flag2 = true;
					SpatialItemData itemData = gridCellData.spatialItemInstance.GetItemData<SpatialItemData>();
					if (itemData.damageMode == DamageMode.DESTROY)
					{
						GameEvents.Instance.TriggerItemDestroyed(itemData, false);
						inventory.RemoveObjectFromGridData(gridCellData.spatialItemInstance, true);
						this.PrepareItemNameForNotification(NotificationType.ITEM_REMOVED, "notification.damage-sustained-item-lost", itemData.itemNameKey, DredgeColorTypeEnum.NEGATIVE);
					}
					if (itemData.damageMode == DamageMode.DURABILITY)
					{
						float durability = gridCellData.spatialItemInstance.durability;
						gridCellData.spatialItemInstance.ChangeDurability(-(itemData as DeployableItemData).MaxDurabilityDays * GameManager.Instance.GameConfigData.DeployableDamagePerHitProportional);
						if (itemData.itemSubtype == ItemSubtype.NET && durability > 0f && gridCellData.spatialItemInstance.durability <= 0f)
						{
							GameEvents.Instance.TriggerItemInventoryChanged(gridCellData.spatialItemInstance.GetItemData<DeployableItemData>());
						}
						this.PrepareItemNameForNotification(NotificationType.DURABILITY_LOST, "notification.damage-sustained-durability-lost", itemData.itemNameKey, DredgeColorTypeEnum.NEGATIVE);
					}
				}
				flag = true;
				inventory.AddObjectToGridData(spatialItemInstance, vector3Int, true, null);
			}
		}
		if (!flag2 && flag)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.DAMAGE_TAKEN, "notification.damage-sustained");
		}
		if (flag)
		{
			GameEvents.Instance.TriggerOnPlayerDamageChanged();
		}
		return flag;
	}

	private void InfectItemAtInventoryPosition(int x, int y)
	{
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		GridCellData gridCellData = inventory.Grid[x, y];
		if (gridCellData != null && gridCellData.spatialItemInstance != null && gridCellData.spatialItemInstance is FishItemInstance && (gridCellData.spatialItemInstance as FishItemInstance).GetItemData<FishItemData>().CanBeInfected)
		{
			this.InfectItem(gridCellData.spatialItemInstance as FishItemInstance, inventory, false);
			GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.infection-start", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
		}
	}

	public void InfectRandomItemInInventory()
	{
		SerializableGrid inventory = GameManager.Instance.SaveData.Inventory;
		FishItemInstance fishItemInstance = inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH).Shuffle<FishItemInstance>().ToList<FishItemInstance>()
			.FirstOrDefault((FishItemInstance f) => !f.isInfected && f.GetItemData<FishItemData>().CanBeInfected);
		if (fishItemInstance != null)
		{
			this.InfectItem(fishItemInstance, inventory, false);
			GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.SPOOKY_EVENT, "notification.infection-start", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
		}
	}

	public void TryAddDarkSplashToInventory()
	{
		Vector3Int zero = Vector3Int.zero;
		if (GameManager.Instance.SaveData.Inventory.FindRandomPositionForObject(this.darkSplashItemData, out zero))
		{
			SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(this.darkSplashItemData);
			GameManager.Instance.SaveData.Inventory.AddObjectToGridData(spatialItemInstance, zero, true, new Action<GridObject>(this.PostCreateDarkSplash));
			GameManager.Instance.UI.ShowNotificationWithColor(NotificationType.DARK_SPLASH_ADDED, "notification.dark-splash-added", GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.CRITICAL));
			GameManager.Instance.VibrationManager.Vibrate(this.darkSplashVibrationData, VibrationRegion.WholeBody, true);
			int num = GameManager.Instance.SaveData.Inventory.GridConfiguration.columns - 1;
			int num2 = GameManager.Instance.SaveData.Inventory.GridConfiguration.rows - 1;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						int num3 = zero.x + i;
						int num4 = zero.y + j;
						if (num3 >= 0 && num3 <= num && num4 >= 0 && num4 <= num2)
						{
							GridCellData gridCellData = GameManager.Instance.SaveData.Inventory.Grid[num3, num4];
							if (gridCellData.spatialItemInstance is FishItemInstance && !(gridCellData.spatialItemInstance as FishItemInstance).isInfected && (gridCellData.spatialItemInstance as FishItemInstance).GetItemData<FishItemData>().CanBeInfected && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.DarkSplashInfectionChance)
							{
								GameManager.Instance.GridManager.InfectItem(gridCellData.spatialItemInstance as FishItemInstance, GameManager.Instance.SaveData.Inventory, true);
							}
						}
					}
				}
			}
		}
	}

	private void PostCreateDarkSplash(GridObject darkSplashGridObject)
	{
		if (GameManager.Instance.UI.InventoryGrid.gameObject.activeInHierarchy)
		{
			global::UnityEngine.Object.Instantiate<GameObject>(this.darkSplashSpawnVFX, this.darkSplashSpawnVFXContainer).transform.position = darkSplashGridObject.transform.position;
		}
	}

	public bool InfectItem(FishItemInstance itemInstance, SerializableGrid grid, bool canBecomeAberrations)
	{
		itemInstance.Infect();
		if (canBecomeAberrations && global::UnityEngine.Random.value < GameManager.Instance.GameConfigData.InfectionAberrationSwapChance)
		{
			GameManager.Instance.ItemManager.ReplaceFishWithAberration(itemInstance, grid, true, true);
		}
		return true;
	}

	private void PrepareItemNameForNotification(NotificationType notificationType, string notificationKey, LocalizedString itemNameKey, DredgeColorTypeEnum itemNameColor)
	{
		LocalizationSettings.StringDatabase.GetLocalizedStringAsync(itemNameKey.TableReference, itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>()).Completed += delegate(AsyncOperationHandle<string> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				GameManager.Instance.UI.ShowNotification(notificationType, notificationKey, new object[] { string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(itemNameColor),
					">",
					op.Result,
					"</color>"
				}) });
			}
		};
	}

	private bool CheckForDamage(SpatialItemInstance spatialItemInstance)
	{
		IEnumerable<GridCellData> cellsAffectedByObjectAtPosition = GameManager.Instance.SaveData.Inventory.GetCellsAffectedByObjectAtPosition(spatialItemInstance.GetItemData<SpatialItemData>().dimensions, spatialItemInstance.GetPosition());
		bool isOnDamagedCell = spatialItemInstance.GetIsOnDamagedCell();
		bool flag = cellsAffectedByObjectAtPosition.Any(delegate(GridCellData gcd)
		{
			SpatialItemInstance underlaySpatialItemInstance = gcd.underlaySpatialItemInstance;
			return underlaySpatialItemInstance != null && underlaySpatialItemInstance.GetItemData<SpatialItemData>().itemType == ItemType.DAMAGE;
		});
		if (spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.OPERATION)
		{
			spatialItemInstance.SetIsOnDamagedCell(flag);
		}
		return isOnDamagedCell != flag;
	}

	private void OnPlayerDamageChanged()
	{
		SpatialItemData thisItemChangedDamageState = null;
		GameManager.Instance.SaveData.Inventory.spatialItems.ForEach(delegate(SpatialItemInstance spatialItemInstance)
		{
			bool flag = this.CheckForDamage(spatialItemInstance);
			if (thisItemChangedDamageState == null && flag && spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.OPERATION)
			{
				thisItemChangedDamageState = spatialItemInstance.GetItemData<SpatialItemData>();
				if (spatialItemInstance.GetIsOnDamagedCell())
				{
					this.PrepareItemNameForNotification(NotificationType.EQUIPMENT_DAMAGED, "notification.damage-sustained-eq-damaged", spatialItemInstance.GetItemData<SpatialItemData>().itemNameKey, DredgeColorTypeEnum.NEGATIVE);
					return;
				}
				this.PrepareItemNameForNotification(NotificationType.EQUIPMENT_REPAIRED, "notification.eq-repaired", spatialItemInstance.GetItemData<SpatialItemData>().itemNameKey, DredgeColorTypeEnum.POSITIVE);
			}
		});
		if (thisItemChangedDamageState != null)
		{
			GameEvents.Instance.TriggerEquipmentDamageChanged(thisItemChangedDamageState);
		}
	}

	public GridObject AddItemOfTypeToCursor(SpatialItemInstance spatialItemInstance, GridObjectState state)
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.gridObjectPrefab, this.heldItemContainer.transform);
		gameObject.name = spatialItemInstance.GetItemData<SpatialItemData>().name;
		GridObject component = gameObject.GetComponent<GridObject>();
		component.Init(spatialItemInstance, null, state);
		RectTransform rectTransform = component.transform as RectTransform;
		Vector2 vector = new Vector2((float)component.ItemData.GetWidth() * this.cellSize, (float)component.ItemData.GetHeight() * this.cellSize);
		rectTransform.sizeDelta = vector;
		(component.transform as RectTransform).SetPivot(new Vector2(0.5f, 0.5f));
		component.SetIsPickedUp(true);
		this.ObjectPickedUp(component);
		if (GameManager.Instance.Input.IsUsingController)
		{
			Vector3Int zero = Vector3Int.zero;
			GridCell gridCell;
			if (GameManager.Instance.UI.InventoryGrid.linkedGrid.FindPositionForObject(component.ItemData, out zero, 0, false))
			{
				gridCell = GameManager.Instance.UI.InventoryGrid.GetCellAtPos(zero.x, zero.y);
			}
			else
			{
				gridCell = GameManager.Instance.UI.InventoryGrid.GetFirstEmptyCell(component.ItemData);
			}
			if (gridCell == null)
			{
				gridCell = GameManager.Instance.UI.InventoryGrid.GetFirstCellForItemType(component.ItemData, true);
			}
			if (gridCell)
			{
				component.transform.position = gridCell.transform.position;
				float num = 0f;
				float num2 = 0f;
				if (component.ItemData.GetWidth() % 2 == 0)
				{
					num = this.ScaledCellSize * 0.5f;
				}
				if (component.ItemData.GetHeight() % 2 == 0)
				{
					num2 = -(this.ScaledCellSize * 0.5f);
				}
				this.CurrentHeldObjectSelectionOffset = new Vector2(num, num2);
				EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
				gridCell.OnSelected();
			}
		}
		else
		{
			this.TrySelectPreviousCell();
		}
		return component;
	}

	private void LateUpdate()
	{
		if (this.currentlyHeldObject)
		{
			this.currentlyHeldObject.transform.position = this.cursorProxy.GetPosition();
		}
		if (this.shouldTriggerNewCellFocus)
		{
			GameEvents.Instance.TriggerFocusedGridCellChanged(this.newCellFocus);
			this.shouldTriggerNewCellFocus = false;
			this.newCellFocus = null;
		}
		if (this.suppressNextSelectUpdate)
		{
			this.suppressNextSelectUpdate = false;
		}
	}

	private void FixedUpdate()
	{
		if (!GameManager.Instance.Input.IsUsingController)
		{
			this.timeUntilForceGridCellRepaint -= Time.fixedDeltaTime;
			if (this.timeUntilForceGridCellRepaint <= 0f)
			{
				this.timeUntilForceGridCellRepaint = this.forceRefreshCellRepaintInterval;
				this._isDirty = true;
			}
		}
		if (this._isDirty)
		{
			foreach (GridUI gridUI in this.gridsShowing)
			{
				gridUI.ClearAllCellEffects();
			}
			if (this.currentlyHeldObject && this.lastSelectedCell)
			{
				this.ColorAffectedCellsForCurrentObject();
			}
			this._isDirty = false;
		}
	}

	private void ColorAffectedCellsForCurrentObject()
	{
		GridObjectPlacementResult placementResult = this.currentlyHeldObject.GetPlacementResult();
		CellState state = CellState.INVALID;
		if (placementResult.placementCellsValid && placementResult.placementUnobstructed && placementResult.placementCellsAcceptObject && (placementResult.placementCellsAreUndamaged || this.currentlyHeldObject.ItemData.ignoreDamageWhenPlacing))
		{
			state = CellState.VALID;
		}
		else if (!placementResult.placementCellsAreUndamaged && !this.currentlyHeldObject.ItemData.ignoreDamageWhenPlacing)
		{
			state = CellState.INVALID;
		}
		else if (!placementResult.placementCellsAcceptObject)
		{
			state = CellState.INVALID;
		}
		else if (!placementResult.placementCellsValid)
		{
			state = CellState.INVALID;
		}
		else if (!placementResult.placementUnobstructed && placementResult.objects.Count == 1 && (placementResult.objects[0].ItemData.moveMode == MoveMode.FREE || GameManager.Instance.Player.CanMoveInstalledItems))
		{
			state = CellState.SEMI_VALID;
		}
		if (placementResult.placementCellsAreInStorageTray && this.currentlyHeldObject.ItemData.ForbidStorageTray)
		{
			state = CellState.INVALID;
		}
		placementResult.cells.ForEach(delegate(GridCell gc)
		{
			gc.SetCellState(state);
		});
	}

	private void OnPickUpPressed()
	{
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			GridObject gridObject = null;
			GridCell gridCell = null;
			this.DoHitTestFromCursorProxy(out gridObject, out gridCell);
			if (gridObject && gridObject.ItemData.GetCanBeMoved())
			{
				gridObject.ParentGrid.PickUpObject(gridObject);
				this._isDirty = true;
			}
		}
	}

	private void OnPlacePressed()
	{
		if (this.currentlyHeldObject)
		{
			this.TryPlaceObject();
			this._isDirty = true;
		}
	}

	public void DiscardCurrentObject()
	{
		GridObject currentlyFocusedObject = this.GetCurrentlyFocusedObject();
		if (currentlyFocusedObject)
		{
			if (currentlyFocusedObject.ItemData.itemSubtype == ItemSubtype.FISH)
			{
				SaveData saveData = GameManager.Instance.SaveData;
				int fishDiscardCount = saveData.FishDiscardCount;
				saveData.FishDiscardCount = fishDiscardCount + 1;
			}
			this.RemoveItem(currentlyFocusedObject, true);
			GameEvents.Instance.TriggerItemDestroyed(currentlyFocusedObject.ItemData, true);
			this.TrySelectPreviousCell();
			if (currentlyFocusedObject.ItemData.hasSpecialDiscardAction)
			{
				GameEvents.Instance.TriggerSpecialItemHandlerRequest(currentlyFocusedObject.ItemData);
			}
		}
	}

	public bool AddItemInstanceToGrid(SpatialItemInstance spatialItemInstance, bool prioritizeRotation, SerializableGrid destinationGrid, SerializableGrid backupGrid = null)
	{
		bool flag = false;
		if (spatialItemInstance != null)
		{
			if (destinationGrid != null)
			{
				Vector3Int zero = Vector3Int.zero;
				bool flag2;
				if (prioritizeRotation)
				{
					flag2 = destinationGrid.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out zero, spatialItemInstance.z, true);
				}
				else
				{
					flag2 = destinationGrid.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out zero, 0, false);
				}
				if (flag2)
				{
					destinationGrid.AddObjectToGridData(spatialItemInstance, zero, true, null);
					this.TrySelectPreviousCell();
					flag = true;
				}
				else
				{
					flag = backupGrid != null && this.AddItemInstanceToGrid(spatialItemInstance, prioritizeRotation, backupGrid, null);
				}
			}
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public bool AddBulkItemInstancesToGrid(List<SpatialItemInstance> spatialItemInstances, bool prioritizeRotation, SerializableGrid destinationGrid, SerializableGrid backupGrid = null)
	{
		List<GridManager.FindPositionResult> list = new List<GridManager.FindPositionResult>();
		List<SpatialItemInstance> list2 = new List<SpatialItemInstance>();
		bool flag = true;
		int num = 0;
		int[] array = new int[] { 0, 90, 180, 270 };
		Action<SpatialItemInstance> <>9__1;
		while (flag && num < this.maxMigrateAttempts)
		{
			num++;
			list.Clear();
			list2.Clear();
			for (int i = 0; i < spatialItemInstances.Count; i++)
			{
				SpatialItemInstance spatialItemInstance = spatialItemInstances[i];
				GridManager.FindPositionResult findPositionResult = default(GridManager.FindPositionResult);
				if (prioritizeRotation)
				{
					findPositionResult.didFindPos = destinationGrid.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out findPositionResult.position, spatialItemInstance.z, true);
				}
				else
				{
					int num2 = array.PickRandom<int>();
					findPositionResult.didFindPos = destinationGrid.FindPositionForObject(spatialItemInstance.GetItemData<SpatialItemData>(), out findPositionResult.position, num2, false);
				}
				list.Add(findPositionResult);
				if (!findPositionResult.didFindPos)
				{
					flag = true;
					prioritizeRotation = false;
					List<SpatialItemInstance> list3 = list2;
					Action<SpatialItemInstance> action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate(SpatialItemInstance addedItem)
						{
							destinationGrid.RemoveObjectFromGridData(addedItem, false);
						});
					}
					list3.ForEach(action);
					spatialItemInstances = spatialItemInstances.Shuffle<SpatialItemInstance>().ToList<SpatialItemInstance>();
					break;
				}
				destinationGrid.AddObjectToGridData(spatialItemInstances[i], findPositionResult.position, false, null);
				list2.Add(spatialItemInstances[i]);
			}
			if (list.All((GridManager.FindPositionResult p) => p.didFindPos))
			{
				flag = false;
			}
		}
		bool flag2 = false;
		if (flag)
		{
			for (int j = 0; j < spatialItemInstances.Count; j++)
			{
				SpatialItemInstance spatialItemInstance2 = spatialItemInstances[j];
				GridManager.FindPositionResult findPositionResult2 = default(GridManager.FindPositionResult);
				findPositionResult2.didFindPos = destinationGrid.FindPositionForObject(spatialItemInstance2.GetItemData<SpatialItemData>(), out findPositionResult2.position, spatialItemInstance2.z, false);
				if (findPositionResult2.didFindPos)
				{
					destinationGrid.AddObjectToGridData(spatialItemInstance2, findPositionResult2.position, true, null);
				}
				else if (!this.AddItemInstanceToGrid(spatialItemInstance2, prioritizeRotation, backupGrid, null))
				{
					if (this.AddItemInstanceToGrid(spatialItemInstance2, prioritizeRotation, GameManager.Instance.SaveData.OverflowStorage, null))
					{
						flag2 = true;
					}
					else
					{
						decimal itemValue = GameManager.Instance.ItemManager.GetItemValue(spatialItemInstance2, ItemManager.BuySellMode.SELL, 1f);
						GameManager.Instance.AddFunds(itemValue);
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < spatialItemInstances.Count; k++)
			{
				destinationGrid.ForceTriggerItemAddEvent(spatialItemInstances[k]);
			}
		}
		if (flag2)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.ERROR, "notification.upgrade.items-sent-to-overflow");
		}
		return !flag;
	}

	public bool QuickTransferItemToGrid(GridObject gridObject, GridKey destinationGridKey, bool disregardSpace)
	{
		bool flag;
		if (gridObject)
		{
			if (destinationGridKey == GridKey.NONE)
			{
				this.RemoveItem(gridObject, true);
				this.TrySelectPreviousCell();
				flag = true;
			}
			else
			{
				SerializableGrid gridByKey = GameManager.Instance.SaveData.GetGridByKey(destinationGridKey);
				Vector3Int zero = Vector3Int.zero;
				bool flag2 = gridByKey.FindPositionForObject(gridObject.ItemData, out zero, 0, false);
				if (flag2 || disregardSpace)
				{
					this.RemoveItem(gridObject, true);
					if (flag2)
					{
						gridByKey.AddObjectToGridData(gridObject.SpatialItemInstance, zero, true, null);
					}
					this.TrySelectPreviousCell();
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public void RemoveItem(GridObject gridObject, bool deleteFromData)
	{
		if (gridObject)
		{
			if (gridObject.ParentGrid)
			{
				gridObject.ParentGrid.linkedGrid.RemoveObjectFromGridData(gridObject.SpatialItemInstance, true);
				return;
			}
			this.OnItemRemoved(gridObject);
		}
	}

	public void OnItemRemoved(GridObject gridObject)
	{
		if (gridObject)
		{
			if (this.currentlyHeldObject == gridObject)
			{
				this.currentlyHeldObject = null;
				if (this.currentlyHoveredObject == gridObject)
				{
					this.currentlyHoveredObject = null;
				}
				GameEvents.Instance.TriggerItemRemovedFromCursor(gridObject);
			}
			else if (this.currentlyHoveredObject == gridObject)
			{
				this.currentlyHoveredObject = null;
				GameEvents.Instance.TriggerItemRemovedFromCursor(gridObject);
			}
			if (!gridObject.ParentGrid)
			{
				global::UnityEngine.Object.Destroy(gridObject.gameObject);
			}
		}
	}

	private void TrySelectPreviousCell()
	{
		if (this.lastSelectedCell)
		{
			GridCell gridCell = this.lastSelectedCell;
			this.lastSelectedCell = null;
			gridCell.OnSelected();
		}
	}

	private void OnRotatePressed(float val)
	{
		if (this.currentlyHeldObject)
		{
			float num = 0f;
			if (val > 0f)
			{
				num = -90f;
			}
			else if (val < 0f)
			{
				num = 90f;
			}
			if (num != 0f)
			{
				this.currentlyHeldObject.SetRotation((float)this.currentlyHeldObject.CurrentRotation + num, false);
				GameEvents.Instance.TriggerItemRotated(this.currentlyHeldObject);
				if (this.lastSelectedCell)
				{
					this.lastSelectedCell.OnSelected();
				}
				this._isDirty = true;
			}
		}
	}

	public void TryPlaceObject()
	{
		if (!this.currentlyHeldObject)
		{
			return;
		}
		GridCell currentRootCell = this.currentlyHeldObject.GetCurrentRootCell();
		if (currentRootCell && currentRootCell.ParentGrid)
		{
			currentRootCell.ParentGrid.TryPlaceObject(this.currentlyHeldObject);
			return;
		}
		this.NotifyOfPlacementAttempt(this.currentlyHeldObject, false);
	}

	public void NotifyOfPlacementAttempt(GridObject o, bool placementSuccess)
	{
		if (o.ParentGrid)
		{
			o.ParentGrid.OnObjectPlaced(o, placementSuccess);
		}
		if (placementSuccess)
		{
			this.currentlyHeldObject = null;
			this.currentlyHoveredObject = null;
		}
		GameEvents.Instance.TriggerItemPlaceCompleteEvent(o, placementSuccess);
		if (placementSuccess)
		{
			if (o.state == GridObjectState.IN_INVENTORY)
			{
				GameEvents.Instance.TriggerItemAddedEvent(o.SpatialItemInstance, true);
			}
			this.TrySelectPreviousCell();
		}
	}

	public void DoHitTestFromCursorProxy(out GridObject gridObjectHit, out GridCell gridCellHit)
	{
		gridObjectHit = null;
		gridCellHit = null;
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = this.cursorProxy.CursorSquare.position;
		this.results.Clear();
		GameManager.Instance.GridManager.GraphicRaycaster.Raycast(pointerEventData, this.results);
		foreach (RaycastResult raycastResult in this.results)
		{
			if (this.gridCellLayer.Contains(raycastResult.gameObject.layer))
			{
				GridCell component = raycastResult.gameObject.GetComponent<GridCell>();
				if (component)
				{
					gridCellHit = component;
					if (component.OccupyingObject)
					{
						gridObjectHit = component.OccupyingObject;
					}
				}
			}
		}
	}

	public void TrySelectCellUnderCursor()
	{
		if (GameManager.Instance.Input.IsUsingController && this.IsShowingGrid)
		{
			GridObject gridObject = null;
			GridCell gridCell = null;
			this.DoHitTestFromCursorProxy(out gridObject, out gridCell);
			if (gridCell)
			{
				EventSystem.current.SetSelectedGameObject(gridCell.gameObject);
				gridCell.OnSelected();
				return;
			}
			if (GameManager.Instance.GridManager.LastActiveGrid != null)
			{
				GameManager.Instance.GridManager.LastActiveGrid.SelectFirstPlaceableCell();
			}
		}
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("damage", new Action<CommandArg[]>(this.DebugAddDamage), 0, 2, "Adds damage at a random location. Optionally takes 2 int coordinates as parameters.");
			Terminal.Shell.AddCommand("infect", new Action<CommandArg[]>(this.DebugInfectItem), 0, 2, "Infects an item. Use no parameters for random, or 2 int coordinates.");
			Terminal.Shell.AddCommand("func.hullmigrate.attempts", new Action<CommandArg[]>(this.DebugSetHullMigrateAttempts), 1, 1, "Sets the number of times the hull migration function should attempt to lay out items.");
		}
	}

	private void DebugInfectItem(CommandArg[] args)
	{
		if (args.Length == 2)
		{
			this.InfectItemAtInventoryPosition(args[0].Int, args[1].Int);
			return;
		}
		this.InfectRandomItemInInventory();
	}

	private void DebugAddDamage(CommandArg[] args)
	{
		if (args.Length == 2)
		{
			this.AddDamageToInventory(1, args[0].Int, args[1].Int);
			return;
		}
		this.AddDamageToInventory(1, -1, -1);
	}

	private void DebugSetHullMigrateAttempts(CommandArg[] args)
	{
		this.maxMigrateAttempts = args[0].Int;
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("damage");
		Terminal.Shell.RemoveCommand("infect");
		Terminal.Shell.RemoveCommand("func.hullmigrate.attempts");
	}

	[Header("Configuration")]
	[SerializeField]
	public GameObject gridObjectPrefab;

	[SerializeField]
	public GameObject heldItemContainer;

	[SerializeField]
	private GraphicRaycaster graphicRaycaster;

	[SerializeField]
	private UIController uiController;

	[SerializeField]
	private CursorProxy cursorProxy;

	[SerializeField]
	public float cellSize;

	[SerializeField]
	private LayerMask gridCellLayer;

	[SerializeField]
	private string shipwrightDestinationKey;

	[SerializeField]
	private SpatialItemData darkSplashItemData;

	[SerializeField]
	private VibrationData darkSplashVibrationData;

	[SerializeField]
	private GameObject darkSplashSpawnVFX;

	[SerializeField]
	private Transform darkSplashSpawnVFXContainer;

	private Vector2 currentHeldObjectSelectionOffset;

	private GridCell lastSelectedCell;

	private bool _isDirty = true;

	private GridObject currentlyHeldObject;

	private GridObject pendingHeldObject;

	private GridObject currentlyHoveredObject;

	private GridUI lastActiveGrid;

	private List<GridUI> activeGrids = new List<GridUI>();

	private float timeUntilForceGridCellRepaint;

	private float forceRefreshCellRepaintInterval = 0.2f;

	[SerializeField]
	private CanvasScaler canvasScaler;

	private HashSet<GridUI> gridsShowing = new HashSet<GridUI>();

	private bool shouldTriggerNewCellFocus;

	private GridCell newCellFocus;

	private GridObject lastObject;

	private GridObject newObject;

	private List<RaycastResult> results = new List<RaycastResult>();

	private List<BaseGridModeActionHandler> activeActionHandlers = new List<BaseGridModeActionHandler>();

	public bool suppressNextSelectUpdate;

	private int maxMigrateAttempts = 100;

	private struct FindPositionResult
	{
		public bool didFindPos;

		public Vector3Int position;
	}
}
