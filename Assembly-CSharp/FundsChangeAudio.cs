using System;
using UnityEngine;

public class FundsChangeAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerFundsChanged += this.OnPlayerFundsChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerFundsChanged -= this.OnPlayerFundsChanged;
	}

	private void OnPlayerFundsChanged(decimal total, decimal change)
	{
		if (change > 0m && this.increaseClips.Length != 0)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.increaseClips[global::UnityEngine.Random.Range(0, this.increaseClips.Length)], AudioLayer.SFX_UI, 1f, 1f);
			return;
		}
		if (change < 0m && this.decreaseClips.Length != 0)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.decreaseClips[global::UnityEngine.Random.Range(0, this.decreaseClips.Length)], AudioLayer.SFX_UI, 1f, 1f);
		}
	}

	[SerializeField]
	private AudioClip[] increaseClips;

	[SerializeField]
	private AudioClip[] decreaseClips;
}
