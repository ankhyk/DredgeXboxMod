using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SpiralMinigame : HarvestMinigame
{
	protected override void Awake()
	{
		base.Awake();
		this.spiralImage.transform.rotation = Quaternion.Euler(0f, 0f, this.spiralConfig.baseGameRotation);
	}

	protected override float GetCurrentIndicatorAngle()
	{
		return 0f;
	}

	protected override void OnInputDisabled()
	{
		this.progressDisabled = true;
		this.DisableAllNotches();
	}

	protected override void OnInputReenabled()
	{
		this.progressDisabled = false;
		this.ReenableAllNotches();
	}

	protected override void GenerateTargets()
	{
		this.canGenerateTargets = true;
		int spiralNumNotches = this.difficultyConfig.spiralNumNotches;
		float num = 1f / (float)spiralNumNotches;
		this.notches = new List<Vector2>();
		for (int i = 0; i < spiralNumNotches; i++)
		{
			Vector2 vector = default(Vector2);
			vector.y = global::UnityEngine.Random.Range(this.difficultyConfig.spiralMinNotchWidth, this.difficultyConfig.spiralMaxNotchWidth);
			float num2 = ((i == 0) ? this.startDeadzone : 0f);
			float num3 = ((i == spiralNumNotches - 1) ? (num - this.endDeadzone - vector.y) : (num - vector.y));
			vector.x = global::UnityEngine.Random.Range(num2, num3) + num * (float)i;
			this.notches.Add(vector);
		}
		this.gateStates = new List<bool>();
		this.gateAnimators = new List<Animator>();
		for (int j = 0; j < this.maxNumNotches; j++)
		{
			this.gateStates.Add(false);
		}
		this.trophyNotchIndex = (this.showTrophyNotch ? 1 : (-1));
		Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE);
		Color color2 = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
		for (int k = 0; k < this.maxNumNotches; k++)
		{
			bool flag = this.trophyNotchIndex == k;
			string text = string.Format("_notch{0}", k + 1);
			if (k < this.notches.Count)
			{
				this.CreateNotch(this.notches[k], text, flag ? color : color2);
			}
			else
			{
				this.HideNotch(text);
			}
		}
	}

	private void CreateNotch(Vector2 notchConfig, string notchPrefix, Color color)
	{
		float num = notchConfig.x + notchConfig.y;
		this.spiralImage.material.SetFloat(notchPrefix + "_start", notchConfig.x);
		this.spiralImage.material.SetFloat(notchPrefix + "_end", num);
		this.spiralImage.material.SetFloat(notchPrefix + "_opacity", 1f);
		this.spiralImage.material.SetColor(notchPrefix + "_color", color);
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.gatePrefab, this.gateContainer);
		SpiralComponent component = gameObject.GetComponent<SpiralComponent>();
		this.gateAnimators.Add(gameObject.GetComponent<Animator>());
		component.SetPosition(num, this.spiralConfig);
		component.SetRotation(num, this.spiralConfig);
	}

	private void HideNotch(string notchPrefix)
	{
		this.spiralImage.material.SetFloat(notchPrefix + "_start", 0f);
		this.spiralImage.material.SetFloat(notchPrefix + "_end", 0f);
		this.spiralImage.material.SetFloat(notchPrefix + "_opacity", 0f);
	}

	private void DisableAllNotches()
	{
		Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED);
		for (int i = 0; i < this.maxNumNotches; i++)
		{
			this.SetNotchToColorByIndex(i, color);
		}
	}

	private void ReenableAllNotches()
	{
		Color color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE);
		Color color2 = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED);
		for (int i = 0; i < this.maxNumNotches; i++)
		{
			this.SetNotchToColorByIndex(i, this.gateStates[i] ? color2 : color);
		}
	}

	private void SetNotchToColorByIndex(int index, Color color)
	{
		string text = string.Format("_notch{0}", index + 1);
		this.spiralImage.material.SetColor(text + "_color", color);
	}

	private void DestroyAllGates()
	{
		for (int i = this.gateContainer.transform.childCount - 1; i >= 0; i--)
		{
			if (Application.isPlaying)
			{
				global::UnityEngine.Object.Destroy(this.gateContainer.transform.GetChild(i).gameObject);
			}
			else
			{
				global::UnityEngine.Object.DestroyImmediate(this.gateContainer.transform.GetChild(i).gameObject);
			}
		}
	}

	protected override void ClearTargetImages()
	{
		this.ClearBoard();
	}

	private void ClearBoard()
	{
		for (int i = 0; i < this.maxNumNotches; i++)
		{
			this.HideNotch(string.Format("_notch{0}", i + 1));
		}
		this.DestroyAllGates();
	}

	protected override void ClearTargetConfigs()
	{
		this.gateStates = new List<bool>();
		this.notches = new List<Vector2>();
	}

	public override void OnMinigameInteractPress()
	{
		AssetReference assetReference = null;
		float num = global::UnityEngine.Random.Range(this.randomPitchMin, this.randomPitchMax);
		if (this.inputEnabled)
		{
			SpiralMinigame.HitResult hitResult = SpiralMinigame.HitResult.MISS;
			int notchByProp = this.GetNotchByProp(this.currentBallProgressProp);
			if (notchByProp == -1)
			{
				hitResult = SpiralMinigame.HitResult.MISS;
			}
			else if (this.trophyNotchIndex == notchByProp)
			{
				hitResult = SpiralMinigame.HitResult.SPECIAL_HIT;
			}
			else if (!this.gateStates[notchByProp])
			{
				hitResult = SpiralMinigame.HitResult.HIT;
			}
			else if (this.gateStates[notchByProp])
			{
				hitResult = SpiralMinigame.HitResult.MISS;
			}
			string text;
			if (hitResult == SpiralMinigame.HitResult.SPECIAL_HIT)
			{
				base.AddProgress(1f);
				text = "hit-special";
				this.didHitSpecialTarget = true;
				GameManager.Instance.VibrationManager.Vibrate(GameManager.Instance.Player.FishingSuccessVibration, VibrationRegion.WholeBody, false);
			}
			else if (hitResult == SpiralMinigame.HitResult.HIT)
			{
				base.AddProgress(this.difficultyConfig.targetValue * this.equipmentSpeed * this.difficultyConfig.spiralValueFactor);
				text = "hit";
				this.gateAnimators[notchByProp].SetBool("open", true);
				this.SetNotchToColorByIndex(notchByProp, GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.DISABLED));
				this.gateStates[notchByProp] = true;
				this.justOpenedGateIndexThatNeedsClosing = notchByProp;
				this.currentlyBlockingGateIndex++;
				this.RefreshNextGateBounds();
				GameManager.Instance.AudioPlayer.PlaySFX(this.openGate, AudioLayer.SFX_UI, 1f, 1f);
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
				this.trophyNotchIndex = -1;
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

	private int GetNotchByProp(float prop)
	{
		int num = -1;
		for (int i = 0; i < this.notches.Count; i++)
		{
			Vector2 vector = this.notches[i];
			if (prop >= vector.x - this.preNotchPadding && prop <= vector.x + vector.y)
			{
				num = i;
				break;
			}
		}
		return num;
	}

	public override void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.showTrophyNotch = showTrophyNotch;
		this.showTargets = showTargets;
		this.movingForward = true;
		base.StartGame(difficultyConfig, currentPOI, showTargets, showTrophyNotch);
		this.equipmentSpeed = GameManager.Instance.PlayerStats.MinigameFishingSpeedModifier;
		this.currentlyBlockingGateIndex = 0;
		this.justOpenedGateIndexThatNeedsClosing = -1;
		this.RefreshPrevGateBounds();
		this.RefreshNextGateBounds();
	}

	public override void PrepareGame(HarvestDifficultyConfigData difficultyConfig)
	{
		base.PrepareGame(difficultyConfig);
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
		this.currentlyBlockingGateIndex = 0;
		this.currentBallProgressProp = 0f;
		this.ballSpiralComponent.SetPosition(this.currentBallProgressProp, this.spiralConfig);
	}

	private void RefreshPrevGateBounds()
	{
		if (this.currentlyBlockingGateIndex == 0)
		{
			this.prevGateProp = 0f;
			return;
		}
		this.prevGateProp = this.notches[this.currentlyBlockingGateIndex - 1].x + this.notches[this.currentlyBlockingGateIndex - 1].y;
		this.prevGateProp += this.preAndPostGateBufferProp;
	}

	private void RefreshNextGateBounds()
	{
		if (this.currentlyBlockingGateIndex == this.notches.Count)
		{
			this.nextGateProp = 1f;
			return;
		}
		this.nextGateProp = this.notches[this.currentlyBlockingGateIndex].x + this.notches[this.currentlyBlockingGateIndex].y;
		this.nextGateProp -= this.preAndPostGateBufferProp;
	}

	protected override void Update()
	{
		if (this.isGameRunning)
		{
			if (this.movingForward && this.currentBallProgressProp > this.nextGateProp && (this.currentlyBlockingGateIndex >= this.gateStates.Count || !this.gateStates[this.currentlyBlockingGateIndex]))
			{
				this.movingForward = false;
				GameManager.Instance.AudioPlayer.PlaySFX(this.hitGate, AudioLayer.SFX_UI, 1f, 1f);
			}
			else if (!this.movingForward && this.currentBallProgressProp < this.prevGateProp)
			{
				this.movingForward = true;
				GameManager.Instance.AudioPlayer.PlaySFX(this.hitGate, AudioLayer.SFX_UI, 1f, 1f);
			}
			if (this.movingForward)
			{
				this.currentBallProgressProp += this.baseSpeed * this.difficultyConfig.spiralRotationSpeed * Time.deltaTime;
			}
			else
			{
				this.currentBallProgressProp -= this.baseSpeed * this.difficultyConfig.spiralRotationSpeed * Time.deltaTime;
			}
			if (this.currentBallProgressProp >= 1f && this.showTargets)
			{
				base.AddProgress(1f);
				this.currentBallProgressProp = 1f;
			}
			this.ballSpiralComponent.SetPosition(this.currentBallProgressProp, this.spiralConfig);
			if (this.justOpenedGateIndexThatNeedsClosing != -1 && this.currentBallProgressProp > this.GetGatePropByNotchIndex(this.justOpenedGateIndexThatNeedsClosing))
			{
				this.gateAnimators[this.justOpenedGateIndexThatNeedsClosing].SetBool("open", false);
				GameManager.Instance.AudioPlayer.PlaySFX(this.closeGate, AudioLayer.SFX_UI, 1f, 1f);
				this.justOpenedGateIndexThatNeedsClosing = -1;
				this.RefreshPrevGateBounds();
			}
		}
		base.Update();
	}

	private float GetGatePropByNotchIndex(int index)
	{
		return this.notches[index].x + this.notches[index].y;
	}

	[SerializeField]
	private Image spiralImage;

	[SerializeField]
	private Transform gateContainer;

	[SerializeField]
	private GameObject gatePrefab;

	[SerializeField]
	private float baseSpeed;

	[SerializeField]
	protected SpiralComponent ballSpiralComponent;

	[SerializeField]
	private float startDeadzone;

	[SerializeField]
	private float endDeadzone;

	[SerializeField]
	private float preAndPostGateBufferProp;

	[SerializeField]
	private float preNotchPadding;

	[SerializeField]
	private int maxNumNotches = 5;

	[SerializeField]
	private SpiralMinigame.SpiralConfig spiralConfig;

	[SerializeField]
	private AssetReference openGate;

	[SerializeField]
	private AssetReference closeGate;

	[SerializeField]
	private AssetReference hitGate;

	private float currentBallProgressProp;

	private List<bool> gateStates;

	private List<Vector2> notches;

	private List<Animator> gateAnimators;

	private bool canGenerateTargets;

	private bool showTrophyNotch;

	private int trophyNotchIndex;

	private bool movingForward;

	private int currentlyBlockingGateIndex;

	private int justOpenedGateIndexThatNeedsClosing;

	private float prevGateProp;

	private float nextGateProp;

	private bool showTargets;

	[Serializable]
	public struct SpiralConfig
	{
		[SerializeField]
		public float baseGameRotation;

		[SerializeField]
		public float startRadius;

		[SerializeField]
		public float endRadius;

		[SerializeField]
		public float totalDegrees;

		[SerializeField]
		public float startAngleOffset;

		[SerializeField]
		public float direction;
	}

	private enum HitResult
	{
		HIT,
		SPECIAL_HIT,
		MISS,
		AVOID
	}
}
