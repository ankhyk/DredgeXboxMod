using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BallCatcherMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.showTrophyNotch = showTrophyNotch;
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
		this.equipmentSpeed = GameManager.Instance.PlayerStats.MinigameFishingSpeedModifier;
		if (showTargets)
		{
			this.GenerateNewPattern();
		}
		else
		{
			this.currentState = BallCatcherMinigame.BallCatcherState.NONE;
		}
		this.ballDelaySec = 0f;
		this.patternDelaySec = 0f;
	}

	public override void PrepareGame(HarvestDifficultyConfigData difficultyConfig)
	{
		base.PrepareGame(difficultyConfig);
		float num = 0f;
		this.targetZoneAngleMin = MathUtil.TransformAngleToNegative180Positive180(-(difficultyConfig.targetZoneDegrees * 0.5f) - num);
		this.targetZoneAngleMax = MathUtil.TransformAngleToNegative180Positive180(difficultyConfig.targetZoneDegrees * 0.5f + num);
		this.targetZoneImage.fillAmount = difficultyConfig.targetZoneDegrees / 360f;
		this.targetZoneImage.gameObject.transform.eulerAngles = new Vector3(0f, 0f, this.targetZoneAngleMax - num);
	}

	public override void StopGame()
	{
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
		base.StopGame();
		this.currentState = BallCatcherMinigame.BallCatcherState.NONE;
	}

	public override void ResetGame()
	{
		base.ResetGame();
		this.canGenerateTargets = false;
		this.currentState = BallCatcherMinigame.BallCatcherState.NONE;
	}

	protected override void ClearTargetImages()
	{
		for (int i = this.balls.Count - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.balls[i].gameObject);
		}
		this.balls.Clear();
	}

	protected override float GetCurrentIndicatorAngle()
	{
		return 0f;
	}

	protected override void ClearTargetConfigs()
	{
	}

	protected override void OnInputDisabled()
	{
		this.progressDisabled = true;
		this.balls.ForEach(delegate(BallCatcherBall b)
		{
			b.OnInputDisabled();
		});
	}

	protected override void OnInputReenabled()
	{
		this.progressDisabled = false;
		this.balls.ForEach(delegate(BallCatcherBall b)
		{
			b.OnInputReenabled();
		});
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			if (this.currentState == BallCatcherMinigame.BallCatcherState.FIRING_PATTERN || this.currentState == BallCatcherMinigame.BallCatcherState.FINISHING_PATTERN)
			{
				this.ballDelaySec -= Time.deltaTime;
				if (this.ballDelaySec <= 0f)
				{
					if (this.currentState == BallCatcherMinigame.BallCatcherState.FIRING_PATTERN)
					{
						this.FireBall();
					}
					else
					{
						this.currentState = BallCatcherMinigame.BallCatcherState.PATTERN_FINISHED;
					}
				}
			}
			if (this.currentState == BallCatcherMinigame.BallCatcherState.PATTERN_FINISHED)
			{
				this.GenerateNewPattern();
			}
			if (this.currentState == BallCatcherMinigame.BallCatcherState.WAITING_FOR_PATTERN_TO_START)
			{
				this.patternDelaySec -= Time.deltaTime;
				if (this.patternDelaySec < 0f)
				{
					this.currentState = BallCatcherMinigame.BallCatcherState.FIRING_PATTERN;
				}
			}
		}
		base.Update();
	}

	private void GenerateNewPattern()
	{
		this.currentPattern = this.difficultyConfig.ballCatcherPatterns.PickRandom<List<BallCatcherBallConfig>>();
		this.indexInCurrentPattern = 0;
		this.patternDelaySec = this.basePatternDelaySec / this.difficultyConfig.speedFactor;
		this.currentState = BallCatcherMinigame.BallCatcherState.WAITING_FOR_PATTERN_TO_START;
	}

	protected override void GenerateTargets()
	{
		this.canGenerateTargets = true;
	}

	private void FireBall()
	{
		BallCatcherBallConfig ballCatcherBallConfig = this.currentPattern[this.indexInCurrentPattern];
		BallCatcherBallType ballCatcherBallType = ballCatcherBallConfig.ballType;
		float num = this.baseSpeed * this.difficultyConfig.speedFactor;
		if (this.showTrophyNotch)
		{
			ballCatcherBallType = BallCatcherBallType.SPECIAL;
			num /= this.difficultyConfig.ballTrophySpeedFactor;
			this.showTrophyNotch = false;
		}
		BallCatcherBall component = global::UnityEngine.Object.Instantiate<GameObject>(this.ballPrefab, this.ballContainer).GetComponent<BallCatcherBall>();
		BallCatcherBall ballCatcherBall = component;
		ballCatcherBall.OnReachedOtherSide = (Action<BallCatcherBall>)Delegate.Combine(ballCatcherBall.OnReachedOtherSide, new Action<BallCatcherBall>(this.OnBallReachedOtherSide));
		component.Init(ballCatcherBallConfig.direction, ballCatcherBallType, num, this.targetZoneAngleMin, this.targetZoneAngleMax);
		this.balls.Add(component);
		this.indexInCurrentPattern++;
		this.ballDelaySec = ballCatcherBallConfig.delayBeforeNextBall / this.difficultyConfig.speedFactor;
		if (this.indexInCurrentPattern > this.currentPattern.Count - 1)
		{
			this.currentState = BallCatcherMinigame.BallCatcherState.FINISHING_PATTERN;
		}
	}

	private void OnBallReachedOtherSide(BallCatcherBall ball)
	{
		this.balls.Remove(ball);
		global::UnityEngine.Object.Destroy(ball.gameObject);
	}

	public override void OnMinigameInteractPress()
	{
		if (!this.inputEnabled)
		{
			return;
		}
		List<BallCatcherBall> list = this.balls.FindAll((BallCatcherBall b) => !b.HasGonePastTargetZone && this.DoesHitTarget(b));
		BallCatcherMinigame.HitResult bestHitSoFar = BallCatcherMinigame.HitResult.MISS;
		if (!list.Any((BallCatcherBall b) => b.BallType == BallCatcherBallType.OBSTACLE))
		{
			list.ForEach(delegate(BallCatcherBall ball)
			{
				BallCatcherMinigame.HitResult hitResult = this.TryCatchBall(ball);
				if ((bestHitSoFar == BallCatcherMinigame.HitResult.MISS && hitResult != BallCatcherMinigame.HitResult.MISS) || (bestHitSoFar == BallCatcherMinigame.HitResult.HIT && hitResult == BallCatcherMinigame.HitResult.SPECIAL_HIT))
				{
					bestHitSoFar = hitResult;
				}
			});
		}
		list.ForEach(delegate(BallCatcherBall ball)
		{
			this.balls.Remove(ball);
			global::UnityEngine.Object.Destroy(ball.gameObject);
		});
		string text = "";
		AssetReference assetReference = null;
		float num = global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax);
		if (bestHitSoFar == BallCatcherMinigame.HitResult.SPECIAL_HIT)
		{
			base.AddProgress(1f);
			text = "hit-special";
			this.didHitSpecialTarget = true;
			GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
		}
		else if (bestHitSoFar == BallCatcherMinigame.HitResult.HIT)
		{
			base.AddProgress(this.difficultyConfig.targetValue * this.equipmentSpeed);
			text = "hit";
			if (base.Progress < 1f)
			{
				assetReference = this.hitSFX;
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingBlipVibration, VibrationRegion.WholeBody, false);
			}
			else
			{
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
			}
		}
		else if (bestHitSoFar == BallCatcherMinigame.HitResult.MISS)
		{
			if (this.removeProgressOnMiss)
			{
				base.RemoveProgress(this.difficultyConfig.targetValue * this.difficultyConfig.valueFactor, true);
			}
			text = "miss";
			assetReference = this.missSFX;
			this.inputEnabled = false;
			this.inputEnableCoroutine = base.StartCoroutine(base.InputEnableDelayed());
			GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingFailVibration, VibrationRegion.WholeBody, false);
		}
		else if (bestHitSoFar == BallCatcherMinigame.HitResult.AVOID)
		{
			text = "avoid";
		}
		if (assetReference != null)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, num);
		}
		if (this.feedbackAnimationController != null && text != "")
		{
			this.feedbackAnimationController.SetTrigger(text);
		}
	}

	private BallCatcherMinigame.HitResult TryCatchBall(BallCatcherBall ball)
	{
		if (!this.DoesHitTarget(ball))
		{
			return BallCatcherMinigame.HitResult.MISS;
		}
		if (ball.BallType != BallCatcherBallType.SPECIAL)
		{
			return BallCatcherMinigame.HitResult.HIT;
		}
		return BallCatcherMinigame.HitResult.SPECIAL_HIT;
	}

	private bool DoesHitTarget(BallCatcherBall ball)
	{
		return MathUtil.IsBetween(ball.TransformedAngle, this.targetZoneAngleMin, this.targetZoneAngleMax);
	}

	[SerializeField]
	private Image targetZoneImage;

	[SerializeField]
	private GameObject ballPrefab;

	[SerializeField]
	private Transform ballContainer;

	[SerializeField]
	private float basePatternDelaySec;

	[SerializeField]
	private float baseSpeed;

	[SerializeField]
	protected List<BallCatcherBall> balls;

	private float targetZoneAngleMin;

	private float targetZoneAngleMax;

	private float speed;

	private float ballDelaySec;

	private float patternDelaySec;

	private bool canGenerateTargets;

	private bool showTrophyNotch;

	private BallCatcherMinigame.BallCatcherState currentState;

	private List<BallCatcherBallConfig> currentPattern;

	private int indexInCurrentPattern;

	private enum BallCatcherState
	{
		NONE,
		FIRING_PATTERN,
		FINISHING_PATTERN,
		PATTERN_FINISHED,
		WAITING_FOR_PATTERN_TO_START
	}

	private enum HitResult
	{
		HIT,
		SPECIAL_HIT,
		MISS,
		AVOID
	}
}
