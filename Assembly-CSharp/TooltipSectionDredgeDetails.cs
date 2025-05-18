using System;
using UnityEngine;

public class TooltipSectionDredgeDetails : TooltipSection<DredgeItemData>
{
	public override void Init<T>(T itemData, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = false;
		this.harvestableTypeTagUI.Init(HarvestableType.DREDGE, false);
		this.isLayedOut = true;
	}

	[SerializeField]
	private HarvestableTypeTagUI harvestableTypeTagUI;
}
