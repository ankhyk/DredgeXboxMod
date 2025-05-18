using System;
using System.Collections.Generic;
using System.Linq;
using CommandTerminal;
using Sirenix.OdinInspector;
using UnityEngine;

public class HarvestValidator : SerializedMonoBehaviour
{
	private void Awake()
	{
		GameManager.Instance.HarvestPOIManager = this;
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
		this.AddTerminalCommands();
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
		this.RemoveTerminalCommands();
	}

	private void OnFinaleVoyageStarted()
	{
		this.allHarvestPOIs.Where((HarvestPOI i) => i != null).ToList<HarvestPOI>().ForEach(delegate(HarvestPOI p)
		{
			p.AddStock(float.NegativeInfinity, false);
		});
		this.allItemPOIs.Where((ItemPOI i) => i != null).ToList<ItemPOI>().ForEach(delegate(ItemPOI p)
		{
			p.OnHarvested(true);
		});
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("tp", new Action<CommandArg[]>(this.TeleportPlayerToHarvestSpot), 1, 1, "teleports player to harvest spot matching the provided id");
		Terminal.Shell.AddCommand("restock", new Action<CommandArg[]>(this.RestockHarvestSpot), 1, 1, "fully stocks the harvest spot matching the provided id");
		Terminal.Shell.AddCommand("deplete", new Action<CommandArg[]>(this.DepleteHarvestSpots), 0, 0, "Depletes ALL harvest spots in the world, halving their current stock.");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("tp");
		Terminal.Shell.RemoveCommand("restock");
		Terminal.Shell.RemoveCommand("deplete");
	}

	private void TeleportPlayerToHarvestSpot(CommandArg[] args)
	{
		string targetId = args[0].String;
		HarvestPOI harvestPOI = this.allHarvestPOIs.Find((HarvestPOI h) => h.Harvestable.GetId() == targetId);
		if (harvestPOI == null)
		{
			CustomDebug.EditorLogError("[HarvestValidator] TeleportPlayerToHarvestSpot(" + targetId + ") there is no spot with that ID.");
			return;
		}
		Vector3 vector = harvestPOI.transform.position;
		Vector3 waveDisplacement = WaveDisplacement.GetWaveDisplacement(vector, GameManager.Instance.WaveController.Steepness, GameManager.Instance.WaveController.Wavelength, GameManager.Instance.WaveController.Speed, GameManager.Instance.WaveController.Directions);
		vector += waveDisplacement;
		BuoyantObject componentInChildren = GameManager.Instance.Player.GetComponentInChildren<BuoyantObject>();
		if (componentInChildren)
		{
			vector.y = -componentInChildren.objectDepth;
		}
		Vector3 vector2 = vector - GameManager.Instance.Player.transform.position;
		GameManager.Instance.PlayerCamera.CinemachineCamera.OnTargetObjectWarped(GameManager.Instance.PlayerCamera.CinemachineCamera.m_Follow.gameObject.transform, vector2);
		GameManager.Instance.Player.transform.position = vector;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
	}

	private void RestockHarvestSpot(CommandArg[] args)
	{
		string targetId = args[0].String;
		HarvestPOI harvestPOI = this.allHarvestPOIs.Find((HarvestPOI h) => h.Harvestable.GetId() == targetId);
		if (harvestPOI == null)
		{
			CustomDebug.EditorLogError("[HarvestValidator] RestockHarvestSpot(" + targetId + ") there is no spot with that ID.");
			return;
		}
		harvestPOI.AddStock(harvestPOI.MaxStock, true);
	}

	private void DepleteHarvestSpots(CommandArg[] args)
	{
		this.allHarvestPOIs.ForEach(delegate(HarvestPOI i)
		{
			i.AddStock(i.Stock * -0.5f, false);
		});
	}

	public List<string> GetIdsForItem(string id)
	{
		return (from h in this.allHarvestPOIs
			where h.Harvestable.GetFirstHarvestableItem().id == id
			select h.Harvestable.GetId()).ToList<string>();
	}

	[SerializeField]
	private List<HarvestPOI> allHarvestPOIs;

	[SerializeField]
	private List<ItemPOI> allItemPOIs;

	public Dictionary<string, HarvestPOI> harvestPOILookup;
}
