using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private void OnEnable()
	{
		ApplicationEvents.Instance.OnGameStartable += this.OnGameStartable;
		GameEvents.Instance.OnFinaleVoyageStarted += this.OnFinaleVoyageStarted;
	}

	private void OnFinaleVoyageStarted()
	{
		this.isInFinale = true;
	}

	private void OnGameStartable()
	{
		ApplicationEvents.Instance.OnGameStartable -= this.OnGameStartable;
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPopupWindowToggled += this.OnPopupWindowToggled;
		GameEvents.Instance.OnDialogueCompleted += this.OnDialogueCompleted;
		GameEvents.Instance.OnRadialMenuShowingToggled += this.OnRadialMenuShowingToggled;
		ApplicationEvents.Instance.OnUIWindowToggled += this.OnUIWindowToggled;
	}

	private void OnDisable()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		GameEvents.Instance.OnPopupWindowToggled -= this.OnPopupWindowToggled;
		GameEvents.Instance.OnDialogueCompleted -= this.OnDialogueCompleted;
		GameEvents.Instance.OnRadialMenuShowingToggled -= this.OnRadialMenuShowingToggled;
		GameEvents.Instance.OnFinaleVoyageStarted -= this.OnFinaleVoyageStarted;
		ApplicationEvents.Instance.OnUIWindowToggled -= this.OnUIWindowToggled;
	}

	private void OnRadialMenuShowingToggled(bool showing)
	{
		this.EvaluateViewChange();
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock == null && !this.hasUndockedSinceLoaded)
		{
			this.hasUndockedSinceLoaded = true;
			this.QueueSteps();
		}
		this.EvaluateViewChange();
	}

	private void OnDialogueCompleted()
	{
		this.EvaluateViewChange();
	}

	private void OnPopupWindowToggled(bool showing)
	{
		this.isPopupWindowShowing = showing;
		this.EvaluateViewChange();
	}

	private void OnUIWindowToggled(UIWindowType windowType, bool showing)
	{
		this.EvaluateViewChange();
	}

	private void EvaluateViewChange()
	{
		if (this.currentlyShowingStepData)
		{
			bool showResultFromUI = this.GetShowResultFromUI(this.currentlyShowingStepData);
			if (this.currentState == TutorialManager.TutorialManagerState.SHOWING && !showResultFromUI)
			{
				this.OnHideConditionsMet(this.currentlyShowingStepData, false);
				return;
			}
			if (this.currentState == TutorialManager.TutorialManagerState.WAITING_TO_SHOW && showResultFromUI)
			{
				this.EvaluateShowResultFromConditions(this.currentlyShowingStepData);
				return;
			}
		}
		else
		{
			this.QueueSteps();
		}
	}

	private bool GetShowResultFromUI(TutorialStepData stepData)
	{
		bool flag = true;
		if (stepData != null)
		{
			if (flag && GameManager.Instance.SettingsSaveData.tutorials == 0)
			{
				flag = false;
			}
			if (flag && this.isInFinale)
			{
				flag = false;
			}
			if (flag && !stepData.viewModes.HasFlag(TutorialStepViewEnum.POPUP) && this.isPopupWindowShowing)
			{
				flag = false;
			}
			if (flag && (stepData.viewModes == TutorialStepViewEnum.NONE || !stepData.viewModes.HasFlag(TutorialStepViewEnum.INVENTORY)) && GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.PLAYER_COMBINED_PANEL))
			{
				flag = false;
			}
			if (flag && (stepData.viewModes == TutorialStepViewEnum.NONE || !stepData.viewModes.HasFlag(TutorialStepViewEnum.DOCKED)) && GameManager.Instance.Player && GameManager.Instance.Player.IsDocked)
			{
				flag = false;
			}
			if (flag && (stepData.viewModes == TutorialStepViewEnum.NONE || !stepData.viewModes.HasFlag(TutorialStepViewEnum.DIALOGUE)) && GameManager.Instance.DialogueRunner && GameManager.Instance.DialogueRunner.IsDialogueRunning)
			{
				flag = false;
			}
			if (flag && (stepData.viewModes == TutorialStepViewEnum.NONE || !stepData.viewModes.HasFlag(TutorialStepViewEnum.RADIAL)) && GameManager.Instance.UI.IsShowingRadialMenu)
			{
				flag = false;
			}
		}
		return flag;
	}

	private void QueueSteps()
	{
		this.currentState = TutorialManager.TutorialManagerState.NO_STEP;
		this.currentlySubscribedSteps = this.tutorialData.tutorialSteps.Where((TutorialStepData stepData) => !this.GetTutorialStepComplete(stepData.stepId) && stepData.prerequisiteSteps.All((TutorialStepEnum p) => this.GetTutorialStepComplete(p))).ToList<TutorialStepData>();
		this.currentState = TutorialManager.TutorialManagerState.WAITING_TO_SHOW;
		this.currentlySubscribedSteps.ForEach(delegate(TutorialStepData candidateStepData)
		{
			candidateStepData.showConditions.ForEach(delegate(TutorialStepCondition s)
			{
				s.Subscribe(new Action<TutorialStepData>(this.EvaluateShowResultFromConditions), candidateStepData);
			});
		});
	}

	private void EvaluateShowResultFromConditions(TutorialStepData stepData)
	{
		if (stepData.showConditions.All((TutorialStepCondition s) => s.IsConditionMet))
		{
			this.OnShowConditionsMet(stepData);
		}
	}

	private void OnShowConditionsMet(TutorialStepData stepToShow)
	{
		if (this.GetShowResultFromUI(stepToShow))
		{
			this.currentlySubscribedSteps.ForEach(delegate(TutorialStepData subscribedStep)
			{
				subscribedStep.showConditions.ForEach(delegate(TutorialStepCondition s)
				{
					s.Unsubscribe();
				});
			});
			this.ShowTutorialStep(stepToShow);
		}
	}

	private void EvaluateHideConditions(TutorialStepData stepData)
	{
		if (stepData.hideConditions.All((TutorialStepCondition s) => s.IsConditionMet))
		{
			this.OnHideConditionsMet(stepData, true);
		}
	}

	private void OnHideConditionsMet(TutorialStepData stepData, bool completed)
	{
		stepData.hideConditions.ForEach(delegate(TutorialStepCondition s)
		{
			s.Unsubscribe();
		});
		this.HideTutorialStep(stepData, completed);
	}

	private void Update()
	{
		this.currentlySubscribedSteps.ForEach(delegate(TutorialStepData stepData)
		{
			if (this.currentState == TutorialManager.TutorialManagerState.WAITING_TO_SHOW)
			{
				stepData.showConditions.ForEach(delegate(TutorialStepCondition s)
				{
					s.Update(Time.deltaTime);
				});
			}
		});
		if (this.currentState == TutorialManager.TutorialManagerState.SHOWING && this.currentlyShowingStepData != null)
		{
			this.currentlyShowingStepData.hideConditions.ForEach(delegate(TutorialStepCondition s)
			{
				s.Update(Time.deltaTime);
			});
			if (this.currentlyShowingStepData != null)
			{
				this.tutorialPopup.SetProgress(this.currentlyShowingStepData.GetProgress());
			}
		}
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		if (this.currentState == TutorialManager.TutorialManagerState.SHOWING)
		{
			this.FormatString(this.currentlyShowingStepData);
		}
	}

	private void ShowTutorialStep(TutorialStepData stepData)
	{
		this.currentState = TutorialManager.TutorialManagerState.ANIMATING_SHOW;
		this.currentlyShowingStepData = stepData;
		this.FormatString(this.currentlyShowingStepData);
		this.tutorialPopup.Show(this.currentlyShowingStepData);
		this.currentlyShowingStepData.hideConditions.ForEach(delegate(TutorialStepCondition s)
		{
			s.Subscribe(new Action<TutorialStepData>(this.EvaluateHideConditions), this.currentlyShowingStepData);
		});
		this.currentState = TutorialManager.TutorialManagerState.SHOWING;
	}

	private void FormatString(TutorialStepData stepData)
	{
		if (this.currentlyShowingStepData == null)
		{
			return;
		}
		this.tutorialPopup.SetData(this.currentlyShowingStepData);
	}

	private void HideTutorialStep(TutorialStepData stepData, bool completed)
	{
		this.currentState = TutorialManager.TutorialManagerState.ANIMATING_HIDE;
		this.tutorialPopup.Hide();
		this.currentState = TutorialManager.TutorialManagerState.NO_STEP;
		if (completed)
		{
			this.SetTutorialStepComplete(stepData.stepId, true);
			this.currentlyShowingStepData = null;
		}
		this.QueueSteps();
	}

	private void SetTutorialStepComplete(TutorialStepEnum stepId, bool complete)
	{
		GameManager.Instance.SaveData.SetBoolVariable(string.Format("tutorial-step-complete-{0}", stepId), complete);
	}

	private bool GetTutorialStepComplete(TutorialStepEnum stepId)
	{
		return GameManager.Instance.SaveData.GetBoolVariable(string.Format("tutorial-step-complete-{0}", stepId), false);
	}

	[SerializeField]
	private TutorialPopup tutorialPopup;

	[SerializeField]
	private TutorialData tutorialData;

	private TutorialStepData currentlyShowingStepData;

	private List<TutorialStepData> currentlySubscribedSteps = new List<TutorialStepData>();

	private TutorialManager.TutorialManagerState currentState;

	private bool hasUndockedSinceLoaded;

	private bool isInFinale;

	private bool isPopupWindowShowing;

	private enum TutorialManagerState
	{
		NO_STEP,
		WAITING_TO_SHOW,
		ANIMATING_SHOW,
		SHOWING,
		ANIMATING_HIDE
	}
}
