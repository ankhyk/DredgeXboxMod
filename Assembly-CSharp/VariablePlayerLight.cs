using System;

public class VariablePlayerLight : PlayerLight
{
	protected override void RefreshLightStrength()
	{
		this.calculatedIntensity = GameManager.Instance.PlayerStats.LightLumens * this.lumensIntensityCoefficient;
		this.calculatedRange = GameManager.Instance.PlayerStats.LightRange;
		base.RefreshLightStrength();
	}
}
