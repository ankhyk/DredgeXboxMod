using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PatrollingWildlifeManager : SerializedMonoBehaviour
{
	[SerializeField]
	private List<PatrollingWildlifeManager.PatrollingWildlifeConfig> configs = new List<PatrollingWildlifeManager.PatrollingWildlifeConfig>();

	private class PatrollingWildlifeConfig
	{
		public PatrollingWildlife wildlifeObject;

		public List<Transform> path;
	}
}
