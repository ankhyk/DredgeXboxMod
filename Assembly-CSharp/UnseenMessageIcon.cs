using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnseenMessageIcon : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshUI();
		GameEvents.Instance.OnHasUnseenItemsChanged += this.RefreshUI;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnHasUnseenItemsChanged -= this.RefreshUI;
	}

	private void RefreshUI()
	{
		this.icon.enabled = GameManager.Instance.SaveData.GetMessages().Any((NonSpatialItemInstance x) => x.isNew);
	}

	[SerializeField]
	private Image icon;
}
