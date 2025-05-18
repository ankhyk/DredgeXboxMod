using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MessagesTab : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshUnseenMessageStates();
		GameEvents.Instance.OnHasUnseenItemsChanged += this.RefreshUnseenMessageStates;
	}

	public void OnDisable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.RefreshUnseenMessageStates;
	}

	private void RefreshUnseenMessageStates()
	{
		List<NonSpatialItemInstance> list = (from m in GameManager.Instance.SaveData.GetMessages()
			where m.GetItemData<MessageItemData>().set == this.set
			select m).ToList<NonSpatialItemInstance>();
		this.unseenItemIcon.SetActive(list.Any((NonSpatialItemInstance m) => m.isNew));
	}

	[SerializeField]
	private int set;

	[SerializeField]
	public GameObject unseenItemIcon;
}
