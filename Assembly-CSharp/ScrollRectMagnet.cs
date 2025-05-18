using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectMagnet : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public void OnSelect(BaseEventData eventData)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			this.ScrollToThis();
		}
	}

	public void ScrollToThis()
	{
		this.scrollRect.content.anchoredPosition = this.scrollRect.GetSnapToPositionToBringChildIntoView(base.gameObject.transform as RectTransform);
	}

	[SerializeField]
	private ScrollRect scrollRect;
}
