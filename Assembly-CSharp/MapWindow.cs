using System;
using System.Collections.Generic;
using CommandTerminal;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using InControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MapWindow : PopupWindow
{
	protected override void Awake()
	{
		this.AddTerminalCommands();
		this.worldHeight = GameManager.Instance.GameConfigData.WorldSize;
		this.halfWorldHeight = this.worldHeight * 0.5f;
		this.worldWidth = this.worldHeight * ((float)this.horizontalSectors / (float)this.verticalSectors);
		this.halfWorldWidth = this.worldWidth * 0.5f;
		this.proportionOfWorldRepresentedOnMap = this.mapViewRectWidth / 2000f;
		this.halfMapViewRectWidth = this.mapViewRectWidth * 0.5f;
		this.halfMapViewRectHeight = this.mapViewRectHeight * 0.5f;
		if (GameManager.Instance.BuildInfo && GameManager.Instance.BuildInfo.advancedMap)
		{
			this.advancedMap = true;
		}
		this.crosshairImage.gameObject.SetActive(this.advancedMap);
		if (this.advancedMap)
		{
			this.zoomAction = new DredgePlayerActionAxis("prompt.zoom-map", GameManager.Instance.Input.Controls.ZoomMap);
			this.zoomAction.evaluateWhenPaused = true;
			this.zoomAction.showInControlArea = true;
			this.moveAction = new DredgePlayerActionTwoAxis("prompt.move", GameManager.Instance.Input.Controls.MoveMap);
			this.moveAction.showInControlArea = false;
			this.recenterMapAction = new DredgePlayerActionPress("prompt.recenter-map", GameManager.Instance.Input.Controls.CameraRecenter);
			this.recenterMapAction.showInControlArea = true;
			this.recenterMapAction.priority = 1;
			this.recenterMapAction.evaluateWhenPaused = true;
			DredgePlayerActionPress dredgePlayerActionPress = this.recenterMapAction;
			dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.OnRecenterMapPressComplete));
			this.placeMarkerAction = new DredgePlayerActionPress("prompt.place-map-marker", GameManager.Instance.Input.Controls.Interact);
			this.placeMarkerAction.showInControlArea = true;
			this.placeMarkerAction.priority = 2;
			this.placeMarkerAction.evaluateWhenPaused = true;
			DredgePlayerActionPress dredgePlayerActionPress2 = this.placeMarkerAction;
			dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnPlaceMarkerPressComplete));
			this.removeAllMarkersAction = new DredgePlayerActionHold("prompt.remove-all-map-markers", GameManager.Instance.Input.Controls.RemoveMarker, 3f);
			this.removeAllMarkersAction.showInControlArea = true;
			this.removeAllMarkersAction.priority = 3;
			this.removeAllMarkersAction.evaluateWhenPaused = true;
			DredgePlayerActionHold dredgePlayerActionHold = this.removeAllMarkersAction;
			dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(this.OnRemoveAllMarkersPressComplete));
			this.removeMarkerAction = new DredgePlayerActionPress("prompt.remove-map-marker", GameManager.Instance.Input.Controls.RemoveMarker);
			this.removeMarkerAction.showInControlArea = true;
			this.removeMarkerAction.priority = 4;
			this.removeMarkerAction.evaluateWhenPaused = true;
			DredgePlayerActionPress dredgePlayerActionPress3 = this.removeMarkerAction;
			dredgePlayerActionPress3.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressComplete, new Action(this.OnRemoveMarkerPressComplete));
		}
		base.Awake();
	}

	private void OnDestroy()
	{
		this.RemoveTerminalCommands();
	}

	public override void Show()
	{
		this.boatImage.sprite = this.boatSprites[Mathf.RoundToInt(GameManager.Instance.Player.transform.eulerAngles.y / 22.5f)];
		base.Show();
		if (this.advancedMap)
		{
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.moveAction, this.zoomAction, this.placeMarkerAction, this.recenterMapAction, this.removeMarkerAction }, ActionLayer.POPUP_WINDOW);
		}
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Combine(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		this.isUsingController = GameManager.Instance.Input.IsUsingController;
		this.ZoomMapTo(this.currentZoomLevel);
		this.RefreshPlayerMarkerPosition();
		this.PlaceMapMarkersAndStamps();
		if (this.advancedMap)
		{
			this.CenterMapOnPlayer();
			this.UpdateSensitivityValues();
			this.EvaluateHasMarkers();
			this.CheckForSelectableMarkerHovered();
		}
		this.elementScaleMaintainer.IsDirty = true;
	}

	public override void Hide(PopupWindow.WindowHideMode windowHideMode)
	{
		if (this.isShowingStampContainer)
		{
			MapStampContainer mapStampContainer = this.mapStampContainer;
			mapStampContainer.StampClickedAction = (Action)Delegate.Remove(mapStampContainer.StampClickedAction, new Action(this.OnPlaceMarkerPressComplete));
			this.mapStampContainer.gameObject.SetActive(false);
			this.isShowingStampContainer = false;
			if (windowHideMode == PopupWindow.WindowHideMode.CLOSE)
			{
				return;
			}
		}
		this.allMapMarkers.ForEach(delegate(GameObject m)
		{
			this.elementScaleMaintainer.RemoveElement(m.transform as RectTransform);
			global::UnityEngine.Object.Destroy(m);
		});
		this.allMapMarkers.Clear();
		this.allSelectableMarkers.ForEach(delegate(SelectableMapMarker m)
		{
			this.elementScaleMaintainer.RemoveElement(m.transform as RectTransform);
			global::UnityEngine.Object.Destroy(m.gameObject);
		});
		this.allSelectableMarkers.Clear();
		if (this.advancedMap)
		{
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.moveAction, this.zoomAction, this.placeMarkerAction, this.removeMarkerAction, this.removeAllMarkersAction, this.recenterMapAction }, ActionLayer.POPUP_WINDOW);
		}
		DredgeInputManager input = GameManager.Instance.Input;
		input.OnInputChanged = (Action<BindingSourceType, InputDeviceStyle>)Delegate.Remove(input.OnInputChanged, new Action<BindingSourceType, InputDeviceStyle>(this.OnInputChanged));
		this.OnNotificationHideComplete(null, 0);
		base.Hide(windowHideMode);
	}

	private void OnInputChanged(BindingSourceType bindingSourceType, InputDeviceStyle deviceStyle)
	{
		this.isUsingController = GameManager.Instance.Input.IsUsingController;
		this.UpdateSensitivityValues();
	}

	private void UpdateSensitivityValues()
	{
		if (this.isUsingController)
		{
			this.moveSensitivity = this.controllerMoveSensitivity;
			return;
		}
		this.moveSensitivity = this.mouseMoveSensitivity;
	}

	private void Update()
	{
		if (!base.IsShowing || !this.advancedMap)
		{
			return;
		}
		this.moveFactorFromZoomLevel = Mathf.Lerp(1f, this.minZoom / this.maxZoom, Mathf.InverseLerp(this.minZoom, this.maxZoom, this.mapContents.localScale.x));
		if (this.dragReceiver.IsDragging)
		{
			this.mousePositionChange = this.dragReceiver.PrevMousePosition - Input.mousePosition;
			this.xMove = this.mousePositionChange.x / this.mapContents.rect.width * this.moveFactorFromZoomLevel;
			this.yMove = this.mousePositionChange.y / this.mapContents.rect.height * this.moveFactorFromZoomLevel;
		}
		else
		{
			this.xMove = this.moveAction.Value.x * this.moveFactorFromZoomLevel * this.moveSensitivity * Time.unscaledDeltaTime;
			this.yMove = this.moveAction.Value.y * this.moveFactorFromZoomLevel * this.moveSensitivity * Time.unscaledDeltaTime;
		}
		if (GameManager.Instance.Input.IsUsingController)
		{
			this.zoomSensitivity = this.controllerZoomSensitivity;
		}
		else if (GameManager.Instance.Input.Controls.LastInputType == BindingSourceType.MouseBindingSource)
		{
			this.zoomSensitivity = this.mouseZoomSensitivity;
		}
		else
		{
			this.zoomSensitivity = this.keyboardZoomSensitivity;
		}
		float value = this.zoomAction.Value;
		this.zoomMove = value * this.zoomSensitivity * Time.unscaledDeltaTime;
		if (this.isUsingController || this.dragReceiver.IsDragging)
		{
			this.smoothedXMove = this.xMove;
			this.smoothedYMove = this.yMove;
		}
		else if (this.xMove == 0f && this.yMove == 0f)
		{
			this.smoothedXMove = 0f;
			this.smoothedYMove = 0f;
		}
		else
		{
			this.smoothedXMove = Mathf.Lerp(this.smoothedXMove, this.xMove, Time.unscaledDeltaTime * this.moveAccelerationFactor);
			this.smoothedYMove = Mathf.Lerp(this.smoothedYMove, this.yMove, Time.unscaledDeltaTime * this.moveAccelerationFactor);
		}
		if (this.smoothedXMove != 0f || this.smoothedYMove != 0f)
		{
			this.MoveMapBy(this.smoothedXMove, this.smoothedYMove);
		}
		if (this.zoomMove != 0f)
		{
			this.ZoomMapBy(this.zoomMove);
		}
		this.RefreshPlayerMarkerPosition();
		this.timeUntilNextMapMarkerProximityCheck -= Time.deltaTime;
		if (this.timeUntilNextMapMarkerProximityCheck <= 0f)
		{
			this.timeUntilNextMapMarkerProximityCheck = 0.2f;
			this.CheckForSelectableMarkerHovered();
		}
	}

	private void LateUpdate()
	{
		if (this.didJustOpenMapStampContainer)
		{
			this.didJustOpenMapStampContainer = false;
		}
		this.youAreHereMarkerTransform.localPosition = new Vector2(Mathf.Clamp(this.youAreHereMarkerTransform.localPosition.x, -this.zoomedMapWidthHelper, this.zoomedMapWidthHelper), Mathf.Clamp(this.youAreHereMarkerTransform.localPosition.y, -this.zoomedMapHeightHelper, this.zoomedMapHeightHelper));
	}

	private void MoveMapBy(float xDelta, float yDelta)
	{
		this.MoveMapTo(this.mapContents.pivot.x + xDelta, this.mapContents.pivot.y + yDelta);
	}

	private void MoveMapTo(float x, float y)
	{
		this.mapContents.pivot = Vector2.ClampMagnitude(new Vector2(x, y) - this.offset, this.confinementRadius) + this.offset;
		x = this.mapContents.pivot.x;
		y = this.mapContents.pivot.y;
		this.linesX = x / (1f / (float)this.horizontalSectors) + 0.5f;
		this.linesY = y / (1f / (float)this.verticalSectors) + 0.5f;
		this.r = this.gridLinesImage.uvRect;
		this.r.position = new Vector2(this.linesX, this.linesY);
		this.gridLinesImage.uvRect = this.r;
		this.coordsHorizontal.pivot = new Vector2(x, 0.5f);
		this.coordsVertical.pivot = new Vector2(0.5f, y);
		this.CheckForSelectableMarkerHovered();
	}

	private void ZoomMapBy(float zoomDelta)
	{
		this.ZoomMapTo(this.mapContents.localScale.x + zoomDelta);
	}

	private void ZoomMapTo(float newZoom)
	{
		this.currentZoomLevel = Mathf.Max(Mathf.Min(newZoom, this.maxZoom), this.minZoom);
		this.newScaleVector = new Vector3(this.currentZoomLevel, this.currentZoomLevel, this.currentZoomLevel);
		this.mapContents.localScale = this.newScaleVector;
		this.gridLines.localScale = this.newScaleVector;
		this.coordsHorizontal.localScale = this.newScaleVector;
		this.coordsVertical.localScale = this.newScaleVector;
		this.zoomedMapWidthHelper = this.halfMapViewRectWidth / this.currentZoomLevel;
		this.zoomedMapHeightHelper = this.halfMapViewRectHeight / this.currentZoomLevel;
		this.elementScaleMaintainer.IsDirty = true;
	}

	private Vector2 GetPlayerPositionAsMapPosition()
	{
		Vector3 position = GameManager.Instance.Player.transform.position;
		return this.GetMapPositionFromWorldPosition(position.x, position.z);
	}

	private void RefreshPlayerMarkerPosition()
	{
		this.youAreHereMarkerTransform.anchoredPosition = this.GetPlayerPositionAsMapPosition();
	}

	private void CenterMapOnPlayer()
	{
		Vector2 playerPositionAsMapPosition = this.GetPlayerPositionAsMapPosition();
		Vector2 mapPivotFromMapPosition = this.GetMapPivotFromMapPosition(playerPositionAsMapPosition.x, playerPositionAsMapPosition.y);
		this.MoveMapTo(mapPivotFromMapPosition.x, mapPivotFromMapPosition.y);
	}

	private void PlaceMapMarkersAndStamps()
	{
		for (int i = 0; i < GameManager.Instance.SaveData.mapMarkers.Count; i++)
		{
			SerializedMapMarker savedMapMarker = GameManager.Instance.SaveData.mapMarkers[i];
			MapMarkerData mapMarkerData = GameManager.Instance.DataLoader.allMapMarkers.Find((MapMarkerData m) => m.name == savedMapMarker.id);
			if (mapMarkerData == null)
			{
				CustomDebug.EditorLogError("[MapWindow] PlaceMapMarkersAndStamps() couldn't find map marker data for " + savedMapMarker.id);
			}
			else
			{
				GameObject gameObject = this.mainMapMarkerPrefab;
				if (mapMarkerData.mapMarkerType == MapMarkerType.IRONHAVEN_WRECK)
				{
					gameObject = this.ironhavenWreckMapMarkerPrefab;
				}
				this.AddMapMarker(mapMarkerData.x, mapMarkerData.z, gameObject, this.mapMarkerContainer);
				if (!savedMapMarker.seen)
				{
					savedMapMarker.seen = true;
					GameEvents.Instance.TriggerHasUnseenItemsChanged();
				}
			}
		}
		for (int j = 0; j < GameManager.Instance.SaveData.serializedCrabPotPOIs.Count; j++)
		{
			Vector2 position = GameManager.Instance.SaveData.serializedCrabPotPOIs[j].GetPosition();
			this.AddMapMarker(position.x, position.y, this.crabPotPrefab, this.mapMarkerContainer);
		}
		if (GameManager.Instance.OozePatchManager)
		{
			List<OozePatch> allActiveOozePatches = GameManager.Instance.OozePatchManager.GetAllActiveOozePatches();
			for (int k = 0; k < allActiveOozePatches.Count; k++)
			{
				Vector3 position2 = allActiveOozePatches[k].transform.position;
				this.AddMapMarker(position2.x, position2.z, this.oozeSpotPrefab, this.oozeMarkerContainer);
			}
		}
		GameManager.Instance.SaveData.serializedCrabPotPOIs.ForEach(delegate(SerializedCrabPotPOIData potData)
		{
			Vector2 position3 = potData.GetPosition();
			this.AddMapMarker(position3.x, position3.y, (potData.deployableItemId == this.materialPotData.id) ? this.materialPotPrefab : this.crabPotPrefab, this.mapMarkerContainer);
		});
		for (int l = 0; l < GameManager.Instance.SaveData.mapStamps.Count; l++)
		{
			this.AddMapStamp(GameManager.Instance.SaveData.mapStamps[l], false);
		}
		GameManager.Instance.SaveData.harvestPOIMapMarkers.ForEach(new Action<string>(this.AddHarvestPOIMarker));
	}

	private void AddMapMarker(float worldX, float worldZ, GameObject prefab, Transform container)
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab, container);
		this.allMapMarkers.Add(gameObject);
		this.elementScaleMaintainer.AddElement(gameObject.transform as RectTransform);
		(gameObject.transform as RectTransform).anchoredPosition = this.GetMapPositionFromWorldPosition(worldX, worldZ);
	}

	private void AnimateMapMarker(GameObject marker)
	{
		Image componentInChildren = marker.GetComponentInChildren<Image>();
		(componentInChildren.transform as RectTransform).DOScale(1f, this.mapMarkerAnimationDuration).From(this.mapMarkerMaxScale, true, false).SetUpdate(true);
		componentInChildren.DOFade(1f, this.mapMarkerAnimationDuration).From(0f, true, false).SetUpdate(true);
	}

	private void AddHarvestPOIMarker(string id)
	{
		HarvestPOI harvestPOI = null;
		if (GameManager.Instance.HarvestPOIManager.harvestPOILookup.TryGetValue(id, out harvestPOI))
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.harvestPOIPrefab, this.harvestPOIContainer);
			MapHarvestPOIMarker component = gameObject.GetComponent<MapHarvestPOIMarker>();
			if (component)
			{
				component.Init(harvestPOI);
				this.allSelectableMarkers.Add(component);
				this.elementScaleMaintainer.AddElement(gameObject.transform as RectTransform);
				(gameObject.transform as RectTransform).anchoredPosition = this.GetMapPositionFromWorldPosition(harvestPOI.transform.position.x, harvestPOI.transform.position.z);
			}
		}
	}

	private Vector2 GetWorldPositionFromMapPivot(float x, float y)
	{
		return new Vector2(this.worldWidth * (x - 0.5f), this.worldHeight * (y - 0.5f));
	}

	private Vector2 GetMapPositionFromWorldPosition(float x, float z)
	{
		return new Vector2(x * this.proportionOfWorldRepresentedOnMap, z * this.proportionOfWorldRepresentedOnMap) / 0.95f;
	}

	private Vector2 GetMapPivotFromMapPosition(float x, float y)
	{
		return new Vector2(x / this.mapContents.rect.width + 0.5f, y / this.mapContents.rect.height + 0.5f);
	}

	private void OnPlaceMarkerPressComplete()
	{
		if (!this.isShowingStampContainer || this.didJustOpenMapStampContainer)
		{
			if (!this.isShowingStampContainer)
			{
				this.mapStampContainer.gameObject.SetActive(true);
				this.isShowingStampContainer = true;
				this.didJustOpenMapStampContainer = true;
				MapStampContainer mapStampContainer = this.mapStampContainer;
				mapStampContainer.StampClickedAction = (Action)Delegate.Combine(mapStampContainer.StampClickedAction, new Action(this.OnPlaceMarkerPressComplete));
			}
			return;
		}
		if ((float)GameManager.Instance.SaveData.GetTotalUserPlacedMapMarkers() >= GameManager.Instance.GameConfigData.MaxNumMapMarkers)
		{
			this.ShowMaxMarkerNotification();
			return;
		}
		Vector2 worldPositionFromMapPivot = this.GetWorldPositionFromMapPivot(this.mapContents.pivot.x, this.mapContents.pivot.y);
		SerializedMapStamp serializedMapStamp = new SerializedMapStamp();
		serializedMapStamp.stampType = this.mapStampContainer.CurrentStampIndex;
		serializedMapStamp.x = worldPositionFromMapPivot.x;
		serializedMapStamp.z = worldPositionFromMapPivot.y;
		GameManager.Instance.SaveData.mapStamps.Add(serializedMapStamp);
		MapStamp mapStamp = this.AddMapStamp(serializedMapStamp, true);
		GameManager.Instance.AudioPlayer.PlaySFX(this.submitSFX, AudioLayer.SFX_UI, 1f, 1f);
		this.SelectSelectableMapMarker(mapStamp);
		this.EvaluateHasMarkers();
	}

	private MapStamp AddMapStamp(SerializedMapStamp stampData, bool isNewlyPlaced)
	{
		Vector2 mapPositionFromWorldPosition = this.GetMapPositionFromWorldPosition(stampData.x, stampData.z);
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.mapStampPrefab, this.mapMarkerContainer);
		(gameObject.transform as RectTransform).localPosition = new Vector2(this.mapContents.anchoredPosition.x + mapPositionFromWorldPosition.x, this.mapContents.anchoredPosition.y + mapPositionFromWorldPosition.y);
		MapStamp component = gameObject.GetComponent<MapStamp>();
		component.Init(this.config.stampSprites[stampData.stampType], GameManager.Instance.LanguageManager.GetColor(this.config.stampColors[stampData.stampType]), stampData);
		this.elementScaleMaintainer.AddElement(component.transform as RectTransform);
		this.allSelectableMarkers.Add(component);
		if (isNewlyPlaced)
		{
			this.AnimateMapMarker(gameObject);
		}
		return component;
	}

	private void OnRecenterMapPressComplete()
	{
		this.CenterMapOnPlayer();
	}

	private void OnRemoveMarkerPressComplete()
	{
		if (this.currentlySelectedMapStamp)
		{
			this.RemoveSelectableMarker(this.currentlySelectedMapStamp);
		}
	}

	private void OnRemoveAllMarkersPressComplete()
	{
		this.removeAllMarkersAction.Disable(false);
		for (int i = this.allSelectableMarkers.Count - 1; i >= 0; i--)
		{
			this.RemoveSelectableMarker(this.allSelectableMarkers[i]);
		}
		this.EvaluateHasMarkers();
	}

	private void EvaluateHasMarkers()
	{
		if (this.allSelectableMarkers.Count > 0)
		{
			this.removeAllMarkersAction.Enable();
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.removeAllMarkersAction }, ActionLayer.POPUP_WINDOW);
			return;
		}
		GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.removeAllMarkersAction }, ActionLayer.POPUP_WINDOW);
	}

	private void RemoveSelectableMarker(SelectableMapMarker selectableMapMarker)
	{
		selectableMapMarker.RemoveMarkerFromData();
		this.allSelectableMarkers.Remove(selectableMapMarker);
		this.elementScaleMaintainer.RemoveElement(selectableMapMarker.transform as RectTransform);
		global::UnityEngine.Object.Destroy(selectableMapMarker.gameObject);
		if (selectableMapMarker == this.currentlySelectedMapStamp)
		{
			this.SelectSelectableMapMarker(null);
		}
		this.CheckForSelectableMarkerHovered();
	}

	private void AddTerminalCommands()
	{
		Terminal.Shell.AddCommand("map.moveto", new Action<CommandArg[]>(this.DebugMoveMapTo), 2, 2, "Moves the map to specified co-ordinates");
		Terminal.Shell.AddCommand("map.moveby", new Action<CommandArg[]>(this.DebugMoveMapBy), 2, 2, "Moves the map by specified co-ordinates");
		Terminal.Shell.AddCommand("map.add", new Action<CommandArg[]>(this.DebugAddMapMarker), 2, 2, "Adds a marker at the specified co-ordinates");
		Terminal.Shell.AddCommand("map.add-poi", new Action<CommandArg[]>(this.DebugAddMapMarkerPOI), 1, 1, "Adds a marker with a POI's id");
	}

	private void RemoveTerminalCommands()
	{
		Terminal.Shell.RemoveCommand("map.moveto");
		Terminal.Shell.RemoveCommand("map.moveby");
		Terminal.Shell.RemoveCommand("map.add");
		Terminal.Shell.RemoveCommand("map.add-poi");
	}

	private void DebugMoveMapTo(CommandArg[] args)
	{
		this.MoveMapTo(args[0].Float, args[1].Float);
	}

	private void DebugMoveMapBy(CommandArg[] args)
	{
		this.MoveMapBy(args[0].Float, args[1].Float);
	}

	private void DebugAddMapMarker(CommandArg[] args)
	{
		float @float = args[0].Float;
		float float2 = args[1].Float;
		Vector2 worldPositionFromMapPivot = this.GetWorldPositionFromMapPivot(@float, float2);
		SerializedMapStamp serializedMapStamp = new SerializedMapStamp();
		serializedMapStamp.stampType = 2;
		serializedMapStamp.x = worldPositionFromMapPivot.x;
		serializedMapStamp.z = worldPositionFromMapPivot.y;
		GameManager.Instance.SaveData.mapStamps.Add(serializedMapStamp);
		this.AddMapStamp(serializedMapStamp, true);
		this.EvaluateHasMarkers();
	}

	private void DebugAddMapMarkerPOI(CommandArg[] args)
	{
		string @string = args[0].String;
		GameManager.Instance.SaveData.AddHarvestPOIMarker(@string);
	}

	private void CheckForSelectableMarkerHovered()
	{
		SelectableMapMarker closestSelectableMapMarker = null;
		float closestStampDistance = float.PositiveInfinity;
		this.allSelectableMarkers.ForEach(delegate(SelectableMapMarker selectableMapMarker)
		{
			this.thisDistance = Vector3.Distance(selectableMapMarker.transform.position, this.mapContents.transform.position);
			if (this.thisDistance < this.selectThreshold && this.thisDistance < closestStampDistance)
			{
				closestSelectableMapMarker = selectableMapMarker;
				closestStampDistance = this.thisDistance;
			}
		});
		if (closestSelectableMapMarker && (this.currentlySelectedMapStamp == null || closestSelectableMapMarker != this.currentlySelectedMapStamp))
		{
			this.SelectSelectableMapMarker(closestSelectableMapMarker);
			return;
		}
		if (closestSelectableMapMarker == null)
		{
			this.SelectSelectableMapMarker(null);
		}
	}

	private void SelectSelectableMapMarker(SelectableMapMarker selectableMapMarker)
	{
		if (this.currentlySelectedMapStamp)
		{
			this.currentlySelectedMapStamp.Deselect();
			this.removeMarkerAction.Disable(false);
		}
		this.currentlySelectedMapStamp = selectableMapMarker;
		if (this.currentlySelectedMapStamp)
		{
			this.currentlySelectedMapStamp.Select();
			this.removeMarkerAction.Enable();
			this.currentlySelectedMapStamp.transform.SetAsLastSibling();
		}
	}

	private void ShowMaxMarkerNotification()
	{
		if (this.currentNotification != null)
		{
			this.OnNotificationHideComplete(null, 0);
		}
		this.currentNotification = this.notificationPrefab.Spawn(this.notificationHolder);
		RectTransform rectTransform = this.currentNotification.transform as RectTransform;
		rectTransform.anchoredPosition = new Vector2(0f, -10f);
		NotificationUI component = this.currentNotification.GetComponent<NotificationUI>();
		component.SetUseUnscaledTime(true);
		string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.STRING_TABLE, "notification.map-marker-limit", null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
		component.Init(NotificationType.ERROR, localizedString, 3f, 0, new Dictionary<NotificationType, DredgeColorTypeEnum> { 
		{
			NotificationType.ERROR,
			DredgeColorTypeEnum.NEGATIVE
		} });
		rectTransform.DOAnchorPosX(this.notificationExtendWidth, this.slideInTimeSec, false).SetEase(Ease.OutExpo).SetUpdate(true);
		component.OnHideComplete = (Action<NotificationUI, int>)Delegate.Combine(component.OnHideComplete, new Action<NotificationUI, int>(this.OnNotificationHideComplete));
	}

	private void OnNotificationHideComplete(NotificationUI ui, int index)
	{
		global::UnityEngine.Object.Destroy(this.currentNotification);
		this.currentNotification = null;
	}

	[SerializeField]
	private Sprite[] boatSprites;

	[SerializeField]
	private Image boatImage;

	[SerializeField]
	private RectTransform youAreHereMarkerTransform;

	[SerializeField]
	private int horizontalSectors;

	[SerializeField]
	private int verticalSectors;

	[SerializeField]
	private Transform mapMarkerContainer;

	[SerializeField]
	private Transform oozeMarkerContainer;

	[SerializeField]
	private GameObject mainMapMarkerPrefab;

	[SerializeField]
	private GameObject ironhavenWreckMapMarkerPrefab;

	[SerializeField]
	private GameObject crabPotPrefab;

	[SerializeField]
	private GameObject materialPotPrefab;

	[SerializeField]
	private GameObject harvestPOIPrefab;

	[SerializeField]
	private GameObject oozeSpotPrefab;

	[SerializeField]
	private SpatialItemData materialPotData;

	[SerializeField]
	private float mapMarkerAnimationDuration;

	[SerializeField]
	private float mapMarkerMaxScale;

	[SerializeField]
	private RectTransform mapContents;

	[SerializeField]
	private RectTransform gridLines;

	[SerializeField]
	private RectTransform coordsHorizontal;

	[SerializeField]
	private RectTransform coordsVertical;

	[SerializeField]
	private RawImage gridLinesImage;

	[SerializeField]
	private MapStampContainer mapStampContainer;

	[SerializeField]
	private MapStampConfig config;

	[SerializeField]
	private GameObject mapStampPrefab;

	[SerializeField]
	private MapElementScaleMaintainer elementScaleMaintainer;

	[SerializeField]
	private Image crosshairImage;

	[SerializeField]
	private AssetReference submitSFX;

	private List<SelectableMapMarker> allSelectableMarkers = new List<SelectableMapMarker>();

	private float moveSensitivity;

	private float zoomSensitivity;

	[SerializeField]
	private float mouseMoveSensitivity;

	[SerializeField]
	private float mouseZoomSensitivity;

	[SerializeField]
	private float controllerMoveSensitivity;

	[SerializeField]
	private float controllerZoomSensitivity;

	[SerializeField]
	private float keyboardZoomSensitivity;

	[SerializeField]
	private float maxZoom;

	[SerializeField]
	private float minZoom;

	[SerializeField]
	private float moveAccelerationFactor;

	[SerializeField]
	private SimpleDragReceiver dragReceiver;

	[SerializeField]
	private float proportionOfWorldRepresentedOnMap;

	private List<GameObject> allMapMarkers = new List<GameObject>();

	[SerializeField]
	private float mapViewRectWidth;

	[SerializeField]
	private float mapViewRectHeight;

	[SerializeField]
	private Transform harvestPOIContainer;

	private float worldHeight;

	private float halfWorldHeight;

	private float worldWidth;

	private float halfWorldWidth;

	private bool isUsingController;

	private bool isShowingStampContainer;

	private float xMove;

	private float yMove;

	private float smoothedXMove;

	private float smoothedYMove;

	private float moveFactorFromZoomLevel = 1f;

	private float currentZoomLevel = 1f;

	private Vector2 mousePositionChange;

	private DredgePlayerActionTwoAxis moveAction;

	private DredgePlayerActionAxis zoomAction;

	private DredgePlayerActionPress recenterMapAction;

	private DredgePlayerActionPress placeMarkerAction;

	private DredgePlayerActionPress removeMarkerAction;

	private DredgePlayerActionHold removeAllMarkersAction;

	private bool didJustOpenMapStampContainer;

	private float halfMapViewRectWidth;

	private float halfMapViewRectHeight;

	private float zoomedMapWidthHelper;

	private float zoomedMapHeightHelper;

	private bool advancedMap;

	private float zoomMove;

	private float timeUntilNextMapMarkerProximityCheck;

	private Rect r;

	private float linesX;

	private float linesY;

	[SerializeField]
	private float confinementRadius;

	private Vector2 offset = new Vector2(0.5f, 0.5f);

	private Vector3 newScaleVector;

	private float selectThreshold = 50f;

	private float thisDistance;

	private SelectableMapMarker currentlySelectedMapStamp;

	[SerializeField]
	private Transform notificationHolder;

	[SerializeField]
	private GameObject notificationPrefab;

	[SerializeField]
	private float notificationExtendWidth;

	[SerializeField]
	private float slideInTimeSec;

	private GameObject currentNotification;
}
