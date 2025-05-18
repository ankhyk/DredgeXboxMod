using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipRequester : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
	public void OnSelect(BaseEventData eventData)
	{
		ApplicationEvents.Instance.TriggerUITooltipRequested(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ApplicationEvents.Instance.TriggerUITooltipRequested(this);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		ApplicationEvents.Instance.TriggerUITooltipClearRequested(this, false);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!eventData.fullyExited)
		{
			return;
		}
		ApplicationEvents.Instance.TriggerUITooltipClearRequested(this, false);
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.TriggerUITooltipClearRequested(this, false);
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.TriggerUITooltipClearRequested(this, false);
	}
}
