using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class RepairActionHandler : BaseGridModeActionHandler
{
	public static string GetFormattedCostString(decimal cost)
	{
		string text = cost.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		string text2;
		if (GameManager.Instance.SaveData.Funds >= cost)
		{
			text2 = ColorUtility.ToHtmlStringRGB(Color.white);
		}
		else
		{
			text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
		}
		return string.Concat(new string[] { "<color=#", text2, ">$", text, "</color>" });
	}

	public RepairActionHandler()
	{
		this.mode = GridMode.MAINTENANCE;
		this.isOnCargoTab = true;
		this.toggleRepairModeAction = new DredgePlayerActionPress("prompt.enter-repair-mode", GameManager.Instance.Input.Controls.RepairMode);
		this.toggleRepairModeAction.showInControlArea = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.toggleRepairModeAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.ToggleRepairMode));
		this.toggleRepairModeAction.priority = 1;
		this.repairAllAction = new DredgePlayerActionHold("button.repair-all", GameManager.Instance.Input.Controls.RepairAll, 1f);
		this.repairAllAction.showInControlArea = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.repairAllAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.RepairAll));
		this.repairAllAction.priority = 2;
		this.repairAllAction.LocalizationArguments = new object[] { this };
		this.repairAction = new DredgePlayerActionPress("prompt.repair", GameManager.Instance.Input.Controls.RepairItem);
		this.repairAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.repairAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.RepairFocusedItem));
		this.repairAction.priority = 3;
		this.repairAction.LocalizationArguments = new object[] { this };
		GameEvents.Instance.OnItemQuickMoved += this.OnItemQuickMoved;
		GameEvents.Instance.OnPlayerDamageChanged += this.RefreshRepairAllPrompt;
		GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
		GameEvents.Instance.OnItemsRepaired += this.OnItemsRepaired;
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.toggleRepairModeAction, this.repairAllAction }, ActionLayer.UI_WINDOW);
		this.RefreshRepairAllPrompt();
	}

	public override void Shutdown()
	{
		base.Shutdown();
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.toggleRepairModeAction, this.repairAllAction, this.repairAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionPress dredgePlayerActionPress = this.repairAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.RepairFocusedItem));
		GameEvents.Instance.OnItemQuickMoved -= this.OnItemQuickMoved;
		GameEvents.Instance.OnPlayerDamageChanged -= this.RefreshRepairAllPrompt;
		GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
		GameEvents.Instance.OnItemsRepaired -= this.OnItemsRepaired;
	}

	private void OnItemQuickMoved(GridObject gridObject)
	{
		this.RefreshRepairAllPrompt();
	}

	private void OnItemsRepaired()
	{
		this.RefreshRepairAllPrompt();
	}

	public void SetIsOnCargoTab(bool isOnCargoTab)
	{
		this.isOnCargoTab = isOnCargoTab;
		this.UpdateShowRepairMode();
	}

	private void UpdateShowRepairMode()
	{
		if (this.toggleRepairModeAction != null)
		{
			if (this.isOnCargoTab && GameManager.Instance.GridManager.CurrentlyHeldObject == null)
			{
				GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.toggleRepairModeAction }, ActionLayer.UI_WINDOW);
				return;
			}
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.toggleRepairModeAction }, ActionLayer.UI_WINDOW);
		}
	}

	private void ToggleRepairMode()
	{
		GameManager.Instance.GridManager.ToggleRepairMode();
		if (GameManager.Instance.GridManager.IsInRepairMode)
		{
			this.toggleRepairModeAction.SetPromptString("prompt.exit-repair-mode");
		}
		else
		{
			this.toggleRepairModeAction.SetPromptString("prompt.enter-repair-mode");
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.repairAction }, ActionLayer.UI_WINDOW);
		}
		GameManager.Instance.GridManager.RefreshCurrentlySelectedCell();
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		if (gridObject == null)
		{
			return;
		}
		if (!GameManager.Instance.GridManager.IsInRepairMode)
		{
			return;
		}
		bool flag = false;
		if (!GameManager.Instance.GridManager.CurrentlyHeldObject)
		{
			if (gridObject.ItemData.itemType == ItemType.DAMAGE)
			{
				this.repairCost = GameManager.Instance.GameConfigData.HullRepairCostPerSquare;
				flag = true;
			}
			else if (gridObject.ItemData.damageMode == DamageMode.DURABILITY)
			{
				float missingDurabilityAmount = gridObject.SpatialItemInstance.GetMissingDurabilityAmount();
				if (missingDurabilityAmount > 0f)
				{
					if (gridObject.ItemData.itemSubtype == ItemSubtype.POT)
					{
						this.repairCost = GameManager.Instance.GameConfigData.PotRepairCostPerDay * (decimal)missingDurabilityAmount;
					}
					else if (gridObject.ItemData.itemSubtype == ItemSubtype.NET)
					{
						this.repairCost = GameManager.Instance.GameConfigData.NetRepairCostPerDay * (decimal)missingDurabilityAmount;
					}
					flag = true;
				}
			}
		}
		if (!flag)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.repairAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.repairAction }, ActionLayer.UI_WINDOW);
		this.repairAction.LocalizationArguments = new object[] { RepairActionHandler.GetFormattedCostString(this.repairCost) };
		this.repairAction.TriggerPromptArgumentsChanged();
		if (GameManager.Instance.SaveData.Funds >= this.repairCost)
		{
			this.repairAction.Enable();
			return;
		}
		this.repairAction.Disable(false);
	}

	private void RepairFocusedItem()
	{
		GridCell lastSelectedCell = GameManager.Instance.GridManager.LastSelectedCell;
		if (lastSelectedCell && lastSelectedCell.OccupyingUnderlayObject && lastSelectedCell.OccupyingUnderlayObject.ItemData.itemType == ItemType.DAMAGE)
		{
			if (GameManager.Instance.SaveData.Funds >= this.repairCost)
			{
				GameManager.Instance.AddFunds(-this.repairCost);
				lastSelectedCell.ParentGrid.linkedGrid.RemoveObjectFromGridData(lastSelectedCell.OccupyingUnderlayObject.SpatialItemInstance, true);
				GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.repairAction }, ActionLayer.UI_WINDOW);
				GameEvents.Instance.TriggerItemHoveredChanged(lastSelectedCell.OccupyingObject);
				GameEvents.Instance.TriggerOnPlayerDamageChanged();
			}
		}
		else if (lastSelectedCell && lastSelectedCell.OccupyingObject && lastSelectedCell.OccupyingObject.ItemData.damageMode == DamageMode.DURABILITY && GameManager.Instance.SaveData.Funds >= this.repairCost)
		{
			GameManager.Instance.AddFunds(-this.repairCost);
			lastSelectedCell.OccupyingObject.SpatialItemInstance.RepairToFullDurability();
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.repairAction }, ActionLayer.UI_WINDOW);
			GameEvents.Instance.TriggerItemInventoryChanged(lastSelectedCell.OccupyingObject.ItemData);
			GameEvents.Instance.TriggerItemHoveredChanged(lastSelectedCell.OccupyingObject);
			GameEvents.Instance.TriggerItemsRepaired();
		}
		this.RefreshRepairAllPrompt();
	}

	private void OnPlayerFundsChanged(decimal total, decimal change)
	{
		this.RefreshRepairAllPrompt();
	}

	private void RefreshRepairAllPrompt()
	{
		this.repairAllCost = GameManager.Instance.ItemManager.GetRepairAllCost();
		this.repairAllAction.LocalizationArguments = new object[] { RepairActionHandler.GetFormattedCostString(this.repairAllCost) };
		this.repairAllAction.TriggerPromptArgumentsChanged();
		if (this.repairAllCost > 0m && GameManager.Instance.SaveData.Funds >= this.repairAllCost)
		{
			this.repairAllAction.Enable();
			return;
		}
		this.repairAllAction.Disable(true);
	}

	private void RepairAll()
	{
		GameManager.Instance.ItemManager.RepairHullDamage(false);
		GameManager.Instance.ItemManager.RepairAllItemDurability();
	}

	protected override void OnItemPickedUp(GridObject gridObject)
	{
		base.OnItemPickedUp(gridObject);
		this.UpdateShowRepairMode();
	}

	protected override void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		base.OnItemPlaceComplete(gridObject, result);
		if (result)
		{
			this.RefreshRepairAllPrompt();
			this.UpdateShowRepairMode();
		}
	}

	protected override void OnItemRemovedFromCursor(GridObject gridObject)
	{
		base.OnItemRemovedFromCursor(gridObject);
		this.UpdateShowRepairMode();
	}

	protected DredgePlayerActionPress toggleRepairModeAction;

	protected DredgePlayerActionPress repairAction;

	protected DredgePlayerActionHold repairAllAction;

	private decimal repairCost;

	private decimal repairAllCost;

	private bool isOnCargoTab;
}
