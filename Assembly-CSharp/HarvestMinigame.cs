using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class HarvestMinigame : MonoBehaviour
{
	public float Progress
	{
		get
		{
			return this.progress;
		}
	}

	public bool IsGameRunning
	{
		get
		{
			return this.isGameRunning;
		}
	}

	public bool DidHitSpecialTarget
	{
		get
		{
			return this.didHitSpecialTarget;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnEnable()
	{
		this.feedbackAnimationController.runtimeAnimatorController = this.controller;
	}

	public virtual void StartGame(HarvestDifficultyConfigData difficultyConfig, HarvestPOI currentPOI, bool showTargets, bool showTrophyNotch)
	{
		this.ResetGame();
		this.difficultyConfig = difficultyConfig;
		this.currentPOI = currentPOI;
		if (showTargets)
		{
			this.GenerateTargets();
		}
		else
		{
			this.ClearTargetConfigs();
		}
		this.loopAudioSource.clip = this.loopSFX;
		this.loopAudioSource.Play();
		this.inputEnabled = true;
		this.isGameRunning = true;
		if (this.feedbackAnimationController != null)
		{
			this.feedbackAnimationController.SetTrigger("start");
		}
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.FISHING);
	}

	public virtual void StopGame()
	{
		this.loopAudioSource.Stop();
		GameManager.Instance.AudioPlayer.PlaySFX(this.endSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.isGameRunning = false;
		this.ClearTargetImages();
		if (this.feedbackAnimationController != null)
		{
			this.feedbackAnimationController.SetTrigger(this.didHitSpecialTarget ? "end-special" : "end");
		}
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
	}

	protected virtual void Update()
	{
		if (this.isGameRunning)
		{
			if (this.progressDisabled)
			{
				this.progressChange = 0f;
			}
			else if (this.currentPOI.IsDredgePOI)
			{
				this.progressChange = 1f / (this.difficultyConfig.secondsToPassivelyCatch / this.equipmentSpeed) * Time.deltaTime;
			}
			else
			{
				this.progressChange = 1f / this.difficultyConfig.secondsToPassivelyCatch * Time.deltaTime;
			}
			this.progress += this.progressChange;
		}
	}

	public virtual void PrepareGame(HarvestDifficultyConfigData difficultyConfig)
	{
	}

	public virtual void ResetGame()
	{
		this.isGameRunning = false;
		if (this.inputEnableCoroutine != null)
		{
			base.StopCoroutine(this.inputEnableCoroutine);
			this.inputEnableCoroutine = null;
		}
		this.inputEnabled = false;
		this.targetIndexesHitThisRotation.Clear();
		this.progress = 0f;
		this.progressDisabled = false;
		this.didHitSpecialTarget = false;
		this.ClearTargetImages();
		if (this.loopAudioSource.isPlaying)
		{
			this.loopAudioSource.Stop();
		}
	}

	protected IEnumerator InputEnableDelayed()
	{
		this.OnInputDisabled();
		yield return new WaitForSeconds(this.inputDisablePenaltySec);
		this.OnInputReenabled();
		this.inputEnabled = true;
		yield break;
	}

	protected void RemoveProgress(float amount, bool initialHit = true)
	{
		if (GameManager.Instance.SettingsSaveData.noFailBehaviour == 1)
		{
			return;
		}
		this.progress -= amount;
		this.progress = Mathf.Clamp01(this.progress);
		if (initialHit)
		{
			Action onProgressRemoved = this.OnProgressRemoved;
			if (onProgressRemoved == null)
			{
				return;
			}
			onProgressRemoved();
		}
	}

	protected void AddProgress(float amount)
	{
		this.progress += amount;
		this.progress = Mathf.Clamp01(this.progress);
	}

	protected abstract float GetCurrentIndicatorAngle();

	protected abstract void OnInputDisabled();

	protected abstract void OnInputReenabled();

	protected abstract void GenerateTargets();

	protected abstract void ClearTargetImages();

	protected abstract void ClearTargetConfigs();

	public abstract void OnMinigameInteractPress();

	[SerializeField]
	public HarvestMinigameType harvestMinigameType;

	[SerializeField]
	protected float inputDisablePenaltySec;

	[SerializeField]
	protected bool removeProgressOnMiss;

	[SerializeField]
	protected Animator feedbackAnimationController;

	[SerializeField]
	protected RuntimeAnimatorController controller;

	[SerializeField]
	protected AudioSource loopAudioSource;

	[SerializeField]
	protected AssetReference hitSFX;

	[SerializeField]
	protected AssetReference missSFX;

	[SerializeField]
	protected AssetReference specialSFX;

	[SerializeField]
	protected AssetReference endSFX;

	[SerializeField]
	protected AudioClip loopSFX;

	[SerializeField]
	protected float indicatorWidthDeg;

	[SerializeField]
	protected float randomPitchMin;

	[SerializeField]
	protected float randomPitchMax;

	protected bool isGameRunning;

	protected float equipmentSpeed;

	protected bool progressDisabled;

	private float progress;

	protected float progressChange;

	protected bool didHitSpecialTarget;

	protected HarvestDifficultyConfigData difficultyConfig;

	protected List<int> targetIndexesHitThisRotation = new List<int>();

	protected bool inputEnabled;

	protected Coroutine inputEnableCoroutine;

	protected HarvestPOI currentPOI;

	public Action OnProgressRemoved;

	protected internal class TargetConfig
	{
		public float angleDeg;

		public float widthDeg;

		public bool isSpecial;
	}
}
