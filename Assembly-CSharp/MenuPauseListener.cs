using System;

public class MenuPauseListener : PauseListener
{
	protected new void Awake()
	{
		base.Awake();
		DredgePlayerActionPress pauseAction = this.pauseAction;
		pauseAction.OnPressComplete = (Action)Delegate.Combine(pauseAction.OnPressComplete, new Action(this.OnPausePressComplete));
		DredgePlayerActionPress unpauseAction = this.unpauseAction;
		unpauseAction.OnPressComplete = (Action)Delegate.Combine(unpauseAction.OnPressComplete, new Action(this.OnUnpausePressComplete));
		this.unpauseAction.promptString = "prompt.back";
	}

	public override void OnPausePressComplete()
	{
		if (PopupControllerDropout.Open)
		{
			return;
		}
		if (this.isLocked)
		{
			return;
		}
		ApplicationEvents.Instance.TriggerToggleSettings(true);
		base.OnPausePressComplete();
	}

	public override void OnUnpausePressComplete()
	{
		if (PopupControllerDropout.Open)
		{
			return;
		}
		if (this.isLocked)
		{
			return;
		}
		ApplicationEvents.Instance.TriggerToggleSettings(false);
		base.OnUnpausePressComplete();
	}

	public override void OnSettingsToggled(bool open)
	{
		if (open)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.unpauseAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
		}
	}
}
