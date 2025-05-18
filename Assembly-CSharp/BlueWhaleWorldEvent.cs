using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class BlueWhaleWorldEvent : WorldEvent
{
	private void Awake()
	{
		this.podContainer.transform.position = new Vector3(this.podContainer.transform.position.x, this.exitY, this.podContainer.transform.position.z);
		this.prevPos = this.podContainer.position;
		this.swimAudio.DOFade(1f, 1f).From(0f, true, false);
		this.timeUntilEmergeCall = this.emergeSoundDelay;
	}

	public override void Activate()
	{
		this.currentState = BlueWhaleWorldEvent.CetaceanPodState.IDLING;
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
		Transform transform = GameManager.Instance.Player.transform;
		Vector3 vector = base.worldEventData.depthTestPath[0];
		Vector3 vector2 = base.worldEventData.depthTestPath[base.worldEventData.depthTestPath.Count - 1];
		Vector3 vector3 = transform.position + vector.x * transform.right + vector.z * transform.forward;
		this.endPos = transform.position + vector2.x * transform.right + vector2.z * transform.forward;
		vector3.y = 0f;
		this.endPos.y = 0f;
		float num = Vector3.Distance(vector3, this.endPos);
		this.podContainer.rotation = Quaternion.LookRotation(this.endPos - vector3);
		base.transform.DOMove(this.endPos, num / this.horizontalSpeed, false).SetEase(Ease.Linear);
	}

	private void Update()
	{
		if (!this.finishRequested && Vector3.Distance(base.transform.position, this.endPos) < this.destinationProximityThreshold)
		{
			this.RequestEventFinish();
		}
		this.prevPos = this.podContainer.position;
		if (this.currentState == BlueWhaleWorldEvent.CetaceanPodState.IDLING)
		{
			float num = Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position);
			bool flag = num < this.duckDistanceThreshold;
			if (flag)
			{
				this.isDucked = true;
				float num2 = Mathf.InverseLerp(0f, this.duckDistanceThreshold, num);
				this.targetYPos = Mathf.Lerp(this.duckedY, this.idleY, num2);
			}
			else
			{
				this.targetYPos = this.idleY;
			}
			this.adjustedPos.x = this.podContainer.position.x;
			this.adjustedPos.y = this.targetYPos;
			this.adjustedPos.z = this.podContainer.position.z;
			this.podContainer.position = this.adjustedPos;
			if (!flag && this.isDucked && this.podContainer.position.y >= this.idleY - Mathf.Epsilon)
			{
				this.isDucked = false;
			}
			this.timeUntilNextCall -= Time.deltaTime;
			if (this.timeUntilNextCall <= 0f)
			{
				this.PlayCallSFX();
			}
		}
		if (!this.hasPlayedEmergeCall)
		{
			this.timeUntilEmergeCall -= Time.deltaTime;
			if (this.timeUntilEmergeCall <= 0f)
			{
				this.emergeAudio.Play();
				this.hasPlayedEmergeCall = true;
			}
		}
	}

	public void PlayCallSFX()
	{
		this.callAudio.PlayOneShot(this.callClips.PickRandom<AudioClip>());
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.currentState = BlueWhaleWorldEvent.CetaceanPodState.EXITING;
			if (this.podHeightTween != null)
			{
				this.podHeightTween.Kill(false);
				this.podHeightTween = null;
			}
			float num = Mathf.Abs(this.podContainer.position.y - this.exitY) / this.exitVerticalSpeed;
			this.podHeightTween = this.podContainer.DOMoveY(this.exitY, num, false).OnComplete(delegate
			{
				this.DelayedEventFinish();
			});
			this.podHeightTween.SetEase(Ease.InSine);
			this.swimAudio.DOFade(0f, num);
		}
	}

	private void DelayedEventFinish()
	{
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private AudioSource emergeAudio;

	[SerializeField]
	private AudioSource swimAudio;

	[SerializeField]
	private AudioSource callAudio;

	[SerializeField]
	private List<AudioClip> callClips;

	[SerializeField]
	private Transform podContainer;

	[SerializeField]
	private float duckDistanceThreshold;

	[SerializeField]
	private float idleY;

	[SerializeField]
	private float enterY;

	[SerializeField]
	private float exitY;

	[SerializeField]
	private float duckedY;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private float horizontalSpeed;

	[SerializeField]
	private float exitVerticalSpeed;

	[SerializeField]
	private float destinationProximityThreshold;

	[SerializeField]
	private float timeBetweenCallsMin;

	[SerializeField]
	private float timeBetweenCallsMax;

	[SerializeField]
	private float emergeSoundDelay;

	private float targetYPos;

	private Tweener podHeightTween;

	private bool isDucked;

	private Vector3 endPos;

	private Vector3 prevPos;

	private BlueWhaleWorldEvent.CetaceanPodState currentState;

	private Vector3 adjustedPos;

	private float timeUntilNextCall;

	private bool hasPlayedEmergeCall;

	private float timeUntilEmergeCall;

	private enum CetaceanPodState
	{
		NONE,
		ENTERING,
		IDLING,
		EXITING
	}
}
