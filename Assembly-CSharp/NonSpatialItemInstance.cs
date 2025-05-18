using System;

[Serializable]
public class NonSpatialItemInstance : ItemInstance
{
	public int CompareTo(NonSpatialItemInstance compareItem)
	{
		if (compareItem == null)
		{
			return 1;
		}
		if (this.isNew && !compareItem.isNew)
		{
			return -1;
		}
		if (!this.isNew && compareItem.isNew)
		{
			return 1;
		}
		bool flag = this.isNew;
		bool flag2 = compareItem.isNew;
		return 0;
	}

	public bool isNew;
}
