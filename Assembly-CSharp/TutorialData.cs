using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Dredge/TutorialData", order = 0)]
public class TutorialData : SerializedScriptableObject
{
	[SerializeField]
	public List<TutorialStepData> tutorialSteps = new List<TutorialStepData>();
}
