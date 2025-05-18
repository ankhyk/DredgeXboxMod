using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-950)]
public class DredgeInputManager : SerializedMonoBehaviour
{
	public DredgeControlBindings Controls
	{
		get
		{
			return this.controls;
		}
	}

	public BindingSourceType CurrentBindingSource
	{
		get
		{
			return this.prevInputControlType;
		}
	}

	public InputDeviceStyle CurrentDeviceStyle
	{
		get
		{
			return this.prevInputDeviceStyle;
		}
	}

	public bool IsUsingController
	{
		get
		{
			return this.prevInputControlType == BindingSourceType.DeviceBindingSource;
		}
	}

	private void Awake()
	{
		this.Reset();
		this.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(this.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.HandleOnInputChanged));
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		ApplicationEvents.Instance.OnFeedbackFormToggled += this.OnFeedbackFormToggled;
		ApplicationEvents.Instance.OnSaveManagerInitialized += this.OnSaveManagerInitialized;
	}

	private void OnSaveManagerInitialized()
	{
		ApplicationEvents.Instance.OnSaveManagerInitialized -= this.OnSaveManagerInitialized;
		this.RefreshDeviceStyleOverride();
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CONSTRAIN_CURSOR)
		{
			this.RefreshCursorLockState();
			return;
		}
		if (settingType == SettingType.CONTROL_ICON_STYLE)
		{
			this.RefreshDeviceStyleOverride();
		}
	}

	private void RefreshDeviceStyleOverride()
	{
		int controlIconStyle = GameManager.Instance.SettingsSaveData.controlIconStyle;
		switch (controlIconStyle)
		{
		case 0:
			this.deviceStyleOverride = InputDeviceStyle.Unknown;
			break;
		case 1:
			this.deviceStyleOverride = InputDeviceStyle.XboxOne;
			break;
		case 2:
			this.deviceStyleOverride = InputDeviceStyle.PlayStation5;
			break;
		case 3:
			this.deviceStyleOverride = InputDeviceStyle.NintendoSwitch;
			break;
		}
		if (controlIconStyle != 0)
		{
			Action<BindingSourceType, InputDeviceStyle> onInputChanged = this.OnInputChanged;
			if (onInputChanged == null)
			{
				return;
			}
			onInputChanged(BindingSourceType.DeviceBindingSource, this.deviceStyleOverride);
		}
	}

	private void HandleOnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle inputDeviceStyle)
	{
		this.RefreshCursorLockState();
	}

	public void RefreshCursorLockState()
	{
		if (this.IsUsingController)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			return;
		}
		bool flag = false;
		if (GameManager.Instance.SettingsSaveData != null)
		{
			flag = GameManager.Instance.SettingsSaveData.constrainCursor == 1;
		}
		Cursor.lockState = (flag ? CursorLockMode.Confined : CursorLockMode.None);
		if (this.cursorVisibilityStates.ContainsKey(this.activeActionLayer))
		{
			Cursor.visible = this.cursorVisibilityStates[this.activeActionLayer];
		}
	}

	private void Start()
	{
		if (GameManager.Instance.IsRunnningOnSteamDeck)
		{
			this.prevInputControlType = BindingSourceType.DeviceBindingSource;
			this.prevInputDeviceStyle = InputDeviceStyle.XboxOne;
		}
		else
		{
			this.prevInputControlType = BindingSourceType.KeyBindingSource;
			this.prevInputDeviceStyle = InputDeviceStyle.Unknown;
		}
		this.controls = new DredgeControlBindings();
		this.controls.LastInputType = this.prevInputControlType;
		this.controls.LastDeviceStyle = this.prevInputDeviceStyle;
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Combine(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Combine(instance2.OnGameEnded, new Action(this.OnGameEnded));
		SceneManager.sceneUnloaded += this.OnSceneUnloaded;
	}

	private void OnDestroy()
	{
		GameManager instance = GameManager.Instance;
		instance.OnGameStarted = (Action)Delegate.Remove(instance.OnGameStarted, new Action(this.OnGameStarted));
		GameManager instance2 = GameManager.Instance;
		instance2.OnGameEnded = (Action)Delegate.Remove(instance2.OnGameEnded, new Action(this.OnGameEnded));
		ApplicationEvents.Instance.OnFeedbackFormToggled -= this.OnFeedbackFormToggled;
		ApplicationEvents.Instance.OnSaveManagerInitialized -= this.OnSaveManagerInitialized;
		this.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(this.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.HandleOnInputChanged));
	}

	private void OnFocus()
	{
		this.RefreshCursorLockState();
	}

	private void OnFeedbackFormToggled(bool showing)
	{
		this.isShowingFeedbackForm = showing;
	}

	private void OnGameStarted()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged += this.OnTimeForcefullyPassingChanged;
		GameEvents.Instance.OnPauseChange += this.OnPauseChange;
	}

	private void OnGameEnded()
	{
		GameEvents.Instance.OnTimeForcefullyPassingChanged -= this.OnTimeForcefullyPassingChanged;
		GameEvents.Instance.OnPauseChange -= this.OnPauseChange;
	}

	private void OnPauseChange(bool isPaused)
	{
		if (isPaused)
		{
			this.DisableGameInput();
			return;
		}
		this.ResumeGameInput();
	}

	private void OnTimeForcefullyPassingChanged(bool isPassing, string reasonKey, TimePassageMode mode)
	{
		if (isPassing)
		{
			this.DisableGameInput();
			return;
		}
		this.ResumeGameInput();
	}

	private void DisableGameInput()
	{
		if (this.activeActionLayer != ActionLayer.SYSTEM)
		{
			this.cachedActionLayer = this.activeActionLayer;
		}
		this.SetActiveActionLayer(ActionLayer.SYSTEM);
	}

	private void ResumeGameInput()
	{
		this.playerActionSets[(int)this.cachedActionLayer].ResetAllActions();
		this.SetActiveActionLayer(this.cachedActionLayer);
	}

	private void Reset()
	{
		this.playerActionSets = new DredgePlayerActionSet[9];
		for (int i = 0; i < 9; i++)
		{
			this.playerActionSets[i] = new DredgePlayerActionSet();
		}
	}

	public void ResetAllBindings()
	{
		for (int i = 0; i < GameManager.Instance.Input.Controls.Actions.Count; i++)
		{
			this.ResetBinding(GameManager.Instance.Input.Controls.Actions[i]);
		}
		ApplicationEvents.Instance.TriggerSettingChanged(SettingType.CONTROL_BINDINGS);
	}

	public void ResetBinding(PlayerAction playerAction)
	{
		playerAction.ResetBindings();
		ApplicationEvents.Instance.TriggerPlayerActionBindingChanged(playerAction);
	}

	private void OnSceneUnloaded(Scene scene)
	{
		this.Reset();
	}

	public void AddActionListener(DredgePlayerActionBase[] actions, ActionLayer actionLayer)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (DredgePlayerActionBase dredgePlayerActionBase in actions)
		{
			dredgePlayerActionBase.Reset();
			dredgePlayerActionBase.actionLayer = actionLayer;
			flag2 = this.playerActionSets[(int)actionLayer].AddAction(dredgePlayerActionBase) || flag2;
			if (dredgePlayerActionBase.showInControlArea || dredgePlayerActionBase.showInTooltip)
			{
				flag = true;
			}
		}
		if (flag2 && actionLayer == this.activeActionLayer && flag)
		{
			this._isDirty = true;
		}
	}

	public void RemoveActionListener(DredgePlayerActionBase[] actions, ActionLayer actionLayer)
	{
		bool flag = false;
		bool flag2 = false;
		foreach (DredgePlayerActionBase dredgePlayerActionBase in actions)
		{
			if (dredgePlayerActionBase != null)
			{
				dredgePlayerActionBase.Reset();
				flag2 = this.playerActionSets[(int)actionLayer].RemoveAction(dredgePlayerActionBase) || flag2;
				if (dredgePlayerActionBase.showInControlArea || dredgePlayerActionBase.showInTooltip)
				{
					flag = true;
				}
			}
		}
		if (flag2 && actionLayer == this.activeActionLayer && flag)
		{
			this._isDirty = true;
		}
	}

	public void SetActiveActionLayer(ActionLayer actionLayer)
	{
		bool flag = this.activeActionLayer != actionLayer;
		this.activeActionLayer = actionLayer;
		if (flag)
		{
			ApplicationEvents.Instance.TriggerInputActionLayerChanged(actionLayer);
			this.RefreshCursorLockState();
			this._isDirty = true;
		}
	}

	public DredgePlayerActionSet GetActiveActionSet()
	{
		return this.playerActionSets[(int)this.activeActionLayer];
	}

	public ActionLayer GetActiveActionLayer()
	{
		return this.activeActionLayer;
	}

	private void Update()
	{
		if (this.isShowingFeedbackForm)
		{
			return;
		}
		if (this.controls.LastDeviceStyle != this.prevInputDeviceStyle || this.controls.LastInputType != this.prevInputControlType)
		{
			if ((this.prevInputControlType == BindingSourceType.KeyBindingSource && this.controls.LastInputType == BindingSourceType.MouseBindingSource) || (this.prevInputControlType == BindingSourceType.MouseBindingSource && this.controls.LastInputType == BindingSourceType.KeyBindingSource))
			{
				this.prevInputDeviceStyle = this.controls.LastDeviceStyle;
				this.prevInputControlType = this.controls.LastInputType;
			}
			else
			{
				this.prevInputDeviceStyle = this.controls.LastDeviceStyle;
				this.prevInputControlType = this.controls.LastInputType;
				Action<BindingSourceType, InputDeviceStyle> onInputChanged = this.OnInputChanged;
				if (onInputChanged != null)
				{
					onInputChanged(this.prevInputControlType, this.prevInputDeviceStyle);
				}
			}
		}
		DredgePlayerActionSet activeActionSet = this.GetActiveActionSet();
		if (activeActionSet != null)
		{
			activeActionSet.Update();
		}
		if (this.activeActionLayer != ActionLayer.PERSISTENT && this.activeActionLayer != ActionLayer.NONE && this.activeActionLayer != ActionLayer.SYSTEM)
		{
			DredgePlayerActionSet dredgePlayerActionSet = this.playerActionSets[2];
			if (dredgePlayerActionSet != null)
			{
				dredgePlayerActionSet.Update();
			}
		}
		if (this.activeActionLayer != ActionLayer.SYSTEM)
		{
			DredgePlayerActionSet dredgePlayerActionSet2 = this.playerActionSets[1];
			if (dredgePlayerActionSet2 != null)
			{
				dredgePlayerActionSet2.Update();
			}
		}
		if (this._isDirty)
		{
			Action<DredgePlayerActionSet> onPlayerActionSetChanged = this.OnPlayerActionSetChanged;
			if (onPlayerActionSetChanged != null)
			{
				onPlayerActionSetChanged(this.GetActiveActionSet());
			}
			this._isDirty = false;
		}
	}

	private void LateUpdate()
	{
		DredgePlayerActionSet activeActionSet = this.GetActiveActionSet();
		if (activeActionSet != null)
		{
			activeActionSet.LateUpdate();
		}
		if (this.activeActionLayer != ActionLayer.PERSISTENT && this.activeActionLayer != ActionLayer.NONE && this.activeActionLayer != ActionLayer.SYSTEM)
		{
			DredgePlayerActionSet dredgePlayerActionSet = this.playerActionSets[2];
			if (dredgePlayerActionSet != null)
			{
				dredgePlayerActionSet.LateUpdate();
			}
		}
		if (this.activeActionLayer != ActionLayer.SYSTEM)
		{
			DredgePlayerActionSet dredgePlayerActionSet2 = this.playerActionSets[1];
			if (dredgePlayerActionSet2 == null)
			{
				return;
			}
			dredgePlayerActionSet2.LateUpdate();
		}
	}

	public bool GetIsActionHeld(DredgePlayerActionBase playerActionBase)
	{
		return playerActionBase.IsHeld() && playerActionBase.actionLayer == this.activeActionLayer;
	}

	public float GetValue(DredgePlayerActionAxis playerActionAxis)
	{
		if (playerActionAxis.actionLayer == this.activeActionLayer)
		{
			return playerActionAxis.Value;
		}
		return 0f;
	}

	public Vector2 GetValue(DredgePlayerActionTwoAxis playerActionAxis)
	{
		if (playerActionAxis.actionLayer == this.activeActionLayer)
		{
			return playerActionAxis.Value;
		}
		return Vector2.zero;
	}

	public bool GetHasBindingForAction(PlayerAction action, BindingSourceType requestedBindingSourceType, bool combineMouseKeyboard)
	{
		return this.GetBindingForAction(action, requestedBindingSourceType, combineMouseKeyboard) != null;
	}

	public BindingSource GetBindingForAction(PlayerAction action, BindingSourceType requestedBindingSourceType, bool combineMouseKeyboard)
	{
		if (requestedBindingSourceType == BindingSourceType.None)
		{
			requestedBindingSourceType = BindingSourceType.KeyBindingSource;
		}
		BindingSource bindingSource;
		if (combineMouseKeyboard && (requestedBindingSourceType == BindingSourceType.KeyBindingSource || requestedBindingSourceType == BindingSourceType.MouseBindingSource))
		{
			bindingSource = action.Bindings.FirstOrDefault((BindingSource bs) => bs.BindingSourceType == requestedBindingSourceType);
			if (bindingSource == null)
			{
				BindingSourceType altBindingSourceType = ((requestedBindingSourceType == BindingSourceType.KeyBindingSource) ? BindingSourceType.MouseBindingSource : BindingSourceType.KeyBindingSource);
				bindingSource = action.Bindings.FirstOrDefault((BindingSource bs) => bs.BindingSourceType == altBindingSourceType);
			}
		}
		else
		{
			bindingSource = action.Bindings.FirstOrDefault((BindingSource bs) => bs.BindingSourceType == requestedBindingSourceType);
		}
		return bindingSource;
	}

	public ControlIconData GetControlIconForActionWithDefault(PlayerAction action)
	{
		return this.GetControlIconForAction(action, this.controls.LastInputType, true) ?? this.defaultControlIconData;
	}

	public ControlIconData GetControlIconForAction(PlayerAction action, BindingSourceType requestedBindingSourceType, bool combineMouseKeyboard)
	{
		if (requestedBindingSourceType == BindingSourceType.None)
		{
			requestedBindingSourceType = BindingSourceType.KeyBindingSource;
		}
		for (int i = 0; i < action.Bindings.Count; i++)
		{
			BindingSource bindingSource = action.Bindings[i];
			if (bindingSource != null)
			{
				if (requestedBindingSourceType == BindingSourceType.DeviceBindingSource)
				{
					if (bindingSource is DeviceBindingSource)
					{
						InputControlType control = (bindingSource as DeviceBindingSource).Control;
						InputDeviceStyle inputDeviceStyle = this.controls.LastDeviceStyle;
						if (this.deviceStyleOverride != InputDeviceStyle.Unknown)
						{
							inputDeviceStyle = this.deviceStyleOverride;
						}
						if (inputDeviceStyle == InputDeviceStyle.Unknown)
						{
							inputDeviceStyle = InputDeviceStyle.Xbox360;
						}
						DeviceControlIconData deviceControlIconData;
						if (!this.deviceControlIcons.TryGetValue(inputDeviceStyle, out deviceControlIconData))
						{
							this.deviceControlIcons.TryGetValue(InputDeviceStyle.Xbox360, out deviceControlIconData);
						}
						if (deviceControlIconData.controlIcons.ContainsKey(control))
						{
							return deviceControlIconData.controlIcons[control];
						}
					}
				}
				else if (combineMouseKeyboard && (requestedBindingSourceType == BindingSourceType.KeyBindingSource || requestedBindingSourceType == BindingSourceType.MouseBindingSource))
				{
					if (bindingSource is KeyBindingSource)
					{
						return this.GetControlIconForKeyboardBinding(bindingSource as KeyBindingSource);
					}
					if (bindingSource is MouseBindingSource)
					{
						return this.GetControlIconForMouseBinding(bindingSource as MouseBindingSource);
					}
				}
				else
				{
					if (requestedBindingSourceType == BindingSourceType.KeyBindingSource && bindingSource is KeyBindingSource)
					{
						return this.GetControlIconForKeyboardBinding(bindingSource as KeyBindingSource);
					}
					if (requestedBindingSourceType == BindingSourceType.MouseBindingSource && bindingSource is MouseBindingSource)
					{
						return this.GetControlIconForMouseBinding(bindingSource as MouseBindingSource);
					}
				}
			}
		}
		return null;
	}

	private ControlIconData GetControlIconForKeyboardBinding(KeyBindingSource keyBindingSource)
	{
		Key include = keyBindingSource.Control.GetInclude(0);
		if (this.keyboardControlIconData.controlIcons.ContainsKey(include))
		{
			return this.keyboardControlIconData.controlIcons[include];
		}
		return null;
	}

	private ControlIconData GetControlIconForMouseBinding(MouseBindingSource mouseBindingSource)
	{
		Mouse control = mouseBindingSource.Control;
		if (this.mouseControlIconData.controlIcons.ContainsKey(control))
		{
			return this.mouseControlIconData.controlIcons[control];
		}
		return null;
	}

	public ControlIconData GetControlIconForActions(PlayerAction actionA, PlayerAction actionB)
	{
		return null;
	}

	public string GetControlStringForAction(PlayerAction action, BindingSourceType requestedBindingSourceType)
	{
		for (int i = 0; i < action.Bindings.Count; i++)
		{
			BindingSource bindingSource = action.Bindings[i];
			if (bindingSource != null)
			{
				if (requestedBindingSourceType == BindingSourceType.DeviceBindingSource && bindingSource is DeviceBindingSource)
				{
					return (bindingSource as DeviceBindingSource).Control.ToString();
				}
				if (requestedBindingSourceType == BindingSourceType.KeyBindingSource && bindingSource is KeyBindingSource)
				{
					return (bindingSource as KeyBindingSource).Control.GetInclude(0).ToString();
				}
				if (requestedBindingSourceType == BindingSourceType.MouseBindingSource && bindingSource is MouseBindingSource)
				{
					return (bindingSource as MouseBindingSource).Control.ToString();
				}
			}
		}
		return "-";
	}

	private DredgeControlBindings controls;

	private DredgePlayerActionSet[] playerActionSets;

	[SerializeField]
	private ActionLayer activeActionLayer = ActionLayer.SYSTEM;

	[SerializeField]
	private Dictionary<ActionLayer, bool> cursorVisibilityStates;

	[HideInInspector]
	public Action<DredgePlayerActionSet> OnPlayerActionSetChanged;

	public KeyboardControlIconData keyboardControlIconData;

	public MouseControlIconData mouseControlIconData;

	public ControlIconData defaultControlIconData;

	public Dictionary<InputDeviceStyle, DeviceControlIconData> deviceControlIcons = new Dictionary<InputDeviceStyle, DeviceControlIconData>();

	[HideInInspector]
	public Action<BindingSourceType, InputDeviceStyle> OnInputChanged;

	private BindingSourceType prevInputControlType;

	private InputDeviceStyle prevInputDeviceStyle;

	private ActionLayer cachedActionLayer;

	private bool isShowingFeedbackForm;

	private bool isWaitingToShowDisconnectionMessage;

	private InputDeviceStyle deviceStyleOverride;

	private bool _isDirty;
}
