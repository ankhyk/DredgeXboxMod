using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScrollview : MonoBehaviour
{
	private void OnEnable()
	{
		base.GetComponentsInChildren<SettingsUIComponentEventNotifier>().ToList<SettingsUIComponentEventNotifier>().ForEach(delegate(SettingsUIComponentEventNotifier d)
		{
			d.OnComponentSelected = (Action<SettingsUIComponentEventNotifier>)Delegate.Combine(d.OnComponentSelected, new Action<SettingsUIComponentEventNotifier>(this.OnComponentSelected));
		});
	}

	private void OnDisable()
	{
		base.GetComponentsInChildren<SettingsUIComponentEventNotifier>(true).ToList<SettingsUIComponentEventNotifier>().ForEach(delegate(SettingsUIComponentEventNotifier d)
		{
			d.OnComponentSelected = (Action<SettingsUIComponentEventNotifier>)Delegate.Remove(d.OnComponentSelected, new Action<SettingsUIComponentEventNotifier>(this.OnComponentSelected));
		});
	}

	private IEnumerator LerpToDestinationPos(Vector2 destinationPos)
	{
		Canvas.ForceUpdateCanvases();
		this.isLerpingToDestinationPos = true;
		float scrollTime = 0f;
		float maxScrollTimeSec = 0.15f;
		float distanceThreshold = 10f;
		while (this.isLerpingToDestinationPos)
		{
			scrollTime += Time.unscaledDeltaTime;
			float num = Mathf.Min(10f * Time.unscaledDeltaTime, 1f);
			this.scrollRect.content.anchoredPosition = Vector2.Lerp(this.scrollRect.content.anchoredPosition, destinationPos, num);
			if (scrollTime >= maxScrollTimeSec || Vector2.SqrMagnitude(this.scrollRect.content.anchoredPosition - destinationPos) < distanceThreshold)
			{
				this.scrollRect.content.anchoredPosition = destinationPos;
				this.isLerpingToDestinationPos = false;
			}
			yield return null;
		}
		this.lerpCoroutine = null;
		yield break;
	}

	private void OnComponentSelected(SettingsUIComponentEventNotifier d)
	{
		this.OnEntrySelected(d.transform as RectTransform);
	}

	private void OnEntrySelected(RectTransform rt)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			if (this.lerpCoroutine != null)
			{
				base.StopCoroutine(this.lerpCoroutine);
			}
			Vector2 snapToPositionToBringChildIntoView = this.scrollRect.GetSnapToPositionToBringChildIntoView(rt);
			this.lerpCoroutine = base.StartCoroutine(this.LerpToDestinationPos(snapToPositionToBringChildIntoView));
		}
	}

	[SerializeField]
	private ScrollRect scrollRect;

	private bool isLerpingToDestinationPos;

	private Coroutine lerpCoroutine;
}
