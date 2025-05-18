using System;
using UnityEngine;
using UnityEngine.Localization.Components;

public class MessageDetailWindow : PopupWindow
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void Show()
	{
		GameEvents.Instance.TriggerDetailPopupShowChange(true);
		base.Show();
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		base.Hide(windowHideMode);
		GameEvents.Instance.TriggerDetailPopupShowChange(false);
	}

	public void Init(MessageItemData messageData)
	{
		this.localizedTitleString.StringReference = messageData.itemNameKey;
		this.localizedTitleString.RefreshString();
		this.localizedBodyString.StringReference = messageData.messageBodyKey;
		this.localizedBodyString.RefreshString();
		this.containerCanvasGroup.alpha = 1f;
	}

	[SerializeField]
	private LocalizeStringEvent localizedTitleString;

	[SerializeField]
	private LocalizeStringEvent localizedBodyString;

	[SerializeField]
	private CanvasGroup containerCanvasGroup;
}
