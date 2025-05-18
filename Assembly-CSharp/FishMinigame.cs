using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class FishMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		this.targetConfigs = new List<HarvestMinigame.TargetConfig>();
		base.Awake();
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.equipmentSpeed = GameManager.Instance.PlayerStats.MinigameFishingSpeedModifier;
		this.isTrophyNotchShowing = showTrophyNotch;
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
	}

	public override void StopGame()
	{
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
		base.StopGame();
	}

	public override void ResetGame()
	{
		this.indicatorAngle = 0f;
		this.UpdateIndicatorPosition();
		this.prevIndicatorRotation = float.PositiveInfinity;
		base.ResetGame();
	}

	protected override float GetCurrentIndicatorAngle()
	{
		return (this.indicatorAngle + 360f) % 360f;
	}

	protected override void ClearTargetImages()
	{
		for (int i = 0; i < this.targetImages.Count; i++)
		{
			this.targetImages[i].gameObject.SetActive(false);
			this.targetImages[i].color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
		}
	}

	protected override void ClearTargetConfigs()
	{
		if (this.targetConfigs != null)
		{
			this.targetConfigs.Clear();
		}
	}

	protected override void OnInputDisabled()
	{
		for (int i = 0; i < this.targetImages.Count; i++)
		{
			this.targetImages[i].color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
		}
		this.progressDisabled = true;
	}

	protected override void OnInputReenabled()
	{
		for (int i = 0; i < this.targetConfigs.Count; i++)
		{
			this.targetImages[i].color = (this.targetConfigs[i].isSpecial ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
		}
		this.progressDisabled = false;
	}

	protected override void GenerateTargets()
	{
		int num = global::UnityEngine.Random.Range(this.difficultyConfig.minTargets, this.difficultyConfig.maxTargets + 1);
		this.targetConfigs.Clear();
		float num2 = 60f;
		float num3 = 20f;
		float num4 = (360f - num2) / (float)num;
		for (int i = 0; i < num; i++)
		{
			HarvestMinigame.TargetConfig targetConfig = new HarvestMinigame.TargetConfig();
			if (this.isTrophyNotchShowing && i == 1)
			{
				targetConfig.isSpecial = true;
				this.specialTargetIndex = i;
				targetConfig.widthDeg = (float)this.difficultyConfig.specialTargetWidth;
			}
			else
			{
				targetConfig.widthDeg = (float)global::UnityEngine.Random.Range(this.difficultyConfig.minTargetWidth, this.difficultyConfig.maxTargetWidth);
			}
			float num5 = num4 * (float)i + num4 * 0.5f;
			float num6 = num4 - targetConfig.widthDeg - num3;
			float num7 = num5 - num6 * 0.5f;
			float num8 = num5 + num6 * 0.5f;
			targetConfig.angleDeg = global::UnityEngine.Random.Range(num7, num8);
			this.targetConfigs.Add(targetConfig);
		}
		for (int j = 0; j < this.targetConfigs.Count; j++)
		{
			HarvestMinigame.TargetConfig targetConfig2 = this.targetConfigs[j];
			Image image = this.targetImages[j];
			image.gameObject.SetActive(true);
			float num9 = targetConfig2.angleDeg;
			num9 += targetConfig2.widthDeg * 0.5f;
			image.transform.eulerAngles = new Vector3(image.transform.eulerAngles.x, image.transform.eulerAngles.y, num9);
			image.fillAmount = targetConfig2.widthDeg / 360f;
			image.color = (targetConfig2.isSpecial ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
		}
	}

	private bool TestTargetProximity(HarvestMinigame.TargetConfig targetConfig)
	{
		float proximityThreshold = 30f;
		float thisMinBound = targetConfig.angleDeg - targetConfig.widthDeg * 0.5f;
		float thisMaxBound = targetConfig.angleDeg + targetConfig.widthDeg * 0.5f;
		return this.targetConfigs.TrueForAll(delegate(HarvestMinigame.TargetConfig tc)
		{
			float num = tc.angleDeg - tc.widthDeg * 0.5f;
			float num2 = tc.angleDeg + tc.widthDeg * 0.5f;
			return Mathf.Abs(thisMinBound - num2) > proximityThreshold && Mathf.Abs(num - thisMaxBound) > proximityThreshold;
		});
	}

	private void UpdateIndicatorPosition()
	{
		this.indicatorAngle = (this.indicatorAngle + 360f) % 360f;
		this.indicatorObject.transform.eulerAngles = new Vector3(this.indicatorObject.transform.eulerAngles.x, this.indicatorObject.transform.eulerAngles.y, this.indicatorAngle);
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			this.indicatorAngle += -this.difficultyConfig.rotationSpeed * Time.deltaTime;
			this.UpdateIndicatorPosition();
			if (this.prevIndicatorRotation < this.indicatorAngle)
			{
				this.targetIndexesHitThisRotation.Clear();
				if (this.isTrophyNotchShowing)
				{
					this.targetImages[this.specialTargetIndex].gameObject.SetActive(false);
					this.isTrophyNotchShowing = false;
				}
			}
			this.prevIndicatorRotation = this.indicatorAngle;
		}
		base.Update();
	}

	private FishMinigame.HitResult TryHitTarget()
	{
		int i = 0;
		while (i < this.targetConfigs.Count)
		{
			int num = i;
			HarvestMinigame.TargetConfig targetConfig = this.targetConfigs[num];
			if (this.DoesHitTarget(targetConfig) && !this.targetIndexesHitThisRotation.Contains(num))
			{
				this.targetIndexesHitThisRotation.Add(num);
				if (!targetConfig.isSpecial)
				{
					return FishMinigame.HitResult.HIT;
				}
				return FishMinigame.HitResult.SPECIAL_HIT;
			}
			else
			{
				i++;
			}
		}
		return FishMinigame.HitResult.MISS;
	}

	private bool DoesHitTarget(HarvestMinigame.TargetConfig targetConfig)
	{
		if (targetConfig.isSpecial && !this.isTrophyNotchShowing)
		{
			return false;
		}
		float num = 0f;
		float currentIndicatorAngle = this.GetCurrentIndicatorAngle();
		float num2 = targetConfig.angleDeg - targetConfig.widthDeg * 0.5f - num;
		float num3 = targetConfig.angleDeg + targetConfig.widthDeg * 0.5f;
		return currentIndicatorAngle > num2 && currentIndicatorAngle < num3;
	}

	public override void OnMinigameInteractPress()
	{
		if (this.inputEnabled)
		{
			AssetReference assetReference = null;
			FishMinigame.HitResult hitResult = this.TryHitTarget();
			float num = global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax);
			string text;
			if (hitResult == FishMinigame.HitResult.SPECIAL_HIT)
			{
				base.AddProgress(1f);
				text = "hit-special";
				this.didHitSpecialTarget = true;
				this.targetImages[this.specialTargetIndex].gameObject.SetActive(false);
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
			}
			else if (hitResult == FishMinigame.HitResult.HIT)
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
			else
			{
				if (this.removeProgressOnMiss)
				{
					base.RemoveProgress(this.difficultyConfig.targetValue, true);
				}
				text = "miss";
				assetReference = this.missSFX;
				this.inputEnabled = false;
				this.inputEnableCoroutine = base.StartCoroutine(base.InputEnableDelayed());
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingFailVibration, VibrationRegion.WholeBody, false);
			}
			GameManager.Instance.AudioPlayer.PlaySFX(assetReference, AudioLayer.SFX_UI, 1f, num);
			if (this.feedbackAnimationController != null)
			{
				this.feedbackAnimationController.SetTrigger(text);
			}
		}
	}

	[SerializeField]
	private GameObject indicatorObject;

	[SerializeField]
	protected List<Image> targetImages;

	protected List<HarvestMinigame.TargetConfig> targetConfigs;

	protected float indicatorAngle;

	private float prevIndicatorRotation;

	private bool isTrophyNotchShowing;

	private int specialTargetIndex;

	private VibrationData FishingBlipVibration;

	private VibrationData FishingSuccessVibration;

	private VibrationData FishingFailVibration;

	private enum HitResult
	{
		HIT,
		SPECIAL_HIT,
		MISS
	}
}
