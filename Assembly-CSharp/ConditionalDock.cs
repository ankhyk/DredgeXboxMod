using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class ConditionalDock : Dock
{
	private ConditionalDock.ConditionalDockStruct GetConditionalDockData()
	{
		ConditionalDock.ConditionalDockStruct result = this.conditionalData["default"];
		this.conditionalData.Keys.ToList<string>().ForEach(delegate(string key)
		{
			if (GameManager.Instance.SaveData.GetBoolVariable(key, false))
			{
				result = this.conditionalData[key];
			}
		});
		return result;
	}

	public override List<BaseDestination> GetDestinations()
	{
		return this.GetConditionalDockData().destinations;
	}

	public override CinemachineVirtualCamera GetVCam()
	{
		return this.GetConditionalDockData().cam;
	}

	public override void EnableVCam()
	{
		this.GetVCam().enabled = true;
	}

	public override void DisableVCam()
	{
		this.conditionalData.Values.ToList<ConditionalDock.ConditionalDockStruct>().ForEach(delegate(ConditionalDock.ConditionalDockStruct c)
		{
			c.cam.enabled = false;
		});
	}

	[SerializeField]
	private Dictionary<string, ConditionalDock.ConditionalDockStruct> conditionalData = new Dictionary<string, ConditionalDock.ConditionalDockStruct>();

	private struct ConditionalDockStruct
	{
		public CinemachineVirtualCamera cam;

		public List<BaseDestination> destinations;
	}
}
