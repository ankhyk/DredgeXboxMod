using System;
using System.Collections.Generic;
using InControl;

public class DredgeControlBindings : PlayerActionSet
{
	private BindingListenOptions CreateListenOptions(bool includeMouseButtons)
	{
		BindingListenOptions bindingListenOptions = new BindingListenOptions();
		bindingListenOptions.MaxAllowedBindingsPerType = 1U;
		bindingListenOptions.AllowDuplicateBindingsPerSet = true;
		bindingListenOptions.IncludeModifiersAsFirstClassKeys = true;
		bindingListenOptions.IncludeMouseButtons = includeMouseButtons;
		bindingListenOptions.IncludeMouseScrollWheel = false;
		bindingListenOptions.OnBindingAdded = delegate(PlayerAction playerAction, BindingSource bindingSource)
		{
			ApplicationEvents.Instance.TriggerPlayerActionBindingChanged(playerAction);
			ApplicationEvents.Instance.TriggerSettingChanged(SettingType.CONTROL_BINDINGS);
		};
		bindingListenOptions.OnBindingEnded = delegate(PlayerAction playerAction)
		{
			ApplicationEvents.Instance.TriggerPlayerActionBindingEnded(playerAction);
		};
		bindingListenOptions.OnBindingFound = delegate(PlayerAction action, BindingSource binding)
		{
			bool flag = false;
			if (binding is KeyBindingSource)
			{
				Key include = (binding as KeyBindingSource).Control.GetInclude(0);
				if (this.cancelBindingKeys.Contains(include))
				{
					flag = true;
				}
			}
			else if (binding is DeviceBindingSource)
			{
				DeviceBindingSource deviceBindingSource = binding as DeviceBindingSource;
				if (this.cancelBindingInputs.Contains(deviceBindingSource.Control))
				{
					flag = true;
				}
			}
			if (flag)
			{
				action.StopListeningForBinding();
				return false;
			}
			return true;
		};
		return bindingListenOptions;
	}

	public DredgeControlBindings()
	{
		base.ListenOptions = this.CreateListenOptions(true);
		BindingListenOptions bindingListenOptions = this.CreateListenOptions(false);
		this.cancelBindingKeys.Add(Key.Escape);
		this.cancelBindingKeys.Add(Key.LeftCommand);
		this.cancelBindingKeys.Add(Key.RightCommand);
		this.cancelBindingInputs.Add(this.GetDefaultBackButton());
		this.cancelBindingInputs.Add(InputControlType.RightCommand);
		this.cancelBindingInputs.Add(InputControlType.Options);
		this.cancelBindingInputs.Add(InputControlType.Start);
		this.cancelBindingInputs.Add(InputControlType.Menu);
		this.cancelBindingInputs.Add(InputControlType.Plus);
		this.cancelBindingInputs.Add(InputControlType.LeftCommand);
		this.cancelBindingInputs.Add(InputControlType.Share);
		this.cancelBindingInputs.Add(InputControlType.Select);
		this.cancelBindingInputs.Add(InputControlType.Menu);
		this.cancelBindingInputs.Add(InputControlType.Minus);
		this.cancelBindingInputs.Add(InputControlType.Create);
		this.cancelBindingInputs.Add(InputControlType.Capture);
		this.cancelBindingInputs.Add(InputControlType.Back);
		this.cancelBindingInputs.Add(InputControlType.System);
		this.cancelBindingInputs.Add(InputControlType.Pause);
		this.cancelBindingInputs.Add(InputControlType.Home);
		this.cancelBindingInputs.Add(InputControlType.View);
		this.cancelBindingInputs.Add(InputControlType.Assistant);
		this.cancelBindingInputs.Add(InputControlType.Mute);
		this.MoveForward = base.CreatePlayerAction("settings.binding.move-forwards");
		this.MoveForward.AddDefaultBinding(new Key[] { Key.W });
		this.MoveForward.AddDefaultBinding(InputControlType.LeftStickUp);
		this.MoveBack = base.CreatePlayerAction("settings.binding.move-backwards");
		this.MoveBack.AddDefaultBinding(new Key[] { Key.S });
		this.MoveBack.AddDefaultBinding(InputControlType.LeftStickDown);
		this.MoveLeft = base.CreatePlayerAction("settings.binding.move-left");
		this.MoveLeft.AddDefaultBinding(new Key[] { Key.A });
		this.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.MoveRight = base.CreatePlayerAction("settings.binding.move-right");
		this.MoveRight.AddDefaultBinding(new Key[] { Key.D });
		this.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
		this.Move = base.CreateTwoAxisPlayerAction(this.MoveLeft, this.MoveRight, this.MoveBack, this.MoveForward);
		this.CameraMoveButton = base.CreatePlayerAction("settings.binding.move-camera");
		this.CameraMoveButton.AddDefaultBinding(Mouse.LeftButton);
		this.CameraRecenter = base.CreatePlayerAction("settings.binding.camera-recenter");
		this.CameraRecenter.AddDefaultBinding(Mouse.MiddleButton);
		this.CameraRecenter.AddDefaultBinding(InputControlType.RightStickButton);
		this.CameraLeft = base.CreatePlayerAction("settings.binding.camera-left");
		this.CameraLeft.AddDefaultBinding(Mouse.NegativeX);
		this.CameraLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.CameraRight = base.CreatePlayerAction("settings.binding.camera-right");
		this.CameraRight.AddDefaultBinding(Mouse.PositiveX);
		this.CameraRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.CameraBack = base.CreatePlayerAction("settings.binding.camera-down");
		this.CameraBack.AddDefaultBinding(Mouse.NegativeY);
		this.CameraBack.AddDefaultBinding(InputControlType.RightStickDown);
		this.CameraForward = base.CreatePlayerAction("settings.binding.camera-up");
		this.CameraForward.AddDefaultBinding(Mouse.PositiveY);
		this.CameraForward.AddDefaultBinding(InputControlType.RightStickUp);
		this.CameraMove = base.CreateTwoAxisPlayerAction(this.CameraLeft, this.CameraRight, this.CameraBack, this.CameraForward);
		this.RadialSelectLeft = base.CreatePlayerAction("settings.binding.radial-select-left");
		this.RadialSelectLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.RadialSelectRight = base.CreatePlayerAction("settings.binding.radial-select-right");
		this.RadialSelectRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.RadialSelectDown = base.CreatePlayerAction("settings.binding.radial-select-down");
		this.RadialSelectDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.RadialSelectUp = base.CreatePlayerAction("settings.binding.radial-select-up");
		this.RadialSelectUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.RadialSelect = base.CreateTwoAxisPlayerAction(this.RadialSelectLeft, this.RadialSelectRight, this.RadialSelectDown, this.RadialSelectUp);
		this.RadialSelectShow = base.CreatePlayerAction("settings.binding.radial-select-show");
		this.RadialSelectShow.AddDefaultBinding(new Key[] { Key.E });
		this.RadialSelectShow.AddDefaultBinding(InputControlType.LeftBumper);
		this.ToggleCargo = base.CreatePlayerAction("settings.binding.cargo");
		this.ToggleCargo.AddDefaultBinding(new Key[] { Key.Tab });
		this.ToggleCargo.AddDefaultBinding(InputControlType.Action4);
		this.Undock = base.CreatePlayerAction("settings.binding.undock");
		this.Undock.AddDefaultBinding(new Key[] { Key.X });
		this.Undock.AddDefaultBinding(this.GetDefaultBackButton());
		this.Interact = base.CreatePlayerAction("settings.binding.interact");
		this.Interact.AddDefaultBinding(new Key[] { Key.F });
		this.Interact.AddDefaultBinding(this.GetDefaultConfirmButton());
		this.CreateSecondaryInteractionBinding();
		this.Reel = base.CreatePlayerAction("settings.binding.reel");
		this.Reel.AddDefaultBinding(new Key[] { Key.F });
		this.Reel.AddDefaultBinding(InputControlType.Action3);
		this.PickUpPlace = base.CreatePlayerAction("settings.binding.pick-up");
		this.PickUpPlace.AddDefaultBinding(Mouse.LeftButton);
		this.PickUpPlace.AddDefaultBinding(this.GetDefaultConfirmButton());
		this.RotateCounterClockwise = base.CreatePlayerAction("settings.binding.rotate-counter-clockwise");
		this.RotateCounterClockwise.AddDefaultBinding(InputControlType.LeftBumper);
		this.RotateClockwise = base.CreatePlayerAction("settings.binding.rotate-clockwise");
		this.RotateClockwise.AddDefaultBinding(Mouse.RightButton);
		this.RotateClockwise.AddDefaultBinding(InputControlType.RightBumper);
		this.RotateItem = base.CreateOneAxisPlayerAction(this.RotateCounterClockwise, this.RotateClockwise);
		this.SellItem = base.CreatePlayerAction("settings.binding.sell");
		this.SellItem.AddDefaultBinding(new Key[] { Key.F });
		this.SellItem.AddDefaultBinding(InputControlType.Action3);
		this.BuyItem = base.CreatePlayerAction("settings.binding.buy");
		this.BuyItem.AddDefaultBinding(Mouse.LeftButton);
		this.BuyItem.AddDefaultBinding(this.GetDefaultConfirmButton());
		this.TabLeft = base.CreatePlayerAction("settings.binding.prev-page");
		this.TabLeft.AddDefaultBinding(new Key[] { Key.Q });
		this.TabLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.TabLeft.ListenOptions = bindingListenOptions;
		this.TabRight = base.CreatePlayerAction("settings.binding.next-page");
		this.TabRight.AddDefaultBinding(new Key[] { Key.E });
		this.TabRight.AddDefaultBinding(InputControlType.RightBumper);
		this.TabRight.ListenOptions = bindingListenOptions;
		this.SelectLeft = base.CreatePlayerAction("settings.binding.select-left-side");
		this.SelectLeft.AddDefaultBinding(InputControlType.LeftStickButton);
		this.SelectLeft.ListenOptions = bindingListenOptions;
		this.SelectRight = base.CreatePlayerAction("settings.binding.select-right-side");
		this.SelectRight.AddDefaultBinding(InputControlType.RightStickButton);
		this.SelectRight.ListenOptions = bindingListenOptions;
		this.DoAbility = base.CreatePlayerAction("settings.binding.ability");
		this.DoAbility.AddDefaultBinding(Mouse.RightButton);
		this.DoAbility.AddDefaultBinding(InputControlType.Action3);
		this.QuickMove = base.CreatePlayerAction("settings.binding.quick-move");
		this.QuickMove.AddDefaultBinding(Mouse.MiddleButton);
		this.QuickMove.AddDefaultBinding(InputControlType.Action4);
		this.Deploy = base.CreatePlayerAction("settings.binding.deploy");
		this.Deploy.AddDefaultBinding(new Key[] { Key.D });
		this.Deploy.AddDefaultBinding(InputControlType.RightTrigger);
		this.DiscardItem = base.CreatePlayerAction("settings.binding.discard");
		this.DiscardItem.AddDefaultBinding(new Key[] { Key.Z });
		this.DiscardItem.AddDefaultBinding(InputControlType.LeftTrigger);
		this.RepairItem = base.CreatePlayerAction("settings.binding.repair");
		this.RepairItem.AddDefaultBinding(Mouse.LeftButton);
		this.RepairItem.AddDefaultBinding(this.GetDefaultConfirmButton());
		this.RepairAll = base.CreatePlayerAction("settings.binding.repair-all");
		this.RepairAll.AddDefaultBinding(new Key[] { Key.R });
		this.RepairAll.AddDefaultBinding(InputControlType.Action4);
		this.RepairMode = base.CreatePlayerAction("settings.binding.repair-mode");
		this.RepairMode.AddDefaultBinding(new Key[] { Key.T });
		this.RepairMode.AddDefaultBinding(InputControlType.RightTrigger);
		this.Back = base.CreatePlayerAction("settings.binding.back");
		this.Back.AddDefaultBinding(new Key[] { Key.X });
		this.Back.AddDefaultBinding(new Key[] { Key.Escape });
		this.Back.AddDefaultBinding(this.GetDefaultBackButton());
		this.Confirm = base.CreatePlayerAction("settings.binding.confirm");
		this.Confirm.AddDefaultBinding(Mouse.LeftButton);
		this.Confirm.AddDefaultBinding(this.GetDefaultConfirmButton());
		this.OpenJournal = base.CreatePlayerAction("settings.binding.journal");
		this.OpenJournal.AddDefaultBinding(new Key[] { Key.J });
		this.OpenJournal.AddDefaultBinding(InputControlType.DPadLeft);
		this.OpenMap = base.CreatePlayerAction("settings.binding.map");
		this.OpenMap.AddDefaultBinding(new Key[] { Key.M });
		this.OpenMap.AddDefaultBinding(InputControlType.DPadUp);
		this.OpenEncyclopedia = base.CreatePlayerAction("settings.binding.encyclopedia");
		this.OpenEncyclopedia.AddDefaultBinding(new Key[] { Key.L });
		this.OpenEncyclopedia.AddDefaultBinding(InputControlType.DPadDown);
		this.OpenMessages = base.CreatePlayerAction("settings.binding.messages");
		this.OpenMessages.AddDefaultBinding(new Key[] { Key.I });
		this.OpenMessages.AddDefaultBinding(InputControlType.DPadRight);
		this.CycleAbilityPrev = base.CreatePlayerAction("settings.binding.cycle-ability-prev");
		this.CycleAbilityPrev.AddDefaultBinding(new Key[] { Key.Z });
		this.CycleAbilityPrev.AddDefaultBinding(InputControlType.LeftTrigger);
		this.CycleAbilityNext = base.CreatePlayerAction("settings.binding.cycle-ability-next");
		this.CycleAbilityNext.AddDefaultBinding(new Key[] { Key.X });
		this.CycleAbilityNext.AddDefaultBinding(InputControlType.RightTrigger);
		this.TagSpyglassMarker = base.CreatePlayerAction("settings.binding.tag-spyglass-marker");
		this.TagSpyglassMarker.AddDefaultBinding(Mouse.LeftButton);
		this.TagSpyglassMarker.AddDefaultBinding(InputControlType.RightStickButton);
		this.Pause = base.CreatePlayerAction("settings.binding.pause");
		this.Pause.AddDefaultBinding(new Key[] { Key.Escape });
		this.Pause.AddDefaultBinding(InputControlType.RightCommand);
		this.Pause.AddDefaultBinding(InputControlType.Options);
		this.Pause.AddDefaultBinding(InputControlType.Start);
		this.Pause.AddDefaultBinding(InputControlType.Menu);
		this.Pause.AddDefaultBinding(InputControlType.Plus);
		this.Pause.ListenOptions = null;
		this.Unpause = base.CreatePlayerAction("settings.binding.unpause");
		this.Unpause.AddDefaultBinding(new Key[] { Key.Escape });
		this.Unpause.AddDefaultBinding(this.GetDefaultBackButton());
		this.Unpause.AddDefaultBinding(InputControlType.RightCommand);
		this.Unpause.AddDefaultBinding(InputControlType.Options);
		this.Unpause.AddDefaultBinding(InputControlType.Start);
		this.Unpause.AddDefaultBinding(InputControlType.Menu);
		this.Unpause.AddDefaultBinding(InputControlType.Plus);
		this.Unpause.ListenOptions = null;
		this.CancelBinding = base.CreatePlayerAction("settings.binding.cancel-binding");
		this.CancelBinding.AddDefaultBinding(new Key[] { Key.Escape });
		this.CancelBinding.AddDefaultBinding(InputControlType.RightCommand);
		this.CancelBinding.AddDefaultBinding(InputControlType.Options);
		this.CancelBinding.AddDefaultBinding(InputControlType.Start);
		this.CancelBinding.AddDefaultBinding(InputControlType.Menu);
		this.CancelBinding.AddDefaultBinding(InputControlType.Plus);
		this.CancelBinding.ListenOptions = null;
		this.UnbindControl = base.CreatePlayerAction("settings.binding.unbind");
		this.UnbindControl.AddDefaultBinding(Mouse.RightButton);
		this.UnbindControl.AddDefaultBinding(InputControlType.Action4);
		this.Skip = base.CreatePlayerAction("settings.binding.skip");
		this.Skip.AddDefaultBinding(new Key[] { Key.Escape });
		this.Skip.AddDefaultBinding(this.GetDefaultBackButton());
		this.CreateMapBindings();
		this.CreatePhotoModeBindings();
		this.unRebindable.Add(this.Deploy);
		this.unRebindable.Add(this.CancelBinding);
		this.unRebindable.Add(this.Pause);
		this.unRebindable.Add(this.Unpause);
		this.unRebindable.Add(this.UnbindControl);
		this.unRebindable.Add(this.RadialSelectDown);
		this.unRebindable.Add(this.RadialSelectUp);
		this.unRebindable.Add(this.RadialSelectLeft);
		this.unRebindable.Add(this.RadialSelectRight);
		this.unRebindable.Add(this.Skip);
		this.unUnbindable.Add(this.Confirm);
		this.unUnbindable.Add(this.Pause);
		this.unUnbindable.Add(this.Interact);
		if (GameManager.Instance.IsRunnningOnSteamDeck)
		{
			this.unRebindable.Add(this.Back);
			this.unRebindable.Add(this.TabLeft);
			this.unRebindable.Add(this.TabRight);
			this.unUnbindable.Add(this.TabLeft);
			this.unUnbindable.Add(this.TabRight);
			this.unUnbindable.Add(this.Back);
		}
		this.hidden.Add(this.Deploy);
		this.hidden.Add(this.CancelBinding);
		this.hidden.Add(this.Unpause);
		this.hidden.Add(this.UnbindControl);
		this.hidden.Add(this.RadialSelectDown);
		this.hidden.Add(this.RadialSelectUp);
		this.hidden.Add(this.RadialSelectLeft);
		this.hidden.Add(this.RadialSelectRight);
		this.hidden.Add(this.Skip);
		ApplicationEvents.Instance.OnBuildInfoChanged += this.OnBuildInfoChanged;
		if (GameManager.Instance.SettingsSaveData != null)
		{
			this.LoadCustomBindings();
			return;
		}
		ApplicationEvents.Instance.OnSaveManagerInitialized += this.LoadCustomBindings;
	}

	private void LoadCustomBindings()
	{
		base.Load(GameManager.Instance.SettingsSaveData.controlBindings);
		this.CreateMapBindings();
		this.CreatePhotoModeBindings();
	}

	private void CheckMapBindingVisibility()
	{
		if (GameManager.Instance.BuildInfo)
		{
			List<PlayerAction> list = new List<PlayerAction> { this.MoveMapUp, this.MoveMapDown, this.MoveMapLeft, this.MoveMapRight, this.ZoomMapIn, this.ZoomMapOut, this.RemoveMarker };
			this.ToggleActions(list, GameManager.Instance.BuildInfo.advancedMap);
		}
	}

	private void CheckCameraBindingVisibility()
	{
		if (GameManager.Instance.BuildInfo)
		{
			List<PlayerAction> list = new List<PlayerAction>
			{
				this.PhotoCameraAbsoluteDown, this.PhotoCameraAbsoluteUp, this.PhotoCameraRelativeLeft, this.PhotoCameraRelativeRight, this.PhotoCameraRelativeForward, this.PhotoCameraRelativeBack, this.PhotoCameraRollLeft, this.PhotoCameraRollRight, this.PhotoCameraZoomIn, this.PhotoCameraZoomOut,
				this.TogglePhotoOverlay
			};
			this.ToggleActions(list, GameManager.Instance.BuildInfo.photoMode);
		}
	}

	private void ToggleActions(List<PlayerAction> actions, bool show)
	{
		if (show)
		{
			actions.ForEach(delegate(PlayerAction a)
			{
				if (this.hidden.Contains(a))
				{
					this.hidden.Remove(a);
				}
			});
			return;
		}
		actions.ForEach(delegate(PlayerAction a)
		{
			if (!this.hidden.Contains(a))
			{
				this.hidden.Add(a);
			}
		});
	}

	private void OnBuildInfoChanged()
	{
		this.CheckMapBindingVisibility();
		this.CheckCameraBindingVisibility();
	}

	private void CreateMapBindings()
	{
		if (base.GetPlayerActionByName("settings.binding.move-map-forwards") == null)
		{
			this.MoveMapUp = base.CreatePlayerAction("settings.binding.move-map-forwards");
			this.MoveMapUp.AddDefaultBinding(new Key[] { Key.W });
			this.MoveMapUp.AddDefaultBinding(InputControlType.LeftStickUp);
		}
		if (base.GetPlayerActionByName("settings.binding.move-map-backwards") == null)
		{
			this.MoveMapDown = base.CreatePlayerAction("settings.binding.move-map-backwards");
			this.MoveMapDown.AddDefaultBinding(new Key[] { Key.S });
			this.MoveMapDown.AddDefaultBinding(InputControlType.LeftStickDown);
		}
		if (base.GetPlayerActionByName("settings.binding.move-map-left") == null)
		{
			this.MoveMapLeft = base.CreatePlayerAction("settings.binding.move-map-left");
			this.MoveMapLeft.AddDefaultBinding(new Key[] { Key.A });
			this.MoveMapLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		}
		if (base.GetPlayerActionByName("settings.binding.move-map-right") == null)
		{
			this.MoveMapRight = base.CreatePlayerAction("settings.binding.move-map-right");
			this.MoveMapRight.AddDefaultBinding(new Key[] { Key.D });
			this.MoveMapRight.AddDefaultBinding(InputControlType.LeftStickRight);
		}
		this.MoveMap = base.CreateTwoAxisPlayerAction(this.MoveMapLeft, this.MoveMapRight, this.MoveMapDown, this.MoveMapUp);
		if (base.GetPlayerActionByName("settings.binding.zoom-map-in") == null)
		{
			this.ZoomMapIn = base.CreatePlayerAction("settings.binding.zoom-map-in");
			this.ZoomMapIn.AddDefaultBinding(Mouse.PositiveScrollWheel);
			this.ZoomMapIn.AddDefaultBinding(InputControlType.RightTrigger);
			this.ZoomMapIn.AddDefaultBinding(InputControlType.RightStickUp);
		}
		if (base.GetPlayerActionByName("settings.binding.zoom-map-out") == null)
		{
			this.ZoomMapOut = base.CreatePlayerAction("settings.binding.zoom-map-out");
			this.ZoomMapOut.AddDefaultBinding(Mouse.NegativeScrollWheel);
			this.ZoomMapOut.AddDefaultBinding(InputControlType.LeftTrigger);
			this.ZoomMapOut.AddDefaultBinding(InputControlType.RightStickDown);
		}
		this.ZoomMap = base.CreateOneAxisPlayerAction(this.ZoomMapOut, this.ZoomMapIn);
		if (base.GetPlayerActionByName("settings.binding.remove-map-marker") == null)
		{
			this.RemoveMarker = base.CreatePlayerAction("settings.binding.remove-map-marker");
			this.RemoveMarker.AddDefaultBinding(new Key[] { Key.R });
			this.RemoveMarker.AddDefaultBinding(InputControlType.Action3);
		}
		this.CheckMapBindingVisibility();
	}

	private void CreatePhotoModeBindings()
	{
		if (base.GetPlayerActionByName("settings.binding.toggle-photo-overlay") == null)
		{
			this.TogglePhotoOverlay = base.CreatePlayerAction("settings.binding.toggle-photo-overlay");
			this.TogglePhotoOverlay.AddDefaultBinding(new Key[] { Key.T });
			this.TogglePhotoOverlay.AddDefaultBinding(InputControlType.Action4);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-move-left") == null)
		{
			this.PhotoCameraRelativeLeft = base.CreatePlayerAction("settings.binding.photo-camera-move-left");
			this.PhotoCameraRelativeLeft.AddDefaultBinding(new Key[] { Key.A });
			this.PhotoCameraRelativeLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-move-right") == null)
		{
			this.PhotoCameraRelativeRight = base.CreatePlayerAction("settings.binding.photo-camera-move-right");
			this.PhotoCameraRelativeRight.AddDefaultBinding(new Key[] { Key.D });
			this.PhotoCameraRelativeRight.AddDefaultBinding(InputControlType.LeftStickRight);
		}
		this.PhotoCameraRelativeX = base.CreateOneAxisPlayerAction(this.PhotoCameraRelativeLeft, this.PhotoCameraRelativeRight);
		if (base.GetPlayerActionByName("settings.binding.photo-camera-move-forward") == null)
		{
			this.PhotoCameraRelativeForward = base.CreatePlayerAction("settings.binding.photo-camera-move-forward");
			this.PhotoCameraRelativeForward.AddDefaultBinding(new Key[] { Key.W });
			this.PhotoCameraRelativeForward.AddDefaultBinding(InputControlType.LeftStickUp);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-move-back") == null)
		{
			this.PhotoCameraRelativeBack = base.CreatePlayerAction("settings.binding.photo-camera-move-back");
			this.PhotoCameraRelativeBack.AddDefaultBinding(new Key[] { Key.S });
			this.PhotoCameraRelativeBack.AddDefaultBinding(InputControlType.LeftStickDown);
		}
		this.PhotoCameraRelativeZ = base.CreateOneAxisPlayerAction(this.PhotoCameraRelativeBack, this.PhotoCameraRelativeForward);
		if (base.GetPlayerActionByName("settings.binding.photo-camera-roll-left") == null)
		{
			this.PhotoCameraRollLeft = base.CreatePlayerAction("settings.binding.photo-camera-roll-left");
			this.PhotoCameraRollLeft.AddDefaultBinding(new Key[] { Key.LeftArrow });
			this.PhotoCameraRollLeft.AddDefaultBinding(InputControlType.LeftBumper);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-roll-right") == null)
		{
			this.PhotoCameraRollRight = base.CreatePlayerAction("settings.binding.photo-camera-roll-right");
			this.PhotoCameraRollRight.AddDefaultBinding(new Key[] { Key.RightArrow });
			this.PhotoCameraRollRight.AddDefaultBinding(InputControlType.RightBumper);
		}
		this.PhotoCameraRoll = base.CreateOneAxisPlayerAction(this.PhotoCameraRollLeft, this.PhotoCameraRollRight);
		if (base.GetPlayerActionByName("settings.binding.photo-camera-down") == null)
		{
			this.PhotoCameraAbsoluteDown = base.CreatePlayerAction("settings.binding.photo-camera-down");
			this.PhotoCameraAbsoluteDown.AddDefaultBinding(new Key[] { Key.E });
			this.PhotoCameraAbsoluteDown.AddDefaultBinding(InputControlType.DPadDown);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-up") == null)
		{
			this.PhotoCameraAbsoluteUp = base.CreatePlayerAction("settings.binding.photo-camera-up");
			this.PhotoCameraAbsoluteUp.AddDefaultBinding(new Key[] { Key.Q });
			this.PhotoCameraAbsoluteUp.AddDefaultBinding(InputControlType.DPadUp);
		}
		this.PhotoCameraAbsoluteY = base.CreateOneAxisPlayerAction(this.PhotoCameraAbsoluteUp, this.PhotoCameraAbsoluteDown);
		if (base.GetPlayerActionByName("settings.binding.photo-camera-zoom-in") == null)
		{
			this.PhotoCameraZoomIn = base.CreatePlayerAction("settings.binding.photo-camera-zoom-in");
			this.PhotoCameraZoomIn.AddDefaultBinding(new Key[] { Key.UpArrow });
			this.PhotoCameraZoomIn.AddDefaultBinding(InputControlType.RightTrigger);
		}
		if (base.GetPlayerActionByName("settings.binding.photo-camera-zoom-out") == null)
		{
			this.PhotoCameraZoomOut = base.CreatePlayerAction("settings.binding.photo-camera-zoom-out");
			this.PhotoCameraZoomOut.AddDefaultBinding(new Key[] { Key.DownArrow });
			this.PhotoCameraZoomOut.AddDefaultBinding(InputControlType.LeftTrigger);
		}
		this.PhotoCameraZoom = base.CreateOneAxisPlayerAction(this.PhotoCameraZoomOut, this.PhotoCameraZoomIn);
		this.CheckCameraBindingVisibility();
	}

	private void CreateSecondaryInteractionBinding()
	{
		if (base.GetPlayerActionByName("settings.binding.interact-secondary") == null)
		{
			this.InteractSecondary = base.CreatePlayerAction("settings.binding.interact-secondary");
			this.InteractSecondary.AddDefaultBinding(new Key[] { Key.X });
			this.InteractSecondary.AddDefaultBinding(this.GetDefaultBackButton());
		}
	}

	public PlayerAction GetPlayerAction(DredgeControlEnum dce)
	{
		PlayerAction playerAction = null;
		switch (dce)
		{
		case DredgeControlEnum.MOVE_FORWARD:
			playerAction = this.MoveForward;
			break;
		case DredgeControlEnum.MOVE_BACK:
			playerAction = this.MoveBack;
			break;
		case DredgeControlEnum.MOVE_LEFT:
			playerAction = this.MoveLeft;
			break;
		case DredgeControlEnum.MOVE_RIGHT:
			playerAction = this.MoveRight;
			break;
		case DredgeControlEnum.CAMERA_UP:
			playerAction = this.CameraForward;
			break;
		case DredgeControlEnum.CAMERA_DOWN:
			playerAction = this.CameraBack;
			break;
		case DredgeControlEnum.CAMERA_LEFT:
			playerAction = this.CameraLeft;
			break;
		case DredgeControlEnum.CAMERA_RIGHT:
			playerAction = this.CameraRight;
			break;
		case DredgeControlEnum.ABILITY_USE:
			playerAction = this.DoAbility;
			break;
		case DredgeControlEnum.ABILITY_CHANGE:
			playerAction = this.RadialSelectShow;
			break;
		case DredgeControlEnum.INTERACT:
			playerAction = this.Interact;
			break;
		case DredgeControlEnum.MAP:
			playerAction = this.OpenMap;
			break;
		case DredgeControlEnum.ENCYCLOPEDIA:
			playerAction = this.OpenEncyclopedia;
			break;
		case DredgeControlEnum.JOURNAL:
			playerAction = this.OpenJournal;
			break;
		case DredgeControlEnum.MESSAGES:
			playerAction = this.OpenMessages;
			break;
		case DredgeControlEnum.BACK:
			playerAction = this.Back;
			break;
		case DredgeControlEnum.CANCEL_BINDING:
			playerAction = this.CancelBinding;
			break;
		case DredgeControlEnum.SELECT_LEFT:
			playerAction = this.SelectLeft;
			break;
		case DredgeControlEnum.SELECT_RIGHT:
			playerAction = this.SelectRight;
			break;
		case DredgeControlEnum.REEL:
			playerAction = this.Reel;
			break;
		case DredgeControlEnum.CYCLE_ABILITY_PREV:
			playerAction = this.CycleAbilityPrev;
			break;
		case DredgeControlEnum.CYCLE_ABILITY_NEXT:
			playerAction = this.CycleAbilityNext;
			break;
		case DredgeControlEnum.TAG_SPYGLASS_MARKER:
			playerAction = this.TagSpyglassMarker;
			break;
		}
		return playerAction;
	}

	private InputControlType GetDefaultBackButton()
	{
		return InputControlType.Action2;
	}

	private InputControlType GetDefaultConfirmButton()
	{
		return InputControlType.Action1;
	}

	public PlayerAction MoveLeft;

	public PlayerAction MoveRight;

	public PlayerAction MoveForward;

	public PlayerAction MoveBack;

	public PlayerTwoAxisAction Move;

	public PlayerAction RadialSelectLeft;

	public PlayerAction RadialSelectRight;

	public PlayerAction RadialSelectUp;

	public PlayerAction RadialSelectDown;

	public PlayerTwoAxisAction RadialSelect;

	public PlayerAction RadialSelectShow;

	public PlayerAction CameraLeft;

	public PlayerAction CameraRight;

	public PlayerAction CameraForward;

	public PlayerAction CameraBack;

	public PlayerTwoAxisAction CameraMove;

	public PlayerAction PhotoCameraAbsoluteDown;

	public PlayerAction PhotoCameraAbsoluteUp;

	public PlayerOneAxisAction PhotoCameraAbsoluteY;

	public PlayerAction PhotoCameraRelativeLeft;

	public PlayerAction PhotoCameraRelativeRight;

	public PlayerOneAxisAction PhotoCameraRelativeX;

	public PlayerAction PhotoCameraRelativeForward;

	public PlayerAction PhotoCameraRelativeBack;

	public PlayerOneAxisAction PhotoCameraRelativeZ;

	public PlayerAction PhotoCameraRollLeft;

	public PlayerAction PhotoCameraRollRight;

	public PlayerOneAxisAction PhotoCameraRoll;

	public PlayerAction PhotoCameraZoomIn;

	public PlayerAction PhotoCameraZoomOut;

	public PlayerOneAxisAction PhotoCameraZoom;

	public PlayerAction CameraMoveButton;

	public PlayerAction CameraRecenter;

	public PlayerAction RotateClockwise;

	public PlayerAction RotateCounterClockwise;

	public PlayerOneAxisAction RotateItem;

	public PlayerAction CancelBinding;

	public PlayerAction Pause;

	public PlayerAction Unpause;

	public PlayerAction Interact;

	public PlayerAction InteractSecondary;

	public PlayerAction Back;

	public PlayerAction Undock;

	public PlayerAction PickUpPlace;

	public PlayerAction QuickMove;

	public PlayerAction Deploy;

	public PlayerAction SellItem;

	public PlayerAction BuyItem;

	public PlayerAction RepairItem;

	public PlayerAction RepairAll;

	public PlayerAction RepairMode;

	public PlayerAction DiscardItem;

	public PlayerAction ToggleCargo;

	public PlayerAction Reel;

	public PlayerAction TabLeft;

	public PlayerAction TabRight;

	public PlayerAction SelectLeft;

	public PlayerAction SelectRight;

	public PlayerAction DoAbility;

	public PlayerAction Confirm;

	public PlayerAction OpenJournal;

	public PlayerAction OpenMap;

	public PlayerAction OpenMessages;

	public PlayerAction OpenEncyclopedia;

	public PlayerAction UnbindControl;

	public PlayerAction Skip;

	public PlayerAction MoveMapLeft;

	public PlayerAction MoveMapRight;

	public PlayerAction MoveMapUp;

	public PlayerAction MoveMapDown;

	public PlayerTwoAxisAction MoveMap;

	public PlayerAction ZoomMapIn;

	public PlayerAction ZoomMapOut;

	public PlayerOneAxisAction ZoomMap;

	public PlayerAction RemoveMarker;

	public PlayerAction TogglePhotoOverlay;

	public PlayerAction CycleAbilityPrev;

	public PlayerAction CycleAbilityNext;

	public PlayerAction TagSpyglassMarker;

	public List<PlayerAction> unUnbindable = new List<PlayerAction>();

	public List<PlayerAction> unRebindable = new List<PlayerAction>();

	public List<PlayerAction> hidden = new List<PlayerAction>();

	private List<InputControlType> cancelBindingInputs = new List<InputControlType>();

	private List<Key> cancelBindingKeys = new List<Key>();
}
