using System;
using UnityEngine;
using UnityEngine.Events;

public class GameItemPickupListener : MonoBehaviour
{
	public void Subscribe()
	{
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUpCallback;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursorCallback;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceCompleteCallback;
	}

	public void Unsubscribe()
	{
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUpCallback;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursorCallback;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceCompleteCallback;
	}

	private void OnItemPickedUpCallback(GridObject go)
	{
		UnityEvent onItemPickedUp = this.OnItemPickedUp;
		if (onItemPickedUp == null)
		{
			return;
		}
		onItemPickedUp.Invoke();
	}

	private void OnItemRemovedFromCursorCallback(GridObject go)
	{
		UnityEvent onItemReleased = this.OnItemReleased;
		if (onItemReleased == null)
		{
			return;
		}
		onItemReleased.Invoke();
	}

	private void OnItemPlaceCompleteCallback(GridObject go, bool success)
	{
		if (success)
		{
			UnityEvent onItemRemovedFromCursor = this.OnItemRemovedFromCursor;
			if (onItemRemovedFromCursor == null)
			{
				return;
			}
			onItemRemovedFromCursor.Invoke();
		}
	}

	public UnityEvent OnItemPickedUp;

	public UnityEvent OnItemReleased;

	public UnityEvent OnItemRemovedFromCursor;
}
