using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldStateChanger : SerializedMonoBehaviour
{
	private void FindConfigs()
	{
		this.allConfigs = global::UnityEngine.Object.FindObjectsOfType<WorldStateChangerConfig>().ToList<WorldStateChangerConfig>();
	}

	private void OnEnable()
	{
		this.allConfigs.ForEach(delegate(WorldStateChangerConfig c)
		{
			c.CheckPersistence();
		});
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		this.dueForRefresh = true;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		this.dueForRefresh = true;
	}

	private void Update()
	{
		if (this.dueForRefresh)
		{
			this.dueForRefresh = false;
			this.allConfigs.ForEach(delegate(WorldStateChangerConfig c)
			{
				if (!c.Done && c.EvaluateCondition())
				{
					c.Do();
				}
			});
		}
	}

	[SerializeField]
	private List<WorldStateChangerConfig> allConfigs;

	private bool dueForRefresh;
}
