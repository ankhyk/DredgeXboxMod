using System;
using UnityEngine;

public abstract class PauseListener : MonoBehaviour
{
	protected void Awake()
	{
		GameManager.Instance.PauseListener = this;
		this.pauseAction = new DredgePlayerActionPress("prompt.pause", GameManager.Instance.Input.Controls.Pause);
		this.unpauseAction = new DredgePlayerActionPress("prompt.resume", GameManager.Instance.Input.Controls.Unpause);
		this.unpauseAction.showInControlArea = true;
		this.unpauseAction.evaluateWhenPaused = true;
		ApplicationEvents.Instance.OnCreditsToggled += this.OnCreditsToggled;
	}

	protected virtual void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingsConfirmationWindowToggled += this.OnSettingsConfirmationWindowToggled;
	}

	protected virtual void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingsConfirmationWindowToggled -= this.OnSettingsConfirmationWindowToggled;
	}

	public void OnSettingsConfirmationWindowToggled(bool open)
	{
		DredgePlayerActionBase[] array;
		if (open)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.unpauseAction };
			input.RemoveActionListener(array, ActionLayer.SYSTEM);
			return;
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.unpauseAction };
		input2.AddActionListener(array, ActionLayer.SYSTEM);
	}

	private void OnDestroy()
	{
		ApplicationEvents.Instance.OnCreditsToggled -= this.OnCreditsToggled;
	}

	private void OnCreditsToggled(bool showing)
	{
		this.isLocked = showing;
	}

	public virtual void OnPausePressComplete()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.pauseAction };
		input.RemoveActionListener(array, ActionLayer.SYSTEM);
	}

	public virtual void OnUnpausePressComplete()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.unpauseAction };
		input.RemoveActionListener(array, ActionLayer.SYSTEM);
	}

	public virtual void OnSettingsToggled(bool open)
	{
	}

	public virtual void CanShowUnpauseAction(bool canShow)
	{
		DredgePlayerActionBase[] array;
		if (canShow)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.unpauseAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
			return;
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.unpauseAction };
		input2.RemoveActionListener(array, ActionLayer.SYSTEM);
	}

	protected DredgePlayerActionPress pauseAction;

	protected DredgePlayerActionPress unpauseAction;

	protected bool isLocked;
}
