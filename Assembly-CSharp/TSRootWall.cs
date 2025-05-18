using System;
using UnityEngine;
using UnityEngine.Audio;

public class TSRootWall : MonoBehaviour
{
	private void Start()
	{
		this.timeUntilNextStateCheck = global::UnityEngine.Random.Range(this.minTimeBetweenChecks, this.maxTimeBetweenChecks);
	}

	private void OnEnable()
	{
		this.animator.SetBool("isShowing", this.isShowing);
	}

	private void Update()
	{
		this.timeUntilNextStateCheck -= Time.deltaTime;
		if (this.timeUntilNextStateCheck <= 0f)
		{
			this.TryChangeState();
			this.timeUntilNextStateCheck = global::UnityEngine.Random.Range(this.minTimeBetweenChecks, this.maxTimeBetweenChecks);
		}
	}

	private void TryChangeState()
	{
		float num = Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position);
		if (num > this.maxPlayerDistance)
		{
			return;
		}
		float currentSanity = GameManager.Instance.Player.Sanity.CurrentSanity;
		if (!this.isShowing && currentSanity > this.playerSanityThreshold)
		{
			return;
		}
		if (num < this.minPlayerDistance && !this.isShowing)
		{
			return;
		}
		this.isShowing = !this.isShowing;
		this.animator.SetBool("isShowing", this.isShowing);
		AudioUtil.PlayClipAtPoint(this.isShowing ? this.emergeClip : this.submergeClip, base.transform.position, 1f, this.audioMixerGroup, AudioRolloffMode.Linear, this.sfxCloseDistance, this.sfxFarDistance, false, false);
	}

	[SerializeField]
	private AudioClip emergeClip;

	[SerializeField]
	private AudioClip submergeClip;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioMixerGroup audioMixerGroup;

	[SerializeField]
	private float minTimeBetweenChecks;

	[SerializeField]
	private float maxTimeBetweenChecks;

	[SerializeField]
	private float minPlayerDistance;

	[SerializeField]
	private float maxPlayerDistance;

	[SerializeField]
	private float playerSanityThreshold;

	[SerializeField]
	private float sfxCloseDistance;

	[SerializeField]
	private float sfxFarDistance;

	private bool isShowing;

	private float timeUntilNextStateCheck;
}
