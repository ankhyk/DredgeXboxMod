using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCinematicLogic : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnGameLoaded += this.OnGameLoaded;
		ApplicationEvents.Instance.OnGameStartable += this.OnGameStartable;
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnGameLoaded -= this.OnGameLoaded;
		ApplicationEvents.Instance.OnGameStartable -= this.OnGameStartable;
	}

	private void OnGameLoaded()
	{
		if (GameManager.Instance.SaveData.GetBoolVariable("played-intro-cinematic", false))
		{
			this.virtualCamera.gameObject.SetActive(false);
			return;
		}
		this.virtualCamera.gameObject.SetActive(true);
		GameEvents.Instance.TriggerCutsceneToggled(true);
	}

	private void OnGameStartable()
	{
		if (GameManager.Instance.SaveData.GetBoolVariable("played-intro-cinematic", false))
		{
			GameManager.Instance.BeginGame();
			GameManager.Instance.UI.OnTopUIToggleRequested(true);
			return;
		}
		this.director.Play(this.introCinematicAsset);
	}

	public void OnIntroCutsceneEndSignal()
	{
		GameManager.Instance.SaveData.SetBoolVariable("played-intro-cinematic", true);
		GameManager.Instance.BeginGame();
		GameEvents.Instance.TriggerCutsceneToggled(false);
		GameEvents.Instance.TriggerTopUIToggleRequest(true);
	}

	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private PlayableAsset introCinematicAsset;

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;

	[SerializeField]
	private GameObject lookAtTarget;
}
