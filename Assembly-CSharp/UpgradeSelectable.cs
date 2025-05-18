using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSelectable : UISelectable
{
	public UpgradeData upgradeData { get; set; }

	public bool CanBeUpgraded { get; set; }

	public override void OnSelect(BaseEventData eventData)
	{
		if (this.clearLastGridCell)
		{
			GameManager.Instance.GridManager.ClearLastSelectedCell();
		}
		base.OnSelect(eventData);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (this.clearLastGridCell)
		{
			GameManager.Instance.GridManager.ClearLastSelectedCell();
		}
		base.OnPointerEnter(eventData);
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		base.OnPointerExit(eventData);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	[SerializeField]
	private bool clearLastGridCell;
}
