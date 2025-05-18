using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerEngineAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnUpgradesChanged += this.OnUpgradesChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnUpgradesChanged -= this.OnUpgradesChanged;
	}

	private void Start()
	{
		this.shouldUpdateOoze = GameManager.Instance.OozePatchManager;
		if (this.shouldUpdateOoze)
		{
			this.oozeAudioSource.Play();
			return;
		}
		this.oozeAudioSource.Stop();
	}

	private void OnUpgradesChanged(UpgradeData upgradeData)
	{
		if (upgradeData != null && upgradeData is HullUpgradeData)
		{
			AssetReference engineAudioClip = (upgradeData as HullUpgradeData).engineAudioClip;
			if (engineAudioClip != null && engineAudioClip.RuntimeKeyIsValid())
			{
				Addressables.LoadAssetAsync<AudioClip>(engineAudioClip).Completed += delegate(AsyncOperationHandle<AudioClip> op)
				{
					if (op.Status == AsyncOperationStatus.Succeeded)
					{
						this.audioSource.clip = op.Result;
						this.audioSource.Play();
					}
				};
			}
		}
	}

	private void Awake()
	{
		this.audioSource.pitch = this.minPitch;
		this.audioSource.volume = this.minVolume;
		this.OnUpgradesChanged(GameManager.Instance.UpgradeManager.GetHullUpgradeDataByTier(GameManager.Instance.SaveData.HullTier));
	}

	private void Update()
	{
		if (this.playerControllerRef.IsAutoMoving)
		{
			this.UpdateEngineSound(this.playerControllerRef.AutoMoveSpeed * this.autoMoveVelocityModifier, this.autoMovePitchModifier);
		}
		else
		{
			this.UpdateEngineSound(this.playerControllerRef.Velocity.magnitude, 1f);
		}
		if (this.shouldUpdateOoze)
		{
			this.UpdateOozeSound(GameManager.Instance.OozePatchManager.isOozeNearToPlayer, this.playerControllerRef.Velocity.magnitude, 1f);
		}
	}

	private void UpdateEngineSound(float velocity, float pitchModifier = 1f)
	{
		if (GameManager.Instance.PlayerStats.AttachedMonsterCount > 0)
		{
			this.attachedMonsterLoopingAudioSource.volume = 1f;
			this.audioSource.volume = 0f;
			return;
		}
		this.attachedMonsterLoopingAudioSource.volume = GameManager.Instance.Player.Controller.InputMagnitude * 0f;
		float num = Mathf.Clamp01(velocity * this.velocityModifier);
		this.audioSource.pitch = Mathf.Lerp(this.minPitch, this.maxPitch, num) * pitchModifier;
		this.audioSource.volume = Mathf.Lerp(this.minVolume, this.maxVolume, num);
	}

	private void UpdateOozeSound(bool isInOoze, float velocity, float pitchModifier = 1f)
	{
		float num = 0f;
		if (isInOoze)
		{
			float num2 = Mathf.Clamp01(velocity * this.oozeVelocityModifier);
			this.oozeAudioSource.pitch = Mathf.Lerp(this.oozeMinPitch, this.oozeMaxPitch, num2) * pitchModifier;
			num = Mathf.Lerp(this.oozeMinVolume, this.oozeMaxVolume, num2);
		}
		this.oozeAudioSource.volume = Mathf.Lerp(this.oozeAudioSource.volume, num, Time.deltaTime * this.oozeChangeModifier);
	}

	[SerializeField]
	private PlayerController playerControllerRef;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioSource attachedMonsterLoopingAudioSource;

	[SerializeField]
	private AudioSource oozeAudioSource;

	[SerializeField]
	private float velocityModifier;

	[SerializeField]
	private float minPitch;

	[SerializeField]
	private float maxPitch;

	[SerializeField]
	private float minVolume;

	[SerializeField]
	private float maxVolume;

	[SerializeField]
	private float oozeMinPitch;

	[SerializeField]
	private float oozeMaxPitch;

	[SerializeField]
	private float oozeMinVolume;

	[SerializeField]
	private float oozeMaxVolume;

	[SerializeField]
	private float oozeChangeModifier;

	[SerializeField]
	private float oozeVelocityModifier;

	[SerializeField]
	private float autoMoveVelocityModifier;

	[SerializeField]
	private float autoMovePitchModifier;

	private bool shouldUpdateOoze;
}
