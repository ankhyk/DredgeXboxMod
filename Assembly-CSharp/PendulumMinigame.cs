using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PendulumMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		base.Awake();
	}

	public override void PrepareGame(HarvestDifficultyConfigData difficultyConfig)
	{
		this.numSegmentsToUse = difficultyConfig.numPendulumSegments;
		for (int i = 0; i < 3; i++)
		{
			this.segments[i].SetActive(i < this.numSegmentsToUse);
		}
		this.activeSegmentIndex = 0;
		this.indicatorAngle = this.GetAngleForSegmentStart(this.activeSegmentIndex);
		this.RedrawIndicatorPosition();
		this.isPendulumSwingingRight = true;
		this.didHitTargetThisSwing = false;
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.shouldTrophyNotchSpawnInThisRound = showTrophyNotch;
		this.specialNotchCounter = this.notchesUntilSpecialNotchCanSpawn;
		this.targetConfigs = new HarvestMinigame.TargetConfig[this.numSegmentsToUse];
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
		this.equipmentSpeed = GameManager.Instance.PlayerStats.MinigameFishingSpeedModifier;
	}

	public override void StopGame()
	{
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
		base.StopGame();
	}

	public override void ResetGame()
	{
		this.indicatorAngle = this.GetAngleForSegmentStart(this.activeSegmentIndex);
		this.RedrawIndicatorPosition();
		base.ResetGame();
		this.canGenerateTargets = false;
	}

	protected override float GetCurrentIndicatorAngle()
	{
		return (this.indicatorAngle + 360f) % 360f;
	}

	protected override void ClearTargetImages()
	{
		for (int i = 0; i < this.segmentTargets.Count; i++)
		{
			this.segmentTargets[i].gameObject.SetActive(false);
			this.segmentTargets[i].color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
		}
	}

	protected override void ClearTargetConfigs()
	{
		this.targetConfigs = new HarvestMinigame.TargetConfig[this.numSegmentsToUse];
	}

	protected override void OnInputDisabled()
	{
		if (this.shouldTrophyNotchSpawnInThisRound)
		{
			this.shouldTrophyNotchSpawnInThisRound = false;
		}
		for (int i = 0; i < this.numSegmentsToUse; i++)
		{
			if (this.targetConfigs[i] != null)
			{
				if (this.targetConfigs[i].isSpecial)
				{
					this.GenerateTargetForSegment(i);
				}
				this.segmentTargets[i].color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
			}
		}
		this.progressDisabled = true;
	}

	protected override void OnInputReenabled()
	{
		for (int i = 0; i < this.numSegmentsToUse; i++)
		{
			if (this.targetConfigs[i] != null)
			{
				this.segmentTargets[i].color = (this.targetConfigs[i].isSpecial ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
			}
		}
		this.progressDisabled = false;
	}

	protected override void GenerateTargets()
	{
		for (int i = 0; i < this.numSegmentsToUse; i++)
		{
			this.GenerateTargetForSegment(i);
		}
		this.canGenerateTargets = true;
	}

	private void GenerateTargetForSegment(int segmentIndex)
	{
		HarvestMinigame.TargetConfig targetConfig = new HarvestMinigame.TargetConfig();
		Image image = this.segmentTargets[segmentIndex];
		bool flag = false;
		if (this.shouldTrophyNotchSpawnInThisRound)
		{
			this.specialNotchCounter--;
			if (this.specialNotchCounter <= 0)
			{
				flag = true;
				this.shouldTrophyNotchSpawnInThisRound = false;
			}
		}
		if (flag)
		{
			targetConfig.isSpecial = true;
			this.specialTargetIndex = segmentIndex;
			targetConfig.widthDeg = (float)this.difficultyConfig.specialTargetWidth;
		}
		else
		{
			targetConfig.widthDeg = (float)global::UnityEngine.Random.Range(this.difficultyConfig.minTargetWidth, this.difficultyConfig.maxTargetWidth);
		}
		this.targetConfigs[segmentIndex] = targetConfig;
		float num = targetConfig.widthDeg * 0.5f;
		float num2 = this.segmentAngleArcHalfWidth - num;
		float num4;
		if (this.numSegmentsToUse == 1)
		{
			int num3 = 0;
			do
			{
				num3++;
				if (this.lastTargetPos < 0f)
				{
					num4 = global::UnityEngine.Random.Range(0f, num2);
				}
				else if (this.lastTargetPos > 0f)
				{
					num4 = global::UnityEngine.Random.Range(-num2, 0f);
				}
				else
				{
					num4 = global::UnityEngine.Random.Range(-num2, num2);
				}
				if (num3 >= 10)
				{
					break;
				}
			}
			while (Mathf.Abs(this.lastTargetPos) - Mathf.Abs(num4) < this.segmentAngleArcHalfWidth * 0.5f);
		}
		else
		{
			num4 = global::UnityEngine.Random.Range(-num2, num2);
		}
		this.lastTargetPos = num4;
		num4 += num;
		image.transform.localEulerAngles = new Vector3(image.transform.localEulerAngles.x, image.transform.localEulerAngles.y, num4);
		image.fillAmount = targetConfig.widthDeg / 360f;
		image.color = (targetConfig.isSpecial ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
		image.gameObject.SetActive(true);
	}

	private void RedrawIndicatorPosition()
	{
		this.pendulumObject.transform.eulerAngles = new Vector3(this.pendulumObject.transform.eulerAngles.x, this.pendulumObject.transform.eulerAngles.y, this.indicatorAngle);
	}

	private float GetAngleForSegmentStart(int segmentIndex)
	{
		return this.segments[segmentIndex].transform.eulerAngles.z + this.segmentAngleArcHalfWidth;
	}

	private float GetAngleForSegmentEnd(int segmentIndex)
	{
		return this.segments[segmentIndex].transform.eulerAngles.z - this.segmentAngleArcHalfWidth;
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			if (this.isPendulumSwingingRight)
			{
				this.indicatorAngle -= this.difficultyConfig.rotationSpeed * Time.deltaTime;
				if (this.indicatorAngle < this.GetAngleForSegmentEnd(this.activeSegmentIndex))
				{
					this.isPendulumSwingingRight = false;
					this.didHitTargetThisSwing = false;
				}
			}
			else
			{
				this.indicatorAngle += this.difficultyConfig.rotationSpeed * Time.deltaTime;
				if (this.indicatorAngle > this.GetAngleForSegmentStart(this.activeSegmentIndex))
				{
					this.isPendulumSwingingRight = true;
					this.didHitTargetThisSwing = false;
				}
			}
			this.RedrawIndicatorPosition();
		}
		base.Update();
	}

	private PendulumMinigame.HitResult TryHitTarget()
	{
		if (!this.DoesHitTarget(this.targetConfigs[this.activeSegmentIndex]) || this.didHitTargetThisSwing)
		{
			return PendulumMinigame.HitResult.MISS;
		}
		this.didHitTargetThisSwing = true;
		if (!this.targetConfigs[this.activeSegmentIndex].isSpecial)
		{
			return PendulumMinigame.HitResult.HIT;
		}
		return PendulumMinigame.HitResult.SPECIAL_HIT;
	}

	private bool DoesHitTarget(HarvestMinigame.TargetConfig targetConfig)
	{
		if (targetConfig == null)
		{
			return false;
		}
		float num = 0f;
		float num2 = MathUtil.TransformAngleToNegative180Positive180(this.pendulumObject.transform.eulerAngles.z);
		float z = this.segmentTargets[this.activeSegmentIndex].transform.eulerAngles.z;
		float num3 = MathUtil.TransformAngleToNegative180Positive180(z + (this.isPendulumSwingingRight ? 0f : num));
		float num4 = MathUtil.TransformAngleToNegative180Positive180(z - this.targetConfigs[this.activeSegmentIndex].widthDeg - (this.isPendulumSwingingRight ? num : 0f));
		return MathUtil.IsBetween(num2, num3, num4);
	}

	public override void OnMinigameInteractPress()
	{
		if (this.inputEnabled)
		{
			AssetReference assetReference = null;
			PendulumMinigame.HitResult hitResult = this.TryHitTarget();
			float num = global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax);
			string text;
			if (hitResult == PendulumMinigame.HitResult.SPECIAL_HIT)
			{
				base.AddProgress(1f);
				text = "hit-special";
				this.didHitSpecialTarget = true;
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
			}
			else if (hitResult == PendulumMinigame.HitResult.HIT)
			{
				base.AddProgress(this.difficultyConfig.targetValue * this.equipmentSpeed);
				text = "hit";
				if (base.Progress < 1f)
				{
					assetReference = this.hitSFX;
					GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingBlipVibration, VibrationRegion.WholeBody, false);
					this.MoveToNextSegment();
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

	private void MoveToNextSegment()
	{
		this.didHitTargetThisSwing = false;
		int num = this.activeSegmentIndex;
		this.GenerateTargetForSegment(this.activeSegmentIndex);
		this.activeSegmentIndex += (this.isPendulumSwingingRight ? 1 : (-1));
		this.activeSegmentIndex = MathUtil.NegativeMod(this.activeSegmentIndex, this.numSegmentsToUse);
		if (this.activeSegmentIndex != num)
		{
			this.indicatorAngle = (this.isPendulumSwingingRight ? this.GetAngleForSegmentStart(this.activeSegmentIndex) : this.GetAngleForSegmentEnd(this.activeSegmentIndex));
		}
	}

	[SerializeField]
	private GameObject pendulumObject;

	[SerializeField]
	protected List<GameObject> segments;

	[SerializeField]
	protected List<Image> segmentTargets;

	[SerializeField]
	private float segmentAngleArcHalfWidth;

	[SerializeField]
	private int numSegmentsToUse = 3;

	[SerializeField]
	private int notchesUntilSpecialNotchCanSpawn = 3;

	protected HarvestMinigame.TargetConfig[] targetConfigs;

	protected float indicatorAngle;

	private bool shouldTrophyNotchSpawnInThisRound;

	private int specialNotchCounter;

	private int specialTargetIndex;

	private int activeSegmentIndex;

	private bool didHitTargetThisSwing;

	private bool isPendulumSwingingRight;

	private bool canGenerateTargets;

	private float lastTargetPos;

	private enum HitResult
	{
		HIT,
		SPECIAL_HIT,
		MISS
	}
}
