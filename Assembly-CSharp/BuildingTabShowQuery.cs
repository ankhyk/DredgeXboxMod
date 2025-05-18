using System;

public class BuildingTabShowQuery : TabShowQuery
{
	public BuildingTierId BuildingTierId
	{
		get
		{
			return this.buildingTierId;
		}
		set
		{
			this.buildingTierId = value;
		}
	}

	public override bool GetCanNavigate()
	{
		return GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(this.buildingTierId) || GameManager.Instance.ConstructableBuildingManager.GetCanBuildingBeConstructed(this.buildingTierId);
	}

	public override bool GetCanShow()
	{
		return true;
	}

	private BuildingTierId buildingTierId;
}
