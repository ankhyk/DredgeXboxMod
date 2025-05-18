using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class FoghornAbility : Ability
{
	public override void Init()
	{
		base.Init();
		this.advancedFoghornSonarParticleSystemMainModule = this.advancedFoghornSonarParticleSystem.main;
		this.RefreshFoghornStyle();
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnFoghornStyleChanged += this.RefreshFoghornStyle;
		this.hasAdvancedFoghorn = GameManager.Instance.SaveData.unlockedAbilities.Contains(this.abilityData.linkedAdvancedVersion.name);
		if (!this.hasAdvancedFoghorn)
		{
			GameEvents.Instance.OnPlayerAbilitiesChanged += this.OnPlayerAbilitiesChanged;
		}
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnFoghornStyleChanged -= this.RefreshFoghornStyle;
		GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
	}

	private void OnPlayerAbilitiesChanged(AbilityData abilityData)
	{
		if (abilityData.name == this.abilityData.linkedAdvancedVersion.name)
		{
			GameEvents.Instance.OnPlayerAbilitiesChanged -= this.OnPlayerAbilitiesChanged;
			this.hasAdvancedFoghorn = true;
		}
	}

	private void OnSonarCast()
	{
		base.StartCoroutine(this.DoSonarAtPosition(base.transform.position, false, 0f));
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.mainSonarSize, this.sonarLayerMask);
		float num = this.mainSonarSize / this.mainSonarLifetimeSec;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position = array[i].transform.position;
			position.y = base.transform.position.y;
			float num2 = Vector3.Distance(base.transform.position, position) / num;
			base.StartCoroutine(this.DoSonarAtPosition(position, true, num2));
		}
	}

	private IEnumerator DoSonarAtPosition(Vector3 position, bool isMini, float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		this.advancedFoghornSonarParticleSystemMainModule.startSize = (isMini ? this.miniSonarSize : this.mainSonarSize);
		this.advancedFoghornSonarParticleSystemMainModule.startLifetime = (isMini ? this.miniSonarLifetimeSec : this.mainSonarLifetimeSec);
		this.advancedFoghornSonarParticleSystem.transform.position = position;
		this.advancedFoghornSonarParticleSystem.Emit(1);
		GameManager.Instance.AudioPlayer.PlaySFX(isMini ? this.advancedFoghornPingAssetRef : this.advancedFoghornAssetRef, position, isMini ? 0.7f : 1f, this.audioMixerGroup, AudioRolloffMode.Logarithmic, 5f, 50f, false, false);
		VibrationData vibrationData = this.abilityData.linkedAdvancedVersion.primaryVibration;
		if (isMini)
		{
			vibrationData = this.abilityData.linkedAdvancedVersion.secondaryVibration;
		}
		GameManager.Instance.VibrationManager.Vibrate(vibrationData, VibrationRegion.WholeBody, true);
		yield break;
	}

	private void RefreshFoghornStyle()
	{
		this.pitchIndex = GameManager.Instance.SaveData.GetIntVariable("foghorn-style-index", 0);
		this.pitchStart = this.pitchValues[this.pitchIndex];
		this.pitchEnd = this.pitchStart - this.pitchChange;
	}

	public override bool Activate()
	{
		this.isActive = true;
		if (this.pitchTween != null)
		{
			this.pitchTween.Kill(false);
		}
		this.foghornMidSource.pitch = this.pitchStart;
		this.pitchTween = this.foghornMidSource.DOPitch(this.pitchEnd, this.fadeInSec);
		if (this.volumeTween != null)
		{
			this.volumeTween.Kill(false);
		}
		this.foghornMidSource.volume = this.volumeStart;
		this.volumeTween = this.foghornMidSource.DOFade(this.volumeEnd, this.fadeInSec);
		this.foghornMidSource.Play();
		if (this.hasAdvancedFoghorn && Time.time > this.timeOfLastAdvancedFoghornCast + this.delayBetweenAdvancedFoghornCast + this.mainSonarLifetimeSec)
		{
			this.timeOfLastAdvancedFoghornCast = Time.time;
			this.OnSonarCast();
		}
		base.Activate();
		return true;
	}

	public override void Deactivate()
	{
		if (base.IsActive)
		{
			this.foghornMidSource.Stop();
			this.foghornEndSource.pitch = this.pitchEnd;
			this.foghornEndSource.PlayOneShot(this.foghornEndClip);
		}
		base.Deactivate();
	}

	[SerializeField]
	private AudioSource foghornEndSource;

	[SerializeField]
	private AudioSource foghornMidSource;

	[SerializeField]
	private AudioClip foghornEndClip;

	[SerializeField]
	private AssetReference advancedFoghornAssetRef;

	[SerializeField]
	private AssetReference advancedFoghornPingAssetRef;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private float fadeInSec;

	[SerializeField]
	private float volumeStart;

	[SerializeField]
	private float volumeEnd;

	[SerializeField]
	private float pitchChange;

	[SerializeField]
	private List<float> pitchValues = new List<float>();

	[SerializeField]
	private ParticleSystem advancedFoghornSonarParticleSystem;

	[SerializeField]
	private ParticleSystem.MainModule advancedFoghornSonarParticleSystemMainModule;

	[SerializeField]
	private float delayBetweenAdvancedFoghornCast;

	[SerializeField]
	private float mainSonarLifetimeSec;

	[SerializeField]
	private float mainSonarSize;

	[SerializeField]
	private float miniSonarLifetimeSec;

	[SerializeField]
	private float miniSonarSize;

	[SerializeField]
	private LayerMask sonarLayerMask;

	private float pitchStart;

	private float pitchEnd;

	private int pitchIndex;

	private Tweener pitchTween;

	private Tweener volumeTween;

	private bool hasAdvancedFoghorn;

	private float timeOfLastAdvancedFoghornCast;
}
