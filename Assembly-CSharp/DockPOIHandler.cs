using System;
using UnityEngine;

public class DockPOIHandler : MonoBehaviour
{
	public Player PlayerRef { get; set; }

	public bool IsHandlerActive { get; set; }

	private void Awake()
	{
		this.dockAction = new DredgePlayerActionHoldDelegate("prompt.dock", GameManager.Instance.Input.Controls.Interact);
		this.dockAction.showInControlArea = true;
		this.dockAction.allowPreholding = true;
	}

	private void OnEnable()
	{
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate = this.dockAction;
		dredgePlayerActionHoldDelegate.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate.OnPressComplete, new Action(this.OnPressComplete));
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate2 = this.dockAction;
		dredgePlayerActionHoldDelegate2.OnPressBegin = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate2.OnPressBegin, new Action(this.OnPressBegin));
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate3 = this.dockAction;
		dredgePlayerActionHoldDelegate3.OnPressHold = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate3.OnPressHold, new Action(this.OnProgressChange));
		GameEvents.Instance.OnPlayerDockedToggled += this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPauseChange += this.OnPauseChange;
		GameEvents.Instance.OnPopupWindowToggled += this.OnPopupWindowToggled;
		GameEvents.Instance.OnGameWindowToggled += this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerAbilityToggled += this.OnPlayerAbilityToggled;
	}

	private void OnDisable()
	{
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate = this.dockAction;
		dredgePlayerActionHoldDelegate.OnPressComplete = (Action)Delegate.Remove(dredgePlayerActionHoldDelegate.OnPressComplete, new Action(this.OnPressComplete));
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate2 = this.dockAction;
		dredgePlayerActionHoldDelegate2.OnPressBegin = (Action)Delegate.Remove(dredgePlayerActionHoldDelegate2.OnPressBegin, new Action(this.OnPressBegin));
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate3 = this.dockAction;
		dredgePlayerActionHoldDelegate3.OnPressHold = (Action)Delegate.Remove(dredgePlayerActionHoldDelegate3.OnPressHold, new Action(this.OnProgressChange));
		GameEvents.Instance.OnPlayerDockedToggled -= this.OnPlayerDockedToggled;
		GameEvents.Instance.OnPauseChange -= this.OnPauseChange;
		GameEvents.Instance.OnPopupWindowToggled -= this.OnPopupWindowToggled;
		GameEvents.Instance.OnGameWindowToggled -= this.OnGameWindowToggled;
		GameEvents.Instance.OnPlayerAbilityToggled -= this.OnPlayerAbilityToggled;
	}

	private void OnPlayerAbilityToggled(AbilityData ability, bool isActive)
	{
		if (isActive && ability.name == this.cameraAbility.name)
		{
			this.CancelDockingAttempt();
		}
	}

	private void OnPauseChange(bool paused)
	{
		if (this.isDockingInProgress)
		{
			this.CancelDockingAttempt();
		}
	}

	private void OnPopupWindowToggled(bool showing)
	{
		if (this.isDockingInProgress && showing)
		{
			this.CancelDockingAttempt();
		}
	}

	private void OnGameWindowToggled()
	{
		if (this.isDockingInProgress)
		{
			this.CancelDockingAttempt();
		}
	}

	private void OnPlayerDockedToggled(Dock dock)
	{
		if (dock)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHoldDelegate[] { this.dockAction };
			input.RemoveActionListener(array, ActionLayer.BASE);
			GameManager.Instance.ChromaManager.StopAllAnimations();
			return;
		}
		if (this.targetDockPOI)
		{
			this.Activate(this.targetDockPOI);
		}
		GameManager.Instance.ChromaManager.PlayAnimation(ChromaManager.DredgeChromaAnimation.SAILING);
	}

	public void Activate(DockPOI dockPoi)
	{
		this.IsHandlerActive = true;
		this.targetDockPOI = dockPoi;
		if (!this.PlayerRef.IsDocked)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionHoldDelegate[] { this.dockAction };
			input.AddActionListener(array, ActionLayer.BASE);
		}
	}

	public void Deactivate()
	{
		this.StopDockingAudio();
		this.StopDockingCamera();
		this.StopDockingMovement();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHoldDelegate[] { this.dockAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		this.targetDockPOI = null;
		this.IsHandlerActive = false;
	}

	private void OnPressBegin()
	{
		this.targetDockSlotIndex = VectorUtils.GetClosestIndex(GameManager.Instance.Player.transform, this.targetDockPOI.dockSlots);
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate = this.dockAction;
		dredgePlayerActionHoldDelegate.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionHoldDelegate.OnPressEnd, new Action(this.OnPressEnd));
		this.isDockingInProgress = true;
		if (this.targetDockSlotIndex == -1)
		{
			CustomDebug.EditorLogError("Couldn't find closest dock slot");
			return;
		}
		GameManager.Instance.PlayerCamera.ToggleDockingCameraMode(true);
		Transform transform = this.targetDockPOI.dockSlots[this.targetDockSlotIndex];
		this.targetDockPos = new Vector2(transform.position.x, transform.position.z);
		float num = Quaternion.Angle(base.transform.rotation, Quaternion.LookRotation(transform.forward));
		float num2 = Quaternion.Angle(base.transform.rotation, Quaternion.LookRotation(transform.forward * -1f));
		if (num < num2)
		{
			this.targetDockRotation = transform.forward;
		}
		else
		{
			this.targetDockRotation = transform.forward * -1f;
		}
		GameManager.Instance.Player.Controller.SetAutoMoveTarget(transform);
		GameManager.Instance.Player.Controller.SetAutoRotateTarget(this.targetDockRotation);
		this.dockingAudio.ToggleDockingLoop(true);
	}

	private void OnProgressChange()
	{
		if (this.targetDockSlotIndex != -1 && this.targetDockSlotIndex < this.targetDockPOI.dockSlots.Length)
		{
			float num = Mathf.Abs(Vector3.Angle(base.transform.forward, this.targetDockRotation));
			if (Vector2.Distance(new Vector2(GameManager.Instance.Player.Controller.transform.position.x, GameManager.Instance.Player.Controller.transform.position.z), this.targetDockPos) < this.dockProximityThreshold && num < this.dockRotationThreshold && this.dockAction.currentHoldTime >= this.minHoldTime)
			{
				this.dockAction.Resolve();
			}
		}
	}

	private void CancelDockingAttempt()
	{
		this.isDockingInProgress = false;
		this.StopDockingAudio();
		this.StopDockingCamera();
		this.StopDockingMovement();
	}

	private void OnPressEnd()
	{
		DredgePlayerActionHoldDelegate dredgePlayerActionHoldDelegate = this.dockAction;
		dredgePlayerActionHoldDelegate.OnPressEnd = (Action)Delegate.Remove(dredgePlayerActionHoldDelegate.OnPressEnd, new Action(this.OnPressEnd));
		this.targetDockSlotIndex = -1;
		this.CancelDockingAttempt();
	}

	private void OnPressComplete()
	{
		this.isDockingInProgress = false;
		this.StopDockingAudio();
		this.StopDockingMovement();
		GameEvents.Instance.TriggerPlayerInteractedWithPOI();
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionHoldDelegate[] { this.dockAction };
		input.RemoveActionListener(array, ActionLayer.BASE);
		GameManager.Instance.Player.Dock(this.targetDockPOI.dock, this.targetDockSlotIndex, true);
	}

	private void StopDockingAudio()
	{
		this.dockingAudio.ToggleDockingLoop(false);
	}

	private void StopDockingCamera()
	{
		GameManager.Instance.PlayerCamera.ToggleDockingCameraMode(false);
	}

	private void StopDockingMovement()
	{
		GameManager.Instance.Player.Controller.ClearAutoMoveTarget();
		GameManager.Instance.Player.Controller.ClearAutoRotateTarget();
	}

	[Tooltip("How close does the player have to be do the dock slot transform for it to count as being docked?")]
	[SerializeField]
	private float dockProximityThreshold;

	[Tooltip("How close does the player have to be do the dock's rotation for it to count as being docked?")]
	[SerializeField]
	private float dockRotationThreshold;

	[SerializeField]
	private DockingAudio dockingAudio;

	[SerializeField]
	private AbilityData manifestAbility;

	[SerializeField]
	private AbilityData cameraAbility;

	private DredgePlayerActionHoldDelegate dockAction;

	private int targetDockSlotIndex;

	private DockPOI targetDockPOI;

	private Vector3 targetDockRotation;

	private Vector2 targetDockPos;

	private bool isDockingInProgress;

	private float minHoldTime = 0.5f;
}
