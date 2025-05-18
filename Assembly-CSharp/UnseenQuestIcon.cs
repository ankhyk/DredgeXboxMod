using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnseenQuestIcon : MonoBehaviour
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
		this.icon.enabled = GameManager.Instance.SaveData.questEntries.Values.Any((SerializedQuestEntry x) => x.hasUnseenUpdate);
	}

	[SerializeField]
	private Image icon;
}
