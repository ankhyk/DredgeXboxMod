using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HasteVolume : MonoBehaviour
{
	private void Awake()
	{
		this.FetchAbility();
		if (!this.hasteAbility)
		{
			GameEvents.Instance.OnPlayerAbilityRegistered += this.OnPlayerAbilityRegistered;
		}
	}

	private void Start()
	{
		VolumeProfile profile = this.volume.profile;
		profile.TryGet<Vignette>(out this.vignette);
		if (this.vignette == null)
		{
			Debug.LogWarning("[PlayerCamera] Start() failed to retrieve vignette volume component.");
		}
		profile.TryGet<MotionBlur>(out this.motionBlur);
		if (this.motionBlur == null)
		{
			Debug.LogWarning("[PlayerCamera] Start() failed to retrieve motion blur volume component.");
		}
	}

	private void OnPlayerAbilityRegistered(AbilityData abilityData)
	{
		if (abilityData.name == this.hasteAbilityData.name)
		{
			this.FetchAbility();
			GameEvents.Instance.OnPlayerAbilityRegistered -= this.OnPlayerAbilityRegistered;
		}
	}

	private void FetchAbility()
	{
		this.hasteAbility = GameManager.Instance.PlayerAbilities.GetAbilityForData(this.hasteAbilityData) as BoostAbility;
	}

	private void Update()
	{
		if (!this.hasteAbility)
		{
			return;
		}
		this.smoothedBurnValue = Mathf.Lerp(this.smoothedBurnValue, this.useHasteVFX ? this.hasteAbility.CurrentBurnProp : 0f, Time.deltaTime);
		if (this.vignette)
		{
			this.vignette.intensity.Override(Mathf.Lerp(this.defaultVignetteIntensity, this.hasteVignetteIntensity, this.smoothedBurnValue));
		}
		if (this.motionBlur)
		{
			this.motionBlur.intensity.Override(Mathf.Lerp(this.defaultMotionBlurIntensity, this.hasteMotionBlurIntensity, this.smoothedBurnValue));
		}
	}

	[SerializeField]
	private Volume volume;

	[SerializeField]
	private AbilityData hasteAbilityData;

	[SerializeField]
	private float defaultVignetteIntensity;

	[SerializeField]
	private float defaultMotionBlurIntensity;

	[SerializeField]
	private float hasteVignetteIntensity;

	[SerializeField]
	private float hasteMotionBlurIntensity;

	private float smoothedBurnValue;

	private Vignette vignette;

	private MotionBlur motionBlur;

	private BoostAbility hasteAbility;

	public bool useHasteVFX = true;
}
