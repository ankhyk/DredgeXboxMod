using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridObjectCell : MonoBehaviour
{
	public GridObject ParentObject
	{
		get
		{
			return this.parentObject;
		}
		set
		{
			this.parentObject = value;
		}
	}

	public void DoHitTest(out GridCell cellHit, out GridObject objectHit)
	{
		cellHit = null;
		objectHit = null;
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = base.transform.position;
		List<RaycastResult> list = new List<RaycastResult>();
		GameManager.Instance.GridManager.GraphicRaycaster.Raycast(pointerEventData, list);
		foreach (RaycastResult raycastResult in list)
		{
			if (this.slotLayer.Contains(raycastResult.gameObject.layer))
			{
				GridCell component = raycastResult.gameObject.GetComponent<GridCell>();
				if (component)
				{
					cellHit = component;
					if (component.OccupyingObject)
					{
						objectHit = component.OccupyingObject;
					}
				}
			}
		}
	}

	[SerializeField]
	private LayerMask slotLayer;

	private GridObject parentObject;
}
