using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotUpgradeData", menuName = "Dredge/UpgradeData/SlotUpgradeData", order = 0)]
public class SlotUpgradeData : UpgradeData
{
	public List<CellGroupConfiguration> CellGroupConfigs
	{
		get
		{
			return this.cellGroupConfigs;
		}
	}

	public override int GetNewCellCount()
	{
		int count = 0;
		this.cellGroupConfigs.ForEach(delegate(CellGroupConfiguration x)
		{
			count += x.cells.Count;
		});
		return count;
	}

	[SerializeField]
	private List<CellGroupConfiguration> cellGroupConfigs = new List<CellGroupConfiguration>();
}
