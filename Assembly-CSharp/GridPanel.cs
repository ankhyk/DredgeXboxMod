using System;
using UnityEngine;

public class GridPanel : TabbedPanel
{
	public GridUI GridUI
	{
		get
		{
			return this.grid;
		}
		set
		{
			this.grid = value;
		}
	}

	public override void ShowStart()
	{
		base.ShowStart();
		GameManager.Instance.GridManager.ClearLastSelectedCell();
	}

	public override void ShowFinish()
	{
		GameManager.Instance.GridManager.suppressNextSelectUpdate = GameManager.Instance.UI.IsHarvesting;
		GameManager.Instance.GridManager.CursorProxy.UpdateShouldShowCursor();
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		this.grid.EnableInput();
		if (this.selectFirstCellOnShow)
		{
			this.grid.SelectFirstPlaceableCell();
		}
		GameManager.Instance.GridManager.TrySelectCellUnderCursor();
		base.ShowFinish();
	}

	public override void HideStart()
	{
		this.grid.DisableInput();
		GameManager.Instance.GridManager.CursorProxy.Hide();
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, false);
		base.HideStart();
	}

	public override void HideFinish()
	{
		base.HideFinish();
	}

	public override void SwitchToSide()
	{
		this.grid.SelectFirstPlaceableCell();
	}

	[SerializeField]
	private GridUI grid;

	[SerializeField]
	private UIWindowType windowType;

	[SerializeField]
	private bool selectFirstCellOnShow;
}
