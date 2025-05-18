using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DiamondMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
		this.equipmentSpeed = GameManager.Instance.PlayerStats.MinigameFishingSpeedModifier;
		this.currentState = (showTargets ? DiamondMinigame.DiamondMinigameState.FIRING : DiamondMinigame.DiamondMinigameState.NONE);
		if (showTrophyNotch)
		{
			this.FireTarget(true);
		}
	}

	public override void StopGame()
	{
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
		base.StopGame();
	}

	public override void ResetGame()
	{
		base.ResetGame();
		this.canGenerateTargets = false;
	}

	protected override void ClearTargetImages()
	{
		for (int i = this.targets.Count - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.targets[i].gameObject);
		}
		this.targets.Clear();
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
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].OnInputDisabled();
		}
		this.progressDisabled = true;
	}

	protected override void OnInputReenabled()
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].OnInputEnabled();
		}
		this.progressDisabled = false;
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			if (this.currentState == DiamondMinigame.DiamondMinigameState.WAITING)
			{
				this.timeUntilNextTarget -= Time.deltaTime;
				if (this.timeUntilNextTarget <= 0f)
				{
					this.currentState = DiamondMinigame.DiamondMinigameState.FIRING;
				}
			}
			if (this.currentState == DiamondMinigame.DiamondMinigameState.FIRING)
			{
				this.FireTarget(false);
			}
		}
		base.Update();
	}

	protected override void GenerateTargets()
	{
		this.canGenerateTargets = true;
	}

	private void FireTarget(bool forceTrophy = false)
	{
		float num = this.difficultyConfig.diamondScaleUpTimeSec;
		if (forceTrophy)
		{
			num *= this.difficultyConfig.diamondTrophySpeedFactor;
		}
		DiamondMinigameTarget component = global::UnityEngine.Object.Instantiate<GameObject>(this.targetPrefab, this.targetContainer).GetComponent<DiamondMinigameTarget>();
		component.Init(num, forceTrophy, this.thresholdMax, this.difficultyConfig.diamondRotation);
		DiamondMinigameTarget diamondMinigameTarget = component;
		diamondMinigameTarget.OnScaleLimitReached = (Action<DiamondMinigameTarget>)Delegate.Combine(diamondMinigameTarget.OnScaleLimitReached, new Action<DiamondMinigameTarget>(this.OnScaleLimitReached));
		DiamondMinigameTarget diamondMinigameTarget2 = component;
		diamondMinigameTarget2.OnDismissed = (Action<DiamondMinigameTarget>)Delegate.Combine(diamondMinigameTarget2.OnDismissed, new Action<DiamondMinigameTarget>(this.OnDismissed));
		this.targets.Add(component);
		this.timeUntilNextTarget = global::UnityEngine.Random.Range(this.difficultyConfig.timeBetweenDiamondTargetsMin, this.difficultyConfig.timeBetweenDiamondTargetsMax);
		this.currentState = DiamondMinigame.DiamondMinigameState.WAITING;
	}

	private DiamondMinigame.HitResult TryHitTarget(DiamondMinigameTarget target)
	{
		if (!(target != null) || !this.DoesHitTarget(target.GetCurrentScale()))
		{
			return DiamondMinigame.HitResult.MISS;
		}
		if (!target.IsSpecial)
		{
			return DiamondMinigame.HitResult.HIT;
		}
		return DiamondMinigame.HitResult.SPECIAL_HIT;
	}

	private bool DoesHitTarget(float scale)
	{
		return scale > this.thresholdMin && scale < this.thresholdMax;
	}

	public override void OnMinigameInteractPress()
	{
		if (this.inputEnabled)
		{
			List<DiamondMinigameTarget> list = this.targets.FindAll((DiamondMinigameTarget t) => t.IsInPlay && this.DoesHitTarget(t.GetCurrentScale()));
			DiamondMinigame.HitResult hitResult = DiamondMinigame.HitResult.MISS;
			this.targets.ForEach(delegate(DiamondMinigameTarget t)
			{
				DiamondMinigame.HitResult hitResult2 = this.TryHitTarget(t);
				if ((hitResult == DiamondMinigame.HitResult.MISS && hitResult2 != DiamondMinigame.HitResult.MISS) || (hitResult == DiamondMinigame.HitResult.HIT && hitResult2 == DiamondMinigame.HitResult.SPECIAL_HIT))
				{
					hitResult = hitResult2;
				}
			});
			list.ForEach(delegate(DiamondMinigameTarget t)
			{
				t.Dismiss(true);
			});
			AssetReference assetReference = null;
			float num = global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax);
			if (hitResult == DiamondMinigame.HitResult.SPECIAL_HIT)
			{
				base.AddProgress(1f);
				this.didHitSpecialTarget = true;
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
			}
			else if (hitResult == DiamondMinigame.HitResult.HIT)
			{
				base.AddProgress(this.difficultyConfig.targetValue * this.equipmentSpeed);
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
			else if (hitResult == DiamondMinigame.HitResult.MISS)
			{
				if (this.removeProgressOnMiss)
				{
					base.RemoveProgress(this.difficultyConfig.targetValue * this.difficultyConfig.valueFactor, true);
				}
				assetReference = this.missSFX;
				this.inputEnabled = false;
				this.inputEnableCoroutine = base.StartCoroutine(base.InputEnableDelayed());
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingFailVibration, VibrationRegion.WholeBody, false);
			}
			if (assetReference != null)
			{
				GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, num);
			}
		}
	}

	private void OnScaleLimitReached(DiamondMinigameTarget target)
	{
		target.OnScaleLimitReached = (Action<DiamondMinigameTarget>)Delegate.Remove(target.OnScaleLimitReached, new Action<DiamondMinigameTarget>(this.OnScaleLimitReached));
		target.Dismiss(false);
	}

	private void OnDismissed(DiamondMinigameTarget target)
	{
		target.OnDismissed = (Action<DiamondMinigameTarget>)Delegate.Remove(target.OnDismissed, new Action<DiamondMinigameTarget>(this.OnDismissed));
		this.targets.Remove(target);
		global::UnityEngine.Object.Destroy(target.gameObject);
	}

	[SerializeField]
	private GameObject targetPrefab;

	[SerializeField]
	private Transform targetContainer;

	[SerializeField]
	protected List<DiamondMinigameTarget> targets;

	[SerializeField]
	private float thresholdMin;

	[SerializeField]
	private float thresholdMax;

	private bool canGenerateTargets;

	private float timeUntilNextTarget;

	private DiamondMinigame.DiamondMinigameState currentState;

	private enum DiamondMinigameState
	{
		NONE,
		WAITING,
		FIRING
	}

	private enum HitResult
	{
		HIT,
		SPECIAL_HIT,
		MISS
	}
}
