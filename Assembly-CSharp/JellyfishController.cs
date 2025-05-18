using System;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishController : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbility.name)
		{
			this.isBanishActive = enabled;
			if (this.isBanishActive)
			{
				this.BanishAll();
			}
		}
	}

	private void ShowAll()
	{
		this.jellyfishes.ForEach(delegate(Jellyfish j)
		{
			j.Show();
		});
	}

	private void HideAll()
	{
		this.jellyfishes.ForEach(delegate(Jellyfish j)
		{
			j.Hide(false);
		});
	}

	private void BanishAll()
	{
		this.jellyfishes.ForEach(delegate(Jellyfish j)
		{
			j.Hide(true);
		});
	}

	private void Update()
	{
		this.timeUntilNextCheck -= Time.deltaTime;
		if (this.timeUntilNextCheck <= 0f)
		{
			this.timeUntilNextCheck = this.timeBetweenChecks;
			if (GameManager.Instance.Time.IsDaytime)
			{
				this.HideAll();
				return;
			}
			if (!this.isBanishActive)
			{
				this.ShowAll();
			}
		}
	}

	[SerializeField]
	private List<Jellyfish> jellyfishes;

	[SerializeField]
	private float timeBetweenChecks;

	[SerializeField]
	private AbilityData banishAbility;

	private float timeUntilNextCheck;

	private bool isBanishActive;
}
