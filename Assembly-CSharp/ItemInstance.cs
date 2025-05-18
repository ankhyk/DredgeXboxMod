using System;
using UnityEngine;

[Serializable]
public class ItemInstance
{
	public virtual T GetItemData<T>() where T : ItemData
	{
		if (this._itemData == null)
		{
			this._itemData = GameManager.Instance.ItemManager.GetItemDataById<ItemData>(this.id);
		}
		if (this._itemData == null)
		{
			Debug.LogWarning("[ItemInstance] GetItemData() error: could not find itemData for item id: " + this.id);
			return default(T);
		}
		return this._itemData as T;
	}

	public string id;

	[NonSerialized]
	protected ItemData _itemData;
}
