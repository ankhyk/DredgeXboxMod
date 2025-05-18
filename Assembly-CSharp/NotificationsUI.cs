using System;
using System.Collections.Generic;
using CommandTerminal;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class NotificationsUI : SerializedMonoBehaviour
{
	private void Awake()
	{
		this.notifications = new NotificationUI[this.maxNotifications];
	}

	private int GetEmptyNotificationSlot()
	{
		for (int i = 0; i < this.notifications.Length; i++)
		{
			if (this.notifications[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	private int GetOldestNotificationSlot()
	{
		int num = -1;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < this.notifications.Length; i++)
		{
			if (this.notifications[i].AppearTime < num2)
			{
				num2 = this.notifications[i].AppearTime;
				num = i;
			}
		}
		return num;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnNotificationTriggered += this.OnNotificationTriggered;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnNotificationTriggered -= this.OnNotificationTriggered;
		this.RemoveTerminalCommands();
	}

	private void OnNotificationTriggered(NotificationType notificationType, string notificationString)
	{
		NotificationsUI.NotificationQueueEntry notificationQueueEntry = new NotificationsUI.NotificationQueueEntry();
		notificationQueueEntry.notificationType = notificationType;
		notificationQueueEntry.str = notificationString;
		this.notificationQueue.Enqueue(notificationQueueEntry);
		this.TryShowNotification();
	}

	private void TryShowNotification()
	{
		if (this.notificationQueue.Count <= 0 || GameManager.Instance.Player == null || !GameManager.Instance.Player.IsAlive)
		{
			return;
		}
		int emptyNotificationSlot = this.GetEmptyNotificationSlot();
		if (emptyNotificationSlot != -1)
		{
			NotificationsUI.NotificationQueueEntry notificationQueueEntry = this.notificationQueue.Dequeue();
			GameObject gameObject = this.notificationPrefab.Spawn(base.transform);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.anchoredPosition = new Vector2(0f, -((float)emptyNotificationSlot * this.notificationHeight) - this.notificationPadY * (float)(emptyNotificationSlot + 1));
			NotificationUI component = gameObject.GetComponent<NotificationUI>();
			this.notifications[emptyNotificationSlot] = component;
			float num = this.holdTimes[GameManager.Instance.SettingsSaveData.notificationDuration];
			component.Init(notificationQueueEntry.notificationType, notificationQueueEntry.str, num, emptyNotificationSlot, this.colorMap);
			if (Time.time > this.timeOfLastNotificationSFX + this.minTimeBetweenNotificationSFX)
			{
				if (this.notificationSFXReferences.ContainsKey(notificationQueueEntry.notificationType))
				{
					GameManager.Instance.AudioPlayer.PlaySFX(this.notificationSFXReferences[notificationQueueEntry.notificationType], AudioLayer.SFX_UI, 1f, 1f);
				}
				else
				{
					GameManager.Instance.AudioPlayer.PlaySFX(this.defaultNotificationReference, AudioLayer.SFX_UI, 1f, 1f);
				}
				this.timeOfLastNotificationSFX = Time.time;
			}
			rectTransform.DOAnchorPosX(this.notificationExtendWidth, this.slideInTimeSec, false).SetEase(Ease.OutExpo);
			NotificationUI notificationUI = component;
			notificationUI.OnHideComplete = (Action<NotificationUI, int>)Delegate.Combine(notificationUI.OnHideComplete, new Action<NotificationUI, int>(this.OnNotificationHideComplete));
			return;
		}
		int oldestNotificationSlot = this.GetOldestNotificationSlot();
		if (oldestNotificationSlot != -1 && !this.notifications[oldestNotificationSlot].IsAnimatingOut)
		{
			this.notifications[oldestNotificationSlot].Hide();
		}
	}

	private void OnNotificationHideComplete(NotificationUI notification, int index)
	{
		notification.Recycle<NotificationUI>();
		this.notifications[index] = null;
		this.TryShowNotification();
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("note", new Action<CommandArg[]>(this.SendNotification), 1, 1, "Sends a string notification");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("note");
		}
	}

	private void SendNotification(CommandArg[] args)
	{
		GameEvents.Instance.TriggerNotification(NotificationType.NONE, args[0].String);
	}

	[SerializeField]
	private Dictionary<NotificationType, AssetReference> notificationSFXReferences = new Dictionary<NotificationType, AssetReference>();

	[SerializeField]
	private AssetReference defaultNotificationReference;

	[SerializeField]
	private float notificationHeight;

	[SerializeField]
	private float notificationExtendWidth;

	[SerializeField]
	private float notificationPadX;

	[SerializeField]
	private float notificationPadY;

	[SerializeField]
	private float slideInTimeSec;

	[SerializeField]
	private List<float> holdTimes;

	[SerializeField]
	private int maxNotifications;

	[SerializeField]
	private float minTimeBetweenNotificationSFX;

	[SerializeField]
	private GameObject notificationPrefab;

	[SerializeField]
	private Dictionary<NotificationType, DredgeColorTypeEnum> colorMap;

	private NotificationUI[] notifications;

	private Queue<NotificationsUI.NotificationQueueEntry> notificationQueue = new Queue<NotificationsUI.NotificationQueueEntry>();

	private float timeOfLastNotificationSFX;

	private class NotificationQueueEntry
	{
		public NotificationType notificationType;

		public string str;
	}
}
