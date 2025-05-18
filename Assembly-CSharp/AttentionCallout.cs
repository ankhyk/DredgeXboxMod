using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class AttentionCallout : SerializedMonoBehaviour
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(AttentionCalloutCondition));
	}

	public void Evaluate()
	{
		bool flag = false;
		if (this.conditions != null && this.conditions.Count > 0)
		{
			flag = this.conditions.Any((AttentionCalloutCondition c) => c.Evaluate(this));
		}
		this.attentionCalloutObject.SetActive(flag);
	}

	private void OnDisable()
	{
		this.attentionCalloutObject.SetActive(false);
	}

	[SerializeField]
	public ConstructableDestinationUI constructableDestinationUI;

	[SerializeField]
	public RecipeEntry recipeEntry;

	[SerializeField]
	private GameObject attentionCalloutObject;

	[SerializeField]
	public List<AttentionCalloutCondition> conditions = new List<AttentionCalloutCondition>();
}
