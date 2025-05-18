using System;
using UnityEngine;

public class LightAbility : Ability
{
	public override void Init()
	{
		base.Init();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnBoatModelChanged += this.OnBoatModelChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnBoatModelChanged -= this.OnBoatModelChanged;
	}

	private void OnDestroy()
	{
		this.playerRef.BoatModelProxy.SetLightStrength(0f);
	}

	private void OnBoatModelChanged()
	{
		if (this.isActive)
		{
			this.DoActivate();
		}
	}

	public override bool Activate()
	{
		if (this.Locked)
		{
			return false;
		}
		bool flag = false;
		if (this.isActive)
		{
			this.Deactivate();
		}
		else
		{
			this.isActive = true;
			this.DoActivate();
			flag = base.Activate();
		}
		VibrationData vibrationData = this.abilityData.primaryVibration;
		if (base.GetOwnsAdvancedVersion())
		{
			vibrationData = this.abilityData.linkedAdvancedVersion.primaryVibration;
		}
		GameManager.Instance.VibrationManager.Vibrate(vibrationData, VibrationRegion.WholeBody, true);
		return flag;
	}

	private void DoActivate()
	{
		this.playerRef.BoatModelProxy.SetLightStrength(4f);
		for (int i = 0; i < this.playerRef.BoatModelProxy.Lights.Length; i++)
		{
			this.playerRef.BoatModelProxy.Lights[i].SetActive(true);
		}
		for (int j = 0; j < this.playerRef.BoatModelProxy.LightBeams.Length; j++)
		{
			this.playerRef.BoatModelProxy.LightBeams[j].SetActive(true);
		}
		this.sanityModifier.SetActive(true);
	}

	public override void Deactivate()
	{
		if (this.Locked)
		{
			return;
		}
		if (this.isActive)
		{
			this.playerRef.BoatModelProxy.SetLightStrength(0f);
			for (int i = 0; i < this.playerRef.BoatModelProxy.Lights.Length; i++)
			{
				this.playerRef.BoatModelProxy.Lights[i].SetActive(false);
			}
			for (int j = 0; j < this.playerRef.BoatModelProxy.LightBeams.Length; j++)
			{
				this.playerRef.BoatModelProxy.LightBeams[j].SetActive(false);
			}
		}
		this.sanityModifier.SetActive(false);
		base.Deactivate();
	}

	[SerializeField]
	private Player playerRef;

	[SerializeField]
	private GameObject sanityModifier;
}
