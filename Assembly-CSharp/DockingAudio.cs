using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DockingAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (!GameSceneInitializer.Instance.IsDone())
		{
			return;
		}
		if (dock == null)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.undockingSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
			return;
		}
		GameManager.Instance.AudioPlayer.PlaySFX(this.dockingCompleteSFX, AudioLayer.SFX_PLAYER, 1f, 1f);
	}

	public void ToggleDockingLoop(bool play)
	{
		if (play && !this.loopAudioSource.isPlaying)
		{
			this.loopAudioSource.Play();
			return;
		}
		if (!play)
		{
			this.loopAudioSource.Stop();
		}
	}

	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private AssetReference undockingSFX;

	[SerializeField]
	private AssetReference dockingCompleteSFX;
}
