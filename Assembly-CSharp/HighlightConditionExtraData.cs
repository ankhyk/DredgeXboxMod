using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HighlightConditionExtraData", menuName = "Dredge/HighlightConditionExtraData", order = 0)]
public class HighlightConditionExtraData : SerializedScriptableObject
{
	public IEnumerable<Type> GetQuestStepConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(QuestStepCondition));
	}

	public bool Evaluate()
	{
		bool flag = true;
		if (this.andTheseConditionsTrue.Count > 0)
		{
			if (this.allConditionsMustBeTrue)
			{
				flag = this.andTheseConditionsTrue.All((QuestStepCondition c) => c.Evaluate());
			}
			else
			{
				flag = this.andTheseConditionsTrue.Any((QuestStepCondition c) => c.Evaluate());
			}
		}
		bool flag2 = true;
		if (this.andTheseConditionsFalse.Count > 0)
		{
			if (this.allConditionsMustBeFalse)
			{
				flag2 = this.andTheseConditionsFalse.All((QuestStepCondition c) => !c.Evaluate());
			}
			else
			{
				flag2 = this.andTheseConditionsFalse.Any((QuestStepCondition c) => !c.Evaluate());
			}
		}
		return flag && flag2;
	}

	public bool allConditionsMustBeTrue = true;

	[SerializeField]
	public List<QuestStepCondition> andTheseConditionsTrue = new List<QuestStepCondition>();

	public bool allConditionsMustBeFalse = true;

	[SerializeField]
	public List<QuestStepCondition> andTheseConditionsFalse = new List<QuestStepCondition>();
}
