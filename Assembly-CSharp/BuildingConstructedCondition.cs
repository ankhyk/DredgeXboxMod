using System;

public class BuildingConstructedCondition : QuestStepCondition
{
	public override bool Evaluate()
	{
		return GameManager.Instance.ConstructableBuildingManager && GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(this.buildingTierId);
	}

	public BuildingTierId buildingTierId;
}
