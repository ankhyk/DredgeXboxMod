using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagesWindow : PopupWindow
{
	protected override void Awake()
	{
		this.controllerFocusGrabber.enabled = false;
		base.Awake();
	}

	public override void Show()
	{
		this.controllerFocusGrabber.enabled = true;
		this.RefreshUI();
		base.Show();
		this.tabbedPanelContainer.ShowStart();
		this.tabbedPanelContainer.ShowFinish();
		this.PostRefreshUI();
	}

	private void RefreshUI()
	{
		this.messageEntries = new List<MessageListEntry>();
		foreach (object obj in this.messageEntryContainer)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		List<NonSpatialItemInstance> list = (from m in GameManager.Instance.SaveData.GetMessages()
			where m.GetItemData<MessageItemData>().set == this.currentSet
			select m into x
			orderby x.isNew descending, x.GetItemData<MessageItemData>().chronologicalOrder
			select x).ToList<NonSpatialItemInstance>();
		list.ForEach(delegate(NonSpatialItemInstance messageInstance)
		{
			MessageListEntry component = global::UnityEngine.Object.Instantiate<GameObject>(this.listEntryPrefab, this.messageEntryContainer).GetComponent<MessageListEntry>();
			MessageListEntry messageListEntry = component;
			messageListEntry.OnEntrySelected = (Action<MessageListEntry>)Delegate.Combine(messageListEntry.OnEntrySelected, new Action<MessageListEntry>(this.OnEntrySelected));
			MessageListEntry messageListEntry2 = component;
			messageListEntry2.OnEntrySubmitted = (Action<MessageListEntry>)Delegate.Combine(messageListEntry2.OnEntrySubmitted, new Action<MessageListEntry>(this.OnEntrySubmitted));
			component.Init(messageInstance);
			this.messageEntries.Add(component);
		});
		int count = (from m in GameManager.Instance.ItemManager.GetAllItemsOfType<MessageItemData>()
			where m.set == this.currentSet
			select m).ToList<MessageItemData>().Count;
		this.messageCounterText.text = string.Format("{0} / {1}", list.Count, count);
	}

	private void PostRefreshUI()
	{
		if (this.messageEntries.Count > 0)
		{
			this.controllerFocusGrabber.SetSelectable(this.messageEntries[0].Button);
			this.controllerFocusGrabber.SelectSelectable();
			this.OnEntrySelected(this.messageEntries[0]);
		}
		else
		{
			this.controllerFocusGrabber.SetSelectable(null);
		}
		this.messageEntryContainer.position = new Vector2(this.messageEntryContainer.position.x, this.messageEntryContainer.sizeDelta.y * -0.5f);
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		if (this.messageDetailWindow.IsShowing)
		{
			this.messageDetailWindow.Hide(windowHideMode);
		}
		this.controllerFocusGrabber.enabled = false;
		base.Hide(windowHideMode);
		this.tabbedPanelContainer.HideStart();
		this.tabbedPanelContainer.HideFinish();
	}

	private void OnEnable()
	{
		TabbedPanelContainer tabbedPanelContainer = this.tabbedPanelContainer;
		tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Combine(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnTabChanged));
	}

	private void OnDisable()
	{
		if (this.messageEntries != null && this.messageEntries.Count > 0)
		{
			this.messageEntries.ForEach(delegate(MessageListEntry mle)
			{
				mle.OnEntrySelected = (Action<MessageListEntry>)Delegate.Remove(mle.OnEntrySelected, new Action<MessageListEntry>(this.OnEntrySelected));
				mle.OnEntrySubmitted = (Action<MessageListEntry>)Delegate.Remove(mle.OnEntrySubmitted, new Action<MessageListEntry>(this.OnEntrySubmitted));
			});
		}
		TabbedPanelContainer tabbedPanelContainer = this.tabbedPanelContainer;
		tabbedPanelContainer.OnTabChanged = (Action<int>)Delegate.Remove(tabbedPanelContainer.OnTabChanged, new Action<int>(this.OnTabChanged));
	}

	private void OnTabChanged(int index)
	{
		this.currentSet = index;
		this.RefreshUI();
		this.PostRefreshUI();
	}

	private void OnEntrySelected(MessageListEntry entryUI)
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

	private void OnEntrySubmitted(MessageListEntry entryUI)
	{
		if (this.isShowingDetailedView)
		{
			return;
		}
		this.isShowingDetailedView = true;
		this.canvasGroupDisabler.Disable();
		this.controllerFocusGrabber.SetSelectable(entryUI.Button);
		this.controllerFocusGrabber.enabled = false;
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.RemoveActionListener(array, ActionLayer.POPUP_WINDOW);
		this.messageDetailWindow.Init(entryUI.MessageItemData);
		this.messageDetailWindow.Show();
		MessageDetailWindow messageDetailWindow = this.messageDetailWindow;
		messageDetailWindow.OnHideComplete = (Action)Delegate.Combine(messageDetailWindow.OnHideComplete, new Action(this.OnMessageDetailPopupWindowHidden));
	}

	private void OnMessageDetailPopupWindowHidden()
	{
		MessageDetailWindow messageDetailWindow = this.messageDetailWindow;
		messageDetailWindow.OnHideComplete = (Action)Delegate.Remove(messageDetailWindow.OnHideComplete, new Action(this.OnMessageDetailPopupWindowHidden));
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.backAction };
		input.AddActionListener(array, ActionLayer.POPUP_WINDOW);
		this.controllerFocusGrabber.enabled = true;
		this.canvasGroupDisabler.Enable();
		this.isShowingDetailedView = false;
	}

	private IEnumerator LerpToDestinationPos(Vector2 destinationPos)
	{
		Canvas.ForceUpdateCanvases();
		this.isLerpingToDestinationPos = true;
		while (this.isLerpingToDestinationPos)
		{
			float num = Mathf.Min(10f * Time.unscaledDeltaTime, 1f);
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

	[SerializeField]
	private TabbedPanelContainer tabbedPanelContainer;

	[SerializeField]
	private MessageDetailWindow messageDetailWindow;

	[SerializeField]
	private GameObject listEntryPrefab;

	[SerializeField]
	private RectTransform messageEntryContainer;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private CanvasGroupDisabler canvasGroupDisabler;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private TextMeshProUGUI messageCounterText;

	private List<MessageListEntry> messageEntries;

	private Coroutine lerpCoroutine;

	private bool isLerpingToDestinationPos;

	private bool isShowingDetailedView;

	private int currentSet;
}
