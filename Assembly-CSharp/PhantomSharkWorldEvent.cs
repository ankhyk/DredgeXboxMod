using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class PhantomSharkWorldEvent : WorldEvent
{
	private void Awake()
	{
		this.material = this.meshToFade.material;
		this.material.SetFloat("_Opacity", 0f);
	}

	public override void Activate()
	{
		base.Activate();
		Vector3 position = base.transform.position;
		position.y = -1f;
		base.transform.position = position;
		this.player = GameManager.Instance.Player.transform;
		this.currentVelocity = this.initialSpeed;
		this.currentState = PhantomSharkWorldEvent.States.Appear;
		this.LerpOpacityTo(1f, 1.5f, delegate
		{
			this.currentState = PhantomSharkWorldEvent.States.Chase;
		});
		this.movementAudioSource.DOFade(1f, 1f).From(0f, true, false);
		this.movementAudioSource.PlayOneShot(this.appearSFXClip, 1f);
		GameManager.Instance.VibrationManager.Vibrate(this.spawnVibration, VibrationRegion.WholeBody, true).Run();
		if (this.player != null)
		{
			Vector3 right = this.player.right;
			right.y = 0f;
			Vector3 vector = base.transform.position + right.normalized;
			base.transform.LookAt(vector);
			return;
		}
		this.RequestEventFinish();
	}

	private void FixedUpdate()
	{
		if (this.player == null)
		{
			this.player = GameManager.Instance.Player.transform;
			return;
		}
		Vector3 position = this.player.position;
		this.currentTurnSpeed = Mathf.MoveTowards(this.currentTurnSpeed, 10f, Time.deltaTime * this.turnSpeed);
		switch (this.currentState)
		{
		case PhantomSharkWorldEvent.States.Appear:
			this.SmoothLookAt(position, this.currentTurnSpeed);
			break;
		case PhantomSharkWorldEvent.States.Chase:
		{
			this.SmoothLookAt(position, this.currentTurnSpeed);
			Vector3 normalized = (base.transform.position - this.player.position).normalized;
			if (Mathf.Abs(Vector3.Dot(base.transform.forward, normalized)) < this.dodgeSensitivity)
			{
				this.currentState = PhantomSharkWorldEvent.States.Disappear;
			}
			break;
		}
		case PhantomSharkWorldEvent.States.Disappear:
			this.RequestEventFinish();
			break;
		}
		if (this.rb.velocity.magnitude < this.maxSpeed)
		{
			Vector3 forward = base.transform.forward;
			this.currentVelocity = Mathf.MoveTowards(this.currentVelocity, this.maxSpeed, Time.fixedDeltaTime * this.acceleration);
			this.rb.velocity = forward * this.currentVelocity;
		}
	}

	private void LerpOpacityTo(float newOpacity, float durationSec, Action onComplete)
	{
		if (this.opacityTween != null)
		{
			this.opacityTween.Kill(false);
			this.opacityTween = null;
		}
		this.opacityTween = DOTween.To(() => this.currentOpacityVal, delegate(float x)
		{
			this.currentOpacityVal = x;
		}, newOpacity, durationSec).OnUpdate(new TweenCallback(this.OnOpacityUpdated)).OnComplete(delegate
		{
			this.opacityTween = null;
			if (onComplete != null)
			{
				Action onComplete2 = onComplete;
				if (onComplete2 == null)
				{
					return;
				}
				onComplete2();
			}
		});
	}

	private void OnOpacityUpdated()
	{
		this.material.SetFloat("_Opacity", this.currentOpacityVal);
	}

	private void SmoothLookAt(Vector3 target, float turnSpeed)
	{
		Quaternion quaternion = Quaternion.LookRotation(target - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, turnSpeed * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.currentState = PhantomSharkWorldEvent.States.Disappear;
			this.playerCollider.SetActive(false);
			this.anim.SetBool("Bite", true);
			this.movementAudioSource.Stop();
			this.movementAudioSource.clip = this.impactSFXClip;
			this.movementAudioSource.Play();
		}
	}

	public override void RequestEventFinish()
	{
		this.currentState = PhantomSharkWorldEvent.States.Exiting;
		if (!this.finishRequested)
		{
			base.RequestEventFinish();
			this.playerCollider.SetActive(false);
			this.currentVelocity = 10f;
			if (!this.disappearParticles.activeInHierarchy)
			{
				this.disappearParticles.SetActive(true);
			}
			this.LerpOpacityTo(0f, this.delayBeforeDestroying, null);
			this.movementAudioSource.DOFade(0f, this.delayBeforeDestroying);
			this.vFXVolumeFader.BlendDurationSec = this.delayBeforeDestroying;
			base.StartCoroutine(this.vFXVolumeFader.FadeOut());
			base.StartCoroutine(this.DelayedEventFinish());
		}
	}

	private IEnumerator DelayedEventFinish()
	{
		yield return new WaitForSeconds(this.delayBeforeDestroying);
		this.EventFinished();
		global::UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private SkinnedMeshRenderer meshToFade;

	[SerializeField]
	private Rigidbody rb;

	[SerializeField]
	private GameObject playerCollider;

	[SerializeField]
	private float maxSpeed = 5f;

	[SerializeField]
	private float acceleration = 1f;

	[SerializeField]
	private float initialSpeed = 10f;

	[SerializeField]
	private float turnSpeed = 1f;

	[SerializeField]
	private float dodgeSensitivity = 0.7f;

	[SerializeField]
	private GameObject disappearParticles;

	[SerializeField]
	private float delayBeforeDestroying;

	[SerializeField]
	private VFXVolumeFader vFXVolumeFader;

	[SerializeField]
	private AudioClip appearSFXClip;

	[SerializeField]
	private AudioClip impactSFXClip;

	[SerializeField]
	private AudioSource movementAudioSource;

	[SerializeField]
	private VibrationData spawnVibration;

	private PhantomSharkWorldEvent.States currentState;

	private Material material;

	private float currentTurnSpeed;

	private float currentVelocity;

	private Transform player;

	private WaveController waveController;

	private Tweener opacityTween;

	private float currentOpacityVal;

	private enum States
	{
		Appear,
		Chase,
		Disappear,
		Exiting
	}
}
