using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSelectable : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	private void OnEnable()
	{
		if (this.subscribeToItemEvents && GameEvents.Instance)
		{
			GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
			GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
			if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject())
			{
				this.selectable.interactable = true;
			}
		}
	}

	private void OnDisable()
	{
		if (this.subscribeToItemEvents && GameEvents.Instance)
		{
			GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
			GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
			GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		}
	}

	private void OnItemPickedUp(GridObject gridObject)
	{
		this.selectable.interactable = false;
	}

	private void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.selectable.interactable = true;
	}

	private void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		if (result)
		{
			this.selectable.interactable = true;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (GameManager.Instance.GridManager)
		{
			GameManager.Instance.GridManager.OnSelectUpdate(null);
			GameManager.Instance.GridManager.CursorProxy.Hide();
		}
	}

	[SerializeField]
	private Selectable selectable;

	[SerializeField]
	private bool subscribeToItemEvents = true;
}
