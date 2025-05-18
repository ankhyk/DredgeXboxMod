using System;
using System.Collections.Generic;

public class ResearchCompleteCondition : AchievementCondition
{
	public override bool Evaluate()
	{
		SaveData saveData = GameManager.Instance.SaveData;
		return this.items.TrueForAll((SpatialItemData i) => saveData.GetIsItemResearched(i));
	}

	public override string Print()
	{
		int count = this.items.FindAll((SpatialItemData i) => GameManager.Instance.SaveData.GetIsItemResearched(i)).Count;
		return string.Format("ResearchCompleteCondition: {0} [{1} / {2}]", count >= this.items.Count, count, this.items.Count);
	}

	public List<SpatialItemData> items = new List<SpatialItemData>();
}
