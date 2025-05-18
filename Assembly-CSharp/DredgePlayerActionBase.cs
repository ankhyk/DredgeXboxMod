using System;
using InControl;

[Serializable]
public abstract class DredgePlayerActionBase
{
	public abstract PlayerAction GetPrimaryPlayerAction();

	public abstract PlayerAction GetSecondaryPlayerAction();

	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
	}

	public void Enable()
	{
		if (!this.enabled)
		{
			this.Reset();
			this.enabled = true;
			this.forcePressed = false;
			Action<bool> onEnableStatusChanged = this.OnEnableStatusChanged;
			if (onEnableStatusChanged == null)
			{
				return;
			}
			onEnableStatusChanged(this.enabled);
		}
	}

	public void Disable(bool dispatchPressEnd)
	{
		if (this.enabled)
		{
			this.enabled = false;
			this.forcePressed = false;
			this.Reset();
			Action<bool> onEnableStatusChanged = this.OnEnableStatusChanged;
			if (onEnableStatusChanged != null)
			{
				onEnableStatusChanged(this.enabled);
			}
			if (dispatchPressEnd)
			{
				Action onPressEnd = this.OnPressEnd;
				if (onPressEnd == null)
				{
					return;
				}
				onPressEnd();
			}
		}
	}

	public void ClearListeners()
	{
		this.OnPressComplete = null;
		this.OnPressBegin = null;
		this.OnPressEnd = null;
		this.OnPressHold = null;
	}

	public bool IsHeld()
	{
		return this.playerAction.IsPressed;
	}

	public bool WasReleased()
	{
		return this.playerAction.WasReleased;
	}

	public bool IsForcePressed()
	{
		return this.forcePressed;
	}

	public void TriggerPromptArgumentsChanged()
	{
		Action onPromptArgumentsChanged = this.OnPromptArgumentsChanged;
		if (onPromptArgumentsChanged == null)
		{
			return;
		}
		onPromptArgumentsChanged();
	}

	public void SetPromptString(string promptString)
	{
		this.promptString = promptString;
		Action<string> onPromptStringChanged = this.OnPromptStringChanged;
		if (onPromptStringChanged == null)
		{
			return;
		}
		onPromptStringChanged(promptString);
	}

	public DredgePlayerActionBase(string promptString, IInputControl playerAction)
	{
		this.promptString = promptString;
		this.playerAction = playerAction;
	}

	public virtual void Update()
	{
		if (!this.enabled)
		{
			return;
		}
		if (!this.canProcessEvents)
		{
			return;
		}
		if (this.playerAction.WasPressed || this.forcePressed)
		{
			Action onPressBegin = this.OnPressBegin;
			if (onPressBegin != null)
			{
				onPressBegin();
			}
			this.hasDispatchedPressBeginEvent = true;
		}
		if ((this.playerAction.WasReleased || this.forcePressedLastFrame) && (this.hasDispatchedPressBeginEvent || this.allowPreholding || this.forcePressedLastFrame))
		{
			Action onPressEnd = this.OnPressEnd;
			if (onPressEnd != null)
			{
				onPressEnd();
			}
		}
		if (this.playerAction.IsPressed || this.forcePressed)
		{
			Action onPressHold = this.OnPressHold;
			if (onPressHold != null)
			{
				onPressHold();
			}
			if (!this.hasDispatchedPressBeginEvent && this.allowPreholding)
			{
				Action onPressBegin2 = this.OnPressBegin;
				if (onPressBegin2 != null)
				{
					onPressBegin2();
				}
				this.hasDispatchedPressBeginEvent = true;
			}
		}
	}

	public void ForcePointerDown()
	{
		this.forcePressed = true;
	}

	public void ForcePointerUp()
	{
		this.forcePressed = false;
		this.forcePressedLastFrame = true;
	}

	public virtual void LateUpdate()
	{
		if (!this.canProcessEvents)
		{
			this.canProcessEvents = true;
		}
		this.forcePressedLastFrame = false;
	}

	public virtual void Reset()
	{
		this.hasDispatchedPressBeginEvent = false;
		this.canProcessEvents = false;
	}

	public IInputControl playerAction;

	public Action OnPressComplete;

	public Action OnPressBegin;

	public Action OnPressEnd;

	public Action OnPressHold;

	public Action<bool> OnEnableStatusChanged;

	public string promptString;

	public Action<string> OnPromptStringChanged;

	public Action OnPromptArgumentsChanged;

	public ActionLayer actionLayer;

	public object[] LocalizationArguments;

	public bool showInControlArea;

	public bool showInTooltip;

	public bool evaluateWhenPaused;

	public bool allowPreholding;

	public int priority;

	protected bool enabled = true;

	protected bool hasDispatchedPressBeginEvent;

	protected bool canProcessEvents;

	protected bool forcePressed;

	protected bool forcePressedLastFrame;
}
