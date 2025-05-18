using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerCollisionAudio : MonoBehaviour
{
	public void PlayRandom()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.clipRefs.PickRandom<AssetReference>(), AudioLayer.SFX_PLAYER, 0.5f, 1f);
	}

	public void PlayRandomSafe()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.safeClipRefs.PickRandom<AssetReference>(), AudioLayer.SFX_PLAYER, 0.5f, 1f);
	}

	[SerializeField]
	private AssetReference[] clipRefs;

	[SerializeField]
	private AssetReference[] safeClipRefs;
}
