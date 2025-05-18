using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityAsyncAwaitUtil;
using UnityEngine;

public class Jellyfish : MonoBehaviour
{
	private void OnEnable()
	{
		this.rotator.counterClockwise = this.counterClockwise;
		base.transform.position = new Vector3(base.transform.position.x, this.downY, base.transform.position.z);
		this.bodyTransform.transform.localPosition = new Vector3(0f, 0f, this.orbitRadius - this.orbitVariance);
		if (this.orbitVariance != 0f)
		{
			if (this.varianceTween != null)
			{
				this.varianceTween.Kill(false);
			}
			this.varianceTween = this.bodyTransform.DOLocalMoveZ(this.orbitRadius + this.orbitVariance, this.varianceSpeed, false);
			this.varianceTween.SetEase(Ease.InOutSine);
			this.varianceTween.SetLoops(-1, LoopType.Yoyo);
			this.varianceTween.SetDelay(this.varianceDelay);
		}
	}

	private void OnDisable()
	{
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
	}

	private void OnPlayerDetected()
	{
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnSignalFired = (Action)Delegate.Combine(animationEvents.OnSignalFired, new Action(this.OnChargeUpComplete));
		this.explodeAnimator.SetTrigger("explode");
	}

	private void OnChargeUpComplete()
	{
		AnimationEvents animationEvents = this.animationEvents;
		animationEvents.OnSignalFired = (Action)Delegate.Remove(animationEvents.OnSignalFired, new Action(this.OnChargeUpComplete));
		global::UnityEngine.Object.Instantiate<GameObject>(this.sporeEffectPrefab, this.bodyTransform).transform.SetParent(null);
		this.Hide(true);
		GameManager.Instance.VibrationManager.Vibrate(this.explodeVibration, VibrationRegion.WholeBody, true).Run();
	}

	public void Show()
	{
		if (this.isChanging || this.isUp)
		{
			return;
		}
		this.isChanging = true;
		base.gameObject.SetActive(true);
		base.transform.DOMoveY(this.upY, this.riseDuration, false).OnComplete(delegate
		{
			PlayerDetector playerDetector = this.playerDetector;
			playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
			this.isChanging = false;
			this.isUp = true;
			this.waterTrailObject.SetActive(true);
		});
	}

	public void Hide(bool didHit)
	{
		if (this.isChanging || !this.isUp)
		{
			return;
		}
		this.isUp = false;
		this.isChanging = true;
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
		this.waterTrailObject.SetActive(false);
		base.transform.DOMoveY(this.downY, didHit ? this.retreatDuration : this.fallDuration, false).OnComplete(delegate
		{
			this.isChanging = false;
			base.gameObject.SetActive(false);
		});
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.orbitRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.orbitRadius - this.orbitVariance);
		Gizmos.DrawWireSphere(base.transform.position, this.orbitRadius + this.orbitVariance);
	}

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private GameObject sporeEffectPrefab;

	[SerializeField]
	private Animator explodeAnimator;

	[SerializeField]
	private AnimationEvents animationEvents;

	[SerializeField]
	private Transform bodyTransform;

	[SerializeField]
	private GameObject waterTrailObject;

	[SerializeField]
	private ConstantlyRotateOnY rotator;

	[SerializeField]
	private float upY;

	[SerializeField]
	private float downY;

	[SerializeField]
	private float riseDuration;

	[SerializeField]
	private float fallDuration;

	[SerializeField]
	private float retreatDuration;

	[SerializeField]
	private float orbitRadius;

	[SerializeField]
	private float orbitVariance;

	[SerializeField]
	private float varianceSpeed;

	[SerializeField]
	private float varianceDelay;

	[SerializeField]
	public bool counterClockwise;

	private Tweener varianceTween;

	private bool isUp;

	private bool isChanging;

	[SerializeField]
	private VibrationData explodeVibration;
}
