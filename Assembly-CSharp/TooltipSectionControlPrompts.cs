using System;

public class TooltipSectionControlPrompts : TooltipSection<ItemInstance>
{
	public override void Init<T>(T itemInstance, TooltipUI.TooltipMode tooltipMode)
	{
		this.isLayedOut = true;
	}

	public override void Init()
	{
		this.isLayedOut = true;
	}
}
