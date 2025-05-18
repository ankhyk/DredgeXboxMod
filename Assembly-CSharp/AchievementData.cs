using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "Dredge/AchievementData", order = 0)]
public class AchievementData : SerializedScriptableObject
{
	public IEnumerable<Type> GetAchievementConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(AchievementCondition));
	}

	public bool Evaluate()
	{
		if (this.evaluationConditions.Count > 0)
		{
			return this.evaluationConditions.TrueForAll((AchievementCondition c) => c.Evaluate());
		}
		return false;
	}

	public void Print()
	{
		string printStr = string.Format("{0}: ", this.id);
		if (this.evaluationConditions.Count > 0)
		{
			this.evaluationConditions.ForEach(delegate(AchievementCondition c)
			{
				printStr = printStr + c.Print() + " ";
			});
			return;
		}
		printStr += "This achievement is triggered manually.";
	}

	public DredgeAchievementId id;

	public string steamId;

	public int playStationId;

	public string xboxId;

	[SerializeField]
	public List<AchievementCondition> evaluationConditions = new List<AchievementCondition>();
}
