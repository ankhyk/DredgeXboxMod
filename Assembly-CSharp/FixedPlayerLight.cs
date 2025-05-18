using System;
using UnityEngine;

public class FixedPlayerLight : PlayerLight
{
	protected override void RefreshLightStrength()
	{
		this.calculatedIntensity = this.fixedIntensity;
		this.calculatedRange = this.fixedRange;
		base.RefreshLightStrength();
	}

	[SerializeField]
	private float fixedIntensity;

	[SerializeField]
	private float fixedRange;
}
