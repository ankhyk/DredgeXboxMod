using System;
using UnityEngine;

public class TriggerableConstructionTimeline : TriggerableTimeline
{
	public void DoConstruct()
	{
		GameManager.Instance.ConstructableBuildingManager.SetIsBuildingConstructed(this.buildingTierId, true, true);
	}

	public void OnScaffoldingFallComplete()
	{
		GameManager.Instance.VibrationManager.Vibrate(this.buildingCompleteVibrationData, VibrationRegion.WholeBody, true);
	}

	[SerializeField]
	private BuildingTierId buildingTierId;

	[SerializeField]
	private VibrationData buildingCompleteVibrationData;
}
