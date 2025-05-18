using System;
using UnityEngine;

public class VariableSanityModifier : MonoBehaviour
{
	private void OnEnable()
	{
		this.RefreshSanityModifierStrength();
		GameEvents.Instance.OnPlayerStatsChanged += this.RefreshSanityModifierStrength;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerStatsChanged -= this.RefreshSanityModifierStrength;
	}

	private void RefreshSanityModifierStrength()
	{
		if (this.affectsNightValue)
		{
			this.sanityModifier.SetNightValue(GameManager.Instance.PlayerStats.SanityModifier);
		}
		if (this.affectsDayValue)
		{
			this.sanityModifier.SetDayValue(GameManager.Instance.PlayerStats.SanityModifier);
		}
	}

	[SerializeField]
	private SanityModifier sanityModifier;

	[SerializeField]
	private bool affectsNightValue;

	[SerializeField]
	private bool affectsDayValue;
}
