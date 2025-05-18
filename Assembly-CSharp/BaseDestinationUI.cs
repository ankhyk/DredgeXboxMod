using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BaseDestinationUI : MonoBehaviour
{
	public UIWindowType WindowType
	{
		get
		{
			return this.windowType;
		}
	}

	public bool HasFinishedLoading
	{
		get
		{
			return this.hasFinishedLoading;
		}
	}

	private void Awake()
	{
		this.leaveAction = new DredgePlayerActionPress("prompt.return-to-town", GameManager.Instance.Input.Controls.Back);
		this.leaveAction.showInControlArea = true;
		this.leaveAction.allowPreholding = true;
		this.leaveAction.priority = 0;
	}

	protected virtual void OnEnable()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.leaveAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnLeavePressComplete));
		GameEvents.Instance.OnItemPickedUp += this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete += this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor += this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnMaintenanceModeToggled += this.OnMaintenanceModeToggled;
		GameEvents.Instance.OnQuestGridViewChange += this.OnQuestGridViewChange;
	}

	protected virtual void OnDisable()
	{
		DredgePlayerActionPress dredgePlayerActionPress = this.leaveAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionPress.OnPressComplete, new Action(this.OnLeavePressComplete));
		GameEvents.Instance.OnItemPickedUp -= this.OnItemPickedUp;
		GameEvents.Instance.OnItemPlaceComplete -= this.OnItemPlaceComplete;
		GameEvents.Instance.OnItemRemovedFromCursor -= this.OnItemRemovedFromCursor;
		GameEvents.Instance.OnMaintenanceModeToggled -= this.OnMaintenanceModeToggled;
		GameEvents.Instance.OnQuestGridViewChange -= this.OnQuestGridViewChange;
	}

	protected virtual void OnMaintenanceModeToggled(bool isActive)
	{
		if (isActive)
		{
			GameManager.Instance.GridManager.ClearActionHandlers();
			return;
		}
		this.ConfigureActionHandlers();
	}

	protected virtual void ConfigureActionHandlers()
	{
	}

	private void OnQuestGridViewChange()
	{
		this.RefreshShowLeaveAction();
	}

	protected virtual void OnLeavePressComplete()
	{
		if (this.destination.VCam && this.destination.VCam != GameManager.Instance.Player.CurrentDock.GetVCam())
		{
			this.destination.VCam.enabled = false;
		}
		GameManager.Instance.GridManager.SetRepairMode(false);
		GameManager.Instance.Player.CanMoveInstalledItems = false;
		GameManager.Instance.GridManager.ClearActionHandlers();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.leaveAction };
		input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
		GameManager.Instance.UI.HideCurrentDestination();
	}

	protected virtual void OnItemPickedUp(GridObject gridObject)
	{
		this.RefreshShowLeaveAction();
	}

	protected virtual void OnItemPlaceComplete(GridObject gridObject, bool result)
	{
		if (result)
		{
			this.RefreshShowLeaveAction();
		}
	}

	protected virtual void OnItemRemovedFromCursor(GridObject gridObject)
	{
		this.RefreshShowLeaveAction();
	}

	protected virtual void RefreshShowLeaveAction()
	{
		DredgePlayerActionBase[] array;
		if (!this.hasInput)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.leaveAction };
			input.RemoveActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		if (!GameManager.Instance.GridManager.IsCurrentlyHoldingObject() && !GameManager.Instance.UI.IsShowingQuestGrid)
		{
			DredgeInputManager input2 = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.leaveAction };
			input2.AddActionListener(array, ActionLayer.UI_WINDOW);
			return;
		}
		DredgeInputManager input3 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.leaveAction };
		input3.RemoveActionListener(array, ActionLayer.UI_WINDOW);
	}

	public void Show(BaseDestination destination)
	{
		this.destination = destination;
		this.hasFinishedLoading = false;
		ApplicationEvents.Instance.TriggerUIWindowToggled(this.windowType, true);
		GameManager.Instance.SaveData.RecordShopVisit(destination.Id, GameManager.Instance.Time.Day);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.NONE);
		if (destination.VCam)
		{
			destination.VCam.enabled = true;
		}
		string text = "";
		List<QuestStepData> list = (from qs in GameManager.Instance.QuestManager.GetStepDataForAllActiveQuests()
			where qs && qs.showAtSpeaker && qs.stepSpeaker && destination.SpeakerData && qs.stepSpeaker.name == destination.SpeakerData.name && qs.CanBeShown()
			select qs).ToList<QuestStepData>();
		if (list != null && list.Count > 0)
		{
			text = list[0].yarnRootNode;
		}
		if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(destination.SpeakerRootNodeOverride))
		{
			text = destination.SpeakerRootNodeOverride;
		}
		if (string.IsNullOrEmpty(text) && destination.SpeakerData)
		{
			text = destination.SpeakerData.yarnRootNode;
		}
		if (!string.IsNullOrEmpty(text))
		{
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
			GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnDestinationDialogueComplete));
			GameManager.Instance.DialogueRunner.StartDialogue(text);
		}
		else
		{
			this.ShowMainUI();
			if (this.hasInput)
			{
				base.StartCoroutine(this.ActivateInputWithDelay(destination));
			}
			else
			{
				this.hasFinishedLoading = true;
			}
		}
		GameEvents.Instance.TriggerDestinationVisited(destination, true);
	}

	private void OnDestinationDialogueComplete()
	{
		GameManager.Instance.DialogueRunner.onDialogueComplete.RemoveListener(new UnityAction(this.OnDestinationDialogueComplete));
		if (GameManager.Instance.UI.ExitDestinationRequested)
		{
			GameManager.Instance.UI.ExitDestinationRequested = false;
			this.OnLeavePressComplete();
			return;
		}
		this.ShowMainUI();
		if (this.hasInput)
		{
			base.StartCoroutine(this.ActivateInputWithDelay(this.destination));
			return;
		}
		this.hasFinishedLoading = true;
	}

	private IEnumerator ActivateInputWithDelay(BaseDestination destination)
	{
		yield return new WaitForSeconds(this.inputDelaySec);
		this.RefreshShowLeaveAction();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.UI_WINDOW);
		this.hasFinishedLoading = true;
		yield break;
	}

	protected virtual void ShowMainUI()
	{
		this.ConfigureActionHandlers();
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		GameManager.Instance.GridManager.ClearActionHandlers();
		this.hasFinishedLoading = false;
		base.gameObject.SetActive(false);
		GameEvents.Instance.TriggerDestinationVisited(null, false);
	}

	[SerializeField]
	private UIWindowType windowType;

	[SerializeField]
	protected bool hasInput = true;

	protected DredgePlayerActionPress leaveAction;

	protected BaseDestination destination;

	protected float inputDelaySec = 0.5f;

	private bool hasFinishedLoading;
}
