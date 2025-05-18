using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DredgeMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		this.outerTargetConfigs = new List<HarvestMinigame.TargetConfig>();
		this.innerTargetConfigs = new List<HarvestMinigame.TargetConfig>();
		base.Awake();
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.equipmentSpeed = GameManager.Instance.PlayerStats.DredgingSpeedModifier;
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
	}

	public override void StopGame()
	{
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.DredgingSuccessVibration, VibrationRegion.WholeBody, true);
		base.StopGame();
	}

	public override void ResetGame()
	{
		this.isInner = false;
		this.currentRotationOffset = 0f;
		this.UpdateRingPositions();
		this.innerIndicator.gameObject.SetActive(this.isInner);
		this.outerIndicator.gameObject.SetActive(!this.isInner);
		base.ResetGame();
	}

	private void UpdateRingPositions()
	{
		this.currentRotationOffset = (this.currentRotationOffset + 360f) % 360f;
		this.innerRing.transform.eulerAngles = new Vector3(this.innerRing.transform.eulerAngles.x, this.innerRing.transform.eulerAngles.y, this.currentRotationOffset);
		this.outerRing.transform.eulerAngles = new Vector3(this.outerRing.transform.eulerAngles.x, this.outerRing.transform.eulerAngles.y, this.currentRotationOffset);
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			float rotationSpeed = this.difficultyConfig.rotationSpeed;
			this.currentRotationOffset += rotationSpeed * Time.deltaTime;
			this.UpdateRingPositions();
			if (this.IsOverlappingTarget())
			{
				if (!this.progressDisabled)
				{
					base.RemoveProgress(this.difficultyConfig.targetValue, true);
					if (this.feedbackAnimationController != null)
					{
						this.feedbackAnimationController.SetTrigger("miss");
					}
					GameManager.Instance.AudioPlayer.PlaySFX(this.hitSFX, AudioLayer.SFX_UI, 1f, global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax));
					this.timeUntilProgressReenabled = this.inputDisablePenaltySec;
				}
				this.progressDisabled = true;
				base.RemoveProgress(0.01f, false);
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.DredgingFailVibration, VibrationRegion.WholeBody, true);
			}
			this.timeUntilProgressReenabled -= Time.deltaTime;
			if (this.timeUntilProgressReenabled <= 0f)
			{
				this.progressDisabled = false;
			}
			if (!GameManager.Instance.VibrationManager.IsPlayingVibration())
			{
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.DredgingContinuousVibration, VibrationRegion.WholeBody, false);
			}
		}
		base.Update();
	}

	private void LateUpdate()
	{
		if (this.isGameRunning)
		{
			this.innerRingImage.color = (this.isInner ? this.activeRingColor : this.inactiveRingColor);
			this.outerRingImage.color = ((!this.isInner) ? this.activeRingColor : this.inactiveRingColor);
		}
	}

	protected override void ClearTargetImages()
	{
		for (int i = 0; i < this.outerTargetImages.Count; i++)
		{
			this.outerTargetImages[i].gameObject.SetActive(false);
			this.outerTargetImages[i].color = Color.black;
		}
		for (int j = 0; j < this.innerTargetImages.Count; j++)
		{
			this.innerTargetImages[j].gameObject.SetActive(false);
			this.innerTargetImages[j].color = Color.black;
		}
	}

	protected override void OnInputDisabled()
	{
		base.RemoveProgress(this.difficultyConfig.targetValue, true);
		if (this.feedbackAnimationController != null)
		{
			this.feedbackAnimationController.SetTrigger("miss");
		}
		for (int i = 0; i < this.outerTargetImages.Count; i++)
		{
			this.outerTargetImages[i].color = Color.black;
		}
		for (int j = 0; j < this.innerTargetImages.Count; j++)
		{
			this.innerTargetImages[j].color = Color.black;
		}
	}

	protected override void OnInputReenabled()
	{
		for (int i = 0; i < this.outerTargetImages.Count; i++)
		{
			this.outerTargetImages[i].color = Color.black;
		}
		for (int j = 0; j < this.innerTargetImages.Count; j++)
		{
			this.innerTargetImages[j].color = Color.black;
		}
	}

	protected override void GenerateTargets()
	{
		int num = global::UnityEngine.Random.Range(this.difficultyConfig.minTargets, this.difficultyConfig.maxTargets + 1);
		this.outerTargetConfigs.Clear();
		this.innerTargetConfigs.Clear();
		float num2 = 60f;
		float num3 = 20f;
		float num4 = (360f - num2) / (float)num;
		bool flag = true;
		for (int i = 0; i < num; i++)
		{
			HarvestMinigame.TargetConfig targetConfig = new HarvestMinigame.TargetConfig();
			targetConfig.widthDeg = (float)global::UnityEngine.Random.Range(this.difficultyConfig.minTargetWidth, this.difficultyConfig.maxTargetWidth) * this.widthFactor;
			float num5 = num4 * (float)i + num4 * 0.5f;
			float num6 = num4 - targetConfig.widthDeg - num3;
			float num7 = num5 - num6 * 0.5f;
			float num8 = num5 + num6 * 0.5f;
			targetConfig.angleDeg = global::UnityEngine.Random.Range(num7, num8);
			if (i == num - 1)
			{
				flag = true;
			}
			else if ((double)global::UnityEngine.Random.value < 0.9)
			{
				flag = !flag;
			}
			if (flag)
			{
				this.innerTargetConfigs.Add(targetConfig);
			}
			else
			{
				this.outerTargetConfigs.Add(targetConfig);
			}
		}
		for (int j = 0; j < this.outerTargetConfigs.Count; j++)
		{
			this.ConfigureTargetImage(this.outerTargetImages[j], this.outerTargetConfigs[j]);
		}
		for (int k = 0; k < this.innerTargetConfigs.Count; k++)
		{
			this.ConfigureTargetImage(this.innerTargetImages[k], this.innerTargetConfigs[k]);
		}
	}

	private void ConfigureTargetImage(Image targetImage, HarvestMinigame.TargetConfig targetConfig)
	{
		targetImage.gameObject.SetActive(true);
		float num = targetConfig.angleDeg;
		num += targetConfig.widthDeg * 0.5f;
		targetImage.transform.eulerAngles = new Vector3(targetImage.transform.eulerAngles.x, targetImage.transform.eulerAngles.y, num);
		targetImage.fillAmount = targetConfig.widthDeg / 360f;
	}

	private bool TestTargetProximity(HarvestMinigame.TargetConfig targetConfig)
	{
		float proximityThreshold = 30f;
		float thisMinBound = targetConfig.angleDeg - targetConfig.widthDeg * 0.5f;
		float thisMaxBound = targetConfig.angleDeg + targetConfig.widthDeg * 0.5f;
		this.collisionFreeOuter = this.outerTargetConfigs.TrueForAll(delegate(HarvestMinigame.TargetConfig tc)
		{
			float num = tc.angleDeg - tc.widthDeg * 0.5f;
			float num2 = tc.angleDeg + tc.widthDeg * 0.5f;
			return Mathf.Abs(thisMinBound - num2) > proximityThreshold && Mathf.Abs(num - thisMaxBound) > proximityThreshold;
		});
		this.collisionFreeInner = this.innerTargetConfigs.TrueForAll(delegate(HarvestMinigame.TargetConfig tc)
		{
			float num3 = tc.angleDeg - tc.widthDeg * 0.5f;
			float num4 = tc.angleDeg + tc.widthDeg * 0.5f;
			return Mathf.Abs(thisMinBound - num4) > proximityThreshold && Mathf.Abs(num3 - thisMaxBound) > proximityThreshold;
		});
		return this.collisionFreeOuter && this.collisionFreeInner;
	}

	private void SwitchLane()
	{
		this.isInner = !this.isInner;
		this.innerIndicator.gameObject.SetActive(this.isInner);
		this.outerIndicator.gameObject.SetActive(!this.isInner);
		GameManager.Instance.AudioPlayer.PlaySFX(this.specialSFX, AudioLayer.SFX_UI, 1f, global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax));
		GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.DredgingSwitchLaneVibration, VibrationRegion.WholeBody, true);
	}

	private bool IsOverlappingTarget()
	{
		float currentIndicatorAngle = this.GetCurrentIndicatorAngle();
		bool flag = false;
		List<HarvestMinigame.TargetConfig> list = (this.isInner ? this.innerTargetConfigs : this.outerTargetConfigs);
		for (int i = 0; i < list.Count; i++)
		{
			HarvestMinigame.TargetConfig targetConfig = list[i];
			float num = targetConfig.angleDeg - targetConfig.widthDeg * 0.5f;
			float num2 = targetConfig.angleDeg + targetConfig.widthDeg * 0.5f;
			if (currentIndicatorAngle > num && currentIndicatorAngle < num2)
			{
				flag = true;
			}
		}
		return flag;
	}

	public override void OnMinigameInteractPress()
	{
		if (this.inputEnabled)
		{
			this.SwitchLane();
		}
	}

	protected override float GetCurrentIndicatorAngle()
	{
		return (-this.currentRotationOffset + 360f) % 360f;
	}

	protected override void ClearTargetConfigs()
	{
	}

	[SerializeField]
	protected List<Image> outerTargetImages;

	[SerializeField]
	protected List<Image> innerTargetImages;

	[SerializeField]
	protected GameObject outerRing;

	[SerializeField]
	protected GameObject innerRing;

	[SerializeField]
	protected Image outerRingImage;

	[SerializeField]
	protected Image innerRingImage;

	[SerializeField]
	protected Image outerIndicator;

	[SerializeField]
	protected Image innerIndicator;

	[SerializeField]
	protected float widthFactor;

	[SerializeField]
	protected Color activeRingColor;

	[SerializeField]
	protected Color inactiveRingColor;

	protected List<HarvestMinigame.TargetConfig> outerTargetConfigs;

	protected List<HarvestMinigame.TargetConfig> innerTargetConfigs;

	private bool isInner;

	private float currentRotationOffset;

	private float timeUntilProgressReenabled;

	private bool collisionFreeOuter;

	private bool collisionFreeInner;
}
