using System;

public class SpatialItemTooltipRequester : TooltipRequester
{
	public SpatialItemData SpatialItemData { get; set; }

	public TooltipUI.TooltipMode TooltipMode { get; set; }

	public RecipeData RecipeData { get; set; }
}
