using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CetaceanPodWorldEvent : WorldEvent
{
	private void Awake()
	{
		this.podContainer.transform.position = new Vector3(this.podContainer.transform.position.x, this.downY, this.podContainer.transform.position.z);
		this.prevPos = this.podContainer.position;
		if (this.swimAudio)
		{
			this.swimAudio.DOFade(1f, 1f).From(0f, true, false);
		}
	}

	public override void Activate()
	{
		float num = Mathf.Abs(this.podContainer.position.y - this.upY) / this.enterVerticalSpeed;
		this.podHeightTween = this.podContainer.DOMoveY(this.upY, num, false).OnComplete(delegate
		{
			this.cetaceans.ForEach(delegate(Cetacean d)
			{
				d.CanJump = true;
			});
			this.podHeightTween = null;
			this.currentState = CetaceanPodWorldEvent.CetaceanPodState.IDLING;
		});
		this.podHeightTween.SetEase(Ease.OutQuad);
		Transform transform = GameManager.Instance.Player.transform;
		Vector3 position = base.transform.position;
		this.endPos = position + transform.forward * this.pathLength;
		this.endPos.y = 0f;
		base.transform.DOMove(this.endPos, this.pathLength / this.horizontalSpeed, false).SetEase(Ease.Linear);
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
		this.currentState = CetaceanPodWorldEvent.CetaceanPodState.ENTERING;
	}

	private void Update()
	{
		if (!this.finishRequested && Vector3.Distance(base.transform.position, this.endPos) < this.destinationProximityThreshold)
		{
			this.RequestEventFinish();
		}
		this.podContainer.rotation = Quaternion.Slerp(this.podContainer.rotation, Quaternion.LookRotation(this.podContainer.position - this.prevPos), Time.deltaTime * this.rotationSpeed);
		this.prevPos = this.podContainer.position;
		if (this.currentState == CetaceanPodWorldEvent.CetaceanPodState.IDLING)
		{
			float num = Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position);
			bool flag = num < this.duckDistanceThreshold;
			if (flag)
			{
				if (!this.isDucked)
				{
					this.cetaceans.ForEach(delegate(Cetacean d)
					{
						d.CanJump = false;
					});
				}
				this.isDucked = true;
				float num2 = Mathf.InverseLerp(0f, this.duckDistanceThreshold, num);
				this.targetYPos = Mathf.Lerp(this.duckedY, this.upY, num2);
			}
			else
			{
				this.targetYPos = this.upY;
			}
			this.adjustedPos.x = this.podContainer.position.x;
			this.adjustedPos.y = this.targetYPos;
			this.adjustedPos.z = this.podContainer.position.z;
			this.podContainer.position = this.adjustedPos;
			if (!flag && this.isDucked && this.podContainer.position.y >= this.upY - Mathf.Epsilon)
			{
				this.isDucked = false;
				this.cetaceans.ForEach(delegate(Cetacean d)
				{
					d.CanJump = true;
				});
			}
		}
		if (this.currentState != CetaceanPodWorldEvent.CetaceanPodState.EXITING)
		{
			this.timeUntilNextCall -= Time.deltaTime;
			if (this.timeUntilNextCall <= 0f)
			{
				this.PlayCallSFX();
			}
		}
	}

	public void PlayCallSFX()
	{
		if (this.callAudio)
		{
			this.callAudio.PlayOneShot(this.callClips.PickRandom<AudioClip>());
		}
		this.timeUntilNextCall = global::UnityEngine.Random.Range(this.timeBetweenCallsMin, this.timeBetweenCallsMax);
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.currentState = CetaceanPodWorldEvent.CetaceanPodState.EXITING;
			this.cetaceans.ForEach(delegate(Cetacean d)
			{
				d.CanJump = false;
			});
			if (this.podHeightTween != null)
			{
				this.podHeightTween.Kill(false);
				this.podHeightTween = null;
			}
			float num = Mathf.Abs(this.podContainer.position.y - this.downY) / this.exitVerticalSpeed;
			this.podHeightTween = this.podContainer.DOMoveY(this.downY, num, false).OnComplete(delegate
			{
				this.DelayedEventFinish();
			});
			this.podHeightTween.SetEase(Ease.InSine);
			if (this.swimAudio)
			{
				this.swimAudio.DOFade(0f, num);
			}
			if (!this.isDucked && this.submergeAudio)
			{
				this.submergeAudio.Play();
			}
		}
	}

	private void DelayedEventFinish()
	{
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private AudioSource swimAudio;

	[SerializeField]
	private AudioSource submergeAudio;

	[SerializeField]
	private AudioSource callAudio;

	[SerializeField]
	private List<AudioClip> callClips;

	[SerializeField]
	private List<Cetacean> cetaceans;

	[SerializeField]
	private Transform podContainer;

	[SerializeField]
	private float duckDistanceThreshold;

	[SerializeField]
	private float upY;

	[SerializeField]
	private float duckedY;

	[SerializeField]
	private float downY;

	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private float horizontalSpeed;

	[SerializeField]
	private float enterVerticalSpeed;

	[SerializeField]
	private float exitVerticalSpeed;

	[SerializeField]
	private float destinationProximityThreshold;

	[SerializeField]
	private float pathLength;

	[SerializeField]
	private float timeBetweenCallsMin;

	[SerializeField]
	private float timeBetweenCallsMax;

	private float targetYPos;

	private Tweener podHeightTween;

	private bool isDucked;

	private Vector3 endPos;

	private Vector3 prevPos;

	private CetaceanPodWorldEvent.CetaceanPodState currentState;

	private Vector3 adjustedPos;

	private float timeUntilNextCall;

	private enum CetaceanPodState
	{
		NONE,
		ENTERING,
		IDLING,
		EXITING
	}
}
