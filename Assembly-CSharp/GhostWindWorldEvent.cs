using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GhostWindWorldEvent : WorldEvent
{
	public override void Activate()
	{
		base.Activate();
		this.spawnTime = Time.time;
		base.transform.SetParent(GameManager.Instance.Player.transform, false);
		base.transform.localPosition = Vector3.zero;
		GameEvents.Instance.OnPlayerInteractedWithPOI += this.OnPlayerInteractedWithPOI;
		List<POI> list = global::UnityEngine.Object.FindObjectsOfType<POI>(true).ToList<POI>();
		float dist = 0f;
		POI poi = list.Find(delegate(POI p)
		{
			dist = Vector3.Distance(p.transform.position, this.transform.position);
			return dist < this.maxRange && dist > this.minRange && p.CanBeGhostWindTarget();
		});
		if (poi)
		{
			this.target = poi.GhostWindTargetTransform;
			if (!this.target)
			{
				this.target = poi.transform;
			}
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.destinationPrefab, this.target);
			this.destinationParticles = gameObject.GetComponent<ParticleSystem>();
			if (!this.destinationParticles)
			{
				this.RequestEventFinish();
				return;
			}
			this.finishDelaySec = Mathf.Max(this.directionalParticles.main.startLifetime.constantMax, this.destinationParticles.main.startLifetime.constantMax);
			if (!GameManager.Instance.AudioPlayer.IsPlayingStinger)
			{
				this.audioSource.Play();
				this.audioSource.DOFade(this.volumeMax, this.volumeFadeInDuration);
				return;
			}
		}
		else
		{
			this.RequestEventFinish();
		}
	}

	private void Update()
	{
		if (this.target)
		{
			base.transform.LookAt(this.target, Vector3.up);
		}
		if (!this.finishRequested && Time.time > this.spawnTime + base.worldEventData.durationSec)
		{
			this.RequestEventFinish();
		}
	}

	public override void RequestEventFinish()
	{
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			GameEvents.Instance.OnPlayerInteractedWithPOI -= this.OnPlayerInteractedWithPOI;
			this.directionalParticles.Stop();
			if (this.destinationParticles)
			{
				this.destinationParticles.Stop();
			}
			this.audioSource.DOFade(0f, this.volumeFadeOutDuration);
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private void OnPlayerInteractedWithPOI()
	{
		this.RequestEventFinish();
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.finishDelaySec);
		this.audioSource.Stop();
		if (this.destinationParticles)
		{
			global::UnityEngine.Object.Destroy(this.destinationParticles.gameObject);
		}
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private float maxRange;

	[SerializeField]
	private float minRange;

	[SerializeField]
	private GameObject destinationPrefab;

	[SerializeField]
	private ParticleSystem directionalParticles;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float volumeMax;

	[SerializeField]
	private float volumeFadeInDuration;

	[SerializeField]
	private float volumeFadeOutDuration;

	private ParticleSystem destinationParticles;

	private float finishDelaySec = 5f;

	private Transform target;

	private float spawnTime;
}
