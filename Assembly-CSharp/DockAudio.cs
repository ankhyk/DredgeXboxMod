using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DockAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnTIRWorldPhaseChanged += this.OnTIRWorldPhaseChanged;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnTIRWorldPhaseChanged -= this.OnTIRWorldPhaseChanged;
	}

	private void OnTIRWorldPhaseChanged(int newPhase)
	{
		this.RefreshDockAudio(this.currentDock);
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		this.isDocked = dock != null;
		this.currentDock = dock;
		if (dock)
		{
			GameManager.Instance.AudioPlayer.RequestStingerStop();
			this.RefreshDockAudio(this.currentDock);
			GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_OUTDOORS, this.fadeInDuration);
			return;
		}
		GameManager.Instance.AudioPlayer.RequestMusicStop();
		GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.UNDOCKED, this.fadeOutDuration);
	}

	private void RefreshDockAudio(Dock dock)
	{
		if (dock == null)
		{
			return;
		}
		AssetReference assetReference = dock.Data.MusicAssetReference;
		AssetReferenceOverride assetReferenceOverride = dock.Data.MusicAssetOverrides.Find(delegate(AssetReferenceOverride o)
		{
			if (o.nodesVisited.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s)))
			{
				if (o.boolValues.All((string b) => GameManager.Instance.SaveData.GetBoolVariable(b, false)))
				{
					return GameManager.Instance.SaveData.TIRWorldPhase >= o.tirWorldPhase;
				}
			}
			return false;
		});
		if (assetReferenceOverride != null)
		{
			assetReference = assetReferenceOverride.assetReference;
		}
		if (assetReference != null && assetReference.RuntimeKeyIsValid())
		{
			GameManager.Instance.AudioPlayer.PlayMusic(assetReference, AudioLayer.MUSIC_WORLD);
		}
		AssetReference assetReference2 = dock.Data.AmbienceDayAssetReference;
		AssetReference assetReference3 = dock.Data.AmbienceNightAssetReference;
		AssetReferenceOverride assetReferenceOverride2 = dock.Data.AmbienceDayAssetOverrides.Find(delegate(AssetReferenceOverride o)
		{
			if (o.nodesVisited.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s)))
			{
				if (o.boolValues.All((string b) => GameManager.Instance.SaveData.GetBoolVariable(b, false)))
				{
					return GameManager.Instance.SaveData.TIRWorldPhase >= o.tirWorldPhase;
				}
			}
			return false;
		});
		if (assetReferenceOverride2 != null)
		{
			assetReference2 = assetReferenceOverride2.assetReference;
		}
		AssetReferenceOverride assetReferenceOverride3 = dock.Data.AmbienceNightAssetOverrides.Find(delegate(AssetReferenceOverride o)
		{
			if (o.nodesVisited.All((string s) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(s)))
			{
				if (o.boolValues.All((string b) => GameManager.Instance.SaveData.GetBoolVariable(b, false)))
				{
					return GameManager.Instance.SaveData.TIRWorldPhase >= o.tirWorldPhase;
				}
			}
			return false;
		});
		if (assetReferenceOverride3 != null)
		{
			assetReference3 = assetReferenceOverride3.assetReference;
		}
		this.PlayDockAmbienceAudio(assetReference2, assetReference3, AudioLayer.SFX_WORLD);
	}

	public void PlayDockAmbienceAudio(AssetReference dayAssetReference, AssetReference nightAssetReference, AudioLayer audioLayer)
	{
		if (dayAssetReference != null && dayAssetReference.RuntimeKeyIsValid())
		{
			Addressables.LoadAssetAsync<AudioClip>(dayAssetReference).Completed += delegate(AsyncOperationHandle<AudioClip> op)
			{
				if (op.Status == AsyncOperationStatus.Succeeded)
				{
					this.audioSourceDockAmbienceDay.clip = op.Result;
					this.audioSourceDockAmbienceDay.Play();
				}
			};
		}
		else
		{
			this.audioSourceDockAmbienceDay.Stop();
		}
		if (nightAssetReference != null && nightAssetReference.RuntimeKeyIsValid())
		{
			Addressables.LoadAssetAsync<AudioClip>(nightAssetReference).Completed += delegate(AsyncOperationHandle<AudioClip> op)
			{
				if (op.Status == AsyncOperationStatus.Succeeded)
				{
					this.audioSourceDockAmbienceNight.clip = op.Result;
					this.audioSourceDockAmbienceNight.Play();
				}
			};
			return;
		}
		this.audioSourceDockAmbienceNight.Stop();
	}

	public void StopDockAmbienceAudio()
	{
		this.audioSourceDockAmbienceDay.Stop();
		this.audioSourceDockAmbienceNight.Stop();
	}

	private void OnDestroy()
	{
		this.StopDockAmbienceAudio();
		GameManager.Instance.AudioPlayer.RequestMusicStop();
	}

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private AudioMixer masterAudioMixer;

	[SerializeField]
	private AudioSource audioSourceDockAmbienceDay;

	[SerializeField]
	private AudioSource audioSourceDockAmbienceNight;

	private Tweener volumeTween;

	private bool isDocked;

	private float helperVar;

	private float dockAmbienceVolume;

	private Dock currentDock;
}
