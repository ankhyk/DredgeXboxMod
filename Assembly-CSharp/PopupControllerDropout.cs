using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InControl;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PopupControllerDropout : MonoBehaviour
{
	public async Task DisplayAsync(ControllerDropoutParams dropoutParms, bool wasBetweenScenes)
	{
		PopupControllerDropout.Open = true;
		this.cachedCanPause = GameManager.Instance.CanPause;
		this.cachedCanUnpause = GameManager.Instance.CanUnpause;
		GameManager.Instance.CanPause = false;
		GameManager.Instance.CanUnpause = false;
		this.cachedSelectedUI = EventSystem.current.currentSelectedGameObject;
		EventSystem.current.SetSelectedGameObject(null);
		this.cachedActionLayer = GameManager.Instance.Input.GetActiveActionLayer();
		GameManager.Instance.Input.SetActiveActionLayer(ActionLayer.NONE);
		ApplicationEvents.Instance.TriggerDialogToggled(true);
		this.cachedTimeScale = Time.timeScale;
		if (wasBetweenScenes)
		{
			this.cachedTimeScale = 1f;
		}
		Time.timeScale = 0f;
		InputManager.OnDeviceAttached += this.OnDeviceAttached;
		await Awaiters.SecondsRealtime(0.5f);
		bool hasGamepadDevice = false;
		do
		{
			hasGamepadDevice = PopupControllerDropout.HasDevice(dropoutParms.forceGamepad);
			await Awaiters.SecondsRealtime(0.1f);
		}
		while (!hasGamepadDevice);
		this.Hide();
	}

	public async void Hide()
	{
		InputManager.OnDeviceAttached -= this.OnDeviceAttached;
		ApplicationEvents.Instance.TriggerDialogToggled(false);
		GameManager.Instance.CanPause = this.cachedCanPause;
		GameManager.Instance.CanUnpause = this.cachedCanUnpause;
		Time.timeScale = this.cachedTimeScale;
		if (this.cachedSelectedUI != null)
		{
			EventSystem.current.SetSelectedGameObject(this.cachedSelectedUI);
		}
		else if (SceneManager.GetActiveScene().name == "Title")
		{
			GameObject gameObject = GameObject.Find("ButtonContainer");
			if (gameObject != null)
			{
				gameObject.GetComponent<ControllerFocusGrabber>().SelectSelectable();
			}
		}
		await Awaiters.NextFrame;
		GameManager.Instance.Input.SetActiveActionLayer(this.cachedActionLayer);
		global::UnityEngine.Object.Destroy(base.gameObject);
		PopupControllerDropout.Open = false;
	}

	public void OnDeviceAttached(InputDevice device)
	{
	}

	public static bool HasDevice(bool mustBeGamepad)
	{
		if (mustBeGamepad)
		{
			using (IEnumerator<InputDevice> enumerator = InputManager.ActiveDevices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.DeviceClass == InputDeviceClass.Controller)
					{
						return true;
					}
				}
			}
			return InputManager.ActiveDevices.Count > 0;
		}
		return InputManager.Devices.Count > 0;
	}

	public static bool Open;

	private bool cachedCanPause;

	private bool cachedCanUnpause;

	private ActionLayer cachedActionLayer;

	private float cachedTimeScale;

	private GameObject cachedSelectedUI;
}
