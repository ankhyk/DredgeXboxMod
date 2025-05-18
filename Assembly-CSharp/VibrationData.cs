using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VibrationData", menuName = "Dredge/VibrationData", order = 0)]
public class VibrationData : ScriptableObject
{
	public List<VibrationParams> vibrationParamsList = new List<VibrationParams>();
}
