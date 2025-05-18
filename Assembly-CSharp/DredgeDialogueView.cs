using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Febucci.UI;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Yarn.Unity;

public class DredgeDialogueView : DialogueViewBase
{
	private void Awake()
	{
		this.container.SetActive(false);
		this.characterNameContainer.SetActive(false);
		this.optionsContainer.SetActive(false);
		this.localizedDialogueTextField.gameObject.SetActive(false);
		this.continueLineAction = new DredgePlayerActionPress("prompt.continue", GameManager.Instance.Input.Controls.Confirm);
		this.continueLineAction.showInControlArea = true;
		this.continueLineAction.allowPreholding = false;
		this.continueLineAction.priority = 0;
		DredgePlayerActionPress dredgePlayerActionPress = this.continueLineAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnContinueLinePressComplete));
		this.exitLineAction = new DredgePlayerActionPress("prompt.back", GameManager.Instance.Input.Controls.Back);
		this.exitLineAction.showInControlArea = true;
		this.exitLineAction.allowPreholding = false;
		this.exitLineAction.priority = 1;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.exitLineAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnExitLinePressComplete));
		this.textAnimatorPlayer.onTypewriterStart.AddListener(new UnityAction(this.OnTypewriterStarted));
		this.textAnimatorPlayer.onTextShowed.AddListener(new UnityAction(this.OnTypewriterCompleted));
		GameManager.Instance.DialogueRunner.onNodeComplete.AddListener(new UnityAction<string>(this.OnNodeComplete));
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		this.OnSettingChanged(SettingType.TEXT_SPEED);
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.TEXT_SPEED)
		{
			this.textAnimatorPlayer.SetTypewriterSpeed(this.typewriterSpeeds[GameManager.Instance.SettingsSaveData.textSpeed]);
		}
	}

	private void OnDestroy()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.continueLineAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnContinueLinePressComplete));
	}

	private void OnNodeComplete(string nodeName)
	{
		GameManager.Instance.SaveData.visitedNodes.Add(nodeName);
		GameEvents.Instance.TriggerNodeVisited(nodeName);
	}

	public void Hide()
	{
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.continueLineAction }, ActionLayer.DIALOGUE);
		this.needsEnabling = true;
		this.lastSpeakerPortrait = null;
		this.localizedDialogueTextField.gameObject.SetActive(false);
		this.characterNameContainer.SetActive(false);
		foreach (object obj in this.characterPortraitContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		base.StartCoroutine(this.TransitionDialogueBoxOut());
	}

	private IEnumerator TransitionDialogueBoxOut()
	{
		this.dialogueTextContainerEffect.Hide(true);
		yield return new WaitForSeconds(this.dialogueTextContainerEffect.duration);
		this.container.SetActive(false);
		yield break;
	}

	public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
	{
		this.lineCompleteTime = float.PositiveInfinity;
		this.onDialogueLineFinished = onDialogueLineFinished;
		string text = Markup.Parse(dialogueLine.TextWithoutCharacterName);
		this.textAnimatorPlayer.textAnimator.SetText(text, true);
		this.textAnimatorPlayer.textAnimator.ResetEffectsTime(false);
		this.textAnimatorPlayer.StartShowingText(true);
		if (!GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.DIALOGUE))
		{
			ApplicationEvents.Instance.TriggerUIWindowToggled(UIWindowType.DIALOGUE, true);
		}
		SpeakerData speakerData = null;
		if (!string.IsNullOrEmpty(dialogueLine.CharacterName))
		{
			this.speakerDataLookup.lookupTable.TryGetValue(dialogueLine.CharacterName.ToUpper(), out speakerData);
		}
		bool flag = false;
		if (speakerData)
		{
			this.ShowPortrait(speakerData);
			if (!string.IsNullOrEmpty(dialogueLine.CharacterName) && !speakerData.hideNameplate)
			{
				string text2 = speakerData.speakerNameKey;
				if (speakerData.speakerNameKeyOverrides.Count > 0)
				{
					NameKeyOverride nameKeyOverride = speakerData.speakerNameKeyOverrides.FirstOrDefault((NameKeyOverride o) => o.nodesVisited.All((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n)));
					if (!string.IsNullOrEmpty(nameKeyOverride.speakerNameKey))
					{
						text2 = nameKeyOverride.speakerNameKey;
					}
				}
				this.characterNameTextField.StringReference.SetReference(LanguageManager.CHARACTER_TABLE, text2);
				this.characterNameTextField.RefreshString();
				flag = true;
			}
			this.PlayAnyParalinguistic(dialogueLine.Metadata, speakerData);
		}
		else if (this.lastSpeakerData)
		{
			this.PlayAnyParalinguistic(dialogueLine.Metadata, this.lastSpeakerData);
		}
		if (flag)
		{
			this.characterNameContainer.SetActive(true);
			this.dialogBoxBackground.sprite = this.speechBackgroundSprite;
			this.dialogBoxBackground.color = Color.white;
			this.dialogueTextField.color = Color.white;
		}
		else
		{
			this.characterNameContainer.SetActive(false);
			this.dialogBoxBackground.sprite = this.plainBackgroundSprite;
			bool flag2 = dialogueLine.Metadata != null && dialogueLine.Metadata.Length != 0 && dialogueLine.Metadata.Contains("system-alert");
			this.dialogBoxBackground.color = (flag2 ? Color.white : Color.black);
			this.dialogueTextField.color = (flag2 ? Color.black : Color.white);
		}
		this.localizedDialogueTextField.gameObject.SetActive(true);
		this.dialogueTextContainer.SetActive(true);
		this.container.SetActive(true);
		if (this.needsEnabling)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.continueLineAction }, ActionLayer.DIALOGUE);
			this.dialogueTextContainerEffect.effectFactor = 0f;
			this.dialogueTextContainerEffect.Show(true);
			this.needsEnabling = false;
		}
		this.lineDisplayTime = Time.time;
		if (speakerData != null)
		{
			this.lastSpeakerData = speakerData;
		}
		GameManager.Instance.DialogueRunner.DidPlayAnyLines = true;
	}

	private void PlayAnyParalinguistic(string[] metadata, SpeakerData speakerData)
	{
		if (metadata == null || metadata.Length == 0)
		{
			return;
		}
		foreach (object obj in Enum.GetValues(typeof(ParalinguisticType)))
		{
			ParalinguisticType paralinguisticType = (ParalinguisticType)obj;
			if (metadata.Contains(paralinguisticType.ToString().ToLower()))
			{
				AssetReference paralinguisticByType = speakerData.GetParalinguisticByType(paralinguisticType);
				GameManager.Instance.AudioPlayer.PlaySFX(paralinguisticByType, AudioLayer.SFX_VOCALS, 1f, 1f);
			}
		}
	}

	public void ShowPortrait(string id)
	{
		SpeakerData speakerData = null;
		if (!string.IsNullOrEmpty(id))
		{
			this.speakerDataLookup.lookupTable.TryGetValue(id.ToUpper(), out speakerData);
		}
		if (speakerData)
		{
			this.ShowPortrait(speakerData);
		}
	}

	public void ShowPortrait(SpeakerData speakerData)
	{
		GameObject gameObject = speakerData.portraitPrefab;
		if (speakerData.portraitOverrideConditions.Count > 0)
		{
			PortraitOverride portraitOverride = speakerData.portraitOverrideConditions.Find(delegate(PortraitOverride po)
			{
				if (po.useManualState)
				{
					return GameManager.Instance.SaveData.GetIntVariable(po.stateName, 0) == po.stateValue;
				}
				return po.nodesVisited.All((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n));
			});
			if (portraitOverride.portraitPrefab)
			{
				gameObject = portraitOverride.portraitPrefab;
			}
		}
		if (gameObject && gameObject != this.lastSpeakerPortrait)
		{
			this.lastSpeakerPortrait = gameObject;
			this.HidePortrait(false);
			global::UnityEngine.Object.Instantiate<GameObject>(gameObject, this.characterPortraitContainer.transform);
		}
	}

	public void HidePortrait(bool clearLastPortraitFromMemory)
	{
		foreach (object obj in this.characterPortraitContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		if (clearLastPortraitFromMemory)
		{
			this.lastSpeakerPortrait = null;
		}
	}

	public void ClearDialogue()
	{
		this.characterNameContainer.SetActive(false);
		this.dialogueTextContainer.SetActive(false);
		this.textAnimatorPlayer.textAnimator.SetText("", true);
	}

	public void ClearDialogueViewListener()
	{
		this.onDialogueLineFinished = null;
	}

	private void OnTypewriterStarted()
	{
		this.isTypewriterRunning = true;
		if (GameManager.Instance.DialogueRunner.ShouldImmediatelyResolveNextLine)
		{
			GameManager.Instance.DialogueRunner.ShouldImmediatelyResolveNextLine = false;
			this.OnTypewriterCompleted();
		}
	}

	private void OnTypewriterCompleted()
	{
		this.isTypewriterRunning = false;
		this.needsDismissing = true;
		this.lineCompleteTime = Time.time;
		if (GameManager.Instance.DialogueRunner.ShouldAutoResolveNextLine)
		{
			this.lineCompleteTime = float.NegativeInfinity;
			GameManager.Instance.DialogueRunner.ShouldAutoResolveNextLine = false;
			if (this.needsDismissing)
			{
				this.needsDismissing = false;
				Action action = this.onDialogueLineFinished;
				if (action == null)
				{
					return;
				}
				action();
			}
		}
	}

	private void OnContinueLinePressComplete()
	{
		if (this.isTypewriterRunning && Time.time > this.lineDisplayTime + this.delayBeforeCanSkipTypewriter)
		{
			this.textAnimatorPlayer.SkipTypewriter();
			GameManager.Instance.AudioPlayer.PlaySFX(this.skipSFX, AudioLayer.SFX_UI, 1f, 1f);
			return;
		}
		if (Time.time > this.lineCompleteTime + this.delayAfterLineCompleteBeforeCanContinue && this.needsDismissing)
		{
			GameManager.Instance.AudioPlayer.PlaySFX(this.continueSFX, AudioLayer.SFX_UI, 1f, 1f);
			this.needsDismissing = false;
			Action action = this.onDialogueLineFinished;
			if (action == null)
			{
				return;
			}
			action();
		}
	}

	private void OnExitLinePressComplete()
	{
		TextOptionButton textOptionButton = this.allButtons.First((TextOptionButton b) => b.HasQuickExit);
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.exitLineAction };
		input.RemoveActionListener(array, ActionLayer.DIALOGUE);
		if (textOptionButton)
		{
			textOptionButton.DoSelect();
		}
	}

	public override void DismissLine(Action onDismissalComplete)
	{
		onDismissalComplete();
	}

	private void ResetOptions()
	{
		foreach (object obj in this.optionsContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.exitLineAction };
		input.RemoveActionListener(array, ActionLayer.DIALOGUE);
		this.allButtons.Clear();
	}

	public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
	{
		this.ResetOptions();
		this.firstValidOptionButton = null;
		int num = 0;
		for (int i = 0; i < dialogueOptions.Length; i++)
		{
			DialogueOption dialogueOption = dialogueOptions[i];
			if (GameManager.Instance.DialogueRunner.ShouldShowUnavailableOptions || dialogueOption.IsAvailable)
			{
				num++;
				TextOptionButton component = global::UnityEngine.Object.Instantiate<GameObject>(this.optionButtonPrefab, this.optionsContainer.transform).GetComponent<TextOptionButton>();
				component.Init(dialogueOption, i, num, new Action<int>(this.OnOptionSelected));
				this.allButtons.Add(component);
				if (this.firstValidOptionButton == null && dialogueOption.IsAvailable)
				{
					this.firstValidOptionButton = component.BasicButtonWrapper;
				}
			}
		}
		this.onOptionSelected = onOptionSelected;
		this.optionsContainer.SetActive(true);
		base.StartCoroutine(this.SelectFirstOptionButton());
		if (this.allButtons.Any((TextOptionButton b) => b.HasQuickExit))
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.exitLineAction };
			input.AddActionListener(array, ActionLayer.DIALOGUE);
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		input2.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input2.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChangedDuringOptionSelect));
	}

	private void OnInputChangedDuringOptionSelect(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			base.StartCoroutine(this.SelectFirstOptionButton());
		}
	}

	private IEnumerator SelectFirstOptionButton()
	{
		if (this.firstValidOptionButton && GameManager.Instance.Input.IsUsingController)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(this.firstValidOptionButton.gameObject);
			this.firstValidOptionButton.ManualOnSelect();
		}
		yield return new WaitForSecondsRealtime(0.75f);
		this.allButtons.ForEach(delegate(TextOptionButton b)
		{
			b.BasicButtonWrapper.SetCanBeClicked(true);
		});
		yield break;
	}

	private void OnOptionSelected(int index)
	{
		this.optionsContainer.SetActive(false);
		this.ResetOptions();
		Action<int> action = this.onOptionSelected;
		if (action != null)
		{
			action(index);
		}
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChangedDuringOptionSelect));
	}

	[SerializeField]
	private SpeakerDataLookup speakerDataLookup;

	[SerializeField]
	private LocalizeStringEvent localizedDialogueTextField;

	[SerializeField]
	private TextMeshProUGUI dialogueTextField;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject dialogueTextContainer;

	[SerializeField]
	private UITransitionEffect dialogueTextContainerEffect;

	[SerializeField]
	private GameObject optionsContainer;

	[SerializeField]
	private GameObject optionButtonPrefab;

	[SerializeField]
	private GameObject characterPortraitContainer;

	[SerializeField]
	private GameObject characterNameContainer;

	[SerializeField]
	private LocalizeStringEvent characterNameTextField;

	[SerializeField]
	private TextAnimatorPlayer textAnimatorPlayer;

	[SerializeField]
	private float delayBeforeCanSkipTypewriter;

	[SerializeField]
	private float delayAfterLineCompleteBeforeCanContinue;

	[SerializeField]
	private AssetReference skipSFX;

	[SerializeField]
	private AssetReference continueSFX;

	[SerializeField]
	private Image dialogBoxBackground;

	[SerializeField]
	private Sprite plainBackgroundSprite;

	[SerializeField]
	private Sprite speechBackgroundSprite;

	[SerializeField]
	private List<float> typewriterSpeeds;

	private DredgePlayerActionPress continueLineAction;

	private DredgePlayerActionPress exitLineAction;

	private Action onDialogueLineFinished;

	private Action<int> onOptionSelected;

	private float lineDisplayTime;

	private float lineCompleteTime;

	private bool isTypewriterRunning;

	private bool needsEnabling = true;

	private bool needsDismissing;

	private BasicButtonWrapper firstValidOptionButton;

	private List<TextOptionButton> allButtons = new List<TextOptionButton>();

	private GameObject lastSpeakerPortrait;

	private SpeakerData lastSpeakerData;
}
