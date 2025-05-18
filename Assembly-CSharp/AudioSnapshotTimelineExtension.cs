using System;
using UnityEngine;

public class AudioSnapshotTimelineExtension : MonoBehaviour
{
	public void OnTimelineStarted()
	{
		this.didJustActivate = true;
		this.snapshotToRestore = GameManager.Instance.AudioPlayer.CurrentSnapshotType;
		GameManager.Instance.AudioPlayer.TransitionToSnapshot(this.snapshotType, this.transitionInDurationSec);
	}

	public void OnTimelineComplete()
	{
		if (!this.didJustActivate)
		{
			return;
		}
		this.didJustActivate = false;
		GameManager.Instance.AudioPlayer.TransitionToSnapshot(this.snapshotToRestore, this.transitionOutDurationSec);
	}

	[SerializeField]
	private float transitionInDurationSec;

	[SerializeField]
	private float transitionOutDurationSec;

	[SerializeField]
	private SnapshotType snapshotType;

	private SnapshotType snapshotToRestore;

	private bool didJustActivate;
}
