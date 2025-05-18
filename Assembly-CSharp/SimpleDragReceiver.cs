using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleDragReceiver : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public bool IsDragging
	{
		get
		{
			return this.isDragging;
		}
	}

	public Vector2 PrevMousePosition
	{
		get
		{
			return this.prevMousePosition;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		this.isDragging = true;
		this.prevMousePosition = Input.mousePosition;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		this.isDragging = false;
	}

	private void LateUpdate()
	{
		this.prevMousePosition = Input.mousePosition;
	}

	private bool isDragging;

	private Vector2 prevMousePosition;
}
