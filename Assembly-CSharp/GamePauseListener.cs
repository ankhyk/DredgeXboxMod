using System;

public class GamePauseListener : PauseListener
{
	protected override void OnEnable()
	{
		base.OnEnable();
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Combine(instance.OnGameEnded, new Action(this.OnGameEnded));
		ApplicationEvents.Instance.OnGameStartable += this.OnGameStartable;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		GameManager instance = GameManager.Instance;
		instance.OnGameEnded = (Action)Delegate.Remove(instance.OnGameEnded, new Action(this.OnGameEnded));
		ApplicationEvents.Instance.OnGameStartable -= this.OnGameStartable;
	}

	private void OnGameStartable()
	{
		DredgePlayerActionPress pauseAction = this.pauseAction;
		pauseAction.OnPressComplete = (Action)Delegate.Combine(pauseAction.OnPressComplete, new Action(this.OnPausePressComplete));
		DredgePlayerActionPress unpauseAction = this.unpauseAction;
		unpauseAction.OnPressComplete = (Action)Delegate.Combine(unpauseAction.OnPressComplete, new Action(this.OnUnpausePressComplete));
	}

	private void Start()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.pauseAction };
		input.AddActionListener(array, ActionLayer.SYSTEM);
	}

	private void OnGameEnded()
	{
		DredgeInputManager input = GameManager.Instance.Input;
		DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.pauseAction, this.unpauseAction };
		input.RemoveActionListener(array, ActionLayer.SYSTEM);
	}

	public override void OnSettingsToggled(bool open)
	{
		DredgePlayerActionBase[] array;
		if (open)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			array = new DredgePlayerActionPress[] { this.unpauseAction };
			input.AddActionListener(array, ActionLayer.SYSTEM);
			return;
		}
		DredgeInputManager input2 = GameManager.Instance.Input;
		array = new DredgePlayerActionPress[] { this.pauseAction };
		input2.AddActionListener(array, ActionLayer.SYSTEM);
	}

	public override void OnPausePressComplete()
	{
		if (!this.isLocked && !GameManager.Instance.IsPaused && GameManager.Instance.CanPause && !GameManager.Instance.UI.ShowingWindowTypesChangedThisFrame && (GameManager.Instance.UI.ShowingWindowTypes.Count == 0 || (GameManager.Instance.UI.ShowingWindowTypes.Contains(UIWindowType.DOCK) && GameManager.Instance.UI.ShowingWindowTypes.Count == 1)))
		{
			GameManager.Instance.PauseAndShowSettings();
			ApplicationEvents.Instance.TriggerToggleSettings(true);
			base.OnPausePressComplete();
		}
	}

	public override void OnUnpausePressComplete()
	{
		if (!this.isLocked && GameManager.Instance.IsPaused && GameManager.Instance.CanUnpause && !GameManager.Instance.UI.ShowingWindowTypesChangedThisFrame)
		{
			DredgeInputManager input = GameManager.Instance.Input;
			DredgePlayerActionBase[] array = new DredgePlayerActionPress[] { this.unpauseAction };
			input.RemoveActionListener(array, ActionLayer.SYSTEM);
			GameManager.Instance.UnpauseAndDismissSettings();
			ApplicationEvents.Instance.TriggerToggleSettings(false);
			base.OnUnpausePressComplete();
		}
	}
}
