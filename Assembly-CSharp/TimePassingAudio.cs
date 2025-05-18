using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TimePassingAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
	}

	private void OnTimeForcefullyPassingChanged(bool isPassing, string reason, TimePassageMode mode)
	{
		if (isPassing)
		{
			this.loopAudioSource.Play();
			return;
		}
		this.loopAudioSource.Stop();
		GameManager.Instance.AudioPlayer.PlaySFX(this.completeAudioReference, AudioLayer.SFX_UI, 1f, 1f);
	}

	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private AssetReference completeAudioReference;
}
