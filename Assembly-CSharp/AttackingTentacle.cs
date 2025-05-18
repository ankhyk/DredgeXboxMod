using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AttackingTentacle : WorldEvent
{
	public bool IsAttackFinished { get; set; }

	private void OnEnable()
	{
		this.playerTransform = GameManager.Instance.Player.transform;
		base.Invoke("PlayAnimation", this.animationDelay);
	}

	private void PlayAnimation()
	{
		this.animator.SetTrigger("play");
		GameManager.Instance.VibrationManager.Vibrate(this.emergeVibration, VibrationRegion.WholeBody, true);
	}

	private void Update()
	{
		this.TrackPlayer();
	}

	public void AttackFinished()
	{
		this.IsAttackFinished = true;
		Action onAttackComplete = this.OnAttackComplete;
		if (onAttackComplete == null)
		{
			return;
		}
		onAttackComplete();
	}

	public void RequestAttackFinish()
	{
		this.animator.SetBool("exit", true);
	}

	public void PlayEmergeSFX()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.emergeSFX, AudioLayer.SFX_WORLD, 1f, 1f);
	}

	public void PlaySubmergeSFX()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.submergeSFX, AudioLayer.SFX_WORLD, 1f, 1f);
	}

	public void PlayAttackSFX()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackSFX, AudioLayer.SFX_WORLD, 1f, 1f);
	}

	private void TrackPlayer()
	{
		this.playerFollowStrength = Mathf.Lerp(this.playerFollowStrength, 0f, Time.deltaTime);
		this.playerTrackingStrength = Mathf.Lerp(this.playerTrackingStrength, this.playerTrackingStrengthTarget, Time.deltaTime);
		Vector3 vector = this.playerTransform.position;
		vector += this.playerTransform.forward * this.tentacleAnchorPos.z;
		vector += this.playerTransform.right * this.tentacleAnchorPos.x;
		vector.y = 0f;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * Mathf.Lerp(0f, 5f, this.playerFollowStrength));
		Vector3 vector2 = this.playerTransform.position - base.transform.position;
		vector2.y = 0f;
		if (vector2.magnitude != 0f)
		{
			Quaternion quaternion = Quaternion.LookRotation(vector2);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, Time.deltaTime * Mathf.Lerp(0f, 40f, this.playerTrackingStrength));
		}
	}

	public void StartTrackingPlayer()
	{
		this.splashEffect.SetActive(false);
		this.playerFollowStrength = this.trackingStrength;
		this.playerTrackingStrengthTarget = 1f;
	}

	public void StopTrackingPlayer()
	{
		this.playerTrackingStrengthTarget = 0f;
	}

	public void PlaySplashEffect()
	{
		this.splashEffect.SetActive(true);
	}

	public void TurnOffColliders()
	{
		for (int i = 0; i < this.colliders.Length; i++)
		{
			this.colliders[i].enabled = false;
		}
	}

	[SerializeField]
	private AssetReference emergeSFX;

	[SerializeField]
	private AssetReference submergeSFX;

	[SerializeField]
	private AssetReference attackSFX;

	[SerializeField]
	private VibrationData emergeVibration;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject splashEffect;

	[SerializeField]
	private Vector3 tentacleAnchorPos;

	[SerializeField]
	private float trackingStrength = 1f;

	[SerializeField]
	private float animationDelay;

	private float playerTrackingStrength = 1f;

	private float playerFollowStrength;

	private Transform playerTransform;

	[SerializeField]
	private Collider[] colliders;

	public Action OnAttackComplete;

	private float playerTrackingStrengthTarget;
}
