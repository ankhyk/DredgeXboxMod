using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

[DefaultExecutionOrder(-850)]
public class AudioPlayer : MonoBehaviour
{
	public AudioMixer AudioMixer { get; set; }

	public bool IsMixerLoaded { get; set; }

	public bool IsPlayingStinger
	{
		get
		{
			return this.audioSourceStingerMusic.isPlaying;
		}
	}

	public SnapshotType CurrentSnapshotType
	{
		get
		{
			return this.currentSnapshotType;
		}
	}

	public DockAudio DockAudio
	{
		get
		{
			return this.dockAudio;
		}
		set
		{
			this.dockAudio = value;
		}
	}

	private void Awake()
	{
		Addressables.LoadAssetAsync<AudioMixer>(this.mixerAssetRef).Completed += delegate(AsyncOperationHandle<AudioMixer> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				this.AudioMixer = handle.Result;
				this.IsMixerLoaded = true;
				Action onMixerLoaded = this.OnMixerLoaded;
				if (onMixerLoaded == null)
				{
					return;
				}
				onMixerLoaded();
			}
		};
		GameManager.Instance.AudioPlayer = this;
	}

	private void OnEnable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Combine(instance.OnGameEnded, new Action(this.OnGameEnded));
	}

	private void OnDisable()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Remove(instance.OnGameEnded, new Action(this.OnGameEnded));
	}

	private void OnGameEnded()
	{
		this.TransitionToSnapshot(SnapshotType.MENU, 0.5f);
	}

	public void SetIsButtonHeld(bool isHeld)
	{
		if (isHeld == this.isButtonHeld)
		{
			return;
		}
		this.isButtonHeld = isHeld;
		if (isHeld && !this.audioSourceHoldUISFX.isPlaying)
		{
			this.audioSourceHoldUISFX.Play();
			return;
		}
		if (!isHeld && this.audioSourceHoldUISFX.isPlaying)
		{
			this.audioSourceHoldUISFX.Stop();
		}
	}

	public void OnHoldActionComplete()
	{
		this.audioSourceHoldUICompleteSFX.Play();
	}

	public void PlayMusic(AudioClip clip, AudioLayer musicLayer)
	{
		if (!this.IsMixerLoaded)
		{
			return;
		}
		if (musicLayer == AudioLayer.MUSIC_WORLD)
		{
			this.PlayMusic(clip);
		}
		if (musicLayer == AudioLayer.MUSIC_STINGER)
		{
			this.PlayStinger(clip);
		}
	}

	public void RequestStingerStop()
	{
		this.isStingerStopping = true;
		DOTween.Kill(this.stingerFadeTween, true);
		this.stingerFadeTween = this.audioSourceStingerMusic.DOFade(0f, this.musicFadeDownTimeSec).OnComplete(delegate
		{
			this.audioSourceStingerMusic.volume = 0f;
			this.audioSourceStingerMusic.Stop();
			this.isStingerStopping = false;
			if (this.queuedMusicClip)
			{
				this.PlayMusic(this.queuedMusicClip);
			}
			this.stingerFadeTween = null;
		}).SetUpdate(true);
	}

	public void RequestMusicStop()
	{
		this.isMusicStopping = true;
		DOTween.Kill(this.musicFadeTween, false);
		this.musicFadeTween = this.audioSourceWorldMusic.DOFade(0f, this.musicFadeDownTimeSec).OnComplete(delegate
		{
			this.audioSourceWorldMusic.Stop();
			this.isMusicStopping = false;
			if (this.queuedMusicClip)
			{
				this.PlayMusic(this.queuedMusicClip);
			}
			this.musicFadeTween = null;
		}).SetUpdate(true);
	}

	private void PlayStinger(AudioClip stingerClip)
	{
		if (stingerClip == null)
		{
			return;
		}
		if (this.audioSourceWorldMusic.isPlaying)
		{
			return;
		}
		this.isStingerStopping = false;
		this.audioSourceStingerMusic.volume = this.maxMusicVolume;
		this.audioSourceStingerMusic.clip = stingerClip;
		this.audioSourceStingerMusic.Play();
	}

	private void PlayMusic(AudioClip musicClip)
	{
		if (musicClip == null)
		{
			return;
		}
		this.queuedMusicClip = null;
		if (this.audioSourceStingerMusic.isPlaying)
		{
			this.RequestStingerStop();
			this.queuedMusicClip = musicClip;
			return;
		}
		if (!this.audioSourceWorldMusic.isPlaying)
		{
			this.audioSourceWorldMusic.volume = this.maxMusicVolume;
			this.audioSourceWorldMusic.clip = musicClip;
			this.audioSourceWorldMusic.Play();
			return;
		}
		if (this.audioSourceWorldMusic.clip == musicClip)
		{
			DOTween.Kill(this.audioSourceWorldMusic, false);
			this.audioSourceWorldMusic.DOFade(this.maxMusicVolume, 1f);
			return;
		}
		this.RequestMusicStop();
		this.queuedMusicClip = musicClip;
	}

	public void PlaySFX(AudioClip clip, AudioLayer audioLayer, float volumeScale = 1f, float pitch = 1f)
	{
		if (!this.IsMixerLoaded)
		{
			return;
		}
		switch (audioLayer)
		{
		case AudioLayer.SFX_PLAYER:
			this.audioSourcePlayerSFX.pitch = pitch;
			this.audioSourcePlayerSFX.PlayOneShot(clip, volumeScale);
			return;
		case AudioLayer.SFX_WORLD:
			this.audioSourceWorldSFX.pitch = pitch;
			this.audioSourceWorldSFX.PlayOneShot(clip, volumeScale);
			return;
		case AudioLayer.SFX_UI:
			this.audioSourceUISFX.pitch = pitch;
			this.audioSourceUISFX.PlayOneShot(clip, volumeScale);
			return;
		case AudioLayer.SFX_VOCALS:
			this.audioSourceVocalSFX.pitch = pitch;
			this.audioSourceVocalSFX.PlayOneShot(clip, volumeScale);
			return;
		default:
			return;
		}
	}

	public void PlaySFX(string clipName, AudioLayer audioLayer, float volumeScale = 1f, float pitch = 1f)
	{
		Addressables.LoadAssetAsync<AudioClip>(clipName).Completed += delegate(AsyncOperationHandle<AudioClip> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				this.PlaySFX(op.Result, audioLayer, volumeScale, pitch);
			}
		};
	}

	public void PlaySFX(AssetReference assetReference, AudioLayer audioLayer, float volumeScale = 1f, float pitch = 1f)
	{
		if (assetReference == null || !assetReference.RuntimeKeyIsValid())
		{
			return;
		}
		Addressables.LoadAssetAsync<AudioClip>(assetReference).Completed += delegate(AsyncOperationHandle<AudioClip> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				this.PlaySFX(op.Result, audioLayer, volumeScale, pitch);
			}
		};
	}

	public void PlaySFX(AudioClip clip, Vector3 position, float volumeScale, AudioMixerGroup audioMixerGroup, AudioRolloffMode mode, float min, float max, bool use2DVolume, bool ignorePause)
	{
		AudioUtil.PlayClipAtPoint(clip, position, volumeScale, audioMixerGroup, mode, min, max, use2DVolume, ignorePause);
	}

	public void PlaySFX(AssetReference assetReference, Vector3 position, float volumeScale, AudioMixerGroup audioMixerGroup, AudioRolloffMode mode, float min, float max, bool use2DVolume, bool ignorePause)
	{
		if (assetReference == null || !assetReference.RuntimeKeyIsValid())
		{
			return;
		}
		Addressables.LoadAssetAsync<AudioClip>(assetReference).Completed += delegate(AsyncOperationHandle<AudioClip> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				AudioUtil.PlayClipAtPoint(op.Result, position, volumeScale, audioMixerGroup, mode, min, max, use2DVolume, ignorePause);
			}
		};
	}

	public void PlayMusic(AssetReference assetReference, AudioLayer audioLayer)
	{
		Addressables.LoadAssetAsync<AudioClip>(assetReference).Completed += delegate(AsyncOperationHandle<AudioClip> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded)
			{
				this.PlayMusic(op.Result, audioLayer);
			}
		};
	}

	public void PlayLoopingDialogueAudio(string clipName, AudioLayer audioLayer, float volumeScale = 1f)
	{
		Addressables.LoadAssetAsync<AudioClip>(clipName).Completed += delegate(AsyncOperationHandle<AudioClip> op)
		{
			if (op.Status == AsyncOperationStatus.Succeeded && this.audioSourceDialogueLoopSFX.clip != op.Result)
			{
				DOTween.Kill(this.audioSourceDialogueLoopSFX, false);
				this.audioSourceDialogueLoopSFX.clip = op.Result;
				this.audioSourceDialogueLoopSFX.DOFade(this.loopingDialogueVolume, this.loopingDialogueFadeDuration);
				this.audioSourceDialogueLoopSFX.Play();
			}
		};
	}

	public void StopLoopingDialogueAudio()
	{
		if (this.audioSourceDialogueLoopSFX.isPlaying)
		{
			this.audioSourceDialogueLoopSFX.DOFade(0f, this.loopingDialogueFadeDuration).OnComplete(delegate
			{
				this.audioSourceDialogueLoopSFX.Stop();
				this.audioSourceDialogueLoopSFX.clip = null;
			});
		}
	}

	public void RestartWorldMusic()
	{
		this.audioSourceWorldMusic.time = 0f;
	}

	public void TransitionToSnapshot(SnapshotType snapshotType, float durationSec)
	{
		AudioMixerSnapshot audioMixerSnapshot;
		switch (snapshotType)
		{
		case SnapshotType.MUSIC_ONLY:
			audioMixerSnapshot = this.musicOnlySnapshot;
			goto IL_0065;
		case SnapshotType.DOCKED_INDOORS:
			audioMixerSnapshot = this.indoorDockedSnapshot;
			goto IL_0065;
		case SnapshotType.DOCKED_OUTDOORS:
			audioMixerSnapshot = this.outdoorDockedSnapshot;
			goto IL_0065;
		case SnapshotType.MENU:
			audioMixerSnapshot = this.menuSnapshot;
			goto IL_0065;
		case SnapshotType.LOADING:
			audioMixerSnapshot = this.loadingSnapshot;
			goto IL_0065;
		case SnapshotType.CUTSCENE:
			audioMixerSnapshot = this.cutsceneSnapshot;
			goto IL_0065;
		}
		audioMixerSnapshot = this.undockedSnapshot;
		IL_0065:
		this.currentSnapshotType = snapshotType;
		audioMixerSnapshot.TransitionTo(durationSec);
	}

	[SerializeField]
	private AudioSource audioSourceStingerMusic;

	[SerializeField]
	private AudioSource audioSourceWorldMusic;

	[SerializeField]
	private AudioSource audioSourcePlayerSFX;

	[SerializeField]
	private AudioSource audioSourceWorldSFX;

	[SerializeField]
	private AudioSource audioSourceUISFX;

	[SerializeField]
	private AudioSource audioSourceVocalSFX;

	[SerializeField]
	private AudioSource audioSourceHoldUISFX;

	[SerializeField]
	private AudioSource audioSourceHoldUICompleteSFX;

	[SerializeField]
	private AudioSource audioSourceDialogueLoopSFX;

	[SerializeField]
	private float maxMusicVolume;

	[SerializeField]
	private float musicFadeDownTimeSec;

	[SerializeField]
	private float loopingDialogueVolume;

	[SerializeField]
	private float loopingDialogueFadeDuration;

	[SerializeField]
	private AssetReference mixerAssetRef;

	[SerializeField]
	private AudioMixerSnapshot loadingSnapshot;

	[SerializeField]
	private AudioMixerSnapshot menuSnapshot;

	[SerializeField]
	private AudioMixerSnapshot undockedSnapshot;

	[SerializeField]
	private AudioMixerSnapshot musicOnlySnapshot;

	[SerializeField]
	private AudioMixerSnapshot indoorDockedSnapshot;

	[SerializeField]
	private AudioMixerSnapshot outdoorDockedSnapshot;

	[SerializeField]
	private AudioMixerSnapshot cutsceneSnapshot;

	private DockAudio dockAudio;

	private SnapshotType currentSnapshotType;

	public Action OnMixerLoaded;

	private bool isStingerStopping;

	private bool isMusicStopping;

	private AudioClip queuedMusicClip;

	private Tweener stingerFadeTween;

	private Tweener musicFadeTween;

	private bool isButtonHeld;
}
