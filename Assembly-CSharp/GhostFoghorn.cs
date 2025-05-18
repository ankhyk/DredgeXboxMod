using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GhostFoghorn : MonoBehaviour
{
	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool enabled)
	{
		if ((!this.isRecording && !this.canRecord) || this.isReplaying)
		{
			return;
		}
		if (ability.name == this.foghornAbility.name)
		{
			if (enabled && this.canRecord && !this.isRecording && !this.isReplaying)
			{
				this.isRecording = true;
				this.blareTimings = new List<float>();
			}
			if (this.blareTimings == null || (this.blareTimings.Count == 0 && !enabled))
			{
				return;
			}
			if (this.blareTimings.Count == 0)
			{
				this.blareTimings = new List<float>();
				this.isRecording = true;
				this.startTime = Time.time;
				this.timeOfLastEvent = GameManager.Instance.Time.TimeAndDay;
			}
			this.blareTimings.Add(Time.time);
			this.isOpen = this.blareTimings.Count % 2 == 1;
		}
	}

	private void Update()
	{
		this.canRecord = (GameManager.Instance.Time.Time > this.minTimeOfDay || GameManager.Instance.Time.Time < this.maxTimeOfDay) && GameManager.Instance.Time.TimeAndDay > this.timeOfLastEvent + this.daysBetweenEvents;
		if (this.isRecording && this.blareTimings.Count > 0 && ((!this.isOpen && Time.time > this.blareTimings[this.blareTimings.Count - 1] + this.timeoutSinceLastHeardSec) || Time.time > this.blareTimings[0] + this.maxTimeSec))
		{
			this.isRecording = false;
			if (this.blareTimings.Count % 2 == 1)
			{
				this.blareTimings.Add(Time.time);
			}
			this.isReplaying = true;
			Vector2 vector = global::UnityEngine.Random.insideUnitCircle;
			vector *= 1000f;
			base.transform.position = new Vector3(vector.x, 0f, vector.y);
			for (int i = 0; i < this.blareTimings.Count; i++)
			{
				if (i % 2 == 0)
				{
					base.Invoke("StartBlare", this.blareTimings[i] - this.startTime);
				}
				else
				{
					base.Invoke("EndBlare", this.blareTimings[i] - this.startTime);
				}
			}
			base.Invoke("EndReplaying", this.blareTimings[this.blareTimings.Count - 1] - this.startTime);
		}
	}

	public void EndReplaying()
	{
		this.isReplaying = false;
		GameManager.Instance.AchievementManager.SetAchievementState(DredgeAchievementId.ABILITY_FOGHORN, true);
	}

	public void StartBlare()
	{
		if (this.pitchTween != null)
		{
			this.pitchTween.Kill(false);
		}
		this.foghornMidSource.pitch = this.pitchStart;
		this.pitchTween = this.foghornMidSource.DOPitch(this.pitchEnd, this.fadeInSec);
		if (this.volumeTween != null)
		{
			this.volumeTween.Kill(false);
		}
		this.foghornMidSource.volume = this.volumeStart;
		this.volumeTween = this.foghornMidSource.DOFade(this.volumeEnd, this.fadeInSec);
		this.foghornMidSource.Play();
	}

	public void EndBlare()
	{
		this.foghornMidSource.Stop();
		this.audioSource.PlayOneShot(this.foghornEndClip);
	}

	[SerializeField]
	private AbilityData foghornAbility;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioSource foghornMidSource;

	[SerializeField]
	private AudioClip foghornEndClip;

	[SerializeField]
	private float fadeInSec;

	[SerializeField]
	private float pitchStart;

	[SerializeField]
	private float pitchEnd;

	[Range(0f, 1f)]
	[SerializeField]
	private float volumeStart;

	[Range(0f, 1f)]
	[SerializeField]
	private float volumeEnd;

	[Range(0f, 1f)]
	[SerializeField]
	private float minTimeOfDay;

	[Range(0f, 1f)]
	[SerializeField]
	private float maxTimeOfDay;

	[SerializeField]
	private float daysBetweenEvents;

	[SerializeField]
	private float maxTimeSec;

	[SerializeField]
	private float timeoutSinceLastHeardSec;

	[SerializeField]
	private float replayDelayTimeSec;

	private float timeOfLastEvent = float.NegativeInfinity;

	private Tweener pitchTween;

	private Tweener volumeTween;

	private bool canRecord;

	private bool isRecording;

	private bool isReplaying;

	private float startTime;

	private bool isOpen;

	private List<float> blareTimings;
}
