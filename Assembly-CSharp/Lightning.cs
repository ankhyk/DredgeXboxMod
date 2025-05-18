using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class Lightning : MonoBehaviour
{
	public void SetLightningDelay(float min, float max)
	{
		this.lightningDelayMin = min;
		this.lightningDelayMax = max;
		if (this.lightningDelayMin < Mathf.Epsilon || this.lightningDelayMax < Mathf.Epsilon)
		{
			this.isPlayingLightning = false;
			return;
		}
		this.isPlayingLightning = true;
		this.timeUntilNextStrike = global::UnityEngine.Random.Range(this.lightningDelayMin, this.lightningDelayMax);
	}

	private void Update()
	{
		if (!this.isPlayingLightning)
		{
			return;
		}
		this.timeUntilNextStrike -= Time.deltaTime;
		if (this.timeUntilNextStrike <= 0f)
		{
			this.Emit();
			this.timeUntilNextStrike = global::UnityEngine.Random.Range(this.lightningDelayMin, this.lightningDelayMax);
		}
	}

	public void Emit()
	{
		Vector2 vector = global::UnityEngine.Random.insideUnitCircle.normalized * global::UnityEngine.Random.Range(this.minRange, this.maxRange);
		Vector3 vector2 = Vector3.zero;
		if (this.lightningCenterProxy)
		{
			vector2 = this.lightningCenterProxy.position;
		}
		if (GameManager.Instance && GameManager.Instance.Player)
		{
			vector2 = GameManager.Instance.Player.transform.position;
		}
		base.transform.position = new Vector3(vector2.x + vector.x, 0f, vector2.z + vector.y);
		GameManager.Instance.AudioPlayer.PlaySFX(this.lightningSFXRefs.PickRandom<AssetReference>(), base.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, 50f, 250f, false, true);
		this.particles.Emit(default(ParticleSystem.EmitParams), 1);
		if (this.delayedThunderPlay != null)
		{
			base.StopCoroutine(this.delayedThunderPlay);
			this.delayedThunderPlay = null;
		}
		this.delayedThunderPlay = base.StartCoroutine(this.DelayedThunder(vector.magnitude));
	}

	private IEnumerator DelayedThunder(float distance)
	{
		yield return new WaitForSecondsRealtime(distance * this.thunderDelay);
		GameManager.Instance.AudioPlayer.PlaySFX(this.thunderSFXRefs.PickRandom<AssetReference>(), base.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, 50f, 250f, false, true);
		this.delayedThunderPlay = null;
		yield break;
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AssetReference> lightningSFXRefs;

	[SerializeField]
	private List<AssetReference> thunderSFXRefs;

	[SerializeField]
	private ParticleSystem particles;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private float minRange;

	[SerializeField]
	private float maxRange;

	[SerializeField]
	private float lightningDelayMin;

	[SerializeField]
	private float lightningDelayMax;

	[SerializeField]
	private float thunderDelay;

	public Transform lightningCenterProxy;

	private Coroutine delayedThunderPlay;

	private bool isPlayingLightning;

	private float timeUntilNextStrike;
}
