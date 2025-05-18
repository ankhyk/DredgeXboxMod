using System;

public class OverflowStorageDestination : BaseDestination
{
	public override bool AlwaysShow
	{
		get
		{
			return GameManager.Instance.SaveData.OverflowStorage.spatialItems.Count > 0;
		}
	}
}
