using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BuyModeActionHandler : BaseGridModeActionHandler
{
	public string _BuyPrice { get; set; }

	public BuyModeActionHandler()
	{
		this.mode = GridMode.BUY;
		this.buyAction = new DredgePlayerActionPress("prompt.buy", GameManager.Instance.Input.Controls.BuyItem);
		this.buyAction.showInTooltip = true;
		DredgePlayerActionPress dredgePlayerActionPress = this.buyAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.BuyFocusedItem));
		this.buyAction.priority = 3;
	}

	public void SetDestination(BaseDestination destination)
	{
		this.destination = destination;
	}

	public override void Shutdown()
	{
		base.Shutdown();
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.buyAction }, ActionLayer.UI_WINDOW);
		DredgePlayerActionPress dredgePlayerActionPress = this.buyAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.BuyFocusedItem));
	}

	protected override void OnItemHoveredChanged(GridObject gridObject)
	{
		base.OnItemHoveredChanged(gridObject);
		bool flag = false;
		if (!GameManager.Instance.GridManager.CurrentlyHeldObject && gridObject && gridObject.state == GridObjectState.IN_SHOP)
		{
			flag = true;
		}
		if (!flag)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.buyAction }, ActionLayer.UI_WINDOW);
			return;
		}
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.buyAction }, ActionLayer.UI_WINDOW);
		decimal itemValue = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.BUY, 1f);
		bool flag2 = GameManager.Instance.SaveData.Funds >= itemValue;
		string text = itemValue.ToString("n2", LocalizationSettings.SelectedLocale.Formatter);
		string text2 = ColorUtility.ToHtmlStringRGB(flag2 ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE));
		this._BuyPrice = string.Concat(new string[] { "<color=#", text2, ">$", text, "</color>" });
		this.buyAction.LocalizationArguments = new object[] { this._BuyPrice };
		this.buyAction.TriggerPromptArgumentsChanged();
		if (flag2)
		{
			this.buyAction.Enable();
			return;
		}
		this.buyAction.Disable(false);
	}

	private void BuyFocusedItem()
	{
		GridObject currentlyFocusedObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
		if (currentlyFocusedObject)
		{
			decimal itemValue = GameManager.Instance.ItemManager.GetItemValue(currentlyFocusedObject.SpatialItemInstance, ItemManager.BuySellMode.BUY, 1f);
			if (GameManager.Instance.SaveData.Funds >= itemValue)
			{
				GameManager.Instance.AddFunds(-itemValue);
				GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.buyAction }, ActionLayer.UI_WINDOW);
				Action onPickUpPressedCallback = this.OnPickUpPressedCallback;
				if (onPickUpPressedCallback != null)
				{
					onPickUpPressedCallback();
				}
				GameManager.Instance.UI.InventoryGrid.SelectMostSuitableCellForObjectPlacement(currentlyFocusedObject.SpatialItemInstance);
				GameEvents.Instance.TriggerItemPurchased(currentlyFocusedObject.ItemData);
				GameManager.Instance.ItemManager.SetItemSeen(currentlyFocusedObject.SpatialItemInstance);
				GameManager.Instance.SaveData.RecordShopTransaction(this.destination.Id, GameManager.Instance.Time.Day, itemValue);
				GameManager.Instance.SaveData.RecordItemTransaction(currentlyFocusedObject.ItemData.id, false);
			}
		}
	}

	protected BaseDestination destination;

	protected DredgePlayerActionPress buyAction;
}
