using System;
using UnityEngine;

public class DSAltarFlame : MonoBehaviour
{
	private void Awake()
	{
		if (GameManager.Instance.DialogueRunner)
		{
			this.AddFunction();
			return;
		}
		ApplicationEvents.Instance.OnGameLoaded += this.AddFunction;
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnGameLoaded -= this.AddFunction;
	}

	private void AddFunction()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler<bool>("ToggleDSAltarFlame", new Action<bool>(this.ToggleDSAltarFlame));
	}

	private void ToggleDSAltarFlame(bool enable)
	{
		if (enable)
		{
			this.toggleObject.SetActive(true);
			return;
		}
		this.safeParticleDestroyer.Destroy();
	}

	[SerializeField]
	private GameObject toggleObject;

	[SerializeField]
	private SafeParticleDestroyer safeParticleDestroyer;
}
