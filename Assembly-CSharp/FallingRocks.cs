using System;
using System.Collections;
using Cinemachine;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class FallingRocks : MonoBehaviour
{
	public GaleCliffsMonsterHole AssociatedHole
	{
		get
		{
			return this.associatedHole;
		}
	}

	public GaleCliffsIsland Island
	{
		get
		{
			return this.island;
		}
	}

	public void TriggerWarningDelayed(float delaySec)
	{
		base.StartCoroutine(this.DoTriggerWarningDelayed(delaySec));
	}

	private IEnumerator DoTriggerWarningDelayed(float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		this.TriggerWarning();
		yield break;
	}

	public void TriggerWarning()
	{
		this.warning.gameObject.SetActive(false);
		this.warning.gameObject.SetActive(true);
		this.warningImpulseSource.GenerateImpulse();
	}

	public void TriggerRockfallDelayed(float delaySec)
	{
		base.StartCoroutine(this.DoTriggerRockfallDelayed(delaySec));
	}

	private IEnumerator DoTriggerRockfallDelayed(float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		this.TriggerRockfall();
		yield break;
	}

	public void TriggerRockfall()
	{
		this.rockfallImpulseSource.GenerateImpulse();
		this.rockFall.gameObject.SetActive(false);
		this.rockfallCollider.transform.localEulerAngles = Vector3.zero;
		this.rockfallCollider.velocity = Vector3.zero;
		this.rockfallCollider.transform.localPosition = Vector3.zero;
		this.rockFall.gameObject.SetActive(true);
		GameManager.Instance.VibrationManager.Vibrate(this.rockFallVibration, VibrationRegion.WholeBody, true).Run();
	}

	[SerializeField]
	private GameObject rockFall;

	[SerializeField]
	private Rigidbody rockfallCollider;

	[SerializeField]
	private GameObject warning;

	[SerializeField]
	private GaleCliffsMonsterHole associatedHole;

	[SerializeField]
	private GaleCliffsIsland island;

	[SerializeField]
	private CinemachineImpulseSource warningImpulseSource;

	[SerializeField]
	private CinemachineImpulseSource rockfallImpulseSource;

	[SerializeField]
	private VibrationData rockFallVibration;
}
