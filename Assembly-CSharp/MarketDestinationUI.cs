using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;

public class MarketDestinationUI : BaseDestinationUI
{
	protected override void ShowMainUI()
	{
		base.ShowMainUI();
		bool flag = true;
		List<MarketTabConfig> marketTabs = (this.destination as MarketDestination).marketTabs;
		if (this.marketTabbedPanel == null)
		{
			this.marketGridUI.LinkedGridKey = GridKey.NONE;
			if (marketTabs[0].isUnlockedBasedOnDialogue)
			{
				flag = marketTabs[0].unlockDialogueNodes.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
			}
			if (flag)
			{
				this.marketGridUI.LinkedGridKey = marketTabs[0].gridKey;
			}
		}
		else
		{
			List<int> list = new List<int>();
			this.marketGridUI.LinkedGridKey = GridKey.NONE;
			for (int i = 0; i < marketTabs.Count; i++)
			{
				if (marketTabs[i].isUnlockedBasedOnDialogue)
				{
					flag = marketTabs[i].unlockDialogueNodes.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s));
				}
				if (flag)
				{
					if (this.marketGridUI.LinkedGridKey == GridKey.NONE)
					{
						this.marketGridUI.LinkedGridKey = marketTabs[i].gridKey;
					}
					this.marketTabbedPanel.SetTabImage(i, marketTabs[i].tabSprite);
					list.Add(i);
				}
			}
			this.marketTabbedPanel.RequestShowablePanels(list);
			TabbedPanelContainer tabbedPanelContainer = this.marketTabbedPanel;
			tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Combine(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnMarketTabChanged));
		}
		GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(this.destination.PlayerInventoryTabIndexesToShow);
		TabbedPanelContainer playerTabbedPanel = GameManager.Instance.UI.PlayerTabbedPanel;
		playerTabbedPanel.OnTabChanged = (Action<int>)Delegate.Combine(playerTabbedPanel.OnTabChanged, new Action<int>(this.OnPlayerTabChanged));
		this.inventorySlidePanel.Toggle(true, false);
		if (this.marketGridUI.LinkedGridKey != GridKey.NONE)
		{
			this.marketSlidePanel.Toggle(true, false);
		}
	}

	protected override void OnMaintenanceModeToggled(bool isActive)
	{
		if (isActive)
		{
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.DEFAULT);
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.EQUIPMENT);
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.STORAGE);
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.BUY);
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.SELL_SPECIFIC);
			GameManager.Instance.GridManager.RemoveGridActionHander(GridMode.SELL_TYPE);
			return;
		}
		this.ConfigureActionHandlers();
	}

	protected override void ConfigureActionHandlers()
	{
		base.ConfigureActionHandlers();
		if ((this.destination as MarketDestination).AllowRepairs && GameManager.Instance.GridManager.GetActionHandlerOfType<RepairActionHandler>() == null)
		{
			this.repairActionHandler = GameManager.Instance.GridManager.AddGridActionHandler(GridMode.MAINTENANCE) as RepairActionHandler;
		}
		if (GameManager.Instance.GridManager.IsInRepairMode)
		{
			return;
		}
		if (this.localizedTitleString != null && !this.destination.TitleKey.IsEmpty)
		{
			this.localizedTitleString.StringReference = this.destination.TitleKey;
		}
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.EQUIPMENT);
		if ((this.destination as MarketDestination).AllowStorageAccess)
		{
			GameManager.Instance.GridManager.AddGridActionHandler(GridMode.STORAGE);
		}
		GameManager.Instance.Player.CanMoveInstalledItems = true;
		(GameManager.Instance.GridManager.AddGridActionHandler(GridMode.BUY) as BuyModeActionHandler).SetDestination(this.destination);
		SellModeActionHandler sellModeActionHandler;
		if ((this.destination as MarketDestination).SpecificItemsBought.Length != 0)
		{
			sellModeActionHandler = GameManager.Instance.GridManager.AddGridActionHandler(GridMode.SELL_SPECIFIC) as SpecificSellModeActionHandler;
		}
		else
		{
			sellModeActionHandler = GameManager.Instance.GridManager.AddGridActionHandler(GridMode.SELL_TYPE) as TypeSellModeActionHandler;
		}
		sellModeActionHandler.SetDestination(this.destination);
	}

	protected override void OnLeavePressComplete()
	{
		this.inventorySlidePanel.Toggle(false, false);
		this.marketSlidePanel.Toggle(false, false);
		if (this.marketTabbedPanel != null)
		{
			TabbedPanelContainer tabbedPanelContainer = this.marketTabbedPanel;
			tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Remove(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnMarketTabChanged));
		}
		TabbedPanelContainer playerTabbedPanel = GameManager.Instance.UI.PlayerTabbedPanel;
		playerTabbedPanel.OnTabChanged = (Action<int>)Delegate.Remove(playerTabbedPanel.OnTabChanged, new Action<int>(this.OnPlayerTabChanged));
		this.repairActionHandler = null;
		GameManager.Instance.Player.CanMoveInstalledItems = false;
		base.OnLeavePressComplete();
	}

	private void OnPlayerTabChanged(int tabIndex)
	{
		if (this.repairActionHandler != null)
		{
			this.repairActionHandler.SetIsOnCargoTab(tabIndex == 1);
		}
	}

	private void OnMarketTabChanged(int tabIndex)
	{
		this.marketGridUI.LinkedGridKey = (this.destination as MarketDestination).marketTabs[tabIndex].gridKey;
		this.localizedTitleString.StringReference = (this.destination as MarketDestination).marketTabs[tabIndex].titleKey;
	}

	[SerializeField]
	private SlidePanel inventorySlidePanel;

	[SerializeField]
	private SlidePanel marketSlidePanel;

	[SerializeField]
	private TabbedPanelContainer marketTabbedPanel;

	[SerializeField]
	private GridUI marketGridUI;

	[SerializeField]
	private LocalizeStringEvent localizedTitleString;

	private RepairActionHandler repairActionHandler;
}
