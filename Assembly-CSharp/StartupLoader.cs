using System;
using System.Collections;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class StartupLoader : MonoBehaviour
{
	private void Awake()
	{
		this.splashContent.SetActive(false);
	}

	private void Start()
	{
		base.StartCoroutine(this.Load());
	}

	private IEnumerator Load()
	{
		TAnimBuilder.InitializeGlobalDatabase();
		Shader.SetGlobalFloat("_WorldSize", GameManager.Instance.GameConfigData.WorldSize);
		while (!LocalizationSettings.InitializationOperation.IsDone)
		{
			yield return null;
		}
		GameManager.Instance.UpgradeManager.LoadData();
		while (!GameManager.Instance.UpgradeManager.HasLoadedUpgradeData)
		{
			yield return null;
		}
		GameManager instance = GameManager.Instance;
		if (instance.hasBankedId)
		{
			SaveData saveData = GameManager.Instance.SaveManager.LoadIntoMemory(GameManager.Instance.SaveManager.ActiveSettingsData.lastSaveSlot);
			if ((saveData.GetSaveUsesEntitlement(Entitlement.DLC_1) && !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_1)) || (saveData.GetSaveUsesEntitlement(Entitlement.DLC_2) && !GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2)))
			{
				this.OnLoadComplete();
			}
			else
			{
				instance.Loader.LoadGameFromStartup();
			}
		}
		else
		{
			this.OnLoadComplete();
		}
		yield break;
	}

	private void OnLoadComplete()
	{
		if (!GameManager.Instance.SaveManager.HasAnySaveFiles() && !GameManager.Instance.hasBankedId)
		{
			this.splashContent.SetActive(true);
			this.continueAction = new DredgePlayerActionHold("prompt.begin", GameManager.Instance.Input.Controls.Interact, 0.5f);
			this.continueAction.allowPreholding = true;
			DredgePlayerActionHold dredgePlayerActionHold = this.continueAction;
			dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.LoadTitle));
			this.controlPrompt.Init(this.continueAction);
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.continueAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
			return;
		}
		this.LoadTitle();
	}

	private void LoadTitle()
	{
		this.controlPromptGroup.SetActive(false);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.continueAction };
		input.RemoveActionListener(array, ActionLayer.SYSTEM);
		if (!this.hasReceivedInput)
		{
			this.hasReceivedInput = true;
			GameManager.Instance.Loader.LoadTitleFromStartup();
		}
	}

	private void Update()
	{
		this.timeSinceLoad += Time.deltaTime;
		if (this.timeSinceLoad > this.attentionEnableTime && !this.attentionGrabber.activeSelf && this.attentionGrabber != null)
		{
			this.attentionGrabber.SetActive(true);
		}
	}

	[SerializeField]
	private GameObject splashContent;

	[SerializeField]
	private GameObject controlPromptGroup;

	[SerializeField]
	private ControlPromptIcon controlPrompt;

	[SerializeField]
	private GameObject attentionGrabber;

	[SerializeField]
	private float attentionEnableTime;

	private bool hasReceivedInput;

	private DredgePlayerActionHold continueAction;

	private float timeSinceLoad;
}
