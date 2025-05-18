using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public abstract class SellModeActionHandler : BaseGridModeActionHandler
{
	public string _SellPrice { get; set; }

	private DredgePlayerActionBase GetSellAction(GridObject focusedObject)
	{
		if (focusedObject.ItemData.itemType == ItemType.EQUIPMENT)
		{
			return this.sellHoldAction;
		}
		return this.sellAction;
	}

	public SellModeActionHandler()
	{
		this.sellAction = new DredgePlayerActionPress("prompt.sell", GameManager.Instance.Input.Controls.SellItem);
		this.sellAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.sellAction;
		dredgePlayerActionPress.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressEnd, new Action(this.SellFocusedItem));
		this.sellAction.priority = 4;
		this.sellHoldAction = new DredgePlayerActionHold("prompt.sell", GameManager.Instance.Input.Controls.SellItem, 0.75f);
		this.sellHoldAction.showInTooltip = true;
		DredgePlayerActionHold dredgePlayerActionHold = this.sellHoldAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.SellFocusedItem));
		this.sellHoldAction.priority = 4;
		this.sellAllAction = new DredgePlayerActionHold("prompt.sell-all", GameManager.Instance.Input.Controls.SellItem, 0.5f);
		DredgePlayerActionBase dredgePlayerActionBase = this.sellAllAction;
		object[] array = new string[] { this.sellAllValueString };
		dredgePlayerActionBase.LocalizationArguments = array;
		this.sellAllAction.showInControlArea = true;
		DredgePlayerActionHold dredgePlayerActionHold2 = this.sellAllAction;
		dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold2.OnPressComplete, new Action(this.OnSellAllPressed));
		this.sellAllAction.priority = 5;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
	}

	public virtual void SetDestination(BaseDestination destination)
	{
		MarketDestination marketDestination = destination as MarketDestination;
		this.destination = marketDestination;
		this.sellValueModifier = marketDestination.SellValueModifier;
		this.allowSellIfGridFull = (destination as MarketDestination).AllowSellIfGridFull;
		if (marketDestination.AllowBulkSell)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.sellAllAction }, ActionLayer.UI_WINDOW);
			this.sellAllAction.SetPromptString(marketDestination.BulkSellPromptString);
		}
		else
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.sellAllAction }, ActionLayer.UI_WINDOW);
		}
		this.CheckSellAllValidity();
	}

	public override void Shutdown()
	{
		base.Shutdown();
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.sellAction, this.sellHoldAction, this.sellAllAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionPress dredgePlayerActionPress = this.sellAction;
		dredgePlayerActionPress.OnPressEnd = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressEnd, new Action(this.SellFocusedItem));
		DredgePlayerActionHold dredgePlayerActionHold = this.sellHoldAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold.OnPressComplete, new Action(this.SellFocusedItem));
		DredgePlayerActionHold dredgePlayerActionHold2 = this.sellAllAction;
		dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHold2.OnPressComplete, new Action(this.OnSellAllPressed));
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		base.OnItemHoveredChanged(gridObject);
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			this.OnFocusedItemChanged(gridObject);
		}
	}

	protected override void OnItemInventoryChanged(SpatialItemData spatialItemData)
	{
		base.OnItemInventoryChanged(spatialItemData);
		this.CheckSellAllValidity();
	}

	protected override void OnItemPickedUp(GridObject gridObject)
	{
		base.OnItemPickedUp(gridObject);
		if (gridObject.state == GridObjectState.IN_SHOP)
		{
			gridObject.state = GridObjectState.JUST_PURCHASED;
		}
		this.OnFocusedItemChanged(gridObject);
		this.CheckSellAllValidity();
	}

	protected override void OnFocusedGridCellChanged(GridCell gc)
	{
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
		{
			this.OnFocusedItemChanged(GameManager.Instance.GridManager.GetCurrentlyFocusedObject());
		}
	}

	private void OnFocusedItemChanged(GridObject gridObject = null)
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.sellAction, this.sellHoldAction }, ActionLayer.UI_WINDOW);
		if (!gridObject)
		{
			gridObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		}
		if (GameManager.Instance.GridManager.CurrentlyHeldObject && gridObject != GameManager.Instance.GridManager.CurrentlyHeldObject)
		{
			return;
		}
		if (gridObject && this.DoesStoreAcceptThisItem(gridObject.ItemData, false))
		{
			decimal num = 0m;
			if (gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.IN_STORAGE || gridObject.state == GridObjectState.BEING_HARVESTED)
			{
				num = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.SELL, this.sellValueModifier);
				this.sellAction.promptString = "prompt.sell";
				this.sellHoldAction.promptString = "prompt.sell";
			}
			else if (gridObject.state == GridObjectState.JUST_PURCHASED)
			{
				num = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.BUY, 1f);
				this.sellAction.promptString = "prompt.refund";
				this.sellHoldAction.promptString = "prompt.refund";
			}
			else
			{
				if (gridObject.state == GridObjectState.IN_SHOP)
				{
					return;
				}
				Debug.LogWarning(string.Format("[SellModeActionHandler] OnFocusedItemChanged({0}) has a weird state: {1}", gridObject, gridObject.state));
				return;
			}
			string text = num.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
			string text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			this._SellPrice = string.Concat(new string[] { "<color=#", text2, ">$", text, "</color>" });
			object[] array = new object[] { this._SellPrice };
			this.sellAction.LocalizationArguments = array;
			this.sellHoldAction.LocalizationArguments = array;
			this.sellAction.TriggerPromptArgumentsChanged();
			this.sellHoldAction.TriggerPromptArgumentsChanged();
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.GetSellAction(gridObject) }, ActionLayer.UI_WINDOW);
		}
	}

	protected abstract bool DoesStoreAcceptThisItem(SpatialItemData itemData, bool bulkMode);

	protected override void OnItemRemovedFromCursor(GridObject gridObject)
	{
		base.OnItemRemovedFromCursor(gridObject);
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.sellAction, this.sellHoldAction }, ActionLayer.UI_WINDOW);
		this.CheckSellAllValidity();
	}

	protected void CheckSellAllValidity()
	{
		bool flag = true;
		if (GameManager.Instance.GridManager.CurrentlyHeldObject != null && GameManager.Instance.GridManager.CurrentlyHeldObject.state == GridObjectState.JUST_PURCHASED)
		{
			flag = false;
		}
		if (flag)
		{
			List<SpatialItemInstance> bulkSellableItemInstances = this.GetBulkSellableItemInstances();
			decimal sellAllValue = 0m;
			bulkSellableItemInstances.ForEach(delegate(SpatialItemInstance itemInstance)
			{
				sellAllValue += GameManager.Instance.ItemManager.GetItemValue(itemInstance, ItemManager.BuySellMode.SELL, this.sellValueModifier);
			});
			this.sellAllValueString = "$" + sellAllValue.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
			DredgePlayerActionBase dredgePlayerActionBase = this.sellAllAction;
			object[] array = new string[] { this.sellAllValueString };
			dredgePlayerActionBase.LocalizationArguments = array;
			this.sellAllAction.TriggerPromptArgumentsChanged();
			flag = flag && bulkSellableItemInstances.Count > 0;
			if (bulkSellableItemInstances.Count > 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.sellAllAction.Enable();
			return;
		}
		this.sellAllAction.Disable(true);
	}

	private List<SpatialItemInstance> GetBulkSellableItemInstances()
	{
		List<SpatialItemInstance> list = (from i in GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(this.bulkItemTypesBought, this.bulkItemSubtypesBought)
			where i.GetItemData<SpatialItemData>().canBeSoldInBulkAction && this.DoesStoreAcceptThisItem(i.GetItemData<SpatialItemData>(), true)
			select i).ToList<SpatialItemInstance>();
		List<SpatialItemInstance> list2 = (from i in GameManager.Instance.SaveData.TrawlNet.GetAllItemsOfType<SpatialItemInstance>(this.bulkItemTypesBought, this.bulkItemSubtypesBought)
			where i.GetItemData<SpatialItemData>().canBeSoldInBulkAction && this.DoesStoreAcceptThisItem(i.GetItemData<SpatialItemData>(), true)
			select i).ToList<SpatialItemInstance>();
		list.AddRange(list2);
		return list;
	}

	private void SellFocusedItem()
	{
		GridObject currentlyFocusedObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		if (currentlyFocusedObject)
		{
			if (!this.DoesStoreAcceptThisItem(currentlyFocusedObject.ItemData, false))
			{
				return;
			}
			GridKey gridKey = (this.destination as MarketDestination).GetGridKeyForItemType(currentlyFocusedObject.ItemData.itemType, currentlyFocusedObject.ItemData.itemSubtype);
			if (currentlyFocusedObject.ItemData.itemSubtype == ItemSubtype.FISH)
			{
				gridKey = GridKey.NONE;
			}
			if (GameManager.Instance.GridManager.QuickTransferItemToGrid(currentlyFocusedObject, gridKey, this.allowSellIfGridFull))
			{
				bool flag = false;
				decimal num = 0m;
				if (currentlyFocusedObject.state == GridObjectState.JUST_PURCHASED)
				{
					num = GameManager.Instance.ItemManager.GetItemValue(currentlyFocusedObject.SpatialItemInstance, ItemManager.BuySellMode.BUY, 1f);
					flag = true;
				}
				else
				{
					num = GameManager.Instance.ItemManager.GetItemValue(currentlyFocusedObject.SpatialItemInstance, ItemManager.BuySellMode.SELL, this.sellValueModifier);
					if (currentlyFocusedObject.ItemData.itemSubtype == ItemSubtype.FISH)
					{
						GameManager.Instance.SaveData.FishSaleTotal += num;
					}
					else if (currentlyFocusedObject.ItemData.itemSubtype == ItemSubtype.TRINKET)
					{
						GameManager.Instance.SaveData.TrinketSaleTotal += num;
					}
				}
				decimal num2 = 0m;
				decimal num3 = 0m;
				if (this.destination.Id == "destination.gm-fishmonger")
				{
					num3 = this.ProcessDebtRepayment(num, out num2);
				}
				else
				{
					num2 = num;
				}
				GameManager.Instance.UI.PrepareItemNameForSellNotification(NotificationType.MONEY_GAINED, currentlyFocusedObject.ItemData.itemNameKey, num2, num3, flag);
				GameManager.Instance.AddFunds(num2);
				GameManager.Instance.ItemManager.SetItemSeen(currentlyFocusedObject.SpatialItemInstance);
				currentlyFocusedObject.SpatialItemInstance.SetIsOnDamagedCell(false);
				GameManager.Instance.SaveData.RecordShopTransaction(this.destination.Id, GameManager.Instance.Time.Day, flag ? (-num) : num);
				GameManager.Instance.SaveData.RecordItemTransaction(currentlyFocusedObject.ItemData.id, true);
				GameEvents.Instance.TriggerItemSold(currentlyFocusedObject.SpatialItemInstance.GetItemData<SpatialItemData>());
			}
		}
	}

	public void OnSellAllPressed()
	{
		List<SpatialItemInstance> bulkSellableItemInstances = this.GetBulkSellableItemInstances();
		this.sellAction.Reset();
		this.sellHoldAction.Reset();
		this.OnSellAllPressed(bulkSellableItemInstances, (this.destination as MarketDestination).BulkSellNotificationString);
	}

	public void OnSellAllPressed(List<SpatialItemInstance> items, string bulkSellNotificationString)
	{
		GridObject selectedItem = GameManager.Instance.GridManager.CurrentlyHeldObject;
		if (selectedItem && this.DoesStoreAcceptThisItem(selectedItem.ItemData, true))
		{
			items.Add(selectedItem.SpatialItemInstance);
		}
		decimal totalIncome = 0m;
		decimal num = 0m;
		decimal num2 = 0m;
		items.ForEach(delegate(SpatialItemInstance itemInstance)
		{
			ItemSubtype itemSubtype = itemInstance.GetItemData<SpatialItemData>().itemSubtype;
			decimal itemValue = GameManager.Instance.ItemManager.GetItemValue(itemInstance, ItemManager.BuySellMode.SELL, this.sellValueModifier);
			if (itemSubtype == ItemSubtype.FISH)
			{
				GameManager.Instance.SaveData.FishSaleTotal += itemValue;
			}
			else if (itemSubtype == ItemSubtype.TRINKET)
			{
				GameManager.Instance.SaveData.TrinketSaleTotal += itemValue;
			}
			totalIncome += itemValue;
			GameManager.Instance.ItemManager.SetItemSeen(itemInstance);
			itemInstance.SetIsOnDamagedCell(false);
			if (GameManager.Instance.SaveData.TrawlNet.spatialItems.Contains(itemInstance))
			{
				GameManager.Instance.SaveData.TrawlNet.RemoveObjectFromGridData(itemInstance, true);
			}
			else if (selectedItem && itemInstance == selectedItem.SpatialItemInstance)
			{
				GameManager.Instance.GridManager.QuickTransferItemToGrid(selectedItem, GridKey.NONE, this.allowSellIfGridFull);
			}
			else
			{
				GameManager.Instance.SaveData.Inventory.RemoveObjectFromGridData(itemInstance, true);
			}
			GameManager.Instance.SaveData.RecordItemTransaction(itemInstance.id, true);
			GameEvents.Instance.TriggerItemSold(itemInstance.GetItemData<SpatialItemData>());
		});
		GameManager.Instance.SaveData.RecordShopTransaction(this.destination.Id, GameManager.Instance.Time.Day, totalIncome);
		if (this.destination.Id == "destination.gm-fishmonger")
		{
			num2 = this.ProcessDebtRepayment(totalIncome, out num);
		}
		else
		{
			num = totalIncome;
		}
		if (num2 > 0m)
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_GAINED, "notification.sell-fish-bulk-debt", new object[]
			{
				items.Count,
				string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
					">+$",
					num.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
					"</color>"
				}),
				string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
					">-$",
					num2.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
					"</color>"
				})
			});
		}
		else
		{
			GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_GAINED, bulkSellNotificationString, new object[]
			{
				items.Count,
				string.Concat(new string[]
				{
					"<color=#",
					GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE),
					">+$",
					num.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
					"</color>"
				})
			});
		}
		GameManager.Instance.AddFunds(num);
		this.CheckSellAllValidity();
	}

	private decimal ProcessDebtRepayment(decimal income, out decimal remainingIncome)
	{
		remainingIncome = 0m;
		decimal greaterMarrowRepayments = GameManager.Instance.SaveData.GreaterMarrowRepayments;
		decimal num = GameManager.Instance.GameConfigData.GreaterMarrowDebt - greaterMarrowRepayments;
		decimal greaterMarrowDebtRepaymentProportion = GameManager.Instance.GameConfigData.GreaterMarrowDebtRepaymentProportion;
		decimal num2 = 0m;
		if (num > 0m)
		{
			num2 = (decimal)Mathf.Min((float)num, (float)(income * greaterMarrowDebtRepaymentProportion));
			GameManager.Instance.SaveData.GreaterMarrowRepayments += decimal.Round(num2, 2);
		}
		remainingIncome = income - num2;
		return num2;
	}

	protected BaseDestination destination;

	protected DredgePlayerActionPress sellAction;

	protected DredgePlayerActionHold sellHoldAction;

	private DredgePlayerActionHold sellAllAction;

	protected ItemType itemTypesBought;

	protected ItemSubtype itemSubtypesBought;

	protected ItemType bulkItemTypesBought;

	protected ItemSubtype bulkItemSubtypesBought;

	private bool allowBulkAction;

	protected bool allowSellIfGridFull;

	private float sellValueModifier = 1f;

	private string sellAllValueString = "$0.00";
}
