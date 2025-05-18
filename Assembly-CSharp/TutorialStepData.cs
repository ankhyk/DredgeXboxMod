using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "TutorialStepData", menuName = "Dredge/TutorialStepData", order = 0)]
public class TutorialStepData : SerializedScriptableObject
{
	public IEnumerable<Type> GetConditionType()
	{
		return UnityExtensions.GetFilteredTypeList(typeof(TutorialStepCondition));
	}

	public float GetProgress()
	{
		int numWithProgress = 0;
		float totalProgress = 0f;
		this.hideConditions.ForEach(delegate(TutorialStepCondition tsc)
		{
			if (tsc.reportsProgress)
			{
				int numWithProgress2 = numWithProgress;
				numWithProgress = numWithProgress2 + 1;
				totalProgress += tsc.GetProgress();
			}
		});
		return totalProgress / (float)numWithProgress;
	}

	[SerializeField]
	public TutorialStepEnum stepId;

	[SerializeField]
	public List<TutorialStepEnum> prerequisiteSteps = new List<TutorialStepEnum>();

	[SerializeField]
	public LocalizedString localizedString;

	[SerializeField]
	public TutorialStepViewEnum viewModes;

	[SerializeField]
	public List<DredgeControlEnum> stringArgumentControls = new List<DredgeControlEnum>();

	[SerializeField]
	public List<TutorialStepCondition> showConditions = new List<TutorialStepCondition>();

	[SerializeField]
	public List<TutorialStepCondition> hideConditions = new List<TutorialStepCondition>();

	[SerializeField]
	public Vector2 position;

	[SerializeField]
	public Vector2 anchorMin;

	[SerializeField]
	public Vector2 anchorMax;

	[SerializeField]
	public Vector2 pivot;
}
