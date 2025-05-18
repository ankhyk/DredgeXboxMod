using System;
using Cinemachine;
using UnityEngine;

public class CameraSetFollowPlayer : MonoBehaviour
{
	private void Update()
	{
		if (this.virtualCamera.m_Follow == null && GameManager.Instance.Player != null)
		{
			this.virtualCamera.m_Follow = GameManager.Instance.Player.transform;
		}
	}

	[SerializeField]
	private CinemachineVirtualCamera virtualCamera;
}
