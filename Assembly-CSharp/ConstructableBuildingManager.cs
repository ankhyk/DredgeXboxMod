using System;
using System.Linq;
using CommandTerminal;
using UnityEngine;

public class ConstructableBuildingManager : MonoBehaviour
{
	private void OnEnable()
	{
		GameManager.Instance.ConstructableBuildingManager = this;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameManager.Instance.ConstructableBuildingManager = null;
		this.RemoveTerminalCommands();
	}

	private void AddTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.AddCommand("construct", new Action<CommandArg[]>(this.DebugConstructBuilding), 2, 2, "Constructs or destructs a building e.g. construct factory-tier-2 1");
			Terminal.Shell.AddCommand("construct.eligible", new Action<CommandArg[]>(this.DebugCheckConstructionEligible), 1, 1, "Checks the eligibility of a building for construction e.g. construct.eligible factory-tier-2");
		}
	}

	private void RemoveTerminalCommands()
	{
		if (Terminal.Shell != null)
		{
			Terminal.Shell.RemoveCommand("construct");
			Terminal.Shell.RemoveCommand("construct.eligible");
		}
	}

	private void DebugConstructBuilding(CommandArg[] args)
	{
		string @string = args[0].String;
		this.SetIsBuildingConstructed(@string, args[1].Int == 1, true);
	}

	public void DebugCheckConstructionEligible(CommandArg[] args)
	{
		string text = args[0].String.Replace("-", "_").ToUpperInvariant();
		BuildingTierId buildingTierId = BuildingTierId.NONE;
		if (Enum.TryParse<BuildingTierId>(text, out buildingTierId))
		{
			this.GetCanBuildingBeConstructed(buildingTierId);
		}
	}

	public bool GetIsBuildingConstructed(BuildingTierId buildingTierId)
	{
		return GameManager.Instance.SaveData.GetIsBuildingConstructed(buildingTierId);
	}

	public bool GetCanBuildingBeConstructed(BuildingTierId buildingTierId)
	{
		if (this.GetIsBuildingConstructed(buildingTierId))
		{
			return false;
		}
		this.GetIsBuildingConstructed(buildingTierId);
		ConstructableBuildingDependencyData constructableBuildingDependencyData = this.config.data.Find((ConstructableBuildingDependencyData d) => d.tierId == buildingTierId);
		if (constructableBuildingDependencyData == null)
		{
			return false;
		}
		if (GameManager.Instance.SaveData.TIRWorldPhase < constructableBuildingDependencyData.dependentTirWorldPhase)
		{
			return false;
		}
		if (constructableBuildingDependencyData.dependentTierIds.Count > 0 && !constructableBuildingDependencyData.dependentTierIds.All((BuildingTierId d) => this.GetIsBuildingConstructed(d)))
		{
			return false;
		}
		if (constructableBuildingDependencyData.dependentQuestSteps != null && constructableBuildingDependencyData.dependentQuestSteps.Count > 0)
		{
			if (!constructableBuildingDependencyData.dependentQuestSteps.All((QuestStepData q) => GameManager.Instance.QuestManager.GetIsQuestStepCompleted(q.name)))
			{
				return false;
			}
		}
		return true;
	}

	public void SetIsBuildingConstructed(string buildingIdString, bool isConstructed, bool immediate)
	{
		buildingIdString = buildingIdString.Replace("-", "_").ToUpperInvariant();
		BuildingTierId buildingTierId = BuildingTierId.NONE;
		if (Enum.TryParse<BuildingTierId>(buildingIdString, out buildingTierId))
		{
			this.SetIsBuildingConstructed(buildingTierId, isConstructed, immediate);
		}
	}

	public void SetIsBuildingConstructed(BuildingTierId buildingTierId, bool isConstructed, bool immediate)
	{
		GameManager.Instance.SaveData.SetIsBuildingConstructed(buildingTierId, isConstructed);
		if (immediate)
		{
			GameEvents.Instance.TriggerBuildingConstructed(buildingTierId);
		}
	}

	[SerializeField]
	private ConstructableBuildingDependencyConfig config;

	[SerializeField]
	private VibrationData constructionVibrationData;
}
