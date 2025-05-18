using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

public class DockUI : MonoBehaviour
{
	private void Awake()
	{
		this.leaveAction = new DredgePlayerActionHold("destination.undock", GameManager.Instance.Input.Controls.Undock, 0.25f);
		DredgePlayerActionHold dredgePlayerActionHold = this.leaveAction;
		dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.Leave));
		this.leaveAction.showInControlArea = true;
		this.destinationNameContainer.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Combine(instance.OnGameStarted, new Action(this.OnGameStarted));
	}

	private void OnDisable()
	{
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Remove(instance.OnGameStarted, new Action(this.OnGameStarted));
	}

	private void OnGameStarted()
	{
		this.OnPlayerDockedToggled(GameManager.Instance.Player.CurrentDock);
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock)
		{
			dock.EnableVCam();
			if (GameManager.Instance.IsPlaying)
			{
				this.Show(dock, 0.2f);
				return;
			}
		}
		else
		{
			this.HideUI();
		}
	}

	public void HideUI()
	{
		this.ClearUI();
		this.dockProgressUI.Hide();
		this.destinationNameContainer.AnimateOut();
		ApplicationEvents.Instance.TriggerUIWindowToggled(UIWindowType.DOCK, false);
	}

	public void Show(Dock dock, float delaySec = 0.2f)
	{
		this.dock = dock;
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.NONE);
		this.destinationNameLocalizedString.StringReference = dock.Data.DockNameKey;
		string text = dock.Data.YarnRootNode;
		if (GameManager.Instance.SaveData.CanShowFinaleHijack)
		{
			DialogueHijack dialogueHijack = this.dialogueHijacks.Find((DialogueHijack h) => h.visitedNodes.All((string n) => GameManager.Instance.DialogueRunner.GetHasVisitedNode(n)));
			if (dialogueHijack != null)
			{
				text = dialogueHijack.hijackNode;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			base.StartCoroutine(this.ShowUIWithDelay(dock, delaySec));
			return;
		}
		base.StartCoroutine(this.StartDialogueWithDelay(text, delaySec));
	}

	private void OnDockDialogueComplete()
	{
		if (this.speakerVCam)
		{
			this.speakerVCam.enabled = false;
		}
		GameEvents.Instance.TriggerSpeakerVisited(null, false);
		GameManager.Instance.UI.HideDialogueView();
		GameManager.Instance.DialogueRunner.onDialogueComplete.RemoveListener(new UnityAction(this.OnDockDialogueComplete));
		base.StartCoroutine(this.ShowUIWithDelay(this.dock, 0f));
	}

	private IEnumerator StartDialogueWithDelay(string nodeName, float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnDockDialogueComplete));
		GameManager.Instance.DialogueRunner.StartDialogue(nodeName);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
		yield break;
	}

	public void RefreshVCams()
	{
		this.dock.RefreshVCams();
	}

	private IEnumerator ShowUIWithDelay(Dock dock, float delaySec)
	{
		if (this.isShowPending)
		{
			yield return true;
		}
		this.isShowPending = true;
		this.RefreshVCams();
		this.ClearUI();
		yield return new WaitForSeconds(delaySec);
		ApplicationEvents.Instance.TriggerUIWindowToggled(UIWindowType.DOCK, true);
		List<BaseDestination> destinations = dock.GetDestinations();
		for (int i = 0; i < destinations.Count; i++)
		{
			BaseDestination baseDestination = destinations[i];
			if (baseDestination.AlwaysShow || GameManager.Instance.SaveData.availableDestinations.Contains(baseDestination.Id))
			{
				if (baseDestination is ConstructableDestination)
				{
					BuildingTierId tierId = (baseDestination as ConstructableDestination).constructableDestinationData.tiers[0].tierId;
					if (GameManager.Instance.ConstructableBuildingManager && !GameManager.Instance.ConstructableBuildingManager.GetCanBuildingBeConstructed(tierId) && !GameManager.Instance.ConstructableBuildingManager.GetIsBuildingConstructed(tierId))
					{
						goto IL_01DF;
					}
				}
				GameObject gameObject;
				if (baseDestination.useFixedScreenPosition)
				{
					gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.tirDestinationButtonPrefab, this.destinationButtonContainer);
				}
				else
				{
					gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.destinationButtonPrefab, this.destinationButtonContainer);
				}
				gameObject.name = "DestinationButton: " + baseDestination.name;
				DestinationButton component = gameObject.GetComponent<DestinationButton>();
				component.Init(baseDestination);
				this.destinationButtonObjects.Add(gameObject);
				this.destinationButtons.Add(component);
				if (i == 0)
				{
					this.primaryDestinationButton = component.BasicButtonWrapper;
				}
			}
			IL_01DF:;
		}
		this.ConnectTIRDestinationButtons(destinations, this.destinationButtons);
		GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.boatActionsDestinationPrefab, this.destinationButtonContainer);
		BoatActionsDestinationUI component2 = gameObject2.GetComponent<BoatActionsDestinationUI>();
		component2.Init(dock.boatActionsDestination);
		this.destinationButtonObjects.Add(gameObject2);
		if (this.primaryDestinationButton == null)
		{
			this.primaryDestinationButton = component2.UndockDestinationButton.BasicButtonWrapper;
		}
		List<QuestStepData> list = GameManager.Instance.QuestManager.GetStepDataForAllActiveQuests().Where(delegate(QuestStepData qs)
		{
			if (qs.showAtDock)
			{
				DockData stepDock = qs.stepDock;
				if (((stepDock != null) ? stepDock.Id : null) == dock.Data.Id)
				{
					return qs.CanBeShown();
				}
			}
			return false;
		}).ToList<QuestStepData>();
		List<string> list2 = new List<string>();
		int j;
		for (j = 0; j < list.Count; j++)
		{
			this.CreateSpeakerButton(list[j].stepSpeaker, list[j].yarnRootNode, true, j);
			list2.Add(list[j].stepSpeaker.name);
		}
		for (int k = 0; k < dock.Data.Speakers.Count; k++)
		{
			SpeakerData speakerData = dock.Data.Speakers[k];
			if ((speakerData.alwaysAvailable || GameManager.Instance.SaveData.availableSpeakers.Contains(speakerData.name)) && !list2.Contains(speakerData.name))
			{
				bool flag = speakerData.highlightConditions.Any((HighlightCondition h) => h.ShouldHighlight());
				this.CreateSpeakerButton(speakerData, speakerData.yarnRootNode, flag, j);
				list2.Add(speakerData.name);
				j++;
			}
		}
		this.dockProgressUI.Init(dock.Data);
		this.destinationNameContainer.AnimateIn();
		ApplicationEvents.Instance.OnToggleSettings += this.OnToggleSettings;
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.leaveAction };
		input.AddActionListener(array, ActionLayer.DOCKED);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DOCKED);
		this.SelectPrimaryButton();
		this.isShowPending = false;
		yield break;
	}

	private void ConnectTIRDestinationButtons(List<BaseDestination> destinations, List<DestinationButton> destinationButtons)
	{
		destinationButtons.ForEach(delegate(DestinationButton thisDestinationButton)
		{
			if (thisDestinationButton.destination.selectOnLeft.Count > 0)
			{
				for (int i = 0; i < thisDestinationButton.destination.selectOnLeft.Count; i++)
				{
					BaseDestination destinationToMatch2 = thisDestinationButton.destination.selectOnLeft[i];
					DestinationButton destinationButton5 = destinationButtons.Find((DestinationButton destinationButton) => destinationButton.destination == destinationToMatch2);
					if (destinationButton5 != null)
					{
						(thisDestinationButton.BasicButtonWrapper.Button as SelectableWithNavigationPriority).leftSelectable = destinationButton5.BasicButtonWrapper.Button;
						break;
					}
				}
			}
			if (thisDestinationButton.destination.selectOnRight.Count > 0)
			{
				for (int j = 0; j < thisDestinationButton.destination.selectOnRight.Count; j++)
				{
					BaseDestination destinationToMatch3 = thisDestinationButton.destination.selectOnRight[j];
					DestinationButton destinationButton2 = destinationButtons.Find((DestinationButton destinationButton) => destinationButton.destination == destinationToMatch3);
					if (destinationButton2 != null)
					{
						(thisDestinationButton.BasicButtonWrapper.Button as SelectableWithNavigationPriority).rightSelectable = destinationButton2.BasicButtonWrapper.Button;
						break;
					}
				}
			}
			if (thisDestinationButton.destination.selectOnDown.Count > 0)
			{
				for (int k = 0; k < thisDestinationButton.destination.selectOnDown.Count; k++)
				{
					BaseDestination destinationToMatch4 = thisDestinationButton.destination.selectOnDown[k];
					DestinationButton destinationButton3 = destinationButtons.Find((DestinationButton destinationButton) => destinationButton.destination == destinationToMatch4);
					if (destinationButton3 != null)
					{
						(thisDestinationButton.BasicButtonWrapper.Button as SelectableWithNavigationPriority).downSelectable = destinationButton3.BasicButtonWrapper.Button;
						break;
					}
				}
			}
			if (thisDestinationButton.destination.selectOnUp.Count > 0)
			{
				for (int l = 0; l < thisDestinationButton.destination.selectOnUp.Count; l++)
				{
					BaseDestination destinationToMatch = thisDestinationButton.destination.selectOnUp[l];
					DestinationButton destinationButton4 = destinationButtons.Find((DestinationButton destinationButton) => destinationButton.destination == destinationToMatch);
					if (destinationButton4 != null)
					{
						(thisDestinationButton.BasicButtonWrapper.Button as SelectableWithNavigationPriority).upSelectable = destinationButton4.BasicButtonWrapper.Button;
						return;
					}
				}
			}
		});
	}

	private void CreateSpeakerButton(SpeakerData speaker, string yarnRootNode, bool highlightThis, int i)
	{
		float num = 10f;
		float num2 = 50f;
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.speakerButtonPrefab, this.speakerButtonContainer);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		float width = component.rect.width;
		float height = component.rect.height;
		component.anchoredPosition = new Vector2(-(width + num2), height * (float)i + num * (float)(i + 1));
		SpeakerButton component2 = gameObject.GetComponent<SpeakerButton>();
		component2.Init(speaker, yarnRootNode, highlightThis);
		component2.OnSubmitAction = (Action<string, SpeakerData>)Delegate.Combine(component2.OnSubmitAction, new Action<string, SpeakerData>(this.OnSpeakerButtonSubmitted));
		float num3 = this.baseSpeakerButtonAppearDelaySec + (float)i * this.perSpeakerButtonAppearDelaySec;
		component.DOAnchorPosX(0f, 0.35f, false).SetDelay(num3).SetEase(Ease.OutExpo);
	}

	private void OnSpeakerButtonSubmitted(string yarnRootNode, SpeakerData speaker)
	{
		GameManager.Instance.DialogueRunner.onDialogueComplete.AddListener(new UnityAction(this.OnDockDialogueComplete));
		GameManager.Instance.DialogueRunner.StartDialogue(yarnRootNode);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.DIALOGUE);
		GameEvents.Instance.TriggerSpeakerVisited(speaker, true);
		if (this.dock.SpeakerVCams.TryGetValue(speaker.name, out this.speakerVCam))
		{
			this.speakerVCam.enabled = true;
		}
		this.HideUI();
	}

	private void ClearUI()
	{
		for (int i = this.destinationButtonObjects.Count - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.destinationButtonObjects[i]);
		}
		this.destinationButtons.Clear();
		this.primaryDestinationButton = null;
		foreach (object obj in this.speakerButtonContainer.transform)
		{
			global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		ApplicationEvents.Instance.OnToggleSettings -= this.OnToggleSettings;
	}

	private void OnToggleSettings(bool showing)
	{
		if (this.controllerFocusGrabber)
		{
			this.controllerFocusGrabber.enabled = !showing;
		}
	}

	private void SelectPrimaryButton()
	{
		this.primaryDestinationButton.TimeUntilTransition = 0f;
		this.primaryDestinationButton.SetSelectable(this.controllerFocusGrabber);
		this.controllerFocusGrabber.SelectSelectable();
	}

	public void Leave()
	{
		this.dock.DisableVCam();
		GameManager.Instance.Player.Undock();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHold[] { this.leaveAction };
		input.RemoveActionListener(array, ActionLayer.DOCKED);
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
		GameManager.Instance.SaveData.CanShowFinaleHijack = true;
	}

	[SerializeField]
	private GameObject boatActionsDestinationPrefab;

	[SerializeField]
	private GameObject destinationButtonPrefab;

	[SerializeField]
	private GameObject tirDestinationButtonPrefab;

	[SerializeField]
	private GameObject speakerButtonPrefab;

	[SerializeField]
	private TextBannerUI destinationNameContainer;

	[SerializeField]
	private Transform speakerButtonContainer;

	[SerializeField]
	private LocalizeStringEvent destinationNameLocalizedString;

	[SerializeField]
	private Transform destinationButtonContainer;

	[SerializeField]
	private DockProgressUI dockProgressUI;

	[SerializeField]
	private ControllerFocusGrabber controllerFocusGrabber;

	[SerializeField]
	private float baseSpeakerButtonAppearDelaySec;

	[SerializeField]
	private float perSpeakerButtonAppearDelaySec;

	[SerializeField]
	private List<DialogueHijack> dialogueHijacks;

	private List<GameObject> destinationButtonObjects = new List<GameObject>();

	private List<DestinationButton> destinationButtons = new List<DestinationButton>();

	protected DredgePlayerActionHold leaveAction;

	private Dock dock;

	private bool isShowPending;

	private BasicButtonWrapper primaryDestinationButton;

	private CinemachineVirtualCamera speakerVCam;
}
