using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsUIComponentEventNotifier : MonoBehaviour, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Action onComponentSubmitted = this.OnComponentSubmitted;
		if (onComponentSubmitted == null)
		{
			return;
		}
		onComponentSubmitted();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		Action onComponentSubmitted = this.OnComponentSubmitted;
		if (onComponentSubmitted == null)
		{
			return;
		}
		onComponentSubmitted();
	}

	public void OnSelect(BaseEventData eventData)
	{
		Action<SettingsUIComponentEventNotifier> onComponentSelected = this.OnComponentSelected;
		if (onComponentSelected == null)
		{
			return;
		}
		onComponentSelected(this);
	}

	[HideInInspector]
	public Action OnComponentSubmitted;

	[HideInInspector]
	public Action<SettingsUIComponentEventNotifier> OnComponentSelected;
}
