using System;
using System.Collections;
using UnityEngine;

public class ResearchCoroutine : MonoBehaviour
{
	private void OnEnable()
	{
		this.coroutine = base.StartCoroutine(this.ResearchLoop(this.secondsBetweenChecks));
	}

	private void OnDisable()
	{
		if (this.coroutine != null)
		{
			base.StopCoroutine(this.coroutine);
			this.coroutine = null;
		}
	}

	private IEnumerator ResearchLoop(float secondsBetweenUpdates)
	{
		while (GameManager.Instance.IsPlaying)
		{
			float prevGameTime = GameManager.Instance.Time.TimeAndDay;
			this.cachedTimePassageMode = GameManager.Instance.Time.CurrentTimePassageMode;
			yield return new WaitForSeconds(secondsBetweenUpdates);
			if (this.cachedTimePassageMode == TimePassageMode.NONE && GameManager.Instance.Time.CurrentTimePassageMode == TimePassageMode.NONE)
			{
				this.AdjustResearchProgress(GameManager.Instance.Time.TimeAndDay - prevGameTime);
			}
		}
		yield break;
	}

	private void AdjustResearchProgress(float proportionOfDayJustElapsed)
	{
		if (proportionOfDayJustElapsed > 0f)
		{
			ResearchableItemInstance activeResearchItem = GameManager.Instance.SaveData.GetActiveResearchItem();
			if (activeResearchItem == null)
			{
				return;
			}
			int num = ((activeResearchItem.progress >= 1f) ? 1 : 0);
			ResearchableItemData itemData = activeResearchItem.GetItemData<ResearchableItemData>();
			float num2 = proportionOfDayJustElapsed / itemData.daysToResearch;
			activeResearchItem.progress += num2;
			activeResearchItem.progress = Mathf.Clamp01(activeResearchItem.progress);
			bool flag = activeResearchItem.progress >= 1f;
			GameEvents.Instance.TriggerBookReadProgressed(activeResearchItem);
			if (num == 0 && flag)
			{
				activeResearchItem.isActive = false;
				GameEvents.Instance.TriggerBookReadCompleted(activeResearchItem);
			}
		}
	}

	[SerializeField]
	private float secondsBetweenChecks;

	private Coroutine coroutine;

	private TimePassageMode cachedTimePassageMode;
}
