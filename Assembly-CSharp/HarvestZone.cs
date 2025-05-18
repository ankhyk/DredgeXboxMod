using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HarvestZone : MonoBehaviour
{
	public HarvestableItemData[] HarvestableItems
	{
		get
		{
			return this.harvestableItems;
		}
	}

	public bool Day
	{
		get
		{
			return this.day;
		}
	}

	public bool Night
	{
		get
		{
			return this.night;
		}
	}

	public void Rename()
	{
		if (base.gameObject)
		{
			string text = "";
			for (int i = 0; i < this.harvestableItems.Length; i++)
			{
				text += this.harvestableItems[i].id;
				if (i < this.harvestableItems.Length - 1)
				{
					text += ", ";
				}
			}
			base.gameObject.name = text ?? "";
		}
	}

	private void OnEnable()
	{
		if (GameSceneInitializer.Instance.IsDone())
		{
			GameEvents.Instance.TriggerHarvestZoneBecomeActive();
		}
	}

	[SerializeField]
	private HarvestableItemData[] harvestableItems;

	[SerializeField]
	private bool day;

	[SerializeField]
	private bool night;
}
