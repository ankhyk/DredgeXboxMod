using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Dredge/ShopData", order = 0)]
public class ShopData : SerializedScriptableObject
{
	public List<SpatialItemData> GetNewStock()
	{
		List<SpatialItemData> newStock = new List<SpatialItemData>();
		this.alwaysInStock.ForEach(delegate(ShopData.ShopItemData sid)
		{
			for (int i = 0; i < sid.count; i++)
			{
				if (global::UnityEngine.Random.value < sid.chance)
				{
					newStock.Add(sid.itemData);
				}
			}
		});
		(from item in GameManager.Instance.SaveData.GetResearchedItemData()
			where item != null && this.itemSubtypesFromResearchPool.Contains(item.itemSubtype)
			select item).ToList<SpatialItemData>().ForEach(delegate(SpatialItemData itemData)
		{
			for (int j = 0; j < this.countOfEachItemFromResearchPool; j++)
			{
				newStock.Add(itemData);
			}
		});
		Action<ShopData.ShopItemData> <>9__5;
		this.phaseLinkedShopData.ForEach(delegate(ShopData.PhaseLinkedShopData phaseLinkedData)
		{
			if (GameManager.Instance.SaveData.WorldPhase >= phaseLinkedData.phase)
			{
				List<ShopData.ShopItemData> itemData = phaseLinkedData.itemData;
				Action<ShopData.ShopItemData> action;
				if ((action = <>9__5) == null)
				{
					action = (<>9__5 = delegate(ShopData.ShopItemData shopItemData)
					{
						for (int k = 0; k < shopItemData.count; k++)
						{
							newStock.Add(shopItemData.itemData);
						}
					});
				}
				itemData.ForEach(action);
			}
		});
		Action<ShopData.ShopItemData> <>9__8;
		this.dialogueLinkedShopData.ForEach(delegate(ShopData.DialogueLinkedShopData dialogueLinkedData)
		{
			bool flag = false;
			if (dialogueLinkedData.requireMode == ShopData.DialogueLinkedShopData.RequireMode.ALL)
			{
				flag = dialogueLinkedData.dialogueNodes.All((string d) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(d));
			}
			else if (dialogueLinkedData.requireMode == ShopData.DialogueLinkedShopData.RequireMode.ANY)
			{
				flag = dialogueLinkedData.dialogueNodes.Any((string d) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(d));
			}
			if (flag)
			{
				List<ShopData.ShopItemData> itemData2 = dialogueLinkedData.itemData;
				Action<ShopData.ShopItemData> action2;
				if ((action2 = <>9__8) == null)
				{
					action2 = (<>9__8 = delegate(ShopData.ShopItemData shopItemData)
					{
						for (int l = 0; l < shopItemData.count; l++)
						{
							newStock.Add(shopItemData.itemData);
						}
					});
				}
				itemData2.ForEach(action2);
			}
		});
		return newStock;
	}

	[SerializeField]
	private List<ShopData.ShopItemData> alwaysInStock = new List<ShopData.ShopItemData>();

	[SerializeField]
	private List<ShopData.PhaseLinkedShopData> phaseLinkedShopData = new List<ShopData.PhaseLinkedShopData>();

	[SerializeField]
	private List<ShopData.DialogueLinkedShopData> dialogueLinkedShopData = new List<ShopData.DialogueLinkedShopData>();

	[SerializeField]
	private List<ItemSubtype> itemSubtypesFromResearchPool;

	[SerializeField]
	private int countOfEachItemFromResearchPool;

	private class ShopItemData
	{
		public SpatialItemData itemData;

		public int count;

		public float chance = 1f;
	}

	private class PhaseLinkedShopData
	{
		public List<ShopData.ShopItemData> itemData = new List<ShopData.ShopItemData>();

		public int phase;
	}

	private class DialogueLinkedShopData
	{
		public List<ShopData.ShopItemData> itemData = new List<ShopData.ShopItemData>();

		public List<string> dialogueNodes;

		public ShopData.DialogueLinkedShopData.RequireMode requireMode;

		public enum RequireMode
		{
			ANY,
			ALL
		}
	}

	private class ShopItemTypeData
	{
		public ItemSubtype itemSubtype;

		public int count;
	}
}
