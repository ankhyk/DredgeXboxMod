using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BoostAbility : Ability
{
	public float CurrentBurnProp
	{
		get
		{
			return this.currentBurnProp;
		}
	}

	protected new void Awake()
	{
		this.hasteHeatCap = GameManager.Instance.GameConfigData.HasteHeatCap;
		this.hasteHeatGain = GameManager.Instance.GameConfigData.HasteHeatGain;
		this.hasteHeatLoss = GameManager.Instance.GameConfigData.HasteHeatLoss;
		this.hasteHeatCooldown = GameManager.Instance.GameConfigData.HasteHeatCooldown;
		this.shouldEvaluateAchievement = !GameManager.Instance.AchievementManager.GetAchievementState(DredgeAchievementId.ABILITY_HASTE);
		this.OnBoatModelChanged();
		base.Awake();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnBoatModelChanged += this.OnBoatModelChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnBoatModelChanged -= this.OnBoatModelChanged;
	}

	private void OnBoatModelChanged()
	{
		this.chimneyEmission = GameManager.Instance.Player.BoatModelProxy.ChimneySmoke.emission;
	}

	public override void Init()
	{
		base.Init();
	}

	public override bool Activate()
	{
		this.isActive = true;
		this.abilityHoldTime = 0f;
		this.playerSanityRef.AbilitySanityValue = this.sanityValue;
		this.heatSinkModifier = (float)GameManager.Instance.PlayerStats.HeatSinkGadgetModifier;
		this.mainSFX.Play();
		this.crackleSFX.Play();
		base.Activate();
		if (this.useHasteVFX)
		{
			this.kickoffImpulse.GenerateImpulse(Vector3.up);
		}
		return true;
	}

	public override void Deactivate()
	{
		this.playerControllerRef.AbilitySpeedModifier = 1f;
		this.playerSanityRef.AbilitySanityValue = 0f;
		this.mainSFX.Stop();
		base.Deactivate();
	}

	private void Update()
	{
		this.abilityHoldTime += Time.deltaTime;
		if (this.isOnCooldown)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.isOnCooldown = false;
				this.rawBurnAmount = 0f;
			}
		}
		else
		{
			if (this.isActive)
			{
				if (!GameManager.Instance.VibrationManager.IsPlayingVibration())
				{
					GameManager.Instance.VibrationManager.Vibrate(this.abilityData.primaryVibration, VibrationRegion.WholeBody, false);
				}
				float num = this.boostAmount.Evaluate(Mathf.Clamp01(this.abilityHoldTime)) * this.boostMagnitude;
				float num2 = this.burnAmount.Evaluate(Mathf.Clamp01(this.abilityHoldTime)) * this.boostMagnitude;
				this.rawBurnAmount += Time.deltaTime * (this.hasteHeatGain * (num2 / this.heatSinkModifier));
				this.playerControllerRef.AbilitySpeedModifier = num;
			}
			else
			{
				this.rawBurnAmount -= Time.deltaTime * this.hasteHeatLoss * 2f;
			}
			if (this.rawBurnAmount < 0f)
			{
				this.rawBurnAmount = 0f;
			}
			this.currentBurnProp = this.rawBurnAmount / this.hasteHeatCap;
			if (this.isActive)
			{
				this.chimneyEmission.rateOverTime = Mathf.Lerp(0f, this.chimneySmokeEmissionMax, Mathf.Pow(this.currentBurnProp, 2f));
			}
			else
			{
				this.chimneyEmission.rateOverTime = 0f;
			}
			if (this.currentBurnProp >= 1f)
			{
				if (this.useHasteVFX)
				{
					this.explodeImpulse.GenerateImpulse(Vector3.up);
				}
				Action onBurnExceeded = this.OnBurnExceeded;
				if (onBurnExceeded != null)
				{
					onBurnExceeded();
				}
				this.cooldownRemaining = this.hasteHeatCooldown;
				this.isOnCooldown = true;
				GameManager.Instance.AudioPlayer.PlaySFX(this.explosionClips.PickRandom<AudioClip>(), AudioLayer.SFX_PLAYER, 1f, 1f);
				GameManager.Instance.VibrationManager.Vibrate(this.abilityData.secondaryVibration, VibrationRegion.WholeBody, true);
				GameManager.Instance.GridManager.AddDamageToInventory(ItemType.EQUIPMENT, ItemSubtype.ENGINE);
			}
		}
		if (this.shouldEvaluateAchievement)
		{
			if (this.currentBurnProp >= this.achievementBurnThreshold)
			{
				this.currentAchievementBurnDuration += Time.deltaTime;
				if (this.currentAchievementBurnDuration >= this.achievementBurnDuration)
				{
					GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.ABILITY_HASTE, true);
					this.shouldEvaluateAchievement = false;
				}
			}
			else
			{
				this.currentAchievementBurnDuration = 0f;
			}
		}
		if (this.currentBurnProp > 0f || this.smoothedCrackleVolume > 0.01f)
		{
			this.targetCrackleVolume = this.crackleSFXCurve.Evaluate(this.currentBurnProp);
			this.smoothedCrackleVolume = Mathf.Lerp(this.smoothedCrackleVolume, this.targetCrackleVolume, Time.deltaTime * this.volumeBlendSpeed);
			if (!this.isActive && this.smoothedCrackleVolume <= 0.01f)
			{
				this.smoothedCrackleVolume = 0f;
				this.crackleSFX.Stop();
			}
			this.crackleSFX.volume = this.smoothedCrackleVolume;
		}
	}

	[SerializeField]
	private PlayerSanity playerSanityRef;

	[SerializeField]
	private PlayerController playerControllerRef;

	[SerializeField]
	private float sanityValue;

	[SerializeField]
	private float boostMagnitude;

	[SerializeField]
	private AnimationCurve boostAmount;

	[SerializeField]
	private AnimationCurve burnAmount;

	[SerializeField]
	private List<AudioClip> explosionClips;

	[SerializeField]
	private AudioSource crackleSFX;

	[SerializeField]
	private AudioSource mainSFX;

	[SerializeField]
	private AnimationCurve crackleSFXCurve;

	[SerializeField]
	private float volumeBlendSpeed;

	[SerializeField]
	private float chimneySmokeEmissionMax;

	[SerializeField]
	private float achievementBurnThreshold;

	[SerializeField]
	private float achievementBurnDuration;

	[SerializeField]
	private CinemachineImpulseSource kickoffImpulse;

	[SerializeField]
	private CinemachineImpulseSource explodeImpulse;

	[HideInInspector]
	public Action OnBurnExceeded;

	private float currentBurnProp;

	private float rawBurnAmount;

	private float cachedFov;

	private float hasteHeatCap;

	private float hasteHeatGain;

	private float hasteHeatLoss;

	private float hasteHeatCooldown;

	private float cooldownRemaining;

	private bool isOnCooldown;

	private float targetCrackleVolume;

	private float smoothedCrackleVolume;

	private float smoothedChimneySmokeEmission;

	private float abilityHoldTime;

	private bool shouldEvaluateAchievement;

	private float currentAchievementBurnDuration;

	public bool useHasteVFX = true;

	private ParticleSystem.EmissionModule chimneyEmission;

	private float heatSinkModifier;
}
