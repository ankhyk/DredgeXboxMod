using System;
using System.Collections;
using UnityEngine;

public class DestinationAudio : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnDestinationVisited += this.OnDestinationVisited;
		GameEvents.Instance.OnSpeakerVisited += this.OnSpeakerVisited;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnDestinationVisited -= this.OnDestinationVisited;
		GameEvents.Instance.OnSpeakerVisited -= this.OnSpeakerVisited;
	}

	private void OnDestinationVisited(BaseDestination destination, bool visited)
	{
		if (this.audioStopCoroutine != null)
		{
			base.StopCoroutine(this.audioStopCoroutine);
		}
		if (visited)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(destination.VisitSFX, AudioLayer.SFX_WORLD, 1f, 1f);
			AudioSource audioSource = this.outdoorDestinationSource;
			if (destination.IsIndoors)
			{
				audioSource = this.indoorDestinationSource;
				GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_INDOORS, this.transitionDuration);
			}
			if (destination.LoopSFX)
			{
				audioSource.clip = destination.LoopSFX;
				audioSource.Play();
			}
			this.wasPreviousDestinationIndoors = destination.IsIndoors;
			return;
		}
		if (this.wasPreviousDestinationIndoors)
		{
			GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_OUTDOORS, this.transitionDuration);
			this.wasPreviousDestinationIndoors = false;
		}
		this.audioStopCoroutine = base.StartCoroutine(this.DelayedAudioStop());
	}

	private void OnSpeakerVisited(SpeakerData speakerData, bool visited)
	{
		if (this.audioStopCoroutine != null)
		{
			base.StopCoroutine(this.audioStopCoroutine);
		}
		if (visited)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(speakerData.visitSFX, AudioLayer.SFX_WORLD, 1f, 1f);
			AudioSource audioSource = this.outdoorDestinationSource;
			if (speakerData.isIndoors)
			{
				audioSource = this.indoorDestinationSource;
				GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_INDOORS, this.transitionDuration);
			}
			if (speakerData.loopSFX)
			{
				audioSource.clip = speakerData.loopSFX;
				audioSource.Play();
			}
			this.wasPreviousDestinationIndoors = speakerData.isIndoors;
			return;
		}
		if (this.wasPreviousDestinationIndoors)
		{
			GameManager.Instance.AudioPlayer.TransitionToSnapshot(SnapshotType.DOCKED_OUTDOORS, this.transitionDuration);
			this.wasPreviousDestinationIndoors = false;
		}
		this.audioStopCoroutine = base.StartCoroutine(this.DelayedAudioStop());
	}

	private IEnumerator DelayedAudioStop()
	{
		yield return new WaitForSeconds(this.transitionDuration);
		this.outdoorDestinationSource.Stop();
		this.indoorDestinationSource.Stop();
		this.audioStopCoroutine = null;
		yield break;
	}

	[SerializeField]
	private float transitionDuration;

	[SerializeField]
	private AudioSource indoorDestinationSource;

	[SerializeField]
	private AudioSource outdoorDestinationSource;

	private bool wasPreviousDestinationIndoors;

	private Coroutine audioStopCoroutine;
}
