using System;
using Cinemachine;
using UnityEngine;

public class FinaleLookAtCam : MonoBehaviour
{
	private void Start()
	{
		GameManager.Instance.DialogueRunner.AddCommandHandler<bool>("ToggleLookAtFinaleVCam", new Action<bool>(this.ToggleLookAtFinaleVCam));
	}

	private void ToggleLookAtFinaleVCam(bool enable)
	{
		this.virtualCamera.enabled = enable;
	}

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
}
