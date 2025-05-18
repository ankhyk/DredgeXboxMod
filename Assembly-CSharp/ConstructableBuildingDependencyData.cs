using System;
using System.Collections.Generic;

public class ConstructableBuildingDependencyData
{
	public BuildingTierId tierId;

	public List<BuildingTierId> dependentTierIds = new List<BuildingTierId>();

	public int dependentTirWorldPhase;

	public List<QuestStepData> dependentQuestSteps = new List<QuestStepData>();
}
