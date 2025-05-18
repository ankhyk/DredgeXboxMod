using System;
using UnityEngine;

public class AttachedMonsterAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnMonsterAttachedToPlayer += this.OnMonsterAttachedToPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnMonsterAttachedToPlayer -= this.OnMonsterAttachedToPlayer;
	}

	private void OnMonsterAttachedToPlayer()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.latchOnSFX, AudioLayer.SFX_PLAYER, 1f, 1f + (float)GameManager.Instance.PlayerStats.AttachedMonsterCount * 0.05f);
	}

	private void Update()
	{
		this.currentVolumeTarget = Mathf.Lerp(this.currentVolumeTarget, (1f - GameManager.Instance.PlayerStats.AttachedMonsterMovementSpeedFactor) * this.maxVolume, Time.deltaTime);
		this.attachedMonsterLoopingAudioSource.volume = this.currentVolumeTarget;
		if (this.attachedMonsterLoopingAudioSource.volume < Mathf.Epsilon && this.currentVolumeTarget < Mathf.Epsilon)
		{
			if (this.attachedMonsterLoopingAudioSource.isPlaying)
			{
				this.attachedMonsterLoopingAudioSource.Stop();
				return;
			}
		}
		else if (!this.attachedMonsterLoopingAudioSource.isPlaying)
		{
			this.attachedMonsterLoopingAudioSource.Play();
		}
	}

	[SerializeField]
	private AudioSource attachedMonsterLoopingAudioSource;

	[SerializeField]
	private AudioSource attachedMonsterAudioSource;

	[SerializeField]
	private AudioClip latchOnSFX;

	[SerializeField]
	private float maxVolume;

	private float currentVolumeTarget;
}
