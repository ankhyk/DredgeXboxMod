using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;

public class FinaleCutsceneLogic : MonoBehaviour
{
	private void Update()
	{
		Shader.SetGlobalFloat("_FogRemove", this.fogRemove);
		if (this.shouldBumpSteepness)
		{
			Shader.SetGlobalFloat("_WaveSteepness", GameManager.Instance.WaveController.steepness + this.waveBump);
		}
	}

	private void Start()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler("PlayBadFinaleCutscene", new Action(this.PlayBadFinaleCutscene));
		GameManager.Instance.DialogueRunner.AddCommandHandler("PlayGoodFinaleCutscene", new Action(this.PlayGoodFinaleCutscene));
		GameManager.Instance.DialogueRunner.AddCommandHandler("UnlockPlayerMovement", new Action(this.UnlockPlayerMovement));
	}

	private void PlayBadFinaleCutscene()
	{
		GameManager.Instance.Player.IgnoreDamage();
		GameEvents.Instance.TriggerCutsceneToggled(true);
		this.director.playableAsset = this.badFinalePlayable;
		this.director.enabled = true;
		AutoSplitterData.isRunning = 2;
	}

	private void PlayGoodFinaleCutscene()
	{
		GameManager.Instance.Player.IgnoreDamage();
		GameEvents.Instance.TriggerCutsceneToggled(true);
		this.director.playableAsset = this.goodFinalePlayable;
		this.director.enabled = true;
		AutoSplitterData.isRunning = 2;
	}

	private void UnlockPlayerMovement()
	{
		GameManager.Instance.Player.Controller.ClearAutoMoveTarget();
		GameManager.Instance.Player.Controller.ClearAutoRotateTarget();
	}

	public void TransitionToWeather(string weatherName)
	{
		GameManager.Instance.WeatherController.TransitionDurationSec = 10f;
		GameManager.Instance.WeatherController.ChangeWeather(weatherName);
	}

	public void ToggleGameUI(bool enabled)
	{
		GameManager.Instance.UI.ToggleGameUI(enabled);
	}

	public void ToggleBoatModel(bool enabled)
	{
		GameManager.Instance.Player.ToggleBoatModel(enabled);
	}

	public void FlickerBoatLights(bool enableAfterFinish)
	{
		GameManager.Instance.Player.BoatModelProxy.LightFlickerEffect.BeginFlicker(this.lightFlickerCurve, this.lightFlickerDurationSec, enableAfterFinish);
	}

	public void ToggleScrim(bool enabled)
	{
		GameManager.Instance.UI.ToggleHUDCoverScrim(enabled, enabled ? this.scrimAppearDurationSec : this.scrimDisppearDurationSec);
	}

	public void ToggleEngineAudio(bool enabled)
	{
		GameManager.Instance.Player.ToggleBoatEngineAudio(enabled);
	}

	public void ToggleFinaleAudioSnapshot(float durationSec)
	{
		GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.MUSIC_ONLY, durationSec);
	}

	public void ToggleRegularAudioSnapshot(float durationSec)
	{
		GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.UNDOCKED, durationSec);
	}

	public void PlayGoodCreditsMusic()
	{
		GameManager.Instance.AudioPlayer.PlayMusic(this.goodCreditsTrackReference, AudioLayer.MUSIC_STINGER);
	}

	public void PlayBadCreditsMusic()
	{
		GameManager.Instance.AudioPlayer.PlayMusic(this.badCreditsTrackReference, AudioLayer.MUSIC_STINGER);
	}

	public void ToggleLighthousePointing(bool point)
	{
		this.lighthouseBeam.TogglePointToFinalePOI(point);
	}

	public void DestroyGreaterMarrow()
	{
		this.existingMarrowsObjects.ForEach(delegate(GameObject o)
		{
			o.SetActive(false);
		});
		global::UnityEngine.Object.Instantiate<GameObject>(this.ruinedMarrowsPrefab, Vector3.zero, Quaternion.identity);
	}

	public void OnVibrationType1SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType1);
	}

	public void OnVibrationType2SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType2);
	}

	public void OnVibrationType3SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType3);
	}

	public void OnVibrationType4SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType4);
	}

	public void OnVibrationType5SignalReceived()
	{
		this.OnVibrationSignalReceived(this.vibrationType5);
	}

	private void OnVibrationSignalReceived(VibrationData vibrationData)
	{
		GameManager.Instance.VibrationManager.Vibrate(vibrationData, VibrationRegion.WholeBody, true);
	}

	public void CutToCredits()
	{
		this.creditsVCam.enabled = true;
		this.UnlockPlayerMovement();
		GameManager.Instance.Player.transform.position = this.creditsPlayerPosition.position;
		global::UnityEngine.Object.Instantiate<GameObject>(this.creditsPrefab).GetComponent<CreditsController>().SetCreditsMode(CreditsMode.SHOWING_IN_GAME);
	}

	[SerializeField]
	private AnimationCurve lightFlickerCurve;

	[SerializeField]
	private float lightFlickerDurationSec;

	[SerializeField]
	private LighthouseBeam lighthouseBeam;

	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private PlayableAsset badFinalePlayable;

	[SerializeField]
	private PlayableAsset goodFinalePlayable;

	[SerializeField]
	private GameObject creditsPrefab;

	[SerializeField]
	private CinemachineVirtualCamera creditsVCam;

	[SerializeField]
	private float scrimAppearDurationSec;

	[SerializeField]
	private float scrimDisppearDurationSec;

	[SerializeField]
	private Transform creditsPlayerPosition;

	[SerializeField]
	private AssetReference goodCreditsTrackReference;

	[SerializeField]
	private AssetReference badCreditsTrackReference;

	[SerializeField]
	private List<GameObject> existingMarrowsObjects;

	[SerializeField]
	private GameObject ruinedMarrowsPrefab;

	[SerializeField]
	private bool shouldBumpSteepness;

	[Range(0f, 1f)]
	[SerializeField]
	private float waveBump;

	[Range(0f, 1f)]
	[SerializeField]
	private float fogRemove;

	[Header("Vibration Data")]
	[SerializeField]
	private VibrationData vibrationType1;

	[SerializeField]
	private VibrationData vibrationType2;

	[SerializeField]
	private VibrationData vibrationType3;

	[SerializeField]
	private VibrationData vibrationType4;

	[SerializeField]
	private VibrationData vibrationType5;
}
