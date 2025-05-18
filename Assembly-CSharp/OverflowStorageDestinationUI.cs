using System;
using UnityEngine;

public class OverflowStorageDestinationUI : BaseDestinationUI
{
	protected override void ShowMainUI()
	{
		base.ShowMainUI();
		GameManager.Instance.UI.PlayerTabbedPanel.RequestShowablePanels(this.destination.PlayerInventoryTabIndexesToShow);
		this.inventorySlidePanel.Toggle(true, false);
		this.overflowStorageSlidePanel.Toggle(true, false);
	}

	protected override void RefreshShowLeaveAction()
	{
		base.RefreshShowLeaveAction();
	}

	protected override void ConfigureActionHandlers()
	{
		base.ConfigureActionHandlers();
		if (GameManager.Instance.GridManager.IsInRepairMode)
		{
			return;
		}
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.EQUIPMENT);
		GameManager.Instance.Player.CanMoveInstalledItems = true;
	}

	protected override void OnLeavePressComplete()
	{
		this.inventorySlidePanel.Toggle(false, false);
		this.overflowStorageSlidePanel.Toggle(false, false);
		GameManager.Instance.Player.CanMoveInstalledItems = false;
		base.OnLeavePressComplete();
	}

	[SerializeField]
	private SlidePanel inventorySlidePanel;

	[SerializeField]
	private SlidePanel overflowStorageSlidePanel;
}
