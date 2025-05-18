using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraAbility : Ability
{
	public override void Init()
	{
		base.Init();
		this.mainCamera = Camera.main;
		this.cinemachineBrain = this.mainCamera.gameObject.GetComponent<CinemachineBrain>();
		this.screenshotStrategy = new GDKPCScreenshotStrategy();
		this.screenshotStrategy.Init();
		this.defaultDutch = this.abilityCamera.m_Lens.Dutch;
		this.defaultFOV = this.abilityCamera.m_Lens.FieldOfView;
		this.abilityCamera.enabled = false;
		this.altBackAction = new DredgePlayerActionPress("settings.binding.back", GameManager.Instance.Input.Controls.DoAbility);
		DredgePlayerActionPress dredgePlayerActionPress = this.altBackAction;
		dredgePlayerActionPress.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressComplete, new Action(this.Deactivate));
		this.toggleOverlayAction = new DredgePlayerActionPress("settings.binding.toggle-photo-overlay", GameManager.Instance.Input.Controls.TogglePhotoOverlay);
		this.toggleOverlayAction.showInControlArea = true;
		this.toggleOverlayAction.priority = 2;
		DredgePlayerActionPress dredgePlayerActionPress2 = this.toggleOverlayAction;
		dredgePlayerActionPress2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress2.OnPressComplete, new Action(this.OnToggleOverlayPressComplete));
		this.takePhotoAction = new DredgePlayerActionPress("settings.binding.take-photo", GameManager.Instance.Input.Controls.Interact);
		this.takePhotoAction.showInControlArea = true;
		this.takePhotoAction.priority = 3;
		DredgePlayerActionPress dredgePlayerActionPress3 = this.takePhotoAction;
		dredgePlayerActionPress3.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionPress3.OnPressComplete, new Action(this.OnTakePhotoPressComplete));
		this.cameraRelativeXAction = new DredgePlayerActionAxis("prompt.photo-camera.x", GameManager.Instance.Input.Controls.PhotoCameraRelativeX);
		this.cameraRelativeXAction.showInControlArea = false;
		this.cameraRelativeXAction.priority = 4;
		this.cameraRelativeZAction = new DredgePlayerActionAxis("prompt.photo-camera.z", GameManager.Instance.Input.Controls.PhotoCameraRelativeZ);
		this.cameraRelativeZAction.showInControlArea = false;
		this.cameraRelativeZAction.priority = 5;
		this.cameraAbsoluteYAction = new DredgePlayerActionAxis("prompt.photo-camera.y", GameManager.Instance.Input.Controls.PhotoCameraAbsoluteY);
		this.cameraAbsoluteYAction.showInControlArea = true;
		this.cameraAbsoluteYAction.priority = 6;
		this.cameraRollAction = new DredgePlayerActionAxis("prompt.photo-camera.roll", GameManager.Instance.Input.Controls.PhotoCameraRoll);
		this.cameraRollAction.showInControlArea = true;
		this.cameraRollAction.priority = 7;
		this.cameraZoomAction = new DredgePlayerActionAxis("prompt.photo-camera.zoom", GameManager.Instance.Input.Controls.PhotoCameraZoom);
		this.cameraZoomAction.showInControlArea = true;
		this.cameraZoomAction.priority = 8;
	}

	private void OnEnable()
	{
		ApplicationEvents.Instance.OnSettingChanged += this.OnSettingChanged;
		GameEvents.Instance.OnPauseChange += this.OnPauseChange;
		this.RefreshCameraSensitivity();
	}

	private void OnDisable()
	{
		ApplicationEvents.Instance.OnSettingChanged -= this.OnSettingChanged;
		GameEvents.Instance.OnPauseChange += this.OnPauseChange;
	}

	private void OnDestroy()
	{
		this.screenshotStrategy.UnInit();
	}

	private void OnToggleOverlayPressComplete()
	{
		this.shouldShowOverlay = !this.shouldShowOverlay;
		GameManager.Instance.UI.TogglePhotoModeCanvasShow(this.shouldShowOverlay);
	}

	private void OnTakePhotoPressComplete()
	{
		base.StartCoroutine(this.DoTakePhoto());
	}

	private IEnumerator DoTakePhoto()
	{
		if (this.isTakingPhoto)
		{
			yield break;
		}
		this.isTakingPhoto = true;
		ApplicationEvents.Instance.TriggerUIDebugToggled(false);
		yield return new WaitForSecondsRealtime(0.25f);
		yield return base.StartCoroutine(this.screenshotStrategy.DoTakePhoto());
		this.audioSource.PlayOneShot(this.takePhotoClips.PickRandom<AudioClip>());
		yield return new WaitForEndOfFrame();
		ApplicationEvents.Instance.TriggerUIDebugToggled(true);
		this.isTakingPhoto = false;
		yield break;
	}

	private void OnSettingChanged(SettingType settingType)
	{
		if (settingType == SettingType.CAMERA_SENSITIVITY_X || settingType == SettingType.CAMERA_SENSITIVITY_Y)
		{
			this.RefreshCameraSensitivity();
		}
	}

	private void OnPauseChange(bool isPaused)
	{
		if (this.isActive)
		{
			this.Deactivate();
		}
	}

	private void RefreshCameraSensitivity()
	{
		this.cameraSensitivityX = GameManager.Instance.SettingsSaveData.cameraSensitivityX;
		this.cameraSensitivityY = GameManager.Instance.SettingsSaveData.cameraSensitivityY;
	}

	public override bool Activate()
	{
		bool flag = false;
		if (this.isActive)
		{
			this.Deactivate();
		}
		else
		{
			this.cinemachineBrain.m_IgnoreTimeScale = true;
			this.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("InteractPointUI"));
			this.ResetCamera();
			this.abilityCamera.enabled = true;
			GameManager.Instance.UI.ToggleGameCanvasShow(false);
			GameManager.Instance.UI.TogglePhotoModeCanvasShow(this.shouldShowOverlay);
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.PHOTO_MODE);
			GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[] { this.cameraRelativeXAction, this.cameraAbsoluteYAction, this.cameraRelativeZAction, this.cameraRollAction, this.cameraZoomAction, this.toggleOverlayAction, this.takePhotoAction, this.altBackAction }, ActionLayer.PHOTO_MODE);
			GameManager.Instance.UI.InventoryPanelTab.Unsubscribe();
			Time.timeScale = 0f;
			this.audioSource.PlayOneShot(this.activateClip);
			AudioListener.pause = true;
			this.isActive = true;
			flag = base.Activate();
		}
		return flag;
	}

	private void ResetCamera()
	{
		this.abilityCamera.m_Lens.Dutch = this.defaultDutch;
		this.abilityCamera.m_Lens.FieldOfView = this.defaultFOV;
	}

	private void Update()
	{
		if (this.isActive)
		{
			this.abilityCamera.transform.position += this.mainCamera.transform.right * this.cameraRelativeXAction.Value * this.panSensitivity * Time.unscaledDeltaTime;
			this.abilityCamera.transform.position += Vector3.up * -this.cameraAbsoluteYAction.Value * this.panSensitivity * Time.unscaledDeltaTime;
			this.abilityCamera.transform.position += this.mainCamera.transform.forward * this.cameraRelativeZAction.Value * this.panSensitivity * Time.unscaledDeltaTime;
			CinemachineVirtualCamera cinemachineVirtualCamera = this.abilityCamera;
			cinemachineVirtualCamera.m_Lens.Dutch = cinemachineVirtualCamera.m_Lens.Dutch - this.cameraRollAction.Value * this.rollSensitivity * Time.unscaledDeltaTime;
			this.abilityCamera.m_Lens.Dutch = Mathf.Clamp(this.abilityCamera.m_Lens.Dutch, this.minRoll, this.maxRoll);
			CinemachineVirtualCamera cinemachineVirtualCamera2 = this.abilityCamera;
			cinemachineVirtualCamera2.m_Lens.FieldOfView = cinemachineVirtualCamera2.m_Lens.FieldOfView - this.cameraZoomAction.Value * this.zoomSensitivity * Time.unscaledDeltaTime;
			this.abilityCamera.m_Lens.FieldOfView = Mathf.Clamp(this.abilityCamera.m_Lens.FieldOfView, this.minZoom, this.maxZoom);
			this.RestrictMovement();
		}
		if (this.screenshotStrategy == null)
		{
			return;
		}
		this.screenshotStrategy.Update();
	}

	private void LateUpdate()
	{
	}

	private void RestrictMovement()
	{
		this.abilityCamera.transform.localPosition = Vector3.ClampMagnitude(this.abilityCamera.transform.localPosition, this.confinementRadius);
		this.abilityCamera.transform.position = new Vector3(this.abilityCamera.transform.position.x, Mathf.Max(this.floorHeight, this.abilityCamera.transform.position.y), this.abilityCamera.transform.position.z);
	}

	public override void Deactivate()
	{
		if (this.isActive)
		{
			this.abilityCamera.enabled = false;
			this.cinemachineBrain.m_IgnoreTimeScale = false;
			this.mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("InteractPointUI");
			if (!GameManager.Instance.IsPaused)
			{
				Time.timeScale = 1f;
				AudioListener.pause = false;
			}
			GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.BASE);
			this.audioSource.PlayOneShot(this.deactivateClip);
			GameManager.Instance.UI.ToggleGameCanvasShow(true);
			GameManager.Instance.UI.TogglePhotoModeCanvasShow(false);
			GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[] { this.cameraRelativeXAction, this.cameraAbsoluteYAction, this.cameraRelativeZAction, this.cameraRollAction, this.cameraZoomAction, this.toggleOverlayAction, this.takePhotoAction, this.altBackAction }, ActionLayer.PHOTO_MODE);
			GameManager.Instance.UI.InventoryPanelTab.Subscribe();
		}
		base.Deactivate();
	}

	[SerializeField]
	private CinemachineVirtualCamera abilityCamera;

	[SerializeField]
	private CinemachineFreeLook playerVCam;

	[SerializeField]
	private float panSensitivity;

	[SerializeField]
	private float rollSensitivity;

	[SerializeField]
	private float minRoll;

	[SerializeField]
	private float maxRoll;

	[SerializeField]
	private float zoomSensitivity;

	[SerializeField]
	private float minZoom;

	[SerializeField]
	private float maxZoom;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip activateClip;

	[SerializeField]
	private AudioClip deactivateClip;

	[SerializeField]
	private List<AudioClip> takePhotoClips;

	private CinemachineBrain cinemachineBrain;

	private DredgePlayerActionAxis cameraZoomAction;

	private DredgePlayerActionAxis cameraRelativeZAction;

	private DredgePlayerActionAxis cameraRelativeXAction;

	private DredgePlayerActionAxis cameraRollAction;

	private DredgePlayerActionAxis cameraAbsoluteYAction;

	private DredgePlayerActionPress toggleOverlayAction;

	private DredgePlayerActionPress takePhotoAction;

	private DredgePlayerActionPress altBackAction;

	private bool shouldShowOverlay = true;

	private float cameraSensitivityX;

	private float cameraSensitivityY;

	private IScreenshotStrategy screenshotStrategy;

	private Camera mainCamera;

	private float defaultDutch;

	private float defaultFOV;

	private bool isTakingPhoto;

	private float panMoveX;

	private float panMoveY;

	[SerializeField]
	private float floorHeight = 2f;

	[SerializeField]
	private float confinementRadius = 10f;
}
