using System;
using UnityEngine;

public abstract class PlayerLight : MonoBehaviour
{
	public Light Light
	{
		get
		{
			return this.light;
		}
	}

	public float Intensity
	{
		get
		{
			return this.light.intensity;
		}
		set
		{
			this.light.intensity = value;
		}
	}

	public float Range
	{
		get
		{
			return this.light.range;
		}
		set
		{
			this.light.range = value;
		}
	}

	public float CalculatedIntensity
	{
		get
		{
			return this.calculatedIntensity;
		}
		set
		{
			this.calculatedIntensity = value;
		}
	}

	public float CalculatedRange
	{
		get
		{
			return this.calculatedRange;
		}
		set
		{
			this.calculatedRange = value;
		}
	}

	private void OnEnable()
	{
		if (this.respondToAdvancedLightsAbility)
		{
			this.hasAdvancedLights = GameManager.Instance.SaveData.unlockedAbilities.Contains("lights-advanced");
			GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		}
		this.RefreshLightStrength();
		GameEvents.Instance.OnPlayerStatsChanged += this.RefreshLightStrength;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerStatsChanged -= this.RefreshLightStrength;
		if (this.respondToAdvancedLightsAbility)
		{
			GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
		}
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		if (abilityData.name == "lights-advanced")
		{
			GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
			this.hasAdvancedLights = true;
			this.RefreshLightStrength();
		}
	}

	protected virtual void RefreshLightStrength()
	{
		if (this.respondToAdvancedLightsAbility && this.hasAdvancedLights)
		{
			this.calculatedIntensity *= GameManager.Instance.GameConfigData.AdvancedLightsIntensityModifier;
			this.calculatedRange *= GameManager.Instance.GameConfigData.AdvancedLightsRangeModifier;
		}
		this.Intensity = this.calculatedIntensity;
		this.Range = this.calculatedRange;
	}

	[SerializeField]
	protected Light light;

	[SerializeField]
	private bool respondToAdvancedLightsAbility;

	[SerializeField]
	protected float lumensIntensityCoefficient;

	protected float calculatedIntensity;

	protected float calculatedRange;

	private bool hasAdvancedLights;
}
