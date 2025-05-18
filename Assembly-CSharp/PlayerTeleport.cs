using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerTeleport : MonoBehaviour
{
	public void Teleport(Vector3 targetPos, float sanityChange, AbilityData abilityToUse = null)
	{
		this.effect.SetActive(false);
		this.effect.SetActive(true);
		GameManager.Instance.Player.BoatModelProxy.gameObject.SetActive(false);
		GameManager.Instance.VibrationManager.Vibrate(this.primaryVibration, VibrationRegion.WholeBody, true);
		GameManager.Instance.Player.IsImmuneModeEnabled = true;
		GameManager.Instance.AudioPlayer.PlaySFX(this.castSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
		base.StartCoroutine(this.DoTeleport(targetPos, sanityChange, abilityToUse));
	}

	private IEnumerator DoTeleport(Vector3 targetPos, float sanityChange, AbilityData abilityToUse = null)
	{
		GameEvents.Instance.TriggerTeleportBegin();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.NONE);
		GameManager.Instance.Player.Controller.IsMovementAllowed = false;
		this.teleportVCam.enabled = true;
		Vector3 waveDisplacement = WaveDisplacement.GetWaveDisplacement(targetPos, GameManager.Instance.WaveController.Steepness, GameManager.Instance.WaveController.Wavelength, GameManager.Instance.WaveController.Speed, GameManager.Instance.WaveController.Directions);
		targetPos += waveDisplacement;
		BuoyantObject componentInChildren = GameManager.Instance.Player.GetComponentInChildren<BuoyantObject>();
		if (componentInChildren)
		{
			targetPos.y = -componentInChildren.objectDepth;
		}
		Vector3 positionDelta = targetPos - GameManager.Instance.Player.transform.position;
		yield return new WaitForSeconds(this.preHoldTimeSec);
		this.teleportVCam.OnTargetObjectWarped(this.teleportVCam.m_Follow.gameObject.transform, positionDelta);
		this.teleportVCam.enabled = false;
		GameManager.Instance.Player.transform.position = targetPos;
		this.teleportVCam.enabled = true;
		GameManager.Instance.Player.Sanity.ChangeSanity(sanityChange);
		yield return new WaitForSeconds(this.holdTimeSec);
		GameManager.Instance.Player.BoatModelProxy.gameObject.SetActive(true);
		GameManager.Instance.VibrationManager.Vibrate(this.secondaryVibration, VibrationRegion.WholeBody, true);
		GameManager.Instance.Player.Controller.IsMovementAllowed = true;
		GameManager.Instance.Player.IsImmuneModeEnabled = false;
		if (abilityToUse)
		{
			GameManager.Instance.PlayerAbilities.DeactivateAbility(abilityToUse);
		}
		this.teleportVCam.enabled = false;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
		GameEvents.Instance.TriggerTeleportComplete();
		yield break;
	}

	[SerializeField]
	private CinemachineVirtualCamera teleportVCam;

	[SerializeField]
	private float preHoldTimeSec;

	[SerializeField]
	private float holdTimeSec;

	[SerializeField]
	public VibrationData primaryVibration;

	[SerializeField]
	public VibrationData secondaryVibration;

	[SerializeField]
	public AssetReference castSFX;

	[SerializeField]
	private GameObject effect;
}
