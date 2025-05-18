using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Harvester : MonoBehaviour
{
	public HarvestPOI CurrentHarvestPOI
	{
		get
		{
			return this.currentHarvestPOI;
		}
		set
		{
			this.currentHarvestPOI = value;
		}
	}

	public bool IsAtrophyMode { get; set; }

	public bool IsCrabBaitMode { get; set; }

	private void Awake()
	{
		this.leaveAction = new DredgePlayerActionPress("prompt.leave", GameManager.Instance.Input.Controls.Back);
		this.leaveAction.showInControlArea = true;
		this.leaveAction.allowPreholding = false;
		this.leaveAction.priority = 1;
		DredgePlayerActionPress dredgePlayerActionPress = this.leaveAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnLeaveActionPressed));
		this.harvestMinigameView = GameManager.Instance.UI.HarvestMinigameView;
		base.enabled = false;
	}

	public void RequestExit()
	{
		this.OnLeaveActionPressed();
	}

	private void OnLeaveActionPressed()
	{
		if (GameManager.Instance.GridManager.CurrentlyHeldObject == null && GameManager.Instance.UI.InventorySlidePanel.IsShowing)
		{
			base.enabled = false;
			GameEvents.Instance.ToggleActivelyHarvesting(false);
		}
	}

	private void OnEnable()
	{
		GameManager.Instance.GridManager.AddGridActionHandler(GridMode.DEFAULT);
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameManager.Instance.UI.IsHarvesting = true;
		GameManager.Instance.UI.CheckInventoryPanelTabShouldShow();
		if (this.IsAtrophyMode)
		{
			this.atrophyVCam.enabled = true;
		}
		else
		{
			this.harvestVCam.enabled = true;
		}
		GameManager.Instance.UI.InventorySlidePanel.OnShowFinish.AddListener(new UnityAction(this.OnInventoryPanelOpenFinish));
		GameEvents.Instance.ToggleHarvestMode(true);
		GameManager.Instance.UI.ToggleInventorySolo(true);
		if (this.IsAtrophyMode)
		{
			this.ShowQuickHarvestGrid(QuickHarvestGridPanel.QuickHarvestGridMode.ATROPHY);
			return;
		}
		if (this.IsCrabBaitMode)
		{
			this.ShowQuickHarvestGrid(QuickHarvestGridPanel.QuickHarvestGridMode.CRAB_BAIT);
			return;
		}
		if (this.currentHarvestPOI && this.currentHarvestPOI.IsCrabPotPOI)
		{
			this.ShowHarvestGrid();
			return;
		}
		if (this.currentHarvestPOI)
		{
			this.harvestMinigameView.Show(this.currentHarvestPOI);
		}
	}

	private void OnInventoryPanelOpenFinish()
	{
		GameManager.Instance.UI.InventorySlidePanel.OnShowFinish.RemoveListener(new UnityAction(this.OnInventoryPanelOpenFinish));
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.leaveAction }, ActionLayer.UI_WINDOW);
	}

	private void ShowHarvestGrid()
	{
		GameManager.Instance.UI.OccasionalGridPanel.Show(this.currentHarvestPOI);
	}

	private void HideHarvestGrid()
	{
		GameManager.Instance.UI.OccasionalGridPanel.Hide();
	}

	private void ShowQuickHarvestGrid(QuickHarvestGridPanel.QuickHarvestGridMode mode)
	{
		GameManager.Instance.UI.QuickHarvestGridPanel.Show(this.currentHarvestPOI, mode);
	}

	private void HideQuickHarvestGrid()
	{
		GameManager.Instance.UI.QuickHarvestGridPanel.Hide();
	}

	private void OnDisable()
	{
		GameManager.Instance.GridManager.ClearActionHandlers();
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameManager.Instance.UI.IsHarvesting = false;
		GameManager.Instance.UI.CheckInventoryPanelTabShouldShow();
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.leaveAction }, ActionLayer.UI_WINDOW);
		if (this.IsAtrophyMode || this.IsCrabBaitMode)
		{
			this.HideQuickHarvestGrid();
			this.IsAtrophyMode = false;
			this.IsCrabBaitMode = false;
		}
		else if (this.currentHarvestPOI && this.currentHarvestPOI.IsCrabPotPOI)
		{
			this.HideHarvestGrid();
		}
		else if (this.currentHarvestPOI)
		{
			this.harvestMinigameView.Hide();
		}
		this.harvestVCam.enabled = false;
		this.atrophyVCam.enabled = false;
		GameEvents.Instance.ToggleHarvestMode(false);
		GameManager.Instance.UI.ToggleInventorySolo(false);
	}

	private void OnItemPickedUp(GridObject o)
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.leaveAction }, ActionLayer.UI_WINDOW);
	}

	private void OnItemRemovedFromCursor(GridObject o)
	{
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.leaveAction }, ActionLayer.UI_WINDOW);
	}

	private void OnItemPlaceComplete(GridObject o, bool result)
	{
		GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.leaveAction }, ActionLayer.UI_WINDOW);
	}

	[SerializeField]
	private CinemachineFreeLook playerVCam;

	[SerializeField]
	private CinemachineClearShot harvestVCam;

	[SerializeField]
	private CinemachineVirtualCamera atrophyVCam;

	[SerializeField]
	private HarvestZoneDetector harvestZoneDetector;

	private HarvestMinigameView harvestMinigameView;

	private DredgePlayerActionPress leaveAction;

	private HarvestPOI currentHarvestPOI;
}
