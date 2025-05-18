using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class BanishMachine : MonoBehaviour
{
	private void Start()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler("ActivateBanishMachine", new Action(this.ActivateBanishMachine));
		this.isBanishMachineActive = GameManager.Instance.Time.TimeAndDay < GameManager.Instance.SaveData.BanishMachineExpiry;
		if (GameManager.Instance.Time.TimeAndDay < GameManager.Instance.SaveData.BanishMachineExpiry)
		{
			this.ToggleBanishMachine(true);
		}
	}

	private void ActivateBanishMachine()
	{
		GameManager.Instance.SaveData.BanishMachineExpiry = GameManager.Instance.Time.TimeAndDay + GameManager.Instance.GameConfigData.BanishMachineDurationDays;
		this.ToggleBanishMachine(true);
	}

	private void ToggleBanishMachine(bool enabled)
	{
		this.isBanishMachineActive = enabled;
		this.toggleObjects.ForEach(delegate(GameObject o)
		{
			o.SetActive(enabled);
		});
		GameEvents.Instance.TriggerBanishMachineToggled(enabled);
		if (enabled)
		{
			DOTween.Kill(this.audioSource, false);
			this.audioSource.volume = this.volumeMax;
			this.audioSource.Play();
			return;
		}
		this.audioSource.DOFade(0f, this.volumeFadeTime).OnComplete(delegate
		{
			this.audioSource.Stop();
		});
	}

	private void Update()
	{
		if (this.isBanishMachineActive && GameManager.Instance.Time.TimeAndDay > GameManager.Instance.SaveData.BanishMachineExpiry)
		{
			this.ToggleBanishMachine(false);
		}
	}

	[SerializeField]
	private List<GameObject> toggleObjects;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float volumeMax;

	[SerializeField]
	private float volumeFadeTime;

	private bool isBanishMachineActive;
}
