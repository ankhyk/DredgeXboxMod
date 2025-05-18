using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CabinItemContainer : MonoBehaviour
{
	private void OnEnable()
	{
		this.PopulateGrid();
		this.itemEntryContainer.position = new Vector2(this.itemEntryContainer.position.x, this.itemEntryContainer.sizeDelta.y * -0.5f);
		GameEvents.Instance.OnBookReadProgressed += this.OnResearchProgressed;
		GameEvents.Instance.OnBookReadCompleted += this.OnResearchCompleted;
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnBookReadProgressed -= this.OnResearchProgressed;
		GameEvents.Instance.OnBookReadCompleted -= this.OnResearchCompleted;
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		this.Clear();
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		if (this.lerpCoroutine != null)
		{
			base.StopCoroutine(this.lerpCoroutine);
			this.lerpCoroutine = null;
		}
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			this.SelectFirstGridEntry();
		}
	}

	private void OnResearchProgressed(ResearchableItemInstance researchableItemInstance)
	{
		this.nonSpatialGridEntries.ForEach(delegate(NonSpatialGridEntryUI ui)
		{
			if (ui.ItemInstance == researchableItemInstance)
			{
				ui.RefreshUI();
			}
		});
	}

	private void OnResearchCompleted(ResearchableItemInstance researchableItemInstance)
	{
		this.nonSpatialGridEntries.ForEach(delegate(NonSpatialGridEntryUI ui)
		{
			if (ui.ItemInstance == researchableItemInstance)
			{
				ui.RefreshUI();
			}
		});
	}

	private void PopulateGrid()
	{
		this.Clear();
		List<NonSpatialItemInstance> list = (from x in GameManager.Instance.SaveData.ownedNonSpatialItems
			orderby x is ResearchableItemInstance && (x as ResearchableItemInstance).isActive descending, x.isNew descending
			select x).ThenByDescending(delegate(NonSpatialItemInstance x)
		{
			if (!(x is ResearchableItemInstance))
			{
				return 0f;
			}
			if ((x as ResearchableItemInstance).progress >= 1f)
			{
				return -1f;
			}
			return (x as ResearchableItemInstance).progress;
		}).ToList<NonSpatialItemInstance>();
		for (int i = 0; i < list.Count; i++)
		{
			NonSpatialItemInstance nonSpatialItemInstance = list[i];
			if (nonSpatialItemInstance.GetItemData<NonSpatialItemData>().showInCabin)
			{
				GameObject gameObject;
				if (nonSpatialItemInstance is ResearchableItemInstance)
				{
					gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.researchableItemEntryPrefab, this.itemEntryContainer);
				}
				else
				{
					gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.itemEntryPrefab, this.itemEntryContainer);
				}
				NonSpatialGridEntryUI component = gameObject.GetComponent<NonSpatialGridEntryUI>();
				component.Init(nonSpatialItemInstance);
				NonSpatialGridEntryUI nonSpatialGridEntryUI = component;
				nonSpatialGridEntryUI.OnEntrySelected = (Action<NonSpatialGridEntryUI>)Delegate.Combine(nonSpatialGridEntryUI.OnEntrySelected, new Action<NonSpatialGridEntryUI>(this.OnEntrySelected));
				NonSpatialGridEntryUI nonSpatialGridEntryUI2 = component;
				nonSpatialGridEntryUI2.OnEntrySubmitted = (Action<NonSpatialGridEntryUI>)Delegate.Combine(nonSpatialGridEntryUI2.OnEntrySubmitted, new Action<NonSpatialGridEntryUI>(this.OnEntrySubmitted));
				this.nonSpatialGridEntries.Add(component);
			}
		}
		for (int j = 0; j < this.nonSpatialGridEntries.Count; j++)
		{
			Navigation navigation = this.nonSpatialGridEntries[j].Button.navigation;
			navigation.mode = Navigation.Mode.Vertical;
			this.nonSpatialGridEntries[j].Button.navigation = navigation;
		}
		Navigation navigation2;
		if (this.nonSpatialGridEntries.Count == 0)
		{
			navigation2 = this.messagesButton.navigation;
			navigation2.selectOnDown = null;
			this.messagesButton.navigation = navigation2;
			navigation2 = this.encyclopediaButton.navigation;
			navigation2.selectOnDown = null;
			this.encyclopediaButton.navigation = navigation2;
			return;
		}
		navigation2 = this.messagesButton.navigation;
		navigation2.selectOnDown = this.nonSpatialGridEntries[0].Button;
		this.messagesButton.navigation = navigation2;
		navigation2 = this.encyclopediaButton.navigation;
		navigation2.selectOnDown = this.nonSpatialGridEntries[0].Button;
		this.encyclopediaButton.navigation = navigation2;
	}

	public void SelectFirstGridEntry()
	{
		if (this.nonSpatialGridEntries.Count > 0)
		{
			EventSystem.current.SetSelectedGameObject(this.nonSpatialGridEntries[0].Button.gameObject);
			this.nonSpatialGridEntries[0].Button.Select();
			if (!GameManager.Instance.Input.IsUsingController)
			{
				this.scrollRect.content.anchoredPosition = this.scrollRect.GetSnapToPositionToBringChildIntoView(this.nonSpatialGridEntries[0].gameObject.transform as RectTransform);
			}
		}
	}

	private void OnEntrySelected(NonSpatialGridEntryUI entryUI)
	{
		if (GameManager.Instance.Input.IsUsingController)
		{
			if (this.lerpCoroutine != null)
			{
				base.StopCoroutine(this.lerpCoroutine);
			}
			Vector2 snapToPositionToBringChildIntoView = this.scrollRect.GetSnapToPositionToBringChildIntoView(entryUI.gameObject.transform as RectTransform);
			this.lerpCoroutine = base.StartCoroutine(this.LerpToDestinationPos(snapToPositionToBringChildIntoView));
		}
	}

	private IEnumerator LerpToDestinationPos(Vector2 destinationPos)
	{
		Canvas.ForceUpdateCanvases();
		this.isLerpingToDestinationPos = true;
		while (this.isLerpingToDestinationPos)
		{
			float num = Mathf.Min(10f * Time.deltaTime, 1f);
			this.scrollRect.content.anchoredPosition = Vector2.Lerp(this.scrollRect.content.anchoredPosition, destinationPos, num);
			if (Vector2.SqrMagnitude(this.scrollRect.content.anchoredPosition - destinationPos) < 10f)
			{
				this.scrollRect.content.anchoredPosition = destinationPos;
				this.isLerpingToDestinationPos = false;
			}
			yield return null;
		}
		this.lerpCoroutine = null;
		yield break;
	}

	private void OnEntrySubmitted(NonSpatialGridEntryUI entryUI)
	{
		if (entryUI.ItemInstance is ResearchableItemInstance && !(entryUI.ItemInstance as ResearchableItemInstance).IsResearchComplete)
		{
			GameManager.Instance.ItemManager.SetActiveResearchableItem(entryUI.ItemInstance);
			this.nonSpatialGridEntries.ForEach(delegate(NonSpatialGridEntryUI e)
			{
				e.RefreshUI();
			});
		}
	}

	private int SortNonSpatialItems(NonSpatialItemInstance a, NonSpatialItemInstance b)
	{
		return a.CompareTo(b);
	}

	private void Clear()
	{
		for (int i = 0; i < this.nonSpatialGridEntries.Count; i++)
		{
			NonSpatialGridEntryUI nonSpatialGridEntryUI = this.nonSpatialGridEntries[i];
			nonSpatialGridEntryUI.OnEntrySelected = (Action<NonSpatialGridEntryUI>)Delegate.Remove(nonSpatialGridEntryUI.OnEntrySelected, new Action<NonSpatialGridEntryUI>(this.OnEntrySelected));
			NonSpatialGridEntryUI nonSpatialGridEntryUI2 = this.nonSpatialGridEntries[i];
			nonSpatialGridEntryUI2.OnEntrySubmitted = (Action<NonSpatialGridEntryUI>)Delegate.Remove(nonSpatialGridEntryUI2.OnEntrySubmitted, new Action<NonSpatialGridEntryUI>(this.OnEntrySubmitted));
			global::UnityEngine.Object.Destroy(this.nonSpatialGridEntries[i].gameObject);
		}
		this.nonSpatialGridEntries.Clear();
		foreach (object obj in this.itemEntryContainer)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	[SerializeField]
	private Button messagesButton;

	[SerializeField]
	private Button encyclopediaButton;

	[SerializeField]
	private GameObject itemEntryPrefab;

	[SerializeField]
	private GameObject researchableItemEntryPrefab;

	[SerializeField]
	private RectTransform itemEntryContainer;

	[SerializeField]
	private ScrollRect scrollRect;

	private List<NonSpatialGridEntryUI> nonSpatialGridEntries = new List<NonSpatialGridEntryUI>();

	private Coroutine lerpCoroutine;

	private bool isLerpingToDestinationPos;
}
