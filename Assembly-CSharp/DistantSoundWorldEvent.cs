using System;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

public class DistantSoundWorldEvent : WorldEvent
{
	public override void Activate()
	{
		base.Activate();
		Vector2 vector = global::UnityEngine.Random.insideUnitCircle.normalized * this.distance;
		Vector3 position = GameManager.Instance.Player.transform.position;
		base.transform.position = new Vector3(position.x + vector.x, 0f, position.z + vector.y);
		GameManager.Instance.AudioPlayer.PlaySFX(this.assetReference, base.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, this.distance, this.distance * 2f, false, true);
		if (this.vibrationData != null)
		{
			GameManager.Instance.VibrationManager.Vibrate(this.vibrationData, VibrationRegion.WholeBody, true).Run();
		}
		this.RequestEventFinish();
	}

	public override void RequestEventFinish()
	{
		base.RequestEventFinish();
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private AssetReference assetReference;

	[SerializeField]
	private float distance;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private VibrationData vibrationData;
}
