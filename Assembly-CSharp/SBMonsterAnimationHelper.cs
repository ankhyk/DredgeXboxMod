using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SBMonsterAnimationHelper : MonoBehaviour, IGameModeResponder
{
	public void OnGameModeChanged(GameMode newGameMode)
	{
		this.currentGameMode = newGameMode;
		if (this.currentGameMode == GameMode.PASSIVE && this.hasDetectedPlayer)
		{
			this.OnPlayerExitDetected();
		}
	}

	private void Start()
	{
		for (int i = 0; i < this.smallTentacleAnimators.Count; i++)
		{
			this.smallTentacleAnimators[i].SetFloat("offset", (float)(1 / this.smallTentacleAnimators.Count * i));
		}
		this.isBanishMachineActive = GameManager.Instance.Time.TimeAndDay < GameManager.Instance.SaveData.BanishMachineExpiry;
		this.RefreshBanishState();
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Combine(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
		PlayerDetector playerDetector2 = this.playerDetector;
		playerDetector2.OnPlayerExitDetected = (Action)Delegate.Combine(playerDetector2.OnPlayerExitDetected, new Action(this.OnPlayerExitDetected));
		this.lookAtTarget.Speed = this.attackTentacleIdleSpeed;
		this.attackTentacleCurrentScaleTarget = this.attackTentacleMinScale;
		this.attackTentacleCurrentScale = this.attackTentacleMinScale;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnBanishMachineToggled += this.OnBanishMachineToggled;
		GameEvents.Instance.OnTeleportBegin += this.OnTeleportBegin;
	}

	private void OnDestroy()
	{
		PlayerDetector playerDetector = this.playerDetector;
		playerDetector.OnPlayerDetected = (Action)Delegate.Remove(playerDetector.OnPlayerDetected, new Action(this.OnPlayerDetected));
		PlayerDetector playerDetector2 = this.playerDetector;
		playerDetector2.OnPlayerExitDetected = (Action)Delegate.Remove(playerDetector2.OnPlayerExitDetected, new Action(this.OnPlayerExitDetected));
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
		GameEvents.Instance.OnBanishMachineToggled -= this.OnBanishMachineToggled;
		GameEvents.Instance.OnTeleportBegin -= this.OnTeleportBegin;
	}

	private void OnPlayerAbilityToggled(AbilityData abilityData, bool enabled)
	{
		if (abilityData.name == this.banishAbility.name)
		{
			this.isBanishAbilityActive = enabled;
			this.RefreshBanishState();
			if (enabled && !this.isBanishMachineActive)
			{
				GameEvents.Instance.TriggerThreatBanished(Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < this.banishAchievementDistanceThreshold);
			}
		}
	}

	private void OnTeleportBegin()
	{
		this.playerDetector.Reset();
	}

	private void OnBanishMachineToggled(bool enabled)
	{
		this.isBanishMachineActive = enabled;
		this.RefreshBanishState();
	}

	private void RefreshBanishState()
	{
		this.SetBanished(this.isBanishAbilityActive || this.isBanishMachineActive);
	}

	private void OnPlayerDetected()
	{
		if (this.currentGameMode == GameMode.PASSIVE)
		{
			return;
		}
		this.hasDetectedPlayer = true;
		this.lookAtTarget.Target = GameManager.Instance.Player.transform;
		this.SetDetectsPlayer(true);
		GameManager.Instance.VibrationManager.Vibrate(this.aggroVibration, VibrationRegion.WholeBody, true);
		if (Time.time > this.timeOfLastAggroSFX + this.aggroDelaySec)
		{
			this.timeOfLastAggroSFX = Time.time;
			this.audioSource.PlayOneShot(this.aggroClips.PickRandom<AudioClip>());
		}
		if (this.attackTentacleSpeedTween != null)
		{
			DOTween.Kill(this.attackTentacleSpeedTween, false);
			this.attackTentacleSpeedTween = null;
		}
		this.attackTentacleSpeedTween = DOTween.To(() => this.lookAtTarget.Speed, delegate(float x)
		{
			this.lookAtTarget.Speed = x;
		}, this.attackTentacleDetectedSpeed, this.attackTentacleSpeedTweenDuration);
	}

	private void OnPlayerExitDetected()
	{
		this.hasDetectedPlayer = false;
		this.lookAtTarget.Target = null;
		this.SetDetectsPlayer(false);
		if (this.attackTentacleSpeedTween != null)
		{
			DOTween.Kill(this.attackTentacleSpeedTween, false);
			this.attackTentacleSpeedTween = null;
		}
		this.lookAtTarget.Speed = this.attackTentacleIdleSpeed;
	}

	private void Update()
	{
		if (this.isBanishAbilityActive || this.isBanishMachineActive)
		{
			return;
		}
		if (this.hasDetectedPlayer && GameManager.Instance.Player.IsAlive)
		{
			float num = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(GameManager.Instance.Player.transform.position.x, GameManager.Instance.Player.transform.position.z));
			float num2 = Mathf.InverseLerp(this.attackTentacleMinProximity, this.attackTentacleMaxProximity, num);
			this.attackTentacleCurrentScaleTarget = Mathf.Lerp(this.attackTentacleMinScale, this.attackTentacleMaxScale, num2);
			if (Vector3.Angle(this.lookAtTarget.transform.forward, GameManager.Instance.Player.transform.position - this.lookAtTarget.transform.position) < this.attackTentacleAngleThreshold)
			{
				if (GameManager.Instance.Player.IsFishing)
				{
					this.timePlayerHasBeenInRange = this.attackTentacleInRangeDurationThreshold;
				}
				else
				{
					this.timePlayerHasBeenInRange += Time.deltaTime;
				}
			}
			else
			{
				this.timePlayerHasBeenInRange = 0f;
			}
			if (this.timePlayerHasBeenInRange >= this.attackTentacleInRangeDurationThreshold)
			{
				this.TriggerAttack();
				this.timePlayerHasBeenInRange = 0f;
			}
			if (Time.time > this.timeOfLastAggroSFX + this.aggroDelaySec)
			{
				this.timeOfLastAggroSFX = Time.time;
				this.audioSource.PlayOneShot(this.aggroClips.PickRandom<AudioClip>());
			}
		}
		else
		{
			this.attackTentacleCurrentScaleTarget = this.attackTentacleMinScale;
			this.timePlayerHasBeenInRange = 0f;
			if (Time.time > this.timeOfLastCallSFX + this.callDelaySec)
			{
				this.timeOfLastCallSFX = Time.time;
				this.audioSource.PlayOneShot(this.callClips.PickRandom<AudioClip>());
			}
		}
		this.attackTentacleCurrentScale = Mathf.Lerp(this.attackTentacleCurrentScale, this.attackTentacleCurrentScaleTarget, Time.deltaTime);
		this.attackTentacleTransform.localScale = new Vector3(this.attackTentacleCurrentScale, this.attackTentacleCurrentScale, this.attackTentacleCurrentScale);
	}

	public void SetBanished(bool banished)
	{
		this.audioSource.PlayOneShot(banished ? this.submergeClip : this.emergeClip);
		this.mouthAnimator.SetBool("banished", banished);
		this.attackTentacleAnimator.SetBool("banished", banished);
		this.largeTentacleAnimator.SetBool("banished", banished);
		this.smallTentacleAnimators.ForEach(delegate(Animator a)
		{
			a.SetBool("banished", banished);
		});
	}

	public void SetDetectsPlayer(bool detectsPlayer)
	{
		this.mouthAnimator.SetBool("detectsPlayer", detectsPlayer);
		this.attackTentacleAnimator.SetBool("detectsPlayer", detectsPlayer);
		this.smallTentacleAnimators.ForEach(delegate(Animator a)
		{
			a.SetBool("detectsPlayer", detectsPlayer);
		});
		this.largeTentacleAnimator.SetBool("detectsPlayer", detectsPlayer);
	}

	public void TriggerAttack()
	{
		this.mouthAnimator.SetTrigger("attack");
		this.attackTentacleAnimator.SetTrigger("attack");
	}

	public void PlayPreAttackSFX()
	{
		this.audioSource.PlayOneShot(this.preAttackClips.PickRandom<AudioClip>());
	}

	public void PlayAttackSFX()
	{
		GameManager.Instance.AudioPlayer.PlaySFX(this.attackClips.PickRandom<AudioClip>(), AudioLayer.SFX_PLAYER, 1f, 1f);
	}

	public void OnAttackComplete()
	{
		this.playerDamager.AddListeners();
	}

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private List<AudioClip> preAttackClips;

	[SerializeField]
	private List<AudioClip> attackClips;

	[SerializeField]
	private List<AudioClip> callClips;

	[SerializeField]
	private List<AudioClip> aggroClips;

	[SerializeField]
	private AudioClip emergeClip;

	[SerializeField]
	private AudioClip submergeClip;

	[SerializeField]
	private float callDelaySec;

	[SerializeField]
	private float aggroDelaySec;

	[SerializeField]
	private Animator mouthAnimator;

	[SerializeField]
	private Animator largeTentacleAnimator;

	[SerializeField]
	private Animator attackTentacleAnimator;

	[SerializeField]
	private List<Animator> smallTentacleAnimators;

	[SerializeField]
	private PlayerDetector playerDetector;

	[SerializeField]
	private VariablePlayerDamager playerDamager;

	[SerializeField]
	private VibrationData aggroVibration;

	[SerializeField]
	private VibrationData hitVibration;

	[SerializeField]
	private ClampedLookAtTarget lookAtTarget;

	[SerializeField]
	private AbilityData banishAbility;

	[SerializeField]
	private AbilityData manifestAbility;

	[Header("Attack Tentacle")]
	[SerializeField]
	private Transform attackTentacleTransform;

	[SerializeField]
	private float attackTentacleIdleSpeed;

	[SerializeField]
	private float attackTentacleDetectedSpeed;

	[SerializeField]
	private float attackTentacleSpeedTweenDuration;

	[SerializeField]
	private float attackTentacleMinScale;

	[SerializeField]
	private float attackTentacleMaxScale;

	[SerializeField]
	private float attackTentacleMinProximity;

	[SerializeField]
	private float attackTentacleMaxProximity;

	[SerializeField]
	private float attackTentacleAngleThreshold;

	[SerializeField]
	private float attackTentacleInRangeDurationThreshold;

	[SerializeField]
	private float banishAchievementDistanceThreshold;

	private float attackTentacleCurrentScaleTarget;

	private float attackTentacleCurrentScale;

	private float timePlayerHasBeenInRange;

	private bool hasDetectedPlayer;

	private Tweener attackTentacleSpeedTween;

	private bool isBanishMachineActive;

	private bool isBanishAbilityActive;

	private float timeOfLastCallSFX;

	private float timeOfLastAggroSFX;

	private GameMode currentGameMode;
}
